using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RosettaCTF.Migrations
{
    public partial class M02_Challenges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:ctf_challenge_difficulty", "none,very_easy,easy,medium,hard,very_hard,ultra_nightmare")
                .Annotation("Npgsql:Enum:ctf_challenge_endpoint_type", "unknown,netcat,http,ssh,ssl");

            migrationBuilder.CreateTable(
                name: "challenge_categories",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    name = table.Column<string>(nullable: false),
                    hidden = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("challenge_category_id", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "challenge_endpoints",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type = table.Column<int>(nullable: false),
                    hostname = table.Column<string>(nullable: false),
                    port = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_challenge_endpoints", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "challenges",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    title = table.Column<string>(nullable: false),
                    category_id = table.Column<string>(nullable: false),
                    flag = table.Column<string>(nullable: false),
                    difficulty = table.Column<int>(nullable: false),
                    description = table.Column<string>(nullable: false),
                    endpoint_id = table.Column<long>(nullable: true),
                    hidden = table.Column<bool>(nullable: false),
                    base_score = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_challenges", x => x.id);
                    table.ForeignKey(
                        name: "fkey_challenge_category_challenges",
                        column: x => x.category_id,
                        principalTable: "challenge_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fkey_challenge_endpoint",
                        column: x => x.endpoint_id,
                        principalTable: "challenge_endpoints",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "challenge_attachments",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    filename = table.Column<string>(nullable: false),
                    type = table.Column<string>(nullable: false),
                    size = table.Column<long>(nullable: false),
                    sha256 = table.Column<string>(nullable: false),
                    sha1 = table.Column<string>(nullable: false),
                    download_url = table.Column<string>(nullable: false),
                    decompressed_id = table.Column<long>(nullable: true),
                    challenge_id = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_challenge_attachments", x => x.id);
                    table.ForeignKey(
                        name: "fkey_challenge_attachments_challenge",
                        column: x => x.challenge_id,
                        principalTable: "challenges",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fkey_challenge_attachments_decompressed",
                        column: x => x.decompressed_id,
                        principalTable: "challenge_attachments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "challenge_hints",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    contents = table.Column<string>(nullable: false),
                    release_after = table.Column<TimeSpan>(type: "interval", nullable: false),
                    challenge_id = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_challenge_hints", x => x.id);
                    table.ForeignKey(
                        name: "fkey_challenge_hints_challenge",
                        column: x => x.challenge_id,
                        principalTable: "challenges",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_challenge_attachments_challenge_id",
                table: "challenge_attachments",
                column: "challenge_id");

            migrationBuilder.CreateIndex(
                name: "IX_challenge_attachments_decompressed_id",
                table: "challenge_attachments",
                column: "decompressed_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "pkey_challenge_attachments_id",
                table: "challenge_attachments",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "pkey_challenge_endpoints_id",
                table: "challenge_endpoints",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_challenge_hints_challenge_id",
                table: "challenge_hints",
                column: "challenge_id");

            migrationBuilder.CreateIndex(
                name: "IX_challenges_category_id",
                table: "challenges",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_challenges_endpoint_id",
                table: "challenges",
                column: "endpoint_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "pkey_challenge_id",
                table: "challenges",
                column: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "challenge_attachments");

            migrationBuilder.DropTable(
                name: "challenge_hints");

            migrationBuilder.DropTable(
                name: "challenges");

            migrationBuilder.DropTable(
                name: "challenge_categories");

            migrationBuilder.DropTable(
                name: "challenge_endpoints");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:Enum:ctf_challenge_difficulty", "none,very_easy,easy,medium,hard,very_hard,ultra_nightmare")
                .OldAnnotation("Npgsql:Enum:ctf_challenge_endpoint_type", "unknown,netcat,http,ssh,ssl");
        }
    }
}
