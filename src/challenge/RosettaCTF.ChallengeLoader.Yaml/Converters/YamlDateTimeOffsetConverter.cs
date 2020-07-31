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
using System.Globalization;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace RosettaCTF.Converters
{
    internal sealed class YamlDateTimeOffsetConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
            => type == typeof(DateTimeOffset);

        public object ReadYaml(IParser parser, Type type)
        {
            var val = parser.Consume<Scalar>().Value;
            return DateTimeOffset.ParseExact(val, "yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture);
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var dto = (DateTimeOffset)value;
            var val = dto.ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture);

            emitter.Emit(new Scalar(null, null, val, ScalarStyle.Any, true, false));
        }
    }
}
