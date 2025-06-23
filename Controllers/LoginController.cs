using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public class LoginController : Controller
{
    private readonly UserService _userService;

    public LoginController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View("/Views/Login/Login.cshtml");
    }

    [HttpPost]
    public async Task<IActionResult> Index(string username, string password)
    {
        var user = _userService.Authenticate(username, password);
        if (user == null)
        {
            ViewBag.Error = "Invalid username or password";
            return View("/Views/Login/Login.cshtml");
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

        return RedirectToAction("Index", "Admin");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Index");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}
