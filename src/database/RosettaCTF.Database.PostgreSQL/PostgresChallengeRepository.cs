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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using RosettaCTF.Data;
using RosettaCTF.Models;

namespace RosettaCTF
{
    internal sealed class PostgresChallengeRepository : ICtfChallengeRepository
    {
        private PostgresDbContext Database { get; }

        public PostgresChallengeRepository(PostgresDbContext database)
        {
            this.Database = database;
        }

        public async Task<IEnumerable<ICtfChallengeCategory>> GetCategoriesAsync(CancellationToken cancellationToken = default)
            => await this.Database.ChallengeCategories
                .Include(x => x.ChallengesInternal).ThenInclude(x => x.AttachmentsInternal)
                .Include(x => x.ChallengesInternal).ThenInclude(x => x.HintsInternal)
                .Include(x => x.ChallengesInternal).ThenInclude(x => x.EndpointInternal)
                .Include(x => x.ChallengesInternal).ThenInclude(x => x.CategoryInternal)
                .ToListAsync(cancellationToken);

        public async Task<ICtfChallengeCategory> GetCategoryAsync(string id, CancellationToken cancellationToken = default)
            => await this.Database.ChallengeCategories
                .Include(x => x.ChallengesInternal).ThenInclude(x => x.AttachmentsInternal)
                .Include(x => x.ChallengesInternal).ThenInclude(x => x.HintsInternal)
                .Include(x => x.ChallengesInternal).ThenInclude(x => x.EndpointInternal)
                .Include(x => x.ChallengesInternal).ThenInclude(x => x.CategoryInternal)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task<IEnumerable<ICtfChallenge>> GetChallengesAsync(CancellationToken cancellationToken = default)
            => await this.Database.Challenges
                .Include(x => x.AttachmentsInternal)
                .Include(x => x.HintsInternal)
                .Include(x => x.EndpointInternal)
                .Include(x => x.CategoryInternal)
                .ToListAsync(cancellationToken);

        public async Task<ICtfChallenge> GetChallengeAsync(string id, CancellationToken cancellationToken = default)
            => await this.Database.Challenges
                .Include(x => x.AttachmentsInternal)
                .Include(x => x.HintsInternal)
                .Include(x => x.EndpointInternal)
                .Include(x => x.CategoryInternal)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task InstallAsync(IEnumerable<ICtfChallengeCategory> categories, CancellationToken cancellationToken = default)
        {
            if (await this.Database.ChallengeCategories.AnyAsync(cancellationToken))
                return;

            var cats = new List<PostgresChallengeCategory>();
            var chls = new List<PostgresChallenge>();
            var hnts = new List<PostgresChallengeHint>();
            var atcs = new List<PostgresChallengeAttachment>();
            var enps = new List<PostgresChallengeEndpoint>();
            foreach (var category in categories)
            {
                var cat = new PostgresChallengeCategory
                {
                    Id = category.Id,
                    Name = category.Name,
                    IsHidden = category.IsHidden
                };
                cats.Add(cat);

                var cchls = new List<PostgresChallenge>();
                foreach (var challenge in category.Challenges)
                {
                    var chl = new PostgresChallenge
                    {
                        Id = challenge.Id,
                        Title = challenge.Title,
                        CategoryId = cat.Id,
                        Flag = challenge.Flag,
                        Difficulty = challenge.Difficulty,
                        Description = challenge.Description,
                        IsHidden = challenge.IsHidden,
                        BaseScore = challenge.BaseScore
                    };
                    chls.Add(chl);

                    if (challenge.Endpoint != null)
                    {
                        var endpoint = challenge.Endpoint;
                        var cenp = new PostgresChallengeEndpoint
                        {
                            Type = endpoint.Type,
                            Hostname = endpoint.Hostname,
                            Port = endpoint.Port,
                            ChallengeInternal = chl
                        };
                        enps.Add(cenp);
                        chl.Endpoint = cenp;
                    }

                    if (challenge.Hints != null && challenge.Hints.Any())
                    {
                        var chnts = new List<PostgresChallengeHint>();
                        foreach (var hint in challenge.Hints)
                        {
                            var hnt = new PostgresChallengeHint
                            {
                                Contents = hint.Contents,
                                ReleaseTime = hint.ReleaseTime,
                                ChallengeInternal = chl
                            };
                            hnts.Add(hnt);
                        }
                        chl.HintsInternal = chnts;
                    }

                    if (challenge.Attachments != null && challenge.Attachments.Any())
                    {
                        var catcs = new List<PostgresChallengeAttachment>();
                        foreach (var attachment in challenge.Attachments)
                        {
                            var atc = new PostgresChallengeAttachment
                            {
                                Name = attachment.Name,
                                Type = attachment.Type,
                                Length = attachment.Length,
                                Sha256 = attachment.Sha256,
                                Sha1 = attachment.Sha1,
                                DownloadUriInternal = attachment.DownloadUri?.ToString(),
                                ChallengeInternal = chl
                            };
                            atcs.Add(atc);

                            if (attachment.DecompressedAttachment != null)
                            {
                                var attachment_dec = attachment.DecompressedAttachment;
                                var atc_dec = new PostgresChallengeAttachment
                                {
                                    Name = attachment_dec.Name,
                                    Type = attachment_dec.Type,
                                    Length = attachment_dec.Length,
                                    Sha256 = attachment_dec.Sha256,
                                    Sha1 = attachment_dec.Sha1,
                                    DownloadUriInternal = attachment_dec.DownloadUri?.ToString(),
                                    ChallengeInternal = chl
                                };
                                atcs.Add(atc_dec);
                                atc.DecompressedAttachmentInternal = atc_dec;
                            }
                        }
                        chl.AttachmentsInternal = catcs;
                    }
                }
                cat.ChallengesInternal = cchls;
            }

            await this.Database.ChallengeCategories.AddRangeAsync(cats, cancellationToken);
            await this.Database.Challenges.AddRangeAsync(chls, cancellationToken);
            await this.Database.ChallengeHints.AddRangeAsync(hnts, cancellationToken);
            await this.Database.ChallengeAttachments.AddRangeAsync(atcs, cancellationToken);
            await this.Database.ChallengeEndpoints.AddRangeAsync(enps, cancellationToken);
            await this.Database.SaveChangesAsync(cancellationToken);
        }

        public async Task<ICtfSolveSubmission> SubmitSolveAsync(string flag, bool isValid, string challengeId, long userId, long teamId, int? score, CancellationToken cancellationToken = default)
        {
            var solve = new PostgresSolveSubmission
            {
                Contents = flag,
                IsValid = isValid,
                ChallengeId = challengeId,
                UserId = userId,
                TeamId = teamId,
                Timestamp = DateTimeOffset.UtcNow,
                Score = score
            };

            await this.Database.Solves.AddAsync(solve, cancellationToken);
            await this.Database.SaveChangesAsync(cancellationToken);

            return solve;
        }
    }
}
