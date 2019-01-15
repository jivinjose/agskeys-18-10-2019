
using agskeys.Models;
using PasswordSecurity;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web.Mvc;

namespace agskeys.Controllers
  
{
    [Authorize]
    public class SuperAdminController : Controller
    {
        agsfinancialsEntities ags = new agsfinancialsEntities();
       
        public ActionResult Index()
        {            
            if (Session["username"] == null)
            {
                RedirectToAction("Login");
            }
            var name = Session["username"].ToString();
            return View();
        }
        public ActionResult Admin()
        {
            if (Session["username"] == null)
            {
                RedirectToAction("Login");
            }
            var subAdmin = (from sub in ags.admin_table orderby sub.id descending select sub).ToList();

            return PartialView(subAdmin);

        }
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["username"] == null)
            {
                RedirectToAction("Login");
            }
            var model = new agskeys.Models.admin_table();//load data from database by RestaurantId
            return PartialView(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(admin_table obj)
        {
            if (Session["username"] == null)
            {
                RedirectToAction("Login");
            }
            if (ModelState.IsValid)
            {
                var usr = (from u in ags.admin_table where u.username == obj.username select u).FirstOrDefault();
                var allowedExtensions = new[] {
                    ".Jpg", ".png", ".jpg", "jpeg"
                };

                if (usr == null)
                {
                    string BigfileName = Path.GetFileNameWithoutExtension(obj.ImageFile.FileName);
                    string fileName = BigfileName.Substring(0, 5);
                    string extension = Path.GetExtension(obj.ImageFile.FileName);
                    if (allowedExtensions.Contains(extension))
                    {
                        fileName = fileName + DateTime.Now.ToString("yyssmmfff") + extension;
                        obj.photo = "~/adminimage/" + fileName;
                        fileName = Path.Combine(Server.MapPath("~/adminimage/"), fileName);
                        obj.ImageFile.SaveAs(fileName);
                    }
                    else
                    {
                        TempData["Message"] = "Only 'Jpg', 'png','jpeg' images formats are alllowed..!";
                        return View();
                    }

                    if (obj.userrole == "1")
                    {
                        obj.userrole = "ab1d92aaed90828a0fc1c95ecd5fda1f";
                    }
                    else if (obj.userrole == "2")
                    {
                        obj.userrole = "079e946e1272938b097a0baab6a36477";
                    }
                    else
                    {
                        obj.userrole = "Undefined";
                    }
                    obj.password = PasswordStorage.CreateHash(obj.password);
                    ags.admin_table.Add(new admin_table
                    {
                        name = obj.name,
                        email = obj.email,
                        phoneno = obj.phoneno,
                        alternatephone = obj.alternatephone,
                        dob = obj.dob,
                        userrole = obj.userrole,
                        username = obj.username,
                        password = obj.password,
                        photo = obj.photo,
                        isActive = obj.isActive,
                        address = obj.address,
                        datex = DateTime.Now.ToString(),
                        addedby = Session["username"].ToString()
                    });
                    ags.SaveChanges();
                    return RedirectToAction("Admin");
                }
                else
                {
                    TempData["AE"] = "This user name is already exist";
                    //return RedirectToAction("Admin");
                }
            }
            return View(obj);
        }

        public ActionResult Details(int Id)
        {
            if (Session["username"] == null)
            {
                RedirectToAction("Login");
            }
            var user = ags.admin_table.Where(x => x.id == Id).FirstOrDefault();
            return PartialView(user);
        }

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
            admin_table admin_table = ags.admin_table.Find(Id);
            if (admin_table == null)
            {
                return HttpNotFound();
            }
            return PartialView(admin_table);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(admin_table admin_table)
        {
            if (ModelState.IsValid)
            {
                var allowedExtensions = new[] {
                    ".Jpg", ".png", ".jpg", "jpeg"
                };
                var userCount = (from u in ags.admin_table where u.username == admin_table.username select u).Count();

                admin_table existing = ags.admin_table.Find(admin_table.id);
                var password = existing.password.ToString();
                var newPassword = admin_table.password.ToString();

                if (admin_table.userrole == "1")
                {
                    admin_table.userrole = "ab1d92aaed90828a0fc1c95ecd5fda1f";
                }
                else if (admin_table.userrole == "2")
                {
                    admin_table.userrole = "079e946e1272938b097a0baab6a36477";
                }
                else
                {
                    admin_table.userrole = "Undefined";
                }
                if (existing.photo == null)
                {
                    string BigfileName = Path.GetFileNameWithoutExtension(admin_table.ImageFile.FileName);
                    string fileName = BigfileName.Substring(0, 5);
                    string extension = Path.GetExtension(admin_table.ImageFile.FileName);
                    if (allowedExtensions.Contains(extension))
                    {
                        fileName = fileName + DateTime.Now.ToString("yyssmmfff") + extension;
                        admin_table.photo = "~/adminimage/" + fileName;
                        fileName = Path.Combine(Server.MapPath("~/adminimage/"), fileName);
                        admin_table.ImageFile.SaveAs(fileName);
                    }
                    else
                    {
                        TempData["Message"] = "Only 'Jpg', 'png','jpeg' images formats are alllowed..!";
                        return View();
                    }
                }


                else if (existing.photo != null && admin_table.photo != null)
                {
                    if (admin_table.ImageFile != null)
                    {
                        string path = Server.MapPath(existing.photo);
                        FileInfo file = new FileInfo(path);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                        string BigfileName = Path.GetFileNameWithoutExtension(admin_table.ImageFile.FileName);
                        string fileName = BigfileName.Substring(0, 5);
                        string extension = Path.GetExtension(admin_table.ImageFile.FileName);
                        if (allowedExtensions.Contains(extension))
                        {
                            fileName = fileName + DateTime.Now.ToString("yyssmmfff") + extension;
                            admin_table.photo = "~/adminimage/" + fileName;
                            fileName = Path.Combine(Server.MapPath("~/adminimage/"), fileName);
                            admin_table.ImageFile.SaveAs(fileName);
                        }
                        else
                        {
                            TempData["Message"] = "Only 'Jpg', 'png','jpeg' images formats are alllowed..!";
                            return View();
                        }

                    }
                    else
                    {
                        existing.photo = existing.photo;
                    }
                }
                else
                {
                    existing.photo = existing.photo;
                }
                existing.name = admin_table.name;
                existing.email = admin_table.email;
                existing.phoneno = admin_table.phoneno;
                existing.alternatephone = admin_table.alternatephone;
                existing.dob = admin_table.dob;
                existing.address = admin_table.address;
                existing.username = admin_table.username;

                if (userCount == '1')
                {
                    existing.username = admin_table.username;
                }
                else
                {
                    TempData["AE"] = "This user name is already exist";
                    return RedirectToAction("Edit", "SuperAdmin");
                }

                existing.isActive = admin_table.isActive;
                existing.photo = admin_table.photo;

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
                    existing.password = admin_table.password;
                }
                else
                {
                    existing.password = PasswordStorage.CreateHash(admin_table.password);
                }
                ags.SaveChanges();
                return RedirectToAction("SuperAdmin");
            }
            return PartialView(admin_table);
        }

        // GET: vendor_table/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            admin_table admin_table = ags.admin_table.Find(id);
            if (admin_table == null)
            {
                return HttpNotFound();
            }
            return PartialView(admin_table);
        }

        // POST: vendor_table/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            admin_table admin_table = ags.admin_table.Find(id);
            string path = Server.MapPath(admin_table.photo);
            FileInfo file = new FileInfo(path);
            if (file.Exists)
            {
                file.Delete();
            }
            ags.admin_table.Remove(admin_table);
            ags.SaveChanges();
            return RedirectToAction("Index");
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