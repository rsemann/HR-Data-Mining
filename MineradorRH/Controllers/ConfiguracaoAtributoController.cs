using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MineradorRH.DAL;
using MineradorRH.Models;
using System.Data.SqlClient;
using ArvoreGeradora;
using System.IO;
using System.Web.Script.Serialization;
using System.Xml;
using System.Text;

namespace MineradorRH.Controllers
{
    public class ConfiguracaoAtributoController : Controller
    {
        private DataContext db = new DataContext();

        [HttpGet]
        // GET: ConfiguracaoAtributo
        public ActionResult Index(int ArvoreId)
        {
            var configAtributos = db.ConfiguracaoAtributo.Where(arvore => arvore.ConfiguracaoArvoreID == ArvoreId).ToList();

            if (configAtributos.Count() <= 0)
            {
                var configArvore = db.ConfiguracaoArvore.FirstOrDefault(arvore => arvore.ID == ArvoreId);

                configAtributos = configArvore.RetornarListaConfiguracaoAtributos();
            }
            return View(configAtributos);
        }

        // POST: ConfiguracaoAtributo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Index(IList<MineradorRH.Models.ConfiguracaoAtributo> atributos)
        {
            string classeMeta = atributos[0].ClasseMeta;

            if (string.IsNullOrEmpty(classeMeta))
            {
                ModelState.AddModelError("", "Um atributo deve ser classe meta.");
                return View(atributos);
            }

            if (atributos.Count(x => x.Tipo == Tipo.Mineração_de_Texto) > 1)
            {
                ModelState.AddModelError("", "Árvore pode possuir apenas um atributo do tipo Mineração de texto.");
                return View(atributos);
            }

            if (ModelState.IsValid)
            {
                foreach (var atributo in atributos.Where(atr => atr.ID > 0))
                {
                    db.Entry(atributo).State = EntityState.Modified;
                }

                foreach (var atributo in atributos.Where(atr => atr.ID <= 0))
                {
                    db.ConfiguracaoAtributo.Add(atributo);
                }

                db.SaveChanges();

                int arvoreID = atributos[0].ConfiguracaoArvoreID;
                var configuracaoArvore = db.ConfiguracaoArvore.FirstOrDefault(configArvore => configArvore.ID == arvoreID);

                //Gerar a árvore
                List<Atributo> atrs = new List<Atributo>();
                foreach (var atr in atributos)
                {
                    atrs.Add(new Atributo { Nome = atr.Nome, TipoAtributo = atr.Tipo, Legenda = atr.Legenda, Nivel = atr.Nivel });
                }

                TabelaTemporaria.PreencherAtributos(atrs, arvoreID);                

                C45 c45 = new C45();
                if (configuracaoArvore.Poda > 0)
                    c45.Poda = configuracaoArvore.Poda;
                c45.Calcular(classeMeta, atrs);

                ArvoreGerada arvoreGerada = new ArvoreGerada();
                arvoreGerada.ConfiguracaoArvoreID = arvoreID;
                arvoreGerada.ClasseMeta = atributos.FirstOrDefault(x => x.Nome.Equals(classeMeta)).Legenda;
                arvoreGerada.JSON = new JavaScriptSerializer().Serialize(c45.arvore.GerarJSON());
                arvoreGerada.UsuarioGeracao = User.Identity.Name;
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

                return View("Grafico", arvoreGerada);
            }

            return View(atributos);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
