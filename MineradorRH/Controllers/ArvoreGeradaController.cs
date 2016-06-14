using ArvoreGeradora;
using MineradorRH.DAL;
using MineradorRH.Models;
using pmml4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;

namespace MineradorRH.Controllers
{
    public class ArvoreGeradaController : Controller
    {

        private DataContext db = new DataContext();
        // GET: ArvoreGerada
        public ActionResult ListaArvores(int configArvore)
        {
            return View(db.ArvoreGerada.Where(arvore => arvore.ConfiguracaoArvoreID == configArvore).ToList());
        }

        public ActionResult Details(int? arvoreGeradaId)
        {
            ArvoreGerada arvoreGerada = db.ArvoreGerada.Find(arvoreGeradaId);
            if (arvoreGerada != null)
                arvoreGerada.ConfiguracaoArvore = db.ConfiguracaoArvore.FirstOrDefault(t => t.ID == arvoreGerada.ConfiguracaoArvoreID);

            return View(arvoreGerada);
        }

        public ActionResult Index()
        {
            var arvores = db.ArvoreGerada.ToList();
            foreach (var arvore in arvores)
            {
                arvore.ConfiguracaoArvore = db.ConfiguracaoArvore.Find(arvore.ConfiguracaoArvoreID);
            }

            return View(arvores);
        }

        public ActionResult Consultas()
        {
            var arvores = db.ArvoreGerada.ToList();
            foreach (var arvore in arvores)
            {
                arvore.ConfiguracaoArvore = db.ConfiguracaoArvore.Find(arvore.ConfiguracaoArvoreID);
            }

            return View(arvores);
        }

        [HttpGet]
        public ActionResult Consultar(int arvoreGeradaId)
        {
            ViewBag.PossuiNosFilhos = true;
            ArvoreGerada arvoreGerada = db.ArvoreGerada.Find(arvoreGeradaId);
            if (arvoreGerada != null)
                arvoreGerada.ConfiguracaoArvore = db.ConfiguracaoArvore.FirstOrDefault(t => t.ID == arvoreGerada.ConfiguracaoArvoreID);

            Arvore arv;
            var xml = new System.Xml.Serialization.XmlSerializer(typeof(Arvore));
            using (StringReader textWriter = new StringReader(arvoreGerada.XML))
            {
                arv = xml.Deserialize(textWriter) as Arvore;
            }
            List<ConsultaPmml> atributosConsulta = new List<ConsultaPmml>();
            atributosConsulta.Add(new ConsultaPmml { Nome = arv.NoRaiz.Nome, Label = arv.NoRaiz.Legenda, ArvoreGeradaId = arvoreGeradaId, Valor = arv.NoRaiz.Valor, ClasseMeta = true });
            atributosConsulta.Add(new ConsultaPmml { Nome = arv.NoRaiz.NosFilhos[0].Nome, Label = arv.NoRaiz.NosFilhos[0].Legenda, ArvoreGeradaId = arvoreGeradaId });

            Session.Clear();
            Session.Add("ConsultaPMML", atributosConsulta);

            atributosConsulta = atributosConsulta.OrderByDescending(ord => ord.ClasseMeta == true).ToList();

            ViewBag.Resultado = string.Empty;
            return View(atributosConsulta);
        }

