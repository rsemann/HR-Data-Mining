using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArvoreGeradora
{
    public class Arvore
    {
        public No NoRaiz { get; set; }
        private List<string> CoresInseridas = new List<string>();

        public JsonArvore GerarJSON()
        {
            JsonArvore jsonArvore = new JsonArvore
            {
                name = NoRaiz.Legenda,
                size = NoRaiz.ValorTotal,
                caminho = NoRaiz.Caminho,
                valor = NoRaiz.Label
            };

            foreach (var no in NoRaiz.NosFilhos)
            {
                var jsonArvoreNo = new JsonArvore();
                jsonArvoreNo = GerarJSON(no, getRandomColor());
                Thread.Sleep(5);
                jsonArvore.children.Add(jsonArvoreNo);
            }

            return jsonArvore;
        }

        private JsonArvore GerarJSON(No no, Color cor)
        {
            var jsonArvore = new JsonArvore
            {
                name = string.Format(no.Label),
                size = no.ValorTotal,
                color = ColorTranslator.ToHtml(cor),
                valorClasseMeta = no.MaiorValorClasseMeta,
                caminho = no.Caminho,
                valor = no.Tipo == Tipo.Contínuo ? ((no.MenorIgual ? "<=" : ">") + no.Valor) : no.Valor,
                legenda = no.Legenda
            };
            var corFilhos = getDarkColor(cor);

            foreach (var noFilho in no.NosFilhos)
            {
                var jsonArvoreNo = GerarJSON(noFilho, corFilhos);
                Thread.Sleep(5);
                jsonArvore.children.Add(jsonArvoreNo);
            }

            return jsonArvore;
        }

        private Color getRandomColor()
        {
            string hexOutput = string.Empty;
            Random rnd = new Random();
            do
            {
                hexOutput = String.Format("{0:X}", rnd.Next(0, 0xFFFFFF));
                while (hexOutput.Length < 6)
                    hexOutput = "0" + hexOutput;
            } while (CoresInseridas.Exists(cor => cor == hexOutput));
            
            CoresInseridas.Add(hexOutput);
            return (Color)ColorTranslator.FromHtml("#" + hexOutput);
        }

        private Color getDarkColor(Color color)
        {
            return Color.FromArgb(color.A, Convert.ToInt32(color.R * 0.8), Convert.ToInt32(color.G * 0.8), Convert.ToInt32(color.B * 0.8));
        }
    }


}
