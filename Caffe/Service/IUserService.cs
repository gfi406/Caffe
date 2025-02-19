using Caffe.Models;

namespace Caffe.Service
{
    public interface IUserService
    {
        Task<List<User>> GetUsersAsync();
        Task<User> GetUserByIdAsync(Guid id);
        Task<User> AddUserAsync(User user);
        Task<User> UpdateUserAsync(User user);
        Task DeleteUserAsync(Guid id);
        Task<User> GetUserByEmailAsync(string email);
        Task<List<Order>> GetOrdersByUserIdAsync(Guid userId);
        Task<Cart> GetCartByUserIdAsync(Guid userId);
    }
}