        [HttpPost]
        public ActionResult Consultar(List<ConsultaPmml> atributos)
        {
            try
            {
                ViewBag.PossuiNosFilhos = true;
                var arvoreGeradaId = atributos[0].ArvoreGeradaId;
                var arvoreGerada = db.ArvoreGerada.FirstOrDefault(arvore => arvore.ID == arvoreGeradaId);
                XmlDocument doc;
                var xml = new System.Xml.Serialization.XmlSerializer(typeof(XmlDocument));
                using (TextReader reader = new StringReader(arvoreGerada.XmlPmml))
                {
                    doc = (XmlDocument)xml.Deserialize(reader);
                }

                doc.Save(HostingEnvironment.MapPath("/Content/pmml.xml"));
                Pmml pmml = Pmml.loadModels(HostingEnvironment.MapPath("/Content/pmml.xml"));
                //busca o modelo
                ModelElement model = pmml.Models[0];

                Dictionary<string, object> dict = new Dictionary<string, object>();
                foreach (var atr in atributos)
                {
                    //Somente utiliza os fields ativos, pois o de predição não deve ser preenchido
                    if (!atr.ClasseMeta)
                    {
                        if (atr.Valor != null)
                            dict.Add(atr.Nome, atr.Valor);
                        else
                        {
                            ModelState.AddModelError("", "Campo " + atr.Label + " não foi preenchido.");
                            return View(atributos);
                        }
                    }
                }

                ScoreResult result = model.Score(dict);
                if (result.Value != null)
                {
                    Session.Clear();
                    Session.Add("ConsultaPMML", atributos);
                    ViewBag.Resultado = result.Value;

                    Arvore arv;
                    var xmlArvore = new System.Xml.Serialization.XmlSerializer(typeof(Arvore));
                    using (StringReader textWriter = new StringReader(arvoreGerada.XML))
                    {
                        arv = xmlArvore.Deserialize(textWriter) as Arvore;
                    }

                    //Verificar se possui filhos para esconder o avançar
                    foreach (var no in arv.NoRaiz.NosFilhos)
                    {
                        if (atributos[1].Nome == no.Nome && no.Valor == atributos[1].Valor)
                        {
                            if (no.NosFilhos.Count <= 0)
                                ViewBag.PossuiNosFilhos = false;

                            if (atributos.Count > 2)
                                VerificaProximoNivel(no, atributos, 2);

                            break;
                        }
                    }

                }
                else
                    ModelState.AddModelError("", "Consulta PMML não obteve resultado. Favor verificar as informações preenchidas.");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Consulta PMML não foi possível. Favor verificar as informações preenchidas.");
            }

            return View(atributos);
        }

        public void VerificaProximoNivel(No no, List<ConsultaPmml> atrConsulta, int indice, bool atributoRepetido = false)
        {
            foreach (var noInterno in no.NosFilhos)
            {
                decimal valor = 0;
                if (noInterno.Tipo == Tipo.Contínuo)
                {
                    if (noInterno.Valor.Contains("."))
                        valor = decimal.Parse(noInterno.Valor.ToString().Split('.')[0].Trim().ToString());
                    else
                        valor = decimal.Parse(noInterno.Valor.Trim());
                }

                if (atrConsulta[indice].Nome == noInterno.Nome &&
                    ((noInterno.Tipo == Tipo.Contínuo &&
                    (noInterno.MenorIgual ? decimal.Parse(atrConsulta[indice].Valor) <= valor : decimal.Parse(atrConsulta[indice].Valor) > valor)) ||
                     noInterno.Valor == atrConsulta[indice].Valor))
                {
                    if (noInterno.NosFilhos.Count <= 0)
                        ViewBag.PossuiNosFilhos = false;

                    if (noInterno.NosFilhos.Count > 0 && atrConsulta.Count(x => x.Nome == noInterno.NosFilhos[0].Nome) > 0)
                    {
                        for (int i = 0; i < atrConsulta.Count; i++)
                        {
                            if (atrConsulta[i].Nome == noInterno.NosFilhos[0].Nome)
                                VerificaProximoNivel(noInterno, atrConsulta, i, true);
                        }
                    }
                    else if (atrConsulta.Count > indice + 1 && !atributoRepetido)
                        VerificaProximoNivel(noInterno, atrConsulta, indice + 1);

                    break;
                }
            }
        }

