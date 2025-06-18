using CleanArchitectureProject.Domain.Entities;

namespace CleanArchitectureProject.Application.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IEnumerable<Category>> GetCategoriesWithProductsAsync();
        Task<Category?> GetCategoryWithProductsAsync(int id);
    }
}