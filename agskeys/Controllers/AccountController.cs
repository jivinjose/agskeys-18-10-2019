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
            var getEmployeeCategoty = ags.emp_category_table.Where(x => x.status == "publish").ToList();
            SelectList list = new SelectList(getEmployeeCategoty, "emp_category_id", "emp_category");
            ViewBag.categoryList = list;
            // ags.admin_table = new admin_table();
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection form, admin_table obj)
        {
            string userName = form["userName"].ToString();
            string password = form["password"].ToString();
            if (obj.userrole == null)
            {
                TempData["Message"] = "please select userrole";
                return RedirectToAction("Login", "Account");
            }
            string userlevel = obj.userrole.ToString();
           
            var user = (from u in ags.admin_table where u.username == userName && u.userrole == userlevel && u.isActive == true select u).FirstOrDefault();
            if (user == null)
            {
                TempData["Message"] = "username or password is wrong";
                return RedirectToAction("Login", "Account");
                // return View();
            }
            else if (user != null)
            {
                var model = ags.admin_table.Where(x => x.username == userName).SingleOrDefault();
                bool result = PasswordStorage.VerifyPassword(password, model.password);

                string usrrole = obj.userrole.ToString();
                var emp = ags.emp_category_table.Where(x => x.emp_category_id.ToString() == usrrole && x.status == "publish").SingleOrDefault();
                if (result)
                {
                    Session["userid"] = user.id.ToString();
                    Session["username"] = user.username.ToString();
                    FormsAuthentication.SetAuthCookie(user.username, false);
                    if (emp.emp_category_id == "super_admin" && emp.emp_category_id == model.userrole)
                    {
                        Session["userlevel"] = obj.userrole.ToString();
                        return RedirectToAction("Index", "SuperAdmin");
                    }
                    else if (emp.emp_category_id == "admin" && emp.emp_category_id == model.userrole)
                    {
                        Session["userlevel"] = obj.userrole.ToString();
                        return RedirectToAction("Index", "Admin");
                    }
                    else if (emp.emp_category_id == "sales_executive" && emp.emp_category_id == model.userrole)
                    {
                        Session["userlevel"] = obj.userrole.ToString();
                        return RedirectToAction("Index", "Admin");
                    }
                    else if (emp.emp_category_id == "tele_marketing" && emp.emp_category_id == model.userrole)
                    {
                        Session["userlevel"] = obj.userrole.ToString();
                        return RedirectToAction("Index", "TeleMarketing");
                    }
                    else if (emp.emp_category_id == "process_team" && emp.emp_category_id == model.userrole)
                    {
                        Session["userlevel"] = obj.userrole.ToString();
                        return RedirectToAction("Index", "ProcessTeam");
                    }
                    else if (emp.emp_category_id == "manager" && emp.emp_category_id == model.userrole)
                    {
                        Session["userlevel"] = obj.userrole.ToString();
                        return RedirectToAction("Index", "Manager");
                    }
                    else
                    {
                        TempData["Message"] = "Enter the valid user credentials";
                        return RedirectToAction("Login", "Account");
                    }
                }
                else
                {
                    TempData["Message"] = "Enter the valid user credentials";
                    return RedirectToAction("Login", "Account");
                }
            }
            else
            {
                TempData["Message"] = "username or password is wrong";
                return RedirectToAction("Login", "Account");
            }
        }

        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Login");
        }
    }
}