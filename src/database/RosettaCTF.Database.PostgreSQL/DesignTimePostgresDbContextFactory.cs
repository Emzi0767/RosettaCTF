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
using System.IO;
using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using RosettaCTF.Data;

namespace RosettaCTF
{
    internal sealed class DesignTimePostgresDbContextFactory : IDesignTimeDbContextFactory<PostgresDbContext>
    {
        public PostgresDbContext CreateDbContext(string[] args)
        {
            var configFile = new FileInfo("../../RosettaCTF.API/config.json");
            ConfigurationRoot config;
            using (var fs = configFile.OpenRead())
            {
                Span<byte> json = stackalloc byte[(int)fs.Length];
                fs.Read(json);

                config = JsonSerializer.Deserialize<ConfigurationRoot>(json, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            }

            var pcp = new PostgresConfigurationProvider(config.Database);
            var dbopts = new DbContextOptionsBuilder<PostgresDbContext>();
            dbopts.UseNpgsql(pcp.ConnectionString,
                pgopts => pgopts.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name)
                    .MigrationsHistoryTable("efcore_migrations"));

            return new PostgresDbContext(dbopts.Options);
        }
    }
}
