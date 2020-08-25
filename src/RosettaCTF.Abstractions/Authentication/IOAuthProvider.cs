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

namespace RosettaCTF.Authentication
{
    /// <summary>
    /// Represents an OAuth2 autentication provider.
    /// </summary>
    public interface IOAuthProvider
    {
        /// <summary>
        /// Gets the URL to send the client to in order for him to authenticate.
        /// </summary>
        /// <param name="ctx">Context in which authentication happens.</param>
        /// <returns>URL of the authentication page, as a string.</returns>
        string GetAuthenticationUrl(AuthenticationContext ctx);

        /// <summary>
        /// Completes OAuth authentication dance by exchanging a code for a token.
        /// </summary>
        /// <param name="ctx">Context in which authentication happens.</param>
        /// <param name="code">Authentication code returned from the login page.</param>
        /// <param name="cancellationToken">Token to cancel any pending operation, if applicable.</param>
        /// <returns>Resulting authentication token data.</returns>
        Task<OAuthResult> CompleteLoginAsync(AuthenticationContext ctx, string code, CancellationToken cancellationToken = default);

        /// <summary>
        /// Refreshes an expiring or expired OAuth token for a new one.
        /// </summary>
        /// <param name="ctx">Context in which authentication happens.</param>
        /// <param name="refreshToken">Refresh token used to obtain a fresh token.</param>
        /// <param name="cancellationToken">Token to cancel any pending operation, if applicable.</param>
        /// <returns>Resulting authentication token data.</returns>
        Task<OAuthResult> RefreshTokenAsync(AuthenticationContext ctx, string refreshToken, CancellationToken cancellationToken = default);

        /// <summary>
        /// Logs the user out of connected application, and revokes any access tokens.
        /// </summary>
        /// <param name="ctx">Context in which authentication happens.</param>
        /// <param name="token">Authentication token to revoke.</param>
        /// <param name="cancellationToken">Token to cancel any pending operation, if applicable.</param>
        /// <returns>Whether the operation was successful.</returns>
        Task<bool> LogoutAsync(AuthenticationContext ctx, string token, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets information about the user represented by the token.
        /// </summary>
        /// <param name="ctx">Context in which authentication happens.</param>
        /// <param name="token">Authentication token representing the authenticated user.</param>
        /// <param name="cancellationToken">Token to cancel any pending operation, if applicable.</param>
        /// <returns>Information about the authenticated user.</returns>
        Task<OAuthUser> GetUserAsync(AuthenticationContext ctx, string token, CancellationToken cancellationToken = default);
    }
}
