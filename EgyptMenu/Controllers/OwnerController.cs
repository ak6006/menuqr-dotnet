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
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Helpers;

namespace EgyptMenu.Controllers
{
    [Authorize]
    public class OwnerController : Controller
    {
        ApplicationDbContext AuthDB = new ApplicationDbContext();
        private Entities db = new Entities();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;


        public OwnerController()
        {
        }

        public OwnerController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public restorant GetRestorant()
        {
            var UserId = User.Identity.GetUserId();
            var UserEmail = AuthDB.Users.Where(u => u.Id == UserId).FirstOrDefault().Email;
            var CurrentUser = db.users.Where(r => r.email == UserEmail).FirstOrDefault();
            var CurrentUserId = CurrentUser.id;
            var CurrentRestaurant = db.restorants.Where(r => r.user_id == CurrentUserId).FirstOrDefault();
            return CurrentRestaurant;
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
                id=CurrentRestaurant.id,
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
                WebImage img = new WebImage(ImgFile.InputStream);
                img.FileName = "L_" + CurrentRestaurant.id + ImgFile.FileName;

                img.Resize(590, 590);
                string physicalPath = Path.Combine("~/Content/images/" +  img.FileName);
                img.Save(physicalPath);
                WebImage img1 = img;
                img1.FileName = "M_" + CurrentRestaurant.id + ImgFile.FileName;

                img1.Resize(300, 300);
                string physicalPath1 = Path.Combine("~/Content/images/" +  img1.FileName);
                img1.Save(physicalPath1);
                WebImage img2 = img;
                img2.FileName = "S_" + CurrentRestaurant.id + ImgFile.FileName;
                img2.Resize(200, 200, false);
                string physicalPath2 = Path.Combine("~/Content/images/" +  img2.FileName);
                img2.Save(physicalPath2);
                CurrentRestaurant.logo = ImgFile.FileName;
                model.RestaurantImage = ImgFile.FileName;
            }
            if (CoverFile != null)
            {
                WebImage img = new WebImage(CoverFile.InputStream);
                img.Resize(590,590);
                img.FileName = "L_C_" + CurrentRestaurant.id + CoverFile.FileName;
                string physicalPath = Path.Combine("~/Content/images/"+ img.FileName);
                img.Save(physicalPath);
                WebImage img1 = img;
                img1.Resize(300, 300);
                img1.FileName = "M_C_" + CurrentRestaurant.id + CoverFile.FileName;
                string physicalPath1 = Path.Combine("~/Content/images/"+ img1.FileName);
                img1.Save(physicalPath1);
                WebImage img2 = img;
                img2.Resize(200, 200,false);
                img2.FileName = "S_C_" + CurrentRestaurant.id + CoverFile.FileName;
                string physicalPath2 = Path.Combine("~/Content/images/"+ img2.FileName);
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
            db.SaveChanges();

            return RedirectToAction("Dashboard", model);
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
                order_index = 1,
                active = 1
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

        public ActionResult AddItemToCat(int id, string item_name, string item_description, decimal item_price
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
            var Restaurant = GetRestorant();
            ViewBag.Url = Request.Url.Scheme + "://" + Request.Url.Authority +
                Request.ApplicationPath.TrimEnd('/') + "/Owner/Restaurant";
            return View(Restaurant);
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
        public ActionResult EditItem(HttpPostedFileBase item_image, item model)
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
                if (model.available == 1)
                {
                    OldItem.available = 1;
                }
                else if (model.available == 0)
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
            return RedirectToAction("Edit", new {id = model.id });
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
        public ActionResult AddExtras(int ItemId, string name, decimal price)
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
                return RedirectToAction("Edit", model);
            }
        }



        [HttpPost] // this action takes the viewModel from the modal
        public ActionResult EditExtra(int ItemId, int ExtraId, string name, decimal price)
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
        public ActionResult DeleteExtra(int @ItemId, int id)
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


        public ActionResult AddVariants(int ItemId)
        {
            ViewBag.ItemId = ItemId;
            ViewBag.ItemName = db.items.Find(ItemId).name;
            var Options = db.options.Where(o => o.item_id == ItemId).ToList();
            
            return View(Options);
        }

