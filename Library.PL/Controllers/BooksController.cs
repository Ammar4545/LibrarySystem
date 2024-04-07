using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Library.DAL.Entities;
using Library.PL.Data;
using Microsoft.AspNetCore.Authorization;
using Library.BLL.Services.Contracts;
using Library.BLL.Services;
using Microsoft.AspNetCore.Hosting;
using Library_Utility;
using Library.PL.ViewModel;

namespace Library.PL.Controllers
{
   
    public class BooksController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public BooksController(IBookService bookService, IWebHostEnvironment webHostEnvironment)
        {
            _bookService = bookService;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Books
        public IActionResult Index()
        {
            if (TempData.ContainsKey("ErrorMessage"))
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }
            return View(_bookService.GetAll("Books"));
        }

        //// GET: Books/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var books = await _context.Books
        //        .FirstOrDefaultAsync(m => m.BookId == id);
        //    if (books == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(books);
        //}

        // GET: Books/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create( Books books)
        {
            //var errors = ModelState
            //     .Where(x => x.Value.Errors.Count > 0)
            //     .Select(x => new { x.Key, x.Value.Errors })
            //     .ToArray();            
             var file = HttpContext.Request.Form.Files;
             string webRootPath = _webHostEnvironment.WebRootPath;

             string uplaod = webRootPath + GlobalConst.ImagePath;
             string filename = Guid.NewGuid().ToString();
             string extention = Path.GetExtension(file[0].FileName);

             using (var filestream = new FileStream(Path.Combine(uplaod, filename + extention), FileMode.Create))
             {
                 file[0].CopyTo(filestream);
             }
             books.Image = filename + extention;

             _bookService.Add(books,"Books");
            
             return RedirectToAction(nameof(Index));
            
            //return View(books);
        }

        // GET: Books/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = _bookService.GetById("Books", id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit
            (int id, [Bind("BookId,Title,Author,Genre,ISBN,TotalCopies,AvailableCopies")] Books book)
        {
            if (id != book.BookId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _bookService.Update(book);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BooksExists(book.BookId))
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
            return View(book);
        }

        // GET: Books/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = _bookService.GetById("Books", id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var book = _bookService.GetById("Books", id);
            if (book != null)
            {
                if (_bookService.DeleteRecord("Books", id) == true)
                    return RedirectToAction(nameof(Index));

            }
            TempData["ErrorMessage"] = "This Book is still borrowed by Users, so it cannot be deleted";

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult BorrowBook(int? bookId)
        {
            var loggedInUser=HttpContext.Session.GetInt32("userId").Value;
            if (loggedInUser==null)
            {
                return NotFound();
            }
            if (bookId == null)
            {
                return NotFound();
            }
            if(_bookService.BorrowBook(loggedInUser, bookId.Value))
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }
        
        private bool BooksExists(int id)
        {
            if (_bookService.GetById("Books", id)!=null)
            {
                return true;
            }
            return false;
        }
    }
}
