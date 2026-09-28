using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EscritorioVirtual.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateTable_Workspace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Workspaces",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Identificador UUID do Workspace / Não Nulo / LGPD: dado não sensível"),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, comment: "Nome da empresa / organização / Não Nulo / LGPD: dado não sensível"),
                    Slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, comment: "Slug amigável para URL / Não Nulo / LGPD: dado não sensível"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Data de criação / Não Nulo / LGPD: dado não sensível"),
                    Guid = table.Column<Guid>(type: "uuid", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    DataAtualizacao = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    UsuarioCriacaoId = table.Column<Guid>(type: "uuid", nullable: true),
                    UsuarioAtualizacaoId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workspaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workspaces_Users_UsuarioAtualizacaoId",
                        column: x => x.UsuarioAtualizacaoId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Workspaces_Users_UsuarioCriacaoId",
                        column: x => x.UsuarioCriacaoId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Tabela responsável por armazenar workspaces/organizações da plataforma");

            migrationBuilder.CreateTable(
                name: "WorkspaceInvites",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Identificador UUID do convite / Não Nulo / LGPD: dado não sensível"),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false, comment: "ID do Workspace / Não Nulo / LGPD: dado não sensível"),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "Código único do convite / Não Nulo / LGPD: dado não sensível"),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, comment: "E-mail destinatário (opcional) / Nulo / LGPD: dado não sensível"),
                    RoleId = table.Column<int>(type: "integer", nullable: false, comment: "Papel atribuído / Não Nulo / LGPD: dado não sensível"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Data de geração do convite / Não Nulo / LGPD: dado não sensível"),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false, comment: "ID do usuário criador / Não Nulo / LGPD: dado não sensível"),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, comment: "Data de expiração / Nulo / LGPD: dado não sensível"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true, comment: "Status ativo / Não Nulo / LGPD: dado não sensível")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkspaceInvites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkspaceInvites_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkspaceInvites_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalSchema: "public",
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Tabela responsável por armazenar convites para acesso ao Workspace");

            migrationBuilder.CreateTable(
                name: "WorkspaceUsers",
                schema: "public",
                columns: table => new
                {
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false, comment: "ID do Workspace / Não Nulo / LGPD: dado não sensível"),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false, comment: "ID do Usuário / Não Nulo / LGPD: dado não sensível"),
                    RoleId = table.Column<int>(type: "integer", nullable: false, comment: "Papel do usuário (1=Owner, 2=Admin, 3=Member, 4=Guest) / Não Nulo / LGPD: dado não sensível"),
                    JoinedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP", comment: "Data de ingresso no Workspace / Não Nulo / LGPD: dado não sensível")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkspaceUsers", x => new { x.WorkspaceId, x.UserId });
                    table.ForeignKey(
                        name: "FK_WorkspaceUsers_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkspaceUsers_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalSchema: "public",
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Tabela responsável por armazenar vínculo entre usuários e workspaces");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceInvites_Code",
                schema: "public",
                table: "WorkspaceInvites",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceInvites_CreatedByUserId",
                schema: "public",
                table: "WorkspaceInvites",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceInvites_WorkspaceId",
                schema: "public",
                table: "WorkspaceInvites",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Workspaces_Slug",
                schema: "public",
                table: "Workspaces",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Workspaces_UsuarioAtualizacaoId",
                schema: "public",
                table: "Workspaces",
                column: "UsuarioAtualizacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Workspaces_UsuarioCriacaoId",
                schema: "public",
                table: "Workspaces",
                column: "UsuarioCriacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceUsers_UserId",
                schema: "public",
                table: "WorkspaceUsers",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkspaceInvites",
                schema: "public");

            migrationBuilder.DropTable(
                name: "WorkspaceUsers",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Workspaces",
                schema: "public");
        }
    }
}
