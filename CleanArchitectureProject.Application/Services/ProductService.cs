using CleanArchitectureProject.Application.DTOs;
using CleanArchitectureProject.Application.Interfaces;
using CleanArchitectureProject.Domain.Entities;

namespace CleanArchitectureProject.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetProductsWithCategoryAsync();
            return products.Select(MapToDto);
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetProductWithCategoryAsync(id);
            return product != null ? MapToDto(product) : null;
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
        {
            // Sprawdź czy kategoria istnieje
            var categoryExists = await _categoryRepository.ExistsAsync(createProductDto.CategoryId);
            if (!categoryExists)
                throw new ArgumentException($"Category with id {createProductDto.CategoryId} not found");

            var product = new Product
            {
                Name = createProductDto.Name,
                Description = createProductDto.Description,
                Price = createProductDto.Price,
                Stock = createProductDto.Stock,
                CategoryId = createProductDto.CategoryId,
                CreatedAt = DateTime.UtcNow
            };

            var createdProduct = await _productRepository.AddAsync(product);
            var productWithCategory = await _productRepository.GetProductWithCategoryAsync(createdProduct.Id);
            return MapToDto(productWithCategory!);
        }

        public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto updateProductDto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new ArgumentException($"Product with id {id} not found");

            // Sprawdź czy nowa kategoria istnieje
            var categoryExists = await _categoryRepository.ExistsAsync(updateProductDto.CategoryId);
            if (!categoryExists)
                throw new ArgumentException($"Category with id {updateProductDto.CategoryId} not found");

            product.Name = updateProductDto.Name;
            product.Description = updateProductDto.Description;
            product.Price = updateProductDto.Price;
            product.Stock = updateProductDto.Stock;
            product.CategoryId = updateProductDto.CategoryId;

            var updatedProduct = await _productRepository.UpdateAsync(product);
            var productWithCategory = await _productRepository.GetProductWithCategoryAsync(updatedProduct.Id);
            return MapToDto(productWithCategory!);
        }

        public async Task DeleteProductAsync(int id)
        {
            await _productRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _productRepository.GetProductsByCategoryAsync(categoryId);
            return products.Select(MapToDto);
        }

        public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm)
        {
            var products = await _productRepository.SearchProductsAsync(searchTerm);
            return products.Select(MapToDto);
        }

        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CreatedAt = product.CreatedAt,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name ?? string.Empty
            };
        }
    }
}