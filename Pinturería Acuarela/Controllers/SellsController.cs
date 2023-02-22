using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using Pinturería_Acuarela;

namespace Pinturería_Acuarela.Controllers
{
    public class SellsController : Controller
    {
        private EFModel db = new EFModel();

        // GET: Sells
        public ActionResult Index()
        {
            var sell = db.Sell.Include(s => s.User);
            return View(sell.ToList());
        }

        // GET: Sells/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sell sell = db.Sell.Find(id);
            if (sell == null)
            {
                return HttpNotFound();
            }
            return View(sell);
        }

        // GET: Sells/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.id_user = new SelectList(db.User, "id", "email");
                ViewBag.id_brand = new SelectList(db.Brand.OrderBy(b => b.name), "id", "name");
                ViewBag.id_category = new SelectList(db.Category.OrderBy(c => c.description), "id", "description");
                ViewBag.id_subcategory = new SelectList(db.Subcategory.OrderBy(s => s.description), "id", "description");
                ViewBag.id_color = new SelectList(db.Color.OrderBy(c => c.name), "id", "name");
                ViewBag.id_capacity = new SelectList(db.Capacity.OrderByDescending(c => c.capacity), "id", "description");
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        // POST: Sells/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,date,id_user")] Sell sell)
        {
            if (ModelState.IsValid)
            {
                db.Sell.Add(sell);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_user = new SelectList(db.User, "id", "email", sell.id_user);
            return View(sell);
        }*/

        // GET: Sells/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sell sell = db.Sell.Find(id);
            if (sell == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_user = new SelectList(db.User, "id", "email", sell.id_user);
            return View(sell);
        }

        // POST: Sells/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,date,id_user")] Sell sell)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sell).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_user = new SelectList(db.User, "id", "email", sell.id_user);
            return View(sell);
        }

        // GET: Sells/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sell sell = db.Sell.Find(id);
            if (sell == null)
            {
                return HttpNotFound();
            }
            return View(sell);
        }

        // POST: Sells/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Sell sell = db.Sell.Find(id);
            db.Sell.Remove(sell);
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

        // GET: Products by filter
        [HttpGet]
        public JsonResult FilterProducts(string id_brand, string id_category, string id_subcategory, string id_color, string id_capacity)
        {
            try
            {
                if (id_brand == "" && id_category == "" && id_subcategory == "" && id_color == "" && id_capacity == "")
                {
                    return Json(null, JsonRequestBehavior.AllowGet);
                }

                User user = Session["User"] as User;
                int id_business = user.Business.id;

                var products = db.Product
                    .Where(p =>
                    p.Product_Business.Any(pb => pb.id_product.Equals(p.id) && pb.id_business.Equals(id_business)) &&
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
                        stock = p.Product_Business.Where(pb => pb.id_product.Equals(p.id) && pb.id_business.Equals(id_business)).FirstOrDefault().stock.ToString(),
                        minimum_stock = p.Product_Business.Where(pb => pb.id_product.Equals(p.id) && pb.id_business.Equals(id_business)).FirstOrDefault().minimum_stock.ToString()
                    }).ToList();
                return Json(products.ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Products by name
        [HttpGet]
        public JsonResult FilterProductsByName(string name)
        {
            try
            {
                User user = Session["User"] as User;
                int id_business = user.Business.id;

                var products = db.Product
                        .Where(p =>
                        p.Product_Business.Any(pb => pb.id_product.Equals(p.id) && pb.id_business.Equals(id_business)) &&
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
                            stock = p.Product_Business.Where(pb => pb.id_product.Equals(p.id) && pb.id_business.Equals(id_business)).FirstOrDefault().stock.ToString(),
                            minimum_stock = p.Product_Business.Where(pb => pb.id_product.Equals(p.id) && pb.id_business.Equals(id_business)).FirstOrDefault().minimum_stock.ToString()
                        }).ToList();

                return Json(products, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Add products to cart
        [HttpGet]
        public int AddToSale(int? id_prod, int? quant)
        {
            try
            {
                List<Product_Sell> sale = new List<Product_Sell>();
                if (id_prod != null && quant != null)
                {
                    if (Session["Sale"] != null)
                    {
                        sale = Session["Sale"] as List<Product_Sell>;
                    }
                    if (quant > 0)
                    {
                        //ViewBag.Error = "Debes seleecionar una cantidad mayor a 1";

                        Product_Sell prod = new Product_Sell();
                        prod.Product = db.Product
                            .Where(p => p.id == id_prod)
                            .Include(p => p.Brand)
                            .Include(p => p.Category)
                            .Include(p => p.Subcategory)
                            .Include(p => p.Capacity)
                            .FirstOrDefault();
                        prod.id_product = db.Product.Where(p => p.id == id_prod).First().id;
                        if (sale.Count == 0)
                        {
                            prod.quantity = quant.Value;
                            sale.Add(prod);
                        }
                        else
                        {
                            int count = sale.Count;
                            for (int index = 0; index < count; index++)
                            {
                                if (sale[index].Product.id == prod.Product.id)
                                {
                                    sale[index].quantity += quant.Value;
                                    break;
                                }
                                else if (index == sale.Count - 1)
                                {
                                    prod.quantity = quant.Value;
                                    sale.Add(prod);
                                }
                            }
                        }
                        Session["Sale"] = sale;
                    }
                }
                return sale.Count;
            }
            catch (Exception)
            {
                Session["Sale"] = null;
                return 0;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveProductSell(int? id_prod)
        {
            try
            {
                if (id_prod != null && Session["Sale"] != null)
                {
                    List<Product_Sell> sale = Session["Sale"] as List<Product_Sell>;
                    Product_Sell prod_s = sale.Where(p => p.id_product == id_prod.Value).FirstOrDefault();
                    sale.Remove(prod_s);
                    if (sale.Count > 0)
                    {
                        Session["Sale"] = sale;
                    }
                    else
                    {
                        Session["Sale"] = null;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return RedirectToAction("Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmSale()
        {
            try
            {
                if (Session["Sale"] != null)
                {
                    using (var transaccion = new TransactionScope())
                    {
                        Sell sell = new Sell();
                        sell.date = DateTime.UtcNow.AddHours(-3);
                        User user = Session["User"] as User;
                        sell.id_user = user.id;
                        db.Sell.Add(sell);
                        db.SaveChanges();
                        List<Product_Sell> obj = Session["Sale"] as List<Product_Sell>;
                        foreach(Product_Sell item in obj)
                        {
                            Product_Sell aux = new Product_Sell();
                            aux.quantity = item.quantity;
                            aux.id_product = item.id_product;
                            aux.id_sell = sell.id;
                            db.Product_Sell.Add(aux);
                        }
                        db.SaveChanges();
                        Session["Sale"] = null;
                        transaccion.Complete();
                    }
                    TempData["Confirm"] = "Venta realizada con exito";
                    return RedirectToAction("Create");
                }
                else
                {
                    TempData["Confirm"] = 0;
                    return RedirectToAction("Create");
                }
            }
            catch (Exception e)
            {
                TempData["Confirm"] = 0;
                return HttpNotFound();
            }
        }
            
    }
}
