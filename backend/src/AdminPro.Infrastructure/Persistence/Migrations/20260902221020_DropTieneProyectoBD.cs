using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminPro.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DropTieneProyectoBD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TieneProyectoBD",
                table: "Applications");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TieneProyectoBD",
                table: "Applications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
