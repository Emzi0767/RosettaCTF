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
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RosettaCTF.Attributes;

namespace RosettaCTF
{
    /// <summary>
    /// Provides selector mechanisms for selecting and initializing datastore implementations.
    /// </summary>
    public sealed class ImplementationSelector
    {
        private readonly IReadOnlyDictionary<string, DatabaseProviderAttribute> _databaseProviders;
        private readonly IReadOnlyDictionary<string, CacheProviderAttribute> _cacheProviders;
        private readonly IReadOnlyDictionary<string, CtfConfigurationLoaderProviderAttribute> _configurationLoaderProviders;
        private readonly IReadOnlyDictionary<string, OAuthProviderAttribute> _oauthProviders;

        /// <summary>
        /// Initializes the selector.
        /// </summary>
        public ImplementationSelector()
        {
            var implAssemblies = AbstractionUtilities.AssemblyCache
                .Select(x => new 
                    { 
                        assembly = x, 
                        database = x.GetCustomAttribute<DatabaseProviderAttribute>(), 
                        cache = x.GetCustomAttribute<CacheProviderAttribute>(),
                        configurationLoader = x.GetCustomAttribute<CtfConfigurationLoaderProviderAttribute>(),
                        oauth = x.GetCustomAttribute<OAuthProviderAttribute>()
                    })
                .Where(x => x.database != null || x.cache != null || x.configurationLoader != null || x.oauth != null)
                .ToList();

            this._databaseProviders = implAssemblies.Where(x => x.database != null)
                .Select(x => x.database)
                .ToDictionary(x => x.Id, StringComparer.InvariantCultureIgnoreCase);

            this._cacheProviders = implAssemblies.Where(x => x.cache != null)
                .Select(x => x.cache)
                .ToDictionary(x => x.Id, StringComparer.InvariantCultureIgnoreCase);

            this._configurationLoaderProviders = implAssemblies.Where(x => x.configurationLoader != null)
                .Select(x => x.configurationLoader)
                .ToDictionary(x => x.Id, StringComparer.CurrentCultureIgnoreCase);

            this._oauthProviders = implAssemblies.Where(x => x.oauth != null)
                .Select(x => x.oauth)
                .ToDictionary(x => x.Id, StringComparer.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Initializes and configures the selected database provider by registering appropriate services in the DB collection.
        /// </summary>
        /// <param name="id">Id of the database implementation provider.</param>
        /// <param name="services">Service collection to register appropriate services in.</param>
        public void ConfigureDatabaseProvider(string id, IServiceCollection services)
        {
            if (!this._databaseProviders.TryGetValue(id, out var dpa))
                throw new MissingProviderException($"db:{id}");

            dpa.GetServiceInitializer().ConfigureServices(services);
        }

        /// <summary>
        /// Performs necessary service initialization tasks for the database provider.
        /// </summary>
        /// <param name="id">Id of the database implementation provider.</param>
        /// <param name="services">Service provider containing services to initialize.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task encapsulating the operation.</returns>
        public async Task InitializeDatabaseProviderAsync(string id, IServiceProvider services, CancellationToken cancellationToken = default)
        {
            if (!this._databaseProviders.TryGetValue(id, out var dpa))
                throw new MissingProviderException($"db:{id}");

            await dpa.GetServiceInitializer().InitializeServicesAsync(services, cancellationToken);
        }

        /// <summary>
        /// Initializes and configures the selected cache provider by registering appropriate services in the DB collection.
        /// </summary>
        /// <param name="id">Id of the cache implementation provider.</param>
        /// <param name="services">Service collection to register appropriate services in.</param>
        public void ConfigureCacheProvider(string id, IServiceCollection services)
        {
            if (!this._cacheProviders.TryGetValue(id, out var cpa))
                throw new MissingProviderException($"cache:{id}");

            cpa.GetServiceInitializer().ConfigureServices(services);
        }

        /// <summary>
        /// Performs necessary service initialization tasks for the cache provider.
        /// </summary>
        /// <param name="id">Id of the cache implementation provider.</param>
        /// <param name="services">Service provider containing services to initialize.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task encapsulating the operation.</returns>
        public async Task InitializeCacheProviderAsync(string id, IServiceProvider services, CancellationToken cancellationToken = default)
        {
            if (!this._cacheProviders.TryGetValue(id, out var cpa))
                throw new MissingProviderException($"cache:{id}");

            await cpa.GetServiceInitializer().InitializeServicesAsync(services, cancellationToken);
        }

        /// <summary>
        /// Initializes and configures the selected CTF configuration loader provider by registering appropriate services in the DB collection.
        /// </summary>
        /// <param name="id">Id of the CTF configuration loader implementation provider.</param>
        /// <param name="services">Service collection to register appropriate services in.</param>
        public void ConfigureCtfConfigurationLoaderProvider(string id, IServiceCollection services)
        {
            if (!this._configurationLoaderProviders.TryGetValue(id, out var clpa))
                throw new MissingProviderException($"config:{id}");

            clpa.GetServiceInitializer().ConfigureServices(services);
        }

        /// <summary>
        /// Performs necessary service initialization tasks for the CTF configuration loader provider.
        /// </summary>
        /// <param name="id">Id of the CTF configuration loader implementation provider.</param>
        /// <param name="services">Service provider containing services to initialize.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task encapsulating the operation.</returns>
        public async Task InitializeCtfConfigurationLoaderProviderAsync(string id, IServiceProvider services, CancellationToken cancellationToken = default)
        {
            if (!this._configurationLoaderProviders.TryGetValue(id, out var clpa))
                throw new MissingProviderException($"config:{id}");

            await clpa.GetServiceInitializer().InitializeServicesAsync(services, cancellationToken);
        }

        /// <summary>
        /// Initializes and configures selected installed OAuth providers.
        /// </summary>
        /// <param name="providers">Ids of the OAuth provider implementations.</param>
        /// <param name="services">Service collection to register appropriate services in.</param>
        public void ConfigureOAuthProviders(IEnumerable<string> providers, IServiceCollection services)
        {
            foreach (var id in providers)
            {
                if (!this._oauthProviders.TryGetValue(id, out var oapa))
                    throw new MissingProviderException($"oauth:{id}");

                oapa.GetServiceInitializer().ConfigureServices(services);
            }
        }

        /// <summary>
        /// Performs necessary service initialization tasks for specified OAuth providers.
        /// </summary>
        /// <param name="providers">Ids of the OAuth provider implementations.</param>
        /// <param name="services">Service provider containing services to initialize.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task encapsulating the operation.</returns>
        public async Task InitializeOAuthProvidersAsync(IEnumerable<string> providers, IServiceProvider services, CancellationToken cancellationToken = default)
        {
            foreach (var id in providers)
            {
                if (!this._oauthProviders.TryGetValue(id, out var oapa))
                    throw new MissingProviderException($"oauth:{id}");

                await oapa.GetServiceInitializer().InitializeServicesAsync(services, cancellationToken);
            }
        }
    }
}
