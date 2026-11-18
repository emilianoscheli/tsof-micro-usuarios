using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsuariosAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LegacyIdUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RolTemporal_LegacyId",
                table: "RolTemporal",
                column: "LegacyId",
                unique: true,
                filter: "[LegacyId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DelegacionTemporal_LegacyId",
                table: "DelegacionTemporal",
                column: "LegacyId",
                unique: true,
                filter: "[LegacyId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RolTemporal_LegacyId",
                table: "RolTemporal");

            migrationBuilder.DropIndex(
                name: "IX_DelegacionTemporal_LegacyId",
                table: "DelegacionTemporal");
        }
    }
}
