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

namespace RosettaCTF.Data
{
    /// <summary>
    /// Contains information about a country.
    /// </summary>
    public interface ICountry
    {
        /// <summary>
        /// Gets the ISO-3166-alpha-2 country code.
        /// </summary>
        string Code { get; }

        /// <summary>
        /// Gets the full country name.
        /// </summary>
        string Name { get; }
    }
}
