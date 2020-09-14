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

#pragma warning disable CA1819 // Properties should not return arrays

using System;
using System.Buffers.Binary;

namespace RosettaCTF
{
    /// <summary>
    /// Represents a continuation token.
    /// </summary>
    public sealed class ActionToken
    {
        /// <summary>
        /// Gets whether this token is the client token.
        /// </summary>
        public bool IsClientPart { get; }

        /// <summary>
        /// Gets the key part of the token, used for validation.
        /// </summary>
        public byte[] Key { get; }

        /// <summary>
        /// Gets the state part of the token, used for identification.
        /// </summary>
        public byte[] State { get; }

        /// <summary>
        /// Gets the signature part of the token.
        /// </summary>
        public byte[] Signature { get; }

        internal ActionToken(bool isClient, byte[] key, byte[] state, byte[] signature)
        {
            this.IsClientPart = isClient;
            this.Key = key;
            this.State = state;
            this.Signature = signature;
        }

        /// <summary>
        /// Exports this token as a string.
        /// </summary>
        /// <param name="destination">Buffer in which to place the characters.</param>
        /// <param name="written">Number of characters written to the buffer.</param>
        /// <returns>Whether the operation was successful.</returns>
        public bool TryExportString(Span<char> destination, out int written)
        {
            var (raw, encoded) = this.EstimateLength();
            written = encoded;

            if (destination.Length < written)
                return false;

            Span<byte> buff = stackalloc byte[raw];

            // write type
            buff[0] = (byte)(this.IsClientPart ? 0xFF : 0x00);

            // write lengths
            BinaryPrimitives.WriteInt16BigEndian(buff.Slice(1), (short)this.Key.Length);
            BinaryPrimitives.WriteInt16BigEndian(buff.Slice(3), (short)this.State.Length);
            BinaryPrimitives.WriteInt16BigEndian(buff.Slice(5), (short)this.Signature.Length);

            // write parts
            this.Key.AsSpan().CopyTo(buff.Slice(7));
            this.State.AsSpan().CopyTo(buff.Slice(7 + this.Key.Length));
            this.Signature.AsSpan().CopyTo(buff.Slice(7 + this.Key.Length + this.State.Length));

            return Convert.TryToBase64Chars(buff, destination, out written, Base64FormattingOptions.None);
        }

        /// <summary>
        /// Exports this token as a string.
        /// </summary>
        /// <returns>Exported token.</returns>
        public string ExportString()
        {
            return string.Create(this.EstimateLength().encoded, this, Base64);

            static void Base64(Span<char> buff, ActionToken state)
                => state.TryExportString(buff, out _);
        }

        /// <summary>
        /// Parses a string as a token.
        /// </summary>
        /// <param name="input">Input string to parse.</param>
        /// <param name="token">Parsed token.</param>
        /// <returns>Whether the operation was successful.</returns>
        public static bool TryParse(ReadOnlySpan<char> input, out ActionToken token)
        {
            token = null;

            Span<byte> raw = stackalloc byte[input.Length * 3 / 4];
            if (!Convert.TryFromBase64Chars(input, raw, out var written))
                return false;

            if (raw[0] != 0x00 && raw[0] != 0xFF)
                return false;

            var isClient = raw[0] == 0xFF;
            var klen   = BinaryPrimitives.ReadInt16BigEndian(raw.Slice(1));
            var stlen  = BinaryPrimitives.ReadInt16BigEndian(raw.Slice(3));
            var siglen = BinaryPrimitives.ReadInt16BigEndian(raw.Slice(5));

            if (klen + stlen + siglen + 1 + 6 != written)
                return false;

            var key       = new byte[klen];
            var state     = new byte[stlen];
            var signature = new byte[siglen];

            raw.Slice(7, klen).CopyTo(key);
            raw.Slice(7 + klen, stlen).CopyTo(state);
            raw.Slice(7 + klen + stlen, siglen).CopyTo(signature);

            token = new ActionToken(isClient, key, state, signature);
            return true;
        }

        private (int raw, int encoded) EstimateLength()
        {
            var len = 1 + this.Key.Length + this.State.Length + this.Signature.Length + 6 /* 3 lengths */;
            return (len, ((4 * len / 3) + 3) & ~3);
        }
    }
}

#pragma warning restore CA1819 // Properties should not return arrays
