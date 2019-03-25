﻿using agskeys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static agskeys.Controllers.AccountController;

namespace agskeys.Controllers.Clientele
{
   [AuthorizeUser]
    public class ClienteleController : Controller
    {
        agsfinancialsEntities ags = new agsfinancialsEntities();
        // GET: Clientele
        public ActionResult Index()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "clientele")
            {
                return this.RedirectToAction("ClientLogout", "Account");
            }
            string username = Session["username"].ToString();
            string userid = Session["userid"].ToString();

            List<customer_profile_table> custprf = ags.customer_profile_table.Where(x => x.id.ToString() == userid).ToList();
            ViewBag.customer_profile_table = custprf;


            // var assigne_id = ags.assigned_table.Where(x => x.assign_emp_id == userid).ToList();
            //var getCustomer = ags.customer_profile_table.ToList();
            var customer_loans = (from s in ags.loan_table
                                  join sa in ags.loan_track_table on s.id.ToString() equals sa.loanid
                                  where s.customerid == userid
                                  orderby sa.datex descending
                                  select s).Distinct().ToList();
            var getbank = (from bank_table in ags.bank_table select bank_table).ToList();

            var bankid = "";
            foreach (var item in customer_loans)
            {
                foreach (var items in getbank)
                {
                    if (item.bankid.ToString() == items.id.ToString())
                    {
                        bankid = items.bankname;
                        break;
                    }
                    else if (items.id.ToString() != item.customerid)
                    {
                        bankid = "Not Updated";
                        continue;
                    }
                }
                item.bankid = bankid;

            }

            //var getVendor = ags.vendor_table.ToList();
            //var partnerid = "";
            //foreach (var item in customer_loans)
            //{
            //    foreach (var items in getVendor)
            //    {
            //        if (item.partnerid == items.id.ToString())
            //        {
            //            partnerid = items.companyname;
            //            break;
            //        }
            //        else if (items.id.ToString() != item.partnerid)
            //        {
            //            partnerid = "Not Updated";
            //            continue;
            //        }

            //    }
            //    item.partnerid = partnerid;
            //}

            //var getloantype = ags.loantype_table.ToList();
            //foreach (var item in customer_loans)
            //{
            //    foreach (var items in getloantype)
            //    {
            //        if (item.loantype == items.id.ToString())
            //        {
            //            item.loantype = items.loan_type;
            //            break;
            //        }
            //        else if (!ags.loan_table.Any(s => s.loantype.ToString() == items.id.ToString()))
            //        {
            //            item.loantype = "Not Updated";
            //        }
            //    }
            //}
            return View("~/Views/Clientele/Index.cshtml", customer_loans);
        }
        //{
        //    if (Session["username"] == null || Session["userlevel"].ToString() != "clientele")
        //    {
        //        return this.RedirectToAction("ClientLogout", "Account");
        //    }
        //    string username = Session["username"].ToString();
        //    string userid = Session["userid"].ToString();
        //    var customer_loans = (from loan_table in ags.loan_table where loan_table.customerid==userid orderby loan_table.id descending select loan_table).ToList();

        //    return View("~/Views/Clientele/Index.cshtml", customer_loans);
        //}


        [HttpGet]
        public ActionResult Track(int? Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "clientele")
            {
                return this.RedirectToAction("ClientLogout", "Account");
            }
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<loan_table> loan = ags.loan_table.Where(x => x.id == Id).ToList();
            List<loan_track_table> employeeLoantrack = ags.loan_track_table.Where(x => x.loanid == Id.ToString()).ToList();
            List<vendor_track_table> vendorLoantrack = ags.vendor_track_table.Where(x => x.loanid == Id.ToString()).ToList();
            List<external_comment_table> externalComment = ags.external_comment_table.ToList();

            var employee = ags.admin_table.ToList();
            loan_track loan_track = new loan_track();
            loan_track.loan_details = loan.ToList();
            loan_track.employee_track = employeeLoantrack.ToList().OrderBy(t => t.tracktime);
            loan_track.vendor_track = vendorLoantrack.ToList().OrderBy(t => t.tracktime);

            var user = ags.loan_table.Where(x => x.id == Id).FirstOrDefault();
            var getCustomer = ags.customer_profile_table.ToList();
            var customerid = "";
            var phonenumber = "";
            var name = "";
            var email = "";
            foreach (var customer in getCustomer)
            {
                if (user.customerid == customer.id.ToString())
                {
                    name = customer.name;
                    customerid = customer.customerid;
                    phonenumber = customer.phoneno;
                    email = customer.email;
                    break;
                }
                else if (user.customerid != customer.id.ToString())
                {
                    customerid = "Not Updated";
                    continue;
                }

            }
            user.customerid = customerid;
            ViewBag.name = name;
            ViewBag.phoneno = phonenumber;
            ViewBag.email = email;

            var employees = ags.admin_table.ToList();

            var employeeid = "";
            foreach (var item in employeeLoantrack)
            {
                foreach (var items in employees)
                {
                    if (item.employeeid != null)
                    {
                        if (item.employeeid.ToString() == items.id.ToString())
                        {
                            string concatenated = items.name + " ( " + items.userrole + " ) ";
                            employeeid = concatenated;
                            break;
                        }
                        else if (items.id.ToString() != item.employeeid)
                        {
                            employeeid = "Not Updated";
                            continue;
                        }
                    }

                }
                item.employeeid = employeeid;

            }


            var extComment = "";
            foreach (var item in employeeLoantrack)
            {
                foreach (var items in externalComment)
                {
                    if (item.externalcomment != null)
                    {
                        if (item.externalcomment.ToString() == items.id.ToString())
                        {
                            extComment = items.externalcomment;
                            break;
                        }
                        else if (items.id.ToString() != item.externalcomment)
                        {
                            extComment = "Not Updated";
                            continue;
                        }
                    }

                }
                item.externalcomment = extComment;

            }



            var vendors = ags.vendor_table.ToList();

            var vendorid = "";
            foreach (var item in vendorLoantrack)
            {
                foreach (var items in vendors)
                {
                    if (item.vendorid != null)
                    {
                        if (item.vendorid.ToString() == items.id.ToString())
                        {
                            string concatenated = items.companyname + " ( " + items.name + " ) ";
                            vendorid = concatenated;
                            break;
                        }
                        else if (items.id.ToString() != item.vendorid)
                        {
                            vendorid = "Not Updated";
                            continue;
                        }
                    }

                }
                item.vendorid = vendorid;

            }


            return PartialView("~/Views/Clientele/Track.cshtml", loan_track);
        }
        public ActionResult Details(int? Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "clientele")
            {
                return this.RedirectToAction("ClientLogout", "Account");
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

            if(user!= null)
            { 
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
            }
            else
            {
                user = null;
            }
            return PartialView("~/Views/Clientele/Details.cshtml", user);
        }

    }

}