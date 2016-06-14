using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MineradorRH.Models
{
    public class ConsultaPmml
    {
        public string Nome { get; set; }

        public string Label { get; set; }
        public string Valor { get; set; }

        public bool ClasseMeta { get; set; }
        public int ConfiguracaoArvoreID { get; set; }

        public int ArvoreGeradaId { get; set; }
    }
}