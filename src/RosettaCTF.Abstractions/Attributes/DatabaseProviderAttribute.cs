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

namespace RosettaCTF.Attributes
{
    /// <summary>
    /// Designates the database provider implementation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public sealed class DatabaseProviderAttribute : Attribute
    {
        /// <summary>
        /// Gets the ID of this implementation.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the type of the service collection initializer.
        /// </summary>
        public Type InitializerType { get; }

        /// <summary>
        /// Designates this database provider implementation.
        /// </summary>
        /// <param name="id">ID of this implementation.</param>
        /// <param name="initializer">Tyep of service collection initializer.</param>
        public DatabaseProviderAttribute(string id, Type initializer)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            if (initializer == null)
                throw new ArgumentNullException(nameof(initializer));

            if (!typeof(IDatabaseServiceInitializer).IsAssignableFrom(initializer) || !initializer.IsClass || initializer.IsAbstract)
                throw new ArgumentException("Supplied initializer type must be a non-abstract, non-static class, implementing IDatabaseServiceInitializer interface.", nameof(initializer));

            this.Id = id;
            this.InitializerType = initializer;
        }

        /// <summary>
        /// Gets an instance of service initializer for this provider.
        /// </summary>
        /// <returns>An <see cref="IDatabaseServiceInitializer"/> instance.</returns>
        public IDatabaseServiceInitializer GetServiceInitializer()
            => Activator.CreateInstance(this.InitializerType) as IDatabaseServiceInitializer;
    }
}
