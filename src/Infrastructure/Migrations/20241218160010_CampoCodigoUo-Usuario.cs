namespace UsuariosAPI.Infrastructure.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    #nullable disable

    /// <inheritdoc />
    public partial class CampoCodigoUoUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodigoUO",
                table: "UsuarioSRC",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioSRC_CodigoUO",
                table: "UsuarioSRC",
                column: "CodigoUO",
                unique: true,
                filter: "[CodigoUO] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UsuarioSRC_CodigoUO",
                table: "UsuarioSRC");

            migrationBuilder.DropColumn(
                name: "CodigoUO",
                table: "UsuarioSRC");
        }
    }
}
