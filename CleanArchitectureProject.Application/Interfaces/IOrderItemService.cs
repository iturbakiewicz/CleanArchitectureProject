using CleanArchitectureProject.Application.DTOs;

namespace CleanArchitectureProject.Application.Interfaces
{
    public interface IOrderItemService
    {
        Task<IEnumerable<OrderItemDto>> GetAllOrderItemsAsync();
        Task<OrderItemDto?> GetOrderItemByIdAsync(int id);
        Task<OrderItemDto> UpdateOrderItemAsync(int id, UpdateOrderItemDto updateOrderItemDto);
        Task DeleteOrderItemAsync(int id);
        Task<IEnumerable<OrderItemDto>> GetOrderItemsByOrderIdAsync(int orderId);
        Task<IEnumerable<OrderItemDto>> GetOrderItemsByProductIdAsync(int productId);
    }
}