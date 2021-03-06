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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace RosettaCTF
{
    /// <summary>
    /// Represents an initializer for database providers. An implementation configures appropriate services for supplied collection.
    /// </summary>
    public interface IDatabaseServiceInitializer
    {
        /// <summary>
        /// Configures services in the supplied collection.
        /// </summary>
        /// <param name="services">Service collection to configure.</param>
        void ConfigureServices(IServiceCollection services);

        /// <summary>
        /// Initializes services in the supplied provider.
        /// </summary>
        /// <param name="services">Service provider to initialize services in.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task encapsulating the operation.</returns>
        Task InitializeServicesAsync(IServiceProvider services, CancellationToken cancellationToken = default);
    }
}
