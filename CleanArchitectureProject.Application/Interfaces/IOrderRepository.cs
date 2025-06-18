using CleanArchitectureProject.Domain.Entities;

namespace CleanArchitectureProject.Application.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersWithItemsAsync();
        Task<Order?> GetOrderWithItemsAsync(int id);
        Task<IEnumerable<Order>> GetOrdersByCustomerEmailAsync(string email);
    }
}