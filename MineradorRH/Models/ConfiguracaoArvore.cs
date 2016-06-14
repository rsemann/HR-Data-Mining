using ArvoreGeradora;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MineradorRH.Models
{
    public class ConfiguracaoArvore
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int ID { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        [DisplayName("Data da criação")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataCriacao { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [DisplayName("Comando SQL")]
        public string Sql { get; set; }

        public string Tabela { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [DisplayName("Descrição")]
        public string Descricao { get; set; }

        [DisplayName("Nível poda")]
        public int Poda { get; set; }

        public List<ConfiguracaoAtributo> RetornarListaConfiguracaoAtributos()
        {
            var configAtributos = new List<ConfiguracaoAtributo>();
            //cria a conexão com o banco de dados
            using (SqlConnection myConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Conexao"].ConnectionString))
            {
                myConnection.Open();
                using (SqlCommand myCommand = new SqlCommand(string.Format("SELECT * FROM {0}", Tabela), myConnection))
                {
                    using (SqlDataReader reader = myCommand.ExecuteReader())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            configAtributos.Add(new ConfiguracaoAtributo { Nome = reader.GetName(i), Legenda = reader.GetName(i), Tipo = Tipo.Categorico, ConfiguracaoArvoreID = ID });
                        }
                    }
                }

                myConnection.Close();
            }

            return configAtributos;
        }
    }
}