        [HttpGet]
        public ActionResult Avancar()
        {
            ViewBag.PossuiNosFilhos = true;
            List<ConsultaPmml> consulta = Session["ConsultaPMML"] as List<ConsultaPmml>;
            ArvoreGerada arvoreGerada = db.ArvoreGerada.Find(consulta[0].ArvoreGeradaId);
            Arvore arv;

            var xml = new System.Xml.Serialization.XmlSerializer(typeof(Arvore));
            using (StringReader textWriter = new StringReader(arvoreGerada.XML))
            {
                arv = xml.Deserialize(textWriter) as Arvore;
            }

            foreach (var no in arv.NoRaiz.NosFilhos)
            {
                if (consulta[1].Nome == no.Nome && no.Valor == consulta[1].Valor)
                {
                    if (no.NosFilhos.Count <= 0)
                        ViewBag.PossuiNosFilhos = false;

                    if (consulta.Count > 2)
                        BuscaProximoNivel(no, consulta, 2);
                    else if (no.NosFilhos.Count > 0 && consulta.Count(x => x.Nome == no.NosFilhos[0].Nome) <= 0)
                        consulta.Add(new ConsultaPmml { Nome = no.NosFilhos[0].Nome, Label = no.NosFilhos[0].Legenda, ArvoreGeradaId = 0 });

                    break;
                }
            }

            return View("Consultar", consulta);
        }

        public void BuscaProximoNivel(No no, List<ConsultaPmml> atrConsulta, int indice, bool atributoRepetido = false)
        {
            foreach (var noInterno in no.NosFilhos)
            {
                decimal valor = 0;
                if (noInterno.Tipo == Tipo.Contínuo)
                {
                    if (noInterno.Valor.Contains("."))
                        valor = decimal.Parse(noInterno.Valor.ToString().Split('.')[0].Trim().ToString());
                    else
                        valor = decimal.Parse(noInterno.Valor.Trim());
                }

                if (atrConsulta[indice].Nome == noInterno.Nome &&
                    ((noInterno.Tipo == Tipo.Contínuo &&
                    (noInterno.MenorIgual ? decimal.Parse(atrConsulta[indice].Valor) <= valor : decimal.Parse(atrConsulta[indice].Valor) > valor)) ||
                     noInterno.Valor == atrConsulta[indice].Valor))
                {
                    if (noInterno.NosFilhos.Count <= 0)
                        ViewBag.PossuiNosFilhos = false;

                    if (noInterno.NosFilhos.Count > 0 && atrConsulta.Count(x => x.Nome == noInterno.NosFilhos[0].Nome) > 0)
                    {
                        for (int i = 0; i < atrConsulta.Count; i++)
                        {
                            if (atrConsulta[i].Nome == noInterno.NosFilhos[0].Nome)
                                BuscaProximoNivel(noInterno, atrConsulta, i, true);
                        }
                    }
                    else if (atrConsulta.Count > indice + 1 && !atributoRepetido)
                        BuscaProximoNivel(noInterno, atrConsulta, indice + 1);
                    else if (noInterno.NosFilhos.Count > 0 && atrConsulta.Count(x => x.Nome == noInterno.NosFilhos[0].Nome) <= 0)
                        atrConsulta.Add(new ConsultaPmml { Nome = noInterno.NosFilhos[0].Nome, Label = noInterno.NosFilhos[0].Legenda, ArvoreGeradaId = 0 });
                    
                    break;
                }
            }
        }

