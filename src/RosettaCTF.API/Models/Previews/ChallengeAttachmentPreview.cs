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

using RosettaCTF.Data;

namespace RosettaCTF.Models
{
    /// <summary>
    /// Represents a preview version of a challenge attachment, that is, a simplified version, produced by the API for consumption.
    /// </summary>
    public sealed class ChallengeAttachmentPreview
    {
        /// <summary>
        /// Gets the name of the attached file.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the type of the attached file.
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Gets the length string of the attached file.
        /// </summary>
        public string Length { get; }

        /// <summary>
        /// Gets the SHA256 checksum of the attached file.
        /// </summary>
        public string Sha256 { get; }

        /// <summary>
        /// Gets the SHA1 checksum of the attached file.
        /// </summary>
        public string Sha1 { get; }

        /// <summary>
        /// Gets the download URL of the attached file.
        /// </summary>
        public string Uri { get; }

        internal ChallengeAttachmentPreview(ICtfChallengeAttachment attachment)
        {
            this.Name = attachment.Name;
            this.Type = attachment.Type;
            this.Length = FormatLength(attachment.Length);
            this.Sha256 = attachment.Sha256;
            this.Sha1 = attachment.Sha1;
            this.Uri = attachment.DownloadUri?.ToString();
        }

        // doubles alloc, and figuring out the number of commas would probably make this unnecessarily complicated
        private static string FormatLength(long length)
            => length > 1000 
                ? $"{FormatSize(length)} ({length:#,##0} bytes)"
                : $"{length:#,##0} bytes";

        private static string[] Prefixes { get; } = new[] { "K", "M", "G", "T" };

        private static string FormatSize(long length)
        {
            double d = length;
            var i = -1;
            while (d >= 1000 && i < Prefixes.Length - 1)
            {
                d /= 1024.0;
                ++i;
            }

            return $"{d:#,##0.00} {Prefixes[i]}iB";
        }
    }
}
