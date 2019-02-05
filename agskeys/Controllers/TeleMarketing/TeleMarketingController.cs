using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace agskeys.Controllers.TeleMarketing
{
    public class TeleMarketingController : Controller
    {
        // GET: TeleMarketing
        public ActionResult Index()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "tele_marketing")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            return View("~/Views/TeleMarketing/TeleMarketing/Index.cshtml");
        }
    }
}