using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Transactions;
using Pinturería_Acuarela;

namespace Pinturería_Acuarela.Controllers
{
    public class OrdersController : Controller
    {
        private EFModel db = new EFModel();

        // GET: Orders
        public ActionResult Index(int? id)
        {
            if (id != null)
            {
                var orders = db.Order.Where(o => o.User.Business.id.Equals(id.Value));
                return View(orders.ToList().OrderBy(o => o.User.Business.id).OrderBy(o => o.date).OrderBy(o => o.status));
            }
            var order = db.Order.Include(o => o.User);
            return View(order.ToList().OrderBy(o => o.User.Business.id).OrderBy(o => o.date).OrderBy(o => o.status));
        }

        // GET: Orders/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Order.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            ViewBag.id_brand = new SelectList(db.Brand, "id", "name");
            ViewBag.id_category = new SelectList(db.Category, "id", "description");
            ViewBag.id_subcategory = new SelectList(db.Subcategory, "id", "description");
            ViewBag.id_color = new SelectList(db.Color, "id", "name");
            ViewBag.id_capacity = new SelectList(db.Capacity, "id", "capacity");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,date,id_user,status")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Order.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.id_user = new SelectList(db.User, "id", "email", order.id_user);
            return View(order);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Order.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.id_user = new SelectList(db.User, "id", "email", order.id_user);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,date,id_user,status")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.id_user = new SelectList(db.User, "id", "email", order.id_user);
            return View(order);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Order.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Order order = db.Order.Find(id);
            var prod_orders = db.Product_Order.Where(po => po.id_order.Equals(id)).ToList();
            using (var transaccion = new TransactionScope())
            {
                foreach (Product_Order item in prod_orders)
                {
                    db.Product_Order.Remove(item);
                }
                db.Order.Remove(order);
                db.SaveChanges();
                transaccion.Complete();
            }
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
                    p.id_capacity.ToString().Contains(id_capacity))
                    .Select(p => new
                    {
                        product_id = p.id,
                        product = p.description,
                        brand = p.Brand.name,
                        category = p.Category.description,
                        subcategory = p.Subcategory.description,
                        color = p.Color.name,
                        hex_color = p.Color.rgb_hex_code,
                        p.Capacity.capacity
                    });
                return Json(products, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json (JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public int AddToCart(int? id, int? cant)
        {
            List<int[]> products = new List<int[]>();
            if (id != null && cant != null)
            {
                if (cant == 0)
                {
                    ViewBag.Error = "Debes seleecionar una cantidad mayor a 1";
                    return 0;
                }
                if (Session["Basket"] != null)
                {
                    products = Session["Basket"] as List<int[]>;
                }
                int[] aux = new int[2];
                aux[0] = id.Value;
                aux[1] = cant.Value;
                products.Add(aux);
                Session["Basket"] = products;
            }
            Session["BasketCount"] = products.Count;
            return products.Count;
        }
        public ActionResult Basket()
        {
            if (Session["Basket"] != null)
            {
                List<int[]> basket = Session["Basket"] as List<int[]>;
                List<Product_Order> products = new List<Product_Order>();
                foreach (int[] item in basket)
                {
                    int aux = item[0];
                    Product prod = db.Product.Where(p => p.id.Equals(aux)).FirstOrDefault();
                    Product_Order po = new Product_Order();
                    po.Product = prod;
                    po.quantity = item[1];
                    products.Add(po);
                }
                return View(products);
            } else
            {
                return RedirectToAction("Create");
            }
        }
        public ActionResult CreateOrder()
        {
            if (Session["Basket"] != null)
            {
                List<int[]> basket = Session["Basket"] as List<int[]>;
                List<Product_Order> products = new List<Product_Order>();
                foreach (int[] item in basket)
                {
                    int aux = item[0];
                    Product prod = db.Product.Where(p => p.id.Equals(aux)).FirstOrDefault();
                    Product_Order po = new Product_Order();
                    po.Product = prod;
                    po.quantity = item[1];
                    products.Add(po);
                }
                Order order = new Order
                {
                    date = DateTime.Now,
                    id_user = int.Parse(Session["idUsuario"].ToString()),
                    status = false
                };
                using (var transaccion = new TransactionScope())
                {
                    db.Order.Add(order);
                    db.SaveChanges();
                    foreach (Product_Order item in products)
                    {
                        item.id_order = order.id;
                        db.Product_Order.Add(item);
                    }
                    db.SaveChanges();
                    transaccion.Complete();
                }
                Session["Basket"] = null;
            }
            return RedirectToAction("Index");
        }
        public ActionResult ConfirmProduct(int? id_product, int? id_order, int? quant)
        {
            if (id_product != null && id_order != null && quant != null)
            {
                Product_Order prod_order = db.Product_Order.Where(p => p.id_product.Equals(id_product.Value) && p.id_order.Equals(id_order.Value)).First();
                if (prod_order != null)
                {
                    if (prod_order.quantity == quant)
                    {
                        prod_order.status = true;
                    } else
                    {
                        prod_order.quantity = prod_order.quantity - quant.Value;
                    }
                    db.Entry(prod_order).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("/Details/" + id_order);
                }
            }
            return View("Index");
        }
        public ActionResult ConfirmOrder(int? id)
        {
            if (id != null)
            {
                var order = db.Order.Where(o => o.id.Equals(id.Value)).First();
                order.status = true;
                foreach (Product_Order item in order.Product_Order)
                {
                    item.status = true;
                }
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
            }
            var orders = db.Order.Include(o => o.User);
            return RedirectToAction("Index");
        }
    }
}
