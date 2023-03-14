using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Pinturería_Acuarela;
using Pinturería_Acuarela.Filter;

namespace Pinturería_Acuarela.Controllers
{
    [Admin]
    public class ColorsController : Controller
    {
        private EFModel db = new EFModel();

        // GET: Colors/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Colors/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,rgb_hex_code")] Color color)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Color repeted = db.Color.Where(c => c.name.Equals(color.name) || c.rgb_hex_code.Equals(color.rgb_hex_code)).FirstOrDefault();
                    if (repeted != null)
                    {
                        ViewBag.Message = "El color o el nombre ingresado ya existe";
                        ViewBag.Error = 1;
                        return View(color);
                    }
                    db.Color.Add(color);
                    db.SaveChanges();
                    TempData["Message"] = "El color se creó correctamente";
                    return RedirectToAction("Index", "Brands");
                }
                else
                {
                    ViewBag.Message = "El nombre ingresado o el color seleccionado no son válidos";
                    ViewBag.Error = 1;
                    return View(color);
                }
            }
            catch (Exception)
            {
                TempData["Message"] = "Ha ocurrido un error inesperado. No se ha podido cargar el color";
                TempData["Error"] = 2;
                return RedirectToAction("Index", "Brands");
            }
        }

        // GET: Colors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Color color = db.Color.Find(id);
            if (color == null)
            {
                ViewBag.Message = "Ha ocurrido un error. No se ha encontrado el color";
                ViewBag.Error = 1;
                color = new Color();
            }
            return View(color);
        }

        // POST: Colors/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,rgb_hex_code")] Color color_edited)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Color repeted = db.Color.Where(c => (c.name.Equals(color_edited.name) || c.rgb_hex_code.Equals(color_edited.rgb_hex_code)) && !c.id.Equals(color_edited.id)).FirstOrDefault();
                    if (repeted != null)
                    {
                        ViewBag.Message = "El color o el nombre ingresado ya existe";
                        ViewBag.Error = 1;
                        return View(color_edited);
                    }
                    Color color = db.Color.Find(color_edited.id);
                    color.name = color_edited.name;
                    color.rgb_hex_code = color_edited.rgb_hex_code;
                    db.SaveChanges();
                    TempData["Message"] = "El color se editó correctamente";
                    return RedirectToAction("Index", "Brands");
                }
                else
                {
                    ViewBag.Message = "El nombre ingresado no es válido";
                    ViewBag.Error = 1;
                    return View(color_edited);
                }
            }
            catch (Exception)
            {
                TempData["Message"] = "Ha ocurrido un error inesperado. No se ha podido guardar el color";
                TempData["Error"] = 2;
                return RedirectToAction("Index", "Brands");
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
