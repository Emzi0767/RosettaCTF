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
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Humanizer;
using Humanizer.Localisation;
using RosettaCTF.Data;

namespace RosettaCTF
{
    /// <summary>
    /// Various static helpers.
    /// </summary>
    public static class AbstractionUtilities
    {
        /// <summary>
        /// Gets the regex pattern used to validate usernames and team names.
        /// </summary>
        public const string NameRegexPattern = @"^[a-zA-Z0-9!@#$%^&*()\-_+=;:'""\\|,<.>\/?€~` ]{2,48}$";

        private static HashSet<string> AntiforgeryTypeNames { get; }

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

        /// <summary>
        /// Gets default JSON serializer options.
        /// </summary>
        public static JsonSerializerOptions DefaultJsonOptions { get; } = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        /// <summary>
        /// Gets the snake_case JSON naming serializer options.
        /// </summary>
        public static JsonSerializerOptions SnakeCaseJsonOptions { get; } = new JsonSerializerOptions
        {
            PropertyNamingPolicy = new SnakeCaseNamingPolicy()
        };

        /// <summary>
        /// Gets the regex used to validate usernames and team names.
        /// </summary>
        public static Regex NameRegex { get; } = new Regex(NameRegexPattern, RegexOptions.Compiled | RegexOptions.ECMAScript);

        static AbstractionUtilities()
        {
            ForceLoadAssemblies();
            AssemblyCache = AppDomain.CurrentDomain.GetAssemblies();
            CurrentDirectory = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            LoadableAssemblies = CurrentDirectory.GetFiles("*.dll", SearchOption.TopDirectoryOnly);

            AntiforgeryTypeNames = new HashSet<string>(2)
            {
                "ValidateAntiforgeryTokenAuthorizationFilter",
                "AutoValidateAntiforgeryTokenAuthorizationFilter"
            };
        }

        /// <summary>
        /// Parses a numeric string as long.
        /// </summary>
        /// <param name="str">String to parse.</param>
        /// <returns>Parsed long.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ParseAsLong(this string str)
            => long.Parse(str, NumberStyles.Integer, CultureInfo.InvariantCulture);

        /// <summary>
        /// Converts a long to a string.
        /// </summary>
        /// <param name="l">Long to convert.</param>
        /// <returns>Converted long.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string AsString(this long l)
            => l.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Parses a numeric string as ulong.
        /// </summary>
        /// <param name="str">String to parse.</param>
        /// <returns>Parsed string.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ParseAsUlong(this string str)
            => ulong.Parse(str, NumberStyles.Integer, CultureInfo.InvariantCulture);

        /// <summary>
        /// Converts a ulong to a string.
        /// </summary>
        /// <param name="l">Ulong to convert.</param>
        /// <returns>Converted ulong.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string AsString(this ulong l)
            => l.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Parses a numeric string as int.
        /// </summary>
        /// <param name="str">String to parse.</param>
        /// <returns>Parsed string.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ParseAsInt(this ReadOnlySpan<char> str)
            => int.Parse(str, NumberStyles.Integer, CultureInfo.InvariantCulture);

        /// <summary>
        /// Converts a timespan to a human-readable string.
        /// </summary>
        /// <param name="timeSpan">Timespan to humanize.</param>
        /// <returns>Humanized string.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToHumanString(this TimeSpan timeSpan)
            => timeSpan.Humanize(3, CultureInfo.InvariantCulture, maxUnit: TimeUnit.Hour, minUnit: TimeUnit.Second);

        /// <summary>
        /// Finds the types of ASP.NET Core CRSF Antiforgery filters and returns them.
        /// </summary>
        /// <returns>Found types.</returns>
        public static IEnumerable<Type> FindAntiforgeryFilters()
            => AssemblyCache.SelectMany(x => x.DefinedTypes)
                .Where(x => AntiforgeryTypeNames.Contains(x.Name));

        /// <summary>
        /// Clamps a value to a specific range.
        /// </summary>
        /// <param name="d">Value to clamp.</param>
        /// <param name="max">Maximum value.</param>
        /// <param name="min">Minimum value.</param>
        /// <returns>Clamped value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Clamp(this double d, double min, double max)
            => double.IsNaN(d)
                ? Math.Max(0, min)
                : Math.Max(Math.Min(d, max), min);

