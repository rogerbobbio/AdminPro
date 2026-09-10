using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminPro.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameReporteParametrosEjecucion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ParametrosEjemplo",
                table: "Reportes",
                newName: "ParametrosEjecucion");

            migrationBuilder.AlterColumn<string>(
                name: "ParametrosEjecucion",
                table: "Reportes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ParametrosEjecucion",
                table: "Reportes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.RenameColumn(
                name: "ParametrosEjecucion",
                table: "Reportes",
                newName: "ParametrosEjemplo");
        }
    }
}
