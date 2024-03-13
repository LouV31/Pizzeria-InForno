namespace Pizzeria_InForno.Models
{
    public class ArticoliPerCarrello
    {
        public int IdArticolo { get; set; }
        public string NomeArticolo { get; set; }
        public string Descrizione { get; set; }
        public double Prezzo { get; set; }
        public string Immagine { get; set; }

        public int TempoConsegna { get; set; }
    }
}
