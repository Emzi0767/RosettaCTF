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
using RosettaCTF.Data;

namespace RosettaCTF.Models
{
    internal sealed class PostgresChallengeAttachment : ICtfChallengeAttachment
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public long Length { get; set; }

        public string Sha256 { get; set; }

        public string Sha1 { get; set; }

        public string DownloadUriInternal { get; set; }

        public Uri DownloadUri
        {
            get => this.DownloadUriInternal != null ? new Uri(this.DownloadUriInternal) : null;
            set => this.DownloadUriInternal = value?.ToString();
        }

        public long? DecompressedAttachmentId { get; set; }

        public PostgresChallengeAttachment DecompressedAttachmentInternal { get; set; }

        public ICtfChallengeAttachment DecompressedAttachment => this.DecompressedAttachmentInternal;

        public PostgresChallengeAttachment CompressedAttachmentInternal { get; set; }

        public string ChallengeId { get; set; }

        public PostgresChallenge ChallengeInternal { get; set; }
    }
}
