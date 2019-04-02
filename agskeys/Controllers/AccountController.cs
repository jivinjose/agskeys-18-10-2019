using agskeys.Models;
using PasswordSecurity;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace agskeys.Controllers
{
    public class AccountController : Controller
    {
        agsfinancialsEntities ags = new agsfinancialsEntities();
        // GET: Account
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }
        public ActionResult Login()
        {
            var getEmployeeCategoty = ags.emp_category_table.Where(x => x.status == "publish").ToList();
            SelectList list = new SelectList(getEmployeeCategoty, "emp_category_id", "emp_category");
            ViewBag.categoryList = list;
            // ags.admin_table = new admin_table();
            return View();
        }
        public ActionResult MobileLogin()
        {

            return RedirectToAction("Index", "AgskeysMobile");
        }
        [HttpPost]
        public ActionResult MobileLogin(FormCollection form, vendor_table obj)
        {
            if (form["userlevel"].ToString() == "")
            {
                TempData["Message"] = "please select userrole";
                return RedirectToAction("Index", "AgskeysMobile");
            }
            else if (form["userlevel"].ToString() == "partner")
            { 
                string userName = form["userName"].ToString();
                string passwordfrom = form["password"].ToString();
                string userlevel = form["userlevel"].ToString();
                var vndr = (from u in ags.vendor_table where u.username == userName select u).FirstOrDefault();
                if (vndr == null)
                {
                    //TempData["Message"] = "<script>alert('username or password is wrong');</script>";
                    TempData["Message"] = "username or password is wrong";
                    return RedirectToAction("Index", "AgskeysMobile");
                }
                else if (vndr != null)
                {
                    var model = ags.vendor_table.Where(x => x.username == userName).SingleOrDefault();
                    bool result = PasswordStorage.VerifyPassword(passwordfrom, model.password);

                    if (result)
                    {
                        Session["userid"] = vndr.id.ToString();
                        Session["username"] = vndr.username.ToString();
                        Session["userlevel"] = "partner";
                        FormsAuthentication.SetAuthCookie(vndr.username, false);
                        return RedirectToAction("Index", "MobileVendor");
                    }
                    else
                    {
                        TempData["Message"] = "Enter the valid user credentials";
                        return RedirectToAction("Index", "AgskeysMobile");
                    }
                }
                else
                {
                    TempData["Message"] = "username or password is wrong";
                    return RedirectToAction("Index", "AgskeysMobile");
                }

            }
            else if (form["userlevel"].ToString() == "clientele")
            {
                string userName = form["userName"].ToString();
                string passwordfrom = form["password"].ToString();
                string userlevel = form["userlevel"].ToString();
                var customer = (from u in ags.customer_profile_table where u.customerid == userName select u).FirstOrDefault();
                if (customer == null)
                {
                    //TempData["Message"] = "<script>alert('username or password is wrong');</script>";
                    TempData["Message"] = "username or password is wrong";
                    return RedirectToAction("Index", "AgskeysMobile");
                }
                else if (customer != null)
                {
                    var model = ags.customer_profile_table.Where(x => x.customerid == userName).SingleOrDefault();
                    bool result = PasswordStorage.VerifyPassword(passwordfrom, model.password);

                    if (result)
                    {
                        Session["userid"] = customer.id.ToString();
                        Session["username"] = customer.customerid.ToString();
                        Session["userlevel"] = "clientele";
                        FormsAuthentication.SetAuthCookie(customer.customerid, false);
                        return RedirectToAction("Index", "MobileClientele");
                    }
                    else
                    {
                        TempData["Message"] = "Enter the valid user credentials";
                        return RedirectToAction("Index", "AgskeysMobile");
                    }
                }
                else
                {
                    TempData["Message"] = "username or password is wrong";
                    return RedirectToAction("Index", "AgskeysMobile");
                }

            }
            else if (form["userlevel"].ToString() == "sales_executive")
            {
                string userName = form["userName"].ToString();
                string passwordfrom = form["password"].ToString();
                string userlevel = form["userlevel"].ToString();
                var sales_executive = (from u in ags.admin_table where u.username == userName && u.isActive == true select u).FirstOrDefault();
                if (sales_executive == null)
                {
                    //TempData["Message"] = "<script>alert('username or password is wrong');</script>";
                    TempData["Message"] = "username or password is wrong";
                    return RedirectToAction("Index", "AgskeysMobile");
                }
                else if (sales_executive != null)
                {
                    var model = ags.admin_table.Where(x => x.username == userName).SingleOrDefault();
                    bool result = PasswordStorage.VerifyPassword(passwordfrom, model.password);
                    var emp = ags.emp_category_table.Where(x => x.emp_category_id.ToString() == userlevel && x.status == "publish").SingleOrDefault();


                    if (result)
                    {
                        Session["userid"] = sales_executive.id.ToString();
                        Session["username"] = sales_executive.username.ToString();
                        FormsAuthentication.SetAuthCookie(sales_executive.username, false);                              
                        if (emp.emp_category_id == "sales_executive" && emp.emp_category_id == model.userrole)
                        {
                            Session["userlevel"] = form["userlevel"].ToString();
                            return RedirectToAction("Index", "MobileSalesExecutive");
                        }
                        else
                        {
                            TempData["Message"] = "Enter the valid user credentials";
                            return RedirectToAction("Index", "AgskeysMobile");
                        }
                    }
                    else
                    {
                        TempData["Message"] = "Enter the valid user credentials";
                        return RedirectToAction("Index", "AgskeysMobile");
                    }
                }
                else
                {
                    TempData["Message"] = "username or password is wrong";
                    return RedirectToAction("Index", "AgskeysMobile");
                }

            }
            return View();

        }



        [HttpPost]
        public ActionResult ClientLogin(FormCollection form, vendor_table obj)
        {
            if (form["userlevel"].ToString() == "")
            {
                TempData["Message"] = "please select userrole";
                return RedirectToAction("Index", "AgskeysSite");
            }
            else if (form["userlevel"].ToString() == "partner")
            {
                string userName = form["userName"].ToString();
                string passwordfrom = form["password"].ToString();
                string userlevel = form["userlevel"].ToString();
                var vndr = (from u in ags.vendor_table where u.username == userName select u).FirstOrDefault();
                if (vndr == null)
                {
                    //TempData["Message"] = "<script>alert('username or password is wrong');</script>";
                    TempData["Message"] = "username or password is wrong";
                    return RedirectToAction("Index", "AgskeysSite");
                }
                else if (vndr != null)
                {
                    var model = ags.vendor_table.Where(x => x.username == userName).SingleOrDefault();
                    bool result = PasswordStorage.VerifyPassword(passwordfrom, model.password);

                    if (result)
                    {
                        Session["userid"] = vndr.id.ToString();
                        Session["username"] = vndr.username.ToString();
                        Session["userlevel"] = "partner";
                        FormsAuthentication.SetAuthCookie(vndr.username, false);
                        return RedirectToAction("Index", "Partner");
                    }
                    else
                    {
                        TempData["Message"] = "Enter the valid user credentials";
                        return RedirectToAction("Index", "AgskeysSite");
                    }
                }
                else
                {
                    TempData["Message"] = "username or password is wrong";
                    return RedirectToAction("Index", "AgskeysSite");
                }

            }
            else if (form["userlevel"].ToString() == "clientele")
            {
                string userName = form["userName"].ToString();
                string passwordfrom = form["password"].ToString();
                string userlevel = form["userlevel"].ToString();
                var customer = (from u in ags.customer_profile_table where u.customerid == userName select u).FirstOrDefault();
                if (customer == null)
                {
                    //TempData["Message"] = "<script>alert('username or password is wrong');</script>";
                    TempData["Message"] = "username or password is wrong";
                    return RedirectToAction("Index", "AgskeysSite");
                }
                else if (customer != null)
                {
                    var model = ags.customer_profile_table.Where(x => x.customerid == userName).SingleOrDefault();
                    bool result = PasswordStorage.VerifyPassword(passwordfrom, model.password);

                    if (result)
                    {
                        Session["userid"] = customer.id.ToString();
                        Session["username"] = customer.customerid.ToString();
                        Session["userlevel"] = "clientele";
                        FormsAuthentication.SetAuthCookie(customer.customerid, false);
                        return RedirectToAction("Index", "clientele");
                    }
                    else
                    {
                        TempData["Message"] = "Enter the valid user credentials";
                        return RedirectToAction("Index", "AgskeysSite");
                    }
                }
                else
                {
                    TempData["Message"] = "username or password is wrong";
                    return RedirectToAction("Index", "AgskeysSite");
                }

            }
            else if (form["userlevel"].ToString() == "sales_executive")
            {
                string userName = form["userName"].ToString();
                string passwordfrom = form["password"].ToString();
                string userlevel = form["userlevel"].ToString();
                var sales_executive = (from u in ags.admin_table where u.username == userName && u.isActive == true select u).FirstOrDefault();
                if (sales_executive == null)
                {
                    //TempData["Message"] = "<script>alert('username or password is wrong');</script>";
                    TempData["Message"] = "username or password is wrong";
                    return RedirectToAction("Index", "AgskeysSite");
                }
                else if (sales_executive != null)
                {
                    var model = ags.admin_table.Where(x => x.username == userName).SingleOrDefault();
                    bool result = PasswordStorage.VerifyPassword(passwordfrom, model.password);
                    var emp = ags.emp_category_table.Where(x => x.emp_category_id.ToString() == userlevel && x.status == "publish").SingleOrDefault();


                    if (result)
                    {
                        Session["userid"] = sales_executive.id.ToString();
                        Session["username"] = sales_executive.username.ToString();
                        FormsAuthentication.SetAuthCookie(sales_executive.username, false);
                        if (emp.emp_category_id == "sales_executive" && emp.emp_category_id == model.userrole)
                        {
                            Session["userlevel"] = form["userlevel"].ToString();
                            return RedirectToAction("Index", "SalesExecutive");
                        }
                        else
                        {
                            TempData["Message"] = "Enter the valid user credentials";
                            return RedirectToAction("Index", "AgskeysSite");
                        }
                    }
                    else
                    {
                        TempData["Message"] = "Enter the valid user credentials";
                        return RedirectToAction("Index", "AgskeysSite");
                    }
                }
                else
                {
                    TempData["Message"] = "username or password is wrong";
                    return RedirectToAction("Index", "AgskeysSite");
                }

            }
            return View();

        }





        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Login");
        }
        [AuthorizeUser]
        public ActionResult ClientLogout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index", "AgskeysSite");
        }

        [AuthorizeMobileUser]
        public ActionResult MobileLogout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index", "AgskeysMobile");
        }



        [HttpPost]
        public ActionResult Login(FormCollection form, admin_table obj)
        {
            string userName = form["userName"].ToString();
            string password = form["password"].ToString();

            string userlevel = obj.userrole.ToString();

            if (obj.userrole == null)
            {
                TempData["Message"] = "please select userrole";
                return RedirectToAction("Login", "Account");
            }
            else if (obj.userrole == "partner")
            {
                var vndr = (from u in ags.vendor_table where u.username == userName select u).FirstOrDefault();
                if (vndr == null)
                {
                    TempData["Message"] = "username or password is wrong";
                    return RedirectToAction("Login", "Account");
                    // return View();
                }
                else if (vndr != null)
                {
                    var model = ags.vendor_table.Where(x => x.username == userName).SingleOrDefault();
                    bool result = PasswordStorage.VerifyPassword(password, model.password);

                    string usrrole = obj.userrole.ToString();
                    var emp = ags.emp_category_table.Where(x => x.emp_category_id.ToString() == usrrole && x.status == "publish").SingleOrDefault();
                    if (result)
                    {
                        Session["userid"] = vndr.id.ToString();
                        Session["username"] = vndr.username.ToString();
                        FormsAuthentication.SetAuthCookie(vndr.username, false);
                        if (emp.emp_category_id == "partner")
                        {
                            Session["userlevel"] = obj.userrole.ToString();
                            return RedirectToAction("Index", "Partner");
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
            else if (obj.userrole == "clientele")
            {
                var clientele = (from u in ags.customer_profile_table where u.customerid == userName select u).FirstOrDefault();
                if (clientele == null)
                {
                    TempData["Message"] = "username or password is wrong";
                    return RedirectToAction("Login", "Account");
                    // return View();
                }
                else if (clientele != null)
                {
                    var model = ags.customer_profile_table.Where(x => x.customerid == userName).SingleOrDefault();
                    bool result = PasswordStorage.VerifyPassword(password, model.password);

                    string usrrole = obj.userrole.ToString();
                    var emp = ags.emp_category_table.Where(x => x.emp_category_id.ToString() == usrrole && x.status == "publish").SingleOrDefault();
                    if (result)
                    {
                        Session["userid"] = clientele.id.ToString();
                        Session["username"] = clientele.customerid.ToString();
                        FormsAuthentication.SetAuthCookie(clientele.customerid, false);
                        if (emp.emp_category_id == "clientele")
                        {
                            Session["userlevel"] = obj.userrole.ToString();
                            return RedirectToAction("Index", "Clientele");
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


            else
            {

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
                            return RedirectToAction("Customer", "SalesExecutive");
                        }
                        else if (emp.emp_category_id == "tele_marketing" && emp.emp_category_id == model.userrole)
                        {
                            Session["userlevel"] = obj.userrole.ToString();
                            return RedirectToAction("Customer", "TeleMarketing");
                        }
                        else if (emp.emp_category_id == "process_team" && emp.emp_category_id == model.userrole)
                        {
                            Session["userlevel"] = obj.userrole.ToString();
                            return RedirectToAction("Customer", "ProcessTeam");
                        }
                        else if (emp.emp_category_id == "manager" && emp.emp_category_id == model.userrole)
                        {
                            Session["userlevel"] = obj.userrole.ToString();
                            return RedirectToAction("Customer", "Manager");
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

        }


        public class AuthorizeUserAttribute : AuthorizeAttribute
        {

            protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
            {
                var username = filterContext.HttpContext.User.Identity.Name;
                if (username != "")
                {
                    base.HandleUnauthorizedRequest(filterContext);
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new
                    RouteValueDictionary(new { controller = "Account", action = "ClientLogin" }));
                }
            }
        }



        public class AuthorizeMobileUserAttribute : AuthorizeAttribute
        {

            protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
            {
                var username = filterContext.HttpContext.User.Identity.Name;
                if (username != "")
                {
                    base.HandleUnauthorizedRequest(filterContext);
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new
                    RouteValueDictionary(new { controller = "Account", action = "MobileLogin" }));
                }
            }
        }
        ////site////
        public ActionResult agskey()
        {
            return View();
        }




    }
}