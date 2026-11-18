#nullable disable

namespace UsuariosAPI.Infrastructure.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    public partial class Gestion_Usuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UsuarioRol_UsuarioSRCId",
                table: "UsuarioRol");

            migrationBuilder.DropIndex(
                name: "IX_RolPermiso_PermisoId",
                table: "RolPermiso");

            migrationBuilder.AlterColumn<string>(
                name: "NombreCompleto",
                table: "UsuarioSRC",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "IdRed",
                table: "UsuarioSRC",
                type: "varchar(7)",
                unicode: false,
                maxLength: 7,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "ContratistaId",
                table: "UsuarioSRC",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorreoElectronico",
                table: "UsuarioSRC",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<int>(
                name: "EstudioAuditorId",
                table: "UsuarioSRC",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SedeId",
                table: "UsuarioSRC",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Rol",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Rol",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "AsignaSociedad",
                table: "Rol",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EsManual",
                table: "Rol",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Permiso",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "Permiso",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Contratista",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RazonSocial = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    BdOrigenId = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    NombreUsuario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
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
                    Nombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    BdOrigenId = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    NombreUsuario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
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
                    Nombre = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    BdOrigenId = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    NombreUsuario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
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
                    Nombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    BdOrigenId = table.Column<int>(type: "int", nullable: false),
                    EstudioAuditorId = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    NombreUsuario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
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

            migrationBuilder.CreateTable(
                name: "UsuarioSociedad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioSRCId = table.Column<int>(type: "int", nullable: false),
                    SociedadId = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    NombreUsuario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioSociedad", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuarioSociedad_Sociedad_SociedadId",
                        column: x => x.SociedadId,
                        principalTable: "Sociedad",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuarioSociedad_UsuarioSRC_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "UsuarioSRC",
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
                name: "IX_UsuarioSRC_IdRed",
                table: "UsuarioSRC",
                column: "IdRed",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioSRC_SedeId",
                table: "UsuarioSRC",
                column: "SedeId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_UsuarioSRC_CorreoElectronico",
                table: "UsuarioSRC",
                sql: "CorreoElectronico LIKE '%@%' AND CorreoElectronico NOT LIKE '%[^a-zA-Z0-9@.]%'");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRol_UsuarioSRCId_RolId",
                table: "UsuarioRol",
                columns: new[] { "UsuarioSRCId", "RolId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolPermiso_PermisoId_RolId",
                table: "RolPermiso",
                columns: new[] { "PermisoId", "RolId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permiso_Codigo",
                table: "Permiso",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contratista_RazonSocial",
                table: "Contratista",
                column: "RazonSocial",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sede_EstudioAuditorId",
                table: "Sede",
                column: "EstudioAuditorId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioSociedad_SociedadId",
                table: "UsuarioSociedad",
                column: "SociedadId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioSociedad_UsuarioId_SociedadId",
                table: "UsuarioSociedad",
                columns: new[] { "UsuarioId", "SociedadId" },
                unique: true,
                filter: "[UsuarioId] IS NOT NULL");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "UsuarioSociedad");

            migrationBuilder.DropTable(
                name: "EstudioAuditor");

            migrationBuilder.DropTable(
                name: "Sociedad");

            migrationBuilder.DropIndex(
                name: "IX_UsuarioSRC_ContratistaId",
                table: "UsuarioSRC");

            migrationBuilder.DropIndex(
                name: "IX_UsuarioSRC_EstudioAuditorId",
                table: "UsuarioSRC");

            migrationBuilder.DropIndex(
                name: "IX_UsuarioSRC_IdRed",
                table: "UsuarioSRC");

            migrationBuilder.DropIndex(
                name: "IX_UsuarioSRC_SedeId",
                table: "UsuarioSRC");

            migrationBuilder.DropCheckConstraint(
                name: "CK_UsuarioSRC_CorreoElectronico",
                table: "UsuarioSRC");

            migrationBuilder.DropIndex(
                name: "IX_UsuarioRol_UsuarioSRCId_RolId",
                table: "UsuarioRol");

            migrationBuilder.DropIndex(
                name: "IX_RolPermiso_PermisoId_RolId",
                table: "RolPermiso");

            migrationBuilder.DropIndex(
                name: "IX_Permiso_Codigo",
                table: "Permiso");

            migrationBuilder.DropColumn(
                name: "ContratistaId",
                table: "UsuarioSRC");

            migrationBuilder.DropColumn(
                name: "CorreoElectronico",
                table: "UsuarioSRC");

            migrationBuilder.DropColumn(
                name: "EstudioAuditorId",
                table: "UsuarioSRC");

            migrationBuilder.DropColumn(
                name: "SedeId",
                table: "UsuarioSRC");

            migrationBuilder.DropColumn(
                name: "AsignaSociedad",
                table: "Rol");

            migrationBuilder.DropColumn(
                name: "EsManual",
                table: "Rol");

            migrationBuilder.AlterColumn<string>(
                name: "NombreCompleto",
                table: "UsuarioSRC",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "IdRed",
                table: "UsuarioSRC",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(7)",
                oldUnicode: false,
                oldMaxLength: 7);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Rol",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Rol",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Permiso",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "Permiso",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRol_UsuarioSRCId",
                table: "UsuarioRol",
                column: "UsuarioSRCId");

            migrationBuilder.CreateIndex(
                name: "IX_RolPermiso_PermisoId",
                table: "RolPermiso",
                column: "PermisoId");
        }
    }
}
