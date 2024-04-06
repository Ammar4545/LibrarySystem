using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Library.DAL.Entities;
using Library.PL.Data;
using Library.DAL.Repositories.Interface;
using Library.BLL.Services.Contracts;
using Library.BLL.Services;
using System.Reflection.Metadata.Ecma335;

namespace Library.PL.Controllers
{
    public class UsersController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public UsersController(IAuthService authService, IUserService userService)
        {
            _userService = userService;
            _authService = authService;
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Users");
        }

        // GET: Users
        public IActionResult Index()
        {
            if (TempData.ContainsKey("ErrorMessage"))
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }
            return View(_userService.GetAll("Users"));
        }

        //// GET: Users/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var users = await _context.Users
        //        .FirstOrDefaultAsync(m => m.UserId == id);
        //    if (users == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(users);
        //}

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("UserId,UserName,Email,Password,Address,Phone")] Users users)
        {
            if (ModelState.IsValid)
            {
                _userService.Add(users, "Users");
                return RedirectToAction(nameof(Index));
            }
            return View(users);
        }

        // GET: Users/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users = _userService.GetById("Users", id);
            if (users == null)
            {
                return NotFound();
            }
            return View(users);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("UserId,UserName,Email,Password,Address,Phone")] Users users)
        {
            if (id != users.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _userService.Update(users);
                   
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsersExists(users.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(users);
        }

        // GET: Users/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users =  _userService.GetById("Users", id);
            if (users == null)
            {
                return NotFound();
            }

            return View(users);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var users = _userService.GetById("Users", id);
            if (users != null)
            {
                if (_userService.DeleteRecord("Users", id)==true)
                    return RedirectToAction(nameof(Index));
                           
            }
            TempData["ErrorMessage"] = "This user is still borrowing some books, so it cannot be deleted";

            return RedirectToAction(nameof(Index));
        }

        private bool UsersExists(int id)
        {
            var user = _userService.GetById("Users", id);
            if (user != null)
            {
                return true;
            }
            return false;
        }
    }
}
