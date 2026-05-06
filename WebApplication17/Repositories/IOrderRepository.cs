using WebApplication17.DTOs;
using WebApplication17.Models;

namespace WebApplication17.Repositories
{
    public interface IOrderRepository
    {
        
        Task<Order> CreateAsync(OrderDTO dto); 

       
        Task<IEnumerable<Order>> GetAllAsync(); 
        
        Task<Order?> GetByIdAsync(int id); 

        
        Task<bool> UpdateAsync(int id, OrderDTO dto);

        
        Task<bool> DeleteAsync(int id); 
    }
}