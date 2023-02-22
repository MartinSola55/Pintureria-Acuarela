using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Pinturería_Acuarela.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index(User user)
        {
            if (user == null)
            {
                user = new User();
            }
            return View(user);
        }
        public ActionResult Logout()
        {
            Session["User"] = null;
            Session["Basket"] = null;
            Session["Sell"] = null;
            return RedirectToAction("Index", "Login");
        }
        public ActionResult Validate(string email, string password)
        {
            try
            {
                //Cifrar contraseña
                EFModel bd = new EFModel();
                SHA256Managed sha = new SHA256Managed();
                byte[] passNoCifrada = Encoding.Default.GetBytes(password);
                byte[] bytesCifrados = sha.ComputeHash(passNoCifrada);
                string passCifrada = BitConverter.ToString(bytesCifrados).Replace("-", string.Empty);

                User user = bd.User.Where(u => u.email.Equals(email) && u.password.Equals(passCifrada)).FirstOrDefault();

                if (user != null)
                {
                    Session["User"] = user;
                    if (user.Rol.id == 1)
                    {
                        return RedirectToAction("AdminIndex", "Home");
                    } else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                ViewBag.Error = 1;
                ViewBag.Message = "Email y/o contraseña incorrectos";
                return View("Index", new User { email = email });
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                ViewBag.Error = 2;
                return View("Index", new User { email = email });
            }
        }
    }
}