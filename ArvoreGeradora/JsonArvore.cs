using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArvoreGeradora
{
    public class JsonArvore
    {
        public string name;
        public string color;
        public int size;
        public string valorClasseMeta;
        public bool caminho;
        public string valor;
        public string legenda;
        public List<JsonArvore> children = new List<JsonArvore>();
    }
}
