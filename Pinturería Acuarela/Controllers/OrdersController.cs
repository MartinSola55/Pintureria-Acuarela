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
        public ActionResult Index(int? id)
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
                id = user.Rol.id != 1 ? user.id_business : id;
                if (id != null)
                {
                    var orders = db.Order.Where(o => o.User.Business.id.Equals(id.Value) && o.deleted_at.Equals(null));
                    return View(orders.ToList().OrderBy(o => o.User.Business.id).OrderBy(o => o.date).OrderBy(o => o.status));
                }
                var order = db.Order.Include(o => o.User).Where(o => o.deleted_at.Equals(null));
                return View(order.ToList().OrderBy(o => o.User.Business.id).ThenBy(o => o.date).ThenBy(o => o.status));
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        // GET: Orders/Details/5
        public ActionResult Details(long? id)
        {
            try
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
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.id_brand = new SelectList(db.Brand.Where(b => b.deleted_at.Equals(null)).OrderBy(b => b.name), "id", "name");
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

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,date,id_user,status")] Order order)
        {
            try
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
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]    
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            try
            {
                Order order = db.Order.Find(id);
                order.deleted_at = DateTime.UtcNow.AddHours(-3);
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
                    p.Brand.deleted_at.Equals(null) &&
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
                return Json (null, JsonRequestBehavior.AllowGet);
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
                        p.Brand.deleted_at.Equals(null) &&
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
        public int AddToCart(int? id_prod, int? quant)
        {
            try
            {
                List<Product_Order> basket = new List<Product_Order>();
                if (id_prod != null && quant != null)
                {
                    if (Session["Basket"] != null)
                    {
                        basket = Session["Basket"] as List<Product_Order>;
                    }
                    if (quant.Value > 0)
                    {
                        //ViewBag.Error = "Debes seleecionar una cantidad mayor a 1";

                        Product_Order prod = new Product_Order();
                        prod.Product = db.Product
                            .Where(p => p.id == id_prod.Value)
                            .Include(p => p.Brand)
                            .Include(p => p.Category)
                            .Include(p => p.Subcategory)
                            .Include(p => p.Capacity)
                            .FirstOrDefault();

                        if (basket.Count == 0)
                        {
                            prod.quantity = quant.Value;
                            basket.Add(prod);
                        } else
                        {
                            int count = basket.Count;
                            for (int index = 0; index < count; index++)
                            {
                                if (basket[index].Product.id == prod.Product.id)
                                {
                                    basket[index].quantity += quant.Value;
                                    break;
                                } else if (index == basket.Count - 1)
                                {
                                    prod.quantity = quant.Value;
                                    basket.Add(prod);
                                }
                            }
                        }
                        Session["Basket"] = basket;
                    }
                }
                return basket.Count;
            }
            catch (Exception)
            {
                Session["Basket"] = null;
                return 0;
            }
        }

        // POST: Remove product from basket
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveProductBasket(int? id_prod)
        {
            try
            {
                if (id_prod != null && Session["Basket"] != null)
                {
                    List<Product_Order> basket = Session["Basket"] as List<Product_Order>;
                    Product_Order prod_o = basket.Where(p => p.Product.id == id_prod.Value).FirstOrDefault();
                    basket.Remove(prod_o);
                    if (basket.Count > 0)
                    {
                        Session["Basket"] = basket;
                    } else
                    {
                        Session["Basket"] = null;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return RedirectToAction("Basket");
        }

        // GET: Basket
        [HttpGet]
        public ActionResult Basket()
        {
            if (Session["Basket"] != null)
            {
                return View();
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
                    User user = Session["User"] as User;
                    List<Product_Order> basket = Session["Basket"] as List<Product_Order>;
                    Order order = new Order
                    {
                        date = DateTime.UtcNow.AddHours(-3),
                        id_user = user.id,
                        status = false
                    };
                    using (var transaccion = new TransactionScope())
                    {
                        db.Order.Add(order);
                        db.SaveChanges();
                        foreach (Product_Order item in basket)
                        {
                            Product_Order prod_order = new Product_Order
                            {
                                id_product = item.Product.id,
                                id_order = order.id,
                                id_business_sender = null,
                                quantity = item.quantity,
                                quantity_send = 0,
                                status = false
                            };

                            db.Product_Order.Add(prod_order);
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
                            TempData["Message"] = "La cantidad seleccionada es mayor a la disponible";
                            TempData["Error"] = 1;
                            return RedirectToAction("Details", new { id = id_order });
                        } else if (quant.Value < 0 )
                        {
                            TempData["Message"] = "Debes seleccionar una cantidad mayor o igual a 0";
                            TempData["Error"] = 1;
                            return RedirectToAction("Details", new { id = id_order });
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
                                    TempData["Message"] = "La sucursal no cuenta con suficiente stock";
                                    TempData["Error"] = 1;
                                    return RedirectToAction("Details", new { id = id_order });
                                }
                            } else
                            {
                                TempData["Message"] = "La sucursal no tiene registrado el producto";
                                TempData["Error"] = 1;
                                return RedirectToAction("Details", new { id = id_order });
                            }

                            Product_Business prod_b_receiver = db.Product_Business.Where(p => p.id_product == id_product.Value && p.id_business == prod_order.Order.User.Business.id).FirstOrDefault();
                            prod_b_receiver.stock += quant.Value;
                        }

                        prod_order.status = true;
                        prod_order.id_business_sender = id_business;

                        db.SaveChanges();
                        TempData["Message"] = "Producto confirmado correctamente";
                        return RedirectToAction("Details", new { id = id_order});
                    }
                }
            }
            catch (Exception)
            {
                TempData["Message"] = "Ha ocurrido un error inesperado. No se ha podido confirmar el producto";
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
                return RedirectToAction("Details", new { id = id_order });
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
                        return RedirectToAction("Details", new { id = id_order });
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
                TempData["Error"] = 2;
                return RedirectToAction("Details", new { id = id_order });
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
                    return RedirectToAction("Details", new { id });
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
                TempData["Error"] = 2;
                return RedirectToAction("Details", new { id });
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
                    return RedirectToAction("Details", new { id });
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
                TempData["Error"] = 2;
                return RedirectToAction("Details", new { id });
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
                    .Where(p =>
                    p.Product.Brand.deleted_at.Equals(null) &&
                    p.deleted_at.Equals(null) &&
                    p.Product.id.Equals(id_product) &&
                    p.id_business != id_business &&
                    p.stock >= quant)
                    .Select( p => new
                    {
                        p.id_business,
                        p.Business.adress,
                        p.stock
                    })
                    .ToList();
                return Json(query, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
