using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsuariosAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DelegacionTemporal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IdRed",
                table: "UsuarioSRC",
                type: "varchar(10)",
                unicode: false,
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(7)",
                oldUnicode: false,
                oldMaxLength: 7);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_UsuarioSRC_IdRed",
                table: "UsuarioSRC",
                column: "IdRed");

            migrationBuilder.CreateTable(
                name: "DelegacionTemporal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DelegadorId = table.Column<int>(type: "int", nullable: false),
                    DelegadoId = table.Column<int>(type: "int", nullable: false),
                    DelegadorIdRed = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DelegadoIdRed = table.Column<string>(type: "varchar(10)", nullable: false),
                    NombreCompletoDelegador = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NombreCompletoDelegado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    TiempoLimite = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaDesde = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaHasta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    NombreUsuario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LegacyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DelegacionTemporal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DelegacionTemporal_UsuarioSRC_DelegadoIdRed",
                        column: x => x.DelegadoIdRed,
                        principalTable: "UsuarioSRC",
                        principalColumn: "IdRed",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DelegacionTemporal_UsuarioSRC_DelegadorId",
                        column: x => x.DelegadorId,
                        principalTable: "UsuarioSRC",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RolTemporal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DelegacionTemporalId = table.Column<int>(type: "int", nullable: false),
                    RolId = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    NombreUsuario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LegacyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolTemporal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolTemporal_DelegacionTemporal_DelegacionTemporalId",
                        column: x => x.DelegacionTemporalId,
                        principalTable: "DelegacionTemporal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolTemporal_Rol_RolId",
                        column: x => x.RolId,
                        principalTable: "Rol",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DelegacionTemporal_DelegadoIdRed",
                table: "DelegacionTemporal",
                column: "DelegadoIdRed");

            migrationBuilder.CreateIndex(
                name: "IX_DelegacionTemporal_DelegadorId",
                table: "DelegacionTemporal",
                column: "DelegadorId");

            migrationBuilder.CreateIndex(
                name: "IX_RolTemporal_DelegacionTemporalId",
                table: "RolTemporal",
                column: "DelegacionTemporalId");

            migrationBuilder.CreateIndex(
                name: "IX_RolTemporal_RolId",
                table: "RolTemporal",
                column: "RolId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RolTemporal");

            migrationBuilder.DropTable(
                name: "DelegacionTemporal");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_UsuarioSRC_IdRed",
                table: "UsuarioSRC");

            migrationBuilder.AlterColumn<string>(
                name: "IdRed",
                table: "UsuarioSRC",
                type: "varchar(7)",
                unicode: false,
                maxLength: 7,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldUnicode: false,
                oldMaxLength: 10);
        }
    }
}
