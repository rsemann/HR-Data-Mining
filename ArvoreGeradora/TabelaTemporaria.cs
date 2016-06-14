using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArvoreGeradora
{
    public static class TabelaTemporaria
    {
        private static string Conexao;

        /// <summary>
        /// Testa o select passado, gera a lista de campos do select e cria a tabela temporária
        /// </summary>
        /// <param name="sql">Sql a ser buscado os campos para a nova tabela</param>
        /// <param name="id">Id para compor o nome da tabela</param>
        /// <returns></returns>
        public static string Gerar(string sql, int id, string conexao)
        {
            try
            {
                DataTable tb = new DataTable("TABELA" + id);
                Conexao = conexao;

                using (SqlConnection myConnection = new SqlConnection(conexao))
                {
                    myConnection.Open();
                    using (SqlCommand myCommand = new SqlCommand(sql, myConnection))
                    {
                        using (SqlDataReader reader = myCommand.ExecuteReader())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                tb.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
                            }
                        }
                    }

                    myConnection.Close();
                }

                try
                {
                    string comando = GerarComandoCriarTabela(tb);

                    using (SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["Conexao"].ConnectionString))
                    {
                        myConnection.Open();
                        using (SqlCommand myCommand = new SqlCommand(comando, myConnection))
                        {
                            myCommand.ExecuteNonQuery();
                        }

                        myConnection.Close();
                    }
                }
                catch
                {
                    throw new Exception("Erro ao criar tabela.");
                }

                IntegrarDadosTabela(sql, "TABELA" + id);
                return string.Format("TABELA{0}", id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void Excluir(int id)
        {
            try
            {
                using (SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["Conexao"].ConnectionString))
                {
                    myConnection.Open();
                    using (SqlCommand myCommand = new SqlCommand("DROP TABLE TABELA" + id, myConnection))
                    {
                        myCommand.ExecuteNonQuery();
                    }

                    myConnection.Close();
                }
            }
            catch (Exception)
            {
                throw new Exception("Erro ao tentar excluir a tabela temporária");
            }
        }

        public static bool VerificarComando(string sql, string conexao)
        {
            try
            {
                using (SqlConnection myConnection = new SqlConnection(conexao))
                {
                    myConnection.Open();
                    using (SqlCommand myCommand = new SqlCommand(sql, myConnection))
                    {
                        myCommand.ExecuteReader();
                    }

                    myConnection.Close();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void IntegrarDadosTabela(string sql, string tabela)
        {
            using (SqlConnection conexaoRh = new SqlConnection(Conexao))
            using (SqlBulkCopy copy = new SqlBulkCopy(ConfigurationManager.ConnectionStrings["Conexao"].ConnectionString))
            {
                {
                    copy.DestinationTableName = tabela;
                    conexaoRh.Open();
                    using (SqlCommand comandoRh = new SqlCommand(sql, conexaoRh))
                    {
                        using (SqlDataReader reader = comandoRh.ExecuteReader())
                        {
                            copy.WriteToServer(reader);
                        }
                    }
                }

                conexaoRh.Close();
            }
        }

        private static string GerarComandoCriarTabela(DataTable table)
        {
            StringBuilder sql = new StringBuilder();
            StringBuilder alterSql = new StringBuilder();

            sql.AppendFormat("CREATE TABLE [{0}] (", table.TableName);

            for (int i = 0; i < table.Columns.Count; i++)
            {

                sql.AppendFormat("\n\t[{0}]", table.Columns[i].ColumnName);

                switch (table.Columns[i].DataType.ToString().ToUpper())
                {
                    case "SYSTEM.INT16":
                        sql.Append(" smallint");
                        break;
                    case "SYSTEM.INT32":
                        sql.Append(" int");
                        break;
                    case "SYSTEM.INT64":
                        sql.Append(" bigint");
                        break;
                    case "SYSTEM.DATETIME":
                        sql.Append(" datetime");
                        break;
                    case "SYSTEM.STRING":
                        sql.AppendFormat(" varchar(MAX)");
                        break;
                    case "SYSTEM.SINGLE":
                        sql.Append(" single");
                        break;
                    case "SYSTEM.DOUBLE":
                        sql.Append(" float");
                        break;
                    case "SYSTEM.DECIMAL":
                        sql.AppendFormat(" decimal(18, 6)");
                        break;
                    default:
                        sql.AppendFormat(" varchar(MAX)");
                        break;
                }

                if (!table.Columns[i].AllowDBNull)
                {
                    sql.Append(" NOT NULL");
                }

                sql.Append(",");
            }

            sql.Remove(sql.Length - 1, 1);
            sql.AppendFormat("\n);\n{0}", alterSql.ToString());

            return sql.ToString();
        }

        private static List<KeyValuePair<string, string>> RetornaListaFrasePalavras(SqlConnection conexao, Nivel nivel)
        {
            var retornos = new List<KeyValuePair<string, string>>();
            StringBuilder sqlTexto = new StringBuilder();

            switch (nivel)
            {
                case Nivel.Primeiro:
                    sqlTexto.Append("SELECT TEXTO1, ");
                    sqlTexto.Append("       (SELECT PALAVRA FROM PALAVRAAGRUPADORA WHERE ID = (SELECT MAX(PALAVRAAGRUPADORAID) FROM DICIONARIOAGRUPADOR WHERE PALAVRASINONIMO = TEXTO2)) FROM RetornaPalavrasComentario()  ");
                    sqlTexto.Append(" WHERE TEXTO2 IN (SELECT PALAVRASINONIMO FROM DICIONARIOAGRUPADOR  ");
                    sqlTexto.Append("				   WHERE PALAVRAAGRUPADORAID = (SELECT TOP 1 PALAVRAAGRUPADORAID FROM  ");
                    sqlTexto.Append("												 (SELECT COUNT(TEXTO2) FREQUENCIA, (SELECT MAX(PALAVRAAGRUPADORAID)  ");
                    sqlTexto.Append("																					 FROM DICIONARIOAGRUPADOR WHERE PALAVRASINONIMO = TEXTO2) PALAVRAAGRUPADORAID ");
                    sqlTexto.Append("												  FROM RetornaPalavrasComentario()  ");
                    sqlTexto.Append("												   WHERE TEXTO2 IN (SELECT PALAVRASINONIMO FROM DICIONARIOAGRUPADOR) ");
                    sqlTexto.Append("	                                               GROUP BY TEXTO2 ");
                    sqlTexto.Append("												  ) MAIORFREQUENCIA ");
                    sqlTexto.Append("	                                            GROUP BY MAIORFREQUENCIA.PALAVRAAGRUPADORAID ");
                    sqlTexto.Append("                                                ORDER BY SUM(MAIORFREQUENCIA.FREQUENCIA) DESC ");
                    sqlTexto.Append("												) ");
                    sqlTexto.Append("				 )  ");
                    sqlTexto.Append("	 AND TEXTO1 NOT IN (select PALAVRA  from StopWord)  ");
                    sqlTexto.Append("  GROUP BY TEXTO1, TEXTO2  ");
                    sqlTexto.Append("  ORDER BY TEXTO1 ");
                    break;
                case Nivel.Segundo:
                    sqlTexto.Append("SELECT TEXTO1, TEXTO2 PALAVRA FROM RetornaPalavrasComentario() ");
                    sqlTexto.Append(" WHERE TEXTO2 IN (SELECT TABELA.TEXTO2 ");
                    sqlTexto.Append("                  FROM (SELECT TOP 1 COUNT(TEXTO2) CONTADOR, TEXTO2 ");
                    sqlTexto.Append("                         FROM RetornaPalavrasComentario() GROUP BY TEXTO2 ORDER BY 1 DESC) TABELA) ");
                    sqlTexto.Append("	 AND TEXTO1 NOT IN (select PALAVRA  from StopWord)  ");
                    sqlTexto.Append("GROUP BY TEXTO1, TEXTO2 ");
                    sqlTexto.Append("ORDER BY TEXTO1");
                    break;
                default:
                    sqlTexto.Append("SELECT TEXTO1, PALAVRAORIGINAL FROM RetornaPalavrasComentario() ");
                    sqlTexto.Append(" WHERE PALAVRAORIGINAL = (SELECT TOP 1 PALAVRAORIGINAL FROM RetornaPalavrasComentario() ");
                    sqlTexto.Append("						   GROUP BY PALAVRAORIGINAL ");
                    sqlTexto.Append("						   ORDER BY COUNT(PALAVRAORIGINAL) DESC)");
                    sqlTexto.Append("	 AND TEXTO1 NOT IN (select PALAVRA  from StopWord)  ");
                    break;
            }

            using (SqlCommand comando = new SqlCommand(sqlTexto.ToString(), conexao))
            {
                using (SqlDataReader reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        retornos.Add(new KeyValuePair<string, string>(reader[0].ToString(), reader[1].ToString()));
                    }
                }
            }

            return retornos;
        }

        public static void PreencherAtributos(List<Atributo> atributos, int configuracaoArvoreId)
        {
            List<KeyValuePair<string, string>> Textos = new List<KeyValuePair<string, string>>();

            //Limpar e preencher a tabela dos comentários de texto
            if (atributos.Count(x => x.TipoAtributo == Tipo.Mineração_de_Texto) > 0)
                PreencheTabelaTemporariaTexto(atributos.FirstOrDefault(x => x.TipoAtributo == Tipo.Mineração_de_Texto), "TABELA" + configuracaoArvoreId);

            //cria a conexão com o banco de dados
            using (SqlConnection myConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Conexao"].ConnectionString))
            {
                myConnection.Open();
                using (SqlCommand myCommand = new SqlCommand("SELECT * FROM TABELA" + configuracaoArvoreId, myConnection))
                {
                    using (SqlDataReader reader = myCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var atributo = atributos.FirstOrDefault(atr => atr.Nome.Equals(reader.GetName(i)));
                                if (atributo.TipoAtributo == Tipo.Mineração_de_Texto)
                                {
                                    if (Textos.Count <= 0)
                                        Textos = TabelaTemporaria.RetornaListaFrasePalavras(myConnection, atributo.Nivel);

                                    if (string.IsNullOrEmpty(reader[i].ToString()))
                                    {
                                        atributo.valores.Add("Sem comentário");
                                        continue;
                                    }

                                    #region Texto
                                    StringBuilder str = new StringBuilder();
                                    foreach (var item in Textos)
                                    {
                                        if (reader[i].ToString().Contains(item.Key))
                                        {
                                            str.Append(item.Value);
                                            break;
                                        }
                                    }
                                    if (string.IsNullOrEmpty(str.ToString()))
                                        atributo.valores.Add("Outras palavras");
                                    else
                                        atributo.valores.Add(str.ToString());
                                    #endregion
                                }
                                else
                                    atributo.valores.Add(reader[i]);
                            }
                        }
                    }
                }

                myConnection.Close();
            }
        }

        private static void PreencheTabelaTemporariaTexto(Atributo atributo, string tabela)
        {
            using (SqlConnection conexao = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Conexao"].ConnectionString))
            {
                conexao.Open();
                using (SqlCommand comando = new SqlCommand("DELETE TEXTOSMINERADOSTEMP", conexao))
                {
                    comando.ExecuteNonQuery();
                }

                using (SqlCommand comando = new SqlCommand(string.Format("UPDATE {1} SET COMENTARIO = '' WHERE COMENTARIO IS NULL", atributo.Nome, tabela), conexao))
                {
                    comando.ExecuteNonQuery();
                }

                using (SqlCommand comando = new SqlCommand(string.Format("INSERT INTO TEXTOSMINERADOSTEMP SELECT {0} FROM {1}", atributo.Nome, tabela), conexao))
                {
                    comando.ExecuteNonQuery();
                }

                conexao.Close();
            }
        }

        public static List<JsonNuvemPalavra> RetornaNuvemPalavras(Nivel nivel)
        {
            List<JsonNuvemPalavra> palavras = new List<JsonNuvemPalavra>();
            StringBuilder sqlTexto = new StringBuilder();
            float ajusteTamanho = 1;
            switch (nivel)
            {
                case Nivel.Primeiro:
                    sqlTexto.Append("SELECT TOTALIZACAO.TOTAL, ");
                    sqlTexto.Append("       (SELECT PALAVRA ");
                    sqlTexto.Append("        FROM   PALAVRAAGRUPADORA ");
                    sqlTexto.Append("        WHERE  ID = TOTALIZACAO.PALAVRAID) PALAVRA ");
                    sqlTexto.Append("FROM  (SELECT MAIORFREQUENCIA.PALAVRAAGRUPADORAID PALAVRAID, ");
                    sqlTexto.Append("              SUM(MAIORFREQUENCIA.FREQUENCIA)     TOTAL ");
                    sqlTexto.Append("       FROM   (SELECT Count(texto2)                     FREQUENCIA, ");
                    sqlTexto.Append("                      (SELECT Max(palavraagrupadoraid) ");
                    sqlTexto.Append("                       FROM   dicionarioagrupador ");
                    sqlTexto.Append("                       WHERE  palavrasinonimo = texto2) PALAVRAAGRUPADORAID ");
                    sqlTexto.Append("               FROM   Retornapalavrascomentario() ");
                    sqlTexto.Append("               WHERE  texto2 IN (SELECT palavrasinonimo ");
                    sqlTexto.Append("                                 FROM   dicionarioagrupador) ");
                    sqlTexto.Append("	              AND texto2 NOT IN (select PALAVRA  from StopWord)  ");
                    sqlTexto.Append("               GROUP  BY texto2) MAIORFREQUENCIA ");
                    sqlTexto.Append("       GROUP  BY MAIORFREQUENCIA.palavraagrupadoraid) TOTALIZACAO");
                    ajusteTamanho = 0.9f;
                    break;
                case Nivel.Segundo:
                    sqlTexto.Append("SELECT COUNT(TEXTO2) CONTADOR, TEXTO2 ");
                    sqlTexto.Append(" FROM RetornaPalavrasComentario() ");
                    sqlTexto.Append("	 WHERE TEXTO2 NOT IN (select PALAVRA  from StopWord)  ");
                    sqlTexto.Append("GROUP BY TEXTO2 ");
                    sqlTexto.Append("HAVING COUNT(TEXTO2) > 10 ");
                    sqlTexto.Append("ORDER BY COUNT(TEXTO2) DESC");
                    ajusteTamanho = 0.40f;
                    break;
                default:
                    sqlTexto.Append("SELECT COUNT(PALAVRAORIGINAL), PALAVRAORIGINAL FROM RetornaPalavrasComentario() ");
                    sqlTexto.Append("	 WHERE PALAVRAORIGINAL NOT IN (select PALAVRA  from StopWord)  ");
                    sqlTexto.Append(" GROUP BY PALAVRAORIGINAL ");
                    sqlTexto.Append(" HAVING COUNT(PALAVRAORIGINAL) > 20 ");
                    sqlTexto.Append("ORDER BY COUNT(PALAVRAORIGINAL) DESC");
                    break;
            }

            bool passou500 = false;
            using (SqlConnection myConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Conexao"].ConnectionString))
            {
                myConnection.Open();
                using (SqlCommand comando = new SqlCommand(sqlTexto.ToString(), myConnection))
                {
                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (int.Parse(reader[0].ToString()) > 500)
                                passou500 = true;
                            palavras.Add(new JsonNuvemPalavra { amount = int.Parse(reader[0].ToString()), sizeLetter = int.Parse(reader[0].ToString()), text = reader[1].ToString() });
                        }
                    }
                }
                myConnection.Close();
            }

            if (passou500)
            {
                for (int i = 0; i < palavras.Count; i++)
                {
                    var palavra = palavras[i];
                    palavra.sizeLetter = Convert.ToInt32(palavra.sizeLetter - (palavra.sizeLetter * ajusteTamanho));
                }
            }

            return palavras;
        }
    }
}
