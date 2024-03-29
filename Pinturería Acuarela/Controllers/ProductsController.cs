﻿using System;
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
            ViewBag.id_brand = new SelectList(db.Brand.Where(b => b.deleted_at.Equals(null)).OrderBy(b => b.name), "id", "name");
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
            ViewBag.id_brand = new SelectList(db.Brand.Where(b => b.deleted_at.Equals(null)).OrderBy(b => b.name), "id", "name");
            ViewBag.id_capacity = new SelectList(db.Capacity.OrderByDescending(c => c.capacity), "id", "description");
            ViewBag.id_category = new SelectList(db.Category, "id", "description");
            ViewBag.id_color = new SelectList(db.Color, "id", "name");
            ViewBag.id_subcategory = new SelectList(db.Subcategory, "id", "description");
            return View(new Product());
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
                    if (db.Product.Where(p => p.internal_code.Equals(product.internal_code)).ToList().Count > 0)
                    {
                        ViewBag.Message = "Ya existe un producto con el mismo código interno";
                        ViewBag.Error = 1;
                        ViewBag.id_brand = new SelectList(db.Brand.Where(b => b.deleted_at.Equals(null)).OrderBy(b => b.name), "id", "name");
                        ViewBag.id_capacity = new SelectList(db.Capacity.OrderByDescending(c => c.capacity), "id", "description");
                        ViewBag.id_category = new SelectList(db.Category, "id", "description");
                        ViewBag.id_color = new SelectList(db.Color, "id", "name");
                        ViewBag.id_subcategory = new SelectList(db.Subcategory, "id", "description");
                        return View(product);
                    }
                    product.created_at= DateTime.UtcNow.AddHours(-3);
                    db.Product.Add(product);
                    db.SaveChanges();
                    TempData["Message"] = "El producto se creó correctamente";
                    return RedirectToAction("Index");
                } else
                {
                    ViewBag.Message = "Alguno de los campos ingresados no son válidos";
                    ViewBag.Error = 1;
                }

                ViewBag.id_brand = new SelectList(db.Brand.Where(b => b.deleted_at.Equals(null)).OrderBy(b => b.name), "id", "name", product.id_brand);
                ViewBag.id_capacity = new SelectList(db.Capacity.OrderByDescending(c => c.capacity), "id", "description", product.id_capacity);
                ViewBag.id_category = new SelectList(db.Category, "id", "description", product.id_category);
                ViewBag.id_color = new SelectList(db.Color, "id", "name", product.id_color);
                ViewBag.id_subcategory = new SelectList(db.Subcategory, "id", "description", product.id_subcategory);
                return View(product);
            }
            catch (Exception)
            {
                TempData["Message"] = "Ha ocurrido un error inesperado. No se ha podido agregar el producto";
                TempData["Error"] = 2;
                return RedirectToAction("Index");
            }
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Product product = db.Product.Find(id);
                if (product == null)
                {
                    ViewBag.Message = "Ha ocurrido un error. No se ha encontrado el producto";
                    ViewBag.Error = 1;
                    product = new Product();
                }

                ViewBag.id_brand = new SelectList(db.Brand.Where(b => b.deleted_at.Equals(null)).OrderBy(b => b.name), "id", "name", product.id_brand);
                ViewBag.id_capacity = new SelectList(db.Capacity.OrderByDescending(c => c.capacity), "id", "description", product.id_capacity);
                ViewBag.id_category = new SelectList(db.Category, "id", "description", product.id_category);
                ViewBag.id_color = new SelectList(db.Color, "id", "name", product.id_color);
                ViewBag.id_subcategory = new SelectList(db.Subcategory, "id", "description", product.id_subcategory);
                return View(product);
            }
            catch (Exception)
            {
                TempData["Message"] = "Ha ocurrido un error inesperado.";
                TempData["Error"] = 2;
                return RedirectToAction("Index");
            }
        }

        // POST: Products/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,description,id_brand,id_category,id_subcategory,id_capacity,id_color,internal_code")] Product product_edited)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (db.Product.Where(p => p.internal_code.Equals(product_edited.internal_code) && !p.id.Equals(product_edited.id)).ToList().Count > 0)
                    {
                        ViewBag.Message = "Ya existe un producto con el mismo código interno";
                        ViewBag.Error = 1;
                        ViewBag.id_brand = new SelectList(db.Brand.Where(b => b.deleted_at.Equals(null)).OrderBy(b => b.name), "id", "name");
                        ViewBag.id_capacity = new SelectList(db.Capacity.OrderByDescending(c => c.capacity), "id", "description");
                        ViewBag.id_category = new SelectList(db.Category, "id", "description");
                        ViewBag.id_color = new SelectList(db.Color, "id", "name");
                        ViewBag.id_subcategory = new SelectList(db.Subcategory, "id", "description");
                        return View(product_edited);
                    }

                    Product product = db.Product.Find(product_edited.id);
                    product.description = product_edited.description;
                    product.id_brand = product_edited.id_brand;
                    product.id_category = product_edited.id_category;
                    product.id_subcategory = product_edited.id_subcategory;
                    product.id_capacity = product_edited.id_capacity;
                    product.id_color = product_edited.id_color;
                    product.internal_code = product_edited.internal_code;                   
                    db.SaveChanges();
                    TempData["Message"] = "El producto se guardó correctamente";
                    return RedirectToAction("Index");
                } else
                {
                    ViewBag.Message = "Alguno de los campos ingresados no son válidos";
                    ViewBag.Error = 1;
                }
                ViewBag.id_brand = new SelectList(db.Brand.Where(b => b.deleted_at.Equals(null)).OrderBy(b => b.name), "id", "name", product_edited.id_brand);
                ViewBag.id_capacity = new SelectList(db.Capacity.OrderByDescending(c => c.capacity), "id", "description", product_edited.id_capacity);
                ViewBag.id_category = new SelectList(db.Category, "id", "description", product_edited.id_category);
                ViewBag.id_color = new SelectList(db.Color, "id", "name", product_edited.id_color);
                ViewBag.id_subcategory = new SelectList(db.Subcategory, "id", "description", product_edited.id_subcategory);
                return View(product_edited);
            }
            catch (Exception)
            {
                TempData["Message"] = "Ha ocurrido un error inesperado. No se ha podido guardar el producto";
                TempData["Error"] = 2;
                return RedirectToAction("Index");
            }
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Product product = db.Product.Find(id);
                if (product != null)
                {
                    product.deleted_at = DateTime.UtcNow.AddHours(-3);
                    db.SaveChanges();
                    TempData["Message"] = "El producto fue eliminado";
                    return RedirectToAction("Index");
                } else
                {
                    TempData["Message"] = "Ha ocurrido un error. No se ha encontrado el producto";
                    TempData["Error"] = 1;
                    return RedirectToAction("Index");
                }
            }
            catch(Exception)
            {
                TempData["Message"] = "Ha ocurrido un error inesperado. No se ha podido eliminar el producto";
                TempData["Error"] = 2;
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

        // GET: Filter products by name or internal code
        [HttpGet]
        public JsonResult FilterProductsByName(string name)
        {
            try
            {
                var products = db.Product
                        .Where(p =>
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
                        }).ToList();

                return Json(products, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Filter products
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
