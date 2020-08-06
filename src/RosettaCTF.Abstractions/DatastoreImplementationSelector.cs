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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using RosettaCTF.Attributes;

namespace RosettaCTF
{
    /// <summary>
    /// Provides selector mechanisms for selecting and initializing datastore implementations.
    /// </summary>
    public sealed class DatastoreImplementationSelector
    {
        private readonly IReadOnlyDictionary<string, DatabaseProviderAttribute> _databaseProviders;
        private readonly IReadOnlyDictionary<string, CacheProviderAttribute> _cacheProviders;
        private readonly IReadOnlyDictionary<string, CtfConfigurationLoaderProviderAttribute> _configurationLoaderProviders;

        /// <summary>
        /// Initializes the selector.
        /// </summary>
        public DatastoreImplementationSelector()
        {
            var implAssemblies = AbstractionUtilities.AssemblyCache
                .Select(x => new 
                    { 
                        assembly = x, 
                        database = x.GetCustomAttribute<DatabaseProviderAttribute>(), 
                        cache = x.GetCustomAttribute<CacheProviderAttribute>(),
                        configurationLoader = x.GetCustomAttribute<CtfConfigurationLoaderProviderAttribute>()
                    })
                .Where(x => x.database != null || x.cache != null || x.configurationLoader != null)
                .ToList();

            this._databaseProviders = implAssemblies.Where(x => x.database != null)
                .Select(x => x.database)
                .ToDictionary(x => x.Id, x => x, StringComparer.InvariantCultureIgnoreCase);

            this._cacheProviders = implAssemblies.Where(x => x.cache != null)
                .Select(x => x.cache)
                .ToDictionary(x => x.Id, x => x, StringComparer.InvariantCultureIgnoreCase);

            this._configurationLoaderProviders = implAssemblies.Where(x => x.configurationLoader != null)
                .Select(x => x.configurationLoader)
                .ToDictionary(x => x.Id, x => x, StringComparer.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Initializes and configures the selected database provider by registering appropriate services in the DB collection.
        /// </summary>
        /// <param name="id">Id of the database implementation provider.</param>
        /// <param name="services">Service collection to register appropriate services in.</param>
        public void ConfigureDatabaseProvider(string id, IServiceCollection services)
        {
            if (this._databaseProviders.TryGetValue(id, out var dpa))
                dpa.GetServiceInitializer().ConfigureServices(services);
        }

        /// <summary>
        /// Initializes and configures the selected cache provider by registering appropriate services in the DB collection.
        /// </summary>
        /// <param name="id">Id of the cache implementation provider.</param>
        /// <param name="services">Service collection to register appropriate services in.</param>
        public void ConfigureCacheProvider(string id, IServiceCollection services)
        {
            if (this._cacheProviders.TryGetValue(id, out var cpa))
                cpa.GetServiceInitializer().ConfigureServices(services);
        }

        /// <summary>
        /// Initializes and configures the selected CTF configuration loader provider by registering appropriate services in the DB collection.
        /// </summary>
        /// <param name="id">Id of the CTF configuration loader implementation provider.</param>
        /// <param name="services">Service collection to register appropriate services in.</param>
        public void ConfigureCtfConfigurationLoaderProvider(string id, IServiceCollection services)
        {
            if (this._configurationLoaderProviders.TryGetValue(id, out var clpa))
                clpa.GetServiceInitializer().ConfigureServices(services);
        }
    }
}
