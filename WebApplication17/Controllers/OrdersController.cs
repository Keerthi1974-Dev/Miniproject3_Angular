using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using WebApplication17.DTOs;
using WebApplication17.Services;

namespace WebApplication17.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrdersController(IOrderService service)
        {
            _service = service;
        }

        // UserId extraction
        private int GetUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                throw new Exception("User ID not found in token");

            return int.Parse(userId);
        }

        private string GetUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value;
        }

        // CREATE ORDER
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderDTO dto)
        {
            if (dto == null || dto.Items == null || !dto.Items.Any())
                return BadRequest("Order items cannot be empty");

            try
            {
                dto.UserId = GetUserId(); //JWT Token

                var order = await _service.CreateOrderAsync(dto);
                return Ok(order);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var role = GetUserRole();
            var userId = GetUserId();

            var orders = await _service.GetAllAsync();

            if (role == "User")
                orders = orders.Where(o => o.UserId == userId);

            return Ok(orders);
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _service.GetByIdAsync(id);

            if (order == null)
                return NotFound("Order not found..!");

            var role = GetUserRole();
            var userId = GetUserId();

            if (role == "User" && order.UserId != userId)
                return Forbid();

            return Ok(order);
        }

        // UPDATE (Admin)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, OrderDTO dto)
        {
            var result = await _service.UpdateAsync(id, dto);

            if (!result)
                return NotFound("Order not found..!");

            return Ok("Order updated successfully");
        }

        // DELETE (Admin)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);

            if (!result)
                return NotFound("Order not found....Enter valid Id");

            return Ok("Order deleted successfully!!");
        }
    }
}