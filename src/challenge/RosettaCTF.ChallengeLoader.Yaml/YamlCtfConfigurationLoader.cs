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
using SharpYaml;
using SharpYaml.Events;
using SharpYaml.Serialization;

namespace RosettaCTF
{
    /// <summary>
    /// Provides a YAML-based event configuration loader, which includes basic event settings, and defined challenges.
    /// </summary>
    public sealed class YamlCtfConfigurationLoader : ICtfConfigurationLoader
    {
        private readonly ICtfEvent _eventConfig;
        private readonly IEnumerable<ICtfChallengeCategory> _categories;

        /// <summary>
        /// Creates a new instance of the loader.
        /// </summary>
        /// <param name="opts">Application configuration.</param>
        public YamlCtfConfigurationLoader(IOptions<ConfigurationRoot> opts)
        {
            var des = new SerializerSettings
            {
                ObjectFactory = new YamlObjectFactory()
            };

            des.RegisterSerializerFactory(new YamlTimeSpanConverter());
            des.RegisterSerializerFactory(new YamlDateTimeOffsetConverter());
            des.RegisterSerializerFactory(new YamlUriConverter());

            var deserializer = new Serializer(des);

            var optv = opts.Value;
            var fi = new FileInfo(optv.EventConfiguration);
            using (var fs = fi.OpenRead())
            using (var sr = new StreamReader(fs, AbstractionUtilities.UTF8))
            {
                var parser = new Parser(sr);
                var er = new EventReader(parser);
                if (er.Expect<StreamStart>() == null)
                    throw new InvalidDataException("YAML configuration is malformed.");

                this._eventConfig = ReadEvent(er, deserializer);
                this._categories = ReadCategories(er, deserializer);

                foreach (var cat in this._categories)
                    foreach (var chall in cat.Challenges)
                        (chall as YamlCtfChallenge).Category = cat;
            }
        }

        /// <inheritdoc />
        public ICtfEvent LoadEventData()
            => this._eventConfig;

        /// <inheritdoc />
        public IEnumerable<ICtfChallengeCategory> LoadChallenges()
            => this._categories;

        private static ICtfEvent ReadEvent(EventReader eventReader, Serializer serializer)
            => serializer.Deserialize<YamlCtfEvent>(eventReader);

        private static IEnumerable<ICtfChallengeCategory> ReadCategories(EventReader eventReader, Serializer serializer)
            => serializer.Deserialize<List<YamlCtfChallengeCategory>>(eventReader);
    }
}
