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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BookShop.Areas.Seller.Controllers
{
    [Area("Seller")]
    [Authorize(Roles = "Seller")]
    public class OrderDetailsController : Controller
    {
        private readonly UserContext _context;
        private readonly UserManager<BookShopUser> _userManager;
        public OrderDetailsController(UserContext context, UserManager<BookShopUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Seller/OrderDetails
        public async Task<IActionResult> Index(int id)
        {
            var userid = _userManager.GetUserId(HttpContext.User);
            var userContext = _context.OrderDetails.Include(o => o.Book).Include(o => o.Order)
                .Where(c => c.Book.UId == userid)
                .Where(f => f.OrderId == id);

            return View(await userContext.ToListAsync());
        }

      
    }
}
