using System.ComponentModel.DataAnnotations;

namespace Pizzeria_InForno.Models
{
    public class Ingredienti
    {
        [Key]
        public int IdIngrediente { get; set; }
        [Required]
        [Display(Name = "Nome Ingrediente")]
        public string NomeIngrediente { get; set; }

        public virtual ICollection<Articoli> Articoli { get; set; }

    }
}
