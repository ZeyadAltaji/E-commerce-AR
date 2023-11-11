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
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;

namespace E_CommerceAR.UI.Controllers
{
    public class AccountsController : BaseController
    {
        FirebaseAuthProvider auth;
		private readonly FirestoreDb firestoreDb;

		public AccountsController()
        {
            auth = new FirebaseAuthProvider(
                        new FirebaseConfig(ApiKey));
			System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "C:\\Users\\ziada\\Source\\repos\\E-commerce AR\\E-CommerceAR.UI\\Extensions\\finalprojectar-d85ea-5769d392d7b0.json");

		}
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
                    var user = await FetchUserFromDatabase(loginModel.Email);

                    if (user != null)
                    {

                        switch (user.Role)
                        {
                            case 1:
                                return RedirectToAction("Index", "Home", new { area = "AdminDashboard" });

                            case 2:
                                return RedirectToAction("Index", "Home", new { area = "AdminDashboard" });
							default:
								return StatusCode(404);

						}

                    }
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
		private async Task<Login> FetchUserFromDatabase(string email)
		{
			try
			{
				FirestoreDb db = FirestoreDb.Create("finalprojectar-d85ea");

				// Assuming "users" is your collection and "Email" is the field you want to search
				Query query = db.Collection("user").WhereEqualTo("email", email);
				QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

				if (querySnapshot.Documents.Count > 0)
				{
					// Assuming you have a User class to represent your user data
					return querySnapshot.Documents[0].ConvertTo<Login>();
				}

				return null;

			}
			catch (Exception ex)
			{
 				return null;
			}
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

				// Set default values for properties
				SignupModel.IsActive = true;
				SignupModel.IsDeleted = false;
				SignupModel.Role = 2;

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

			return RedirectToAction("Login", "Accounts");
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
