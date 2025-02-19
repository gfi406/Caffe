using Caffe.Models;
using Microsoft.EntityFrameworkCore;

namespace Caffe.Service.Impl
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;

        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Cart>> GetCartsAsync()
        {
            return await _context.Carts
                .Include(c => c.Items)   // Включаем элементы корзины
                .Include(c => c.User)    // Включаем информацию о пользователе
                .Include(c => c.Order)   // Включаем информацию о заказе
                .ToListAsync();
        }

        public async Task<Cart> GetCartByIdAsync(Guid id)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .Include(c => c.User)
                .Include(c => c.Order)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Cart> AddCartAsync(Cart cart)
        {
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<Cart> UpdateCartAsync(Cart cart)
        {
            _context.Carts.Update(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task DeleteCartAsync(Guid id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Cart> GetCartByUserIdAsync(Guid userId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .Include(c => c.User)
                .Include(c => c.Order)
                .FirstOrDefaultAsync(c => c.user_id == userId);
        }
    }
    
}
