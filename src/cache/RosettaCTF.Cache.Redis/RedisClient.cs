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
using System.Globalization;
using System.Linq;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RosettaCTF.Data;
using StackExchange.Redis;

namespace RosettaCTF
{
    internal sealed class RedisClient : IDisposable
    {
        private const string KeySeparator = "::";
        private const string KeyPrefix = "RosettaCTF";

        private ConnectionMultiplexer ConnectionMultiplexer { get; set; }
        private RosettaConfigurationCache Configuration { get; }
        private ILogger<RedisClient> Logger { get; }

        public RedisClient(
            IOptions<RosettaConfigurationCache> config,
            ILoggerFactory loggerFactory)
        {
            this.Configuration = config.Value;
            this.Logger = loggerFactory.CreateLogger<RedisClient>();
        }

        internal async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            var cfg = new ConfigurationOptions
            {
                AllowAdmin = false,
                ClientName = "RosettaCTF",
                Ssl = this.Configuration.UseSsl,
                Password = this.Configuration.Password,
                SslProtocols = SslProtocols.Tls12
            };

            cfg.EndPoints.Add(this.Configuration.Hostname, this.Configuration.Port);

            this.ConnectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(cfg);
            this.Logger.LogDebug("Redis connected to {0}:{1}", this.Configuration.Hostname, this.Configuration.Port);
        }

        public async Task<T> GetValueAsync<T>(params string[] keyIndices)
        {
            var db = this.ConnectionMultiplexer.GetDatabase(this.Configuration.Index);
            var key = FormatKey(keyIndices);
            var val = await db.StringGetAsync(key);

            this.Logger.LogDebug("Retrieving value '{0}'", key);
            return (T)Convert.ChangeType(val, typeof(T));
        }

        public async Task SetValueAsync<T>(T value, params string[] keyIndices)
        {
            var db = this.ConnectionMultiplexer.GetDatabase(this.Configuration.Index);
            var key = FormatKey(keyIndices);

            this.Logger.LogDebug("Setting value '{0}'", key);
            await db.StringSetAsync(key, Convert.ChangeType(value, TypeCode.String, CultureInfo.InvariantCulture) as string);
        }

        public void Dispose()
        {
            this.ConnectionMultiplexer.Dispose();
        }

        private static string FormatKey(string[] key)
        {
            var blen = key.Sum(x => x.Length) + KeySeparator.Length * key.Length + KeyPrefix.Length;
            return string.Create(blen, key, FormatInner);

            static void FormatInner(Span<char> buff, string[] indices)
            {
                var sep = KeySeparator;
                var pfx = KeyPrefix;
                var slen = sep.Length;

                pfx.AsSpan().CopyTo(buff);
                buff = buff.Slice(pfx.Length);

                for (int i = 0; i < indices.Length; ++i)
                {
                    var idc = indices[i];

                    sep.AsSpan().CopyTo(buff);
                    idc.AsSpan().CopyTo(buff = buff.Slice(slen));
                    buff = buff.Slice(idc.Length);
                }
            }
        }
    }
}
