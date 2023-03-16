using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Pinturería_Acuarela.Filter;


namespace Pinturería_Acuarela.Controllers
{
    [Security]
    public class HomeController : Controller
    {
        private EFModel db = new EFModel();

        // GET: Home
        [Admin]
        public ActionResult AdminIndex()
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

                int stocklessProducts = db.Product_Business.Where(p => p.stock == 0 && p.deleted_at.Equals(null) && p.Product.deleted_at.Equals(null)).Count();
                int stockAlertProducts = db.Product_Business.Where(p => p.stock < p.minimum_stock && p.deleted_at.Equals(null) && p.Product.deleted_at.Equals(null)).Count();
                int pendingOrders = db.Order.Where(o => o.status.Equals(false) && o.deleted_at.Equals(null)).Count();

                ViewBag.StocklessProducts = stocklessProducts;
                ViewBag.StockAlertProducts = stockAlertProducts;
                ViewBag.PendingOrders = pendingOrders;
            }
            catch (Exception)
            {
                throw;
            }
            return View();
        }

        [Employee]
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

                User user = Session["User"] as User;
                int stocklessProducts = db.Product_Business.Where(p => p.id_business.Equals(user.id_business.Value) && p.stock == 0 && p.deleted_at.Equals(null) && p.Product.deleted_at.Equals(null)).Count();
                int stockAlertProducts = db.Product_Business.Where(p => p.id_business.Equals(user.id_business.Value) && p.stock < p.minimum_stock && p.deleted_at.Equals(null) && p.Product.deleted_at.Equals(null)).Count();
                int pendingOrders = db.Order.Where(o => o.User.id.Equals(user.id) && o.status.Equals(false) && o.deleted_at.Equals(null)).Count();

                ViewBag.StocklessProducts = stocklessProducts;
                ViewBag.StockAlertProducts = stockAlertProducts;
                ViewBag.PendingOrders = pendingOrders;
            }
            catch (Exception)
            {
                throw;
            }
            return View();
        }
    }
}