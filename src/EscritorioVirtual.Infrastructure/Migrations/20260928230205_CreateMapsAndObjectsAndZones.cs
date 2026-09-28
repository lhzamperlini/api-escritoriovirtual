using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EscritorioVirtual.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateMapsAndObjectsAndZones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Maps",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    GridWidth = table.Column<int>(type: "integer", nullable: false, defaultValue: 100),
                    GridHeight = table.Column<int>(type: "integer", nullable: false, defaultValue: 100),
                    TileSize = table.Column<int>(type: "integer", nullable: false, defaultValue: 32),
                    TiledMapData = table.Column<string>(type: "jsonb", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Guid = table.Column<Guid>(type: "uuid", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: false),
                    UsuarioCriacaoId = table.Column<Guid>(type: "uuid", nullable: true),
                    UsuarioAtualizacaoId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Maps_Users_UsuarioAtualizacaoId",
                        column: x => x.UsuarioAtualizacaoId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Maps_Users_UsuarioCriacaoId",
                        column: x => x.UsuarioCriacaoId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MapObjects",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MapId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CoordX = table.Column<int>(type: "integer", nullable: false),
                    CoordY = table.Column<int>(type: "integer", nullable: false),
                    Rotation = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsSolid = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    ZIndexOffset = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapObjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MapObjects_Maps_MapId",
                        column: x => x.MapId,
                        principalSchema: "public",
                        principalTable: "Maps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MapZones",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MapId = table.Column<Guid>(type: "uuid", nullable: false),
                    ZoneType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StartX = table.Column<int>(type: "integer", nullable: false),
                    StartY = table.Column<int>(type: "integer", nullable: false),
                    EndX = table.Column<int>(type: "integer", nullable: false),
                    EndY = table.Column<int>(type: "integer", nullable: false),
                    Capacity = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapZones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MapZones_Maps_MapId",
                        column: x => x.MapId,
                        principalSchema: "public",
                        principalTable: "Maps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MapObjects_MapId",
                schema: "public",
                table: "MapObjects",
                column: "MapId");

            migrationBuilder.CreateIndex(
                name: "IX_Maps_UsuarioAtualizacaoId",
                schema: "public",
                table: "Maps",
                column: "UsuarioAtualizacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Maps_UsuarioCriacaoId",
                schema: "public",
                table: "Maps",
                column: "UsuarioCriacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_MapZones_MapId",
                schema: "public",
                table: "MapZones",
                column: "MapId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MapObjects",
                schema: "public");

            migrationBuilder.DropTable(
                name: "MapZones",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Maps",
                schema: "public");
        }
    }
}
