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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Emzi0767;
using EzOTP;
using RosettaCTF.Authentication;
using RosettaCTF.Data;
using RosettaCTF.Models;

namespace RosettaCTF.Services
{
    public sealed class MfaValidatorService
    {
        public bool ValidateCode(string code, IMultiFactorSettings mfa)
        {
            var otps = this.ToTotp(mfa);
            var otp = new OtpGenerator(otps);

            return otp.GenerateWindow().Contains(code);
        }

        public bool ValidateRecoveryCode(string code, IMultiFactorSettings mfa)
        {
            var otps = this.ToHotp(mfa);
            var otp = new OtpGenerator(otps);
            return otp.Generate(offset: mfa.RecoveryTripCount) == code;
        }

        public async Task<IMultiFactorSettings> GenerateMfaAsync(IMfaRepository mfaRepository, long userId, bool generateAdditional, CancellationToken cancellationToken = default)
        {
            var recbase = 0L;
            using (var rng = new SecureRandom())
                recbase = rng.GetInt64();

            var totp = TotpGeneratorSettings.GenerateGoogleAuthenticator(null, null);
            return await mfaRepository.ConfigureMfaAsync(userId, totp.Secret.ToArray(), totp.Digits, (MultiFactorHmac)totp.Algorithm, totp.Period, MultiFactorType.Google, null, recbase, cancellationToken);
        }

        public MfaSettingsModel GenerateClientData(IMultiFactorSettings mfa, string label, string issuer, string continuation)
        {
            var otps = new TotpGeneratorSettings(label, issuer, mfa.Secret, ByteEncoding.Base32, (HmacAlgorithm)mfa.HmacAlgorithm, mfa.Digits, mfa.Additional, mfa.Period);
            var uri = otps.ToUri().OriginalString;

            var recs = this.ToHotp(mfa);
            var otp = new OtpGenerator(recs);
            var reccodes = Enumerable.Range(0, 10).Select(x => otp.Generate(groupSize: 4)).ToArray();

            return new MfaSettingsModel
            {
                AuthenticatorUri = uri,
                RecoveryCodes = reccodes,
                Continuation = continuation
            };
        }

        private OtpGeneratorSettings ToTotp(IMultiFactorSettings mfa)
            => mfa.Type switch
            {
                MultiFactorType.Google => TotpGeneratorSettings.CreateGoogleAuthenticator(null, null, mfa.Secret),
                _                      => new TotpGeneratorSettings(null, null, mfa.Secret, ByteEncoding.Base32, (HmacAlgorithm)mfa.HmacAlgorithm, mfa.Digits, mfa.Additional, mfa.Period)
            };

        private OtpGeneratorSettings ToHotp(IMultiFactorSettings mfa)
            => new HotpGeneratorSettings(null, null, mfa.Secret, ByteEncoding.Base32, (HmacAlgorithm)mfa.HmacAlgorithm, 8, mfa.Additional, mfa.RecoveryCounterBase);
    }
}
