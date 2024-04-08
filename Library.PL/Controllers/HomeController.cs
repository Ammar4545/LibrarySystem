using Library.BLL.Services.Contracts;
using Library.DAL.Entities;
using Library.DAL.Repositories.Interface;
using Library.PL.Models;
using Library.PL.ViewModel;
using Library_Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Diagnostics;
using static System.Reflection.Metadata.BlobBuilder;

namespace Library.PL.Controllers
{
    public class HomeController : Controller
    {
        public readonly IUserRepository _userRepo;
        public readonly IBookRepository _BookRepo;
        public readonly IBookService _bookService;
        public HomeController(IUserRepository userRepo, IBookRepository BookRepo, IBookService bookService)
        {
            _BookRepo = BookRepo;
            _userRepo = userRepo;
            _bookService = bookService;
        }

        
        [HttpGet]
        public IActionResult Index(string? query)
        {
            if (query is null)
            {
                var books = _bookService.GetAll("Books");
                var booksVMToReturn = new BooksVM()
                {
                    Books= books,
                };
                return View(booksVMToReturn);
            }
            
            var filteredBooks = _bookService.SearchBooks(query);
            var filteredbooksVMToReturn = new BooksVM()
            {
                Books = filteredBooks,
            };
            return View(filteredbooksVMToReturn);

            
        }
        public IActionResult Details(int id)
        {
            var book = _bookService.GetById("Books", id);

            return View(book);
        }

        [HttpPost, ActionName("Borrow")]
        [ValidateAntiForgeryToken]
        public IActionResult Borrow(int bookId)
        {
            var userId = HttpContext.Session.GetInt32("userId");
            try
            {
                if (_bookService.BorrowBook(userId.Value, bookId)==true)
                {
                    TempData["Message"] = "Book Borrowing Successfuly";
                    return RedirectToAction("Index", "Home");
                }
                TempData["Message"] = "Erorr During Borrowing";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                TempData["Message"] = "Erorr During Borrowing";
                return RedirectToAction("Index", "Home");
            }
        }


        [HttpGet]
        public IActionResult AllBooksForUser()
        {
            var userId = HttpContext.Session.GetInt32("userId");
            var BorrowedBooksForUser = _bookService.GetBorrowedBooksByUser(userId.Value);
            var booksToReturn = new UserBooksVM()
            {
                Books = BorrowedBooksForUser,
            };
            return View(booksToReturn);
        }
        [HttpGet]
        public IActionResult BorrowedBookDetails(int id)
        {
            var book = _bookService.GetById("Books", id);

            return View(book);
        }
        [HttpPost]
        public IActionResult ReturnBook(int bookId)
        {
            var userId = HttpContext.Session.GetInt32("userId");
            try
            {
                if (_bookService.ReturnBook(userId.Value, bookId) == true)
                {
                    TempData["Message"] = "Book Returned Successfuly";
                    return RedirectToAction("AllBooksForUser", "Home");
                }
                TempData["Message"] = "Erorr During Returning";
                return RedirectToAction("AllBooksForUser", "Home");
            }
            catch (Exception)
            {
                TempData["Message"] = "Erorr During Returning";
                return RedirectToAction("AllBooksForUser", "Home");
            }
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
