using BookShop.Areas.Identity.Data;
using BookShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BookShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<BookShopUser> _userManager;
        private readonly int _recordsPerPage = 32;

        public HomeController(ILogger<HomeController> logger, IEmailSender emailSender, UserManager<BookShopUser> userManager, UserContext context)
        {
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
            _userManager = userManager;
        }
        [Authorize(Roles = "Customer")]
        public IActionResult ForCustomerOnly()
        {
            ViewBag.message = "This is for Customer only! Hi " + _userManager.GetUserName(HttpContext.User);
            return View("Views/Home/Index.cshtml");
        }

        [Authorize(Roles = "Seller")]
        public IActionResult ForSellerOnly()
        {
            ViewBag.message = "This is for Store Owner only!";
            return View("Areas/Seller/View/Home/Index.cshtml");
        }


        public async Task<IActionResult> Index(int categoryInt = 0, int id = 0, string StringSearch = "")
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["CurrentFilter"] = StringSearch;
            ViewData["CurrentCategories"] = categoryInt;


            var books = from b in _context.Books
                        select b;

            ViewBag.CurrentPage = id;

            if (StringSearch == null)

            {
                return View(books);

            }

            else
            {
                if (categoryInt != 0)
                {
                    books = books.Include(b => b.Category)
                       /* .Include(b => b.User)*/
                        .Where(s => s.Title.Contains(StringSearch) || s.Category.Name.Contains(StringSearch) || s.Author.Contains(StringSearch))
                        .Where(t => t.CategoryId == categoryInt);

                    int numOfFilteredBook = books.Count();
                    ViewBag.NumberOfPages = (int)Math.Ceiling((double)numOfFilteredBook / _recordsPerPage);
                    List<Book> booksList = await books.Skip(id * _recordsPerPage)
                           .Take(_recordsPerPage).ToListAsync();
                    return View(booksList);
                }
                else
                {
                    books = books.Include(b => b.Category)
                     /*  .Include(b => b.User)*/
                       .Where(s => s.Title.Contains(StringSearch) || s.Category.Name.Contains(StringSearch) || s.Author.Contains(StringSearch));

                    int numOfFilteredBook = books.Count();
                    ViewBag.NumberOfPages = (int)Math.Ceiling((double)numOfFilteredBook / _recordsPerPage);
                    List<Book> booksList = await books.Skip(id * _recordsPerPage)
                           .Take(_recordsPerPage).ToListAsync();
                    return View(booksList);
                }

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