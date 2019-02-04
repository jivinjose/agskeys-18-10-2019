using agskeys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace agskeys.Controllers
{
    [Authorize]
    public class userprofileController : Controller
    {
        agsfinancialsEntities ags = new agsfinancialsEntities();
        public ActionResult UserProfile()
        {
            if (Session["username"] == null && Session["userid"] == null)
            {
                RedirectToAction("Login");
            }
            else
            {
                var intId = Session["userid"].ToString();
                var Id = Convert.ToInt32(intId);
                var user = ags.admin_table.Where(x => x.id == Id).FirstOrDefault();
                return PartialView(user);
            }
            return View();

        }
    }
}