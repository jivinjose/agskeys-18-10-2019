﻿using agskeys.Models;
using PasswordSecurity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static agskeys.Controllers.AccountController;

namespace agskeys.Controllers.MobileVendor
{
    [AuthorizeMobileUser]
    public class MobileVendorController : Controller
    {
        agsfinancialsEntities ags = new agsfinancialsEntities();
        public ActionResult Index()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "partner")
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            string username = Session["username"].ToString();
            string userid = Session["userid"].ToString();
            var customer_loans = (from loan_table in ags.loan_table where loan_table.partnerid == userid orderby loan_table.id descending select loan_table).ToList();


            var loanamnt = customer_loans.Sum(t => Convert.ToDecimal(t.loanamt));
            var disbursementamnt = customer_loans.Sum(t => Convert.ToDecimal(t.disbursementamt));
            var balance = customer_loans.Sum(s => (Convert.ToDecimal(s.loanamt)) - (Convert.ToDecimal(s.disbursementamt)));

            ViewData["loancount"] = customer_loans.Count();
          
            ViewData["loanamnt"] = loanamnt;
            ViewData["disbursementamnt"] = disbursementamnt;
            ViewData["balance"] = balance;

            if (loanamnt != 0)
            {
                var disbursement_percentage = (disbursementamnt * 100) / loanamnt;
                decimal disbursement_percentages = Math.Round(disbursement_percentage, 2);
                ViewData["disbursement_percentage"] = disbursement_percentages;

                var balance_percentage = (balance * 100) / loanamnt;
                decimal balance_percentages = Math.Round(balance_percentage, 2);
                ViewData["balance_percentage"] = balance_percentages;
            }
            else
            {
                ViewData["disbursement_percentage"] = 0;
                ViewData["balance_percentage"] = 0;
            } 

            var getCustomer = ags.customer_profile_table.ToList();
            SelectList customers = new SelectList(getCustomer, "id", "customerid");
            ViewBag.customerList = customers;

            var customerid = "";
            foreach (var item in customer_loans)
            {
                foreach (var items in getCustomer)
                {
                    if (item.customerid.ToString() == items.id.ToString())
                    {
                        string concatenated = items.name.ToString() + " ( " + items.customerid + " ) ";
                        customerid = concatenated;
                        break;
                    }
                    else if (items.id.ToString() != item.customerid)
                    {
                        customerid = "Not Updated";
                        continue;
                    }
                }
                item.employeetype = customerid;

            }


