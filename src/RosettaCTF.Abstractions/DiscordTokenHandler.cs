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
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Emzi0767;
using Microsoft.Extensions.Options;
using RosettaCTF.Data;

namespace RosettaCTF
{
    /// <summary>
    /// Handles encryption and decryption of Discord tokens for the purpose of storage.
    /// </summary>
    public sealed class DiscordTokenHandler
    {
        private Argon2idKeyDeriver KeyDeriver { get; }

        private byte[] Key { get; }

        /// <summary>
        /// Instantiates the handler and configures the encryption key.
        /// </summary>
        /// <param name="cfg">Configuration for the handler.</param>
        public DiscordTokenHandler(
            IOptions<RosettaConfigurationDiscord> cfg,
            Argon2idKeyDeriver keyDeriver)
        {
            this.KeyDeriver = keyDeriver;
            this.Key = AbstractionUtilities.UTF8.GetBytes(cfg.Value.TokenKey);
        }

        /// <summary>
        /// Encrypts a value.
        /// </summary>
        /// <param name="value">Value to encrypt.</param>
        /// <returns>Encrypted value as a base64 string.</returns>
        public async Task<string> EncryptAsync(string value)
        {
            using var rng = new SecureRandom();

            var v = AbstractionUtilities.UTF8.GetBytes(value);
            var s = new byte[16];
            rng.GetBytes(s);

            using (var aes = new RijndaelManaged())
            {
                var bsize = aes.BlockSize / 8;
                var osize = (v.Length / bsize + 1) * bsize;

                var iv = new byte[bsize];
                rng.GetBytes(iv);

                aes.Key = await this.DeriveKeyAsync(s);
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var enc = aes.CreateEncryptor())
                using (var ms = new MemoryStream(osize + s.Length + aes.IV.Length))
                using (var cs = new CryptoStream(ms, enc, CryptoStreamMode.Write))
                {
                    ms.Write(s);
                    ms.Write(iv);

                    cs.Write(v);
                    cs.FlushFinalBlock();

                    return Convert.ToBase64String(ms.GetBuffer().AsSpan(), Base64FormattingOptions.None);
                }
            }
        }

        /// <summary>
        /// Decrypts a value.
        /// </summary>
        /// <param name="value">Base64 string containing the value to decrypt.</param>
        /// <returns>Decrypted string.</returns>
        public async Task<string> DecryptAsync(string value)
        {
            using var ms = new MemoryStream(Convert.FromBase64String(value));

            var s = new byte[16];
            ms.Read(s);
            
            using (var aes = new RijndaelManaged())
            {
                var bsize = aes.BlockSize / 8;
                var output = new byte[(int)ms.Length - s.Length - aes.IV.Length];
                var olen = 0;

                var iv = new byte[bsize];
                ms.Read(iv);

                aes.Key = await this.DeriveKeyAsync(s);
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var dec = aes.CreateDecryptor())
                using (var cs = new CryptoStream(ms, dec, CryptoStreamMode.Read))
                    olen = cs.Read(output);

                return AbstractionUtilities.UTF8.GetString(output.AsSpan().Slice(0, olen));
            }
        }

        private async Task<byte[]> DeriveKeyAsync(byte[] salt)
            => await this.KeyDeriver.DeriveKeyAsync(this.Key, salt, 256 / 8);
    }
}
