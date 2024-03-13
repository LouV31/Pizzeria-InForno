using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pizzeria_InForno.Migrations
{
    /// <inheritdoc />
    public partial class updateOrdiniTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrdiniIdOrdine",
                table: "DettagliIngrediente",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DettagliIngrediente_OrdiniIdOrdine",
                table: "DettagliIngrediente",
                column: "OrdiniIdOrdine");

            migrationBuilder.AddForeignKey(
                name: "FK_DettagliIngrediente_Ordini_OrdiniIdOrdine",
                table: "DettagliIngrediente",
                column: "OrdiniIdOrdine",
                principalTable: "Ordini",
                principalColumn: "IdOrdine");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DettagliIngrediente_Ordini_OrdiniIdOrdine",
                table: "DettagliIngrediente");

            migrationBuilder.DropIndex(
                name: "IX_DettagliIngrediente_OrdiniIdOrdine",
                table: "DettagliIngrediente");

            migrationBuilder.DropColumn(
                name: "OrdiniIdOrdine",
                table: "DettagliIngrediente");
        }
    }
}
