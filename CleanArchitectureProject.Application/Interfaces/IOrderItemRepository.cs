using CleanArchitectureProject.Domain.Entities;

namespace CleanArchitectureProject.Application.Interfaces
{
    public interface IOrderItemRepository : IRepository<OrderItem>
    {
        Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId);
        Task<IEnumerable<OrderItem>> GetOrderItemsByProductIdAsync(int productId);
    }
}