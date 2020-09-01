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
using Microsoft.EntityFrameworkCore.Migrations;

namespace RosettaCTF.Migrations
{
    public partial class M04_OAuth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "discord_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "refresh_token",
                table: "users");

            migrationBuilder.DropColumn(
                name: "token",
                table: "users");

            migrationBuilder.DropColumn(
                name: "token_expires_at",
                table: "users");

            migrationBuilder.CreateTable(
                name: "passwords",
                columns: table => new
                {
                    name = table.Column<long>(nullable: false),
                    hash = table.Column<byte[]>(type: "bytea", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pkey_user_pwd_id", x => x.name);
                    table.ForeignKey(
                        name: "fkey_user_pwd",
                        column: x => x.name,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users_oauth",
                columns: table => new
                {
                    provider = table.Column<string>(nullable: false),
                    user = table.Column<long>(nullable: false),
                    id = table.Column<string>(nullable: false),
                    username = table.Column<string>(nullable: false),
                    token = table.Column<string>(nullable: true),
                    refresh_token = table.Column<string>(nullable: true),
                    token_expires_at = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pkey_user_oauth_id_provider", x => new { x.user, x.provider });
                    table.UniqueConstraint("ukey_user_oauth_name_provider", x => new { x.id, x.provider });
                    table.ForeignKey(
                        name: "fkey_user_oauth_user",
                        column: x => x.user,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "passwords");

            migrationBuilder.DropTable(
                name: "users_oauth");

            migrationBuilder.AddColumn<long>(
                name: "discord_id",
                table: "users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "refresh_token",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "token",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "token_expires_at",
                table: "users",
                type: "timestamptz",
                nullable: true);
        }
    }
}
