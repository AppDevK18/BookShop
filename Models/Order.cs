﻿using BookShop.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookShop.Models
{
    public class Order
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UId { get; set; }
        public DateTime OrderDate { get; set; }
        public int Total { get; set; }
        public BookShopUser User { get; set; }
        public virtual ICollection<OrderDetail>? OrderDetails { get; set; }

    }
}
