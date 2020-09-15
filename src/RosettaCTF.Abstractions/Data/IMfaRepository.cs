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

using System.Threading;
using System.Threading.Tasks;
using RosettaCTF.Authentication;

namespace RosettaCTF.Data
{
    /// <summary>
    /// Contains methods for querying and modifying Multi-Factor authentication settings for users.
    /// </summary>
    public interface IMfaRepository
    {
        /// <summary>
        /// Retrieves MFA settings for specified user.
        /// </summary>
        /// <param name="userId">ID of the user for which to retrieve MFA settings.</param>
        /// <param name="cancellationToken">A token to cancel pending operations, if any.</param>
        /// <returns>Requested MFA settings.</returns>
        Task<IMultiFactorSettings> GetMfaSettingsAsync(long userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates new MFA configuration for specified user.
        /// </summary>
        /// <param name="userId">ID of the user for which to configure MFA.</param>
        /// <param name="secret">Secret used to generate MFA codes.</param>
        /// <param name="digits">Number of digits in generated codes.</param>
        /// <param name="hmac">Algorithm used to generate codes.</param>
        /// <param name="period">Number of seconds constituting a single counter tick.</param>
        /// <param name="type">Type of the code generator.</param>
        /// <param name="additional">Additional data used for generating codes, if applicable.</param>
        /// <param name="recoveryBase">Base value of recovery counter generator.</param>
        /// <param name="cancellationToken">A token to cancel pending operations, if any.</param>
        /// <returns>Created MFA settings.</returns>
        Task<IMultiFactorSettings> ConfigureMfaAsync(
            long userId, 
            byte[] secret, 
            int digits, 
            MultiFactorHmac hmac, 
            int period, 
            MultiFactorType type, 
            byte[] additional, 
            long recoveryBase, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes configured MFA settings for a given user.
        /// </summary>
        /// <param name="userId">ID of the user for which to remove MFA.</param>
        /// <param name="cancellationToken">A token to cancel pending operations, if any.</param>
        /// <returns>A task encapsulating the operation.</returns>
        Task RemoveMfaAsync(long userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Confirms MFA configuration.
        /// </summary>
        /// <param name="userId">ID of the user for which to remove MFA.</param>
        /// <param name="cancellationToken">A token to cancel pending operations, if any.</param>
        /// <returns>A task encapsulating the operation.</returns>
        Task ConfirmMfaAsync(long userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Trips a recovery code, incrementing the recovery code counter.
        /// </summary>
        /// <param name="userId">ID of the user for which to trip MFA recovery counter.</param>
        /// <param name="cancellationToken">A token to cancel pending operations, if any.</param>
        /// <returns>A task encapsulating the operation.</returns>
        Task TripRecoveryCodeAsync(long userId, CancellationToken cancellationToken = default);
    }
}
