using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using Pinturería_Acuarela;
using Pinturería_Acuarela.Filter;

namespace Pinturería_Acuarela.Controllers
{
    [Security]
    public class SellsController : Controller
    {
        private EFModel db = new EFModel();

        // GET: Sells/Create
        [Employee]
        public ActionResult Create()
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: Products by filter
        [Employee]
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
        [Employee]
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
        [Employee]
        [HttpGet]
        public ActionResult AddToSale(int? id_prod, int? quant)
        {
            try
            {
                User user = Session["User"] as User;
                List<Product_Sell> sell = new List<Product_Sell>();
                if (id_prod != null && quant != null)
                {
                    if (Session["Sell"] != null)
                    {
                        sell = Session["Sell"] as List<Product_Sell>;
                    }
                    if (quant.Value > 0)
                    {
                        // Check del stock
                        int stock = db.Product_Business.Where(b => b.id_business.Equals(user.id_business.Value) && b.id_product.Equals(id_prod.Value)).FirstOrDefault().stock;

                        Product_Sell prod = new Product_Sell();
                        prod.Product = db.Product
                            .Where(p => p.id.Equals(id_prod.Value))
                            .Include(p => p.Brand)
                            .Include(p => p.Category)
                            .Include(p => p.Subcategory)
                            .Include(p => p.Capacity)
                            .Include(p => p.Product_Business)
                            .First();

                        // Si no hay ningun producto agregado
                        if (sell.Count == 0 && quant.Value <= stock)
                        {
                            prod.quantity = quant.Value;
                            sell.Add(prod);
                            TempData["Message"] = "El producto se agregó correctamente";
                            Session["Sell"] = sell.Count == 0 ? null : sell;
                            return RedirectToAction("Create");
                        }
                        else
                        {
                            int count = sell.Count;
                            for (int index = 0; index < count; index++)
                            {
                                // Si se repite el producto
                                if (sell[index].Product.id == prod.Product.id)
                                {
                                    if (sell[index].quantity + quant.Value <= stock)
                                    {
                                        sell[index].quantity += quant.Value;
                                        TempData["Message"] = "El producto se sumó correctamente";
                                        Session["Sell"] = sell.Count == 0 ? null : sell;
                                        return RedirectToAction("Create");
                                    }
                                    else
                                    {
                                        TempData["Message"] = "No cuentas con stock suficiente";
                                        TempData["Error"] = 1;
                                        return RedirectToAction("Create");
                                    }
                                }
                                // Si no se repite el producto
                                else if (index == sell.Count - 1)
                                {
                                    prod.quantity = quant.Value;
                                    sell.Add(prod);
                                    TempData["Message"] = "El producto se agregó correctamente";
                                    Session["Sell"] = sell.Count == 0 ? null : sell;
                                    return RedirectToAction("Create");
                                }
                            }
                        }
                    }
                    TempData["Message"] = "Por favor, selecciona una cantidad mayor a 0";
                    TempData["Error"] = 1;
                    return RedirectToAction("Create");
                }
                TempData["Message"] = "No has seleccionado un producto o una cantidad correctamente";
                TempData["Error"] = 1;
                return RedirectToAction("Create");
            }
            catch (Exception)
            {
                TempData["Message"] = "Hubo un error al agregar el producto la venta";
                TempData["Error"] = 2;
                return RedirectToAction("Create");
            }
        }

