using MineradorRH.DAL;
using MineradorRH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MineradorRH.Controllers
{
    public class AcessoBaseRhController : Controller
    {
        private DataContext db = new DataContext();

        // GET: AcessoBaseRh
        [HttpGet]
        public ActionResult Index()
        {
            var acesso = db.AcessoBaseRh.FirstOrDefault();

            return View(acesso);
        }

        public ActionResult Index(AcessoBaseRh acesso)
        {
            if (ModelState.IsValid)
            {
                //Testar se a conexão funciona
                if (!acesso.VerificarConexao())
                    ModelState.AddModelError("", "Não foi possível estabelecer conexão. Favor verificar as informações cadastradas.");
                else
                {
                    db.AcessoBaseRh.RemoveRange(db.AcessoBaseRh.Where(x => x.ID > 0));
                    db.AcessoBaseRh.Add(acesso);
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
            }

            return View(acesso);
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