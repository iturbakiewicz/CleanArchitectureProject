using CleanArchitectureProject.Application.DTOs;
using CleanArchitectureProject.Application.Interfaces;
using CleanArchitectureProject.Domain.Entities;

namespace CleanArchitectureProject.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetOrdersWithItemsAsync();
            return orders.Select(MapToDto);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepository.GetOrderWithItemsAsync(id);
            return order != null ? MapToDto(order) : null;
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            var order = new Order
            {
                CustomerName = createOrderDto.CustomerName,
                CustomerEmail = createOrderDto.CustomerEmail,
                OrderDate = DateTime.UtcNow,
                Status = "Pending"
            };

            decimal totalAmount = 0;
            var orderItems = new List<OrderItem>();

            foreach (var itemDto in createOrderDto.OrderItems)
            {
                var product = await _productRepository.GetByIdAsync(itemDto.ProductId);
                if (product == null)
                    throw new ArgumentException($"Product with id {itemDto.ProductId} not found");

                if (product.Stock < itemDto.Quantity)
                    throw new ArgumentException($"Insufficient stock for product {product.Name}");

                var orderItem = new OrderItem
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    UnitPrice = product.Price,
                    TotalPrice = product.Price * itemDto.Quantity
                };

                orderItems.Add(orderItem);
                totalAmount += orderItem.TotalPrice;
            }

            order.TotalAmount = totalAmount;
            order.OrderItems = orderItems;

            var createdOrder = await _orderRepository.AddAsync(order);
            var orderWithItems = await _orderRepository.GetOrderWithItemsAsync(createdOrder.Id);
            return MapToDto(orderWithItems!);
        }

        public async Task<OrderDto> UpdateOrderAsync(int id, UpdateOrderDto updateOrderDto)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
                throw new ArgumentException($"Order with id {id} not found");

            order.CustomerName = updateOrderDto.CustomerName;
            order.CustomerEmail = updateOrderDto.CustomerEmail;
            order.Status = updateOrderDto.Status;

            var updatedOrder = await _orderRepository.UpdateAsync(order);
            var orderWithItems = await _orderRepository.GetOrderWithItemsAsync(updatedOrder.Id);
            return MapToDto(orderWithItems!);
        }

        public async Task DeleteOrderAsync(int id)
        {
            await _orderRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByCustomerEmailAsync(string email)
        {
            var orders = await _orderRepository.GetOrdersByCustomerEmailAsync(email);
            return orders.Select(MapToDto);
        }

        private static OrderDto MapToDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                CustomerEmail = order.CustomerEmail,
                TotalAmount = order.TotalAmount,
                OrderDate = order.OrderDate,
                Status = order.Status,
                OrderItems = order.OrderItems?.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    TotalPrice = oi.TotalPrice,
                    OrderId = oi.OrderId,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name ?? string.Empty
                }).ToList() ?? new List<OrderItemDto>()
            };
        }
    }
}