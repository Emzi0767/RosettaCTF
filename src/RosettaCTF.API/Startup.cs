// This file is part of RosettaCTF project.
// 
// Copyright 2020 Emzi0767
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
#if DEBUG
using Microsoft.AspNetCore.SpaServices.AngularCli;
#endif
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RosettaCTF.Data;
using RosettaCTF.Filters;

namespace RosettaCTF.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
#if DEBUG
        private SpaConfigurationAttribute SpaConfiguration { get; }
#endif

        public Startup(IConfiguration configuration)
        {
            // Configuration loading
            // Given an object structure like
            // CONFIG:
            // prop: bool
            // listen: endpoint[]
            // 
            // ENDPOINT:
            // address: string
            // port: int
            //
            // Array indexes are treated as keys
            // To set /prop via:
            // - envvars: ROSETTACTF__PROP=true (note double underscore)
            // - cmdline: --prop=false
            //
            // To set /listen/0/* via:
            // - envvars (note double underscores):
            //   ROSETTACTF__LISTEN__0__ADDRESS=0.0.0.0
            //   ROSETTACTF__LISTEN__0__PORT=5000
            // - cmdline:
            //   --listen:0:address=127.0.0.1
            //   --listen:0:port=4200
            //
            // Supported configuration sources, in order of precedence (first is lowest priority - meaning higher 
            // priority will override its values):
            // 1. appsettings.json
            // 2. appsettings.*.json (* is env, i.e. development, production, or staging)
            // 3. *.json (specified via ROSETTACTF__JSONCONFIGURATION; defaults to config.json)
            // 4. *.yml (specified via ROSETTACTF__YAMLCONFIGURATION; defaults to config.yml)
            // 5. Environment variables
            // 6. Command line

            // Load envvars and cmdline switches
            var cfg = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .AddEnvironmentVariables("ROSETTACTF__")
                .AddCommandLine(Environment.GetCommandLineArgs())
                .Build();

            // Load file config
            var cfgf = new ConfigurationBuilder()
                .AddJsonFile(cfg.GetSection("JsonConfiguration")?.Value ?? "config.json", optional: true)
                .AddJsonFile(cfg.GetSection("YamlConfiguration")?.Value ?? "config.yml", optional: true)
                .Build();

            // Prepend file config so it has lower priority
            this.Configuration = new ConfigurationBuilder()
                .AddConfiguration(cfgf)
                .AddConfiguration(cfg)
                .Build();

#if DEBUG
            this.SpaConfiguration = Assembly.GetExecutingAssembly().GetCustomAttribute<SpaConfigurationAttribute>()
                ?? throw new InvalidProgramException("Missing SPA configuration metadata.");
#endif
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHsts(opts =>
            {
                opts.Preload = true;
                opts.IncludeSubDomains = true;
                opts.MaxAge = TimeSpan.FromDays(365);
            });

            services.AddAntiforgery(x =>
            {
                x.Cookie.Name = "Rosetta-XSRF-Token";
                x.HeaderName = "X-Rosetta-XSRF";
            });

            services.AddOptions<RosettaConfigurationRoot>()
                .Bind(this.Configuration)
                .ValidateDataAnnotations();

            services.AddOptions<RosettaConfigurationDatastore>()
                .Bind(this.Configuration.GetSection("Database"))
                .ValidateDataAnnotations();

            services.AddOptions<RosettaConfigurationCache>()
                .Bind(this.Configuration.GetSection("Cache"))
                .ValidateDataAnnotations();

            services.AddOptions<RosettaConfigurationHttp>()
                .Bind(this.Configuration.GetSection("Http"))
                .ValidateDataAnnotations();

            services.AddOptions<RosettaConfigurationDiscord>()
                .Bind(this.Configuration.GetSection("Discord"))
                .ValidateDataAnnotations();


            // Configure datastore providers
            var dsiSelector = new DatastoreImplementationSelector();
            dsiSelector.ConfigureCtfConfigurationLoaderProvider("yaml", services);
            dsiSelector.ConfigureDatabaseProvider(this.Configuration["Database:Type"], services);
            dsiSelector.ConfigureCacheProvider(this.Configuration["Cache:Type"], services);

            services.AddSingleton<DiscordTokenHandler>();

            services.AddAuthentication(x =>
            {
                
            });

            services.AddAuthorization(x =>
            {
                
            });

#if !DEBUG
            services.AddControllers();
#else
            services.AddControllersWithViews();
            services.AddSpaStaticFiles(cfg => cfg.RootPath = $"{this.SpaConfiguration.Root}/dist");
#endif
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app, 
            IWebHostEnvironment env, 
            IAntiforgery xsrf)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(this.HandlePipelineException)
                    .UseStatusCodePages(this.RenderStatusCode)
                    .UseHsts();
            }

            app.UseHttpsRedirection()
#if DEBUG
                .UseStaticFiles()
#endif
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization()
                .UseMiddleware<XsrfMiddleware>(xsrf, "Rosetta-XSRF")
                .UseEndpoints(endpoints => endpoints.MapControllers());

#if DEBUG
            app.UseSpa(x =>
            {
                x.Options.SourcePath = this.SpaConfiguration.Root;
                x.UseAngularCliServer(npmScript: "start");
            });
#endif
        }

        private Task RenderStatusCode(StatusCodeContext ctx)
            => this.RunHandlerAsync(ctx.HttpContext);

        private void HandlePipelineException(IApplicationBuilder app)
            => app.Run(this.RunHandlerAsync);

        private Task RunHandlerAsync(HttpContext ctx)
        {
            ctx.Response.ContentType = "application/json";

            ApiError error;
            var exhpf = ctx.Features.Get<IExceptionHandlerPathFeature>();
            if (exhpf?.Error != null)
            {
                ctx.Response.StatusCode = 500;

                error = new ApiError(ApiErrorCode.GenericError, "Internal server error occured while processing the request");
            }
            else
            {
                error = new ApiError(ApiErrorCode.GenericError, $"HTTP Error {ctx.Response.StatusCode}");
            }

            using (var utf8json = new Utf8JsonWriter(ctx.Response.Body))
                JsonSerializer.Serialize(ApiResult.FromError<object>(error));

            return Task.CompletedTask;
        }
    }
}
