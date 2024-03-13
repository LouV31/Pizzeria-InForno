using System.ComponentModel.DataAnnotations;

namespace Pizzeria_InForno.Models
{
    public class Ingredienti
    {
        [Key]
        public int IdIngrediente { get; set; }
        [Required]
        [Display(Name = "Nome")]
        public string NomeIngrediente { get; set; }
        [Required]
        public double Prezzo { get; set; }

        public virtual ICollection<DettagliIngredienti> DettagliIngredienti { get; set; }
    }
}
