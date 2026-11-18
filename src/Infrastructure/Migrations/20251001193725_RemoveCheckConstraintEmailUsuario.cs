using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsuariosAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCheckConstraintEmailUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_UsuarioSRC_CorreoElectronico",
                table: "UsuarioSRC");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_UsuarioSRC_CorreoElectronico",
                table: "UsuarioSRC",
                sql: "CorreoElectronico LIKE '%@%' AND CorreoElectronico NOT LIKE '%[^a-zA-Z0-9@.]%'");
        }
    }
}
