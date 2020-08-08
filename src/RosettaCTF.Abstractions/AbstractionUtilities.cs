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
using System.Globalization;
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

        /// <summary>
        /// Gets the directory the current assembly resides in.
        /// </summary>
        private static DirectoryInfo CurrentDirectory { get; }

        /// <summary>
        /// Gets loadable assemblies from assembly's directory.
        /// </summary>
        private static IEnumerable<FileInfo> LoadableAssemblies { get; }

        static AbstractionUtilities()
        {
            ForceLoadAssemblies();
            AssemblyCache = AppDomain.CurrentDomain.GetAssemblies();
            CurrentDirectory = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            LoadableAssemblies = CurrentDirectory.GetFiles("*.dll", SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// Parses a numeric string as long.
        /// </summary>
        /// <param name="str">String to parse.</param>
        /// <returns>Parsed long.</returns>
        public static long ParseAsLong(this string str)
            => long.Parse(str, NumberStyles.Integer, CultureInfo.InvariantCulture);

        /// <summary>
        /// Parses a numeric string as ulong.
        /// </summary>
        /// <param name="str">String to parse.</param>
        /// <returns>Parsed string.</returns>
        public static ulong ParseAsUlong(this string str)
            => ulong.Parse(str, NumberStyles.Integer, CultureInfo.InvariantCulture);

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
                var asnst = new string(asn);
                if (!asns.Contains(asnst))
                    Assembly.Load(asnst);
            }

            // So here's a great question. Why do I need this? I don't know. Without it, .NET just gives up on 
            // assembly loading altogether. Amazing software.
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs ea)
        {
            var an = new AssemblyName(ea.Name);
            var dll = string.Create(an.Name.Length + 4, an.Name, CreateDllString);
            return LoadableAssemblies.Any(x => x.Name == dll)
                ? Assembly.LoadFrom(Path.Combine(CurrentDirectory.FullName, dll))
                : null;

            static void CreateDllString(Span<char> buffer, string state)
            {
                buffer[^1] = 'l';
                buffer[^2] = 'l';
                buffer[^3] = 'd';
                buffer[^4] = '.';
                state.AsSpan().CopyTo(buffer);
            }
        }
    }
}
