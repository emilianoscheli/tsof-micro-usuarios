namespace UsuariosAPI.Infrastructure.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    #nullable disable

    /// <inheritdoc />
    public partial class CodigoUONoUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UsuarioSRC_CodigoUO",
                table: "UsuarioSRC");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UsuarioSRC_CodigoUO",
                table: "UsuarioSRC",
                column: "CodigoUO",
                unique: true,
                filter: "[CodigoUO] IS NOT NULL");
        }
    }
}
