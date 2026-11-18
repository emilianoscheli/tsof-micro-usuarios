using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsuariosAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DelegacionesTemporalesRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DelegacionTemporal_UsuarioSRC_DelegadoIdRed",
                table: "DelegacionTemporal");

            migrationBuilder.DropForeignKey(
                name: "FK_DelegacionTemporal_UsuarioSRC_DelegadorId",
                table: "DelegacionTemporal");

            migrationBuilder.DropIndex(
                name: "IX_DelegacionTemporal_DelegadoIdRed",
                table: "DelegacionTemporal");

            migrationBuilder.DropColumn(
                name: "DelegadoIdRed",
                table: "DelegacionTemporal");

            migrationBuilder.DropColumn(
                name: "DelegadorIdRed",
                table: "DelegacionTemporal");

            migrationBuilder.DropColumn(
                name: "Motivo",
                table: "DelegacionTemporal");

            migrationBuilder.DropColumn(
                name: "NombreCompletoDelegado",
                table: "DelegacionTemporal");

            migrationBuilder.DropColumn(
                name: "NombreCompletoDelegador",
                table: "DelegacionTemporal");

            migrationBuilder.RenameColumn(
                name: "DelegadorId",
                table: "DelegacionTemporal",
                newName: "UsuarioDelegadorId");

            migrationBuilder.RenameColumn(
                name: "DelegadoId",
                table: "DelegacionTemporal",
                newName: "UsuarioDelegadoId");

            migrationBuilder.RenameIndex(
                name: "IX_DelegacionTemporal_DelegadorId",
                table: "DelegacionTemporal",
                newName: "IX_DelegacionTemporal_UsuarioDelegadorId");

            migrationBuilder.AlterColumn<int>(
                name: "TiempoLimite",
                table: "DelegacionTemporal",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Observacion",
                table: "DelegacionTemporal",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "MotivoDelegacionFkId",
                table: "DelegacionTemporal",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // por unica vez, seteo valor 1 para que no se rompa con el 0
            migrationBuilder.Sql(@"
	             UPDATE d
                 SET
		             d.MotivoDelegacionFkId = 1
                 FROM DelegacionTemporal d
                 WHERE d.MotivoDelegacionFkId = 0");

            migrationBuilder.CreateIndex(
                name: "IX_DelegacionTemporal_UsuarioDelegadoId",
                table: "DelegacionTemporal",
                column: "UsuarioDelegadoId");

            migrationBuilder.AddForeignKey(
                name: "FK_DelegacionTemporal_UsuarioSRC_UsuarioDelegadoId",
                table: "DelegacionTemporal",
                column: "UsuarioDelegadoId",
                principalTable: "UsuarioSRC",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DelegacionTemporal_UsuarioSRC_UsuarioDelegadorId",
                table: "DelegacionTemporal",
                column: "UsuarioDelegadorId",
                principalTable: "UsuarioSRC",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DelegacionTemporal_UsuarioSRC_UsuarioDelegadoId",
                table: "DelegacionTemporal");

            migrationBuilder.DropForeignKey(
                name: "FK_DelegacionTemporal_UsuarioSRC_UsuarioDelegadorId",
                table: "DelegacionTemporal");

            migrationBuilder.DropIndex(
                name: "IX_DelegacionTemporal_UsuarioDelegadoId",
                table: "DelegacionTemporal");

            migrationBuilder.DropColumn(
                name: "MotivoDelegacionFkId",
                table: "DelegacionTemporal");

            migrationBuilder.RenameColumn(
                name: "UsuarioDelegadorId",
                table: "DelegacionTemporal",
                newName: "DelegadorId");

            migrationBuilder.RenameColumn(
                name: "UsuarioDelegadoId",
                table: "DelegacionTemporal",
                newName: "DelegadoId");

            migrationBuilder.RenameIndex(
                name: "IX_DelegacionTemporal_UsuarioDelegadorId",
                table: "DelegacionTemporal",
                newName: "IX_DelegacionTemporal_DelegadorId");

            migrationBuilder.AlterColumn<string>(
                name: "TiempoLimite",
                table: "DelegacionTemporal",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Observacion",
                table: "DelegacionTemporal",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DelegadoIdRed",
                table: "DelegacionTemporal",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DelegadorIdRed",
                table: "DelegacionTemporal",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Motivo",
                table: "DelegacionTemporal",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NombreCompletoDelegado",
                table: "DelegacionTemporal",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NombreCompletoDelegador",
                table: "DelegacionTemporal",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_DelegacionTemporal_DelegadoIdRed",
                table: "DelegacionTemporal",
                column: "DelegadoIdRed");

            migrationBuilder.AddForeignKey(
                name: "FK_DelegacionTemporal_UsuarioSRC_DelegadoIdRed",
                table: "DelegacionTemporal",
                column: "DelegadoIdRed",
                principalTable: "UsuarioSRC",
                principalColumn: "IdRed",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DelegacionTemporal_UsuarioSRC_DelegadorId",
                table: "DelegacionTemporal",
                column: "DelegadorId",
                principalTable: "UsuarioSRC",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
