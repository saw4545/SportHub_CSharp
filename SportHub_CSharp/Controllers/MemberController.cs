using System;
using System.Linq;
using System.Web.Mvc;
using SportHub_CSharp.Models;

namespace SportHub_CSharp.Controllers
{
    public class MemberController : Controller
    {
        private SportHubEntities1 db = new SportHubEntities1();


        // =========================================================
        // REGISTER - GET
        // =========================================================

        [HttpGet]
        public ActionResult Register()
        {
            ViewBag.Sports = db.SPORTs
                .OrderBy(s => s.SportName)
                .ToList();

            return View();
        }


        // =========================================================
        // REGISTER - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(
            MEMBER member,
            int[] selectedSports)
        {
            if (ModelState.IsValid)
            {
                // Check duplicate email
                bool emailExists = db.MEMBERs
                    .Any(m => m.Email == member.Email);

                if (emailExists)
                {
                    ModelState.AddModelError(
                        "Email",
                        "This email address is already registered."
                    );

                    ViewBag.Sports = db.SPORTs
                        .OrderBy(s => s.SportName)
                        .ToList();

                    return View(member);
                }

                // Registration date
                member.RegisteredDate = DateTime.Now;

                // Add member
                db.MEMBERs.Add(member);

                // Add preferred sports
                if (selectedSports != null)
                {
                    foreach (int sportId in selectedSports)
                    {
                        var sport = db.SPORTs
                            .FirstOrDefault(s => s.SportID == sportId);

                        if (sport != null)
                        {
                            member.SPORTs.Add(sport);
                        }
                    }
                }

                // Save
                try
                {
                    db.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    foreach (var entityValidationError in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in entityValidationError.ValidationErrors)
                        {
                            ModelState.AddModelError(
                                validationError.PropertyName,
                                validationError.ErrorMessage
                            );

                            System.Diagnostics.Debug.WriteLine(
                                "Property: " +
                                validationError.PropertyName +
                                " | Error: " +
                                validationError.ErrorMessage
                            );
                        }
                    }

                    ViewBag.Sports = db.SPORTs
                        .OrderBy(s => s.SportName)
                        .ToList();

                    return View(member);
                }

                TempData["SuccessMessage"] =
                    "Registration successful. Please sign in.";

                return RedirectToAction("Login");
            }

            ViewBag.Sports = db.SPORTs
                .OrderBy(s => s.SportName)
                .ToList();

            return View(member);
        }


        // =========================================================
        // LOGIN - GET
        // =========================================================

        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }


        // =========================================================
        // LOGIN - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(
            string Email,
            string Password,
            string returnUrl)
        {
            // Validate empty fields
            if (string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password))
            {
                ViewBag.ErrorMessage =
                    "Please enter your email and password.";

                ViewBag.ReturnUrl = returnUrl;

                return View();
            }

            // Find member
            var member = db.MEMBERs.FirstOrDefault(m =>
                m.Email == Email &&
                m.Password == Password
            );

            // Login successful
            if (member != null)
            {
                Session["MemberID"] = member.MemberID;
                Session["MemberName"] = member.Name;
                Session["MemberEmail"] = member.Email;

                // Return to requested page
                if (!string.IsNullOrEmpty(returnUrl) &&
                    Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                // Otherwise Home
                return RedirectToAction("Index", "Home");
            }

            // Login failed
            ViewBag.ErrorMessage =
                "Invalid email or password.";

            ViewBag.ReturnUrl = returnUrl;

            return View();
        }


        // =========================================================
        // FORGOT PASSWORD - GET
        // =========================================================

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }


        // =========================================================
        // FORGOT PASSWORD - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string Email)
        {
            // Check empty email
            if (string.IsNullOrWhiteSpace(Email))
            {
                ViewBag.ErrorMessage =
                    "Please enter your registered email address.";

                return View();
            }

            // Find member by email
            var member = db.MEMBERs
                .FirstOrDefault(m => m.Email == Email);

            // Email does not exist
            if (member == null)
            {
                ViewBag.ErrorMessage =
                    "No SportHub account was found with this email address.";

                return View();
            }

            // Email exists
            // Send user to Reset Password page
            return RedirectToAction(
                "ResetPassword",
                new { email = member.Email }
            );
        }


        // =========================================================
        // RESET PASSWORD - GET
        // =========================================================

        [HttpGet]
        public ActionResult ResetPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return RedirectToAction("ForgotPassword");
            }

            // Check whether email exists
            var member = db.MEMBERs
                .FirstOrDefault(m => m.Email == email);

            if (member == null)
            {
                TempData["ErrorMessage"] =
                    "The email address was not found.";

                return RedirectToAction("ForgotPassword");
            }

            ViewBag.Email = member.Email;

            return View();
        }


        // =========================================================
        // RESET PASSWORD - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(
            string Email,
            string NewPassword,
            string ConfirmPassword)
        {
            // Validate email
            if (string.IsNullOrWhiteSpace(Email))
            {
                ViewBag.ErrorMessage =
                    "Email address is required.";

                return View();
            }

            // Validate new password
            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                ViewBag.ErrorMessage =
                    "Please enter a new password.";

                ViewBag.Email = Email;

                return View();
            }

            // Minimum password length
            if (NewPassword.Length < 6)
            {
                ViewBag.ErrorMessage =
                    "Password must contain at least 6 characters.";

                ViewBag.Email = Email;

                return View();
            }

            // Confirm password
            if (string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ViewBag.ErrorMessage =
                    "Please confirm your new password.";

                ViewBag.Email = Email;

                return View();
            }

            // Check passwords match
            if (NewPassword != ConfirmPassword)
            {
                ViewBag.ErrorMessage =
                    "New password and confirm password do not match.";

                ViewBag.Email = Email;

                return View();
            }

            // Find member
            var member = db.MEMBERs
                .FirstOrDefault(m => m.Email == Email);

            if (member == null)
            {
                ViewBag.ErrorMessage =
                    "No account was found with this email address.";

                return View();
            }

            // Update password
            member.Password = NewPassword;

            db.SaveChanges();

            // Success message
            TempData["SuccessMessage"] =
                "Your password has been reset successfully. Please sign in.";

            return RedirectToAction("Login");
        }


        // =========================================================
        // LOGOUT
        // =========================================================

        public ActionResult Logout()
        {
            Session.Clear();

            TempData["SuccessMessage"] =
                "You have been logged out successfully.";

            return RedirectToAction("Index", "Home");
        }


        // =========================================================
        // DISPOSE
        // =========================================================

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}