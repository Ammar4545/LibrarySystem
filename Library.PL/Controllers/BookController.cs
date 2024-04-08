using Library.BLL.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Library.PL.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }
        public IActionResult Index(int bookId)
        {
            var loggedInUserId = HttpContext.Session.GetInt32("userId");
            if (loggedInUserId != null)
            {
                _bookService.BorrowBook(loggedInUserId.Value, bookId);
            }
            return View();
        }
        public IActionResult ReturnBook(int bookId)
        {
            var loggedInUserId = HttpContext.Session.GetInt32("userId");
            if (loggedInUserId != null)
            {
                _bookService.ReturnBook(loggedInUserId.Value, bookId);
            }
            return View();
        }
        //public IActionResult GetBorrowedBooksForUser()
        //{
        //    var loggedInUserId = HttpContext.Session.GetInt32("userId");
        //    var loggedInUserName = HttpContext.Session.GetString("userName");
        //    if (loggedInUserId != null)
        //    {
        //        var book =_bookService.GetBorrowedBooksByUser(loggedInUserName);
        //    }
        //    return View();
        //}

        public IActionResult Search(string query)
        {
            var books = _bookService.SearchBooks(query);
            return View();
        }

    }
}
