using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Pinturería_Acuarela;
using Pinturería_Acuarela.Filter;

namespace Pinturería_Acuarela.Controllers
{
    [Security]
    public class Product_BusinessController : Controller
    {
        private EFModel db = new EFModel();

        // GET: Product_Business
        public ActionResult Index(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ViewBag.id_brand = new SelectList(db.Brand, "id", "name");
                ViewBag.id_capacity = new SelectList(db.Capacity, "id", "description");
                ViewBag.id_category = new SelectList(db.Category, "id", "description");
                ViewBag.id_color = new SelectList(db.Color, "id", "name");
                ViewBag.id_subcategory = new SelectList(db.Subcategory, "id", "description");
                return View(new Product_Business { Business = db.Business.Find(id.Value) });
            }
            catch (Exception)
            {
                return RedirectToAction(Session["User"].ToString() == "1" ? "AdminIndex" : "Index", "Home");
            }
        }

        // GET: Product_Business/Create
        public ActionResult Create(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ViewBag.id_brand = new SelectList(db.Brand, "id", "name");
                ViewBag.id_capacity = new SelectList(db.Capacity, "id", "description");
                ViewBag.id_category = new SelectList(db.Category, "id", "description");
                ViewBag.id_color = new SelectList(db.Color, "id", "name");
                ViewBag.id_subcategory = new SelectList(db.Subcategory, "id", "description");
                
                return View(new Product_Business { Business = db.Business.Find(id.Value) });
            }
            catch (Exception)
            {
                return RedirectToAction(Session["User"].ToString() == "1" ? "AdminIndex" : "Index", "Home");
            }
        }

        // POST: Product_Business/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_product,id_business,minimum_stock,stock")] Product_Business product_Business)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    product_Business.created_at = DateTime.Now;
                    db.Product_Business.Add(product_Business);
                    db.SaveChanges();
                }
                return RedirectToAction("Index", new { id = product_Business.id_business });
            }
            catch (Exception)
            {
                return RedirectToAction(Session["User"].ToString() == "1" ? "AdminIndex" : "Index", "Home");
            }

        }

        // POST: Product_Business/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_product,id_business,minimum_stock,stock")] Product_Business stock_updated)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Product_Business product_Business = db.Product_Business.Find(stock_updated.id_product, stock_updated.id_business);
                    product_Business.stock = stock_updated.stock;
                    product_Business.minimum_stock = stock_updated.minimum_stock;
                    db.SaveChanges();
                }
                return RedirectToAction("Index", new { id = stock_updated.id_business });
            }
            catch (Exception)
            {
                return RedirectToAction(Session["User"].ToString() == "1" ? "AdminIndex" : "Index", "Home");
            }
        }

        // POST: Product_Business/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Product_Business product_Business = db.Product_Business.Find(id);
                product_Business.deleted_at = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = product_Business.id_business });
            }
            catch (Exception)
            {
                return RedirectToAction(Session["User"].ToString() == "1" ? "AdminIndex" : "Index", "Home");
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

        // GET: Products by filter
        [HttpGet]
        public JsonResult FilterProducts(string id_brand, string id_category, string id_subcategory, string id_color, string id_capacity, int? id_business)
        {
            try
            {
                if (id_brand == "" && id_category == "" && id_subcategory == "" && id_color == "" && id_capacity == "")
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }

                if(id_business != null)
                {

                    var products_in_business = db.Product
                            .Where(p =>
                            p.Product_Business.Any(pb => pb.id_product.Equals(p.id) && pb.id_business.Equals(id_business.Value)) &&
                            p.deleted_at.Equals(null));

                    var products_not_in_business = db.Product
                        .Except(products_in_business)
                    .Where(p =>
                        p.id_brand.ToString().Contains(id_brand) &&
                        p.id_category.ToString().Contains(id_category) &&
                        p.id_subcategory.ToString().Contains(id_subcategory) &&
                        p.id_color.ToString().Contains(id_color) &&
                        p.id_capacity.ToString().Contains(id_capacity) &&
                        p.deleted_at.Equals(null));

                    var response = products_not_in_business.Select(p => new
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

                    return Json(response, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        // GET: Products by name
        [HttpGet]
        public JsonResult FilterProductsByName(string name, int? id_business)
        {
            try
            {
                if (id_business != null)
                {
                    var products_in_business = db.Product
                        .Where(p =>
                        p.Product_Business.Any(pb => pb.id_product.Equals(p.id) && pb.id_business.Equals(id_business.Value)) &&
                        p.deleted_at.Equals(null));

                    var products_not_in_business = db.Product
                        .Except(products_in_business)
                        .Where(p =>
                        (p.description.Contains(name) ||
                        p.internal_code.ToString().Contains(name)) &&
                        p.deleted_at.Equals(null));

                    var response = products_not_in_business.Select(p => new
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

                    return Json(response, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        // GET: All products in business by filter
        [HttpGet]
        public JsonResult FilterAllProducts(string id_brand, string id_category, string id_subcategory, string id_color, string id_capacity, int? id_business)
        {
            try
            {
                if (id_brand == "" && id_category == "" && id_subcategory == "" && id_color == "" && id_capacity == "")
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }

                if (id_business != null)
                {

                    var products = db.Product
                            .Where(p =>
                            p.Product_Business.Any(pb => pb.id_product.Equals(p.id) && pb.id_business.Equals(id_business.Value)) &&
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
                                stock = p.Product_Business.Where(pb => pb.id_product.Equals(p.id) && pb.id_business.Equals(id_business.Value)).FirstOrDefault().stock.ToString(),
                                minimum_stock = p.Product_Business.Where(pb => pb.id_product.Equals(p.id) && pb.id_business.Equals(id_business.Value)).FirstOrDefault().minimum_stock.ToString()
                            }).ToList();

                    return Json(products, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }

        // GET: All products in business by name
        [HttpGet]
        public JsonResult FilterAllProductsByName(string name, int? id_business)
        {
            try
            {
                if (id_business != null)
                {
                    var products = db.Product
                        .Where(p =>
                        p.Product_Business.Any(pb => pb.id_product.Equals(p.id) && pb.id_business.Equals(id_business.Value)) &&
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
                            stock = p.Product_Business.Where(pb => pb.id_product.Equals(p.id) && pb.id_business.Equals(id_business.Value)).FirstOrDefault().stock.ToString(),
                            minimum_stock = p.Product_Business.Where(pb => pb.id_product.Equals(p.id) && pb.id_business.Equals(id_business.Value)).FirstOrDefault().minimum_stock.ToString()
                        }).ToList();

                    return Json(products, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }
}
