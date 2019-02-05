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
    }
}