using Microsoft.AspNetCore.Mvc;

namespace Pharmacy.Areas.Admin.Controllers
{
    public class HomeAdminController : Controller
    {
        //public IActionResult Index()
        //{
        //    if (Session["user"] == null)
        //    {
        //        return RedirectToAction("Login");
        //    }
        //    else
        //    {
        //        return View();
        //    }

        //}

        //public IActionResult Login()
        //{

        //    return View();
        //}

        //[HttpPost]
        //public IActionResult Login(string username, string password)
        //{
        //    QLPharmacyEntities db = new QLPharmacyEntities();

        //    var user = db.Account.SingleOrDefault(u => u.UserName.Equals(username) && u.Password.Equals(password));


        //    if (user != null)
        //    {
        //        Session["user"] = user;
        //        if (user.Type == 0)
        //        {
        //            return RedirectToAction("Index", "../Home");
        //        }

        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        TempData["error"] = "Tài khoản đăng nhập không đúng";
        //        return View();
        //    }
        //}
        //public IActionResult Logout()
        //{
        //    Session.Remove("user");
        //    FormsAuthentication.SignOut();

        //    return RedirectToAction("Login");
        //}
    }
}
