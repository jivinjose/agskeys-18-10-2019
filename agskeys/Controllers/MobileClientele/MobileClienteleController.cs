using agskeys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static agskeys.Controllers.AccountController;

namespace agskeys.Controllers.MobileClientele
{
    [AuthorizeMobileUser]
    public class MobileClienteleController : Controller
    {
        agsfinancialsEntities ags = new agsfinancialsEntities();
        // GET: MobileClientele
        public ActionResult Index()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "clientele")
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            string username = Session["username"].ToString();
            string userid = Session["userid"].ToString();
            List<customer_profile_table> custprf = ags.customer_profile_table.Where(x => x.id.ToString() == userid).ToList();
            ViewBag.customer_profile_table = custprf;
            
            var customer_loans = (from s in ags.loan_table
                                  join sa in ags.loan_track_table on s.id.ToString() equals sa.loanid
                                  where s.customerid == userid
                                  orderby sa.datex descending
                                  select s).Distinct().ToList();
            ViewData["loancount"] = customer_loans.Count();

            //if (!string.IsNullOrEmpty(customer_loans.Sum(t=>t.requestloanamt)))
            //{
            //    
            //}
            //else
            //{
            //    var requestamnt = 0;
            //}

            var requestamnt = customer_loans.Sum(t => Convert.ToDecimal(!string.IsNullOrEmpty(t.requestloanamt)));
            var loanamnt = customer_loans.Sum(t => Convert.ToDecimal(!string.IsNullOrEmpty(t.loanamt)));
            var disbursementamnt = customer_loans.Sum(t => Convert.ToDecimal(!string.IsNullOrEmpty(t.disbursementamt)));
            var balance = customer_loans.Sum(s => (Convert.ToDecimal(!string.IsNullOrEmpty((s.loanamt))) - (Convert.ToDecimal(!string.IsNullOrEmpty(s.disbursementamt)))));
            var interest = customer_loans.Sum(t => Convert.ToDecimal(!string.IsNullOrEmpty(t.rateofinterest)));

           
            ViewData["requestamnt"] = requestamnt;
            ViewData["loanamnt"] = loanamnt;
            ViewData["disbursementamnt"] = disbursementamnt;
            ViewData["balance"] = balance;

            if(requestamnt != 0)
            {
                var sanction_percentage = (loanamnt * 100) / requestamnt;
                decimal sanction_percentages = Math.Round(sanction_percentage, 2);
                ViewData["sanction_percentage"] = sanction_percentages;
            }
            else
            {
                ViewData["sanction_percentage"] = 0;

            }
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
            
            if(customer_loans.Count() != 0)
            {
                ViewData["interest"] = interest / customer_loans.Count();
            }
            else
            {
                ViewData["interest"] = 0;
            }

            
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
            return View("~/Views/MobileClientele/Index.cshtml");
        }


        public ActionResult Loan()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "clientele")
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            string username = Session["username"].ToString();
            string userid = Session["userid"].ToString();

            List<customer_profile_table> custprf = ags.customer_profile_table.Where(x => x.id.ToString() == userid).ToList();
            ViewBag.customer_profile_table = custprf;

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
            return View("~/Views/MobileClientele/Loan.cshtml",customer_loans);
        }
        public ActionResult profile()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "clientele")
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            string username = Session["username"].ToString();
            string userid = Session["userid"].ToString();
            var Id = Convert.ToInt32(userid);
            var user = ags.customer_profile_table.Where(x => x.id == Id).FirstOrDefault();
            if (user == null)
            {
                return HttpNotFound();
            }
            return View("~/Views/MobileClientele/profile.cshtml",user);
        }

        public ActionResult Enquiry()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "clientele")
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            string username = Session["username"].ToString();
            string userid = Session["userid"].ToString();
            var Id = Convert.ToInt32(userid);
            var user = ags.customer_profile_table.Where(x => x.id == Id).FirstOrDefault();
            if (user == null)
            {
                return HttpNotFound();
            }
            return View("~/Views/MobileClientele/Enquiry.cshtml", user);
        }
       
        public ActionResult RequestLoan()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "clientele")
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            string username = Session["username"].ToString();
            string userid = Session["userid"].ToString();
            var Id = Convert.ToInt32(userid);
            var user = ags.customer_profile_table.Where(x => x.id == Id).FirstOrDefault();
            if (user == null)
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            var getloantype = ags.loantype_table.ToList();
            SelectList loantp = new SelectList(getloantype, "id", "loan_type");
            ViewBag.loantypeList = loantp;
            var model = new agskeys.Models.RequestLoan();
            model.name = user.name;
            model.phoneno = user.phoneno;
            model.email = user.email;

            return View("~/Views/MobileClientele/RequestLoan.cshtml", model);
        }
        public ActionResult Refer()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "clientele")
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            string username = Session["username"].ToString();
            string userid = Session["userid"].ToString();

            return View("~/Views/MobileClientele/Refer.cshtml");
        }
        public ActionResult Support()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "clientele")
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            string username = Session["username"].ToString();
            string userid = Session["userid"].ToString();

            return View("~/Views/MobileClientele/Support.cshtml");
        }
        public ActionResult Details(int? Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "clientele")
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

            return PartialView(user);
        }
        [HttpGet]
        public ActionResult Track(int? Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "clientele")
            {
                return this.RedirectToAction("MobileLogout", "Account");
            }
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<loan_table> loan = ags.loan_table.Where(x => x.id == Id).ToList();
            List<loan_track_table> employeeLoantrack = ags.loan_track_table.Where(x => x.loanid == Id.ToString()).ToList();
           

            var employee = ags.admin_table.ToList();
            loan_track loan_track = new loan_track();
            loan_track.loan_details = loan.ToList();
            loan_track.employee_track = employeeLoantrack.ToList().OrderBy(t => t.tracktime);            

            var user = ags.loan_table.Where(x => x.id == Id).FirstOrDefault();
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
            
            return PartialView(loan_track);
        }
    }
}