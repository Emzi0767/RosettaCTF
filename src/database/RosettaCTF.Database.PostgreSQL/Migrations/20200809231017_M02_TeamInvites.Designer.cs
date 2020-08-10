﻿// This file is part of RosettaCTF project.
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

// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using RosettaCTF.Data;

namespace RosettaCTF.Migrations
{
    [DbContext(typeof(PostgresDbContext))]
    [Migration("20200809231017_M02_TeamInvites")]
    partial class M02_TeamInvites
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:Enum:ctf_challenge_difficulty", "none,very_easy,easy,medium,hard,very_hard,ultra_nightmare")
                .HasAnnotation("Npgsql:Enum:ctf_challenge_endpoint_type", "unknown,netcat,http,ssh,ssl")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("RosettaCTF.Models.PostgresChallenge", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnName("id")
                        .HasColumnType("text");

                    b.Property<int>("BaseScore")
                        .HasColumnName("base_score")
                        .HasColumnType("integer");

                    b.Property<string>("CategoryId")
                        .IsRequired()
                        .HasColumnName("category_id")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnName("description")
                        .HasColumnType("text");

                    b.Property<CtfChallengeDifficulty>("Difficulty")
                        .HasColumnName("difficulty")
                        .HasColumnType("ctf_challenge_difficulty");

                    b.Property<long?>("EndpointId")
                        .HasColumnName("endpoint_id")
                        .HasColumnType("bigint");

                    b.Property<string>("Flag")
                        .IsRequired()
                        .HasColumnName("flag")
                        .HasColumnType("text");

                    b.Property<bool>("IsHidden")
                        .HasColumnName("hidden")
                        .HasColumnType("boolean");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnName("title")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("EndpointId")
                        .IsUnique();

                    b.HasIndex("Id")
                        .HasName("pkey_challenge_id");

                    b.ToTable("challenges");
                });

            modelBuilder.Entity("RosettaCTF.Models.PostgresChallengeAttachment", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ChallengeId")
                        .IsRequired()
                        .HasColumnName("challenge_id")
                        .HasColumnType("text");

                    b.Property<long?>("DecompressedAttachmentId")
                        .HasColumnName("decompressed_id")
                        .HasColumnType("bigint");

                    b.Property<string>("DownloadUriInternal")
                        .HasColumnName("download_url")
                        .HasColumnType("text");

                    b.Property<long>("Length")
                        .HasColumnName("size")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("filename")
                        .HasColumnType("text");

                    b.Property<string>("Sha1")
                        .IsRequired()
                        .HasColumnName("sha1")
                        .HasColumnType("text");

                    b.Property<string>("Sha256")
                        .IsRequired()
                        .HasColumnName("sha256")
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnName("type")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ChallengeId");

                    b.HasIndex("DecompressedAttachmentId")
                        .IsUnique();

                    b.HasIndex("Id")
                        .HasName("pkey_challenge_attachments_id");

                    b.ToTable("challenge_attachments");
                });

            modelBuilder.Entity("RosettaCTF.Models.PostgresChallengeCategory", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnName("id")
                        .HasColumnType("text");

                    b.Property<bool>("IsHidden")
                        .HasColumnName("hidden")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("name")
                        .HasColumnType("text");

                    b.HasKey("Id")
                        .HasName("challenge_category_id");

                    b.ToTable("challenge_categories");
                });

            modelBuilder.Entity("RosettaCTF.Models.PostgresChallengeEndpoint", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Hostname")
                        .IsRequired()
                        .HasColumnName("hostname")
                        .HasColumnType("text");

                    b.Property<int>("Port")
                        .HasColumnName("port")
                        .HasColumnType("integer");

                    b.Property<CtfChallengeEndpointType>("Type")
                        .HasColumnName("type")
                        .HasColumnType("ctf_challenge_endpoint_type");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasName("pkey_challenge_endpoints_id");

                    b.ToTable("challenge_endpoints");
                });

            modelBuilder.Entity("RosettaCTF.Models.PostgresChallengeHint", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ChallengeId")
                        .IsRequired()
                        .HasColumnName("challenge_id")
                        .HasColumnType("text");

                    b.Property<string>("Contents")
                        .IsRequired()
                        .HasColumnName("contents")
                        .HasColumnType("text");

                    b.Property<TimeSpan>("ReleaseTime")
                        .HasColumnName("release_after")
                        .HasColumnType("interval");

                    b.HasKey("Id");

                    b.HasIndex("ChallengeId");

                    b.ToTable("challenge_hints");
                });

            modelBuilder.Entity("RosettaCTF.Models.PostgresSolveSubmission", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ChallengeId")
                        .IsRequired()
                        .HasColumnName("challenge_id")
                        .HasColumnType("text");

                    b.Property<string>("Contents")
                        .IsRequired()
                        .HasColumnName("contents")
                        .HasColumnType("text");

                    b.Property<bool>("IsValid")
                        .HasColumnName("valid")
                        .HasColumnType("boolean");

                    b.Property<long>("TeamId")
                        .HasColumnName("team_id")
                        .HasColumnType("bigint");

                    b.Property<DateTimeOffset>("Timestamp")
                        .HasColumnName("timestamp")
                        .HasColumnType("timestamptz");

                    b.Property<long>("UserId")
                        .HasColumnName("user_id")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasName("pkey_solve_id");

                    b.HasIndex("TeamId");

                    b.HasIndex("UserId");

                    b.HasIndex("ChallengeId", "TeamId", "IsValid")
                        .IsUnique()
                        .HasName("ix_solve_unique_valids")
                        .HasFilter("valid = true");

                    b.ToTable("solves");
                });

            modelBuilder.Entity("RosettaCTF.Models.PostgresTeam", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnName("id")
                        .HasColumnType("bigint");

                    b.Property<string>("AvatarUrlInternal")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("avatar")
                        .HasColumnType("text")
                        .HasDefaultValue(null);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("name")
                        .HasColumnType("text");

                    b.HasKey("Id")
                        .HasName("pkey_team_id");

                    b.HasAlternateKey("Name")
                        .HasName("ukey_team_name");

                    b.ToTable("teams");
                });

            modelBuilder.Entity("RosettaCTF.Models.PostgresTeamInvite", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<long>("TeamId")
                        .HasColumnName("team_id")
                        .HasColumnType("bigint");

                    b.Property<long>("UserId")
                        .HasColumnName("user_id")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasAlternateKey("UserId", "TeamId")
                        .HasName("ukey_team_invite_uniq_user_team");

                    b.HasIndex("Id")
                        .HasName("pkey_team_invite_id");

                    b.HasIndex("TeamId");

                    b.ToTable("team_invites");
                });

            modelBuilder.Entity("RosettaCTF.Models.PostgresUser", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnName("id")
                        .HasColumnType("bigint");

                    b.Property<string>("AvatarUrlInternal")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("avatar")
                        .HasColumnType("text")
                        .HasDefaultValue(null);

                    b.Property<long>("DiscordIdInternal")
                        .HasColumnName("discord_id")
                        .HasColumnType("bigint");

                    b.Property<bool>("HasHiddenAccess")
                        .HasColumnName("access_hidden")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsAuthorized")
                        .HasColumnName("authorized")
                        .HasColumnType("boolean");

                    b.Property<string>("RefreshToken")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("refresh_token")
                        .HasColumnType("text")
                        .HasDefaultValue(null);

                    b.Property<long?>("TeamId")
                        .HasColumnName("team_id")
                        .HasColumnType("bigint")
                        .HasDefaultValue(null);

                    b.Property<string>("Token")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("token")
                        .HasColumnType("text")
                        .HasDefaultValue(null);

                    b.Property<DateTimeOffset?>("TokenExpirationTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("token_expires_at")
                        .HasColumnType("timestamptz")
                        .HasDefaultValue(null);

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnName("name")
                        .HasColumnType("text");

                    b.HasKey("Id")
                        .HasName("pkey_user_id");

                    b.HasAlternateKey("Username")
                        .HasName("ukey_user_name");

                    b.HasIndex("TeamId");

                    b.ToTable("users");
                });

            modelBuilder.Entity("RosettaCTF.Models.PostgresChallenge", b =>
                {
                    b.HasOne("RosettaCTF.Models.PostgresChallengeCategory", "CategoryInternal")
                        .WithMany("ChallengesInternal")
                        .HasForeignKey("CategoryId")
                        .HasConstraintName("fkey_challenge_category_challenges")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RosettaCTF.Models.PostgresChallengeEndpoint", "EndpointInternal")
                        .WithOne("ChallengeInternal")
                        .HasForeignKey("RosettaCTF.Models.PostgresChallenge", "EndpointId")
                        .HasConstraintName("fkey_challenge_endpoint");
                });

            modelBuilder.Entity("RosettaCTF.Models.PostgresChallengeAttachment", b =>
                {
                    b.HasOne("RosettaCTF.Models.PostgresChallenge", "ChallengeInternal")
                        .WithMany("AttachmentsInternal")
                        .HasForeignKey("ChallengeId")
                        .HasConstraintName("fkey_challenge_attachments_challenge")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RosettaCTF.Models.PostgresChallengeAttachment", "DecompressedAttachmentInternal")
                        .WithOne("CompressedAttachmentInternal")
                        .HasForeignKey("RosettaCTF.Models.PostgresChallengeAttachment", "DecompressedAttachmentId")
                        .HasConstraintName("fkey_challenge_attachments_decompressed");
                });

            modelBuilder.Entity("RosettaCTF.Models.PostgresChallengeHint", b =>
                {
                    b.HasOne("RosettaCTF.Models.PostgresChallenge", "ChallengeInternal")
                        .WithMany("HintsInternal")
                        .HasForeignKey("ChallengeId")
                        .HasConstraintName("fkey_challenge_hints_challenge")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RosettaCTF.Models.PostgresSolveSubmission", b =>
                {
                    b.HasOne("RosettaCTF.Models.PostgresChallenge", "ChallengeInternal")
                        .WithMany("SolvesInternal")
                        .HasForeignKey("ChallengeId")
                        .HasConstraintName("fkey_solve_challenge")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RosettaCTF.Models.PostgresTeam", "TeamInternal")
                        .WithMany("SolvesInternal")
                        .HasForeignKey("TeamId")
                        .HasConstraintName("fkey_solve_team")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RosettaCTF.Models.PostgresUser", "UserInternal")
                        .WithMany("SolvesInternal")
                        .HasForeignKey("UserId")
                        .HasConstraintName("fkey_solve_user")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RosettaCTF.Models.PostgresTeamInvite", b =>
                {
                    b.HasOne("RosettaCTF.Models.PostgresTeam", "TeamInternal")
                        .WithMany("InvitesInternal")
                        .HasForeignKey("TeamId")
                        .HasConstraintName("fkey_team_invite_team")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RosettaCTF.Models.PostgresUser", "UserInternal")
                        .WithMany("InvitesInternal")
                        .HasForeignKey("UserId")
                        .HasConstraintName("fkey_team_invite_user")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RosettaCTF.Models.PostgresUser", b =>
                {
                    b.HasOne("RosettaCTF.Models.PostgresTeam", "TeamInternal")
                        .WithMany("MembersInternal")
                        .HasForeignKey("TeamId")
                        .HasConstraintName("fkey_user_team");
                });
#pragma warning restore 612, 618
        }
    }
}
