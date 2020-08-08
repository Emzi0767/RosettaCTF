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
using RosettaCTF.Data;
using RosettaCTF.Models;

namespace RosettaCTF
{
    internal sealed class PostgresDbContext : DbContext
    {
        public DbSet<PostgresUser> Users { get; set; }
        public DbSet<PostgresTeam> Teams { get; set; }

        public PostgresDbContext(DbContextOptions<PostgresDbContext> opts)
            : base(opts)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Map enums
            modelBuilder.HasPostgresEnum<CtfChallengeDifficulty>();
            modelBuilder.HasPostgresEnum<CtfChallengeEndpointType>();

            // User
            modelBuilder.Entity<PostgresUser>(e =>
            {
                e.ToTable("users")
                    .Ignore(m => m.DiscordId)
                    .Ignore(m => m.Team)
                    .Ignore(m => m.AvatarUrl);

                e.Property(m => m.Id)
                    .IsRequired()
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                e.Property(m => m.Username)
                    .IsRequired()
                    .HasColumnName("name");

                e.Property(m => m.DiscordIdInternal)
                    .IsRequired()
                    .HasColumnName("discord_id");

                e.Property(m => m.AvatarUrlInternal)
                    .HasColumnName("avatar")
                    .HasColumnType("text")
                    .HasDefaultValue(null);

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

                e.Property(m => m.IsAuthorized)
                    .IsRequired()
                    .HasColumnName("authorized");

                e.Property(m => m.TeamId)
                    .HasColumnName("team_id")
                    .HasDefaultValue(null);

                e.HasKey(m => m.Id)
                    .HasName("pkey_user_id");

                e.HasAlternateKey(m => m.Username)
                    .HasName("ukey_user_name");

                e.HasOne(m => m.TeamInternal)
                    .WithMany(m => m.MembersInternal)
                    .HasForeignKey(m => m.TeamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fkey_user_team");
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
                    .Ignore(m => m.DecompressedAttachment)
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

                e.Property(m => m.DecompressedAttachmentId)
                    .HasColumnName("decompressed_id")
                    .ValueGeneratedNever();

                e.Property(m => m.ChallengeId)
                    .IsRequired()
                    .HasColumnName("challenge_id");

                e.HasIndex(m => m.Id)
                    .HasName("pkey_challenge_attachments_id");

                e.HasOne(m => m.DecompressedAttachmentInternal)
                    .WithOne(m => m.CompressedAttachmentInternal)
                    .HasForeignKey<PostgresChallengeAttachment>(m => m.DecompressedAttachmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fkey_challenge_attachments_decompressed");

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

                e.HasKey(m => m.Id)
                    .HasName("challenge_category_id");
            });
        }
    }
}
