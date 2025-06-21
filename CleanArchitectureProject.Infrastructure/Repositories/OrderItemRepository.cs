using CleanArchitectureProject.Application.Interfaces;
using CleanArchitectureProject.Domain.Entities;
using CleanArchitectureProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureProject.Infrastructure.Repositories
{
    public class OrderItemRepository : Repository<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId)
        {
            return await _dbSet
                .Include(oi => oi.Product)
                .Where(oi => oi.OrderId == orderId)
                .ToListAsync();
        }

        public async Task<IEnumerable<OrderItem>> GetOrderItemsByProductIdAsync(int productId)
        {
            return await _dbSet
                .Include(oi => oi.Order)
                .Where(oi => oi.ProductId == productId)
                .ToListAsync();
        }
    }
}