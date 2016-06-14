using MineradorRH.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using MineradorRH.Models;
using ArvoreGeradora;
using System.Web.Script.Serialization;
using System.IO;

namespace MineradorRHServico
{
    public class Library
    {
        DataContext db = new DataContext();

        public void Gerar()
        {
            foreach (Agendamento agendamento in db.Agendamento.Where(ag => ag.Ativo).ToList())
            {
                if (PermitiGerar(agendamento))
                {
                    var acesso = db.AcessoBaseRh.FirstOrDefault();

                    //Deve conter informações para acesso na base RH e deve ter os atributos da árvore configurados
                    if (acesso != null && db.ConfiguracaoAtributo.Count(config => config.ConfiguracaoArvoreID == agendamento.ConfiguracaoArvoreID) > 0)
                    {
                        //Toda vez que integrar deve excluir a tabela e gerar novamente para buscar as novas informações
                        var configuracaoArvore = db.ConfiguracaoArvore.FirstOrDefault(config => config.ID == agendamento.ConfiguracaoArvoreID);
                        TabelaTemporaria.Excluir(configuracaoArvore.ID);
                        configuracaoArvore.Tabela = TabelaTemporaria.Gerar(configuracaoArvore.Sql, configuracaoArvore.ID, acesso.RetornaStringConexao());
                        db.Entry(configuracaoArvore).State = EntityState.Modified;
                        db.SaveChanges();

                        RodarArvore(agendamento.ConfiguracaoArvoreID);
                        agendamento.UltimaExecucao = DateTime.Now;
                        db.Entry(agendamento).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
        }

        private bool PermitiGerar(Agendamento agendamento)
        {
            if (agendamento.UltimaExecucao.HasValue && agendamento.UltimaExecucao.Value.Date == DateTime.Now.Date)
                return false;

            var horario = agendamento.Horario.TimeOfDay;
            var horaAtual = DateTime.Now.TimeOfDay;
            if (agendamento.FrequenciaAgendamento == FrequenciaAgendamento.Dia)
                return (horario.TotalMinutes >= (horaAtual.TotalMinutes - 10) && horario.TotalMinutes <= (horaAtual.TotalMinutes + 10));
            else if (agendamento.FrequenciaAgendamento == FrequenciaAgendamento.Semana)
                return ((!agendamento.UltimaExecucao.HasValue || agendamento.UltimaExecucao.Value.AddDays(7).Date == DateTime.Now.Date) &&
                        (horario.TotalMinutes >= (horaAtual.TotalMinutes - 10) && horario.TotalMinutes <= (horaAtual.TotalMinutes + 10)));
            else
                return ((!agendamento.UltimaExecucao.HasValue || agendamento.UltimaExecucao.Value.AddMonths(1).Date == DateTime.Now.Date) &&
                            (horario.TotalMinutes >= (horaAtual.TotalMinutes - 10) && horario.TotalMinutes <= (horaAtual.TotalMinutes + 10)));
        }

        private void RodarArvore(int configuracaoArvoreId)
        {
            var atributos = db.ConfiguracaoAtributo.Where(config => config.ConfiguracaoArvoreID == configuracaoArvoreId).ToList();
            string classeMeta = atributos[0].ClasseMeta;
            var configuracaoArvore = db.ConfiguracaoArvore.FirstOrDefault(configArvore => configArvore.ID == configuracaoArvoreId);

            //Gerar a árvore
            List<Atributo> atrs = new List<Atributo>();
            foreach (var atr in atributos)
            {
                atrs.Add(new Atributo { Nome = atr.Nome, TipoAtributo = atr.Tipo, Legenda = atr.Legenda });
            }

            TabelaTemporaria.PreencherAtributos(atrs, configuracaoArvoreId);

            C45 c45 = new C45();
            if (configuracaoArvore.Poda > 0)
                c45.Poda = configuracaoArvore.Poda;
            c45.Calcular(classeMeta, atrs);

            ArvoreGerada arvoreGerada = new ArvoreGerada();
            arvoreGerada.ConfiguracaoArvoreID = configuracaoArvoreId;
            arvoreGerada.ClasseMeta = atributos.FirstOrDefault(x => x.Nome.Equals(classeMeta)).Legenda;
            arvoreGerada.JSON = new JavaScriptSerializer().Serialize(c45.arvore.GerarJSON());
            arvoreGerada.UsuarioGeracao = "Agendamento";
            arvoreGerada.DataGeracao = DateTime.Now;
            var xml = new System.Xml.Serialization.XmlSerializer(c45.arvore.GetType());
            using (StringWriter textWriter = new StringWriter())
            {
                xml.Serialize(textWriter, c45.arvore);
                arvoreGerada.XML = textWriter.ToString();
            }
            var doc = new GeradorPMML().Gerar(atrs, classeMeta, c45.arvore, c45.PercentuaisClasseMeta);
            xml = new System.Xml.Serialization.XmlSerializer(doc.GetType());
            using (StringWriter textWriter = new StringWriter())
            {
                xml.Serialize(textWriter, doc);
                arvoreGerada.XmlPmml = textWriter.ToString();
            }

            if (atributos.Count(x => x.Tipo == Tipo.Mineração_de_Texto) > 0)
            {
                arvoreGerada.JsonNuvemPalavras = new JavaScriptSerializer().Serialize(TabelaTemporaria.RetornaNuvemPalavras(atributos.FirstOrDefault(x => x.Tipo == Tipo.Mineração_de_Texto).Nivel));
            }

            db.ArvoreGerada.Add(arvoreGerada);
            db.SaveChanges();
        }
    }
}
