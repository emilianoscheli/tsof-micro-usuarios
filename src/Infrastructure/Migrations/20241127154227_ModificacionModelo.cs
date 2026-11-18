#nullable disable
namespace UsuariosAPI.Infrastructure.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    public partial class ModificacionModelo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsuarioRed",
                table: "UsuarioRol");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsuarioRed",
                table: "UsuarioRol",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: string.Empty);
        }
    }
}
