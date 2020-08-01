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
using SharpYaml.Serialization;

namespace RosettaCTF.Converters
{
    internal sealed class YamlObjectFactory : IObjectFactory
    {
        private IObjectFactory _fallback;

        public YamlObjectFactory()
        {
            this._fallback = new DefaultObjectFactory();
        }

        public object Create(Type type)
        {
            if (type == typeof(ICtfChallengeCategory))
                return new YamlCtfChallengeCategory();
            else if (type == typeof(ICtfChallenge))
                return new YamlCtfChallenge();
            else if (type == typeof(ICtfChallengeAttachment))
                return new YamlCtfChallengeAttachment();
            else if (type == typeof(ICtfChallengeHint))
                return new YamlCtfChallengeHint();
            else if (type == typeof(ICtfChallengeEndpoint))
                return new YamlCtfChallengeEndpoint();
            else if (type == typeof(ICtfEvent))
                return new YamlCtfEvent();

            return this._fallback.Create(type);
        }
    }
}
