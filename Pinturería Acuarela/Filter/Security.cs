using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Pinturería_Acuarela.Filter
{
    public class Security : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                var user = HttpContext.Current.Session["User"];
                if (user == null)
                {
                    filterContext.Result = new RedirectResult("~/Home");
                }
                base.OnActionExecuting(filterContext);
            }
            catch (Exception)
            {
                filterContext.Result = new RedirectResult("~/Home");
            }
        }
    }
    public class Admin : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                User user = HttpContext.Current.Session["User"] as User;
                if (user != null)
                {
                    Rol rol = user.Rol;
                    if (rol.id != 1)
                    {
                        filterContext.Result = new RedirectResult("~/Home");
                    }
                } else
                {
                    filterContext.Result = new RedirectResult("~/Home");
                }
                base.OnActionExecuting(filterContext);
            }
            catch (Exception)
            {
                filterContext.Result = new RedirectResult("~/Home");
            }
        }
    }
    public class Employee : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                User user = HttpContext.Current.Session["User"] as User;
                if (user != null)
                {
                    Rol rol = user.Rol;
                    if (rol.id != 2)
                    {
                        filterContext.Result = new RedirectResult("~/Home");
                    }
                } else
                {
                    filterContext.Result = new RedirectResult("~/Home");
                }
                base.OnActionExecuting(filterContext);
            }
            catch (Exception)
            {
                filterContext.Result = new RedirectResult("~/Home");
            }
        }
    }
}