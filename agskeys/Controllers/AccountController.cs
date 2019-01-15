using agskeys.Models;
using PasswordSecurity;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace agskeys.Controllers
{
    public class AccountController : Controller
    {
        agsfinancialsEntities ags = new agsfinancialsEntities();
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            // ags.admin_table = new admin_table();
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection form)
        {
            string userName = form["userName"].ToString();
            string password = form["password"].ToString();
            string userlevel = form["userlevel"].ToString();
            if (userlevel == "1")
            {
                userlevel = "ab1d92aaed90828a0fc1c95ecd5fda1f";
            }
            else if (userlevel == "2")
            {
                userlevel = "079e946e1272938b097a0baab6a36477";
            }
            else
            {
                userlevel = "";
            }
            var user = (from u in ags.admin_table where u.username == userName && u.userrole == userlevel && u.isActive == true select u).FirstOrDefault();
            if (user == null)
            {
                //return RedirectToAction("Login", "Account");
                return View();
            }
            var model = ags.admin_table.Where(x => x.username == userName).SingleOrDefault();
            bool result = PasswordStorage.VerifyPassword(password, model.password);
            Session["userid"] = user.id.ToString();
            Session["username"] = user.username.ToString();
            Session["userlevel"] = user.userrole.ToString();
            if (user != null)
            {
                if (result)
                {
                    FormsAuthentication.SetAuthCookie(user.username, false);
                    if (user.userrole == "ab1d92aaed90828a0fc1c95ecd5fda1f")
                    {
                        return RedirectToAction("Index", "SuperAdmin");
                    }
                    else if (user.userrole == "079e946e1272938b097a0baab6a36477")
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        TempData["Message"] = "Enter the valid user credentials";
                        return View();
                    }
               }

            }
            TempData["Message"] = "username or password is wrong";

            return View();
        }
        //public ActionResult Login(string userName, string password)
        //{
        //    if(userName !=null && password != null)
        //    {
        //        var model = ags.admin_table.Where(x => x.username == userName).SingleOrDefault();
        //        if (model == null)
        //        {
        //            //return RedirectToAction("Login", "Account");
        //            return View();
        //        }
        //        bool result = PasswordStorage.VerifyPassword(password, model.password);
        //        if (result)
        //        {
        //            return RedirectToAction("Index", "SuperAdmin");
        //        }               
        //    }
        //    return View();
        //}
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}