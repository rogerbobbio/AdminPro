using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminPro.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationTipo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "Applications",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Applications");
        }
    }
}
