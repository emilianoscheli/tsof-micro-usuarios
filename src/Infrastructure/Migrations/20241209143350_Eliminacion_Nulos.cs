namespace UsuariosAPI.Infrastructure.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    #nullable disable

    /// <inheritdoc />
    public partial class Eliminacion_Nulos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UsuarioSociedad_UsuarioSRCId_SociedadId",
                table: "UsuarioSociedad");

            migrationBuilder.DropIndex(
                name: "IX_UsuarioRol_UsuarioSRCId_RolId",
                table: "UsuarioRol");

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioSRCId",
                table: "UsuarioSociedad",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SociedadId",
                table: "UsuarioSociedad",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioSRCId",
                table: "UsuarioRol",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RolId",
                table: "UsuarioRol",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioSociedad_UsuarioSRCId_SociedadId",
                table: "UsuarioSociedad",
                columns: new[] { "UsuarioSRCId", "SociedadId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRol_UsuarioSRCId_RolId",
                table: "UsuarioRol",
                columns: new[] { "UsuarioSRCId", "RolId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UsuarioSociedad_UsuarioSRCId_SociedadId",
                table: "UsuarioSociedad");

            migrationBuilder.DropIndex(
                name: "IX_UsuarioRol_UsuarioSRCId_RolId",
                table: "UsuarioRol");

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioSRCId",
                table: "UsuarioSociedad",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "SociedadId",
                table: "UsuarioSociedad",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioSRCId",
                table: "UsuarioRol",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "RolId",
                table: "UsuarioRol",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioSociedad_UsuarioSRCId_SociedadId",
                table: "UsuarioSociedad",
                columns: new[] { "UsuarioSRCId", "SociedadId" },
                unique: true,
                filter: "[UsuarioSRCId] IS NOT NULL AND [SociedadId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRol_UsuarioSRCId_RolId",
                table: "UsuarioRol",
                columns: new[] { "UsuarioSRCId", "RolId" },
                unique: true,
                filter: "[UsuarioSRCId] IS NOT NULL AND [RolId] IS NOT NULL");
        }
    }
}
