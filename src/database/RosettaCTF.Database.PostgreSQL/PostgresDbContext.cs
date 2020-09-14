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

using Microsoft.EntityFrameworkCore;
using Npgsql;
using RosettaCTF.Authentication;
using RosettaCTF.Data;
using RosettaCTF.Models;
using RosettaCTF.SeedData;

namespace RosettaCTF
{
    internal sealed class PostgresDbContext : DbContext
    {
        public DbSet<PostgresUser> Users { get; set; }
        public DbSet<PostgresUserPassword> UserPasswords { get; set; }
        public DbSet<PostgresExternalUser> ConnectedAccounts { get; set; }
        public DbSet<PostgresTeam> Teams { get; set; }
        public DbSet<PostgresChallenge> Challenges { get; set; }
        public DbSet<PostgresChallengeCategory> ChallengeCategories { get; set; }
        public DbSet<PostgresChallengeHint> ChallengeHints { get; set; }
        public DbSet<PostgresChallengeEndpoint> ChallengeEndpoints { get; set; }
        public DbSet<PostgresChallengeAttachment> ChallengeAttachments { get; set; }
        public DbSet<PostgresSolveSubmission> Solves { get; set; }
        public DbSet<PostgresTeamInvite> TeamInvites { get; set; }
        public DbSet<PostgresCountry> Countries { get; set; }
        public DbSet<PostgresMfaSettings> MfaSettings { get; set; }

        static PostgresDbContext()
        {
            NpgsqlConnection.GlobalTypeMapper.MapEnum<CtfChallengeDifficulty>("ctf_challenge_difficulty");
            NpgsqlConnection.GlobalTypeMapper.MapEnum<CtfChallengeEndpointType>("ctf_challenge_endpoint_type");
            NpgsqlConnection.GlobalTypeMapper.MapEnum<MultiFactorHmac>("mfa_hmac");
            NpgsqlConnection.GlobalTypeMapper.MapEnum<MultiFactorType>("mfa_type");
        }

        public PostgresDbContext(DbContextOptions<PostgresDbContext> opts)
            : base(opts)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Map enums
            modelBuilder.HasPostgresEnum<CtfChallengeDifficulty>();
            modelBuilder.HasPostgresEnum<CtfChallengeEndpointType>();
            modelBuilder.HasPostgresEnum<MultiFactorHmac>();
            modelBuilder.HasPostgresEnum<MultiFactorType>();

