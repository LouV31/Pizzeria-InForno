using System.ComponentModel.DataAnnotations;

namespace Pizzeria_InForno.Models
{
    public class Articoli
    {
        [Key]
        public int IdArticolo { get; set; }
        [Required]
        public string NomeArticolo { get; set; }
        [Required]
        public string Descrizione { get; set; }
        [Required]
        public double Prezzo { get; set; }
        [Required]
        public string Immagine { get; set; }
        [Required]
        [Display(Name = "Tempi di consegna")]
        public int TempoConsegna { get; set; }
        [Required]
        public string Ingredienti { get; set; }

        // questa proprietà è stata aggiunta per permettere la navigazione
        public ICollection<DettagliOrdine> DettagliOrdine { get; set; }
    }
}
