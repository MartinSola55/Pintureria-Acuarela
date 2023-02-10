using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Pinturería_Acuarela;
using Pinturería_Acuarela.Filter;

namespace Pinturería_Acuarela.Controllers
{
    [Admin]
    public class ProductsController : Controller
    {
        private EFModel db = new EFModel();

        // GET: Products
        public ActionResult Index()
        {
            if (TempData.Count == 1)
            {
                ViewBag.Message = TempData["Message"].ToString();
            }
            else if (TempData.Count == 2)
            {
                ViewBag.Message = TempData["Message"].ToString();
                ViewBag.Error = TempData["Error"];
            }
            ViewBag.id_brand = new SelectList(db.Brand, "id", "name");
            ViewBag.id_capacity = new SelectList(db.Capacity.OrderByDescending(c => c.capacity), "id", "description");
            ViewBag.id_category = new SelectList(db.Category, "id", "description");
            ViewBag.id_color = new SelectList(db.Color, "id", "name");
            ViewBag.id_subcategory = new SelectList(db.Subcategory, "id", "description");
            var product = db.Product.Include(p => p.Brand).Include(p => p.Capacity).Include(p => p.Category).Include(p => p.Color).Include(p => p.Subcategory);
            return View(product.ToList());
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.id_brand = new SelectList(db.Brand, "id", "name");
            ViewBag.id_capacity = new SelectList(db.Capacity.OrderByDescending(c => c.capacity), "id", "description");
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
            try
            {
                if (ModelState.IsValid)
                {
                    product.created_at= DateTime.Now;
                    db.Product.Add(product);
                    db.SaveChanges();
                    TempData["Message"] = "El producto se creó correctamente";
                    return RedirectToAction("Index");
                } else
                {
                    ViewBag.Message = "Alguno de los campos ingresados no es válido";
                    ViewBag.Error = 1;
                }

                ViewBag.id_brand = new SelectList(db.Brand, "id", "name", product.id_brand);
                ViewBag.id_capacity = new SelectList(db.Capacity.OrderByDescending(c => c.capacity), "id", "description", product.id_capacity);
                ViewBag.id_category = new SelectList(db.Category, "id", "description", product.id_category);
                ViewBag.id_color = new SelectList(db.Color, "id", "name", product.id_color);
                ViewBag.id_subcategory = new SelectList(db.Subcategory, "id", "description", product.id_subcategory);
                return View(product);
            }
            catch (Exception)
            {
                TempData["Message"] = "Ha ocurrido un error inesperado. No se ha podido cargar el producto";
                TempData["Error"] = 2;
                return RedirectToAction("Index");
            }
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
            ViewBag.id_capacity = new SelectList(db.Capacity.OrderByDescending(c => c.capacity), "id", "capacity", product.id_capacity);
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
            ViewBag.id_capacity = new SelectList(db.Capacity.OrderByDescending(c => c.capacity), "id", "description", product.id_capacity);
            ViewBag.id_category = new SelectList(db.Category, "id", "description", product.id_category);
            ViewBag.id_color = new SelectList(db.Color, "id", "name", product.id_color);
            ViewBag.id_subcategory = new SelectList(db.Subcategory, "id", "description", product.id_subcategory);
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Product product = db.Product.Find(id);
                product.deleted_at = DateTime.Now;
                db.SaveChanges();
                TempData["DeleteMessage"] = "El producto fue eliminado";
                return RedirectToAction("Index");
            }
            catch(Exception)
            {
               return RedirectToAction("Index");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpGet]
        public JsonResult FilterProductsByName(string name)
        {
            try
            {
                var products = db.Product
                        .Where(p =>
                        (p.description.Contains(name) ||
                        p.internal_code.ToString().Contains(name)) &&
                        p.deleted_at.Equals(null))
                        .Select(p => new
                        {
                            internal_code = p.internal_code != null ? p.internal_code.Value.ToString() : null,
                            product_id = p.id.ToString(),
                            p.description,
                            brand = p.Brand.name,
                            category = p.Category.description,
                            subcategory = p.Subcategory.description,
                            color = p.Color.name,
                            p.Color.rgb_hex_code,
                            capacity = p.Capacity.description,
                        }).ToList();

                return Json(products, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult FilterProducts(string id_brand, string id_category, string id_subcategory, string id_color, string id_capacity)
        {
            try
            {
                if (id_brand == "" && id_category == "" && id_subcategory == "" && id_color == "" && id_capacity == "")
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }

                var products = db.Product
                            .Where(p =>
                            p.id_brand.ToString().Contains(id_brand) &&
                            p.id_category.ToString().Contains(id_category) &&
                            p.id_subcategory.ToString().Contains(id_subcategory) &&
                            p.id_color.ToString().Contains(id_color) &&
                            p.id_capacity.ToString().Contains(id_capacity) &&
                            p.deleted_at.Equals(null))
                            .Select(p => new
                            {
                                internal_code = p.internal_code != null ? p.internal_code.Value.ToString() : null,
                                product_id = p.id.ToString(),
                                p.description,
                                brand = p.Brand.name,
                                category = p.Category.description,
                                subcategory = p.Subcategory.description,
                                color = p.Color.name,
                                p.Color.rgb_hex_code,
                                capacity = p.Capacity.description,
                            }).ToList();

                return Json(products, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
