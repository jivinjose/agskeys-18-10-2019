using agskeys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace agskeys.Controllers
{
    [Authorize]
    public class BankController : Controller
    {
        agsfinancialsEntities ags = new agsfinancialsEntities();
        public ActionResult Bank()
        {
            if (Session["username"] == null)
            {
                RedirectToAction("Login");
            }
            var banks = (from bank in ags.bank_table orderby bank.id descending select bank).ToList();

            return PartialView(banks);
        }
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["username"] == null)
            {
                RedirectToAction("Login");
            }
            var model = new agskeys.Models.bank_table();
            return PartialView(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(bank_table obj)
        {
            if (Session["username"] == null)
            {
                RedirectToAction("Login");
            }
            if (ModelState.IsValid)
            {
                var vendor = (from u in ags.bank_table where u.bankname == obj.bankname select u).FirstOrDefault();

                if (vendor == null)
                {
                    ags.bank_table.Add(new bank_table
                    {
                        bankname = obj.bankname,
                        datex = DateTime.Now.ToString(),
                        addedby = Session["username"].ToString()
                    });
                    ags.SaveChanges();
                    return RedirectToAction("Bank", "Bank");
                }
                else
                {
                    TempData["AE"] = "This bank name is already exist";
                }
            }
            return View(obj);
        }
        public ActionResult Edit(int? Id)
        {
            if (Session["username"] == null)
            {
                RedirectToAction("Login");
            }
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bank_table bank_table = ags.bank_table.Find(Id);
            if (bank_table == null)
            {
                return HttpNotFound();
            }
            return PartialView(bank_table);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(bank_table bank_table)
        {
            if (ModelState.IsValid)
            {
                bank_table existing = ags.bank_table.Find(bank_table.id);
                if (existing.bankname != bank_table.bankname)
                {
                    var count = (from u in ags.bank_table where u.bankname == bank_table.bankname select u).Count();
                    if (count == 0)
                    {
                        existing.bankname = bank_table.bankname;
                    }
                    else
                    {
                        TempData["AE"] = "This bank name is already exist";
                        return RedirectToAction("Edit", "Bank");
                    }
                }

                if (existing.addedby == null)
                {
                    existing.addedby = Session["username"].ToString();
                }
                else
                {
                    existing.addedby = existing.addedby;
                }
                if (existing.datex == null)
                {
                    existing.datex = DateTime.Now.ToString();
                }
                else
                {
                    existing.datex = existing.datex;
                }
                ags.SaveChanges();
                return RedirectToAction("Bank", "Bank");
            }
            return PartialView(bank_table);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bank_table bank_table = ags.bank_table.Find(id);
            if (bank_table == null)
            {
                return HttpNotFound();
            }
            return PartialView(bank_table);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            bank_table bank_table = ags.bank_table.Find(id);
            ags.bank_table.Remove(bank_table);
            ags.SaveChanges();
            return RedirectToAction("Bank", "Bank");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ags.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}