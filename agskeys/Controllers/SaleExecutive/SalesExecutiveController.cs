using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace agskeys.Controllers.SaleExecutive
{
    [Authorize]
    public class SalesExecutiveController : Controller
    {
        // GET: SalesExecutive
        public ActionResult Index()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "sales_executive")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            return View("~/Views/SalesExecutive/SalesExecutive/Index.cshtml");
        }
    }
}