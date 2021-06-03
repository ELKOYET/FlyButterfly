using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FlyButterfly.Models;

namespace FlyButterfly.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationContext _context;
        public AccountController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            var profs = _context.Professions.OrderBy(x => x.Name).ToArray();
            ViewBag.Professions = profs;          
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model, int Profession_ID)
        {
            if (ModelState.IsValid)
            {
                ProfessionModel prof = await _context.Professions.FindAsync(Profession_ID);
                if (prof == null)
                {
                    ModelState.AddModelError("","Не выбрана профессия");
                    var profes = _context.Professions.OrderBy(x => x.Name).ToArray();
                    ViewBag.Professions = profes;
                    return View();
                }

                UserModel user = await _context.Users.FirstOrDefaultAsync(u => u.Login == model.Login);

                if (user == null)
                {

                    // добавляем пользователя в бд
                    user = new UserModel
                    {
                        Login = model.Login,
                        Password = Crypto.SHA1(model.Password),
                        Name = model.Name,
                        Surname = model.Surname,
                        Profession = prof,
                       
                    };

                    RoleModel userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "user");
                    if (userRole != null)
                    {
                        user.Role = userRole;
                    }

                    _context.Users.Add(user);

                    await _context.SaveChangesAsync();
                    //-------------------------------

                    await Authenticate(user); // аутентификация

                    if (user.Role.Name == "admin") return RedirectToAction("AdminPanel", "Admin");
                    else if (user.Role.Name != "admin") return RedirectToAction("UserPanel", "User");
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            var profs = _context.Professions.OrderBy(x => x.Name).ToArray();
            ViewBag.Professions = profs;
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var passHash = Crypto.SHA1(model.Password);

                UserModel user = await _context.Users.FirstOrDefaultAsync( u => u.Login == model.Login && u.Password == passHash);

                if (user != null)
                {
                    await Authenticate(user); // аутентификация

                    if(user.Role.Name=="admin") return RedirectToAction("AdminPanel", "Admin");
                    else if (user.Role.Name != "admin") return RedirectToAction("UserPanel", "User");

                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }
        
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        private async Task Authenticate(UserModel user)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}
