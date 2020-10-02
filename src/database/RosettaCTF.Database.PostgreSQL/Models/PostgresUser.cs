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
using System.Collections.Generic;
using RosettaCTF.Data;

namespace RosettaCTF.Models
{
    internal sealed class PostgresUser : IUser
    {
        public long Id { get; set; }

        public string Username { get; set; }

        public string CountryCode { get; set; }

        public PostgresCountry CountryInternal { get; set; }

        public ICountry Country => this.CountryInternal;

        public string AvatarUrlInternal { get; set; }

        public PostgresUserPassword PasswordInternal { get; set; }

        public byte[] Password => this.PasswordInternal?.PasswordHash;

        public Uri AvatarUrl
        {
            get => this.AvatarUrlInternal != null ? new Uri(this.AvatarUrlInternal) : null;
            set => this.AvatarUrlInternal = value?.ToString();
        }

        public bool IsAuthorized { get; set; }

        public long? TeamId { get; set; }

        public PostgresTeam TeamInternal { get; set; }

        public ITeam Team => this.TeamInternal;

        public IEnumerable<PostgresExternalUser> ConnectedAccountsInternal { get; set; }

        public IEnumerable<IExternalUser> ConnectedAccounts => this.ConnectedAccountsInternal;

        public IEnumerable<PostgresSolveSubmission> SolvesInternal { get; set; }

        public IEnumerable<PostgresTeamInvite> InvitesInternal { get; set; }

        public bool RequiresMfa => this.MfaInternal != null && this.MfaInternal.IsConfirmed;

        public PostgresMfaSettings MfaInternal { get; set; }
    }
}
