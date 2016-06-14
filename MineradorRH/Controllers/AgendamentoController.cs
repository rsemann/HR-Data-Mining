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

namespace MineradorRH.Controllers
{
    public class AgendamentoController : Controller
    {
        private DataContext db = new DataContext();

        // GET: Agendamento
        public ActionResult Index(int ArvoreId)
        {
            var agendamento = db.Agendamento.FirstOrDefault(arv => arv.ConfiguracaoArvoreID == ArvoreId);
            if (agendamento != null)
                agendamento.ConfiguracaoArvoreID = ArvoreId;
            else
                agendamento = new Agendamento { ConfiguracaoArvoreID = ArvoreId };
            return View(agendamento);
        }

        // GET: Agendamento/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Agendamento agendamento = db.Agendamento.Find(id);
            if (agendamento == null)
            {
                return HttpNotFound();
            }
            return View(agendamento);
        }

        // GET: Agendamento/Create
        public ActionResult Create()
        {
            ViewBag.ConfiguracaoArvoreID = new SelectList(db.ConfiguracaoArvore, "ID", "Nome");
            return View();
        }

        // POST: Agendamento/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Agendamento agendamento)
        {
            if (ModelState.IsValid)
            {
                db.Agendamento.RemoveRange(db.Agendamento.Where(ag => ag.ConfiguracaoArvoreID == agendamento.ConfiguracaoArvoreID));

                agendamento.DataCriacao = DateTime.Now;
                db.Agendamento.Add(agendamento);

                db.SaveChanges();
                return RedirectToAction("Index", "ConfiguracaoArvore");
            }

            ViewBag.ConfiguracaoArvoreID = new SelectList(db.ConfiguracaoArvore, "ID", "Nome", agendamento.ConfiguracaoArvoreID);
            return View(agendamento);
        }

        // GET: Agendamento/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Agendamento agendamento = db.Agendamento.Find(id);
            if (agendamento == null)
            {
                return HttpNotFound();
            }
            ViewBag.ConfiguracaoArvoreID = new SelectList(db.ConfiguracaoArvore, "ID", "Nome", agendamento.ConfiguracaoArvoreID);
            return View(agendamento);
        }

        // POST: Agendamento/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,FrequenciaAgendamento,Horario,DataCriacao,ConfiguracaoArvoreID")] Agendamento agendamento)
        {
            if (ModelState.IsValid)
            {
                db.Entry(agendamento).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ConfiguracaoArvoreID = new SelectList(db.ConfiguracaoArvore, "ID", "Nome", agendamento.ConfiguracaoArvoreID);
            return View(agendamento);
        }

        // GET: Agendamento/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Agendamento agendamento = db.Agendamento.Find(id);
            if (agendamento == null)
            {
                return HttpNotFound();
            }
            return View(agendamento);
        }

        // POST: Agendamento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Agendamento agendamento = db.Agendamento.Find(id);
            db.Agendamento.Remove(agendamento);
            db.SaveChanges();
            return RedirectToAction("Index");
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
