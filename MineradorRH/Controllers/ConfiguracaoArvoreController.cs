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
using System.Configuration;

namespace MineradorRH.Controllers
{
    public class ConfiguracaoArvoreController : Controller
    {
        private DataContext db = new DataContext();

        // GET: ConfiguracaoArvore
        public ActionResult Index()
        {
            return View(db.ConfiguracaoArvore.ToList());
        }

        // GET: ConfiguracaoArvore/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfiguracaoArvore configuracaoArvore = db.ConfiguracaoArvore.Find(id);
            if (configuracaoArvore == null)
            {
                return HttpNotFound();
            }
            return View(configuracaoArvore);
        }

        // GET: ConfiguracaoArvore/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ConfiguracaoArvore/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ConfiguracaoArvore configuracaoArvore)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (db.AcessoBaseRh.Count() <= 0)
                        ModelState.AddModelError("", "Não foi realizado cadastro da conexão com a Base RH. Favor verificar.");
                    else
                    {
                        var acesso = db.AcessoBaseRh.FirstOrDefault();

                        if (!TabelaTemporaria.VerificarComando(configuracaoArvore.Sql, acesso.RetornaStringConexao()))
                            ModelState.AddModelError("", "Comando SQL possui problemas. Favor verificar.");
                        else
                        {
                            db.ConfiguracaoArvore.Add(configuracaoArvore);
                            db.SaveChanges();

                            configuracaoArvore.Tabela = TabelaTemporaria.Gerar(configuracaoArvore.Sql, configuracaoArvore.ID, acesso.RetornaStringConexao());
                            db.Entry(configuracaoArvore).State = EntityState.Modified;
                            db.SaveChanges();

                            return RedirectToAction("Index");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            return View(configuracaoArvore);
        }

        // GET: ConfiguracaoArvore/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ConfiguracaoArvore configuracaoArvore = db.ConfiguracaoArvore.Find(id);
            if (db.ArvoreGerada.Count(arvore => arvore.ConfiguracaoArvoreID == configuracaoArvore.ID) > 0)
                ViewBag.PossuiArvoreGerada = true;
            else
                ViewBag.PossuiArvoreGerada = false;

            if (configuracaoArvore == null)
            {
                return HttpNotFound();
            }
            return View(configuracaoArvore);
        }

        // POST: ConfiguracaoArvore/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ConfiguracaoArvore configuracaoArvore)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (db.AcessoBaseRh.Count() <= 0)
                        ModelState.AddModelError("", "Não foi realizado cadastro da conexão com a Base RH. Favor verificar.");
                    else
                    {
                        var acesso = db.AcessoBaseRh.FirstOrDefault();

                        if (!TabelaTemporaria.VerificarComando(configuracaoArvore.Sql, acesso.RetornaStringConexao()))
                            ModelState.AddModelError("", "Comando SQL possui problemas. Favor verificar.");
                        else
                        {
                            TabelaTemporaria.Excluir(configuracaoArvore.ID);
                            configuracaoArvore.Tabela = TabelaTemporaria.Gerar(configuracaoArvore.Sql, configuracaoArvore.ID, acesso.RetornaStringConexao());

                            db.Entry(configuracaoArvore).State = EntityState.Modified;
                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(configuracaoArvore);
        }

        // GET: ConfiguracaoArvore/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfiguracaoArvore configuracaoArvore = db.ConfiguracaoArvore.Find(id);
            if (configuracaoArvore == null)
            {
                return HttpNotFound();
            }
            return View(configuracaoArvore);
        }

        // POST: ConfiguracaoArvore/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (db.ArvoreGerada.Count(arvore => arvore.ConfiguracaoArvoreID == id) > 0)
                ModelState.AddModelError("", "Já existem arvores geradas para essa configuração. Exclusão não permitida.");
            else
            {
                ConfiguracaoArvore configuracaoArvore = db.ConfiguracaoArvore.Find(id);
                TabelaTemporaria.Excluir(configuracaoArvore.ID);
                db.ConfiguracaoAtributo.RemoveRange(db.ConfiguracaoAtributo.Where(atr => atr.ConfiguracaoArvoreID == id));
                db.Agendamento.RemoveRange(db.Agendamento.Where(atr => atr.ConfiguracaoArvoreID == id));
                db.ConfiguracaoArvore.Remove(configuracaoArvore);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View("Delete", db.ConfiguracaoArvore.Find(id));
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
