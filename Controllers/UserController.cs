using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using JWTAutentication.Models;
using System.Threading.Tasks;

namespace JWTAutentication.Controllers
{
    public class UserController
        : Controller
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;

        public UserController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Login, model.Password, false, false);

                if (result.Succeeded)
                {
                    return StatusCode(200);
                }
                else
                {
                    return StatusCode(401);
                }
            }

            return StatusCode(401);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.Login
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return StatusCode(200);
                }
                else
                {
                    return StatusCode(406);
                }
            }
            return StatusCode(406);
        }
    }
}