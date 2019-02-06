using agskeys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace agskeys.Controllers.Manager
{
    [Authorize]
    public class ManagerController : Controller
    {
        // GET: Manager
        agsfinancialsEntities ags = new agsfinancialsEntities();
        public ActionResult Index()
        {

            if (Session["username"] == null || Session["userlevel"].ToString() != "manager")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            return View("~/Views/Manager/Manager/Index.cshtml");
        }



        public ActionResult Customer()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "manager")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            var customers = (from customer in ags.customer_profile_table orderby customer.id descending select customer).ToList();

            return PartialView("~/Views/Manager/Manager/Customer.cshtml");
        }
        public ActionResult Details(int Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "manager")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            var user = ags.customer_profile_table.Where(x => x.id == Id).FirstOrDefault();
            if (user == null)
            {
                return HttpNotFound();
            }
            return PartialView("~/Views/Manager/Manager/Details.cshtml");
        }
    }
}