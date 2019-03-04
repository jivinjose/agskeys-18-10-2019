using agskeys.Models;
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
    public class ProcessLoanController : Controller
    {
        agsfinancialsEntities ags = new agsfinancialsEntities();
        public ActionResult processloan()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "process_team")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            string username = Session["username"].ToString();
            string userid = Session["userid"].ToString();
            List<assigned_table> assign = ags.assigned_table.Where(x => x.assign_emp_id == userid).ToList();
            ViewBag.assigned_loan = assign;


            // var assigne_id = ags.assigned_table.Where(x => x.assign_emp_id == userid).ToList();
            var getCustomer = ags.customer_profile_table.ToList();
            var customer_loans = (from s in ags.loan_table
                                  join sa in ags.loan_track_table on s.id.ToString() equals sa.loanid
                                  where sa.employeeid == userid
                                  orderby sa.datex descending
                                  select s).Distinct().ToList();
            // var customer_loans = (from loan_table in ags.loan_table orderby loan_table.id descending select loan_table).ToList();

            var customerid = "";
            foreach (var item in customer_loans)
            {
                foreach (var items in getCustomer)
                {
                    if (item.customerid.ToString() == items.id.ToString())
                    {
                        customerid = items.customerid;
                        break;
                    }
                    else if (items.id.ToString() != item.customerid)
                    {
                        customerid = "Not Updated";
                        continue;
                    }
                }
                item.customerid = customerid;

            }

            var getVendor = ags.vendor_table.ToList();
            var partnerid = "";
            foreach (var item in customer_loans)
            {
                foreach (var items in getVendor)
                {
                    if (item.partnerid == items.id.ToString())
                    {
                        partnerid = items.companyname;
                        break;
                    }
                    else if (items.id.ToString() != item.partnerid)
                    {
                        partnerid = "Not Updated";
                        continue;
                    }

                }
                item.partnerid = partnerid;
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
            return View("~/Views/ProcessTeam/ProcessLoan/processloan.cshtml", customer_loans);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult processloan(FormCollection form)
        {
                if (Session["username"] == null || Session["userlevel"].ToString() != "process_team")
                {
                    return this.RedirectToAction("Logout", "Account");
                }
                string username = Session["username"].ToString();
                string userid = Session["userid"].ToString();
                List<assigned_table> assign = ags.assigned_table.Where(x => x.assign_emp_id == userid).ToList();
                ViewBag.assigned_loan = assign;


                // var assigne_id = ags.assigned_table.Where(x => x.assign_emp_id == userid).ToList();
                var getCustomer = ags.customer_profile_table.ToList();
                string SearchFor = "";
                var customer_loans = (from s in ags.loan_table
                                      join sa in ags.loan_track_table on s.id.ToString() equals sa.loanid
                                      where sa.employeeid == userid
                                      orderby sa.datex descending 
                                      select s);
                if (form != null)
                {
                    if (form["ongoing"] != null)
                    {
                        SearchFor = form["ongoing"].ToString();
                    }
                    else if (form["Partialydisbursed"] != null)
                    {
                        SearchFor = form["Partialydisbursed"].ToString();
                    }
                    else if (form["fullydisbursed"] != null)
                    {
                        SearchFor = form["fullydisbursed"].ToString();
                    }

                    if (SearchFor == "ongoing")
                    {
                        customer_loans = (from s in ags.loan_table
                                              join sa in ags.loan_track_table on s.id.ToString() equals sa.loanid
                                              where (s.disbursementamt == "0" && sa.employeeid == userid)
                                              orderby sa.datex descending
                                              select s);

                    }
                    else if (SearchFor == "Partialydisbursed")
                    {
                        customer_loans = (from s in ags.loan_table
                                              join sa in ags.loan_track_table on s.id.ToString() equals sa.loanid
                                              where (s.disbursementamt != s.loanamt && s.disbursementamt != "0" && sa.employeeid == userid)
                                              orderby sa.datex descending
                                              select s);
                        

                    }
                    else if (SearchFor == "fullydisbursed")
                    {
                        customer_loans = (from s in ags.loan_table
                                              join sa in ags.loan_track_table on s.id.ToString() equals sa.loanid
                                              where (s.disbursementamt == s.loanamt && sa.employeeid == userid)
                                              orderby sa.datex descending
                                              select s);
                    }

                }
                var customerid = "";
                foreach (var item in customer_loans)
                {
                    foreach (var items in getCustomer)
                    {
                        if (item.customerid.ToString() == items.id.ToString())
                        {
                            customerid = items.customerid;
                            break;
                        }
                        else if (items.id.ToString() != item.customerid)
                        {
                            customerid = "Not Updated";
                            continue;
                        }
                    }
                    item.customerid = customerid;

                }

                var getVendor = ags.vendor_table.ToList();
                var partnerid = "";
                foreach (var item in customer_loans)
                {
                    foreach (var items in getVendor)
                    {
                        if (item.partnerid == items.id.ToString())
                        {
                            partnerid = items.companyname;
                            break;
                        }
                        else if (items.id.ToString() != item.partnerid)
                        {
                            partnerid = "Not Updated";
                            continue;
                        }

                    }
                    item.partnerid = partnerid;
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
                return View("~/Views/ProcessTeam/ProcessLoan/processloan.cshtml", customer_loans);
                     

        }

        public JsonResult GetEmployeeList(string categoryId)
        {
            var username = Session["username"].ToString();
            ags.Configuration.ProxyCreationEnabled = false;
            List<admin_table> employees = ags.admin_table.Where(x => x.userrole.ToString() == categoryId && x.username != username).ToList();
            return Json(employees, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Details(int? Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "process_team")
            {
                return this.RedirectToAction("Logout", "Account");
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

            return PartialView("~/Views/ProcessTeam/ProcessLoan/Details.cshtml", user);
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
            var getCustomer = ags.customer_profile_table.ToList();
            SelectList customers = new SelectList(getCustomer, "id", "customerid");
            ViewBag.customerList = customers;

            var getVendor = ags.vendor_table.ToList();
            SelectList vendors = new SelectList(getVendor, "id", "companyname");
            ViewBag.vendorList = vendors;

            var getBank = ags.bank_table.ToList();
            SelectList banks = new SelectList(getBank, "id", "bankname");
            ViewBag.bankList = banks;

            var getloantype = ags.loantype_table.ToList();
            SelectList loantp = new SelectList(getloantype, "id", "loan_type");
            ViewBag.loantypeList = loantp;

            var empCategory = ags.emp_category_table.Where(x => x.emp_category_id != "admin" && x.emp_category_id != "super_admin").ToList();
            SelectList empCategories = new SelectList(empCategory, "emp_category_id", "emp_category");
            ViewBag.empCategories = empCategories;

            var username = Session["username"].ToString();
            var employee = ags.admin_table.Where(x=>x.username != username).ToList();
            SelectList employees = new SelectList(employee, "id", "name");
            ViewBag.employees = employees;

            var ExtComment = ags.external_comment_table.ToList();
            SelectList commentlist = new SelectList(ExtComment, "id", "externalcomment");
            ViewBag.commentList = commentlist;

            //List<emp_category_table> categoryList = ags.emp_category_table.ToList();
            //ViewBag.empCategories = new SelectList(categoryList, "emp_category_id", "emp_category");


            loan_table loan_table = ags.loan_table.Find(Id);
            if (loan_table == null)
            {
                return HttpNotFound();
            }
            return PartialView("~/Views/ProcessTeam/ProcessLoan/Edit.cshtml", loan_table);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(loan_table loan_table)
        {
            if (ModelState.IsValid)
            {
                var getCustomer = ags.customer_profile_table.ToList();
                SelectList customers = new SelectList(getCustomer, "id", "customerid");
                ViewBag.customerList = customers;

                var getVendor = ags.vendor_table.ToList();
                SelectList vendors = new SelectList(getVendor, "id", "companyname");
                ViewBag.vendorList = vendors;

                var getBank = ags.bank_table.ToList();
                SelectList banks = new SelectList(getBank, "id", "bankname");
                ViewBag.bankList = banks;

                var getloantype = ags.loantype_table.ToList();
                SelectList loantp = new SelectList(getloantype, "id", "loan_type");
                ViewBag.loantypeList = loantp;

                var ExtComment = ags.external_comment_table.ToList();
                SelectList commentlist = new SelectList(ExtComment, "id", "externalcomment");
                ViewBag.commentList = commentlist;

                var empCategory = ags.emp_category_table.Where(x => x.emp_category_id != "admin" && x.emp_category_id != "super_admin").ToList();
                SelectList empCategories = new SelectList(empCategory, "emp_category_id", "emp_category");
                ViewBag.empCategories = empCategories;

                var username = Session["username"].ToString();
                var employee = ags.admin_table.Where(x => x.username != username).ToList();
                SelectList employees = new SelectList(employee, "id", "name");
                ViewBag.employees = employees;


                var allowedExtensions = new[] {
                    ".png", ".jpg", ".jpeg",".doc",".docx",".pdf"
                };
                loan_table existing = ags.loan_table.Find(loan_table.id);
                string partner = existing.partnerid;
                if (existing.sactionedcopy == null)
                {
                    string BigfileName = Path.GetFileNameWithoutExtension(loan_table.sactionedCopyFile.FileName);
                    string fileName = BigfileName.Substring(0, 1);
                    string extension2 = Path.GetExtension(loan_table.sactionedCopyFile.FileName);
                    string sactionedExtension = extension2.ToLower();
                    if (allowedExtensions.Contains(sactionedExtension))
                    {
                        fileName = fileName + DateTime.Now.ToString("yyssmmfff") + sactionedExtension;
                        loan_table.sactionedcopy = "~/sactionedcopyfile/" + fileName;
                        fileName = Path.Combine(Server.MapPath("~/sactionedcopyfile/"), fileName);
                        loan_table.sactionedCopyFile.SaveAs(fileName);
                    }
                    else
                    {
                        TempData["Message"] = "Only 'Jpg', 'png','jpeg','docx','doc','pdf' formats are alllowed..!";
                        return RedirectToAction("processloan");
                    }
                }


                else if (existing.sactionedcopy != null && loan_table.sactionedcopy != null)
                {
                    if (loan_table.sactionedCopyFile != null)
                    {
                        string path = Server.MapPath(existing.sactionedcopy);
                        FileInfo file = new FileInfo(path);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                        string BigfileName = Path.GetFileNameWithoutExtension(loan_table.sactionedCopyFile.FileName);
                        string fileName = BigfileName.Substring(0, 1);
                        string extension2 = Path.GetExtension(loan_table.sactionedCopyFile.FileName);
                        string sactionedExtension = extension2.ToLower();
                        if (allowedExtensions.Contains(sactionedExtension))
                        {
                            fileName = fileName + DateTime.Now.ToString("yyssmmfff") + sactionedExtension;
                            loan_table.sactionedcopy = "~/adminimage/" + fileName;
                            fileName = Path.Combine(Server.MapPath("~/adminimage/"), fileName);
                            loan_table.sactionedCopyFile.SaveAs(fileName);
                        }
                        else
                        {
                            TempData["Message"] = "Only 'Jpg', 'png','jpeg' images formats are alllowed..!";
                            return RedirectToAction("processloan");
                        }

                    }
                    else
                    {
                        existing.sactionedcopy = existing.sactionedcopy;
                    }
                }
                else
                {
                    existing.sactionedcopy = existing.sactionedcopy;
                }

                //ID copy file

                if (existing.idcopy == null)
                {
                    string BigfileName = Path.GetFileNameWithoutExtension(loan_table.idCopyFile.FileName);
                    string fileName = BigfileName.Substring(0, 1);
                    string extension1 = Path.GetExtension(loan_table.idCopyFile.FileName);
                    string idExtension = extension1.ToLower();
                    if (allowedExtensions.Contains(idExtension))
                    {
                        fileName = fileName + DateTime.Now.ToString("yyssmmfff") + idExtension;
                        loan_table.idcopy = "~/idcopyfile/" + fileName;
                        fileName = Path.Combine(Server.MapPath("~/idcopyfile/"), fileName);
                        loan_table.idCopyFile.SaveAs(fileName);
                    }
                    else
                    {
                        TempData["Message"] = "Only 'Jpg', 'png','jpeg','docx','doc','pdf' formats are alllowed..!";
                        return RedirectToAction("processloan");
                    }
                }


                else if (existing.idcopy != null && loan_table.idcopy != null)
                {
                    if (loan_table.idCopyFile != null)
                    {
                        string path = Server.MapPath(existing.idcopy);
                        FileInfo file = new FileInfo(path);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                        string BigfileName = Path.GetFileNameWithoutExtension(loan_table.idCopyFile.FileName);
                        string fileName = BigfileName.Substring(0, 1);
                        string extension1 = Path.GetExtension(loan_table.idCopyFile.FileName);
                        string idExtension = extension1.ToLower();
                        if (allowedExtensions.Contains(idExtension))
                        {
                            fileName = fileName + DateTime.Now.ToString("yyssmmfff") + idExtension;
                            loan_table.idcopy = "~/adminimage/" + fileName;
                            fileName = Path.Combine(Server.MapPath("~/adminimage/"), fileName);
                            loan_table.idCopyFile.SaveAs(fileName);
                        }
                        else
                        {
                            TempData["Message"] = "Only 'Jpg', 'png','jpeg' images formats are alllowed..!";
                            return RedirectToAction("processloan");
                        }

                    }
                    else
                    {
                        existing.idcopy = existing.idcopy;
                    }
                }
                else
                {
                    existing.idcopy = existing.idcopy;
                }

                //property documents

                if (existing.propertydocuments == null)
                {
                    string BigfileName = Path.GetFileNameWithoutExtension(loan_table.propertyDocumentsFile.FileName);
                    string fileName = BigfileName.Substring(0, 1);
                    string extension2 = Path.GetExtension(loan_table.propertyDocumentsFile.FileName);
                    string propertyExtension = extension2.ToLower();
                    if (allowedExtensions.Contains(propertyExtension))
                    {
                        fileName = fileName + DateTime.Now.ToString("yyssmmfff") + propertyExtension;
                        loan_table.propertydocuments = "~/propertyFile/" + fileName;
                        fileName = Path.Combine(Server.MapPath("~/propertyFile/"), fileName);
                        loan_table.propertyDocumentsFile.SaveAs(fileName);
                    }
                    else
                    {
                        TempData["Message"] = "Only 'Jpg', 'png','jpeg','docx','doc','pdf' formats are alllowed..!";
                        return RedirectToAction("Loan");
                    }
                }


                else if (existing.propertydocuments != null && loan_table.propertydocuments != null)
                {
                    if (loan_table.propertyDocumentsFile != null)
                    {
                        string path = Server.MapPath(existing.propertydocuments);
                        FileInfo file = new FileInfo(path);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                        string BigfileName = Path.GetFileNameWithoutExtension(loan_table.propertyDocumentsFile.FileName);
                        string fileName = BigfileName.Substring(0, 1);
                        string extension2 = Path.GetExtension(loan_table.propertyDocumentsFile.FileName);
                        string propertyExtension = extension2.ToLower();
                        if (allowedExtensions.Contains(propertyExtension))
                        {
                            fileName = fileName + DateTime.Now.ToString("yyssmmfff") + propertyExtension;
                            loan_table.idcopy = "~/propertyFile/" + fileName;
                            fileName = Path.Combine(Server.MapPath("~/propertyFile/"), fileName);
                            loan_table.propertyDocumentsFile.SaveAs(fileName);
                        }
                        else
                        {
                            TempData["Message"] = "Only 'Jpg', 'png','jpeg' images formats are alllowed..!";
                            return RedirectToAction("Loan");
                        }

                    }
                    else
                    {
                        existing.propertydocuments = existing.propertydocuments;
                    }
                }
                else
                {
                    existing.propertydocuments = existing.propertydocuments;
                }


                existing.customerid = loan_table.customerid;
                existing.partnerid = loan_table.partnerid;
                existing.bankid = loan_table.bankid;
                existing.loantype = loan_table.loantype;
                existing.requestloanamt = loan_table.requestloanamt;
                existing.loanamt = loan_table.loanamt;
                existing.disbursementamt = loan_table.disbursementamt;
                existing.rateofinterest = loan_table.rateofinterest;
                existing.sactionedcopy = loan_table.sactionedcopy;
                existing.idcopy = loan_table.idcopy;
                existing.propertydocuments = loan_table.propertydocuments;
                existing.propertydetails = loan_table.propertydetails;
                existing.followupdate = loan_table.followupdate;
                existing.loanstatus = loan_table.loanstatus;

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

                int latestloanid = loan_table.id;

                ///Assigned Employee
                loan_track_table loan_track_employee = new loan_track_table();
                if (loan_table.employee != null)
                {
                    loan_track_employee.loanid = latestloanid.ToString();
                    loan_track_employee.employeeid = loan_table.employee;
                    loan_track_employee.tracktime = DateTime.Now.ToString();

                    if (loan_table.internalcomment != null)
                    {
                        loan_track_employee.internalcomment = loan_table.internalcomment;
                    }
                    if (loan_table.externalcomment != null)
                    {
                        loan_track_employee.externalcomment = loan_table.externalcomment;
                    }

                    loan_track_employee.datex = DateTime.Now.ToString();
                    loan_track_employee.addedby = Session["username"].ToString();
                    ags.loan_track_table.Add(loan_track_employee);
                    ags.SaveChanges();
                }


                vendor_track_table vendor_track = new vendor_track_table();
                if (loan_table.partnerid != null)
                {
                    vendor_track.loanid = latestloanid.ToString();
                    if (loan_table.partnerid != partner)
                    {

                        vendor_track.vendorid = loan_table.partnerid;
                        vendor_track.tracktime = DateTime.Now.ToString();
                        vendor_track.comment = "Assigned";

                        vendor_track.datex = DateTime.Now.ToString();
                        vendor_track.addedby = Session["username"].ToString();
                        ags.vendor_track_table.Add(vendor_track);
                        ags.SaveChanges();

                    }
                }



                //assigned table
                assigned_table existing_data = ags.assigned_table.Where(x => x.loanid == loan_table.id.ToString()).FirstOrDefault();


                //existing_data.loanid = latestloanid.ToString();
                if (loan_table.employee != null)
                {
                    existing_data.assign_emp_id = loan_table.employee;
                }
                else
                {
                    existing_data.assign_emp_id = Session["userid"].ToString();
                }
                if (loan_table.partnerid != null)
                {
                    existing_data.assign_vendor_id = loan_table.partnerid;
                }
                else
                {
                    existing_data.assign_vendor_id = Session["userid"].ToString();
                }
                if (existing_data.addedby == null)
                {
                    existing_data.addedby = Session["username"].ToString();
                }
                else
                {
                    existing_data.addedby = existing_data.addedby;
                }
                if (existing_data.datex == null)
                {
                    existing_data.datex = DateTime.Now.ToString();
                }
                else
                {
                    existing_data.datex = existing_data.datex;
                }
                ags.SaveChanges();

                return RedirectToAction("processloan");
            }
            return PartialView("~/Views/ProcessTeam/ProcessLoan/Edit.cshtml", loan_table);
        }

        [HttpGet]
        public ActionResult Track(int? Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "process_team")
            {
                return this.RedirectToAction("Logout", "Account");
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


            return PartialView("~/Views/ProcessTeam/ProcessLoan/Track.cshtml", loan_track);
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