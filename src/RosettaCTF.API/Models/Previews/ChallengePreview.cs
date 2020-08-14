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
using System.Linq;
using System.Runtime.InteropServices;
using RosettaCTF.Data;

namespace RosettaCTF.Models
{
    /// <summary>
    /// Represents a preview version of a challenge, that is, a simplified version, produced by the API for consumption.
    /// </summary>
    public sealed class ChallengePreview
    {
        /// <summary>
        /// Gets the Id of this challenge.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the title of this challenge.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the ID of the category this challenge belongs to.
        /// </summary>
        public string CategoryId { get; }

        /// <summary>
        /// Gets the author-perceived difficulty of this challenge.
        /// </summary>
        public string Difficulty { get; }

        /// <summary>
        /// Gets the description of this challenge.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the hints available for this challenge.
        /// </summary>
        public IEnumerable<string> Hints { get; }

        /// <summary>
        /// Gets the attachments available as part of this challenge.
        /// </summary>
        public IEnumerable<ChallengeAttachmentPreview> Attachments { get; }

        /// <summary>
        /// Gets the endpoint the users are meant to connect to as part of the challenge.
        /// </summary>
        public string Endpoint { get; }

        /// <summary>
        /// Gets the current score for solving the challenge.
        /// </summary>
        public int Score { get; }

        /// <summary>
        /// Gets whether the currently-logged in user's team solved the challenge already.
        /// </summary>
        public bool? IsSolved { get; }

        internal ChallengePreview(ICtfChallenge challenge, TimeSpan elapsed, EnumDisplayConverter enumCv)
            : this(challenge, elapsed, enumCv, null)
        { }

        internal ChallengePreview(ICtfChallenge challenge, TimeSpan elapsed, EnumDisplayConverter enumCv, int? score)
            : this(challenge, elapsed, enumCv, score, null)
        { }

        internal ChallengePreview(ICtfChallenge challenge, TimeSpan elapsed, EnumDisplayConverter enumCv, int? score, bool? solved)
        {
            this.Id = challenge.Id;
            this.Title = challenge.Title;
            this.CategoryId = challenge.Category.Id;
            this.Difficulty = enumCv.Convert(challenge.Difficulty);
            this.Description = challenge.Description;
            if (challenge.Hints != null)
                this.Hints = challenge.Hints
                    .Where(x => x.ReleaseTime <= elapsed)
                    .Select(x => x.Contents)
                    .ToList();

            if (challenge.Attachments != null)
                this.Attachments = challenge.Attachments
                    .Select(x => new ChallengeAttachmentPreview(x))
                    .ToList();

            if (challenge.Endpoint != null)
                this.Endpoint = EndpointToString(challenge.Endpoint);

            this.Score = score ?? challenge.BaseScore;
            this.IsSolved = solved;
        }

