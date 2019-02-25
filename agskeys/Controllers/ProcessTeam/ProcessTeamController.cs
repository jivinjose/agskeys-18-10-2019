﻿using agskeys.Models;
using PasswordSecurity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace agskeys.Controllers.ProcessTeam
{
    [Authorize]
    public class ProcessTeamController : Controller
    {
        // GET: Manager
        agsfinancialsEntities ags = new agsfinancialsEntities();
        public ActionResult Index()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "process_team")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            return View("~/Views/ProcessTeam/ProcessTeam/Index.cshtml");
        }
        public ActionResult Customer()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "process_team")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            var customers = (from customer in ags.customer_profile_table orderby customer.id descending select customer).ToList();

            return PartialView("~/Views/ProcessTeam/ProcessTeam/Customer.cshtml", customers);
        }
        public ActionResult Details(int Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "process_team")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            var user = ags.customer_profile_table.Where(x => x.id == Id).FirstOrDefault();
            if (user == null)
            {
                return HttpNotFound();
            }
            return PartialView("~/Views/ProcessTeam/ProcessTeam/Details.cshtml", user);
        }
        public ActionResult Edit(int? Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "process_team")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            customer_profile_table customer_profile_table = ags.customer_profile_table.Find(Id);
            if (customer_profile_table == null)
            {
                return HttpNotFound();
            }
            return PartialView("~/Views/ProcessTeam/ProcessTeam/Edit.cshtml", customer_profile_table);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(customer_profile_table customer_profile_table)
        {
            if (ModelState.IsValid)
            {
                var allowedExtensions = new[] {
                    ".Jpg", ".png", ".jpg", ".jpeg"
                };
                customer_profile_table existing = ags.customer_profile_table.Find(customer_profile_table.id);
                var password = existing.password.ToString();
                var newPassword = customer_profile_table.password.ToString();
                if (existing.profileimg == null)
                {
                    string BigfileName = Path.GetFileNameWithoutExtension(customer_profile_table.ImageFile.FileName);
                    string fileName = BigfileName.Substring(0, 1);
                    string extension1 = Path.GetExtension(customer_profile_table.ImageFile.FileName);
                    string extension = extension1.ToLower();
                    if (allowedExtensions.Contains(extension))
                    {
                        fileName = fileName + DateTime.Now.ToString("yyssmmfff") + extension;
                        customer_profile_table.profileimg = "~/customerImage/" + fileName;
                        fileName = Path.Combine(Server.MapPath("~/customerImage/"), fileName);
                        customer_profile_table.ImageFile.SaveAs(fileName);
                    }
                    else
                    {
                        TempData["Message"] = "Only 'Jpg', 'png','jpeg' images formats are alllowed..!";
                        return RedirectToAction("Customer");
                    }
                }


                else if (existing.profileimg != null && customer_profile_table.profileimg != null)
                {
                    if (customer_profile_table.ImageFile != null)
                    {
                        string path = Server.MapPath(existing.profileimg);
                        FileInfo file = new FileInfo(path);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                        string BigfileName = Path.GetFileNameWithoutExtension(customer_profile_table.ImageFile.FileName);
                        string fileName = BigfileName.Substring(0, 1);
                        string extension1 = Path.GetExtension(customer_profile_table.ImageFile.FileName);
                        string extension = extension1.ToLower();
                        if (allowedExtensions.Contains(extension))
                        {
                            fileName = fileName + DateTime.Now.ToString("yyssmmfff") + extension;
                            customer_profile_table.profileimg = "~/adminimage/" + fileName;
                            fileName = Path.Combine(Server.MapPath("~/adminimage/"), fileName);
                            customer_profile_table.ImageFile.SaveAs(fileName);
                        }
                        else
                        {
                            TempData["Message"] = "Only 'Jpg', 'png','jpeg' images formats are alllowed..!";
                            return RedirectToAction("Customer");
                        }

                    }
                    else
                    {
                        existing.profileimg = existing.profileimg;
                    }
                }
                else
                {
                    existing.profileimg = existing.profileimg;
                }
                existing.name = customer_profile_table.name;
                existing.email = customer_profile_table.email;
                existing.phoneno = customer_profile_table.phoneno;
                existing.alterphoneno = customer_profile_table.alterphoneno;
                existing.dob = customer_profile_table.dob;
                existing.address = customer_profile_table.address;
                if (existing.customerid != customer_profile_table.customerid)
                {
                    var userCount = (from u in ags.customer_profile_table where u.customerid == customer_profile_table.customerid select u).Count();
                    if (userCount == 0)
                    {
                        existing.customerid = customer_profile_table.customerid;
                    }
                    else
                    {
                        //existing.username = customer_profile_table.username;
                        TempData["AE"] = "This user name is already exist";
                        //return PartialView("Edit", "SuperAdmin");
                        return RedirectToAction("Customer");
                    }
                }
                else
                {
                    existing.customerid = existing.customerid;
                }

                existing.profileimg = customer_profile_table.profileimg;

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
                if (password.Equals(newPassword))
                {
                    existing.password = customer_profile_table.password;
                }
                else
                {
                    existing.password = PasswordStorage.CreateHash(customer_profile_table.password);
                }
                ags.SaveChanges();
                return RedirectToAction("Customer");
            }
            return PartialView("~/Views/ProcessTeam/ProcessTeam/Edit.cshtml", customer_profile_table);
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