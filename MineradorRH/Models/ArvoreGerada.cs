using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MineradorRH.Models
{
    public class ArvoreGerada
    {
        [Key]
        public int ID { get; set; }
        public string XML { get; set; }

        public string JSON { get; set; }

        public string JsonNuvemPalavras { get; set; }

        public DateTime DataGeracao { get; set; }

        public string UsuarioGeracao { get; set; }

        public string XmlPmml { get; set; }

        public string ClasseMeta { get; set; }

        [ForeignKey("ConfiguracaoArvore")]
        public int ConfiguracaoArvoreID { get; set; }
        public ConfiguracaoArvore ConfiguracaoArvore { get; set; }
    }
}