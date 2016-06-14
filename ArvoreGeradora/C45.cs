using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ArvoreGeradora
{
    public class C45
    {
        public Arvore arvore = new Arvore();
        private Atributo _ClasseMeta;
        public HashSet<string> PercentuaisClasseMeta = new HashSet<string>();
        public int? Poda { get; set; }

        public C45()
        {
            if (!Poda.HasValue)
                Poda = 10;
        }

        public void Calcular(string atributoMeta, List<Atributo> atributos)
        {
            _ClasseMeta = atributos.FirstOrDefault(atr => atr.Nome.Equals(atributoMeta));
            No noAtual = CriarNo(_ClasseMeta.Nome,
                                 _ClasseMeta.Legenda,
                                 Tipo.Categorico,
                                 _ClasseMeta.Nome,
                                 atributos[0].valores.Count,
                                 atributos[0].valores.Count,
                                 1);
            arvore.NoRaiz = noAtual;
            GerarC45(_ClasseMeta.Nome, atributos, noAtual);
        }

        /// <summary>
        /// Indução da árvore
        /// </summary>
        /// <param name="atributoMeta">Atributo meta para o nível que a árvore se encontra</param>
        /// <param name="atributos">Lista de atributos para o nível corrente</param>
        /// <param name="no">Referência para o nó do nível acima</param>
        /// <param name="atrMaiorGanhoAcima">Referência para o nível de maior ganho do nível acima</param>
        private void GerarC45(string atributoMeta, List<Atributo> atributos, No no)
        {
            //Realiza poda
            if (Poda.HasValue && no.Nivel >= Poda.Value)
                return;

            //Calcular entropia da classe meta para a quantidade de amostra para o nivel atual
            var valoresClasseMeta = atributos.FirstOrDefault(atr => atr.Nome.Equals(_ClasseMeta.Nome));
            if (valoresClasseMeta.RetornaValoresDistintos().Count <= 1)
                return;

            double valorEntropia = CalcularEntropia(valoresClasseMeta);

            Atributo atributoMaiorGanho = new Atributo();
            double maiorGanho = 0;

            var atributosTemp = atributos.Where(atr => !atr.Nome.Equals(_ClasseMeta.Nome));
            if (atributoMeta == "Outras palavras" || atributoMeta == "Sem comentário")
                atributosTemp = atributos.Where(atr => !atr.Nome.Equals(_ClasseMeta.Nome) && atr.Nome != no.Nome);

            //Determina o atributo de maior ganho para nível atual
            foreach (var atributo in atributosTemp)
            {
                double ganhoAtr = CalcularGanho(valorEntropia, valoresClasseMeta, atributo);
                if (ganhoAtr > maiorGanho)
                {
                    maiorGanho = ganhoAtr;
                    atributoMaiorGanho = atributo;
                }
            }


            No noAtual;
            if (atributoMaiorGanho.TipoAtributo == Tipo.Categorico)
            {
                var valoresDistintos = atributoMaiorGanho.RetornaValoresDistintos();
                foreach (var valor in valoresDistintos)
                {
                    var novosAtributos = CriarNovosAtributosCategorico(valor.ToString(), atributoMaiorGanho, atributos);
                    noAtual = CriarNo(atributoMaiorGanho.Nome, atributoMaiorGanho.Legenda, Tipo.Categorico, valor.ToString(),
                                      novosAtributos[0].valores.Count, no.ValorTotal, novosAtributos.FirstOrDefault(atr => atr.Nome == _ClasseMeta.Nome).RetornaPorcentagemMaiorIncidencia(),
                                      no.Nivel + 1);
                    no.NosFilhos.Add(noAtual);

                    GerarC45(valor.ToString(), novosAtributos, noAtual);
                }
            }
            else if (atributoMaiorGanho.TipoAtributo == Tipo.Mineração_de_Texto)
            {
                List<string> palavrasTop = new List<string>();
                foreach (var palavra in atributoMaiorGanho.RetornaValoresDistintos())
                {
                    if (palavrasTop.Count(p => p.Equals(palavra)) <= 0)
                        palavrasTop.Add(palavra.ToString());
                }

                if (palavrasTop.Count <= 0)
                    return;
                else
                {
                    foreach (var palavra in palavrasTop)
                    {
                        var novosAtributos = CriarNovosAtributosMineracaoTexto(palavra.ToString(), atributoMaiorGanho, atributos);

                        noAtual = CriarNo(atributoMaiorGanho.Nome, atributoMaiorGanho.Legenda, Tipo.Mineração_de_Texto, palavra.ToString(),
                                          novosAtributos[0].valores.Count, no.ValorTotal,
                                          novosAtributos.FirstOrDefault(atr => atr.Nome == _ClasseMeta.Nome).RetornaPorcentagemMaiorIncidencia(), no.Nivel + 1);
                        no.NosFilhos.Add(noAtual);

                        GerarC45(palavra.ToString(), novosAtributos, noAtual);
                    }
                }
            }
            else
            {
                double valorDiscretizado = Math.Round(atributoMaiorGanho.Discretizar(), 2, MidpointRounding.AwayFromZero);
                var novosAtributos = CriarNovosAtributosContinuo(valorDiscretizado, atributoMaiorGanho, atributos, true);

                noAtual = CriarNo(atributoMaiorGanho.Nome,
                                  atributoMaiorGanho.Legenda,
                                  Tipo.Contínuo,
                                  " > " + valorDiscretizado.ToString(),
                                  novosAtributos[0].valores.Count,
                                  no.ValorTotal,
                                  novosAtributos.FirstOrDefault(atr => atr.Nome == _ClasseMeta.Nome).RetornaPorcentagemMaiorIncidencia(),
                                  no.Nivel + 1);
                no.NosFilhos.Add(noAtual);

                GerarC45("> " + valorDiscretizado.ToString(), novosAtributos, noAtual);

                novosAtributos = CriarNovosAtributosContinuo(valorDiscretizado, atributoMaiorGanho, atributos, false);

                noAtual = CriarNo(atributoMaiorGanho.Nome,
                                  atributoMaiorGanho.Legenda,
                                  Tipo.Contínuo,
                                  " <= " + valorDiscretizado.ToString(),
                                  novosAtributos[0].valores.Count,
                                  no.ValorTotal,
                                  novosAtributos.FirstOrDefault(atr => atr.Nome == _ClasseMeta.Nome).RetornaPorcentagemMaiorIncidencia(),
                                  no.Nivel + 1);
                no.NosFilhos.Add(noAtual);

                GerarC45("<= " + valorDiscretizado.ToString(), novosAtributos, noAtual);
            }
        }

        private No CriarNo(string nome, string label, Tipo tipoAtributo, string meta, float valorParcial, float total, int nivel)
        {
            return CriarNo(nome, label, tipoAtributo, meta, valorParcial, total, string.Empty, nivel);
        }

        /// <summary>
        /// Adiciona nó para a árvore
        /// </summary>
        /// <param name="nome"></param>
        /// <param name="label"></param>
        /// <param name="tipoAtributo"></param>
        /// <param name="meta"></param>
        /// <param name="valorParcial"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        private No CriarNo(string nome, string legenda, Tipo tipoAtributo, string meta, float valorParcial, float total, string maiorValorClasseMeta, int nivel)
        {
            No no = new No
            {
                Nome = nome,
                Label = legenda + (meta.Contains("<=") || meta.Contains(">") ? meta : " - " + meta),
                Tipo = tipoAtributo,
                Valor = meta.Replace("<= ", string.Empty).Replace("> ", string.Empty).Replace(",", "."),
                Percentual = Math.Round(Convert.ToDecimal((valorParcial * 100f) / total), 2, MidpointRounding.AwayFromZero),
                ValorTotal = (int)valorParcial,
                MaiorValorClasseMeta = maiorValorClasseMeta,
                Nivel = nivel,
                MenorIgual = tipoAtributo == Tipo.Contínuo && meta.Contains("<="),
                Legenda = legenda
            };

            PercentuaisClasseMeta.Add(maiorValorClasseMeta);
            return no;
        }

        /// <summary>
        /// Cria uma nova lista de atributos com meta (contínuo) para o próximo nível
        /// </summary>
        /// <param name="valor"></param>
        /// <param name="valoresClasseMeta"></param>
        /// <param name="atributos"></param>
        /// <returns></returns>
        private List<Atributo> CriarNovosAtributosCategorico(string valor, Atributo valoresClasseMeta, List<Atributo> atributos)
        {
            List<Atributo> novosAtributos = new List<Atributo>();
            for (int i = 0; i < valoresClasseMeta.valores.Count; i++)
            {
                if (valoresClasseMeta.valores[i].ToString().Equals(valor))
                {
                    foreach (var atributo in atributos)
                    {
                        Atributo novoAtributo;
                        if (novosAtributos.Exists(atr => atr.Nome == atributo.Nome))
                            novoAtributo = novosAtributos.FirstOrDefault(atr => atr.Nome == atributo.Nome);
                        else
                        {
                            novoAtributo = new Atributo { Nome = atributo.Nome, TipoAtributo = atributo.TipoAtributo, Legenda = atributo.Legenda };
                            novosAtributos.Add(novoAtributo);
                        }

                        novoAtributo.valores.Add(atributo.valores[i]);
                    }
                }
            }

            return novosAtributos;
        }

        private List<Atributo> CriarNovosAtributosMineracaoTexto(string valor, Atributo valoresClasseMeta, List<Atributo> atributos)
        {
            List<Atributo> novosAtributos = new List<Atributo>();
            for (int i = 0; i < valoresClasseMeta.valores.Count; i++)
            {
                if (valoresClasseMeta.valores[i].ToString().Contains(valor))
                {
                    //.Where(atr => !atr.Nome.Equals(valoresClasseMeta.Nome))
                    foreach (var atributo in atributos)
                    {
                        Atributo novoAtributo;
                        if (novosAtributos.Exists(atr => atr.Nome == atributo.Nome))
                            novoAtributo = novosAtributos.FirstOrDefault(atr => atr.Nome == atributo.Nome);
                        else
                        {
                            novoAtributo = new Atributo { Nome = atributo.Nome, TipoAtributo = atributo.TipoAtributo, Legenda = atributo.Legenda };
                            novosAtributos.Add(novoAtributo);
                        }

                        novoAtributo.valores.Add(atributo.valores[i]);
                    }
                }
            }

            return novosAtributos;
        }

        /// <summary>
        /// Cria uma lista de atributos com meta (categóríco e texto) para o próximo nível
        /// </summary>
        /// <param name="valor"></param>
        /// <param name="valoresClasseMeta"></param>
        /// <param name="atributos"></param>
        /// <param name="maior"></param>
        /// <returns></returns>
        private List<Atributo> CriarNovosAtributosContinuo(double valor, Atributo valoresClasseMeta, List<Atributo> atributos, bool maior)
        {
            List<Atributo> novosAtributos = new List<Atributo>();
            for (int i = 0; i < valoresClasseMeta.valores.Count; i++)
            {
                double ret = 0;
                bool converte = double.TryParse(valoresClasseMeta.valores[i].ToString(), out ret);

                if ((maior && ret > valor) || (!maior && ret <= valor))
                {

                    foreach (var atributo in atributos)
                    {
                        Atributo novoAtributo;
                        if (novosAtributos.Exists(atr => atr.Nome == atributo.Nome))
                            novoAtributo = novosAtributos.FirstOrDefault(atr => atr.Nome == atributo.Nome);
                        else
                        {
                            novoAtributo = new Atributo { Nome = atributo.Nome, TipoAtributo = atributo.TipoAtributo, Legenda = atributo.Legenda };
                            novosAtributos.Add(novoAtributo);
                        }

                        novoAtributo.valores.Add(atributo.valores[i]);
                    }
                }
            }

            return novosAtributos;
        }

        /// <summary>
        /// Cálcula o valor da entropia (atributo valoresClasseMeta.Atributo)
        /// </summary>
        /// <param name="valoresClasseMeta">Classe meta a ser calculada entropia</param>
        /// <returns>Retorna a entropia do atributo</returns>
        private double CalcularEntropia(Atributo valoresClasseMeta)
        {
            //Encontrar todas as variações de valores dentro dos exemplos para o atributo
            var valoresDistintos = valoresClasseMeta.RetornaValoresDistintos();

            //Total de informações (j) da fórmula
            float totalInformacoes = valoresClasseMeta.valores.Count;

            double entropia = 0;
            //Proporção de casos
            foreach (var valorDistinto in valoresDistintos)
            {
                float divisaoCasosTotal = valoresClasseMeta.RetornaProporcaoCasos(valorDistinto.ToString()) / totalInformacoes;
                entropia += (divisaoCasosTotal) * (Math.Log(divisaoCasosTotal, 2));
            }

            if (entropia < 0)
                entropia *= -1;

            return entropia;
        }

        /// <summary>
        /// Calcular a informação de ganho
        /// </summary>
        /// <returns></returns>
        private double CalcularGanho(double entropia, Atributo valoresClasseMeta, Atributo valoresAtributoCalculo)
        {
            double ganho = 0;
            //Lista com a soma dos (D)
            List<double> listaCalculoD = new List<double>();

            if (valoresAtributoCalculo.TipoAtributo == Tipo.Categorico)
            {
                #region Tipo diferente de número
                //Retorna os valores distintos dentro da coluna
                var valoresDistAtrCalculo = valoresAtributoCalculo.RetornaValoresDistintos();

                //Sobre cada atributo fazer cada possibilidade
                foreach (var valor in valoresDistAtrCalculo)
                {
                    //Total de casos deste valor dentro da coluna
                    float totalCasos = valoresAtributoCalculo.RetornaProporcaoCasos(valor.ToString());

                    //Retorna os diferentes valores contidos dentro da classeMeta para esse valor
                    var casosClasseMeta = valoresAtributoCalculo.RetornaCasosOutroExemplo(valor.ToString(), valoresClasseMeta.valores);
                    double casosSomados = 0;

                    foreach (var caso in casosClasseMeta)
                    {
                        double divisao = caso.Value / totalCasos;
                        casosSomados += divisao * (Math.Log(divisao, 2));
                    }

                    double calculoD = (totalCasos / valoresAtributoCalculo.valores.Count) * casosSomados;
                    listaCalculoD.Add(calculoD < 0 ? calculoD *= -1 : calculoD);
                }
                #endregion
            }
            else if (valoresAtributoCalculo.TipoAtributo == Tipo.Contínuo)
            {
                #region Ganho para números, realiza discretização
                double valorDiscretizado = Math.Round(valoresAtributoCalculo.Discretizar(), 2, MidpointRounding.AwayFromZero);

                //Conjunto maior
                float totalMaior = valoresAtributoCalculo.RetornaCasosConjunto(valorDiscretizado, true);
                var casosClasseMeta = valoresAtributoCalculo.RetornaCasosOutroExemplo(valorDiscretizado, true, valoresClasseMeta.valores);
                double casosSomados = 0;

                foreach (var caso in casosClasseMeta)
                {
                    float divisao = caso.Value / totalMaior;
                    casosSomados += divisao * (Math.Log(divisao, 2));
                }
                double calculoD = (totalMaior / valoresAtributoCalculo.valores.Count) * casosSomados;
                listaCalculoD.Add(calculoD < 0 ? calculoD *= -1 : calculoD);

                //Conjunto menor igual
                float totalMenorIgual = valoresAtributoCalculo.RetornaCasosConjunto(valorDiscretizado, false);
                casosClasseMeta = valoresAtributoCalculo.RetornaCasosOutroExemplo(valorDiscretizado, false, valoresClasseMeta.valores);
                casosSomados = 0;

                foreach (var caso in casosClasseMeta)
                {
                    float divisao = caso.Value / totalMenorIgual;
                    casosSomados += divisao * (Math.Log(divisao, 2));
                }
                calculoD = (totalMenorIgual / valoresAtributoCalculo.valores.Count) * casosSomados;
                listaCalculoD.Add(calculoD < 0 ? calculoD *= -1 : calculoD);
                #endregion
            }
            else
            {
                #region Ganho para textos
                List<string> palavrasTop = new List<string>();
                foreach (var valor in valoresAtributoCalculo.RetornaValoresDistintos())
                {
                    if (!string.IsNullOrEmpty(valor.ToString()))
                    {
                        foreach (var palavra in valor.ToString().Split(','))
                        {
                            if (!string.IsNullOrEmpty(palavra) && palavrasTop.Count(p => p.Equals(palavra)) <= 0)
                                palavrasTop.Add(palavra);
                        }
                    }
                }
                palavrasTop.Add(string.Empty);

                //Sobre cada atributo fazer cada possibilidade
                foreach (var valor in palavrasTop)
                {
                    //Total de casos deste valor dentro da coluna
                    float totalCasos = valoresAtributoCalculo.RetornaProporcaoCasos(valor.ToString());

                    //Retorna os diferentes valores contidos dentro da classeMeta para esse valor
                    var casosClasseMeta = valoresAtributoCalculo.RetornaCasosOutroExemplo(valor.ToString(), valoresClasseMeta.valores);

                    double casosSomados = 0;

                    foreach (var caso in casosClasseMeta)
                    {
                        double divisao = caso.Value / totalCasos;
                        casosSomados += divisao * (Math.Log(divisao, 2));
                    }

                    double calculoD = (totalCasos / valoresAtributoCalculo.valores.Count) * casosSomados;
                    listaCalculoD.Add(calculoD < 0 ? calculoD *= -1 : calculoD);
                }
                #endregion
            }


            //Entropia - (Soma da lista do cálculo do D)
            ganho = entropia - (listaCalculoD.Sum());

            return ganho;
        }
    }
}
