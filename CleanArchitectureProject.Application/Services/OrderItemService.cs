using CleanArchitectureProject.Application.DTOs;
using CleanArchitectureProject.Application.Interfaces;
using CleanArchitectureProject.Domain.Entities;

namespace CleanArchitectureProject.Application.Services
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;

        public OrderItemService(
            IOrderItemRepository orderItemRepository,
            IProductRepository productRepository,
            IOrderRepository orderRepository)
        {
            _orderItemRepository = orderItemRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<OrderItemDto>> GetAllOrderItemsAsync()
        {
            var orderItems = await _orderItemRepository.GetAllAsync();
            var result = new List<OrderItemDto>();

            foreach (var item in orderItems)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                result.Add(MapToDto(item, product?.Name ?? string.Empty));
            }

            return result;
        }

        public async Task<OrderItemDto?> GetOrderItemByIdAsync(int id)
        {
            var orderItem = await _orderItemRepository.GetByIdAsync(id);
            if (orderItem == null) return null;

            var product = await _productRepository.GetByIdAsync(orderItem.ProductId);
            return MapToDto(orderItem, product?.Name ?? string.Empty);
        }

        public async Task<OrderItemDto> UpdateOrderItemAsync(int id, UpdateOrderItemDto updateOrderItemDto)
        {
            var orderItem = await _orderItemRepository.GetByIdAsync(id);
            if (orderItem == null)
                throw new ArgumentException($"OrderItem with id {id} not found");

            var product = await _productRepository.GetByIdAsync(orderItem.ProductId);
            if (product == null)
                throw new ArgumentException($"Product with id {orderItem.ProductId} not found");

            // Sprawdź dostępność stock
            if (product.Stock < updateOrderItemDto.Quantity)
                throw new ArgumentException($"Insufficient stock for product {product.Name}");

            orderItem.Quantity = updateOrderItemDto.Quantity;
            orderItem.TotalPrice = orderItem.UnitPrice * updateOrderItemDto.Quantity;

            var updatedOrderItem = await _orderItemRepository.UpdateAsync(orderItem);

            // Zaktualizuj total amount w zamówieniu
            await UpdateOrderTotalAmount(orderItem.OrderId);

            return MapToDto(updatedOrderItem, product.Name);
        }

        public async Task DeleteOrderItemAsync(int id)
        {
            var orderItem = await _orderItemRepository.GetByIdAsync(id);
            if (orderItem == null)
                throw new ArgumentException($"OrderItem with id {id} not found");

            var orderId = orderItem.OrderId;
            await _orderItemRepository.DeleteAsync(id);

            // Zaktualizuj total amount w zamówieniu
            await UpdateOrderTotalAmount(orderId);
        }

        public async Task<IEnumerable<OrderItemDto>> GetOrderItemsByOrderIdAsync(int orderId)
        {
            var orderItems = await _orderItemRepository.GetOrderItemsByOrderIdAsync(orderId);
            var result = new List<OrderItemDto>();

            foreach (var item in orderItems)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                result.Add(MapToDto(item, product?.Name ?? string.Empty));
            }

            return result;
        }

        public async Task<IEnumerable<OrderItemDto>> GetOrderItemsByProductIdAsync(int productId)
        {
            var orderItems = await _orderItemRepository.GetOrderItemsByProductIdAsync(productId);
            var product = await _productRepository.GetByIdAsync(productId);
            var productName = product?.Name ?? string.Empty;

            return orderItems.Select(item => MapToDto(item, productName));
        }

        private async Task UpdateOrderTotalAmount(int orderId)
        {
            var order = await _orderRepository.GetOrderWithItemsAsync(orderId);
            if (order != null)
            {
                order.TotalAmount = order.OrderItems.Sum(oi => oi.TotalPrice);
                await _orderRepository.UpdateAsync(order);
            }
        }

        private static OrderItemDto MapToDto(OrderItem orderItem, string productName)
        {
            return new OrderItemDto
            {
                Id = orderItem.Id,
                Quantity = orderItem.Quantity,
                UnitPrice = orderItem.UnitPrice,
                TotalPrice = orderItem.TotalPrice,
                OrderId = orderItem.OrderId,
                ProductId = orderItem.ProductId,
                ProductName = productName
            };
        }
    }
}