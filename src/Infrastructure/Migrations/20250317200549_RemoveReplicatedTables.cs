using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsuariosAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveReplicatedTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioSociedad_Sociedad_SociedadId",
                table: "UsuarioSociedad");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioSRC_Contratista_ContratistaId",
                table: "UsuarioSRC");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioSRC_EstudioAuditor_EstudioAuditorId",
                table: "UsuarioSRC");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioSRC_Sede_SedeId",
                table: "UsuarioSRC");

            migrationBuilder.DropTable(
                name: "Contratista");

            migrationBuilder.DropTable(
                name: "Sede");

            migrationBuilder.DropTable(
                name: "Sociedad");

            migrationBuilder.DropTable(
                name: "EstudioAuditor");

            migrationBuilder.DropIndex(
                name: "IX_UsuarioSRC_ContratistaId",
                table: "UsuarioSRC");

            migrationBuilder.DropIndex(
                name: "IX_UsuarioSRC_EstudioAuditorId",
                table: "UsuarioSRC");

            migrationBuilder.DropIndex(
                name: "IX_UsuarioSRC_SedeId",
                table: "UsuarioSRC");

            migrationBuilder.DropIndex(
                name: "IX_UsuarioSociedad_SociedadId",
                table: "UsuarioSociedad");

            migrationBuilder.RenameColumn(
                name: "SedeId",
                table: "UsuarioSRC",
                newName: "SedeFkId");

            migrationBuilder.RenameColumn(
                name: "EstudioAuditorId",
                table: "UsuarioSRC",
                newName: "EstudioAuditorFkId");

            migrationBuilder.RenameColumn(
                name: "ContratistaId",
                table: "UsuarioSRC",
                newName: "ContratistaFkId");

            migrationBuilder.RenameColumn(
                name: "SociedadId",
                table: "UsuarioSociedad",
                newName: "SociedadFkId");

            migrationBuilder.RenameIndex(
                name: "IX_UsuarioSociedad_UsuarioSRCId_SociedadId",
                table: "UsuarioSociedad",
                newName: "IX_UsuarioSociedad_UsuarioSRCId_SociedadFkId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SedeFkId",
                table: "UsuarioSRC",
                newName: "SedeId");

            migrationBuilder.RenameColumn(
                name: "EstudioAuditorFkId",
                table: "UsuarioSRC",
                newName: "EstudioAuditorId");

            migrationBuilder.RenameColumn(
                name: "ContratistaFkId",
                table: "UsuarioSRC",
                newName: "ContratistaId");

            migrationBuilder.RenameColumn(
                name: "SociedadFkId",
                table: "UsuarioSociedad",
                newName: "SociedadId");

            migrationBuilder.RenameIndex(
                name: "IX_UsuarioSociedad_UsuarioSRCId_SociedadFkId",
                table: "UsuarioSociedad",
                newName: "IX_UsuarioSociedad_UsuarioSRCId_SociedadId");

            migrationBuilder.CreateTable(
                name: "Contratista",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BdOrigenId = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false),
                    NombreUsuario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RazonSocial = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contratista", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EstudioAuditor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BdOrigenId = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    NombreUsuario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UsuarioId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstudioAuditor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sociedad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BdOrigenId = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    NombreUsuario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UsuarioId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sociedad", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sede",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EstudioAuditorId = table.Column<int>(type: "int", nullable: false),
                    BdOrigenId = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    NombreUsuario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UsuarioId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sede", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sede_EstudioAuditor_EstudioAuditorId",
                        column: x => x.EstudioAuditorId,
                        principalTable: "EstudioAuditor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioSRC_ContratistaId",
                table: "UsuarioSRC",
                column: "ContratistaId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioSRC_EstudioAuditorId",
                table: "UsuarioSRC",
                column: "EstudioAuditorId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioSRC_SedeId",
                table: "UsuarioSRC",
                column: "SedeId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioSociedad_SociedadId",
                table: "UsuarioSociedad",
                column: "SociedadId");

            migrationBuilder.CreateIndex(
                name: "IX_Contratista_RazonSocial",
                table: "Contratista",
                column: "RazonSocial",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sede_EstudioAuditorId",
                table: "Sede",
                column: "EstudioAuditorId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioSociedad_Sociedad_SociedadId",
                table: "UsuarioSociedad",
                column: "SociedadId",
                principalTable: "Sociedad",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioSRC_Contratista_ContratistaId",
                table: "UsuarioSRC",
                column: "ContratistaId",
                principalTable: "Contratista",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioSRC_EstudioAuditor_EstudioAuditorId",
                table: "UsuarioSRC",
                column: "EstudioAuditorId",
                principalTable: "EstudioAuditor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioSRC_Sede_SedeId",
                table: "UsuarioSRC",
                column: "SedeId",
                principalTable: "Sede",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
