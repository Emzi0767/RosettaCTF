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
    internal sealed class YamlTimeSpanConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
            => type == typeof(TimeSpan);

        public object ReadYaml(IParser parser, Type type)
        {
            var val = long.Parse(parser.Consume<Scalar>().Value, NumberStyles.Integer, CultureInfo.InvariantCulture);
            return TimeSpan.FromSeconds(val);
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var ts = (TimeSpan)value;
            var val = (long)ts.TotalSeconds;

            emitter.Emit(new Scalar(null, null, val.ToString(CultureInfo.InvariantCulture), ScalarStyle.Any, true, false));
        }
    }
}
