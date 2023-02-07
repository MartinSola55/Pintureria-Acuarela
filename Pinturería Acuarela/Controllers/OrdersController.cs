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
using Pinturería_Acuarela.Filter;

namespace Pinturería_Acuarela.Controllers
{
    [Security]
    public class OrdersController : Controller
    {
        private EFModel db = new EFModel();

        // GET: Orders
        [Admin]
        public ActionResult Index(int? id)
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
            if (id != null)
            {
                var orders = db.Order.Where(o => o.User.Business.id.Equals(id.Value) && o.deleted_at.Equals(null));
                return View(orders.ToList().OrderBy(o => o.User.Business.id).OrderBy(o => o.date).OrderBy(o => o.status));
            }
            var order = db.Order.Include(o => o.User).Where(o => o.deleted_at.Equals(null));
            return View(order.ToList().OrderBy(o => o.User.Business.id).OrderBy(o => o.date).OrderBy(o => o.status));
        }

        // GET: Orders/Details/5
        [Admin]
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
            if (TempData.Count == 1)
            {
                ViewBag.Message = TempData["Message"].ToString();
            }
            else if (TempData.Count == 2)
            {
                ViewBag.Message = TempData["Message"].ToString();
                ViewBag.Error = TempData["Error"];
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

        // POST: Orders/Delete/5
        [Admin]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            try
            {
                Order order = db.Order.Find(id);
                order.deleted_at = DateTime.Now;
                
                db.SaveChanges();
                TempData["Message"] = "La orden se eliminó correctamente";
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
                TempData["Error"] = 2;
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
                        capacity = p.id_capacity != null ? p.Capacity.capacity : (double?)null,
                    });
                return Json(products.ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json (JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Add products to cart
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

        // GET: Basket
        [HttpGet]
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

        // POST: Create order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrder()
        {
            try
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
            }
            catch (Exception)
            {
                throw;
            }
            return RedirectToAction("Index");
        }

        // POST: Confirm individual products
        [Admin]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmProduct(int? id_product, int? id_order, int? quant, int? id_business)
        {
            try
            {
                if (id_product != null && id_order != null && quant != null)
                {
                    Product_Order prod_order = db.Product_Order.Where(p => p.id_product.Equals(id_product.Value) && p.id_order.Equals(id_order.Value)).First();
                    if (prod_order != null)
                    {
                        if (prod_order.quantity_send + quant.Value > prod_order.quantity)
                        {
                            TempData["Error"] = 1;
                            throw new Exception("La cantidad seleccionada es mayor a la disponible");
                        } else if (quant.Value < 0 )
                        {
                            TempData["Error"] = 1;
                            throw new Exception("Debes seleccionar una cantidad mayor o igual a 0");
                        }
                        if (prod_order.quantity == quant.Value)
                        {
                            prod_order.quantity_send = quant.Value;
                        } else
                        {
                            prod_order.quantity_send += quant.Value;
                        }

                        if (id_business == null && quant.Value > 0)
                        {
                            id_business = db.Business.Where(b => b.adress.Equals("San Carlos Centro")).Select(b => b.id).FirstOrDefault();
                        }

                        // id_business == null cuando quant = 0 (mandado desde el front)
                        if (id_business != null)
                        {
                            Product_Business prod_b_sender = db.Product_Business.Where(p => p.id_product == id_product.Value && p.id_business == id_business).FirstOrDefault();
                            if (prod_b_sender != null)
                            {
                                if (prod_b_sender.stock >= quant.Value)
                                {
                                    prod_b_sender.stock -= quant.Value;
                                }
                                else
                                {
                                    TempData["Error"] = 1;
                                    throw new Exception("La sucursal no cuenta con suficiente stock");
                                }
                            } else
                            {
                                throw new Exception("La sucursal no tiene registrado el producto");
                            }

                            Product_Business prod_b_receiver = db.Product_Business.Where(p => p.id_product == id_product.Value && p.id_business == prod_order.Order.User.Business.id).FirstOrDefault();
                            prod_b_receiver.stock += quant.Value;
                        }

                        prod_order.status = true;
                        prod_order.id_business_sender = id_business;

                        db.SaveChanges();
                        TempData["Message"] = "Producto confirmado correctamente";
                        return RedirectToAction("/Details", new { id = id_order});
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
                if (TempData["Error"] != null)
                {
                    if (TempData["Error"].ToString() != "1")
                    {
                        TempData["Error"] = 2;
                    }
                } else
                {
                    TempData["Error"] = 2;
                }
                return RedirectToAction("/Details", new { id = id_order });
            }
            return RedirectToAction("Index");
        }

        // POST: Confirm individual product (default store)
        [Admin]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmProductDefault(int? id_product, int? id_order, int? quant)
        {
            try
            {
                int id_business = db.Business.Where(b  => b.adress.Equals("San Carlos Centro")).Select(b => b.id).FirstOrDefault();
                return RedirectToAction("/ConfirmProduct", new { id_product, id_order, quant, id_business });
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
                TempData["Error"] = 2;
                return RedirectToAction("/Details", new { id = id_order });
            }
        }

        // POST: Unconfirm individual products
        [Admin]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UnconfirmProduct(int? id_product, int? id_order)
        {
            try
            {
                if (id_product != null && id_order != null)
                {
                    Product_Order prod_order = db.Product_Order.Where(p => p.id_product.Equals(id_product.Value) && p.id_order.Equals(id_order.Value)).First();
                    if (prod_order != null)
                    {
                        if (prod_order.id_business_sender != null)
                        {
                            Product_Business prod_b_sender = db.Product_Business.Where(p => p.id_product == id_product.Value && p.id_business == prod_order.id_business_sender).First();
                            prod_b_sender.stock += prod_order.quantity_send;

                            Product_Business prod_b_receiver = db.Product_Business.Where(p => p.id_product == id_product.Value && p.id_business == prod_order.Order.User.Business.id).First();
                            prod_b_receiver.stock -= prod_order.quantity_send;
                        } 

                        prod_order.quantity_send = 0;
                        prod_order.id_business_sender = null;
                        prod_order.status = false;
                        prod_order.Order.status = false;

                        db.SaveChanges();
                        TempData["Message"] = "Se ha cancelado la confirmación";
                        return RedirectToAction("/Details", new { id = id_order });
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
                TempData["Error"] = 2;
                return RedirectToAction("/Details", new { id = id_order });
            }
            return RedirectToAction("Index");
        }

        // POST: Confirm whole order
        [Admin]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmOrder(int? id)
        {
            try
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
                    TempData["Message"] = "Se ha confirmado la orden";
                    return RedirectToAction("/Details", new { id });
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
                TempData["Error"] = 2;
                return RedirectToAction("/Details", new { id });
            }
            return RedirectToAction("Index");
        }

        // POST: Unconfirm whole order
        [Admin]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UnconfirmOrder(int? id)
        {
            try
            {
                if (id != null)
                {
                    var order = db.Order.Where(o => o.id.Equals(id.Value)).First();
                    order.status = false;
                    foreach (Product_Order item in order.Product_Order)
                    {
                        if (item.id_business_sender != null)
                        {
                            Product_Business prod_b_sender = db.Product_Business.Where(p => p.id_product == item.id_product && p.id_business == item.id_business_sender).First();
                            prod_b_sender.stock += item.quantity_send;

                            Product_Business prod_b_receiver = db.Product_Business.Where(p => p.id_product == item.id_product && p.id_business == item.Order.User.Business.id).First();
                            prod_b_receiver.stock -= item.quantity_send;
                        }

                        item.quantity_send = 0;
                        item.id_business_sender = null;
                        item.status = false;
                    }
                    db.Entry(order).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["Message"] = "Se ha cancelado la confirmación";
                    return RedirectToAction("/Details", new { id });
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
                TempData["Error"] = 2;
                return RedirectToAction("/Details", new { id });
            }
            return RedirectToAction("Index");
        }

        // GET: Stock in business
        [Admin]
        [HttpGet]
        public JsonResult CheckStock(int id_product, int id_business, int quant)
        {
            try
            {
                var query = db.Product_Business
                    .Where(p => p.id_product.Equals(id_product) && p.id_business != id_business && p.stock >= quant)
                    .Select( p => new
                    {
                        p.id_business,
                        p.Business.adress,
                        p.stock
                    })
                    .ToList();
                return Json(query,JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }
    }
}
