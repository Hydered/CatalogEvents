using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;

namespace WebApplication5.Controllers
{
    public class AccountController : Controller
    {
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Login(string username, string password, string returnUrl)
        {
            var configuredUser = ConfigurationManager.AppSettings["AdminUser"];
            var configuredHash = ConfigurationManager.AppSettings["AdminPasswordHash"];
            if (string.IsNullOrEmpty(configuredHash))
                ModelState.AddModelError("", "Пароль администратора не настроен в Web.config.");
            else if (string.Equals(username, configuredUser, StringComparison.Ordinal) &&
                     string.Equals(HashPassword(password), configuredHash, StringComparison.Ordinal))
            {
                FormsAuthentication.SetAuthCookie(username, false);
                if (Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                return RedirectToAction("Index", "Events");
            }
            else
                ModelState.AddModelError("", "Неверное имя пользователя или пароль.");

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Events");
        }

        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password ?? ""));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}