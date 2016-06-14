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
    public class DicionarioAgrupadorController : Controller
    {
        private DataContext db = new DataContext();

        // GET: DicionarioAgrupador
        public ActionResult Index(int? id)
        {
            ViewBag.PalavraAgrupadora = db.PalavraAgrupadora.Find(id);
            var dicionarioAgrupador = db.DicionarioAgrupador.Where(d => d.PalavraAgrupadoraID == id).Include(d => d.PalavraAgrupadora);
            return View(dicionarioAgrupador.ToList());
        }

        // GET: DicionarioAgrupador/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DicionarioAgrupador dicionarioAgrupador = db.DicionarioAgrupador.Find(id);
            if (dicionarioAgrupador == null)
            {
                return HttpNotFound();
            }
            return View(dicionarioAgrupador);
        }

        // GET: DicionarioAgrupador/Create
        public ActionResult Create(int? id)
        {
            DicionarioAgrupador agrupador = new DicionarioAgrupador();
            agrupador.PalavraAgrupadoraID = id.Value;
            return View(agrupador);
        }

        // POST: DicionarioAgrupador/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Palavra,PalavraSinonimo,PalavraAgrupadoraID")] DicionarioAgrupador dicionarioAgrupador)
        {
            if (ModelState.IsValid)
            {
                db.DicionarioAgrupador.Add(dicionarioAgrupador);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = dicionarioAgrupador.PalavraAgrupadoraID });
            }

            ViewBag.PalavraAgrupadoraID = new SelectList(db.PalavraAgrupadora, "ID", "Palavra", dicionarioAgrupador.PalavraAgrupadoraID);
            return View(dicionarioAgrupador);
        }

        // GET: DicionarioAgrupador/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DicionarioAgrupador dicionarioAgrupador = db.DicionarioAgrupador.Find(id);
            if (dicionarioAgrupador == null)
            {
                return HttpNotFound();
            }
            ViewBag.PalavraAgrupadoraID = new SelectList(db.PalavraAgrupadora, "ID", "Palavra", dicionarioAgrupador.PalavraAgrupadoraID);
            return View(dicionarioAgrupador);
        }

        // POST: DicionarioAgrupador/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Palavra,PalavraSinonimo,PalavraAgrupadoraID")] DicionarioAgrupador dicionarioAgrupador)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dicionarioAgrupador).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = dicionarioAgrupador.PalavraAgrupadoraID });
            }
            ViewBag.PalavraAgrupadoraID = new SelectList(db.PalavraAgrupadora, "ID", "Palavra", dicionarioAgrupador.PalavraAgrupadoraID);
            return View(dicionarioAgrupador);
        }

        // GET: DicionarioAgrupador/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DicionarioAgrupador dicionarioAgrupador = db.DicionarioAgrupador.Find(id);
            if (dicionarioAgrupador == null)
            {
                return HttpNotFound();
            }
            return View(dicionarioAgrupador);
        }

        // POST: DicionarioAgrupador/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DicionarioAgrupador dicionarioAgrupador = db.DicionarioAgrupador.Find(id);
            db.DicionarioAgrupador.Remove(dicionarioAgrupador);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = dicionarioAgrupador.PalavraAgrupadoraID });
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
