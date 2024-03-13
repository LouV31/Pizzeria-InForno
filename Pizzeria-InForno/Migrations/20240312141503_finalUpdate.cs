using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pizzeria_InForno.Migrations
{
    /// <inheritdoc />
    public partial class finalUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Articoli",
                columns: table => new
                {
                    IdArticolo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeArticolo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descrizione = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prezzo = table.Column<double>(type: "float", nullable: false),
                    Immagine = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TempoConsegna = table.Column<int>(type: "int", nullable: false),
                    Ingredienti = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articoli", x => x.IdArticolo);
                });

            migrationBuilder.CreateTable(
                name: "Utenti",
                columns: table => new
                {
                    IdUtente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ruolo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utenti", x => x.IdUtente);
                });

            migrationBuilder.CreateTable(
                name: "Ordini",
                columns: table => new
                {
                    IdOrdine = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUtente = table.Column<int>(type: "int", nullable: false),
                    IndirizzoSpedizione = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsConsegnato = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Totale = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ordini", x => x.IdOrdine);
                    table.ForeignKey(
                        name: "FK_Ordini_Utenti_IdUtente",
                        column: x => x.IdUtente,
                        principalTable: "Utenti",
                        principalColumn: "IdUtente",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DettagliOrdine",
                columns: table => new
                {
                    IdDettaglioOrdine = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdOrdine = table.Column<int>(type: "int", nullable: false),
                    IdArticolo = table.Column<int>(type: "int", nullable: false),
                    Quantita = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DettagliOrdine", x => x.IdDettaglioOrdine);
                    table.ForeignKey(
                        name: "FK_DettagliOrdine_Articoli_IdArticolo",
                        column: x => x.IdArticolo,
                        principalTable: "Articoli",
                        principalColumn: "IdArticolo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DettagliOrdine_Ordini_IdOrdine",
                        column: x => x.IdOrdine,
                        principalTable: "Ordini",
                        principalColumn: "IdOrdine",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DettagliOrdine_IdArticolo",
                table: "DettagliOrdine",
                column: "IdArticolo");

            migrationBuilder.CreateIndex(
                name: "IX_DettagliOrdine_IdOrdine",
                table: "DettagliOrdine",
                column: "IdOrdine");

            migrationBuilder.CreateIndex(
                name: "IX_Ordini_IdUtente",
                table: "Ordini",
                column: "IdUtente");

            migrationBuilder.CreateIndex(
                name: "IX_Utenti_Username",
                table: "Utenti",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DettagliOrdine");

            migrationBuilder.DropTable(
                name: "Articoli");

            migrationBuilder.DropTable(
                name: "Ordini");

            migrationBuilder.DropTable(
                name: "Utenti");
        }
    }
}
