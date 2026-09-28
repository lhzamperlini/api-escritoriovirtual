using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EscritorioVirtual.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWhiteboards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Whiteboards",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    ZoneId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DocumentData = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    LastUpdated = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Guid = table.Column<Guid>(type: "uuid", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: false),
                    UsuarioCriacaoId = table.Column<Guid>(type: "uuid", nullable: true),
                    UsuarioAtualizacaoId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Whiteboards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Whiteboards_Users_UsuarioAtualizacaoId",
                        column: x => x.UsuarioAtualizacaoId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Whiteboards_Users_UsuarioCriacaoId",
                        column: x => x.UsuarioCriacaoId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Whiteboards_UsuarioAtualizacaoId",
                schema: "public",
                table: "Whiteboards",
                column: "UsuarioAtualizacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Whiteboards_UsuarioCriacaoId",
                schema: "public",
                table: "Whiteboards",
                column: "UsuarioCriacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Whiteboards_WorkspaceId",
                schema: "public",
                table: "Whiteboards",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Whiteboards_WorkspaceId_ZoneId",
                schema: "public",
                table: "Whiteboards",
                columns: new[] { "WorkspaceId", "ZoneId" });

            migrationBuilder.CreateIndex(
                name: "IX_Whiteboards_ZoneId",
                schema: "public",
                table: "Whiteboards",
                column: "ZoneId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Whiteboards",
                schema: "public");
        }
    }
}
