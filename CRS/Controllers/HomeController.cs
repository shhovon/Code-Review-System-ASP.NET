using CRS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CRS.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (CRSEntities db = new CRSEntities())
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                int totalSubmissions = db.CodeSubmissions.Count(cs => cs.UserId == userId);

                ViewBag.TotalSubmissions = totalSubmissions;
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Login()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            using (CRSEntities db = new CRSEntities())
            {
                var user = db.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == password);

                if (user != null)
                {
                    Session["UserId"] = user.UserId;
                    FormsAuthentication.SetAuthCookie(user.Username, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password");
                    return View();
                }
            }
        }


        // GET: Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }
    }
}