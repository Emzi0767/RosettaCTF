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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RosettaCTF.Data
{
    /// <summary>
    /// Converts instances of <see cref="Enum"/> to their respective <see cref="string"/> representations, as defined by <see cref="EnumDisplayNameAttribute"/>.
    /// </summary>
    public sealed class EnumDisplayConverter
    {
        private readonly IDictionary<Type, NameCache> _typeNameCaches;

        /// <summary>
        /// Initializes the converter.
        /// </summary>
        public EnumDisplayConverter()
        {
            this._typeNameCaches = new ConcurrentDictionary<Type, NameCache>();
        }

        /// <summary>
        /// Converts a supplied enum value of type <typeparamref name="T"/> to its string representation, as defined by <see cref="EnumDisplayNameAttribute"/>.
        /// </summary>
        /// <typeparam name="T">Type of the enum value.</typeparam>
        /// <param name="enum">Value to convert.</param>
        /// <returns>String representation or default string conversion, if a representation is not defined.</returns>
        public string Convert<T>(T @enum) where T : Enum
        {
            var t = typeof(T);
            if (this._typeNameCaches.TryGetValue(t, out var nc))
                return (nc as NameCache<T>).Convert(@enum);

            var nct = NameCache.CreateForType<T>();
            this._typeNameCaches.TryAdd(t, nct);
            return nct.Convert(@enum);
        }

        private abstract class NameCache
        {
            public static NameCache<T> CreateForType<T>() where T : Enum
                => NameCache<T>.Create();
        }

        private sealed class NameCache<T> : NameCache where T : Enum
        {
            private readonly IReadOnlyDictionary<T, string> _lookup;

            private NameCache(IReadOnlyDictionary<T, string> lookup)
            {
                this._lookup = lookup;
            }

            public string Convert(T @enum)
                => this._lookup.GetValueOrDefault(@enum) ?? @enum.ToString();

            public static NameCache<T> Create()
            {
                var lookup = typeof(T)
                    .GetFields(BindingFlags.Public | BindingFlags.Static)
                    .Select(x => new { Field = x, Value = (T)x.GetValue(null), Display = x.GetCustomAttribute<EnumDisplayNameAttribute>() })
                    .Where(x => x.Display != null)
                    .ToDictionary(x => x.Value, x => x.Display.DisplayName ?? x.Value.ToString());

                return new NameCache<T>(lookup);
            }
        }
    }
}
