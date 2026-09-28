using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EscritorioVirtual.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAvatarConfigToUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarConfig",
                schema: "public",
                table: "Users",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarConfig",
                schema: "public",
                table: "Users");
        }
    }
}
