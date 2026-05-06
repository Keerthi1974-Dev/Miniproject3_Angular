using WebApplication17.DTOs;
using WebApplication17.Models;
using WebApplication17.Repositories;

namespace WebApplication17.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;

        public ProductService(IProductRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
            => await _repo.GetAllAsync();

        public async Task<Product> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new Exception("Invalid product id");

            return await _repo.GetByIdAsync(id);
        }

        // CREATE
        public async Task AddAsync(ProductDTO dto)
        {
            if (dto == null)
                throw new Exception("Invalid data");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new Exception("Product name is required");

            if (dto.Price <= 0)
                throw new Exception("Price must be greater than zero");

            if (dto.Stock < 0)
                throw new Exception("Stock cannot be negative");

            if (string.IsNullOrWhiteSpace(dto.ImageUrl))
                throw new Exception("ImageUrl is required");

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                ImageUrl = dto.ImageUrl,   
                Stock = dto.Stock
            };

            await _repo.AddAsync(product);
        }

        //  UPDATE
        public async Task<bool> UpdateAsync(int id, ProductDTO dto)
        {
            if (id <= 0)
                throw new Exception("Invalid product id");

            var product = await _repo.GetByIdAsync(id);

            if (product == null)
                return false;

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.ImageUrl = dto.ImageUrl; 
            product.Stock = dto.Stock;

            await _repo.UpdateAsync(product);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
                throw new Exception("Invalid product id");

            var product = await _repo.GetByIdAsync(id);

            if (product == null)
                return false;

            await _repo.DeleteAsync(product);

            return true;
        }
    }
}