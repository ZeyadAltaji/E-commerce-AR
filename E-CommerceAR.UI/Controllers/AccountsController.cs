using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using System.Resources;
using System.Security.Cryptography;
using Newtonsoft.Json;
using E_CommerceAR.Domain;
using Firebase.Auth;
using E_CommerceAR.UI.Extensions;

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
        [HttpPost]
        public async Task<IActionResult> Login(Login loginModel)
        {
            try
            {
                if (!IsValidEmail(loginModel.Email))
                {
                    ModelState.AddModelError("Email", "Invalid email address");
                    return View(loginModel);
                }

                var fbAuthLink = await auth.SignInWithEmailAndPasswordAsync(loginModel.Email, loginModel.Password);
                string token = fbAuthLink.FirebaseToken;
                if (token != null)
                {
                    HttpContext.Session.SetString("_UserToken", token);

                    return RedirectToAction("Index");
                }

            }
            catch (FirebaseAuthException ex)
            {
                var firebaseEx = JsonConvert.DeserializeObject<FirebaseError>(ex.ResponseData);
                ModelState.AddModelError(String.Empty, firebaseEx.error.message);
                return View(loginModel);
            }

            return View();
        }

        public IActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Signup(Signup SignupModel)
        {
            try
            {
                if (!IsValidEmail(SignupModel.Email))
                {
                    ModelState.AddModelError("Email", "Invalid email address");
                    return View(SignupModel);
                }

                await auth
                    .CreateUserWithEmailAndPasswordAsync(SignupModel.Email, SignupModel.Password);
                var fbAuthLink = await auth
                                .SignInWithEmailAndPasswordAsync(SignupModel.Email, SignupModel.Password);
                string token = fbAuthLink.FirebaseToken;
                if (token != null)
                {
                    HttpContext.Session.SetString("_UserToken", token);

                    return RedirectToAction("Index");
                }
            }
            catch (FirebaseAuthException ex)
            {
                var firebaseEx = JsonConvert
                    .DeserializeObject<FirebaseError>(ex.ResponseData);
                ModelState.AddModelError(String.Empty, firebaseEx.error.message);
                return View(SignupModel);
            }

            return View();

        }
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
