using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Pinturería_Acuarela;

namespace Pinturería_Acuarela.Controllers
{
    public class CapacitiesController : Controller
    {
        private EFModel db = new EFModel();

        // GET: Capacities
        public ActionResult Index()
        {
            return View(db.Capacity.ToList());
        }

        // GET: Capacities/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Capacity capacity = db.Capacity.Find(id);
            if (capacity == null)
            {
                return HttpNotFound();
            }
            return View(capacity);
        }

        // GET: Capacities/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Capacities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,capacity")] Capacity cap)
        {
            if (ModelState.IsValid)
            {
                db.Capacity.Add(cap);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cap);
        }

        // GET: Capacities/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Capacity capacity = db.Capacity.Find(id);
            if (capacity == null)
            {
                return HttpNotFound();
            }
            return View(capacity);
        }

        // POST: Capacities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,capacity")] Capacity cap)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cap).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cap);
        }

        // GET: Capacities/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Capacity capacity = db.Capacity.Find(id);
            if (capacity == null)
            {
                return HttpNotFound();
            }
            return View(capacity);
        }

        // POST: Capacities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Capacity capacity = db.Capacity.Find(id);
            db.Capacity.Remove(capacity);
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
