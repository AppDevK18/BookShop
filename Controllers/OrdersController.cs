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

namespace BookShop.Controllers
{
    public class OrdersController : Controller
    {
        private readonly UserContext _context;
        private readonly UserManager<BookShopUser> _userManager;
        public OrdersController(UserContext context, UserManager<BookShopUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
             var userid = _userManager.GetUserId(HttpContext.User);
            var userContext = _context.Orders
                .Include(q => q.OrderDetails).ThenInclude(a => a.Book)
                .Include(o => o.User)
                .Where(r => r.UId == userid);
               
            

            return View(await userContext.ToListAsync());
        }

        
    }
}
