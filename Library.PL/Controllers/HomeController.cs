using Library.DAL.Entities;
using Library.DAL.Repositories.Interface;
using Library.PL.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Library.PL.Controllers
{
    public class HomeController : Controller
    {
        public readonly IUserRepository _userRepo;
        public readonly IBookRepository _BookRepo;
        public HomeController(IUserRepository userRepo, IBookRepository BookRepo)
        {
            _BookRepo = BookRepo;
            _userRepo = userRepo;
        }
        
        //public IActionResult Index(string userName, string email, string password, string contactNumber, string address)
        //{
        //    var user = new Users
        //    {
        //        UserName = userName,
        //        Email = email,
        //        Password = password,
        //        Phone = contactNumber,
        //        Address = address
        //    };
        //    var users = _userRepo.Add(user, "Users");
        //    Console.WriteLine(users);
        //    return View();
        //}
        public IActionResult Index(int id)
        {
            var loggedInUserId = HttpContext.Session.GetInt32("userId");
             _userRepo.DeleteRecord("Users", loggedInUserId.Value);
            Console.WriteLine();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
