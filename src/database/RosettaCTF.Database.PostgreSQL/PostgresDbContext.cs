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
            // User
            modelBuilder.Entity<PostgresUser>()
                .ToTable("users")
                .Ignore(x => x.DiscordId)
                .Ignore(x => x.Team)
                .Ignore(x => x.AvatarUrl);

            modelBuilder.Entity<PostgresUser>()
                .Property(x => x.Id)
                .IsRequired()
                .ValueGeneratedNever()
                .HasColumnName("id");

            modelBuilder.Entity<PostgresUser>()
                .Property(x => x.Username)
                .IsRequired()
                .HasColumnName("name");

            modelBuilder.Entity<PostgresUser>()
                .Property(x => x.DiscordIdInternal)
                .IsRequired()
                .HasColumnName("discord_id");

            modelBuilder.Entity<PostgresUser>()
                .Property(x => x.AvatarUrlInternal)
                .HasColumnName("avatar")
                .HasColumnType("text")
                .HasDefaultValue(null);

            modelBuilder.Entity<PostgresUser>()
                .Property(x => x.Token)
                .HasColumnName("token")
                .HasDefaultValue(null);

            modelBuilder.Entity<PostgresUser>()
                .Property(x => x.RefreshToken)
                .HasColumnName("refresh_token")
                .HasDefaultValue(null);

            modelBuilder.Entity<PostgresUser>()
                .Property(x => x.TokenExpirationTime)
                .HasColumnName("token_expires_at")
                .HasColumnType("timestamptz")
                .HasDefaultValue(null);

            modelBuilder.Entity<PostgresUser>()
                .Property(x => x.IsAuthorized)
                .IsRequired()
                .HasColumnName("authorized");

            modelBuilder.Entity<PostgresUser>()
                .Property(x => x.TeamId)
                .HasColumnName("team_id")
                .HasDefaultValue(null);

            modelBuilder.Entity<PostgresUser>()
                .HasKey(x => x.Id)
                .HasName("pkey_user_id");

            modelBuilder.Entity<PostgresUser>()
                .HasAlternateKey(x => x.Username)
                .HasName("ukey_user_name");

            modelBuilder.Entity<PostgresUser>()
                .HasOne(x => x.TeamInternal)
                .WithMany(x => x.MembersInternal)
                .HasForeignKey(x => x.TeamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fkey_user_team");

            // Team
            modelBuilder.Entity<PostgresTeam>()
                .ToTable("teams")
                .Ignore(x => x.Members)
                .Ignore(x => x.AvatarUrl);

            modelBuilder.Entity<PostgresTeam>()
                .Property(x => x.Id)
                .IsRequired()
                .ValueGeneratedNever()
                .HasColumnName("id");

            modelBuilder.Entity<PostgresTeam>()
                .Property(x => x.Name)
                .IsRequired()
                .HasColumnName("name");

            modelBuilder.Entity<PostgresTeam>()
                .Property(x => x.AvatarUrlInternal)
                .HasColumnName("avatar")
                .HasColumnType("text")
                .HasDefaultValue(null);

            modelBuilder.Entity<PostgresTeam>()
                .HasKey(x => x.Id)
                .HasName("pkey_team_id");

            modelBuilder.Entity<PostgresTeam>()
                .HasAlternateKey(x => x.Name)
                .HasName("ukey_team_name");
        }
    }
}
