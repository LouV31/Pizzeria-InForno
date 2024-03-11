using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pizzeria_InForno.Migrations
{
    /// <inheritdoc />
    public partial class AddIngredientiTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ingredienti",
                table: "Articoli");

            migrationBuilder.CreateTable(
                name: "Ingredienti",
                columns: table => new
                {
                    IdIngrediente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeIngrediente = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredienti", x => x.IdIngrediente);
                });

            migrationBuilder.CreateTable(
                name: "ArticoliIngredienti",
                columns: table => new
                {
                    ArticoliIdArticolo = table.Column<int>(type: "int", nullable: false),
                    IngredientiIdIngrediente = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticoliIngredienti", x => new { x.ArticoliIdArticolo, x.IngredientiIdIngrediente });
                    table.ForeignKey(
                        name: "FK_ArticoliIngredienti_Articoli_ArticoliIdArticolo",
                        column: x => x.ArticoliIdArticolo,
                        principalTable: "Articoli",
                        principalColumn: "IdArticolo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticoliIngredienti_Ingredienti_IngredientiIdIngrediente",
                        column: x => x.IngredientiIdIngrediente,
                        principalTable: "Ingredienti",
                        principalColumn: "IdIngrediente",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticoliIngredienti_IngredientiIdIngrediente",
                table: "ArticoliIngredienti",
                column: "IngredientiIdIngrediente");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticoliIngredienti");

            migrationBuilder.DropTable(
                name: "Ingredienti");

            migrationBuilder.AddColumn<string>(
                name: "Ingredienti",
                table: "Articoli",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
