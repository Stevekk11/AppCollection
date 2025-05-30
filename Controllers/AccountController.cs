using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppCollection.Models;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AppCollection.Controllers;
/// <summary>
/// Controller responsible for handling user authentication and registration.
/// </summary>
public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the AccountController.
    /// </summary>
    /// <param name="context">The database context for user authentication.</param>
    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles user registration requests.
    /// </summary>
    /// <param name="model">The login model containing registration information.</param>
    /// <returns>Redirects to the Login page on success or returns the view with errors.</returns>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public IActionResult Register(Login model)
    {
        if (ModelState.IsValid)
        {
            if (_context.Logins.Any(u => u.Username == model.Username))
            {
                ModelState.AddModelError("Username", "Username already exists.");
                return View(model);
            }

            var user = new Login
            {
                Username = model.Username,
                Password = PasswordHelper.HashPassword(model.Password),
                Usertype = model.Usertype
            };

            _context.Logins.Add(user);
            _context.SaveChanges();
            return RedirectToAction("Login");
        }

        return View(model);
    }

    /// <summary>
    /// Authenticates user login attempts.
    /// </summary>
    /// <param name="model">The login model containing credentials.</param>
    /// <returns>Redirects to Home page on successful login or returns the view with errors.</returns>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(Login model)
    {
        if (ModelState.IsValid)
        {
            var user = _context.Logins
                .FirstOrDefault(u => u.Username == model.Username && u.Usertype == model.Usertype);
            if (user != null && PasswordHelper.VerifyPassword(model.Password, user.Password))
            {
                // Set authentication cookie
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, user.Username),
                    new("Usertype", user.Usertype.ToString())
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
        }

        return View(model);
    }

    /// <summary>
    /// Logs out the currently authenticated user and clears their session.
    /// </summary>
    /// <returns>Redirects to the Home page after the user is logged out.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }


    /// <summary>
    /// Displays the login page.
    /// </summary>
    /// <returns>The login view.</returns>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login()
    {
        return View();
    }

    /// <summary>
    /// Displays the registration page.
    /// </summary>
    /// <returns>The registration view.</returns>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        return View();
    }
}