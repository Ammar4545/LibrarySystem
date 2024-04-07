using Library.BLL.Services.Contracts;
using Library.DAL.Entities;
using Library.DAL.Repositories.Interface;
using Library.PL.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace Library.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AccountController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (_authService.CheckUserExists(model.UserName, model.Password))
            {

                var user = _authService.CheckUser(model.UserName, model.Password);

                if (user != null)
				{
					HttpContext.Session.SetString("userName", user.UserName);
					HttpContext.Session.SetInt32("userId", user.UserId);
					return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}
