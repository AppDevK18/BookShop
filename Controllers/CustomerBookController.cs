/*#nullable disable
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

namespace BookShop.Controllers
{
    public class CustomerBookController : Controller
    {
        private readonly UserContext _context;
        private readonly UserManager<BookShopUser> _userManager;

        public CustomerBookController(UserContext context, UserManager<BookShopUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Book
        public async Task<IActionResult> Index()
        {
            var userContext = _context.Books.Include(b => b.Category).Include(b => b.User);
            return View(await userContext.ToListAsync());
        }

        // GET: Book/Details/5
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

        private bool BookExists(string id)
        {
            return _context.Books.Any(e => e.Isbn == id);
        }
    }
}
*/