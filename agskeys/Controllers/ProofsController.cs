﻿using agskeys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace agskeys.Controllers
{
    [Authorize]
    public class ProofsController : Controller
    {
        agsfinancialsEntities ags = new agsfinancialsEntities();

        public ActionResult Index()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            var proofs = (from sub in ags.proof_table orderby sub.id descending select sub).ToList();

            return View(proofs);
        }
        public ActionResult Proof()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            var proofs = (from sub in ags.proof_table orderby sub.id descending select sub).ToList();

            return View(proofs);
        }
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            var model = new agskeys.Models.proof_table();//load data from database by RestaurantId
            return PartialView(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(proof_table obj)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            if (ModelState.IsValid)
            {
                var usr = (from u in ags.proof_table where u.proofname == obj.proofname select u).FirstOrDefault();

                if (usr == null)
                {
                    var max= ags.proof_table.GroupBy(x => x.porder).Select(g => g.OrderByDescending(x => x.porder).FirstOrDefault());
                    
                    ags.proof_table.Add(new proof_table
                    {
                        proofname = obj.proofname,
                        porder = obj.porder,
                        status = obj.status,
                        datex = DateTime.Now.ToString(),
                        addedby = Session["username"].ToString()
                    });
                    ags.SaveChanges();
                    return RedirectToAction("Proof");
                }
                else
                {
                    TempData["AE"] = "This Proof is already exist";
                    return View();
                  //  return Json(new { success = true, responseText = TempData["AE"] }, JsonRequestBehavior.AllowGet);

                }
            }
            return View(obj);
        }
        //public JsonResult UsernameExists(string proofname)
        //{
        //    //var model = new agskeys.Models.proof_table();
        //    var proof_table = ags.proof_table.ToList();
        //    var proofdata = "";
        //    foreach (var items in proof_table)
        //    {
        //        if (proofname == items.proofname)
        //        {
        //            proofdata = items.proofname;
        //            break;
        //        }
        //        else
        //        {
        //            proofdata = items.proofname;
        //            continue;
        //        }


        //    }
        //    return Json(!String.Equals(proofname, proofdata, StringComparison.OrdinalIgnoreCase));

        //}

       // [HttpPost]
       // public JsonResult UsernameExists(string proofname, int id)
       //{
       //     return Json(IsUnique(proofname, id));
       // }

       // private bool IsUnique(string proofname, int id)
       // {
       //     if (id == 0) // its a new object
       //     {
       //         return !ags.proof_table.Any(x => x.proofname == proofname);
       //     }
       //     else // its an existing object so exclude existing objects with the id
       //     {
       //         return !ags.proof_table.Any(x => x.proofname == proofname && x.id != id);
       //     }
       // }






        public ActionResult Edit(int? Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            proof_table proof_table = ags.proof_table.Find(Id);
            if (proof_table == null)
            {
                return HttpNotFound();
            }
            return PartialView(proof_table);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(proof_table proof)

        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            if (ModelState.IsValid)
            {

                proof_table existing = ags.proof_table.Find(proof.id);


                if (proof.status == "publish")
                {
                    existing.status = proof.status;
                }
                else
                {
                    existing.status = proof.status;
                }




                if (existing.proofname != proof.proofname)
                {
                    var userCount = (from u in ags.proof_table where u.proofname == proof.proofname select u).Count();
                    if (userCount == 0)
                    {
                        existing.proofname = proof.proofname;
                    }
                    else
                    {
                        TempData["AE"] = "This Proof is already exist";
                        return View();
                    }
                }


                if (existing.porder != proof.porder)
                {
                    var userCount = (from u in ags.proof_table where u.porder == proof.porder select u).Count();
                    if (userCount == 0)
                    {
                        existing.porder = proof.porder;
                    }
                    else
                    {
                        TempData["AE"] = "This Order Number is already exist";
                        return View();
                    }
                }



                if (existing.addedby == null)
                {
                    existing.addedby = Session["username"].ToString();
                }
                if (existing.datex == null)
                {
                    existing.datex = DateTime.Now.ToString();
                }

                ags.SaveChanges();
                return RedirectToAction("Proofs");
            }
            return PartialView(proof);

        }








        public ActionResult Delete(int? id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            proof_table proof_table = ags.proof_table.Find(id);
            if (proof_table == null)
            {
                return HttpNotFound();
            }
            return PartialView(proof_table);
        }



        // POST: vendor_table/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            proof_table proof_table = ags.proof_table.Find(id);


            ags.proof_table.Remove(proof_table);
            ags.SaveChanges();
            return RedirectToAction("Proof");
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