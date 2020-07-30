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

namespace RosettaCTF.Data
{
    /// <summary>
    /// Represents a preview version of a challenge, that is, a simplified version, produced by the API for consumption.
    /// </summary>
    public interface IChallengePreview
    {
        /// <summary>
        /// Gets the Id of this challenge.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the title of this challenge.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Gets the ID of the category this challenge belongs to.
        /// </summary>
        string CategoryId { get; }

        /// <summary>
        /// Gets the author-perceived difficulty of this challenge.
        /// </summary>
        string Difficulty { get; }

        /// <summary>
        /// Gets the description of this challenge.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the hints available for this challenge.
        /// </summary>
        IEnumerable<string> Hints { get; }

        /// <summary>
        /// Gets the attachments available as part of this challenge.
        /// </summary>
        IEnumerable<IChallengeAttachmentPreview> Attachments { get; }

        /// <summary>
        /// Gets the endpoint the users are meant to connect to as part of the challenge.
        /// </summary>
        string Endpoint { get; }
    }
}
