using System;
using System.Collections.Generic;

namespace WebApplication17.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }   

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>(); // uses it for avoid null values
    }
}