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
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RosettaCTF.Data.Configuration;

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
            // Load envvars and cmdline switches
            var cfg = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .AddEnvironmentVariables("ROSETTACTF__")
                .AddCommandLine(Environment.GetCommandLineArgs())
                .Build();

            // Load file config
            var cfgf = new ConfigurationBuilder()
                .AddJsonFile(cfg.GetSection("JsonConfiguration")?.Value ?? "config.json", true)
                .AddJsonFile(cfg.GetSection("YamlConfiguration")?.Value ?? "config.yaml", true)
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

#if !DEBUG
            services.AddControllers();
#else
            services.AddControllersWithViews();
            services.AddSpaStaticFiles(cfg => cfg.RootPath = $"{this.SpaConfiguration.Root}/dist");
#endif
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();

            app.UseHttpsRedirection()
#if DEBUG
                .UseStaticFiles()
#endif
                .UseRouting()
                .UseEndpoints(endpoints => endpoints.MapControllers());

#if DEBUG
            app.UseSpa(x =>
            {
                x.Options.SourcePath = this.SpaConfiguration.Root;
                x.UseAngularCliServer(npmScript: "start");
            });
#endif
        }
    }
}
