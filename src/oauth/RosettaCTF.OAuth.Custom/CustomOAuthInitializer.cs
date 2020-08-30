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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RosettaCTF;
using RosettaCTF.Attributes;

[assembly: OAuthProvider(CustomOAuthProvider.ProviderType, typeof(CustomOAuthInitializer))]

namespace RosettaCTF
{
    internal sealed class CustomOAuthInitializer : IOAuthProviderServiceInitializer
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<CustomOAuthProvider>();
        }

        public Task InitializeServicesAsync(IServiceProvider services, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}
