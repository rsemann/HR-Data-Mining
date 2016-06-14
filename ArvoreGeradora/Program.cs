using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ArvoreGeradora
{
    class Program
    {
        static void Main(string[] args)
        {

            //List<Atributo> atributos = new List<Atributo>();
            ////cria a conexão com o banco de dados
            //using (SqlConnection myConnection = new SqlConnection(@"server=.\SQLEXPRESS;" +
            //                           "Trusted_Connection=yes;" +
            //                           "database=TstDatamining; " +
            //                           "connection timeout=30"))
            //{
            //    myConnection.Open();
            //    using (SqlCommand myCommand = new SqlCommand("SELECT IDADE, TEMFILHOS, SEXO, CAUSADEMISSAO2, UNIDADE, CARGO, HIERARQUIA, " +
            //                          " ESTADO_CIVIL, NIVEL_ESCOLARIDADE, TURNO, HORASMES, HORASSEMANA FROM RHRAFAEL WHERE CAUSADEMISSAO2 = 0", myConnection))
            //    {
            //        using (SqlDataReader reader = myCommand.ExecuteReader())
            //        {


            //            for (int i = 0; i < reader.FieldCount; i++)
            //            {
            //                atributos.Add(new Atributo { Nome = reader.GetName(i), TipoAtributo = typeof(string) });
            //            }

            //            while (reader.Read())
            //            {
            //                for (int i = 0; i < reader.FieldCount; i++)
            //                {
            //                    var atributo = atributos.FirstOrDefault(atr => atr.Nome.Equals(reader.GetName(i)));
            //                    atributo.valores.Add(reader[i]);
            //                    if (reader[i].GetType() == typeof(double) || reader[i].GetType() == typeof(int) || reader[i].GetType() == typeof(long))
            //                        atributo.TipoAtributo = reader[i].GetType();

            //                }
            //            }
            //        }
            //    }

            //    myConnection.Close();
            //}



            //C45 c45 = new C45();
            //c45.Calcular("CAUSADEMISSAO2", atributos);

            //Console.ReadKey();
        }

        //static void Main(string[] args)
        //{
        //    List<Atributo> exemplos = new List<Atributo>();

        //    Atributo atrJoga = new Atributo
        //    {
        //        Nome = "Joga",
        //        TipoAtributo = typeof(string),
        //        valores = new System.Collections.ArrayList { "Não", "Não", "Sim", "Sim", "Sim", "Não", "Sim", "Não", "Sim", "Sim", "Sim", "Sim", "Sim", "Não" }
        //    };
        //    exemplos.Add(atrJoga);

        //    Atributo atrTempo = new Atributo
        //    {
        //        Nome = "Tempo",
        //        TipoAtributo = typeof(string),
        //        valores = new System.Collections.ArrayList { "Sol", "Sol", "Nublado", "Chuva", "Chuva", "Chuva", "Nublado", "Sol", "Sol", "Chuva", "Sol", "Nublado", "Nublado", "Chuva" }
        //    };
        //    exemplos.Add(atrTempo);

        //    Atributo atrTemperatura = new Atributo
        //    {
        //        Nome = "Temperatura",
        //        TipoAtributo = typeof(double),
        //        valores = new System.Collections.ArrayList { 85, 80, 83, 70, 68, 65, 64, 72, 69, 75, 75, 72, 81, 71 }
        //    };
        //    exemplos.Add(atrTemperatura);

        //    Atributo atrUmidade = new Atributo
        //    {
        //        Nome = "Umidade",
        //        TipoAtributo = typeof(double),
        //        valores = new System.Collections.ArrayList { 85, 90, 86, 96, 80, 70, 65, 95, 70, 80, 70, 90, 75, 91 }
        //    };
        //    exemplos.Add(atrUmidade);

        //    Atributo atrVento = new Atributo
        //    {
        //        Nome = "Vento",
        //        TipoAtributo = typeof(string),
        //        valores = new System.Collections.ArrayList { "Não", "Sim", "Não", "Não", "Não", "Sim", "Sim", "Não", "Não", "Não", "Sim", "Sim", "Não", "Sim" }
        //    };
        //    exemplos.Add(atrVento);

        //    C45 c45 = new C45();
        //    c45.Calcular("Joga", exemplos);

        //    Console.ReadKey();
        //}

        //static void Main(string[] args)
        //{
        //    List<Atributo> exemplos = new List<Atributo>();

        //    Atributo atrJoga = new Atributo
        //    {
        //        Nome = "A1",
        //        TipoAtributo = typeof(string),
        //        valores = new System.Collections.ArrayList { "T", "T", "T", "F", "F", "F" }
        //    };
        //    exemplos.Add(atrJoga);

        //    Atributo atrTempo = new Atributo
        //    {
        //        Nome = "A2",
        //        TipoAtributo = typeof(string),
        //        valores = new System.Collections.ArrayList { "T", "T", "F", "F", "T", "T" }
        //    };
        //    exemplos.Add(atrTempo);

        //    Atributo atrTemperatura = new Atributo
        //    {
        //        Nome = "F",
        //        TipoAtributo = typeof(string),
        //        valores = new System.Collections.ArrayList { "Sim", "Sim", "Não", "Sim", "Não", "Não" }
        //    };
        //    exemplos.Add(atrTemperatura);

        //    C45 c45 = new C45();
        //    c45.Calcular("F", exemplos);

        //    Console.ReadKey();
        //}

        //static void Main(string[] args)
        //{
        //    List<Atributo> exemplos = new List<Atributo>();

        //    Atributo atrTemperatura = new Atributo
        //    {
        //        Nome = "Temperatura do Corpo",
        //        TipoAtributo = typeof(string),
        //        valores = new System.Collections.ArrayList { "sangue quente", "sangue quente", "sangue quente", "sangue quente", "sangue frio", "sangue frio", "sangue frio", "sangue frio",
        //                                                     "sangue quente", "sangue frio"}
        //    };
        //    exemplos.Add(atrTemperatura);

        //    Atributo atrOrigina = new Atributo
        //    {
        //        Nome = "Original",
        //        TipoAtributo = typeof(string),
        //        valores = new System.Collections.ArrayList { "sim", "sim", "sim", "sim", "não", "não", "não", "não", "não", "sim" }
        //    };
        //    exemplos.Add(atrOrigina);

        //    Atributo atrQuatroPatas = new Atributo
        //    {
        //        Nome = "Quatro patas",
        //        TipoAtributo = typeof(string),
        //        valores = new System.Collections.ArrayList { "sim", "sim", "não", "não", "sim", "sim", "não", "não", "não", "não" }
        //    };
        //    exemplos.Add(atrQuatroPatas);

        //    Atributo atrHiberna = new Atributo
        //    {
        //        Nome = "Hiberna",
        //        TipoAtributo = typeof(string),
        //        valores = new System.Collections.ArrayList { "sim", "não", "sim", "não", "sim", "não", "sim", "não", "não", "não" }
        //    };
        //    exemplos.Add(atrHiberna);

        //    Atributo atrMamifero = new Atributo
        //    {
        //        Nome = "Mamífero",
        //        TipoAtributo = typeof(string),
        //        valores = new System.Collections.ArrayList { "sim", "sim", "sim", "sim", "não", "não", "não", "não", "não", "não" }
        //    };
        //    exemplos.Add(atrMamifero);

        //    C45 c45 = new C45();
        //    c45.Calcular("Mamífero", exemplos);

        //    Console.ReadKey();
        //}

        //static void Main(string[] args)
        //{
        //    List<Atributo> exemplos = new List<Atributo>();

        //    Atributo atrEscolaridade = new Atributo
        //    {
        //        Nome = "Escolaridade",
        //        TipoAtributo = typeof(string),
        //        valores = new System.Collections.ArrayList { "Mestrado", "Doutorado", "Mestrado", "Doutorado", "Graduação", "Graduação", "Mestrado", "Mestrado"}
        //    };
        //    exemplos.Add(atrEscolaridade);

        //    Atributo atrIdade = new Atributo
        //    {
        //        Nome = "Idade",
        //        TipoAtributo = typeof(double),
        //        valores = new System.Collections.ArrayList { 35, 30, 28, 38, 21, 45, 42, 21 }
        //    };
        //    exemplos.Add(atrIdade);

        //    Atributo atrRico = new Atributo
        //    {
        //        Nome = "Rico",
        //        TipoAtributo = typeof(string),
        //        valores = new System.Collections.ArrayList { "sim", "sim", "não", "sim", "não", "não", "sim", "não" }
        //    };
        //    exemplos.Add(atrRico);

        //    C45 c45 = new C45();
        //    c45.Calcular("Rico", exemplos);

        //    Console.ReadKey();
        //}

        //static void Main(string[] args)
        //{
        //    //cria a conexão com o banco de dados
        //    OleDbConnection aConnection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Program Files\VisiRex\Data\Cars.mdb");

        //    //cria o objeto command and armazena a consulta SQL
        //    OleDbCommand aCommand = new OleDbCommand("select * from Cars", aConnection);
        //    try
        //    {
        //        aConnection.Open();
        //        //cria o objeto datareader para fazer a conexao com a tabela
        //        OleDbDataReader aReader = aCommand.ExecuteReader();
        //        Console.WriteLine("Os valores retornados da tabela são : ");

        //        List<Atributo> atributos = new List<Atributo>();

        //        for (int i = 0; i < aReader.FieldCount; i++)
        //        {
        //            if (aReader.GetName(i) != "ID" && aReader.GetName(i) != "Year" && aReader.GetName(i) != "Model")
        //                atributos.Add(new Atributo { Nome = aReader.GetName(i), TipoAtributo = typeof(string) });
        //        }

        //        //Faz a interação com o banco de dados lendo os dados da tabela
        //        while (aReader.Read())
        //        {
        //            for (int i = 0; i < aReader.FieldCount; i++)
        //            {
        //                if (aReader.GetName(i) != "ID" && aReader.GetName(i) != "Year" && aReader.GetName(i) != "Model")
        //                {
        //                    var atributo = atributos.FirstOrDefault(atr => atr.Nome.Equals(aReader.GetName(i)));
        //                    atributo.valores.Add(aReader[i]);
        //                    if (aReader[i].GetType() != atributo.TipoAtributo)
        //                        atributo.TipoAtributo = aReader[i].GetType();
        //                }
        //            }
        //        }
        //        //fecha o reader
        //        aReader.Close();
        //        //fecha a conexao 
        //        aConnection.Close();

        //        C45 c45 = new C45();
        //        c45.Calcular("Origin", atributos);

        //        Console.ReadKey();
        //    }
        //    //Trata a exceção
        //    catch (OleDbException e)
        //    {
        //        Console.WriteLine("Error: {0}", e.Errors[0].Message);
        //    }
        //}


        //static void Main(string[] args)
        //{
        //    List<Atributo> exemplos = new List<Atributo>();

        //    Atributo atrTemperatura = new Atributo
        //    {
        //        Nome = "Temperatura do Corpo",
        //        TipoAtributo = typeof(string),
        //        valores = new System.Collections.ArrayList { "sangue frio", "sangue frio", "sangue quente", "sangue quente", "sangue quente" }
        //    };
        //    exemplos.Add(atrTemperatura);

        //    Atributo atrOrigina = new Atributo
        //    {
        //        Nome = "Original",
        //        TipoAtributo = typeof(string),
        //        valores = new System.Collections.ArrayList { "não", "sim", "não", "não", "não" }
        //    };
        //    exemplos.Add(atrOrigina);

        //    Atributo atrQuatroPatas = new Atributo
        //    {
        //        Nome = "Quatro patas",
        //        TipoAtributo = typeof(string),
        //        valores = new System.Collections.ArrayList { "sim", "não", "não", "não", "sim" }
        //    };
        //    exemplos.Add(atrQuatroPatas);

        //    Atributo atrHiberna = new Atributo
        //    {
        //        Nome = "Hiberna",
        //        TipoAtributo = typeof(string),
        //        valores = new System.Collections.ArrayList { "sim", "não", "não", "sim", "sim" }
        //    };
        //    exemplos.Add(atrHiberna);

        //    Atributo atrMamifero = new Atributo
        //    {
        //        Nome = "Mamífero",
        //        TipoAtributo = typeof(string),
        //        valores = new System.Collections.ArrayList { "não", "não", "não", "não", "sim" }
        //    };
        //    exemplos.Add(atrMamifero);

        //    C45 c45 = new C45();
        //    c45.Calcular("Mamífero", exemplos);

        //    Console.ReadKey();
        //}
    }
}
