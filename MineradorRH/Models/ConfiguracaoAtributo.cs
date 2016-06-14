using ArvoreGeradora;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Web;

namespace MineradorRH.Models
{
    public class ConfiguracaoAtributo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ID { get; set; }
        
        [Required]
        public string Nome { get; set; }
        
        [Required]
        public Tipo Tipo { get; set; }

        public Nivel Nivel { get; set; }

        [Required]
        public string Legenda { get; set; }

        public string ClasseMeta { get; set; }

        [ForeignKey("ConfiguracaoArvore")]
        public int ConfiguracaoArvoreID { get; set; }
        public ConfiguracaoArvore ConfiguracaoArvore { get; set; }
    }
}