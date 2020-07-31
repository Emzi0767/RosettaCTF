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

using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Options;
using RosettaCTF.Converters;
using RosettaCTF.Data;
using RosettaCTF.Datatypes;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace RosettaCTF
{
    /// <summary>
    /// Provides a YAML-based event configuration loader, which includes basic event settings, and defined challenges.
    /// </summary>
    public sealed class YamlCtfConfigurationLoader : ICtfConfigurationLoader
    {
        private readonly CtfEvent _eventConfig;
        private readonly IEnumerable<ICtfChallengeCategory> _categories;

        /// <summary>
        /// Creates a new instance of the loader.
        /// </summary>
        /// <param name="opts">Application configuration.</param>
        public YamlCtfConfigurationLoader(IOptions<RosettaConfigurationRoot> opts)
        {
            var deserializer = new DeserializerBuilder()
                .WithTypeConverter(new YamlTimeSpanConverter(), c => c.OnTop())
                .WithTypeConverter(new YamlDateTimeOffsetConverter(), c => c.OnTop())
                .IgnoreFields()
                .Build();

            var optv = opts.Value;
            var fi = new FileInfo(optv.EventConfiguration);
            using (var fs = fi.OpenRead())
            using (var sr = new StreamReader(fs, AbstractionUtilities.UTF8))
            {
                var parser = new Parser(sr);

                this._eventConfig = ReadEvent(parser, deserializer);
                this._categories = ReadCategories(parser, deserializer);
            }
        }

        /// <inheritdoc />
        public CtfEvent LoadEventData()
            => this._eventConfig;

        /// <inheritdoc />
        public IEnumerable<ICtfChallengeCategory> LoadChallenges()
            => this._categories;

        private static CtfEvent ReadEvent(IParser parser, IDeserializer deserializer)
        {
            if (!parser.TryConsume<DocumentStart>(out _))
                throw new InvalidDataException("YAML configuration is malformed.");

            return deserializer.Deserialize<CtfEvent>(parser);
        }

        private static IEnumerable<ICtfChallengeCategory> ReadCategories(IParser parser, IDeserializer deserializer)
        {
            if (!parser.TryConsume<DocumentStart>(out _))
                throw new InvalidDataException("YAML configuration is malformed.");

            return deserializer.Deserialize<List<YamlCtfChallengeCategory>>(parser);
        }
    }
}
