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
    public class BrandsController : Controller
    {
        private EFModel db = new EFModel();

        // GET: Brands
        public ActionResult Index()
        {
            try
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
                var brands = db.Brand.Where(b => b.deleted_at.Equals(null)).ToList();
                var colors = db.Color.ToList();

                Tuple<List<Brand>, List<Color>> model = new Tuple<List<Brand>, List<Color>>(brands, colors);

                return View(model);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Brands/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Brands/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name")] Brand brand)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Brand repeted = db.Brand.Where(b => b.name.Equals(brand.name)).FirstOrDefault();
                    if (repeted != null)
                    {
                        ViewBag.Message = "La marca ingresada ya existe";
                        ViewBag.Error = 1;
                        return View(brand);
                    }
                    brand.created_at = DateTime.UtcNow.AddHours(-3);
                    db.Brand.Add(brand);
                    db.SaveChanges();
                    TempData["Message"] = "La marca se creó correctamente";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "El nombre ingresado no es válido";
                    ViewBag.Error = 1;
                    return View(brand);
                }
            }
            catch (Exception)
            {
                TempData["Message"] = "Ha ocurrido un error inesperado. No se ha podido cargar la marca";
                TempData["Error"] = 2;
                return RedirectToAction("Index");
            }
        }

        // GET: Brands/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Brand brand = db.Brand.Find(id);
            if (brand == null)
            {
                ViewBag.Message = "Ha ocurrido un error. No se ha encontrado la marca";
                ViewBag.Error = 1;
                brand = new Brand();
            }
            return View(brand);
        }

        // POST: Brands/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name")] Brand brand_edited)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Brand repeted = db.Brand.Where(b => b.name.Equals(brand_edited.name) && !b.id.Equals(brand_edited.id)).FirstOrDefault();
                    if (repeted != null)
                    {
                        ViewBag.Message = "La marca ingresada ya existe";
                        ViewBag.Error = 1;
                        return View(brand_edited);
                    }
                    Brand brand = db.Brand.Find(brand_edited.id);
                    brand.name = brand_edited.name;
                    db.SaveChanges();
                    TempData["Message"] = "La marca se editó correctamente";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "El nombre ingresado no es válido";
                    ViewBag.Error = 1;
                    return View(brand_edited);
                }
            }
            catch (Exception)
            {
                TempData["Message"] = "Ha ocurrido un error inesperado. No se ha podido guardar la marca";
                TempData["Error"] = 2;
                return RedirectToAction("Index");
            }
        }

        // POST: Brands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id_brand)
        {
            try
            {
                Brand brand = db.Brand.Find(id_brand);
                if (brand != null)
                {
                    brand.deleted_at = DateTime.UtcNow.AddHours(-3);
                    db.SaveChanges();
                    TempData["Message"] = "La marca fue eliminada";
                    return RedirectToAction("Index");
                } else
                {
                    TempData["Message"] = "Ha ocurrido un error. No se ha encontrado la marca";
                    TempData["Error"] = 1;
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                TempData["Message"] = "Ha ocurrido un error inesperado. No se ha podido eliminar la marca";
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
    }
}
