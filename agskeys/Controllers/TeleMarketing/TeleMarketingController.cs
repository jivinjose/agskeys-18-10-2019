using agskeys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace agskeys.Controllers.TeleMarketing
{
    [Authorize]
    public class TeleMarketingController : Controller
    {
        // GET: TeleMarketing
        agsfinancialsEntities ags = new agsfinancialsEntities();
        public ActionResult Index()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "tele_marketing")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            return View("~/Views/TeleMarketing/TeleMarketing/Index.cshtml");
        }
        public ActionResult Customer()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "tele_marketing")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            var customers = (from customer in ags.customer_profile_table orderby customer.id descending select customer).ToList();

            return PartialView("~/Views/TeleMarketing/TeleMarketing/Customer.cshtml");
        }
        public ActionResult Details(int Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "tele_marketing")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            var user = ags.customer_profile_table.Where(x => x.id == Id).FirstOrDefault();
            if (user == null)
            {
                return HttpNotFound();
            }
            return PartialView("~/Views/TeleMarketing/TeleMarketing/Details.cshtml");
        }
    }
}