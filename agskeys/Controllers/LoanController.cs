using agskeys.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace agskeys.Controllers
{
    [Authorize]
    public class LoanController : Controller
    {
        agsfinancialsEntities ags = new agsfinancialsEntities();
        public ActionResult Loan()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            var getCustomer = ags.customer_profile_table.ToList();
            var customer_loans = (from loan_table in ags.loan_table orderby loan_table.id descending select loan_table).ToList();
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
            
            return PartialView(customer_loans);
        }

        [HttpGet]
        public ActionResult Create()
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
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

            var empCategory = ags.emp_category_table.ToList();
            SelectList empCategories = new SelectList(empCategory, "emp_category_id", "emp_category");
            ViewBag.empCategories = empCategories;

            var employee = ags.admin_table.ToList();
            SelectList employees = new SelectList(employee, "id", "name");
            ViewBag.employees = employees;


            var model = new agskeys.Models.loan_table();
            return PartialView(model);
        }


        public JsonResult GetEmployeeList(string categoryId)
        {
            ags.Configuration.ProxyCreationEnabled = false;
            List<admin_table> employees = ags.admin_table.Where(x => x.userrole.ToString() == categoryId).ToList();
            return Json(employees, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(loan_table obj)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
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

                List<emp_category_table> categoryList = ags.emp_category_table.ToList();
                ViewBag.empCategories = new SelectList(categoryList, "emp_category_id", "emp_category");
                
                var getloantype = ags.loantype_table.ToList();
                SelectList loantp = new SelectList(getloantype, "id", "loan_type");
                ViewBag.loantypeList = loantp;

                var empCategory = ags.emp_category_table.ToList();
                SelectList empCategories = new SelectList(empCategory, "emp_category_id", "emp_category");
                ViewBag.empCategories = empCategories;

                var employee = ags.admin_table.ToList();
                SelectList employees = new SelectList(employee, "id", "name");
                ViewBag.employees = employees;

                // var usr = (from u in ags.loan_table where u. == obj.username select u).FirstOrDefault();
                var allowedExtensions = new[] {
                    ".png", ".jpg", ".jpeg",".doc",".docx",".pdf"
                };
                string sactionedFileName = Path.GetFileNameWithoutExtension(obj.sactionedCopyFile.FileName);
                string fileName = sactionedFileName.Substring(0, 1);
                string extension1 = Path.GetExtension(obj.sactionedCopyFile.FileName);
                string extension = extension1.ToLower();
                if (allowedExtensions.Contains(extension))
                {
                    fileName = fileName + DateTime.Now.ToString("yyssmmfff") + extension;
                    obj.sactionedcopy = "~/sactionedcopyfile/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/sactionedcopyfile/"), fileName);
                    obj.sactionedCopyFile.SaveAs(fileName);
                }
                else
                {
                    TempData["Message"] = "Only 'Jpg','png','jpeg','docx','doc','pdf' images formats are alllowed..!";
                    return View();
                }

                string idCopyFileName = Path.GetFileNameWithoutExtension(obj.idCopyFile.FileName);
                string idFileName = idCopyFileName.Substring(0, 1);
                string extension2 = Path.GetExtension(obj.idCopyFile.FileName);
                string idExtension = extension2.ToLower();
                if (allowedExtensions.Contains(idExtension))
                {
                    idFileName = idFileName + DateTime.Now.ToString("yyssmmfff") + extension;
                    obj.idcopy = "~/idcopyfile/" + idFileName;
                    idFileName = Path.Combine(Server.MapPath("~/idcopyfile/"), idFileName);
                    obj.idCopyFile.SaveAs(idFileName);
                }
                else
                {
                    TempData["Message"] = "Only 'Jpg','png','jpeg','docx','doc','pdf' formats are alllowed..!";
                    return View();
                }
                loan_table loan = new loan_table();
                loan.customerid = obj.customerid;
                loan.partnerid = obj.partnerid;
                loan.bankid = obj.bankid;
                loan.loantype = obj.loantype;
                loan.loanamt = obj.loanamt;
                loan.disbursementamt = obj.disbursementamt;
                loan.rateofinterest = obj.rateofinterest;
                loan.sactionedcopy = obj.sactionedcopy;
                loan.idcopy = obj.idcopy;
                loan.datex = DateTime.Now.ToString();
                loan.addedby = Session["username"].ToString();
                ags.loan_table.Add(loan);
                ags.SaveChanges();

                int latestloanid = loan.id;

                loan_track_table loan_track = new loan_track_table();
                loan_track.loanid = latestloanid.ToString(); 
                if (obj.employee != null)
                {
                    loan_track.employeeid = obj.employee;
                    loan_track.tracktime = DateTime.Now.ToString();
                }
                //if (obj.partnerid != null)
                //{
                //    loan_track.vendorid = obj.partnerid;
                //    loan_track.vendortracktime = DateTime.Now.ToString();

                //}
                if (obj.internalcomment != null)
                {
                    loan_track.internalcomment = obj.internalcomment;
                }
                if (obj.externalcomment != null)
                {
                    loan_track.externalcomment = obj.externalcomment;
                }
                loan_track.datex = DateTime.Now.ToString();
                loan_track.addedby = Session["username"].ToString();
                ags.loan_track_table.Add(loan_track);
                ags.SaveChanges();

                return RedirectToAction("Loan");

            }
            else
            {
                TempData["AE"] = "Something went wrong";
                return RedirectToAction("Loan");
            }
        }
        public ActionResult Details(int? Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
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

            foreach (var customer in getCustomer)
            {
                if (user.customerid == customer.id.ToString())
                {
                    user.customerid = customer.customerid;
                    ViewBag.name = customer.name;
                    ViewBag.phoneno = customer.phoneno;
                    ViewBag.email = customer.email;
                    ViewBag.profileimg = customer.profileimg;
                    break;
                }
                else if (!ags.loan_table.Any(s => s.customerid.ToString() == customer.id.ToString()))
                {
                    user.customerid = "Not Updated";
                }
            }


            var getVendor = ags.vendor_table.ToList();
            foreach (var items in getVendor)
            {
                if (user.partnerid == items.id.ToString())
                {
                    user.partnerid = items.companyname;
                    break;
                }
                else if (!ags.loan_table.Any(s => s.partnerid.ToString() == items.id.ToString()))
                {
                    user.customerid = "Not Updated";
                }
            }

            var getBank = ags.bank_table.ToList();

            foreach (var bank in getBank)
            {
                if (user.bankid == bank.id.ToString())
                {
                    user.bankid = bank.bankname;
                    break;
                }
                else if (!ags.loan_table.Any(s => s.bankid.ToString() == bank.id.ToString()))
                {
                    user.bankid = "Not Updated";
                }
            }

            var getloantype = ags.loantype_table.ToList();
            foreach (var loantp in getloantype)
            {
                if (user.loantype == loantp.id.ToString())
                {
                    user.loantype = loantp.loan_type;
                    break;
                }
                else if (!ags.loan_table.Any(s => s.loantype.ToString() == loantp.id.ToString()))
                {
                    user.loantype = "Not Updated";
                }
            }
            
            return PartialView(user);
        }


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

            loan_table loan_table = ags.loan_table.Find(Id);
            if (loan_table == null)
            {
                return HttpNotFound();
            }
            return PartialView(loan_table);
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

                var allowedExtensions = new[] {
                    ".png", ".jpg", ".jpeg",".doc",".docx",".pdf"
                };
                loan_table existing = ags.loan_table.Find(loan_table.id);
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
                        return RedirectToAction("Loan");
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
                            return RedirectToAction("Loan");
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
                        return RedirectToAction("Loan");
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
                            return RedirectToAction("Loan");
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

                existing.customerid = loan_table.customerid;
                existing.partnerid = loan_table.partnerid;
                existing.bankid = loan_table.bankid;
                existing.loantype = loan_table.loantype;
                existing.loanamt = loan_table.loanamt;
                existing.disbursementamt = loan_table.disbursementamt;
                existing.rateofinterest = loan_table.rateofinterest;
                existing.sactionedcopy = loan_table.sactionedcopy;
                existing.idcopy = loan_table.idcopy;

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
                return RedirectToAction("Loan", "Loan");
            }
            return PartialView(loan_table);
        }

        public ActionResult Delete(int? Id)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
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

            foreach (var customer in getCustomer)
            {
                if (user.customerid == customer.id.ToString())
                {
                    user.customerid = customer.customerid;
                    ViewBag.name = customer.name;
                    ViewBag.phoneno = customer.phoneno;
                    ViewBag.email = customer.email;
                    ViewBag.profileimg = customer.profileimg;
                    break;
                }
                else if (!ags.loan_table.Any(s => s.customerid.ToString() == customer.id.ToString()))
                {
                    user.customerid = "Not Updated";
                }
            }


            var getVendor = ags.vendor_table.ToList();
            foreach (var items in getVendor)
            {
                if (user.partnerid == items.id.ToString())
                {
                    user.partnerid = items.companyname;
                    break;
                }
                else if (!ags.loan_table.Any(s => s.partnerid.ToString() == items.id.ToString()))
                {
                    user.customerid = "Not Updated";
                }
            }

            var getBank = ags.bank_table.ToList();

            foreach (var bank in getBank)
            {
                if (user.bankid == bank.id.ToString())
                {
                    user.bankid = bank.bankname;
                    break;
                }
                else if (!ags.loan_table.Any(s => s.bankid.ToString() == bank.id.ToString()))
                {
                    user.bankid = "Not Updated";
                }
            }

            var getloantype = ags.loantype_table.ToList();
            foreach (var loantp in getloantype)
            {
                if (user.loantype == loantp.id.ToString())
                {
                    user.loantype = loantp.loan_type;
                    break;
                }
                else if (!ags.loan_table.Any(s => s.loantype.ToString() == loantp.id.ToString()))
                {
                    user.loantype = "Not Updated";
                }
            }

            return PartialView(user);
        }
        // POST: vendor_table/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            loan_table loan_table = ags.loan_table.Find(id);
            string idcopypath = Server.MapPath(loan_table.idcopy);
            FileInfo fileIdCopy = new FileInfo(idcopypath);
            if (fileIdCopy.Exists)
            {
                fileIdCopy.Delete();
            }
            string sactionedcopypath = Server.MapPath(loan_table.sactionedcopy);
            FileInfo fileSactioned = new FileInfo(sactionedcopypath);
            if (fileSactioned.Exists)
            {
                fileSactioned.Delete();
            }
            var loan_track = ags.loan_track_table.Where(x => x.loanid == loan_table.id.ToString());
            ags.loan_track_table.RemoveRange(loan_track);
            ags.loan_table.Remove(loan_table);
            ags.SaveChanges();
           
            return RedirectToAction("Loan");
        }

        [HttpGet]
        public ActionResult Track(int loanid)
        {
            if (Session["username"] == null || Session["userlevel"].ToString() != "super_admin")
            {
                return this.RedirectToAction("Logout", "Account");
            }
            var loan_track = (from loan_track_table in ags.loan_track_table orderby loan_track_table.id descending select loan_track_table).ToList().Where(x=>x.loanid == loanid.ToString());
            return PartialView(loan_track);
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