using Caffe.Models;

namespace Caffe.Service
{
    public interface IOrderService
    {
        Task<List<Order>> GetOrdersAsync();
        Task<Order> GetOrderByIdAsync(Guid id);
        Task<Order> AddOrderAsync(Order order);
        Task<Order> UpdateOrderAsync(Order order);
        Task DeleteOrderAsync(Guid id);
        Task<List<Order>> GetOrdersByUserIdAsync(Guid userId);
        Task<List<Order>> GetOrdersByCartIdAsync(Guid cartId);
    }
}
