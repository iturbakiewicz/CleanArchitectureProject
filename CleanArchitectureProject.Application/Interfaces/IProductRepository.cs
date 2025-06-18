using CleanArchitectureProject.Domain.Entities;

namespace CleanArchitectureProject.Application.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<Product>> GetProductsWithCategoryAsync();
        Task<Product?> GetProductWithCategoryAsync(int id);
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
    }
}