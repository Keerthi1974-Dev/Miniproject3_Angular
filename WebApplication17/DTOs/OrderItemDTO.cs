using System.ComponentModel.DataAnnotations;

namespace WebApplication17.DTOs
{
    public class OrderItemDTO
    {
        [Required]
        public int ProductId { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; }

        [Range(0, 1000000)]
        public decimal Price { get; set; } 

        public string? ImageUrl { get; set; }
    }
}