        public JsonResult RetornaArvore()
        {
            List<ConsultaPmml> consulta = Session["ConsultaPMML"] as List<ConsultaPmml>;
            if (consulta.Count <= 0)
                return null;
            ArvoreGerada arvoreGerada;
            arvoreGerada = db.ArvoreGerada.Find(consulta[0].ArvoreGeradaId);
            ViewBag.PossuiNosFilhos = true;

            if (arvoreGerada != null)
            {
                Arvore arv;
                Arvore arvoreNova = new Arvore();

                var xml = new System.Xml.Serialization.XmlSerializer(typeof(Arvore));
                using (StringReader textWriter = new StringReader(arvoreGerada.XML))
                {
                    arv = xml.Deserialize(textWriter) as Arvore;
                }

                arvoreNova.NoRaiz = new No() { Label = arv.NoRaiz.Legenda, ValorTotal = arv.NoRaiz.ValorTotal, Caminho = true };
                if (!string.IsNullOrEmpty(consulta[1].Valor))
                {
                    foreach (var no in arv.NoRaiz.NosFilhos)
                    {
                        No novoNo = new No
                        {
                            Label = no.Label,
                            ValorTotal = no.ValorTotal,
                            MaiorValorClasseMeta = no.MaiorValorClasseMeta,
                            Valor = (no.Tipo == Tipo.Contínuo ? (no.MenorIgual ? "<= " : "> ") : string.Empty) + no.Valor
                        };
                        arvoreNova.NoRaiz.NosFilhos.Add(novoNo);

                        if (consulta[1].Nome == no.Nome && no.Valor == consulta[1].Valor)
                        {
                            novoNo.Caminho = true;
                            if (novoNo.NosFilhos.Count <= 0)
                                ViewBag.PossuiNosFilhos = false;

                            if (consulta.Count > 2 && !string.IsNullOrEmpty(consulta[2].Valor))
                                CriaProximoNivel(no, consulta, 2, novoNo);
                        }
                    }
                }

                var json = new JavaScriptSerializer().Deserialize(new JavaScriptSerializer().Serialize(arvoreNova.GerarJSON()), typeof(JsonArvore));
                return this.Json(json, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public void CriaProximoNivel(No no, List<ConsultaPmml> atrConsulta, int indice, No noAcima, bool atributoRepetido = false)
        {
            if (string.IsNullOrEmpty(atrConsulta[indice].Valor))
                return;

            foreach (var noInterno in no.NosFilhos)
            {
                No novoNo = new No
                {
                    Label = noInterno.Label,
                    ValorTotal = noInterno.ValorTotal,
                    MaiorValorClasseMeta = noInterno.MaiorValorClasseMeta,
                    Valor = (noInterno.Tipo == Tipo.Contínuo ? (noInterno.MenorIgual ? "<= " : "> ") : string.Empty) + noInterno.Valor
                };
                noAcima.NosFilhos.Add(novoNo);

                decimal valor = 0;
                if (noInterno.Tipo == Tipo.Contínuo)
                {
                    if (noInterno.Valor.Contains("."))
                        valor = decimal.Parse(noInterno.Valor.ToString().Split('.')[0].Trim().ToString());
                    else
                        valor = decimal.Parse(noInterno.Valor.Trim());
                }

                if (atrConsulta[indice].Nome == noInterno.Nome &&
                    ((noInterno.Tipo == Tipo.Contínuo &&
                      (noInterno.MenorIgual ? decimal.Parse(atrConsulta[indice].Valor) <= valor
                                            : decimal.Parse(atrConsulta[indice].Valor) > valor)) ||
                     noInterno.Valor == atrConsulta[indice].Valor))
                {
                    novoNo.Caminho = true;

                    if (novoNo.NosFilhos.Count <= 0)
                        ViewBag.PossuiNosFilhos = false;

                    if (noInterno.NosFilhos.Count > 0 && atrConsulta.Count(x => x.Nome == noInterno.NosFilhos[0].Nome) > 0)
                    {
                        for (int i = 0; i < atrConsulta.Count; i++)
                        {
                            if (atrConsulta[i].Nome == noInterno.NosFilhos[0].Nome)
                                CriaProximoNivel(noInterno, atrConsulta, i, novoNo, true);
                        }
                    }
                    else if (atrConsulta.Count > indice + 1 && !atributoRepetido)
                        CriaProximoNivel(noInterno, atrConsulta, indice + 1, novoNo);
                }
            }
        }

        public JsonResult RetornaNuvemPalavras(int? id)
        {
            ArvoreGerada arvoreGerada = db.ArvoreGerada.Find(id);

            if (arvoreGerada != null && !string.IsNullOrEmpty(arvoreGerada.JsonNuvemPalavras))
            {
                var json = new JavaScriptSerializer().Deserialize(arvoreGerada.JsonNuvemPalavras, typeof(List<JsonNuvemPalavra>));
                return this.Json(json, JsonRequestBehavior.AllowGet);
            }
            return null;
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