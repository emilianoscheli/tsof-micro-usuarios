using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsuariosAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixNvarsModelos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DelegacionTemporal_UsuarioSRC_DelegadoIdRed",
                table: "DelegacionTemporal");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_UsuarioSRC_IdRed",
                table: "UsuarioSRC");

            migrationBuilder.AlterColumn<string>(
                name: "NombreCompleto",
                table: "UsuarioSRC",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "IdRed",
                table: "UsuarioSRC",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldUnicode: false,
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Rol",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "DelegadoIdRed",
                table: "DelegacionTemporal",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_UsuarioSRC_IdRed",
                table: "UsuarioSRC",
                column: "IdRed");

            migrationBuilder.AddForeignKey(
                name: "FK_DelegacionTemporal_UsuarioSRC_DelegadoIdRed",
                table: "DelegacionTemporal",
                column: "DelegadoIdRed",
                principalTable: "UsuarioSRC",
                principalColumn: "IdRed",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DelegacionTemporal_UsuarioSRC_DelegadoIdRed",
                table: "DelegacionTemporal");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_UsuarioSRC_IdRed",
                table: "UsuarioSRC");

            migrationBuilder.AlterColumn<string>(
                name: "NombreCompleto",
                table: "UsuarioSRC",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "IdRed",
                table: "UsuarioSRC",
                type: "varchar(10)",
                unicode: false,
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Rol",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "DelegadoIdRed",
                table: "DelegacionTemporal",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_UsuarioSRC_IdRed",
                table: "UsuarioSRC",
                column: "IdRed");

            migrationBuilder.AddForeignKey(
                name: "FK_DelegacionTemporal_UsuarioSRC_DelegadoIdRed",
                table: "DelegacionTemporal",
                column: "DelegadoIdRed",
                principalTable: "UsuarioSRC",
                principalColumn: "IdRed",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
