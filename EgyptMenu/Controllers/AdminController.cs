using EgyptMenu.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace EgyptMenu.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private readonly Entities db = new Entities();
        private readonly ApplicationDbContext AuthDb = new ApplicationDbContext();
        // GET: Admin
        public ActionResult Dashboard()
        {
            return View();
        }
        public ActionResult Restaurants()
        {
            return View(db.restorants.ToList());
        }

        public ActionResult EditRestaurant(int id)
        {
            restorant CurrentRestaurant = db.restorants.Find(id);
            ResMgmtViewModel resMgmtViewModel = new ResMgmtViewModel()
            {
                id = CurrentRestaurant.id,
                OwnerEmail = CurrentRestaurant.user.email,
                OwnerName = CurrentRestaurant.user.name,
                OwnerPhone = CurrentRestaurant.user.phone,
                RestaurantAddress = CurrentRestaurant.address,
                RestaurantName = CurrentRestaurant.name,
                RestaurantDescription = CurrentRestaurant.description,
                RestaurantImage = CurrentRestaurant.logo,
                RestaurantCoverImage = CurrentRestaurant.cover,
                lat = CurrentRestaurant.lat,
                lng = CurrentRestaurant.lng
            };
            return View(resMgmtViewModel);
        }
        [HttpPost]
        public ActionResult EditRestaurant(HttpPostedFileBase ImgFile
            , HttpPostedFileBase CoverFile, ResMgmtViewModel model)
        {
         
            var CurrentUser = db.users.Where(r => r.email == model.OwnerEmail).FirstOrDefault();
            var CurrentUserId = CurrentUser.id;
            var CurrentRestaurant = db.restorants.Where(r => r.user_id == CurrentUserId).FirstOrDefault();

            if (ImgFile != null)
            {
                WebImage img = new WebImage(ImgFile.InputStream);
                img.FileName = "L_" + CurrentRestaurant.id + ImgFile.FileName;

                img.Resize(590, 590);
                string physicalPath = Path.Combine("~/Content/images/" + img.FileName);
                img.Save(physicalPath);
                WebImage img1 = img;
                img1.FileName = "M_" + CurrentRestaurant.id + ImgFile.FileName;

                img1.Resize(300, 300);
                string physicalPath1 = Path.Combine("~/Content/images/" + img1.FileName);
                img1.Save(physicalPath1);
                WebImage img2 = img;
                img2.FileName = "S_" + CurrentRestaurant.id + ImgFile.FileName;
                img2.Resize(200, 200, false);
                string physicalPath2 = Path.Combine("~/Content/images/" + img2.FileName);
                img2.Save(physicalPath2);
                CurrentRestaurant.logo = ImgFile.FileName;
                model.RestaurantImage = ImgFile.FileName;
            }
            if (CoverFile != null)
            {
                WebImage img = new WebImage(CoverFile.InputStream);
                img.Resize(590, 590);
                img.FileName = "L_C_" + CurrentRestaurant.id + CoverFile.FileName;
                string physicalPath = Path.Combine("~/Content/images/" + img.FileName);
                img.Save(physicalPath);
                WebImage img1 = img;
                img1.Resize(300, 300);
                img1.FileName = "M_C_" + CurrentRestaurant.id + CoverFile.FileName;
                string physicalPath1 = Path.Combine("~/Content/images/" + img1.FileName);
                img1.Save(physicalPath1);
                WebImage img2 = img;
                img2.Resize(200, 200, false);
                img2.FileName = "S_C_" + CurrentRestaurant.id + CoverFile.FileName;
                string physicalPath2 = Path.Combine("~/Content/images/" + img2.FileName);
                img2.Save(physicalPath2);
                CurrentRestaurant.cover = CoverFile.FileName;
                model.RestaurantCoverImage = CoverFile.FileName;

            }


            CurrentRestaurant.name = model.RestaurantName;
            CurrentRestaurant.description = model.RestaurantDescription;
            CurrentRestaurant.address = model.RestaurantAddress;
            CurrentRestaurant.lat = model.lat;
            CurrentRestaurant.lng = model.lng;
            model.RestaurantCoverImage = CurrentRestaurant.cover;
            model.RestaurantImage = CurrentRestaurant.logo;
            model.id = CurrentRestaurant.id;
            db.SaveChanges();

            return RedirectToAction("EditRestaurant",new { id = CurrentRestaurant.id });
        }

        [HttpPost]
        public ActionResult DeactivateRestaurant(int id)
        {
            restorant Res = db.restorants.Find(id);
            Res.active = 0;
            db.SaveChanges();
            return RedirectToAction("Restaurants");
        }

        [HttpPost]
        public ActionResult ActivateRestaurant(int id)
        {
            restorant Res = db.restorants.Find(id);
            Res.active = 1;
            db.SaveChanges();
            return RedirectToAction("Restaurants");
        }
        public ActionResult Plans()
        {
            return View();
        }
        public ActionResult LangTranslations()
        {
            return View();
        }
        public ActionResult Env()
        {
            return View();
        }
        public ActionResult Settings()
        {
            return View();
        }

        public ActionResult CreateRestaurant()
        {
            return View();
        }
        public ActionResult CreatePlan()
        {
            return View();
        }
        public ActionResult Systemstatus()
        {
            return View();
        }

    }
}