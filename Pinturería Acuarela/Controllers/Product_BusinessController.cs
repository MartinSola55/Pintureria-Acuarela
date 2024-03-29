﻿using System;
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
                User user = Session["User"] as User;
                if (id == null)
                {
                    id = user.Business.id;
                }

                id = user.Rol.id != 1 ? user.Business.id : id;

                if (TempData.Count == 1)
                {
                    ViewBag.Message = TempData["Message"].ToString();
                }
                else if (TempData.Count == 2)
                {
                    ViewBag.Message = TempData["Message"].ToString();
                    ViewBag.Error = TempData["Error"];
                }

                ViewBag.id_brand = new SelectList(db.Brand.Where(b => b.deleted_at.Equals(null)).OrderBy(b => b.name), "id", "name");
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
                User user = Session["User"] as User;
                if (id == null)
                {
                    id = user.Business.id;
                }

                id = user.Rol.id != 1 ? user.Business.id : id;

                if (TempData.Count == 1)
                {
                    ViewBag.Message = TempData["Message"].ToString();
                }
                else if (TempData.Count == 2)
                {
                    ViewBag.Message = TempData["Message"].ToString();
                    ViewBag.Error = TempData["Error"];
                }

                ViewBag.id_brand = new SelectList(db.Brand.Where(b => b.deleted_at.Equals(null)).OrderBy(b => b.name), "id", "name");
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
                    product_Business.created_at = DateTime.UtcNow.AddHours(-3);
                    db.Product_Business.Add(product_Business);
                    db.SaveChanges();
                    TempData["Message"] = "Producto añadido correctamente";
                }
                return RedirectToAction("Index", new { id = product_Business.id_business });
            }
            catch (Exception)
            {
                TempData["Message"] = "Ha ocurrido un error inesperado. No se ha podido añadir el producto";
                TempData["Error"] = 2;
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
                    TempData["Message"] = "Stock actualizado correctamente";
                }
                return RedirectToAction("Index", new { id = stock_updated.id_business });
            }
            catch (Exception)
            {
                TempData["Message"] = "Ha ocurrido un error inesperado. No se ha podido actualizar el stock";
                TempData["Error"] = 2;
                return RedirectToAction(Session["User"].ToString() == "1" ? "AdminIndex" : "Index", "Home");
            }
        }

        // POST: Product_Business/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id_product_delete, int id_business_delete)
        {
            try
            {
                Product_Business product_Business = db.Product_Business.Where(pb => pb.id_product.Equals(id_product_delete) && pb.id_business.Equals(id_business_delete)).FirstOrDefault();
                if (product_Business != null)
                {
                    product_Business.deleted_at = DateTime.UtcNow.AddHours(-3);
                    db.SaveChanges();
                    TempData["Message"] = "Producto desasociado correctamente";
                    return RedirectToAction("Index", new { id = product_Business.id_business });
                }
                TempData["Error"] = 1;
                TempData["Message"] = "Ha ocurrido un error. No se ha podido encontrar el producto";
                return RedirectToAction("Index", new { id = id_business_delete });

            }
            catch (Exception)
            {
                TempData["Message"] = "Ha ocurrido un error inesperado. No se ha podido desasociar el producto";
                TempData["Error"] = 2;
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

                User user = Session["User"] as User;
                if (id_business == null)
                {
                    id_business = user.Business.id;
                }

                id_business = user.Rol.id != 1 ? user.Business.id : id_business;

                var products_in_business = db.Product
                        .Where(p =>
                        p.Product_Business.Any(pb => pb.id_product.Equals(p.id) && pb.id_business.Equals(id_business.Value)) &&
                        p.Brand.deleted_at.Equals(null) &&
                        p.deleted_at.Equals(null));

                var products_not_in_business = db.Product
                    .Except(products_in_business)
                .Where(p =>
                    p.id_brand.ToString().Contains(id_brand) &&
                    p.id_category.ToString().Contains(id_category) &&
                    p.id_subcategory.ToString().Contains(id_subcategory) &&
                    p.id_color.ToString().Contains(id_color) &&
                    p.id_capacity.ToString().Contains(id_capacity) &&
                    p.Brand.deleted_at.Equals(null) &&
                    p.deleted_at.Equals(null));

                var response = products_not_in_business.Select(p => new
                {
                    p.internal_code,
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
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Products by name
        [HttpGet]
        public JsonResult FilterProductsByName(string name, int? id_business)
        {
            try
            {
                User user = Session["User"] as User;
                if (id_business == null)
                {
                    id_business = user.Business.id;
                }

                id_business = user.Rol.id != 1 ? user.Business.id : id_business;

                var products_in_business = db.Product
                        .Where(p =>
                        p.Product_Business.Any(pb => pb.id_product.Equals(p.id) && pb.id_business.Equals(id_business.Value)) &&
                        p.Brand.deleted_at.Equals(null) &&
                        p.deleted_at.Equals(null));

                    var products_not_in_business = db.Product
                        .Except(products_in_business)
                        .Where(p =>
                        (p.description.Contains(name) ||
                        p.internal_code.ToString().Contains(name)) &&
                        p.Brand.deleted_at.Equals(null) &&
                        p.deleted_at.Equals(null));

                    var response = products_not_in_business.Select(p => new
                    {
                        p.internal_code,
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
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
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

                User user = Session["User"] as User;
                if (id_business == null)
                {
                    id_business = user.Business.id;
                }

                id_business = user.Rol.id != 1 ? user.Business.id : id_business;

                var products = db.Product
                            .Where(p =>
                            p.Product_Business.Any(pb => pb.id_product.Equals(p.id) && pb.id_business.Equals(id_business.Value) && pb.deleted_at.Equals(null)) &&
                            p.id_brand.ToString().Contains(id_brand) &&
                            p.id_category.ToString().Contains(id_category) &&
                            p.id_subcategory.ToString().Contains(id_subcategory) &&
                            p.id_color.ToString().Contains(id_color) &&
                            p.id_capacity.ToString().Contains(id_capacity) &&
                            p.Brand.deleted_at.Equals(null) &&
                            p.deleted_at.Equals(null))
                            .Select(p => new
                            {
                                p.internal_code,
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
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: All products in business by name
        [HttpGet]
        public JsonResult FilterAllProductsByName(string name, int? id_business)
        {
            try
            {
                User user = Session["User"] as User;
                if (id_business == null)
                {
                    id_business = user.Business.id;
                }

                id_business = user.Rol.id != 1 ? user.Business.id : id_business;

                var products = db.Product
                        .Where(p =>
                        p.Product_Business.Any(pb => pb.id_product.Equals(p.id) && pb.id_business.Equals(id_business.Value) && pb.deleted_at.Equals(null)) &&
                        (p.description.Contains(name) ||
                        p.internal_code.ToString().Contains(name)) &&
                        p.Brand.deleted_at.Equals(null) &&
                        p.deleted_at.Equals(null))
                        .Select(p => new
                        {
                            p.internal_code,
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
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Stockless products
        [HttpGet]
        public ActionResult StocklessProducts(int? id)
        {
            try
            {
                User user = Session["User"] as User;
                if (id == null)
                {
                    id = user.Business.id;
                }

                id = user.Rol.id != 1 ? user.Business.id : id;

                ViewBag.id_brand = new SelectList(db.Brand.Where(b => b.deleted_at.Equals(null)).OrderBy(b => b.name), "id", "name");
                ViewBag.id_capacity = new SelectList(db.Capacity, "id", "description");
                ViewBag.id_category = new SelectList(db.Category, "id", "description");
                ViewBag.id_color = new SelectList(db.Color, "id", "name");
                ViewBag.id_subcategory = new SelectList(db.Subcategory, "id", "description");

                List<Product_Business> products = db.Product_Business.Where(pb => pb.id_business.Equals(id.Value) && pb.stock == 0 && pb.deleted_at.Equals(null)).ToList();
                return View(products);
            }
            catch (Exception)
            {
                return RedirectToAction(Session["User"].ToString() == "1" ? "AdminIndex" : "Index", "Home");
            }
        }

        // GET: Stock alert products
        [HttpGet]
        public ActionResult StockAlertProducts(int? id)
        {
            try
            {
                User user = Session["User"] as User;
                if (id == null)
                {
                    id = user.Business.id;
                }

                id = user.Rol.id != 1 ? user.Business.id : id;

                ViewBag.id_brand = new SelectList(db.Brand.Where(b => b.deleted_at.Equals(null)).OrderBy(b => b.name), "id", "name");
                ViewBag.id_capacity = new SelectList(db.Capacity, "id", "description");
                ViewBag.id_category = new SelectList(db.Category, "id", "description");
                ViewBag.id_color = new SelectList(db.Color, "id", "name");
                ViewBag.id_subcategory = new SelectList(db.Subcategory, "id", "description");

                List<Product_Business> products = db.Product_Business.Where(pb => pb.id_business.Equals(id.Value) && pb.stock < pb.minimum_stock && pb.deleted_at.Equals(null)).ToList();
                return View(products);
            }
            catch (Exception)
            {
                return RedirectToAction(Session["User"].ToString() == "1" ? "AdminIndex" : "Index", "Home");
            }
        }

        // GET: Stockless products by filter
        [HttpGet]
        public JsonResult FilterStocklessProducts(string id_brand, string id_category, string id_subcategory, string id_color, string id_capacity, int? id_business)
        {
            try
            {
                if (id_brand == "" && id_category == "" && id_subcategory == "" && id_color == "" && id_capacity == "")
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }

                User user = Session["User"] as User;
                if (id_business == null)
                {
                    id_business = user.Business.id;
                }

                id_business = user.Rol.id != 1 ? user.Business.id : id_business;

                var products = db.Product_Business
                        .Where(pb =>
                        pb.id_business.Equals(id_business.Value) && 
                        pb.stock.Equals(0) &&
                        pb.Product.Brand.deleted_at.Equals(null) &&
                        pb.Product.deleted_at.Equals(null) &&
                        pb.deleted_at.Equals(null));

                if (id_brand != "")
                {
                    products = products.Where(pb => pb.Product.id_brand.ToString().Contains(id_brand));
                }
                if (id_category != "")
                {
                    products = products.Where(pb => pb.Product.id_category.ToString().Contains(id_category));
                }
                if (id_subcategory != "")
                {
                    products = products.Where(pb => pb.Product.id_subcategory.ToString().Contains(id_subcategory));
                }
                if (id_color != "")
                {
                    products = products.Where(pb => pb.Product.id_color.ToString().Contains(id_color));
                }
                if (id_capacity != "")
                {
                    products = products.Where(pb => pb.Product.id_capacity.ToString().Contains(id_capacity));
                }

                var response = products
                        .Select(p => new
                        {
                            p.Product.internal_code,
                            product_id = p.Product.id.ToString(),
                            p.Product.description,
                            brand = p.Product.Brand.name,
                            category = p.Product.Category.description,
                            subcategory = p.Product.Subcategory.description,
                            color = p.Product.Color.name,
                            p.Product.Color.rgb_hex_code,
                            capacity = p.Product.Capacity.description,
                        }).ToList();

                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Stockless products by filter
        [HttpGet]
        public JsonResult FilterStocklessProductsByName(string name, int? id_business)
        {
            try
            {
                User user = Session["User"] as User;
                if (id_business == null)
                {
                    id_business = user.Business.id;
                }

                id_business = user.Rol.id != 1 ? user.Business.id : id_business;

                var products = db.Product_Business
                        .Where(pb =>
                        pb.id_business.Equals(id_business.Value) &&
                        (pb.Product.description.Contains(name) ||
                        pb.Product.internal_code.ToString().Contains(name)) &&
                        pb.stock.Equals(0) &&
                        pb.Product.deleted_at.Equals(null) &&
                        pb.Product.Brand.deleted_at.Equals(null) &&
                        pb.deleted_at.Equals(null))
                        .Select(p => new
                        {
                            p.Product.internal_code,
                            product_id = p.Product.id.ToString(),
                            p.Product.description,
                            brand = p.Product.Brand.name,
                            category = p.Product.Category.description,
                            subcategory = p.Product.Subcategory.description,
                            color = p.Product.Color.name,
                            p.Product.Color.rgb_hex_code,
                            capacity = p.Product.Capacity.description,
                        }).ToList();

                return Json(products, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Stockless products by filter
        [HttpGet]
        public JsonResult FilterStockAlertProducts(string id_brand, string id_category, string id_subcategory, string id_color, string id_capacity, int? id_business)
        {
            try
            {
                if (id_brand == "" && id_category == "" && id_subcategory == "" && id_color == "" && id_capacity == "")
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }

                User user = Session["User"] as User;
                if (id_business == null)
                {
                    id_business = user.Business.id;
                }

                id_business = user.Rol.id != 1 ? user.Business.id : id_business;

                var products = db.Product_Business
                        .Where(pb =>
                        pb.id_business.Equals(id_business.Value) && 
                        pb.stock < pb.minimum_stock && 
                        pb.Product.deleted_at.Equals(null) &&
                        pb.Product.Brand.deleted_at.Equals(null) &&
                        pb.deleted_at.Equals(null));

                if (id_brand != "")
                {
                    products = products.Where(pb => pb.Product.id_brand.ToString().Contains(id_brand));
                }
                if (id_category != "")
                {
                    products = products.Where(pb => pb.Product.id_category.ToString().Contains(id_category));
                }
                if (id_subcategory != "")
                {
                    products = products.Where(pb => pb.Product.id_subcategory.ToString().Contains(id_subcategory));
                }
                if (id_color != "")
                {
                    products = products.Where(pb => pb.Product.id_color.ToString().Contains(id_color));
                }
                if (id_capacity != "")
                {
                    products = products.Where(pb => pb.Product.id_capacity.ToString().Contains(id_capacity));
                }

                var response = products
                        .Select(p => new
                        {
                            p.Product.internal_code,
                            product_id = p.Product.id.ToString(),
                            p.Product.description,
                            brand = p.Product.Brand.name,
                            category = p.Product.Category.description,
                            subcategory = p.Product.Subcategory.description,
                            color = p.Product.Color.name,
                            p.Product.Color.rgb_hex_code,
                            capacity = p.Product.Capacity.description,
                            p.stock,
                            p.minimum_stock,
                        }).ToList();

                return Json(response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Stockless products by filter
        [HttpGet]
        public JsonResult FilterStockAlertProductsByName(string name, int? id_business)
        {
            try
            {
                User user = Session["User"] as User;
                if (id_business == null)
                {
                    id_business = user.Business.id;
                }

                id_business = user.Rol.id != 1 ? user.Business.id : id_business;

                var products = db.Product_Business
                        .Where(pb =>
                        pb.id_business.Equals(id_business.Value) &&
                        (pb.Product.description.Contains(name) ||
                        pb.Product.internal_code.ToString().Contains(name)) &&
                        pb.stock < pb.minimum_stock &&
                        pb.Product.deleted_at.Equals(null) &&
                        pb.Product.Brand.deleted_at.Equals(null) &&
                        pb.deleted_at.Equals(null))
                        .Select(p => new
                        {
                            p.Product.internal_code,
                            product_id = p.Product.id.ToString(),
                            p.Product.description,
                            brand = p.Product.Brand.name,
                            category = p.Product.Category.description,
                            subcategory = p.Product.Subcategory.description,
                            color = p.Product.Color.name,
                            p.Product.Color.rgb_hex_code,
                            capacity = p.Product.Capacity.description,
                            p.stock,
                            p.minimum_stock,
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
