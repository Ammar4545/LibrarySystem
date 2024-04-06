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

namespace Library.PL.Controllers
{
   
    public class BooksController : Controller
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
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
        public IActionResult Create
            ([Bind("BookId,Title,Author,Genre,ISBN,TotalCopies,AvailableCopies")] Books books)
        {
            if (ModelState.IsValid)
            {
                _bookService.Add(books,"Books");
             
                return RedirectToAction(nameof(Index));
            }
            return View(books);
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
