using System.ComponentModel.DataAnnotations;

namespace Pizzeria_InForno.Models
{
    public class Utenti
    {
        [Key]
        public int IdUtente { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]

        public string Password { get; set; }
        public string Ruolo { get; set; } = "user";

        public virtual ICollection<Ordini> Ordini { get; set; }

    }
}