            // User
            modelBuilder.Entity<PostgresUser>(e =>
            {
                e.ToTable("users")
                    .Ignore(m => m.Team)
                    .Ignore(m => m.AvatarUrl)
                    .Ignore(m => m.ConnectedAccounts)
                    .Ignore(m => m.Password)
                    .Ignore(m => m.Country);

                e.Property(m => m.Id)
                    .IsRequired()
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                e.Property(m => m.Username)
                    .IsRequired()
                    .HasColumnName("name");

                e.Property(m => m.CountryCode)
                    .HasColumnName("country")
                    .HasDefaultValue(null);

                e.Property(m => m.AvatarUrlInternal)
                    .HasColumnName("avatar")
                    .HasColumnType("text")
                    .HasDefaultValue(null);

                e.Property(m => m.IsAuthorized)
                    .IsRequired()
                    .HasColumnName("authorized");

                e.Property(m => m.HasHiddenAccess)
                    .IsRequired()
                    .HasColumnName("access_hidden");

                e.Property(m => m.TeamId)
                    .HasColumnName("team_id")
                    .HasDefaultValue(null);

                e.HasKey(m => m.Id)
                    .HasName("pkey_user_id");

                e.HasAlternateKey(m => m.Username)
                    .HasName("ukey_user_name");

                e.HasOne(m => m.CountryInternal)
                    .WithMany(m => m.UsersInternal)
                    .HasForeignKey(m => m.CountryCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fkey_user_country");

                e.HasOne(m => m.TeamInternal)
                    .WithMany(m => m.MembersInternal)
                    .HasForeignKey(m => m.TeamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fkey_user_team");
            });

            // User passwords
            modelBuilder.Entity<PostgresUserPassword>(e =>
            {
                e.ToTable("passwords");

                e.Property(m => m.UserId)
                    .IsRequired()
                    .ValueGeneratedNever()
                    .HasColumnName("name");

                e.Property(m => m.PasswordHash)
                    .HasColumnName("hash")
                    .HasColumnType("bytea")
                    .HasDefaultValue(null);

                e.HasKey(m => m.UserId)
                    .HasName("pkey_user_pwd_id");

                e.HasOne(m => m.UserInternal)
                    .WithOne(m => m.PasswordInternal)
                    .HasForeignKey<PostgresUserPassword>(m => m.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fkey_user_pwd");
            });

            // Connected accounts
            modelBuilder.Entity<PostgresExternalUser>(e =>
            {
                e.ToTable("users_oauth")
                    .Ignore(m => m.User);

                e.Property(m => m.Id)
                    .IsRequired()
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                e.Property(m => m.Username)
                    .IsRequired()
                    .HasColumnName("username");

                e.Property(m => m.ProviderId)
                    .IsRequired()
                    .ValueGeneratedNever()
                    .HasColumnName("provider");

                e.Property(m => m.Token)
                    .HasColumnName("token")
                    .HasDefaultValue(null);

                e.Property(m => m.RefreshToken)
                    .HasColumnName("refresh_token")
                    .HasDefaultValue(null);

                e.Property(m => m.TokenExpirationTime)
                    .HasColumnName("token_expires_at")
                    .HasColumnType("timestamptz")
                    .HasDefaultValue(null);

                e.Property(m => m.UserId)
                    .IsRequired()
                    .HasColumnName("user");

                e.HasKey(m => new { m.UserId, m.ProviderId })
                    .HasName("pkey_user_oauth_id_provider");

                e.HasAlternateKey(m => new { m.Id, m.ProviderId })
                    .HasName("ukey_user_oauth_name_provider");

                e.HasOne(m => m.UserInternal)
                    .WithMany(m => m.ConnectedAccountsInternal)
                    .HasForeignKey(m => m.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fkey_user_oauth_user");
            });

            // Team
            modelBuilder.Entity<PostgresTeam>(e =>
            {
                e.ToTable("teams")
                    .Ignore(m => m.Members)
                    .Ignore(m => m.AvatarUrl);

                e.Property(m => m.Id)
                    .IsRequired()
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                e.Property(m => m.Name)
                    .IsRequired()
                    .HasColumnName("name");

                e.Property(m => m.AvatarUrlInternal)
                    .HasColumnName("avatar")
                    .HasColumnType("text")
                    .HasDefaultValue(null);

                e.HasKey(m => m.Id)
                    .HasName("pkey_team_id");

                e.HasAlternateKey(m => m.Name)
                    .HasName("ukey_team_name");
            });

            // Team invite
            modelBuilder.Entity<PostgresTeamInvite>(e =>
            {
                e.ToTable("team_invites")
                    .Ignore(m => m.Team)
                    .Ignore(m => m.User);

                e.Property(m => m.Id)
                    .IsRequired()
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                e.Property(m => m.UserId)
                    .IsRequired()
                    .HasColumnName("user_id");

                e.Property(m => m.TeamId)
                    .IsRequired()
                    .HasColumnName("team_id");

                e.HasIndex(m => m.Id)
                    .HasName("pkey_team_invite_id");

                e.HasAlternateKey(m => new { m.UserId, m.TeamId })
                    .HasName("ukey_team_invite_uniq_user_team");

                e.HasOne(m => m.UserInternal)
                    .WithMany(m => m.InvitesInternal)
                    .HasForeignKey(m => m.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fkey_team_invite_user");

                e.HasOne(m => m.TeamInternal)
                    .WithMany(m => m.InvitesInternal)
                    .HasForeignKey(m => m.TeamId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fkey_team_invite_team");
            });

            // Challenge / Hint
            modelBuilder.Entity<PostgresChallengeHint>(e =>
            {
                e.ToTable("challenge_hints");

                e.Property(m => m.Id)
                    .IsRequired()
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                e.Property(m => m.Contents)
                    .IsRequired()
                    .HasColumnName("contents");

                e.Property(m => m.ReleaseTime)
                    .IsRequired()
                    .HasColumnName("release_after")
                    .HasColumnType("interval");

                e.Property(m => m.ChallengeId)
                    .IsRequired()
                    .HasColumnName("challenge_id");

                e.HasOne(m => m.ChallengeInternal)
                    .WithMany(m => m.HintsInternal)
                    .HasForeignKey(m => m.ChallengeId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fkey_challenge_hints_challenge");
            });

            // Challenge / Attachment
            modelBuilder.Entity<PostgresChallengeAttachment>(e =>
            {
                e.ToTable("challenge_attachments")
                    .Ignore(m => m.DownloadUri);

                e.Property(m => m.Id)
                    .IsRequired()
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                e.Property(m => m.Name)
                    .IsRequired()
                    .HasColumnName("filename");

                e.Property(m => m.Type)
                    .IsRequired()
                    .HasColumnName("type");

                e.Property(m => m.Length)
                    .IsRequired()
                    .HasColumnName("size");

                e.Property(m => m.Sha256)
                    .IsRequired()
                    .HasColumnName("sha256");

                e.Property(m => m.Sha1)
                    .IsRequired()
                    .HasColumnName("sha1");

                e.Property(m => m.DownloadUriInternal)
                    .IsRequired()
                    .HasColumnName("download_url");

                e.Property(m => m.ChallengeId)
                    .IsRequired()
                    .HasColumnName("challenge_id");

                e.HasIndex(m => m.Id)
                    .HasName("pkey_challenge_attachments_id");

                e.HasOne(m => m.ChallengeInternal)
                    .WithMany(m => m.AttachmentsInternal)
                    .HasForeignKey(m => m.ChallengeId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fkey_challenge_attachments_challenge");
            });

            // Challenge / Endpoint
            modelBuilder.Entity<PostgresChallengeEndpoint>(e =>
            {
                e.ToTable("challenge_endpoints");

                e.Property(m => m.Id)
                    .IsRequired()
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                e.Property(m => m.Type)
                    .IsRequired()
                    .HasColumnName("type");

                e.Property(m => m.Hostname)
                    .IsRequired()
                    .HasColumnName("hostname");

                e.Property(m => m.Port)
                    .IsRequired()
                    .HasColumnName("port");

                e.HasIndex(m => m.Id)
                    .HasName("pkey_challenge_endpoints_id");
            });

            // Challenge
            modelBuilder.Entity<PostgresChallenge>(e =>
            {
                e.ToTable("challenges")
                    .Ignore(m => m.Attachments)
                    .Ignore(m => m.Endpoint)
                    .Ignore(m => m.Hints)
                    .Ignore(m => m.Category);

                e.Property(m => m.Id)
                    .IsRequired()
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                e.Property(m => m.Title)
                    .IsRequired()
                    .HasColumnName("title");

                e.Property(m => m.CategoryId)
                    .IsRequired()
                    .HasColumnName("category_id");

                e.Property(m => m.Flag)
                    .IsRequired()
                    .HasColumnName("flag");

                e.Property(m => m.Difficulty)
                    .IsRequired()
                    .HasColumnName("difficulty");

                e.Property(m => m.Description)
                    .IsRequired()
                    .HasColumnName("description");

                e.Property(m => m.EndpointId)
                    .HasColumnName("endpoint_id");

                e.Property(m => m.IsHidden)
                    .IsRequired()
                    .HasColumnName("hidden");

                e.Property(m => m.BaseScore)
                    .IsRequired()
                    .HasColumnName("base_score");

                e.HasIndex(m => m.Id)
                    .HasName("pkey_challenge_id");

                e.HasOne(m => m.EndpointInternal)
                    .WithOne(m => m.ChallengeInternal)
                    .HasForeignKey<PostgresChallenge>(m => m.EndpointId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fkey_challenge_endpoint");

                e.HasOne(m => m.CategoryInternal)
                    .WithMany(m => m.ChallengesInternal)
                    .HasForeignKey(m => m.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fkey_challenge_category_challenges");
            });

            // Challenge category
            modelBuilder.Entity<PostgresChallengeCategory>(e =>
            {
                e.ToTable("challenge_categories")
                    .Ignore(m => m.Challenges);

                e.Property(m => m.Id)
                    .IsRequired()
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                e.Property(m => m.Name)
                    .IsRequired()
                    .HasColumnName("name");

                e.Property(m => m.IsHidden)
                    .IsRequired()
                    .HasColumnName("hidden");

                e.Property(m => m.Ordinality)
                    .IsRequired()
                    .HasColumnName("order");

                e.HasKey(m => m.Id)
                    .HasName("challenge_category_id");
            });

            // Challenge solve
            modelBuilder.Entity<PostgresSolveSubmission>(e =>
            {
                e.ToTable("solves")
                    .Ignore(m => m.Challenge)
                    .Ignore(m => m.Team)
                    .Ignore(m => m.User);

                e.Property(m => m.Id)
                    .IsRequired()
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                e.Property(m => m.Contents)
                    .IsRequired()
                    .HasColumnName("contents");

                e.Property(m => m.IsValid)
                    .IsRequired()
                    .HasColumnName("valid");

                e.Property(m => m.ChallengeId)
                    .IsRequired()
                    .HasColumnName("challenge_id");

                e.Property(m => m.UserId)
                    .IsRequired()
                    .HasColumnName("user_id");

                e.Property(m => m.TeamId)
                    .IsRequired()
                    .HasColumnName("team_id");

                e.Property(m => m.Timestamp)
                    .IsRequired()
                    .HasColumnName("timestamp")
                    .HasColumnType("timestamptz");

                e.Property(m => m.Score)
                    .HasColumnName("score");

                e.HasIndex(m => m.Id)
                    .HasName("pkey_solve_id");

                e.HasIndex(m => new { m.ChallengeId, m.TeamId, m.IsValid })
                    .IsUnique(true)
                    .HasFilter("valid = true")
                    .HasName("ix_solve_unique_valids");

                e.HasOne(m => m.ChallengeInternal)
                    .WithMany(m => m.SolvesInternal)
                    .HasForeignKey(m => m.ChallengeId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fkey_solve_challenge");

                e.HasOne(m => m.UserInternal)
                    .WithMany(m => m.SolvesInternal)
                    .HasForeignKey(m => m.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fkey_solve_user");

                e.HasOne(m => m.TeamInternal)
                    .WithMany(m => m.SolvesInternal)
                    .HasForeignKey(m => m.TeamId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fkey_solve_team");
            });

            // Country
            modelBuilder.Entity<PostgresCountry>(e =>
            {
                e.ToTable("countries");

                e.Property(m => m.Code)
                    .IsRequired()
                    .ValueGeneratedNever()
                    .HasColumnName("code");

                e.Property(m => m.Name)
                    .IsRequired()
                    .HasColumnName("name");

                e.HasKey(m => m.Code)
                    .HasName("fkey_country"); // oh shit, that's a mistake; has to remain now, I guess

                e.HasAlternateKey(m => m.Name)
                    .HasName("ukey_country");

                e.HasData(CountrySeedData.SeedCountries);
            });

            // MFA settings
            modelBuilder.Entity<PostgresMfaSettings>(e =>
            {
                e.ToTable("mfa_settings");

                e.Property(m => m.UserId)
                    .IsRequired()
                    .ValueGeneratedNever()
                    .HasColumnName("user_id");

                e.Property(m => m.Secret)
                    .IsRequired()
                    .HasColumnName("secret")
                    .HasColumnType("bytea");

                e.Property(m => m.Digits)
                    .IsRequired()
                    .HasColumnName("digits");

                e.Property(m => m.HmacAlgorithm)
                    .IsRequired()
                    .HasColumnName("hmac");

                e.Property(m => m.Period)
                    .IsRequired()
                    .HasColumnName("period");

                e.Property(m => m.Additional)
                    .HasColumnName("additional")
                    .HasColumnType("bytea")
                    .HasDefaultValue(null);

                e.Property(m => m.Type)
                    .IsRequired()
                    .HasColumnName("type");

                e.Property(m => m.RecoveryCounterBase)
                    .IsRequired()
                    .HasColumnName("recovery_base");

                e.Property(m => m.RecoveryTripCount)
                    .IsRequired()
                    .HasColumnName("recovery_trips");

                e.HasKey(m => m.UserId)
                    .HasName("pkey_mfa_user");

                e.HasOne(m => m.UserInternal)
                    .WithOne(m => m.MfaInternal)
                    .HasForeignKey<PostgresMfaSettings>(m => m.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fkey_mfa_user");
            });
        }
    }
}
