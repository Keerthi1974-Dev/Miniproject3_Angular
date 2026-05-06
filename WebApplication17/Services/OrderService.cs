using WebApplication17.DTOs;
using WebApplication17.Models;
using WebApplication17.Repositories;

namespace WebApplication17.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        // Create Order
        public async Task<OrderResponseDTO> CreateOrderAsync(OrderDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            // Total amount
            decimal totalAmount = dto.Items.Sum(item => item.Price * item.Quantity);

            var order = await _orderRepository.CreateAsync(dto);

            return new OrderResponseDTO
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,  // Calculate and return the total amount
                Items = order.OrderItems.Select(i => new OrderItemResponseDTO
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    Price = i.Price,
                    ImageUrl = i.Product.ImageUrl ?? string.Empty
                }).ToList()
            };
        }

        // Get All Orders
        public async Task<IEnumerable<OrderResponseDTO>> GetAllAsync()
        {
            var orders = await _orderRepository.GetAllAsync();

            return orders.Select(order => new OrderResponseDTO
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,  
                Items = order.OrderItems.Select(i => new OrderItemResponseDTO
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    Price = i.Price,
                    ImageUrl = i.Product.ImageUrl
                }).ToList()
            });
        }

        // Get Order by ID
        public async Task<OrderResponseDTO?> GetByIdAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);

            if (order == null)
                return null;

            return new OrderResponseDTO
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                TotalAmount = order.OrderItems.Sum(i => i.Price * i.Quantity),  // Calculate TotalAmount
                Items = order.OrderItems.Select(i => new OrderItemResponseDTO
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    Price = i.Price,
                    ImageUrl = i.Product.ImageUrl
                }).ToList()
            };
        }

        // Update Order
        public async Task<bool> UpdateAsync(int id, OrderDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var order = await _orderRepository.GetByIdAsync(id);

            if (order == null)
                return false;

            order.UserId = dto.UserId;
            order.OrderDate = DateTime.UtcNow;

           
            decimal totalAmount = order.OrderItems.Sum(i => i.Price * i.Quantity);
           

            // Order Items update
            foreach (var item in dto.Items)
            {
                var existingItem = order.OrderItems.FirstOrDefault(i => i.ProductId == item.ProductId);
                if (existingItem != null)
                {
                    existingItem.Quantity = item.Quantity;
                    existingItem.Price = item.Price;
                }
                else
                {
                    order.OrderItems.Add(new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = item.Price
                    });
                }
            }

            await _orderRepository.UpdateAsync(id, dto);
            return true;
        }

        // Delete Order
        public async Task<bool> DeleteAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);

            if (order == null)
                return false;

            await _orderRepository.DeleteAsync(id);
            return true;
        }
    }
}