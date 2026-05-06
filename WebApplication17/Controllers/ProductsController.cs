using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebApplication17.DTOs;
using WebApplication17.Services;

namespace WebApplication17.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;
        private readonly CloudinaryService _cloudinaryService; 

        public ProductsController(
            IProductService service,
            CloudinaryService cloudinaryService) // CloudinaryService for image storage and management..its a third party cloud-based media management
        {
            _service = service;
            _cloudinaryService = cloudinaryService;
        }

        // GET ALL — so anyone can see products
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _service.GetAllAsync();
            return Ok(products);
        }

        // GET BY ID —  so anyone can see
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid product id");

            var product = await _service.GetByIdAsync(id);

            if (product == null)
                return NotFound("Product not found");

            return Ok(product);
        }

        // CREATE —  accepts image file upload
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromForm] ProductDTO dto,
            IFormFile? imageFile) // Accept image file
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Upload image to Cloudinary if file provided
                if (imageFile != null)
                {
                    dto.ImageUrl = await _cloudinaryService.UploadImageAsync(imageFile);
                }

                await _service.AddAsync(dto);
                return Ok("Product created successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // UPDATE
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id,
            [FromForm] ProductDTO dto,
            IFormFile? imageFile) // Accept image file
        {
            if (id <= 0)
                return BadRequest("Invalid product id");

            try
            {
                // Upload new image if provided
                if (imageFile != null)
                {
                    dto.ImageUrl = await _cloudinaryService.UploadImageAsync(imageFile);
                }

                var result = await _service.UpdateAsync(id, dto);

                if (!result)
                    return NotFound("Product not found");

                return Ok("Updated successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE 
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid product id");

            try
            {
                var result = await _service.DeleteAsync(id);

                if (!result)
                    return NotFound("Product not found");

                return Ok("Deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}