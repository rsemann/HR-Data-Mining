using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MineradorRH.Models
{
    public class PalavraAgrupadora
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [Display(Name = "Palavra")]
        public string Palavra { get; set; }
    }
}