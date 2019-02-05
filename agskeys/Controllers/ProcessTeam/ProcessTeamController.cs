using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace agskeys.Controllers.ProcessTeam
{
    [Authorize]
    public class ProcessTeamController : Controller
    {
        // GET: ProcessTeam
        public ActionResult Index()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "process_team")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            return View("~/Views/ProcessTeam/ProcessTeam/Index.cshtml");
        }
    }
}