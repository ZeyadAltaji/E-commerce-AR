using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Globalization;
using Microsoft.AspNetCore.Http;
 namespace E_CommerceAR.UI.Controllers
{
    public class BaseController : Controller
    {
        public string Title { get; set; }
        private string Lang;
        //private User user;

        public string Language
        {
            get
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("language")))
                {
                    Lang = "ar";
                }
                return Lang;
            }
        }
        //public User me
        //{
        //    get
        //    {
        //        if ((User)Session["me"] != null)
        //        {
        //            user = (User)Session["me"];
        //        }
        //        return user;
        //    }
        //}
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string CuurentURL = filterContext.Controller.ToString();
            string originAction = filterContext.RouteData.Values["action"].ToString();

            string theme = Request.Cookies["Theme"];
            string langName = "";

            if (HttpContext.Session.GetString("Theme") == null)
            {
                try
                {
                    if (string.IsNullOrEmpty(theme))
                    {
                        HttpContext.Session.SetString("Theme", "sidebar-mini skin-purple");
                    }
                    else
                    {
                        HttpContext.Session.SetString("Theme", theme);
                    }
                }
                catch (Exception)
                {
                    HttpContext.Session.SetString("Theme", "sidebar-mini skin-purple");
                }
            }

            if (HttpContext.Session.GetString("language") == null)
            {
                langName = "ar-Pl";
                HttpContext.Session.SetString("language", "ar-Pl");
                Response.Cookies.Append("E-CommerceAR_Lang", "ar-Pl", new CookieOptions
                {
                    Expires = DateTime.Now.AddYears(10)
                });
            }
            else
            {
                langName = HttpContext.Session.GetString("language");
                Response.Cookies.Append("E-CommerceAR_Lang", langName, new CookieOptions
                {
                    Expires = DateTime.Now.AddYears(10)
                });
            }

            ViewBag.ID = langName;
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(langName);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(langName);
            CultureInfo ci = new CultureInfo(langName);
            ci.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            Thread.CurrentThread.CurrentCulture = ci;

            if (!filterContext.Controller.ToString().Contains("AccountsController"))
            {
                filterContext.HttpContext.Response.Headers.Add("Cache-Control", "no-store");
                if (HttpContext.Session.GetString("me") == null)
                {
                    filterContext.Result = new RedirectResult(Url.Action("Logout", "Account", new { area = string.Empty }));
                }
            }
        }
    }
}
