using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AdminPro.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedModulos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Modulos",
                columns: new[] { "Id", "Activo", "Color", "CreatedAt", "Icono", "Nombre", "Orden", "RutaBase", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, true, "primary", new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Utc), "bi-kanban", "Gestión de Proyectos", 0, "proyectos", new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, true, "success", new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Utc), "bi-hdd-network", "Catálogo de Servicios", 1, "servicios", new DateTime(2026, 8, 28, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Modulos",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Modulos",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
