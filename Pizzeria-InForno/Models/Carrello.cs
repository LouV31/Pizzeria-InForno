namespace Pizzeria_InForno.Models
{
    public class Carrello
    {
        public ArticoliPerCarrello Articoli { get; set; }
        public int Quantita { get; set; }

        public double Totale { get; set; }
    }
}
