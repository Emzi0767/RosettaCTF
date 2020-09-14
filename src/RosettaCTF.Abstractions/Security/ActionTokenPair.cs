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

namespace RosettaCTF
{
    /// <summary>
    /// Represents a pair of tokens, representing an action the user is continuing.
    /// </summary>
    public sealed class ActionTokenPair
    {
        /// <summary>
        /// Gets the client's token.
        /// </summary>
        public ActionToken Client { get; }

        /// <summary>
        /// Gets the server's token.
        /// </summary>
        public ActionToken Server { get; }

        /// <summary>
        /// Creates a new token pair.
        /// </summary>
        /// <param name="client">Client token part.</param>
        /// <param name="server">Server token part.</param>
        public ActionTokenPair(ActionToken client, ActionToken server)
        {
            this.Client = client;
            this.Server = server;
        }
    }
}
