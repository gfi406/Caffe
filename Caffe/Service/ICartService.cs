using Caffe.Models;

namespace Caffe.Service
{
    public interface ICartService
    {
        Task<List<Cart>> GetCartsAsync();
        Task<Cart> GetCartByIdAsync(Guid id);
        Task<Cart> AddCartAsync(Cart cart);
        Task<Cart> UpdateCartAsync(Cart cart);
        Task DeleteCartAsync(Guid id);
        Task<Cart> GetCartByUserIdAsync(Guid userId);
    }
}
