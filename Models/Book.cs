using BookShop.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace BookShop.Models
{
    public class Book
    {
        [Key]
        public string Isbn { get; set; }
        public string Title { get; set; }
        public int Pages { get; set; }
        public string Author { get; set; }
        public int Price { get; set; }
        public string Desc { get; set; }
        public string? ImgUrl { get; set; }
        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        public string? UId { get; set; }
        public virtual BookShopUser? User  { get; set; }
        public virtual ICollection<OrderDetail>? OrderDetails { get; set; }
        public virtual ICollection<Cart>? Carts { get; set; }

    }
}
