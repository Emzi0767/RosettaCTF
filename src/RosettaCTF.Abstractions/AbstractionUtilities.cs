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
using System.Text;

namespace RosettaCTF
{
    /// <summary>
    /// Various static helpers.
    /// </summary>
    public static class AbstractionUtilities
    {
        /// <summary>
        /// Gets the common, properly-configured instance of the UTF-8 encoder.
        /// </summary>
        public static Encoding UTF8 { get; } = new UTF8Encoding(false);

        /// <summary>
        /// Contains an enumerable of currently-loaded assemblies.
        /// </summary>
        internal static IEnumerable<Assembly> AssemblyCache { get; }

        static AbstractionUtilities()
        {
            ForceLoadAssemblies();
            AssemblyCache = AppDomain.CurrentDomain.GetAssemblies();
        }

        private static void ForceLoadAssemblies()
        {
            var asns = new HashSet<string>(AppDomain.CurrentDomain
                .GetAssemblies()
                .Select(x => x.GetName().Name));

            var a = Assembly.GetEntryAssembly();
            var loc = Path.GetFullPath(a.Location);
            var dir = Path.GetDirectoryName(loc.AsSpan());
            var assemblies = Directory.GetFiles(new string(dir), "RosettaCTF.**.dll");

            foreach (var assembly in assemblies)
            {
                var asn = Path.GetFileNameWithoutExtension(assembly.AsSpan());
                if (!asns.Contains(new string(asn)))
                    Assembly.LoadFile(assembly);
            }
        }
    }
}
