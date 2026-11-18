using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsuariosAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompatibilidadRol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompatibilidadRol",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RolOrigenId = table.Column<int>(type: "int", nullable: false),
                    RolDestinoId = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    NombreUsuario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompatibilidadRol", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompatibilidadRol_Rol_RolOrigenId",
                        column: x => x.RolOrigenId,
                        principalTable: "Rol",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompatibilidadRol_Rol_RolDestinoId",
                        column: x => x.RolDestinoId,
                        principalTable: "Rol",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompatibilidadRol_RolOrigenId_RolDestinoId",
                table: "CompatibilidadRol",
                columns: new[] { "RolOrigenId", "RolDestinoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompatibilidadRol_RolDestinoId",
                table: "CompatibilidadRol",
                column: "RolDestinoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompatibilidadRol");
        }
    }
}