        [Employee]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveProductSell(int? id_prod)
        {
            try
            {
                if (id_prod != null && Session["Sell"] != null)
                {
                    List<Product_Sell> sell = Session["Sell"] as List<Product_Sell>;
                    Product_Sell prod_s = sell.Where(p => p.Product.id.Equals(id_prod.Value)).FirstOrDefault();
                    sell.Remove(prod_s);
                    if (sell.Count > 0)
                    {
                        Session["Sell"] = sell;
                    }
                    else
                    {
                        Session["Sell"] = null;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return RedirectToAction("Create");
        }

        // POST: Confirm a sale
        [Employee]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmSale()
        {
            try
            {
                if (Session["Sell"] != null)
                {
                    User user = Session["User"] as User;
                    List<Product_Sell> products = Session["Sell"] as List<Product_Sell>;
                    
                    Sell sell = new Sell
                    {
                        date = DateTime.UtcNow.AddHours(-3),
                        id_user = user.id
                    };

                    using (var transaccion = new TransactionScope())
                    {
                        db.Sell.Add(sell);
                        db.SaveChanges();
                                                
                        foreach(Product_Sell item in products)
                        {
                            Product_Sell prod_sell = new Product_Sell
                            {
                                id_sell = sell.id,
                                id_product = item.Product.id,
                                quantity = item.quantity,
                            };
                            
                            db.Product_Sell.Add(prod_sell);

                            db.SaveChanges();

                            prod_sell.Product = db.Product.Where(p => p.id == prod_sell.id_product).First();
                            
                            prod_sell.Product.Product_Business.Where(pb => pb.id_business.Equals(user.id_business)).First().stock -= item.quantity;
                            db.SaveChanges();                            
                        }

                        Session["Sell"] = null;
                        transaccion.Complete();
                    }
                    TempData["Message"] = "Venta realizada con éxito";
                    return RedirectToAction("Create");
                }
                else
                {
                    TempData["Error"] = 1;
                    TempData["Message"] = "No se han agregado productos a la venta";
                    return RedirectToAction("Create");
                }
            }
            catch (Exception)
            {
                TempData["Error"] = 2;
                TempData["Message"] = "Ha ocurrido un error inesperado. La venta no se ha podido realizar";
                return RedirectToAction("Create");
            }
        }

        // GET: Stats page
        [Admin]
        public ActionResult Stats()
        {
            return View();
        }

        // GET: Top 10 most sold products
        [Admin]
        public JsonResult MostSoldProducts(string dates, int id_business)
        {
            try
            {
                string[] dates_formated = dates.Trim().Split(',');
                DateTime date_from = Convert.ToDateTime(dates_formated[0]);
                DateTime date_to = Convert.ToDateTime(dates_formated[1]);

                Business business = db.Business.Find(id_business);

                var prod = db.Product_Sell
                    .Where(ps => ps.Sell.date >= date_from && ps.Sell.date <= date_to && ps.Sell.User.Business.id.Equals(id_business))
                    .GroupBy(po => po.id_product)
                    .Select(pr => new
                    {
                        db.Product.Where(p => p.id.Equals(pr.Key)).FirstOrDefault().description,
                        quantity = pr.Sum(p => p.quantity),
                        business_name = business.adress,
                        liters = db.Product.Where(p => p.id.Equals(pr.Key)).FirstOrDefault().id_capacity != null ? db.Product.Where(p => p.id.Equals(pr.Key)).FirstOrDefault().Capacity.capacity * pr.Sum(p => p.quantity) : 0,
                    })
                    .OrderByDescending(a => a.quantity)
                    .Take(10);

                return Json(prod, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: All business ids
        [Admin]
        public JsonResult GetBusinessIDS()
        {
            try
            {
                return Json(db.Business.Select(b => b.id), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult Details()
        {
            try
            {
                return View();
            }
            catch(Exception)
            {
                return HttpNotFound();
            }
        }

        public JsonResult ShowSales(string dates)
        {
            try
            {
                string[] dates_formated = dates.Trim().Split(',');
                DateTime date_from = Convert.ToDateTime(dates_formated[0]);
                DateTime date_to = Convert.ToDateTime(dates_formated[1]);

                User user = Session["User"] as User;

                var sales = db.Sell.Where(s => s.User.id.Equals(user.id) && s.date <= date_to && s.date > date_from)
                    .Select(s => new {s.id, date = s.date.ToString()}).OrderByDescending(s => s.date).ToList();
                return Json(sales,JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                TempData["Error"] = 2;
                TempData["Message"] = "Ha ocurrido un error inesperado. No se puede visualizar el historial de ventas";
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DetailsSale(int? id)
        {
            try
            {
                Session["ProdSell"] = null;
                List<Product> SessionPro = Session["ProdSell"] as List<Product>;
                SessionPro = new List<Product>();
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                List<Product_Sell> product = db.Product_Sell.Where(p => p.id_sell.Equals(id.Value)).ToList();
                for(var i = 0; i< product.Count(); i++)
                {
                    Product pro = db.Product.Find(product[i].id_product);
                    pro.Product_Sell.Add(product[i]);
                    SessionPro.Add(pro);
                }
                Session["ProdSell"] = SessionPro;
                if (product == null)
                {
                    return HttpNotFound();
                }
                
                return View();
            }
            catch (Exception e)
            {
                return RedirectToAction("Create");
            }
        }

    }
}
