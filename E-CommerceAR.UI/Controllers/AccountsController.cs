using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using System.Resources;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace E_CommerceAR.UI.Controllers
{
    public class AccountsController : BaseController
    {
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("me") == null)
            {
                return View();
            }
            return RedirectToAction("Login", "Accounts");

        }
        public IActionResult Signup()
        {
            return View();

        }
        //[HttpPost]
        //[AllowAnonymous]
        //public ActionResult Index(User model, string Captcha)
        //{
        //    ModelState.Clear();

        //    if (model.UserID == null || model.UserPWD == null)
        //    {
        //        if (model.UserID == null)
        //        {
        //            ModelState.AddModelError(string.Empty, Resources.Resource.EmptyEmail);
        //        }
        //        if (model.UserPWD == null)
        //        {
        //            ModelState.AddModelError(string.Empty, Resources.Resource.EmptyPassword);
        //        }
        //        return View();
        //    }
        //    return View();
        //}
        [HttpGet]
        public ActionResult Logout()
        {
            HttpContext.Session.Remove("me"); ;
            return RedirectToAction("Index", "Account");
        }
        [HttpPost]
        public JsonResult ChangeLanguage()
        {
            string r = HttpContext.Session.GetString("language").ToString();
            string l = "";
            if (r == "en")
            {
                l = "ar-Pl";
                HttpContext.Session.SetString("language", l);
                Response.Cookies.Append("E-CommerceAR_Lang", l, new CookieOptions
                {
                    Expires = DateTime.Now.AddYears(10)
                });
            }
            else
            {
                l = "en";
                HttpContext.Session.SetString("language", l);
                Response.Cookies.Append("E-CommerceAR_Lang", l, new CookieOptions
                {
                    Expires = DateTime.Now.AddYears(10)
                });
            }
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(l);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(l);
            CultureInfo ci = new CultureInfo(l);
            ci.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            Thread.CurrentThread.CurrentCulture = ci;
            return Json(new { ok = "ok" },new JsonSerializerSettings()); 
        }
    

    [HttpPost]
        private bool IsValidEmail(string email)
        {
            try
            {
                System.Net.Mail.MailAddress addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
