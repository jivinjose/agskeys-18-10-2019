using agskeys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace agskeys.Controllers.MobileManager
{
    public class MobileManagerController : Controller
    {
        agsfinancialsEntities ags = new agsfinancialsEntities();
        public ActionResult Index()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "manager")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            string username = Session["username"].ToString();
            //var customers = (from customer in ags.customer_profile_table orderby customer.id descending select customer).ToList();
            string userid = Session["userid"].ToString();
            var customers = (from s in ags.customer_profile_table
                             join sa in ags.loan_table on s.id.ToString() equals sa.customerid
                             join sb in ags.loan_track_table on sa.id.ToString() equals sb.loanid
                             where sb.employeeid == userid
                             orderby sb.datex descending
                             select s).Distinct().ToList();

            return PartialView("~/Views/MobileManager/MobileManager/Customer.cshtml", customers);
        }
        public ActionResult Customer()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "manager")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            string username = Session["username"].ToString();
            //var customers = (from customer in ags.customer_profile_table orderby customer.id descending select customer).ToList();
            string userid = Session["userid"].ToString();
            var customers = (from s in ags.customer_profile_table
                             join sa in ags.loan_table on s.id.ToString() equals sa.customerid
                             join sb in ags.loan_track_table on sa.id.ToString() equals sb.loanid
                             where sb.employeeid == userid
                             orderby sb.datex descending
                             select s).Distinct().ToList();

            return PartialView("~/Views/MobileManager/MobileManager/Customer.cshtml", customers);
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
            return PartialView("~/Views/MobileManager/MobileManager/Details.cshtml", user);
        }
        public ActionResult UserProfile()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "manager" || Session["userid"] == null)
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            else
            {
                var intId = Session["userid"].ToString();
                var Id = Convert.ToInt32(intId);
                var user = ags.admin_table.Where(x => x.id == Id).FirstOrDefault();
                return PartialView("~/Views/MobileManager/MobileManager/UserProfile.cshtml", user);
            }

        }
        public ActionResult Support()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "manager")
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            string username = Session["username"].ToString();
            string userid = Session["userid"].ToString();

            return View("~/Views/MobileManager/MobileManager/Support.cshtml");
        }
    }
}