using WebApplication17.DTOs;

namespace WebApplication17.Services
{
    public interface IOrderService
    {
        Task<OrderResponseDTO> CreateOrderAsync(OrderDTO dto); 
        Task<IEnumerable<OrderResponseDTO>> GetAllAsync(); 

        Task<OrderResponseDTO?> GetByIdAsync(int id); 

        Task<bool> UpdateAsync(int id, OrderDTO dto);

        Task<bool> DeleteAsync(int id);
    }
}