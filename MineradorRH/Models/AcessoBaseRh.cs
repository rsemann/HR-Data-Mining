using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MineradorRH.Models
{
    public class AcessoBaseRh
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Servidor { get; set; }

        [Required]
        [Display(Name = "Base de dados")]
        public string Base { get; set; }

        [Required]
        [Display(Name = "Usuário")]
        public string UsuarioAcesso { get; set; }

        [Required]
        [Display(Name = "Senha de acesso")]
        [DataType(DataType.Password)]
        public string SenhaAcesso { get; set; }

        public bool VerificarConexao()
        {
            try
            {
                using (SqlConnection myConnection = new SqlConnection(RetornaStringConexao()))
                {
                    myConnection.Open();
                    myConnection.Close();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string RetornaStringConexao()
        {
            return string.Format(@"server={0};user id={1};password={2};database={3};", Servidor, UsuarioAcesso, SenhaAcesso.ToString(), Base);
        }
    }
}