        /// <summary>
        /// Performs a constant-time comparisons of 2 values.
        /// </summary>
        /// <param name="input">First value.</param>
        /// <param name="known">Second value.</param>
        /// <param name="expectedLength">Expected length of input data.</param>
        /// <returns>Whether the two values are equal.</returns>
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public static bool ConstantTimeEquals(this ReadOnlySpan<byte> input, ReadOnlySpan<byte> known, int expectedLength)
        {
            // https://github.com/jbtule/keyczar-dotnet/blob/aac6e90ffa2cee3bf1e5bf9e3b8c7caf077b6343/Keyczar/Keyczar/Util/Secure.cs#L98-L148
            // https://github.com/sdrapkin/SecurityDriven.Inferno/blob/cfba069191247c8e24b096fd0f2dd899b5a25747/Utils.cs

            // fill dummy array for comparisons shoud lengths mismatch
            ReadOnlySpan<byte> ind = stackalloc byte[2] { 0x01, 0xFE };

            // count differences
            var diffBits = 0;
            var len = Math.Max(input.Length, known.Length);
            for (var i = 0; i < expectedLength; i++)
            {
                if (i >= len)
                    diffBits |= ind[0] ^ ind[1];
                else
                    diffBits |= input[i] ^ known[i];
            }

            return input.Length == known.Length & input.Length == expectedLength & diffBits == 0;
        }

        /// <summary>
        /// Performs a vectorized comparison of 2 byte sequences.
        /// </summary>
        /// <param name="left">First value.</param>
        /// <param name="right">Second value.</param>
        /// <returns>Whether the values are equal.</returns>
        public static bool CompareVectorized(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right)
        {
            if (left.Length != right.Length)
                return false;

            if (Avx.IsSupported && left.Length >= 32)
            {
                var leftover = left.Length % 32;
                var lenlimit = left.Length - leftover;
                if (leftover == 0)
                    return CompareAvx(left, right);
                else
                    return CompareAvx(left.Slice(0, lenlimit), right.Slice(0, lenlimit))
                        && CompareRegular(left.Slice(lenlimit), right.Slice(lenlimit));
            }

            if (Sse2.IsSupported && Sse41.IsSupported && left.Length >= 16)
            {
                var leftover = left.Length % 16;
                var lenlimit = left.Length - leftover;
                if (leftover == 0)
                    return CompareSse(left, right);
                else
                    return CompareSse(left.Slice(0, lenlimit), right.Slice(0, lenlimit))
                        && CompareRegular(left.Slice(lenlimit), right.Slice(lenlimit));
            }

            return CompareRegular(left, right);
        }

        private static void ForceLoadAssemblies()
        {
            var asns = new HashSet<string>(AppDomain.CurrentDomain
                .GetAssemblies()
                .Select(x => x.GetName().Name));

            var a = Assembly.GetEntryAssembly();
            var loc = Path.GetFullPath(a.Location);
            var dir = Path.GetDirectoryName(loc.AsSpan());
            var assemblies = Directory.GetFiles(new string(dir), "RosettaCTF.*.dll");

            foreach (var assembly in a.GetReferencedAssemblies())
                Assembly.Load(assembly);

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

        private static bool CompareRegular(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right)
            => left.SequenceEqual(right);

        private static unsafe bool CompareSse(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right)
        {
            var len = left.Length;

            fixed (byte* lptr = &left.GetPinnableReference())
            fixed (byte* rptr = &right.GetPinnableReference())
            {
                for (var i = 0; i < len; i += 16)
                {
                    var v128_0 = Sse2.LoadVector128(lptr + i);
                    var v128_1 = Sse2.LoadVector128(rptr + i);
                    if (!Sse41.TestC(v128_0, v128_1))
                        return false;
                }
            }

            return true;
        }

        private static unsafe bool CompareAvx(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right)
        {
            var len = left.Length;

            fixed (byte* lptr = &left.GetPinnableReference())
            fixed (byte* rptr = &right.GetPinnableReference())
            {
                for (var i = 0; i < len; i += 32)
                {
                    var v256_0 = Avx.LoadVector256(lptr + i);
                    var v256_1 = Avx.LoadVector256(rptr + i);
                    if (!Avx.TestC(v256_0, v256_1))
                        return false;
                }
            }

            return true;
        }
    }
}
