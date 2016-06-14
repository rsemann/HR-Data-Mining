using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArvoreGeradora
{
    public class Atributo
    {
        public string Nome { get; set; }

        public Tipo TipoAtributo { get; set; }

        public Nivel Nivel { get; set; }

        public string Legenda { get; set; }

        public ArrayList valores = new ArrayList();

        /// <summary>
        /// Filtra as informações distintos dentro da lista de valores do atributo
        /// Se número realiza discretização
        /// </summary>
        /// <returns>Retorna as informações distintas (S, S, T, T, S) = S, T</returns>
        public ArrayList RetornaValoresDistintos()
        {
            ArrayList valoresDistintos = new ArrayList();
            bool distinto;

            foreach (var valor in valores)
            {
                distinto = true;
                foreach (var valorDistinto in valoresDistintos)
                {
                    if (valorDistinto.ToString().Equals(valor.ToString()))
                    {
                        distinto = false;
                        break;
                    }
                }

                if (distinto)
                    valoresDistintos.Add(valor);
            }

            return valoresDistintos;
        }

        public int RetornaQuantidadeIncidenciasValor(string valorBusca)
        {
            int retorno = 0;
            foreach (var valor in valores)
            {
                if (valor.ToString().Equals(valorBusca))
                    retorno++;
            }

            return retorno;
        }

        /// <summary>
        /// Discretiza o atributo
        /// </summary>
        /// <returns>Retorna como ponto de referência a média entre os valores</returns>
        public double Discretizar()
        {
            double pontoReferencia = 0;
            foreach (var valor in valores)
            {
                double ret = 0;
                bool converte = double.TryParse(valor.ToString(), out ret);
                pontoReferencia += ret;
            }

            pontoReferencia = pontoReferencia / valores.Count;
            return pontoReferencia;
        }

        /// <summary>
        /// Retorna o (D) da fórmula de cálculo da entropia
        /// </summary>
        /// <param name="valorProcurar">Valor que deve ser procurado o número de casos</param>
        /// <returns>Retorna o número de casos</returns>
        public float RetornaProporcaoCasos(string valorProcurar)
        {
            float casos = 0;

            foreach (var valor in valores)
            {
                if (valor.ToString().Equals(valorProcurar))
                    casos++;
            }

            return casos;
        }

        /// <summary>
        /// Retorna os casos especificos dentro do valor
        /// (T, S) (T, S) (T, V) (T, L) (T, V) = {("S", 2), ("V", 2), ("L", 3)}
        /// </summary>
        /// <param name="valorProcurar">Valor a procurar os diferentes casos especificos</param>
        /// <param name="valoresOutroExemplo">Valores a serem buscados para formar os casos</param>
        /// <returns>Returna uma coleção com os valores diferentes e suas ocorrências sobre o valor</returns>
        public Dictionary<string, int> RetornaCasosOutroExemplo(string valorProcurar, ArrayList valoresOutroExemplo)
        {
            Dictionary<string, int> casos = new Dictionary<string, int>();

            for (int i = 0; i < valores.Count; i++)
            {
                //Se valor igual, procurar na lista valoresOutroExemplo no mesmo indíce
                if (valores[i].ToString().Equals(valorProcurar))
                {
                    //Se casos já conter o valor apenas incrementa o número de ocorrências
                    if (casos.ContainsKey(valoresOutroExemplo[i].ToString()))
                        casos[valoresOutroExemplo[i].ToString()]++;
                    else
                    {
                        //Se não existe cria o valor nos casos
                        casos.Add(valoresOutroExemplo[i].ToString(), 1);
                    }
                }
            }

            return casos;
        }

        /// <summary>
        /// Retorna a quantidade de casos dentro do ponto de referência passado
        /// </summary>
        /// <param name="pontoReferencia">Valor a ser buscado o conjunto que se encaixa</param>
        /// <param name="maior">Se true, procura > pontoReferencia, senão menor igual que o pontoReferencia</param>
        /// <returns>Retorna a quantidade de casos que formam o conjunto</returns>
        public int RetornaCasosConjunto(double pontoReferencia, bool maior)
        {
            int casos = 0;
            foreach (var valor in valores)
            {
                double ret = 0;
                bool converte = double.TryParse(valor.ToString(), out ret);
                //Conjunto maior
                if ((maior && ret > pontoReferencia) || (!maior && ret <= pontoReferencia))
                    casos++;
            }

            return casos;
        }

        /// <summary>
        /// Casos outro exemplo para tipo número
        /// </summary>
        /// <param name="pontoReferencia">Valor a ser buscado os valores que se encaixam, para buscar através deste o índice nos outros valores</param>
        /// <param name="maior">Se true, procura > pontoReferencia, senão menor igual que o pontoReferencia</param>
        /// <param name="valoresOutroExemplo">Valores a serem buscados para formar os casos</param>
        /// <returns></returns>
        public Dictionary<string, int> RetornaCasosOutroExemplo(double pontoReferencia, bool maior, ArrayList valoresOutroExemplo)
        {
            Dictionary<string, int> casos = new Dictionary<string, int>();

            for (int i = 0; i < valores.Count; i++)
            {
                double ret = 0;
                bool converte = double.TryParse(valores[i].ToString(), out ret);

                //Conjunto maior
                if (maior && ret > pontoReferencia)
                {
                    //Se casos já conter o valor apenas incrementa o número de ocorrências
                    if (casos.ContainsKey(valoresOutroExemplo[i].ToString()))
                        casos[valoresOutroExemplo[i].ToString()]++;
                    else
                        //Se não existe cria o valor nos casos
                        casos.Add(valoresOutroExemplo[i].ToString(), 1);
                }
                //Conjunto menor igual
                else if (!maior && ret <= pontoReferencia)
                {
                    //Se casos já conter o valor apenas incrementa o número de ocorrências
                    if (casos.ContainsKey(valoresOutroExemplo[i].ToString()))
                        casos[valoresOutroExemplo[i].ToString()]++;
                    else
                        //Se não existe cria o valor nos casos
                        casos.Add(valoresOutroExemplo[i].ToString(), 1);
                }
            }

            return casos;
        }

        public string RetornaPorcentagemMaiorIncidencia()
        {
            Dictionary<string, decimal> lista = new Dictionary<string, decimal>();
            
            foreach (var valor in valores)
            {
                if (lista.ContainsKey(valor.ToString()))
                    lista[valor.ToString()]++;
                else
                    lista.Add(valor.ToString(), 1);
            }

            var maiorQuantidade = lista.OrderByDescending(x => x.Value).FirstOrDefault();

            return maiorQuantidade.Key+": "+ Math.Round((maiorQuantidade.Value * 100) / valores.Count, 2, MidpointRounding.AwayFromZero)+"%";
        }
    }

    public enum Tipo
    {
        Categorico = 0,
        Contínuo = 1,
        Mineração_de_Texto = 2
    }

    public enum Nivel
    {
        Primeiro = 1,
        Segundo = 2,
        Terceiro = 3
    }
}
        
