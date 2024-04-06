using Library.DAL.Entities;
using Library.DAL.Repositories.Interface;
using Library.PL.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace Library.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthRepository _authRepo;
        private readonly IUserRepository _userRepo;

        public AccountController(IAuthRepository authRepo,IUserRepository userRepo)
        {
            _authRepo = authRepo;
            _userRepo=userRepo;
        }
        public IActionResult Index(string usrName,string password )
        {
            //http://localhost:5195/account/index?usrname=hossam&password=123456

            if (_authRepo.CheckUserExists(usrName, password))
            {

                var user = _authRepo.CheckUser(usrName, password);

                if (user != null)
                {
                    HttpContext.Session.SetString("userName", user.UserName);
                    HttpContext.Session.SetInt32("userId", user.UserId);
                    return View();
                    //return RedirectToAction("Index", "Home");
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
