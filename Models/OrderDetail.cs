namespace BookShop.Models
{
    public class OrderDetail
    {
        public int OrderId { get; set; }
        public string BookIsbn { get; set; } = null!;
        public int Quantity { get; set; }
        public virtual Book? Book { get; set; }
        public virtual Order? Order { get; set; }
    }
}
