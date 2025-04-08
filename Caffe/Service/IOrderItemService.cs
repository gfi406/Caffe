using Caffe.Models;

namespace Caffe.Service
{
    public interface IOrderItemService
    {
        Task<OrderItem> AddOrderItemAsync(OrderItem orderItem);
        Task<List<OrderItem>> GetOrderItemsByOrderIdAsync(Guid orderId);
        Task<bool> DeleteOrderItemAsync(Guid id);
        Task<List<OrderItem>> CreateOrderItemsFromCartItems(Guid orderId, Guid cartId);
        Task DeleteOrderItemsByOrderIdAsync(Guid orderId);
    }
}
