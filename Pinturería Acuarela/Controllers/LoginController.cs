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
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CloseSession()
        {
            Session["idUsuario"] = null;
            return View("Index");
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

                Usuario user = bd.Usuario.Where(u => u.email.Equals(email) && u.password.Equals(passCifrada)).FirstOrDefault();

                if (user != null)
                {
                    Session["idUsuario"] = user.id;
                    return View("Index");
                }
                ViewBag.Error = 1;
                ViewBag.Message = "Email y/o contraseña incorrectos";
                return View("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                ViewBag.Error = 2;
                return View("Index");
            }
        }
    }
}