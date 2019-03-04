﻿using agskeys.Models;
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
            string username = Session["username"].ToString();
            //var customers = (from customer in ags.customer_profile_table orderby customer.id descending select customer).ToList();
            string userid = Session["userid"].ToString();
            var customers = (from s in ags.customer_profile_table
                             join sa in ags.loan_table on s.id.ToString() equals sa.customerid
                             join sb in ags.loan_track_table on sa.id.ToString() equals sb.loanid
                             where sb.employeeid == userid
                             orderby sb.datex descending
                             select s).Distinct().ToList();

            return PartialView("~/Views/Manager/Manager/Customer.cshtml", customers);
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
            return PartialView("~/Views/Manager/Manager/Details.cshtml", user);
        }
    }
}