        private static string EndpointToString(ICtfChallengeEndpoint endpoint)
        {
            var hnl = endpoint.Hostname.Length;
            var prl = GetPortLength(endpoint.Port);

            return endpoint.Type switch
            {
                CtfChallengeEndpointType.Unknown => string.Create(hnl + prl + 1, (endpoint, prl), BuildUnknown),
                CtfChallengeEndpointType.Netcat => string.Create(hnl + prl + 4, (endpoint, prl), BuildNetcat),
                CtfChallengeEndpointType.Http => string.Create(endpoint.Port == 80 ? hnl + 8 : hnl + prl + 9, (endpoint, prl), BuildHttp),
                CtfChallengeEndpointType.Ssh => string.Create(endpoint.Port == 22 ? hnl + 4 : hnl + prl + 8, (endpoint, prl), BuildSsh),
                CtfChallengeEndpointType.Ssl => string.Create(endpoint.Port == 443 ? hnl + 26 : hnl + prl + 27, (endpoint, prl), BuildSsl),
                CtfChallengeEndpointType.Https => string.Create(endpoint.Port == 443 ? hnl + 9 : hnl + prl + 10, (endpoint, prl), BuildHttps),
                _ => null
            };

            static void BuildUnknown(Span<char> buff, (ICtfChallengeEndpoint ep, int portLength) state)
            {
                var (ep, prl) = state;
                var hnl = ep.Hostname.Length;

                ep.Port.TryFormat(buff.Slice(hnl + 1), out _);
                ep.Hostname.AsSpan().CopyTo(buff);
                buff[hnl] = ':';
            }

            static void BuildNetcat(Span<char> buff, (ICtfChallengeEndpoint ep, int portLength) state)
            {
                var (ep, prl) = state;
                var hnl = ep.Hostname.Length;

                ep.Port.TryFormat(buff.Slice(hnl + 4), out _);
                ep.Hostname.AsSpan().CopyTo(buff.Slice(3));
                buff[hnl + 3] = ' ';
                buff[2] = ' ';
                buff[1] = 'c';
                buff[0] = 'n';
            }

            static void BuildHttp(Span<char> buff, (ICtfChallengeEndpoint ep, int portLength) state)
            {
                var (ep, prl) = state;
                var hnl = ep.Hostname.Length;

                buff[^1] = '/';

                var bbuff = MemoryMarshal.Cast<char, long>(buff);
                bbuff[0] = 0x0070007400740068; // http (LE)
                bbuff[1] = 0x0000002F002F003A; // ://  (LE)

                ep.Hostname.AsSpan().CopyTo(buff.Slice(7));

                if (ep.Port != 80)
                {
                    buff[^(prl + 2)] = ':';
                    ep.Port.TryFormat(buff.Slice(hnl + 8), out _);
                }
            }

            static void BuildSsh(Span<char> buff, (ICtfChallengeEndpoint ep, int portLength) state)
            {
                var (ep, prl) = state;
                var hnl = ep.Hostname.Length;

                var bbuff = MemoryMarshal.Cast<char, long>(buff);
                bbuff[0] = 0x0020006800730073; // ssh  (LE)

                ep.Hostname.AsSpan().CopyTo(buff.Slice(4));

                if (ep.Port != 22)
                {
                    bbuff = MemoryMarshal.Cast<char, long>(buff.Slice(hnl + 4));
                    bbuff[0] = 0x00200070002D0020;

                    ep.Port.TryFormat(buff.Slice(hnl + 8), out _);
                }
            }

            static void BuildSsl(Span<char> buff, (ICtfChallengeEndpoint ep, int portLength) state)
            {
                var (ep, prl) = state;
                var hnl = ep.Hostname.Length;

                buff[^1] = '/';

                var bbuff = MemoryMarshal.Cast<char, long>(buff);
                bbuff[0] = 0x006E00650070006F; // open (LE)
                bbuff[1] = 0x0020006C00730073; // ssl  (LE)
                bbuff[2] = 0x006C0063005F0073; // s_cl (LE)
                bbuff[3] = 0x0074006E00650069; // ient (LE)
                bbuff[4] = 0x006F0063002D0020; //  -co (LE)
                bbuff[5] = 0x00630065006E006E; // nnec (LE)
                buff[24] = 't';
                buff[25] = ' ';

                ep.Hostname.AsSpan().CopyTo(buff.Slice(26));

                if (ep.Port != 443)
                {
                    buff[^(prl + 1)] = ':';
                    ep.Port.TryFormat(buff.Slice(hnl + 27), out _);
                }
            }

            static void BuildHttps(Span<char> buff, (ICtfChallengeEndpoint ep, int portLength) state)
            {
                var (ep, prl) = state;
                var hnl = ep.Hostname.Length;

                buff[^1] = '/';

                var bbuff = MemoryMarshal.Cast<char, long>(buff);
                bbuff[0] = 0x0070007400740068; // http (LE)
                bbuff[1] = 0x002F002F003A0073; // s:// (LE)

                ep.Hostname.AsSpan().CopyTo(buff.Slice(8));

                if (ep.Port != 443)
                {
                    buff[^(prl + 2)] = ':';
                    ep.Port.TryFormat(buff.Slice(hnl + 9), out _);
                }
            }
        }

        private static int GetPortLength(int v)
            => v switch
            {
                int i when i < 10 => 1,
                int i when i < 100 => 2,
                int i when i < 1000 => 3,
                int i when i < 10000 => 4,
                _ => 5
            };
    }
}
