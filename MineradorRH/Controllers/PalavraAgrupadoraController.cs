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
    public class PalavraAgrupadoraController : Controller
    {
        private DataContext db = new DataContext();

        // GET: PalavraAgrupadora
        public ActionResult Index()
        {
            return View(db.PalavraAgrupadora.ToList());
        }

        // GET: PalavraAgrupadora/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PalavraAgrupadora palavraAgrupadora = db.PalavraAgrupadora.Find(id);
            if (palavraAgrupadora == null)
            {
                return HttpNotFound();
            }
            return View(palavraAgrupadora);
        }

        // GET: PalavraAgrupadora/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PalavraAgrupadora/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Palavra")] PalavraAgrupadora palavraAgrupadora)
        {
            if (ModelState.IsValid)
            {
                db.PalavraAgrupadora.Add(palavraAgrupadora);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(palavraAgrupadora);
        }

        // GET: PalavraAgrupadora/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PalavraAgrupadora palavraAgrupadora = db.PalavraAgrupadora.Find(id);
            if (palavraAgrupadora == null)
            {
                return HttpNotFound();
            }
            return View(palavraAgrupadora);
        }

        // POST: PalavraAgrupadora/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Palavra")] PalavraAgrupadora palavraAgrupadora)
        {
            if (ModelState.IsValid)
            {
                db.Entry(palavraAgrupadora).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(palavraAgrupadora);
        }

        // GET: PalavraAgrupadora/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PalavraAgrupadora palavraAgrupadora = db.PalavraAgrupadora.Find(id);
            if (palavraAgrupadora == null)
            {
                return HttpNotFound();
            }
            return View(palavraAgrupadora);
        }

        // POST: PalavraAgrupadora/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PalavraAgrupadora palavraAgrupadora = db.PalavraAgrupadora.Find(id);
            db.PalavraAgrupadora.Remove(palavraAgrupadora);
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
