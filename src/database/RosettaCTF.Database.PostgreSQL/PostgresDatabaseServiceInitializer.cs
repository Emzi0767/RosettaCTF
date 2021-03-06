﻿// This file is part of RosettaCTF project.
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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using RosettaCTF;
using RosettaCTF.Attributes;
using RosettaCTF.Data;

[assembly: DatabaseProvider("postgresql", typeof(PostgresDatabaseServiceInitializer))]

namespace RosettaCTF
{
    internal sealed class PostgresDatabaseServiceInitializer : IDatabaseServiceInitializer
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<PostgresConfigurationProvider>();
            services.AddDbContext<PostgresDbContext>((srv, opts) =>
            {
                var cfg = srv.GetRequiredService<PostgresConfigurationProvider>();
                opts.UseNpgsql(cfg.ConnectionString,
                    pgopts => pgopts.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name)
                        .MigrationsHistoryTable("efcore_migrations"));
            }, contextLifetime: ServiceLifetime.Scoped, optionsLifetime: ServiceLifetime.Singleton);
            services.AddScoped<IUserRepository, PostgresUserRepository>();
            services.AddScoped<ICtfChallengeRepository, PostgresChallengeRepository>();
            services.AddScoped<IMfaRepository, PostgresMfaRepository>();
        }

        public async Task InitializeServicesAsync(IServiceProvider services, CancellationToken cancellationToken = default)
        {
            using (var scope = services.CreateScope())
            {
                var srvs = scope.ServiceProvider;
                using (var db = srvs.GetRequiredService<PostgresDbContext>())
                {
                    await db.Database.MigrateAsync(cancellationToken);

                    using (var conn = db.Database.GetDbConnection() as NpgsqlConnection)
                    {
                        await conn.OpenAsync(cancellationToken);
                        conn.ReloadTypes();
                        await conn.CloseAsync();
                    }
                }
            }
        }
    }
}
