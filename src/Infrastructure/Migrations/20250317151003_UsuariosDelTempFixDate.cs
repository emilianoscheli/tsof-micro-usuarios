using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsuariosAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UsuariosDelTempFixDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "FechaHasta",
                table: "DelegacionTemporal",
                type: "date",
                precision: 0,
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(0)",
                oldPrecision: 0);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "FechaDesde",
                table: "DelegacionTemporal",
                type: "date",
                precision: 0,
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(0)",
                oldPrecision: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaHasta",
                table: "DelegacionTemporal",
                type: "datetime2(0)",
                precision: 0,
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldPrecision: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaDesde",
                table: "DelegacionTemporal",
                type: "datetime2(0)",
                precision: 0,
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldPrecision: 0);
        }
    }
}
