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

namespace BookShop.Controllers
{
    public class OrderDetailsController : Controller
    {
        private readonly UserContext _context;

        public OrderDetailsController(UserContext context)
        {
            _context = context;
        }

        // GET: OrderDetails
        public async Task<IActionResult> Index(int id)
        {
            var userContext = _context.OrderDetails.Include(o => o.Book).Include(o => o.Order)
                .Where(d => d.OrderId == id);
            return View(await userContext.ToListAsync());
        }
    }
}
