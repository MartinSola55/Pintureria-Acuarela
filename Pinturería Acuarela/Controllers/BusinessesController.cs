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
    [Security]
    [Admin]
    public class BusinessesController : Controller
    {
        private EFModel db = new EFModel();

        // GET: Businesses
        public ActionResult Index()
        {
            return View(db.Business.ToList());
        }

        // GET: Businesses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Business business = db.Business.Find(id);
            if (business == null)
            {
                return HttpNotFound();
            }
            int stocklessProducts = db.Product_Business.Where(p => p.id_business.Equals(id.Value) && p.stock == 0 && p.deleted_at.Equals(null) && p.Product.deleted_at.Equals(null)).Count();
            int stockAlertProducts = db.Product_Business.Where(p => p.id_business.Equals(id.Value) && p.stock < p.minimum_stock && p.deleted_at.Equals(null) && p.Product.deleted_at.Equals(null)).Count();
            int totalProducts = db.Product_Business.Where(p => p.id_business.Equals(id.Value) && p.deleted_at.Equals(null) && p.Product.deleted_at.Equals(null)).Count();
            int pendingOrders = db.Order.Where(o => o.User.Business.id.Equals(id.Value) && o.status.Equals(false) && o.deleted_at.Equals(null)).Count();
            double totalLiters = db.Product_Business.Where(p => p.id_business.Equals(id.Value) && p.deleted_at.Equals(null) && p.Product.deleted_at.Equals(null)).Sum(pb => pb.stock * pb.Product.Capacity.capacity);

            ViewBag.PendingOrders = pendingOrders;
            ViewBag.StocklessProducts = stocklessProducts;
            ViewBag.StockAlertProducts = stockAlertProducts;
            ViewBag.TotalProducts = totalProducts;
            ViewBag.TotalLiters = totalLiters;
            return View(business);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult BusinessOrders(int? id)
        {
            if (id != null)
            {   
                return RedirectToAction("Index", "Orders", new { id });
            }
            return RedirectToAction("Index");
        }
    }
}
