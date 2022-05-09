#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookShop.Areas.Identity.Data;
using BookShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace BookShop.Areas.Seller.Controllers
{
    [Area("Seller")]
    [Authorize(Roles = "Seller")]
    public class BooksController : Controller
    {
        private readonly UserContext _context;
        private readonly UserManager<BookShopUser> _userManager;
        private readonly int _recordsPerPage = 10;



        public BooksController(UserContext context, UserManager<BookShopUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


		public async Task<IActionResult> Index(int categoryInt = 0, int id = 0, string StringSearch = "")
		{
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["CurrentFilter"] = StringSearch;
            ViewData["CurrentCategories"] = categoryInt;

            var userid = _userManager.GetUserId(HttpContext.User);
			var books = from b in _context.Books
						select b;

			ViewBag.CurrentPage = id;
/*
            if (StringSearch == null)

            {
                return View(books);
            }*/

            if (categoryInt != 0)
            {
                books = books.Include(b => b.Category)
                    .Include(b => b.User)
                    .Where(c => c.UId == userid)
                    .Where(t => t.CategoryId == categoryInt)
                    .Where(b => b.Title.Contains(StringSearch));
                int numOfFilteredBook = books.Count();
                ViewBag.NumberOfPages = (int)Math.Ceiling((double)numOfFilteredBook / _recordsPerPage);
                List<Book> booksList = await books.Skip(id * _recordsPerPage)
                       .Take(_recordsPerPage).ToListAsync();
                return View(booksList);
            }
            else
            {
                books = books.Include(b => b.Category)
                   .Include(b => b.User)
                   .Where(c => c.UId == userid)
                   .Where(b => b.Title.Contains(StringSearch));
                int numOfFilteredBook = books.Count();
                ViewBag.NumberOfPages = (int)Math.Ceiling((double)numOfFilteredBook / _recordsPerPage);
                List<Book> booksList = await books.Skip(id * _recordsPerPage)
                       .Take(_recordsPerPage).ToListAsync();
                return View(booksList);
            }
               

            



        }


		// GET: Seller/Books/Details/5
		public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Category)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Isbn == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Seller/Books/Create
        public IActionResult Create()
        {

            var userid = _userManager.GetUserId(HttpContext.User);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["Id"] = _context.Users.Where(c => c.Id == userid).FirstOrDefault().UserName;
            return View();
        }

        // POST: Seller/Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Isbn,Title,Pages,Author,Price,Desc,ImgUrl,CategoryId")] Book book, IFormFile image)
        {
           
            if (image != null)
            {
                string imgName = book.Isbn + Path.GetExtension(image.FileName);
                string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", imgName);
                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    image.CopyTo(stream);
                }
                book.ImgUrl = imgName;
            }
            else
            {
                return View(book);
            }
            var userid = _userManager.GetUserId(HttpContext.User);
            if (ModelState.IsValid)
            {
                book.UId = userid;
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", book.CategoryId);
            ViewData["Id"] = _context.Users.Where(c => c.Id == userid).FirstOrDefault().UserName;
            return View(book);
        }

        // GET: Seller/Books/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            var userid = _userManager.GetUserId(HttpContext.User);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", book.CategoryId);
            ViewData["UId"] = _context.Users.Where(c => c.Id == userid).FirstOrDefault().UserName;
            return View(book);
        }

        // POST: Seller/Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Isbn,Title,Pages,Author,Price,Desc,ImgUrl,CategoryId,UId")] Book book, IFormFile image)
        {
          /*  if (id != book.Isbn)
            {
                return NotFound();
            }*/
            if (image == null)
            {


                Book thisProduct = _context.Books.Where(p => p.Isbn == book.Isbn).AsNoTracking().FirstOrDefault();
                book.ImgUrl = thisProduct.ImgUrl;
            }


            else
            {
                
            }
            var userid = _userManager.GetUserId(HttpContext.User);
            if (ModelState.IsValid)
            {
                try
                {
                    book.UId = userid;
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Isbn))
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
           
            ViewData["CategoryId"] = new SelectList(_context.Categories.ToList(), "CategoryId", "Name", book.CategoryId);
            ViewData["Id"] = new SelectList(_context.Users.Where(c => c.Id == userid), "Id", "UserName", book.Isbn);

            return View(book);
        }

        // GET: Seller/Books/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Category)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Isbn == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Seller/Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var book = await _context.Books.FindAsync(id);
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(string id)
        {
            return _context.Books.Any(e => e.Isbn == id);
        }
    }
}
