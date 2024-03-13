using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pizzeria_InForno.Models
{
    public class DettagliIngredienti
    {
        [Key]
        public int IdDettaglioIngrediente { get; set; }
        [ForeignKey("Articoli")]
        [Required]
        public int IdArticolo { get; set; }
        [ForeignKey("Ingredienti")]
        [Required]
        public int IdIngrediente { get; set; }

        public virtual Articoli Articoli { get; set; }
        public virtual Ingredienti Ingredienti { get; set; }
    }
}
