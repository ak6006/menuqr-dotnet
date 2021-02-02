using EgyptMenu.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImageResizer;
using System.Web.Hosting;

namespace EgyptMenu.Controllers
{
    [Authorize(Roles = "Owner")]
    public class OwnerController : Controller
    {
        ApplicationDbContext AuthDB = new ApplicationDbContext();
        private Entities db = new Entities();

        public restorant GetRestorant()
        {
            var UserId = User.Identity.GetUserId();
            var UserEmail = AuthDB.Users.Where(u => u.Id == UserId).FirstOrDefault().Email;
            var CurrentUser = db.users.Where(r => r.email == UserEmail).FirstOrDefault();
            var CurrentUserId = CurrentUser.id;
            var CurrentRestaurant = db.restorants.Where(r => r.user_id == CurrentUserId).FirstOrDefault();
            return CurrentRestaurant;
        }

        [Obsolete]
        public List<string> GenerateVersions(string original)
        {
            string pathURL = HttpContext.Server.MapPath("\\Content\\images");
            Dictionary<string, string> versions = new Dictionary<string, string>();
            //Define the versions to generate and their filename suffixes.
            versions.Add("_thumb", "width=100&height=100&crop=auto&format=jpg"); //Crop to square 
            versions.Add("_medium", "maxwidth=300&maxheight=300&format=jpg"); //Fit inside 400x400
            //versions.Add("_large", "maxwidth=1900&maxheight=1900&format=jpg"); //Fit inside 1900x1200
            //versions.Add("_large", "format=jpg&mode=max&quality=50");
            versions.Add("_large", "maxwidth=500&maxheight=500&format=jpg");
            string basePath = ImageResizer.Util.PathUtils.RemoveExtension(original);
            FileStream fsrw = new FileStream(pathURL+"\\"+original, FileMode.Open, FileAccess.ReadWrite);

            //To store the list of generated paths
            List<string> generatedFiles = new List<string>();

            //Generate each version
            foreach (string suffix in versions.Keys)
                //Let the image builder add the correct extension based on the output file type
                generatedFiles.Add(ImageBuilder.Current.Build(pathURL, basePath + suffix ,
            new ResizeSettings(versions[suffix]), false, true));

            return generatedFiles;
        }

