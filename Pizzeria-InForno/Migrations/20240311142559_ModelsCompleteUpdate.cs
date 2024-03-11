using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pizzeria_InForno.Migrations
{
    /// <inheritdoc />
    public partial class ModelsCompleteUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrezzoTotale",
                table: "DettagliOrdine");

            migrationBuilder.DropColumn(
                name: "PrezzoUnitario",
                table: "DettagliOrdine");

            migrationBuilder.AddColumn<double>(
                name: "Totale",
                table: "Ordini",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Totale",
                table: "Ordini");

            migrationBuilder.AddColumn<double>(
                name: "PrezzoTotale",
                table: "DettagliOrdine",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PrezzoUnitario",
                table: "DettagliOrdine",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
