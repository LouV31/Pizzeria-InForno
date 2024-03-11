using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pizzeria_InForno.Models
{
    public class DettagliOrdine
    {
        [Key]
        public int IdDettaglioOrdine { get; set; }
        [Required]
        [ForeignKey("Ordini")]
        public int IdOrdine { get; set; }
        [Required]
        [ForeignKey("Articoli")]
        public int IdArticolo { get; set; }
        [Required]
        public int Quantita { get; set; }

        public virtual Ordini Ordini { get; set; }
        public virtual Articoli Articoli { get; set; }
    }
}
