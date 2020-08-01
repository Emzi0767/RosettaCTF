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
using SharpYaml.Events;
using SharpYaml.Serialization;
using SharpYaml.Serialization.Serializers;

namespace RosettaCTF.Converters
{
    internal sealed class YamlDateTimeOffsetConverter : ScalarSerializerBase, IYamlSerializableFactory
    {
        public override object ConvertFrom(ref ObjectContext context, Scalar fromScalar)
            => DateTimeOffset.ParseExact(fromScalar.Value, "yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture);

        public override string ConvertTo(ref ObjectContext objectContext)
            => ((DateTimeOffset)objectContext.Instance).ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture);

        public IYamlSerializable TryCreate(SerializerContext context, ITypeDescriptor typeDescriptor)
            => typeDescriptor.Type == typeof(DateTimeOffset)
            ? this
            : null;
    }
}
