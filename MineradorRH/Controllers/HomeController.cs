using ArvoreGeradora;
using MineradorRH.DAL;
using MineradorRH.Models;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Data.Entity.Migrations;
using System.Resources;
using MineradorRH.Migrations;
namespace MineradorRH.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private DataContext db = new DataContext();

        public ActionResult Index()
        {
            

            ArvoreGerada arvoreGerada = db.ArvoreGerada.OrderByDescending(arvore => arvore.DataGeracao).FirstOrDefault();
            if (arvoreGerada != null)
                arvoreGerada.ConfiguracaoArvore = db.ConfiguracaoArvore.FirstOrDefault(t => t.ID == arvoreGerada.ConfiguracaoArvoreID);
            return View(arvoreGerada);
        }

        public JsonResult BuscarGrafico(int? id)
        {
            ArvoreGerada arvoreGerada;
            if (id.HasValue)
                arvoreGerada = db.ArvoreGerada.Find(id.Value);
            else
                arvoreGerada = db.ArvoreGerada.OrderByDescending(arvore => arvore.DataGeracao).FirstOrDefault();

            if (arvoreGerada != null)
            {
                var a = new JavaScriptSerializer().Deserialize(arvoreGerada.JSON, typeof(JsonArvore));

                return this.Json(a, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}