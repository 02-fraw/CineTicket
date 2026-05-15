using CineTicket.Data;
using CineTicket.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace CineTicket.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user, string Password)
        {
            if (user.Password != Password)
            {
                ViewBag.Error = "The passwords don't match !";
                return View(user);
            }

            if (_context.Users.Any(u => u.EmailAddress == user.EmailAddress))
            {
                ViewBag.Error = "This mail is already registered.";
                return View(user);
            }

            user.Role = "Member";
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
        new Claim(ClaimTypes.Email, user.EmailAddress),
        new Claim(ClaimTypes.Role, user.Role),
        new Claim("UserId", user.Id.ToString())
    };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties();

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            var user = _context.Users.FirstOrDefault(x => x.EmailAddress == Email && x.Password == Password);

            if (user != null)
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
            new Claim(ClaimTypes.Email, user.EmailAddress),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("UserId", user.Id.ToString())
        };
                string userRole = string.IsNullOrEmpty(user.Role) ? "Member" : user.Role;
                claims.Add(new Claim(ClaimTypes.Role, userRole));

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties();

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);


                if (user.Role == "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Error = "The Email or Password is incorrect !";
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult Profile()
        {
            var userIdString = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login");
            }
            int userId = int.Parse(userIdString);

            var user = _context.Users
                       .Include(u => u.Tickets)
                           .ThenInclude(t => t.Movie)
                       .Include(u => u.Comments)
                           .ThenInclude(c => c.Movie)
                       .FirstOrDefault(u => u.Id == userId);

            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(User model)
        {
            var userIdString = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdString)) return RedirectToAction("Login");

            int userId = int.Parse(userIdString);

            var user = _context.Users.Find(userId);

            if (user != null)
            {
                if (model.FirstName != null) user.FirstName = model.FirstName;
                if (model.LastName != null) user.LastName = model.LastName;
                if (model.EmailAddress != null) user.EmailAddress = model.EmailAddress;

                _context.SaveChanges();

                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
            new Claim(ClaimTypes.Email, user.EmailAddress),
            new Claim("UserId", user.Id.ToString())
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties();

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                ViewBag.Message = "The profile has been successfully updated !";
            }

            return View("Profile", user);
        }
        [HttpPost]
        public IActionResult ChangePassword(string OldPassword, string NewPassword, string ConfirmPassword)
        {
            var userIdString = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdString)) return RedirectToAction("Login");

            int userId = int.Parse(userIdString);
            var user = _context.Users.Find(userId);

            if (user != null)
            {
                if (string.IsNullOrEmpty(OldPassword) || string.IsNullOrEmpty(NewPassword) || string.IsNullOrEmpty(ConfirmPassword))
                {
                    TempData["PasswordError"] = "Please fill in all fields!";
                    return RedirectToAction("Profile");
                }

                if (user.Password.Trim() != OldPassword.Trim())
                {
                    TempData["PasswordError"] = "You entered your current password incorrectly!";
                    return RedirectToAction("Profile");
                }

                if (NewPassword.Trim() != ConfirmPassword.Trim())
                {
                    TempData["PasswordError"] = "New passwords do not match!";
                    return RedirectToAction("Profile");
                }

                if (user.Password.Trim() == NewPassword.Trim())
                {
                    TempData["PasswordError"] = "New password cannot be the same as the old one!";
                    return RedirectToAction("Profile");
                }

                user.Password = NewPassword.Trim();
                _context.SaveChanges();

                TempData["PasswordSuccess"] = "Password updated successfully!";
            }

            return RedirectToAction("Profile");
        }
        [HttpPost]
        public IActionResult DeleteComment(int commentId)
        {
            var userIdString = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdString)) return RedirectToAction("Login");

            int loggedInUserId = int.Parse(userIdString);

            var commentToDelete = _context.Comments.Find(commentId);

            if (commentToDelete != null && commentToDelete.UserId == loggedInUserId)
            {
                _context.Comments.Remove(commentToDelete);
                _context.SaveChanges();
            }

            return Redirect("/Account/Profile?tab=comments");
        }
    }
}