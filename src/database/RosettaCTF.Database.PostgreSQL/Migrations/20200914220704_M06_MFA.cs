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

using Microsoft.EntityFrameworkCore.Migrations;
using RosettaCTF.Authentication;

namespace RosettaCTF.Migrations
{
    public partial class M06_MFA : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:ctf_challenge_difficulty", "none,very_easy,easy,medium,hard,very_hard,ultra_nightmare")
                .Annotation("Npgsql:Enum:ctf_challenge_endpoint_type", "unknown,netcat,http,ssh,ssl,https")
                .Annotation("Npgsql:Enum:mfa_hmac", "unknown,md5,sha1,sha256,sha384,sha512")
                .Annotation("Npgsql:Enum:mfa_type", "unknown,google")
                .OldAnnotation("Npgsql:Enum:ctf_challenge_difficulty", "none,very_easy,easy,medium,hard,very_hard,ultra_nightmare")
                .OldAnnotation("Npgsql:Enum:ctf_challenge_endpoint_type", "unknown,netcat,http,ssh,ssl,https");

            migrationBuilder.CreateTable(
                name: "mfa_settings",
                columns: table => new
                {
                    user_id = table.Column<long>(nullable: false),
                    secret = table.Column<byte[]>(type: "bytea", nullable: false),
                    digits = table.Column<int>(nullable: false),
                    hmac = table.Column<MultiFactorHmac>(nullable: false),
                    period = table.Column<int>(nullable: false),
                    additional = table.Column<byte[]>(type: "bytea", nullable: true),
                    type = table.Column<MultiFactorType>(nullable: false),
                    recovery_base = table.Column<long>(nullable: false),
                    recovery_trips = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pkey_mfa_user", x => x.user_id);
                    table.ForeignKey(
                        name: "fkey_mfa_user",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mfa_settings");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:ctf_challenge_difficulty", "none,very_easy,easy,medium,hard,very_hard,ultra_nightmare")
                .Annotation("Npgsql:Enum:ctf_challenge_endpoint_type", "unknown,netcat,http,ssh,ssl,https")
                .OldAnnotation("Npgsql:Enum:ctf_challenge_difficulty", "none,very_easy,easy,medium,hard,very_hard,ultra_nightmare")
                .OldAnnotation("Npgsql:Enum:ctf_challenge_endpoint_type", "unknown,netcat,http,ssh,ssl,https")
                .OldAnnotation("Npgsql:Enum:mfa_hmac", "unknown,md5,sha1,sha256,sha384,sha512")
                .OldAnnotation("Npgsql:Enum:mfa_type", "unknown,google");
        }
    }
}
