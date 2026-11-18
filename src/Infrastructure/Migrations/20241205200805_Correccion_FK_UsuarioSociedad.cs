#nullable disable
namespace UsuariosAPI.Infrastructure.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    public partial class Correccion_FK_UsuarioSociedad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioSociedad_UsuarioSRC_UsuarioId",
                table: "UsuarioSociedad");

            migrationBuilder.DropIndex(
                name: "IX_UsuarioSociedad_UsuarioId_SociedadId",
                table: "UsuarioSociedad");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioSociedad_UsuarioSRCId_SociedadId",
                table: "UsuarioSociedad",
                columns: new[] { "UsuarioSRCId", "SociedadId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioSociedad_UsuarioSRC_UsuarioSRCId",
                table: "UsuarioSociedad",
                column: "UsuarioSRCId",
                principalTable: "UsuarioSRC",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioSociedad_UsuarioSRC_UsuarioSRCId",
                table: "UsuarioSociedad");

            migrationBuilder.DropIndex(
                name: "IX_UsuarioSociedad_UsuarioSRCId_SociedadId",
                table: "UsuarioSociedad");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioSociedad_UsuarioId_SociedadId",
                table: "UsuarioSociedad",
                columns: new[] { "UsuarioId", "SociedadId" },
                unique: true,
                filter: "[UsuarioId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioSociedad_UsuarioSRC_UsuarioId",
                table: "UsuarioSociedad",
                column: "UsuarioId",
                principalTable: "UsuarioSRC",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
