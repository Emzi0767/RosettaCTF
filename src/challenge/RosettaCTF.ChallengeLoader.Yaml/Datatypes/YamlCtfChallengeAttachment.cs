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
using YamlDotNet.Serialization;

namespace RosettaCTF
{
    internal sealed class YamlCtfChallengeAttachment : ICtfChallengeAttachment
    {
        [YamlMember(Alias = "filename")]
        public string Name { get; set; }

        [YamlMember(Alias = "type")]
        public string Type { get; set; }

        [YamlMember(Alias = "length")]
        public long Length { get; set; }

        [YamlMember(Alias = "sha256")]
        public string Sha256 { get; set; }

        [YamlMember(Alias = "sha1")]
        public string Sha1 { get; set; }

        [YamlMember(Alias = "url")]
        public Uri DownloadUri { get; set; }

        [YamlMember(Alias = "decompressed", SerializeAs = typeof(YamlCtfChallengeAttachment))]
        public ICtfChallengeAttachment DecompressedAttachment { get; set; }
    }
}
