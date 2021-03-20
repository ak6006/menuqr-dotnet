using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using EgyptMenu.Models;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNet.Identity.EntityFramework;

namespace EgyptMenu.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        ApplicationDbContext AuthDB = new ApplicationDbContext();
        private Entities db = new Entities();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl , string lang = "en")
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToAction("Index", "Home");
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register(string name , string email , string phone)
        {
            
            ViewBag.Name = name;
            ViewBag.Email = email;
            ViewBag.Number = phone;
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.OwnerName,
                    Email = model.Email,
                    EmailConfirmed = true
                };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //var userStore = new UserStore<ApplicationUser>(AuthDB);
                    //var userManager = new UserManager<ApplicationUser>(userStore);
                    try
                    {
                        UserManager.AddToRole(user.Id, "Owner");
                    }
                    catch (Exception e)
                    {
                        return View(e.Message);
                    }
                    user.EmailConfirmed = true;
                    user.PhoneNumberConfirmed = true;
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    user user1 = new user()
                    {
                        email = model.Email,
                        password = model.Password,
                        name = model.OwnerName,
                        phone = model.OwnerPhone,
                        api_token = model.Email,
                        active = 1,
                        plan_status = "",
                        cancel_url = "",
                        update_url = "",
                        checkout_id = "",
                        subscription_plan_id = "",
                        stripe_account = "",
                        birth_date = "",
                        created_at = DateTime.Now,
                        updated_at = DateTime.Now
                    };
                    try
                    {
                        db.users.Add(user1);
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        return View(e.Message);
                    }


                    restorant restorant1 = new restorant()
                    {
                        name = model.RestaurantName,
                        subdomain = model.Email,
                        user_id = user1.id,
                        logo = "cover.jpg",
                        cover = "cover.jpg",
                        active = 1,
                        lat = "29.9751576",
                        lng = "31.141099",
                        address = "Arkan plaza, El-Bostan, Al Sheikh Zayed, Giza Governorate, Egypt",
                        phone = model.OwnerPhone,
                        minimum = "",
                        description = "So tasty and delicious",
                        fee = 1,
                        static_fee = 1,
                        radius = "",
                        is_featured = 1,
                        created_at = DateTime.Now,
                        updated_at = DateTime.Now
                    };
                    try
                    {
                        db.restorants.Add(restorant1);
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        return View(e.Message);
                    }

                    var category = new category()
                    {
                        restorant = restorant1,
                        restorant_id = restorant1.id,
                        name = "Desserts",
                        order_index = 1,
                        active = 1,
                        order = 3
                    };
                    var category1 = new category()
                    {
                        restorant = restorant1,
                        restorant_id = restorant1.id,
                        name = "Main Course",
                        order_index = 1,
                        active = 1,
                        order = 2
                    };
                    var category2 = new category()
                    {
                        restorant = restorant1,
                        restorant_id = restorant1.id,
                        name = "Starter",
                        order_index = 1,
                        active = 1,
                        order = 1
                    };
                    var category3 = new category()
                    {
                        restorant = restorant1,
                        restorant_id = restorant1.id,
                        name = "Beverages",
                        order_index = 1,
                        active = 1,
                        order = 4
                    };
                    try
                    {
                        db.categories.Add(category2);
                        db.categories.Add(category1);
                        db.categories.Add(category);
                        db.categories.Add(category3);
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        return View(e.Message);
                    }

                    var Item = new item()
                    {
                        name = "Hot Dog Pizza",
                        price = 55,
                        category_id = category1.id,
                        description = "Pizza with hot dog pieces",
                        image = "HotDogPizza.jpg",
                        available = 1
                    };
                    var Item1 = new item()
                    {
                        name = "Sausage Pizza",
                        price = 50,
                        category_id = category1.id,
                        description = "Pizza with Sausage pieces",
                        image = "SausagePizza.png",
                        available = 1
                    };

                    var Item2 = new item()
                    {
                        name = "Meat Pizza",
                        price = 45,
                        category_id = category1.id,
                        description = "Pizza with Meat pieces",
                        image = "MeatPizza.jpg",
                        available = 1
                    };

                    var Item3 = new item()
                    {
                        name = "Liver",
                        price = 40,
                        category_id = category1.id,
                        description = "Liver Barbecue meal",
                        image = "LiverBarbeque.jpg",
                        available = 1
                    };
                    var Item4 = new item()
                    {
                        name = "chicken",
                        price = 50,
                        category_id = category1.id,
                        description = "chicken Barbecue meal",
                        image = "ChickenBarbicue.jpg",
                        available = 1
                    };

                    var Item5 = new item()
                    {
                        name = "Meat",
                        price = 47,
                        category_id = category1.id,
                        description = "Meat Barbecue meal",
                        image = "MeatBarbicue.jpg",
                        available = 1
                    };

                    var Item6 = new item()
                    {
                        name = "seezar salad",
                        price = 20,
                        category_id = category2.id,
                        description = "Salade with chicken pieces",
                        image = "SeezarSalad.jpg",
                        available = 1
                    };
                    var Item7 = new item()
                    {
                        name = "Green Salad",
                        price = 10,
                        category_id = category2.id,
                        description = "Vegetables Salad",
                        image = "GreenSalad.jpg",
                        available = 1
                    };

                    var Item8 = new item()
                    {
                        name = "Tahini salad",
                        price = 12,
                        category_id = category2.id,
                        description = "Tahini Salad",
                        image = "TahiniSalad.jpg",
                        available = 1
                    };

                    var Item9 = new item()
                    {
                        name = "Vegetables Soup",
                        price = 12,
                        category_id = category2.id,
                        description = "Vegetables Soup",
                        image = "VegSoup.jpg",
                        available = 1
                    };

                    var Item10 = new item()
                    {
                        name = "Kunafa with mango",
                        price = 29,
                        category_id = category.id,
                        description = "Kunafa with mango",
                        image = "KunMango.jpg",
                        available = 1
                    };

                    var Item11 = new item()
                    {
                        name = "Cheese Cake",
                        price = 29,
                        category_id = category.id,
                        description = "Cheese Cake",
                        image = "CheeseCake.jpg",
                        available = 1
                    };

                    var Item12 = new item()
                    {
                        name = "Seven UP Can",
                        price = 29,
                        category_id = category3.id,
                        description = "Cold 7UP",
                        image = "7up.jpg",
                        available = 1
                    };

                    var Item13 = new item()
                    {
                        name = "Hot Tea",
                        price = 29,
                        category_id = category3.id,
                        description = "Lipton Tea",
                        image = "LiptonTea.jpg",
                        available = 1
                    };

                    db.items.Add(Item);
                    db.items.Add(Item1);
                    db.items.Add(Item2);
                    db.items.Add(Item3);
                    db.items.Add(Item4);
                    db.items.Add(Item5);
                    db.items.Add(Item6);
                    db.items.Add(Item7);
                    db.items.Add(Item8);
                    db.items.Add(Item9);
                    db.items.Add(Item10);
                    db.items.Add(Item11);
                    db.items.Add(Item12);
                    db.items.Add(Item13);
                    db.SaveChanges();
                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    //return View("ForgotPasswordConfirmation");
                    return View(model);

                }
                // Replace sender@example.com with your "From" address. 
                // This address must be verified with Amazon SES.
                String FROM = "info@teamigroup.com";
                String FROMNAME = "QR Menu Maker";

                // Replace recipient@example.com with a "To" address. If your account 
                // is still in the sandbox, this address must be verified.
                String TO = model.Email;

                // Replace smtp_username with your Amazon SES SMTP user name.
                String SMTP_USERNAME = "info@teamigroup.com";

                // Replace smtp_password with your Amazon SES SMTP user name.
                String SMTP_PASSWORD = "ti10203040$";

                // (Optional) the name of a configuration set to use for this message.
                // If you comment out this line, you also need to remove or comment out
                // the "X-SES-CONFIGURATION-SET" header below.
                String CONFIGSET = "ConfigSet";

                // If you're using Amazon SES in a region other than US West (Oregon), 
                // replace email-smtp.us-west-2.amazonaws.com with the Amazon SES SMTP  
                // endpoint in the appropriate AWS Region.
                String HOST = "mail.teamigroup.com";

                // The port you will connect to on the Amazon SES SMTP endpoint. We
                // are choosing port 587 because we will use STARTTLS to encrypt
                // the connection.
                int PORT = 587;
                String Pass = "";
                string Name = "";

                // The subject line of the email
                try
                {
                     Pass = db.users.Where(u => u.email == model.Email).FirstOrDefault().password.ToString();
                     Name = db.users.Where(u => u.email == model.Email).FirstOrDefault().name.ToString();
                }
                catch (Exception)
                {
                    return View(model);
                }


                // The body of the email
                String BODY = "Your User Name is : " + Name + " and Your password is : " + Pass + " Please don't forget it again";
                //"<h1>Amazon SES Test</h1>" +
                //"<p>This email was sent through the " +
                //"<a href='https://aws.amazon.com/ses'>Amazon SES</a> SMTP interface " +
                //"using the .NET System.Net.Mail library.</p>";

                // Create and build a new MailMessage object
                MailMessage message = new MailMessage();
                message.IsBodyHtml = true;
                message.From = new MailAddress(FROM, FROMNAME);
                message.To.Add(new MailAddress(TO));
                message.Subject = "Account Recovery";
                message.Body = BODY;
                // Comment or delete the next line if you are not using a configuration set
                //message.Headers.Add("X-SES-CONFIGURATION-SET", CONFIGSET);
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                using (var client = new System.Net.Mail.SmtpClient(HOST, PORT))
                {
                    // Pass SMTP credentials
                    client.Credentials =
                        new NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);

                    // Enable SSL encryption
                    client.EnableSsl = true;

                    // Try to send the message. Show status in console.
                    try
                    {
                        Console.WriteLine("Attempting to send email...");
                        client.Send(message);
                        Console.WriteLine("Email sent!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("The email was not sent.");
                        Console.WriteLine("Error message: " + ex.Message);
                    }
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                return RedirectToAction("login", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}