        [HttpPost]
        public ActionResult AddVariant(int ItemId, string options, decimal price)
        {
            var model = db.items.Find(ItemId);
            try
            {
                var Variant = new variant()
                {
                    item_id = ItemId,
                    options = options,
                    price = price,
                    qty=1,
                    image="",
                    order= 1
                };
                db.variants.Add(Variant);
                db.SaveChanges();

                List<string> Opts = options.Split(',').ToList();
                foreach (var item in Opts)
                {
                    var OptDetails = db.options_details.Where(o => o.option_name == item).FirstOrDefault();
                    variant_has_option options_Variant = new variant_has_option()
                    {
                        variant_id = Variant.id,
                        variant = Variant,
                        options_details = OptDetails,
                        option_detail_id = OptDetails.id
                    };
                    db.variant_has_option.Add(options_Variant);
                }
                db.SaveChanges();
                return RedirectToAction("Edit", model);
            }
            catch (Exception e)
            {
                return RedirectToAction("Edit", model);
            }
        }

        public ActionResult EditVariants(int ItemId , int id)
        {
            ViewBag.ItemId = ItemId;
            ViewBag.ItemName = db.items.Find(ItemId).name;
            ViewBag.Id = id;
            var Variant = db.variants.Find(id);
            ViewBag.Options = Variant.options;
            ViewBag.Price = Variant.price;
            var Options = db.options.Where(o => o.item_id == ItemId).ToList();
            return View(Options);
        }

        [HttpPost] // this action takes the viewModel from the modal
        public ActionResult EditVariant(int ItemId, int id, string options, decimal price)
        {

            var model = db.items.Find(ItemId);

            if (ModelState.IsValid)
            {
                var OldVariant = db.variants.Find(id);
                OldVariant.options = options;
                OldVariant.price = price;


                var VariantsOptions = db.variant_has_option.Where(v => v.variant_id == id).ToList();
                foreach (var item in VariantsOptions)
                {
                    db.variant_has_option.Remove(item);
                    db.SaveChanges();
                }

                List<string> Opts = options.Split(',').ToList();
                foreach (var item in Opts)
                {
                    var OptDetails = db.options_details.Where(o => o.option_name == item).FirstOrDefault();
                    variant_has_option options_Variant = new variant_has_option()
                    {
                        variant_id = OldVariant.id,
                        variant = OldVariant,
                        options_details = OptDetails,
                        option_detail_id = OptDetails.id
                    };
                    db.variant_has_option.Add(options_Variant);
                }

                db.SaveChanges();
                return RedirectToAction("Edit", model);
            }
            else
            {
                return RedirectToAction("Edit", model);
            }
        }


