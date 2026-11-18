#nullable disable
namespace UsuariosAPI.Infrastructure.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    public partial class inicialUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LogTecnico",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Entidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntidadId = table.Column<int>(type: "int", nullable: false),
                    ValorOriginal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValorNuevo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    NombreUsuario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogTecnico", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permiso",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    NombreUsuario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permiso", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rol",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    NombreUsuario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rol", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioSRC",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCompleto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdRed = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    NombreUsuario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioSRC", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolPermiso",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RolId = table.Column<int>(type: "int", nullable: false),
                    PermisoId = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    NombreUsuario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolPermiso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolPermiso_Permiso_PermisoId",
                        column: x => x.PermisoId,
                        principalTable: "Permiso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolPermiso_Rol_RolId",
                        column: x => x.RolId,
                        principalTable: "Rol",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioRol",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioSRCId = table.Column<int>(type: "int", nullable: false),
                    UsuarioRed = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RolId = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    NombreUsuario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioRol", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuarioRol_Rol_RolId",
                        column: x => x.RolId,
                        principalTable: "Rol",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuarioRol_UsuarioSRC_UsuarioSRCId",
                        column: x => x.UsuarioSRCId,
                        principalTable: "UsuarioSRC",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RolPermiso_PermisoId",
                table: "RolPermiso",
                column: "PermisoId");

            migrationBuilder.CreateIndex(
                name: "IX_RolPermiso_RolId",
                table: "RolPermiso",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRol_RolId",
                table: "UsuarioRol",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRol_UsuarioSRCId",
                table: "UsuarioRol",
                column: "UsuarioSRCId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogTecnico");

            migrationBuilder.DropTable(
                name: "RolPermiso");

            migrationBuilder.DropTable(
                name: "UsuarioRol");

            migrationBuilder.DropTable(
                name: "Permiso");

            migrationBuilder.DropTable(
                name: "Rol");

            migrationBuilder.DropTable(
                name: "UsuarioSRC");
        }
    }
}
