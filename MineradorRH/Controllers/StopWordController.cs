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
    public class StopWordController : Controller
    {
        private DataContext db = new DataContext();

        // GET: StopWord
        public ActionResult Index()
        {
            return View(db.StopWord.ToList());
        }

        // GET: StopWord/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StopWord stopWord = db.StopWord.Find(id);
            if (stopWord == null)
            {
                return HttpNotFound();
            }
            return View(stopWord);
        }

        // GET: StopWord/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StopWord/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Palavra")] StopWord stopWord)
        {
            if (ModelState.IsValid)
            {
                db.StopWord.Add(stopWord);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(stopWord);
        }

        // GET: StopWord/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StopWord stopWord = db.StopWord.Find(id);
            if (stopWord == null)
            {
                return HttpNotFound();
            }
            return View(stopWord);
        }

        // POST: StopWord/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Palavra")] StopWord stopWord)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stopWord).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(stopWord);
        }

        // GET: StopWord/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StopWord stopWord = db.StopWord.Find(id);
            if (stopWord == null)
            {
                return HttpNotFound();
            }
            return View(stopWord);
        }

        // POST: StopWord/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StopWord stopWord = db.StopWord.Find(id);
            db.StopWord.Remove(stopWord);
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
