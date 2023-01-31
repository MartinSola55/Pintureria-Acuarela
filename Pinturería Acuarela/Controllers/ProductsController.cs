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
    public class ProductsController : Controller
    {
        private AcuarelaEntities db = new AcuarelaEntities();

        // GET: Products
        public ActionResult Index()
        {
            var product = db.Product.Include(p => p.Brand).Include(p => p.Capacity).Include(p => p.Category).Include(p => p.Color).Include(p => p.Subcategory);
            return View(product.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.id_brand = new SelectList(db.Brand, "id", "name");
            ViewBag.id_capacity = new SelectList(db.Capacity, "id", "capacity");
            ViewBag.id_category = new SelectList(db.Category, "id", "description");
            ViewBag.id_color = new SelectList(db.Color, "id", "name");
            ViewBag.id_subcategory = new SelectList(db.Subcategory, "id", "description");
            return View();
        }

        // POST: Products/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,description,id_brand,id_category,id_subcategory,id_capacity,id_color,quantity,internal_code,created_at,deleted_at")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.created_at= DateTime.Now;
                db.Product.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_brand = new SelectList(db.Brand, "id", "name", product.id_brand);
            ViewBag.id_capacity = new SelectList(db.Capacity, "id", "capacity", product.id_capacity);
            ViewBag.id_category = new SelectList(db.Category, "id", "description", product.id_category);
            ViewBag.id_color = new SelectList(db.Color, "id", "name", product.id_color);
            ViewBag.id_subcategory = new SelectList(db.Subcategory, "id", "description", product.id_subcategory);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_brand = new SelectList(db.Brand, "id", "name", product.id_brand);
            ViewBag.id_capacity = new SelectList(db.Capacity, "id", "capacity", product.id_capacity);
            ViewBag.id_category = new SelectList(db.Category, "id", "description", product.id_category);
            ViewBag.id_color = new SelectList(db.Color, "id", "name", product.id_color);
            ViewBag.id_subcategory = new SelectList(db.Subcategory, "id", "description", product.id_subcategory);
            return View(product);
        }

        // POST: Products/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,description,id_brand,id_category,id_subcategory,id_capacity,id_color,quantity,internal_code,created_at,deleted_at")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_brand = new SelectList(db.Brand, "id", "name", product.id_brand);
            ViewBag.id_capacity = new SelectList(db.Capacity, "id", "capacity", product.id_capacity);
            ViewBag.id_category = new SelectList(db.Category, "id", "description", product.id_category);
            ViewBag.id_color = new SelectList(db.Color, "id", "name", product.id_color);
            ViewBag.id_subcategory = new SelectList(db.Subcategory, "id", "description", product.id_subcategory);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Product.Find(id);
            db.Product.Remove(product);
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
