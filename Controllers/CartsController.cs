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
    public class CartsController : Controller
    {
        private readonly UserContext _context;
        private readonly UserManager<BookShopUser> _userManager;
        public CartsController(UserContext context, UserManager<BookShopUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Carts
        public ActionResult Index()
        {
            string thisUserId = _userManager.GetUserId(HttpContext.User);
            return View(_context.Carts.Include(b => b.Book).Include(a => a.User).Where(c => c.User.Id == thisUserId));
        }

        public ActionResult BackToLogin()
        {
            return View();
        }


      

        public async Task<IActionResult> AddToCart(string isbn, int quantity = 1)
        {
            string thisUserId = _userManager.GetUserId(HttpContext.User);
            var userCart = await _context.Books
                   .FindAsync(isbn);


            Cart myCart = new Cart()
            {
                UId = thisUserId,
                BookIsbn = isbn,
                Quantity = quantity
            };
            Cart fromDb = _context.Carts.FirstOrDefault(c => c.UId == thisUserId && c.BookIsbn == isbn);
            //if not existing (or null), add it to cart. If already added to Cart before, ignore it.
            if (fromDb == null)
            {
                _context.Add(myCart);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Checkout()
        {
            string thisUserId = _userManager.GetUserId(HttpContext.User);
            List<Cart> myDetailsInCart = await _context.Carts
                .Where(c => c.UId == thisUserId)
                .Include(c => c.Book)
                .ToListAsync();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    //Step 1: create an order
                    Order myOrder = new Order();
                    myOrder.UId = thisUserId;
                    myOrder.OrderDate = DateTime.Now;
                   
                    _context.Add(myOrder);
                    await _context.SaveChangesAsync();

                    //Step 2: insert all order details by var "myDetailsInCart"
                    foreach (var item in myDetailsInCart)
                    {
                        OrderDetail detail = new OrderDetail()
                        {
                            OrderId = myOrder.Id,
                            BookIsbn = item.BookIsbn,
                            Quantity = item.Quantity,
                        };
                        _context.Add(detail);
                    }
                    await _context.SaveChangesAsync();

                    //Step 3: empty/delete the cart we just done for thisUser
                    _context.Carts.RemoveRange(myDetailsInCart);
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Error occurred in Checkout" + ex);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: UserCarts/Edit/5


        // GET: UserCarts/Edit/5
        public async Task<IActionResult> Edit(string uid, string bid)
        {
            if (uid == null || bid == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .FirstOrDefaultAsync(m => m.UId == uid && m.BookIsbn == bid);
            return View(cart);





        }

        // POST: UserCarts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("UId,BookIsbn,Quantity,TotalPerCart")] Cart cart)
        {
            try
            {

                _context.Carts.Update(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction("Edit", new { uid = cart.UId, bid = cart.BookIsbn });
            }

        }



        // GET: UserCarts/Delete/5
        public async Task<IActionResult> Delete(string uid, string bid)
        {
            if (uid == null || bid == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.Book)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.UId == uid && m.BookIsbn == bid);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }


        // POST: UserCarts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Cart cart)
        {
            /*var cart = await _context.Carts.FindAsync(id);
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));*/

            try
            {
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction("Delete", new { uid = cart.UId, bid = cart.BookIsbn });
            }
        }


        private bool CartExists(string id)
        {
            return _context.Carts.Any(e => e.UId == id);
        }
    }
}
