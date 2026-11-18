using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsuariosAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UsuariosExternosInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "UsuarioSRC",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "CodigoIncidencia",
                table: "UsuarioSRC",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cuil",
                table: "UsuarioSRC",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EsExterno",
                table: "UsuarioSRC",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NumeroDocumento",
                table: "UsuarioSRC",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaisFkId",
                table: "UsuarioSRC",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelefonoParticular",
                table: "UsuarioSRC",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoDocumentoFkId",
                table: "UsuarioSRC",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activo",
                table: "UsuarioSRC");

            migrationBuilder.DropColumn(
                name: "CodigoIncidencia",
                table: "UsuarioSRC");

            migrationBuilder.DropColumn(
                name: "Cuil",
                table: "UsuarioSRC");

            migrationBuilder.DropColumn(
                name: "EsExterno",
                table: "UsuarioSRC");

            migrationBuilder.DropColumn(
                name: "NumeroDocumento",
                table: "UsuarioSRC");

            migrationBuilder.DropColumn(
                name: "PaisFkId",
                table: "UsuarioSRC");

            migrationBuilder.DropColumn(
                name: "TelefonoParticular",
                table: "UsuarioSRC");

            migrationBuilder.DropColumn(
                name: "TipoDocumentoFkId",
                table: "UsuarioSRC");
        }
    }
}
