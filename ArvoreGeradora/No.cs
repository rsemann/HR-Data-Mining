using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArvoreGeradora
{
    public class No
    {
        public string Nome { get; set; }
        public string Label { get; set; }
        public Tipo Tipo { get; set; }
        public bool MenorIgual { get; set; }

        public string Valor { get; set; }

        public List<No> NosFilhos = new List<No>();

        public decimal Percentual { get; set; }
        public int ValorTotal { get; set; }

        public string MaiorValorClasseMeta { get; set; }

        public int Nivel { get; set; }

        public bool Caminho { get; set; }

        public string Legenda { get; set; }
    }
}
