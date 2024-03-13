using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pizzeria_InForno.Migrations
{
    /// <inheritdoc />
    public partial class FinalUpdate1 : Migration
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
                    NomeIngrediente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prezzo = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ingredienti", x => x.IdIngrediente);
                });

            migrationBuilder.CreateTable(
                name: "DettagliIngrediente",
                columns: table => new
                {
                    IdDettaglioIngrediente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdArticolo = table.Column<int>(type: "int", nullable: false),
                    IdIngrediente = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DettagliIngrediente", x => x.IdDettaglioIngrediente);
                    table.ForeignKey(
                        name: "FK_DettagliIngrediente_Articoli_IdArticolo",
                        column: x => x.IdArticolo,
                        principalTable: "Articoli",
                        principalColumn: "IdArticolo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DettagliIngrediente_Ingredienti_IdIngrediente",
                        column: x => x.IdIngrediente,
                        principalTable: "Ingredienti",
                        principalColumn: "IdIngrediente",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DettagliIngrediente_IdArticolo",
                table: "DettagliIngrediente",
                column: "IdArticolo");

            migrationBuilder.CreateIndex(
                name: "IX_DettagliIngrediente_IdIngrediente",
                table: "DettagliIngrediente",
                column: "IdIngrediente");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DettagliIngrediente");

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
