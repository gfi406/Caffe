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
                //.Include(c => c.Items)   
                //.Include(c => c.User)    
                //.Include(c => c.Order)   
                .ToListAsync();
        }

        public async Task<Cart> GetCartByIdAsync(Guid id)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .Include(c => c.User)
                //.Include(c => c.Order)
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
        public async Task<CartItem> AddItemToCartAsync(Guid cartId, Guid menuItemId, int quantity, decimal price)
        {
            // Проверяем существование элемента в корзине
            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(i => i.CartId == cartId && i.MenuItemId == menuItemId);

            if (existingItem != null)
            {
                
                existingItem.Quantity += quantity;
            }
            else
            {
               
                existingItem = new CartItem
                {
                    CartId = cartId,
                    MenuItemId = menuItemId,
                    Quantity = quantity,
                    Price = (int)price

                };
                _context.CartItems.Add(existingItem);
            }

            await _context.SaveChangesAsync();
            return existingItem;
        }

        public async Task UpdateCartTotalAsync(Guid cartId)
        {
            var total = await _context.CartItems
                .Where(i => i.CartId == cartId)
                .SumAsync(i => i.Quantity * i.Price);

            await _context.Carts
                .Where(c => c.Id == cartId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(c => c.totalPrice, total));
        }

        public async Task<Cart> GetCartByUserIdAsync(Guid userId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .Include(c => c.User)
                //.Include(c => c.Order)
                .FirstOrDefaultAsync(c => c.user_id == userId);
        }
    }
    
}
