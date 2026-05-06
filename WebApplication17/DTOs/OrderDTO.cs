namespace WebApplication17.DTOs
{
    public class OrderDTO
    {
        public int UserId { get; set; }
        public List<OrderItemDTO> Items { get; set; } = new();
    }

    
}