        // GET: Owner
        public ActionResult Dashboard()
        {
            var UserId = User.Identity.GetUserId();
            var UserEmail = AuthDB.Users.Where(u => u.Id == UserId).FirstOrDefault().Email;
            var CurrentUser = db.users.Where(r => r.email == UserEmail).FirstOrDefault();
            var CurrentUserId = CurrentUser.id;
            var CurrentRestaurant = db.restorants.Where(r => r.user_id == CurrentUserId).FirstOrDefault();
            ResMgmtViewModel resMgmtViewModel = new ResMgmtViewModel()
            {
                OwnerEmail = UserEmail,
                OwnerName = CurrentUser.name,
                OwnerPhone = CurrentUser.phone,
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
        [Obsolete]
        public ActionResult EditRestaurant(HttpPostedFileBase ImgFile
            , HttpPostedFileBase CoverFile, ResMgmtViewModel model)
        {
            var UserId = User.Identity.GetUserId();
            var UserEmail = AuthDB.Users.Where(u => u.Id == UserId).FirstOrDefault().Email;
            var CurrentUser = db.users.Where(r => r.email == UserEmail).FirstOrDefault();
            var CurrentUserId = CurrentUser.id;
            var CurrentRestaurant = db.restorants.Where(r => r.user_id == CurrentUserId).FirstOrDefault();





            if (ImgFile != null)
            {
                string physicalPath1 = HttpContext.Server.MapPath("~/Content/images/" + ImgFile.FileName);
                ImgFile.SaveAs(physicalPath1);
                List<string> ImagesNames = GenerateVersions(ImgFile.FileName);
                foreach (var item in ImagesNames)
                {
                    string physicalPath = HttpContext.Server.MapPath("~/Content/images/" + item);
                    ImgFile.SaveAs(physicalPath);
                }
                CurrentRestaurant.logo = ImgFile.FileName;
                model.RestaurantImage = ImgFile.FileName;

            }
            if (CoverFile != null)
            {
                string physicalPath = HttpContext.Server.MapPath("~/Content/images/" + CoverFile.FileName);
                CoverFile.SaveAs(physicalPath);
                CurrentRestaurant.cover = CoverFile.FileName;
                model.RestaurantCoverImage = CoverFile.FileName;

            }


            CurrentRestaurant.name = model.RestaurantName;
            CurrentRestaurant.description = model.RestaurantDescription;
            CurrentRestaurant.address = model.RestaurantAddress;
            model.RestaurantCoverImage = CurrentRestaurant.cover;
            model.RestaurantImage = CurrentRestaurant.logo;
            db.SaveChanges();

            return View("Dashboard", model);
        }
        public ActionResult Menu()
        {
            restorant CurrentRestaurant = GetRestorant();
            return View(CurrentRestaurant);
        }

        [HttpPost]
        public ActionResult AddCategory(string CatName)
        {
            var CurrentRestaurant = GetRestorant();
            var category = new category()
            {
                restorant = CurrentRestaurant,
                restorant_id = CurrentRestaurant.id,
                name = CatName,
                order_index=1,
                active=1
            };
            try
            {
                db.categories.Add(category);
                db.SaveChanges();
            }
            catch (Exception)
            {
              
            }

            return View("Menu", CurrentRestaurant);
        }

        public ActionResult DeleteCategory(int CatId)
        {
            var CurrentRestaurant = GetRestorant();
            try
            {
                var Catitems = db.items.Where(i => i.category_id == CatId).ToList();
                foreach (var item in Catitems)
                {
                    var extras = db.extras.Where(e => e.item_id == item.id);
                    foreach (var extra in extras)
                    {
                        db.extras.Remove(extra);
                    }
                    db.items.Remove(item);
                }
                db.categories.Remove(db.categories.Find(CatId));
                db.SaveChanges();

            }
            catch (Exception)
            {
                return RedirectToAction("Menu", CurrentRestaurant);
            }
            return RedirectToAction("Menu", CurrentRestaurant);

        }

        public ActionResult AddItemToCat(int id,string item_name,string item_description,decimal item_price
            , HttpPostedFileBase item_image)
        {
            if (item_image != null)
            {
                string physicalPath = HttpContext.Server.MapPath("~/Content/images/" + item_image.FileName);
                item_image.SaveAs(physicalPath);
            }
            var CurrentRestaurant = GetRestorant();
            var Item = new item()
            {
                name = item_name,
                price = item_price,
                category_id = id,
                description = item_description,
                image = item_image.FileName
            };
            db.items.Add(Item);
            db.SaveChanges();
            return RedirectToAction("Menu", CurrentRestaurant);
        }

        public ActionResult QRBuilder()
        {
            return View();
        }
        public ActionResult Plan()
        {
            return View();
        }
        public ActionResult Edit(int id)
        {
            var model = db.items.Find(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult EditItem(HttpPostedFileBase item_image, item model )
        {
            var CurrentRestaurant = GetRestorant();
            try
            {
                var OldItem = db.items.Find(model.id);
                if (item_image != null)
                {
                    string physicalPath = HttpContext.Server.MapPath("~/Content/images/" + item_image.FileName);
                    item_image.SaveAs(physicalPath);
                    OldItem.image = item_image.FileName;
                }
                OldItem.name = model.name;
                OldItem.price = model.price;
                OldItem.description = model.description;
                OldItem.vat = model.vat;
                if (model.available==1)
                {
                    OldItem.available = 1;
                }
                else if(model.available==0)
                {
                    OldItem.available = 0;
                }
                if (model.has_variants == 1)
                {
                    OldItem.has_variants = 1;
                }
                else if (model.has_variants == 0)
                {
                    OldItem.has_variants = 0;
                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                return View(e.Message);
            }
            return RedirectToAction("Menu", CurrentRestaurant);
        }
        
        [HttpPost]
        public ActionResult DeleteItem(int id)
        {
            var CurrentRestaurant = GetRestorant();
            try
            {
                var item = db.items.Find(id);
                var extras = db.extras.Where(e => e.item_id == id).ToList();
                foreach (var extra in extras)
                {
                    db.extras.Remove(extra);
                }
                db.items.Remove(item);
                db.SaveChanges();
                return RedirectToAction("Menu", CurrentRestaurant);
            }
            catch (Exception e)
            {
                return View("Menu", e.Message);
            }
        }

        [HttpPost]
        public ActionResult AddExtras(int ItemId , string name , decimal price)
        {
            var model = db.items.Find(ItemId);
            try
            {
                var extra = new extra()
                {
                    item_id = ItemId,
                    name = name,
                    price = price
                };
                db.extras.Add(extra);
                db.SaveChanges();
                return RedirectToAction("Edit", model);
            }
            catch (Exception)
            {
                return RedirectToAction("Edit",model);
            }
        }

       

        [HttpPost] // this action takes the viewModel from the modal
        public ActionResult EditExtra(int ItemId, int ExtraId, string name , decimal price)
        {
            
            var model = db.items.Find(ItemId);

            if (ModelState.IsValid)
            {
                var OldExtra = db.extras.Find(ExtraId);
                OldExtra.name = name;
                OldExtra.price = price;
                db.SaveChanges();
                return RedirectToAction("Edit", model);
            }
            else
            {
                return RedirectToAction("Edit", model);
            }
        }


        [HttpPost]
        public ActionResult DeleteExtra(int @ItemId , int id)
        {
            var model = db.items.Find(ItemId);
            try
            {
                db.extras.Remove(db.extras.Find(id));
                db.SaveChanges();
                return RedirectToAction("Edit", model);
            }
            catch (Exception e)
            {
                return RedirectToAction("Edit", e.Message);
            }
        }

        public new ActionResult Profile()
        {
            return View();
        }
        public ActionResult Restaurant()
        {
            var restaurant = GetRestorant();
            return View(restaurant);
        }

        public ActionResult ItemModal (int id)
        {
            var item = db.items.Find(id);
            return PartialView("_ItemModal", item);
        }

    }
}