            return View("~/Views/MobileVendor/Index.cshtml");
        }
        public ActionResult Loan()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "partner")
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            string username = Session["username"].ToString();
            string userid = Session["userid"].ToString();
            var customer_loans = (from loan_table in ags.loan_table where loan_table.partnerid == userid orderby loan_table.id descending select loan_table).ToList();

            var getCustomer = ags.customer_profile_table.ToList();
            SelectList customers = new SelectList(getCustomer, "id", "customerid");
            ViewBag.customerList = customers;

            var customerid = "";
            foreach (var item in customer_loans)
            {
                foreach (var items in getCustomer)
                {
                    if (item.customerid.ToString() == items.id.ToString())
                    {
                        string concatenated = items.name.ToString() + " ( " + items.customerid + " ) ";
                        customerid = concatenated;
                        break;
                    }
                    else if (items.id.ToString() != item.customerid)
                    {
                        customerid = "Not Updated";
                        continue;
                    }
                }
                item.employeetype = customerid;

            }
            var getloantype = ags.loantype_table.ToList();
            foreach (var item in customer_loans)
            {
                foreach (var items in getloantype)
                {
                    if (item.loantype == items.id.ToString())
                    {
                        item.loantype = items.loan_type;
                        break;
                    }
                    else if (!ags.loan_table.Any(s => s.loantype.ToString() == items.id.ToString()))
                    {
                        item.loantype = "Not Updated";
                    }
                }
            }
            return View("~/Views/MobileVendor/Loan.cshtml", customer_loans);
        }
        public ActionResult Profiles()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "partner")
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            string username = Session["username"].ToString();
            string userid = Session["userid"].ToString();
            var Id = Convert.ToInt32(userid);
            var user = ags.vendor_table.Where(x => x.id == Id).FirstOrDefault();
            if (user == null)
            {
                return HttpNotFound();
            }
            return View("~/Views/MobileVendor/Profiles.cshtml", user);
        }
        public ActionResult Details(int? Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "partner")
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = ags.loan_table.Where(x => x.id == Id).FirstOrDefault();
            //var getCustomerProfile = ags.customer_profile_table.Where(x=>x.id.ToString() == user.customerid.ToString()).ToList();           
            //SelectList customers = new SelectList(getCustomerProfile, "id", "customerid", "name", "phoneno", "profileimg");
            //ViewBag.customerList = customers;

            var getCustomer = ags.customer_profile_table.ToList();

            string id = "";
            string name = "";
            string phone = "";
            string email = "";
            string profilimg = "";

            foreach (var customer in getCustomer)
            {
                if (user.customerid == customer.id.ToString())
                {
                    id = customer.customerid.ToString();
                    name = customer.name;
                    phone = customer.phoneno;
                    email = customer.email;
                    profilimg = customer.profileimg;
                    break;
                }
                else if (user.customerid != customer.id.ToString())
                {
                    id = "Not Updated";
                    name = "Not Updated";
                    phone = "Not Updated";
                    email = "Not Updated";
                    profilimg = "Not Updated";
                }
            }
            user.customerid = id;
            ViewBag.name = name;
            ViewBag.phoneno = phone;
            ViewBag.email = email;
            ViewBag.profileimg = profilimg;


            var getVendor = ags.vendor_table.ToList();
            string partner = "";
            foreach (var items in getVendor)
            {
                if (user.partnerid == items.id.ToString())
                {
                    string concatenated = items.companyname + " ( Company Name ) ";
                    partner = concatenated;
                    break;
                }
                else if (user.partnerid != items.id.ToString())
                {
                    partner = "Not Updated" + " ( Company Name ) ";
                }
            }
            user.partnerid = partner;

            var getBank = ags.bank_table.ToList();
            string banknm = "";
            foreach (var bank in getBank)
            {
                if (user.bankid == bank.id.ToString())
                {
                    banknm = bank.bankname;
                    break;
                }
                else if (user.bankid != bank.id.ToString())
                {
                    banknm = "Not Updated";
                }
            }
            user.bankid = banknm;

            var getloantype = ags.loantype_table.ToList();
            string loan = "";
            foreach (var loantp in getloantype)
            {
                if (user.loantype == loantp.id.ToString())
                {
                    loan = loantp.loan_type;
                    break;
                }
                else if (user.loantype != loantp.id.ToString())
                {
                    loan = "Not Updated";
                }
            }
            user.loantype = loan;

            return PartialView("~/Views/MobileVendor/Details.cshtml", user);
        }
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "partner")
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            var getloantype = ags.loantype_table.ToList();
            SelectList loantp = new SelectList(getloantype, "id", "loan_type");
            ViewBag.loantypeList = loantp;
            var model = new agskeys.Models.partner_customer();
            return PartialView("~/Views/MobileVendor/Create.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(partner_customer obj)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "partner")
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            var getloantype = ags.loantype_table.ToList();
            SelectList loantp = new SelectList(getloantype, "id", "loan_type");
            ViewBag.loantypeList = loantp;
            if (ModelState.IsValid)
            {
                string vendorName = Session["username"].ToString();
                // var customer = (from u in ags.customer_profile_table where u.customerid == obj.customerid select u).FirstOrDefault();
                var vendor = (from u in ags.vendor_table where u.username == vendorName select u).FirstOrDefault();


                if (vendor != null)
                {
                    customer_profile_table customerprofile = new customer_profile_table();
                    customerprofile.name = obj.name;
                    customerprofile.email = obj.email;
                    customerprofile.phoneno = obj.phoneno;
                    customerprofile.datex = DateTime.Now.ToString();
                    customerprofile.addedby = Session["username"].ToString();
                    ags.customer_profile_table.Add(customerprofile);
                    ags.SaveChanges();

                    int latestcustomerid = customerprofile.id;

                    customer_profile_table existing_Customer_Profile = ags.customer_profile_table.Find(customerprofile.id);
                    existing_Customer_Profile.customerid = latestcustomerid.ToString();
                    existing_Customer_Profile.password = PasswordStorage.CreateHash(existing_Customer_Profile.customerid);
                    ags.SaveChanges();

                    loan_table loan = new loan_table();
                    loan.customerid = latestcustomerid.ToString();
                    loan.partnerid = vendor.id.ToString();
                    loan.loantype = obj.loantype;
                    loan.requestloanamt = obj.requestloanamt;
                    loan.disbursementamt = "0";
                    loan.loanamt = "0";
                    loan.rateofinterest = "0";
                    loan.loanstatus = "Pending";
                    loan.datex = DateTime.Now.ToString();
                    loan.addedby = Session["username"].ToString();
                    ags.loan_table.Add(loan);
                    ags.SaveChanges();


                    //////////////////////////////////////
                    var superadminid = (from u in ags.admin_table where u.userrole == "super_admin" select u).FirstOrDefault();
                    string superemployeeid = superadminid.id.ToString();

                    int latestloanid = loan.id;

                    loan_track_table loan_track = new loan_track_table();
                    loan_track.loanid = latestloanid.ToString();
                    if (superemployeeid != null)
                    {
                        loan_track.employeeid = superemployeeid;
                        loan_track.tracktime = DateTime.Now.ToString();
                    }
                    if (obj.internalcomment != null)
                    {
                        loan_track.internalcomment = obj.internalcomment;
                        loan_track.externalcomment = "Not Updated";
                    }
                    loan_track.datex = DateTime.Now.ToString();
                    loan_track.addedby = Session["username"].ToString();
                    ags.loan_track_table.Add(loan_track);
                    ags.SaveChanges();


                    ///Assigned Employee

                    loan_track_table loan_track_employee = new loan_track_table();
                    if (Session["userid"] != null)
                    {
                        loan_track_employee.loanid = latestloanid.ToString();
                        loan_track_employee.employeeid = superemployeeid;
                        loan_track_employee.tracktime = DateTime.Now.ToString();
                        loan_track_employee.internalcomment = "Vendor Assigned";
                        loan_track_employee.externalcomment = "Vendor Assigned";

                        loan_track_employee.datex = DateTime.Now.ToString();
                        loan_track_employee.addedby = Session["username"].ToString();
                        ags.loan_track_table.Add(loan_track_employee);
                        ags.SaveChanges();
                    }


                    vendor_track_table vendor_track = new vendor_track_table();
                    if (Session["userid"] != null)
                    {
                        vendor_track.loanid = latestloanid.ToString();
                        vendor_track.vendorid = Session["userid"].ToString();
                        vendor_track.tracktime = DateTime.Now.ToString();
                        vendor_track.comment = "Assigned to Super Admin";
                        vendor_track.datex = DateTime.Now.ToString();
                        vendor_track.addedby = Session["username"].ToString();
                        ags.vendor_track_table.Add(vendor_track);
                        ags.SaveChanges();

                    }


                    //assigned table

                    assigned_table assigned = new assigned_table();
                    assigned.loanid = latestloanid.ToString();
                    if (superemployeeid != null)
                    {
                        assigned.assign_emp_id = superemployeeid;
                    }
                    if (Session["userid"] != null)
                    {
                        assigned.assign_vendor_id = Session["userid"].ToString();
                    }
                    assigned.datex = DateTime.Now.ToString();
                    assigned.addedby = Session["username"].ToString();
                    ags.assigned_table.Add(assigned);
                    ags.SaveChanges();

                    ////////////////////////////////////

                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["AE"] = "This customer user name is already exist";
                    return RedirectToAction("Index");
                }
            }
            return View("~/Views/MobileVendor/Create.cshtml", obj);
        }
        public ActionResult Refer()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "partner")
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            string username = Session["username"].ToString();
            string userid = Session["userid"].ToString();

            return View("~/Views/MobileVendor/Refer.cshtml");
        }
        public ActionResult Support()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "partner")
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            string username = Session["username"].ToString();
            string userid = Session["userid"].ToString();

            return View("~/Views/MobileVendor/Support.cshtml");
        }
        public ActionResult Enquiry()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "partner")
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            string username = Session["username"].ToString();
            string userid = Session["userid"].ToString();
            var Id = Convert.ToInt32(userid);
            var user = ags.vendor_table.Where(x => x.id == Id).FirstOrDefault();
            if (user == null)
            {
                return HttpNotFound();
            }
            return View("~/Views/MobileVendor/Enquiry.cshtml", user);
        }

        public ActionResult EditProfile(int? Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "partner")
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            vendor_table vendor_table = ags.vendor_table.Find(Id);

            if (vendor_table == null)
            {
                return HttpNotFound();
            }
            return PartialView(vendor_table);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(vendor_table vendor_table, FormCollection form)
        {
            if (ModelState.IsValid)
            {

                vendor_table existing = ags.vendor_table.Find(vendor_table.id);
                var password = existing.password.ToString();
                var newPassword = vendor_table.password.ToString();



                existing.name = vendor_table.name;
                existing.email = vendor_table.email;
                existing.phoneno = vendor_table.phoneno;
                existing.companyname = vendor_table.companyname;
                existing.address = vendor_table.address;


                if (existing.username != vendor_table.username)
                {
                    var userCount = (from u in ags.vendor_table where u.username == vendor_table.username select u).Count();
                    if (userCount == 0)
                    {
                        existing.username = vendor_table.username;
                    }
                    else
                    {
                        //existing.username = admin_table.username;
                        TempData["AE"] = "This user name is already exist";
                        //return PartialView("Edit", "SuperAdmin");
                        return RedirectToAction("Index", "MobileVendor");
                    }
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
                    existing.password = vendor_table.password;
                }
                else
                {
                    existing.password = PasswordStorage.CreateHash(vendor_table.password);
                }

                ags.SaveChanges();
                return RedirectToAction("Index", "MobileVendor");
            }
            return RedirectToAction("Index", "MobileVendor");
        }

        public ActionResult Password()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "partner")
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            var model = new agskeys.Models.ChangePassword();
            return PartialView(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Password(ChangePassword changePwd)
        {
            if (ModelState.IsValid)
            {
                string userid = Session["userid"].ToString();
                if (userid != null)
                {
                    vendor_table existing = ags.vendor_table.Where(x => x.id.ToString() == userid).FirstOrDefault();

                    var password = existing.password.ToString();
                    var oldPassword = changePwd.password;
                    var newPassword = changePwd.newpassword;
                    var retypePassword = changePwd.retypepassword;
                    bool result = PasswordStorage.VerifyPassword(oldPassword, password);
                    if (result)
                    {
                        if (newPassword == retypePassword)
                        {
                            existing.password = PasswordStorage.CreateHash(newPassword);
                        }
                        else
                        {
                            TempData["NotEqual"] = "<script>alert('password dosen't match');</script>";
                            return RedirectToAction("Index", "MobileVendor");
                        }
                    }
                    else
                    {
                        TempData["logAgain"] = "Oops.! Please Provide Valid Credentials.";
                        return RedirectToAction("MobileLogout", "Account");
                    }
                    ags.SaveChanges();



                    return RedirectToAction("Index", "MobileVendor");


                }
                else
                {
                    TempData["logAgain"] = "Oops.! Something Went Wrong.";
                    return RedirectToAction("MobileLogout", "Account");
                }
            }
            return RedirectToAction("Index", "MobileVendor");
        }

    }
}
