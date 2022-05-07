using BookShop.Areas.Identity.Data;

namespace BookShop.Models
{
    public class Cart
    {
        public string? UId { get; set; }
        public string? BookIsbn { get; set; }
        public int Quantity { get; set; }
        public int TotalPerCart { get; set; }
        public BookShopUser? User { get; set; }
        public Book? Book { get; set; }
    }
}
