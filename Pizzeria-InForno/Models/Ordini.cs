﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pizzeria_InForno.Models
{
    public class Ordini
    {
        [Key]
        public int IdOrdine { get; set; }
        [Required]
        [ForeignKey("Utenti")]
        public int IdUtente { get; set; }
        [Required]
        public string IndirizzoSpedizione { get; set; }


        [Required]
        public bool IsConsegnato { get; set; } = false;
        [Required]
        public string Note { get; set; } = "";

        public double? Totale { get; set; }
        public virtual Utenti Utenti { get; set; }

        public virtual ICollection<DettagliOrdine> DettagliOrdine { get; set; }


    }
}
