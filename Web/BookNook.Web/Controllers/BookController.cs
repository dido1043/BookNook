using BookNook.Data.DTO;
using BookNook.Services.Data.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookNook.Controllers;

[Authorize(Roles = "Admin")]
public class BookController : Controller
{
    private readonly BookService _bookService;

    public BookController(BookService bookService)
    {
        _bookService = bookService;
    }

    public async Task<IActionResult> Index()
    {
        var books = await _bookService.AllBooks();
        return View(books);
    }

    public async Task<IActionResult> Details(int id)
    {
        var book = await _bookService.GetBookById(id);
        if (book == null)
        {
            return NotFound();
        }
        return View(book);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(BookDto book)
    {
        if (!ModelState.IsValid)
        {
            return View(book);
        }
        await _bookService.AddBook(book);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var book = await _bookService.GetBookById(id);
        if (book == null)
        {
            return NotFound();
        }
        return View(book);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id, BookDto book)
    {
        if (id != book.Id)
        {
            return BadRequest();
        }
        if (!ModelState.IsValid)
        {
            return View(book);
        }
        await _bookService.UpdateBook(book);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var book = await _bookService.GetBookById(id);
        if (book == null)
        {
            return NotFound();
        }
        return View(book);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var book = await _bookService.GetBookById(id);
        if (book == null)
        {
            return NotFound();
        }

        var errorMessage = await _bookService.DeleteBook(book);

        if (errorMessage != null)
        {
            ModelState.AddModelError(string.Empty, errorMessage);
            return View(book);
        }

        return RedirectToAction(nameof(Index));
    }
}
