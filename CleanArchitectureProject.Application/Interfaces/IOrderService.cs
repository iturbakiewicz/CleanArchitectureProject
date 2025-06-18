using CleanArchitectureProject.Application.DTOs;

namespace CleanArchitectureProject.Application.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto);
        Task<OrderDto> UpdateOrderAsync(int id, UpdateOrderDto updateOrderDto);
        Task DeleteOrderAsync(int id);
        Task<IEnumerable<OrderDto>> GetOrdersByCustomerEmailAsync(string email);
    }
}