        [HttpPost]
        public ActionResult DeleteVariant(int @ItemId, int id)
        {

            var VariantsOptions = db.variant_has_option.Where(v => v.variant_id == id).ToList();
            foreach (var item in VariantsOptions)
            {
                db.variant_has_option.Remove(item);
                db.SaveChanges();
            }

            var model = db.items.Find(ItemId);
            try
            {
                db.variants.Remove(db.variants.Find(id));
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
            var CurrentRestaurant = GetRestorant();
            var user1 = db.users.Find(CurrentRestaurant.user_id);
            return View(user1);
        }

        [HttpPost]
        public ActionResult UpdatePassword(int id,string oldPassword, string password, string confirmation)
        {
            if (password!=confirmation)
            {
                TempData["Msg"] = "New password and confirmation not match";
                return RedirectToAction("Profile", "Owner");
            }

            //////Update User
            user OldUser1 = db.users.Find(id);
            if (OldUser1.password!=oldPassword)
            {
                TempData["Msg"] = "Your password is not correct";
                return RedirectToAction("Profile", "Owner");
            }

            OldUser1.password = password;
            OldUser1.updated_at = DateTime.Now;


            //// update AspNetUser
            UserStore<ApplicationUser> store =
                            new UserStore<ApplicationUser>(new ApplicationDbContext());
            ApplicationUser OldUser = UserManager.FindById(User.Identity.GetUserId());
            var newPasswordHash = UserManager.PasswordHasher.HashPassword(password);
            store.SetPasswordHashAsync(OldUser, newPasswordHash);
            UserManager.Update(OldUser);
            db.SaveChanges();
            TempData["Msg"] = "Your Password changed successfully";
            return RedirectToAction("Profile", "Owner");
        }


        [HttpPost]
        public ActionResult UpdateProfile(int id , string name , string email , string phone)
        {

            //// Update Restaurant 
            var OldRestaurant = GetRestorant();
            OldRestaurant.subdomain = email;
            OldRestaurant.updated_at = DateTime.Now;
            OldRestaurant.phone = phone;

            //////Update User
            user OldUser1 = db.users.Find(id);
            OldUser1.email = email;
            OldUser1.name = name;
            OldUser1.phone = phone;
            OldUser1.updated_at = DateTime.Now;


            //// update AspNetUser
            ApplicationUser OldUser = UserManager.FindById(User.Identity.GetUserId());
            OldUser.Email = email;
            OldUser.UserName = name;
            OldUser.PhoneNumber = phone;
            UserManager.Update(OldUser);

    
            db.SaveChanges();
            return RedirectToAction("Profile", "Owner");
        }

        [AllowAnonymous]
        public ActionResult Restaurant(string name)
        {
            return View(db.restorants.Where(r => r.name == name).FirstOrDefault());
        }

        [AllowAnonymous]
        public ActionResult ItemModal(int? id  )
        {
            var item = db.items.Find(id);
            return View(item);
        }

        public ActionResult EditOptions(int id)
        {
            var Options = db.options.Where(o => o.item_id == id).ToList();
            ViewBag.ItemId = id;
            ViewBag.ItemName = db.items.Find(id).name;
            return View(Options);
        }

        public ActionResult AddOption(int id)
        {
            ViewBag.ItemId = id;
            ViewBag.ItemName = db.items.Find(id).name;
            return View();
        }

        [HttpPost]
        public ActionResult AddOptions(int ItemId,string name , string options)
        {
            
            option NewOption = new option()
            {
                item_id = ItemId,
                name = name,
                options = options,
            };
            db.options.Add(NewOption);
            db.SaveChanges();

            List<string> Opts = options.Split(',').ToList();
            foreach (var item in Opts)
            {
                options_details options_Details = new options_details()
                {
                    option_id = NewOption.id,
                    option_name= item,
                    option = NewOption,
                };
                db.options_details.Add(options_Details);
            }
            db.SaveChanges();
            //var Options = db.options.Where(o => o.item_id == ItemId).ToList();
            return RedirectToAction("EditOptions", new { id = ItemId });
        }

        public ActionResult EditOption(int id)
        {
            var Op = db.options.Find(id);
            return View(Op);
        }

        [HttpPost]
        public ActionResult EditOp(int id , int ItemId,string name , string options)
        {
            var OldOp = db.options.Find(id);
            OldOp.name = name;
            OldOp.options = options;
            db.SaveChanges();
            var OptDetails = db.options_details.Where(o => o.option_id == OldOp.id).ToList();
            foreach (var item in OptDetails)
            {
                db.options_details.Remove(item);
            }

            db.SaveChanges();

            List<string> Opts = options.Split(',').ToList();
            foreach (var item in Opts)
            {
                options_details options_Details = new options_details()
                {
                    option_id = OldOp.id,
                    option_name = item,
                    option = OldOp,
                };
                db.options_details.Add(options_Details);
            }

            db.SaveChanges();

            return RedirectToAction("EditOptions",new {id = ItemId });
        }
        [HttpPost]
        public ActionResult DeleteOption(int id)
        {
            var OptDetails = db.options_details.Where(o => o.option_id == id).ToList();
            foreach (var item in OptDetails)
            {
                db.options_details.Remove(item);
            }
            var op = db.options.Find(id);
            db.options.Remove(op);
            db.SaveChanges();
            //return View();
            return RedirectToAction("EditOptions", new { id =op.item_id  });
        }
    }
}