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
using System.Security.Cryptography;
using Emzi0767;
using Emzi0767.Utilities;
using Microsoft.Extensions.Options;
using RosettaCTF.Data;

namespace RosettaCTF
{
    /// <summary>
    /// Performs signing and validation operations on action tokens.
    /// </summary>
    public sealed class ActionTokenPairHandler
    {
        private const int SignatureSize = 256 / 8;
        private const int RsaSize = 2048;

        private ConfigurationAuthentication Configuration { get; }
        private byte[] SigningKey { get; }

        /// <summary>
        /// Creates a new token handler.
        /// </summary>
        /// <param name="cfg">Configuration settings.</param>
        /// <param name="keyDeriver">Encryption key deriver.</param>
        public ActionTokenPairHandler(
            IOptions<ConfigurationAuthentication> cfg,
            IKeyDeriver keyDeriver)
        {
            this.Configuration = cfg.Value;

            var keyBase = AbstractionUtilities.UTF8.GetBytes(this.Configuration.TokenKey);
            if (keyBase.Length < 8)
                throw new ArgumentException("Token key must be at least 8 bytes-long.", nameof(cfg));

            var saltBytes = new byte[96];
            using (var rng = new SecureRandom())
                rng.GetBytes(saltBytes);

            var async = new AsyncExecutor();
            this.SigningKey = async.Execute(keyDeriver.DeriveKeyAsync(
                value: keyBase,
                salt: saltBytes,
                byteCount: 256 / 8));
        }

        /// <summary>
        /// Issues a new token pair.
        /// </summary>
        /// <param name="actionId">Action for which the token pair is issued.</param>
        /// <returns>Issued token pair or null if issuing fails.</returns>
        public ActionTokenPair IssueTokenPair(string actionId)
        {
            var state = Guid.NewGuid().ToByteArray();

            byte[] kclient, kserver, sigclient = new byte[SignatureSize], sigserver = new byte[SignatureSize];
            using (var rsa = RSA.Create(RsaSize))
            {
                kclient = rsa.ExportRSAPublicKey();
                kserver = rsa.ExportRSAPrivateKey();
            }

            if (!this.GenerateSignatures(actionId, state, kclient, kserver, sigclient, sigserver))
                return null;
            
            var tkclient = new ActionToken(true, kclient, state, sigclient);
            var tkserver = new ActionToken(false, kserver, state, sigserver);
            return new ActionTokenPair(tkclient, tkserver);
        }

        /// <summary>
        /// Validates a token pair against a given action.
        /// </summary>
        /// <param name="tokenPair">Token pair to validate.</param>
        /// <param name="actionId">ID of the action to validate.</param>
        /// <returns>Whether the token pair is valid.</returns>
        public bool ValidateTokenPair(ActionTokenPair tokenPair, string actionId)
        {
            if (tokenPair == null ||
                tokenPair.Client == null ||
                tokenPair.Server == null ||
                !tokenPair.Client.IsClientPart ||
                tokenPair.Server.IsClientPart)
                return false;

            var aclen = AbstractionUtilities.UTF8.GetByteCount(actionId);
            var tkc = tokenPair.Client;
            var tks = tokenPair.Server;

            if (!AbstractionUtilities.CompareVectorized(tkc.State, tks.State))
                return false;

            var state = tkc.State;
            Span<byte> sigclient = stackalloc byte[SignatureSize];
            Span<byte> sigserver = stackalloc byte[SignatureSize];

            if (!this.GenerateSignatures(actionId, state, tkc.Key, tks.Key, sigclient, sigserver))
                return false;

            if (!AbstractionUtilities.CompareVectorized(sigclient, tkc.Signature) ||
                !AbstractionUtilities.CompareVectorized(sigserver, tks.Signature))
                return false;

            // round-trip some random data
            // if the data round-trips successfully, the tokens are a pair
            // otherwise they are not, and validation fails
            Span<byte> round0 = stackalloc byte[128];
            Span<byte> round1 = stackalloc byte[128];
            Span<byte> roundE = stackalloc byte[256];
            round1.Fill(0);
            using (var rng = new SecureRandom())
                rng.GetNonZeroBytes(round0);

            using (var rsa = RSA.Create(RsaSize))
            {
                rsa.ImportRSAPublicKey(tkc.Key, out _);
                if (!rsa.TryEncrypt(round0, roundE, RSAEncryptionPadding.OaepSHA256, out _)) // will always be 256
                    return false;
            }

            using (var rsa = RSA.Create(RsaSize))
            {
                rsa.ImportRSAPrivateKey(tks.Key, out _);
                if (!rsa.TryDecrypt(roundE, round1, RSAEncryptionPadding.OaepSHA256, out var decw) || decw != round0.Length)
                    return false;
            }

            return AbstractionUtilities.CompareVectorized(round0, round1);
        }

        private bool GenerateSignatures(
            string actionId, 
            ReadOnlySpan<byte> state, 
            ReadOnlySpan<byte> kclient, 
            ReadOnlySpan<byte> kserver, 
            Span<byte> sigclient, 
            Span<byte> sigserver)
        {
            var aclen = AbstractionUtilities.UTF8.GetByteCount(actionId);
            Span<byte> buff = stackalloc byte[1 + aclen + state.Length + Math.Max(kclient.Length, kserver.Length)];

            // common
            AbstractionUtilities.UTF8.GetBytes(actionId, buff.Slice(1));
            state.CopyTo(buff.Slice(1 + aclen));

            // client
            buff[0] = 0xFF;
            kclient.CopyTo(buff.Slice(1 + aclen + state.Length));
            if (!this.TrySign(buff.Slice(0, 1 + aclen + state.Length + kclient.Length), sigclient))
                return false;

            // server
            buff[0] = 0x00;
            kserver.CopyTo(buff.Slice(1 + aclen + state.Length));
            if (!this.TrySign(buff.Slice(0, 1 + aclen + state.Length + kserver.Length), sigserver))
                return false;

            return true;
        }

        private bool TrySign(ReadOnlySpan<byte> input, Span<byte> output)
        {
            using (var hmac = new HMACSHA256(this.SigningKey))
                return hmac.TryComputeHash(input, output, out _);
        }
    }
}
