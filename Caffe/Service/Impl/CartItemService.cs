using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caffe.Models;
using Microsoft.EntityFrameworkCore;

namespace Caffe.Service
{
    public class CartItemService : ICartItemService
    {
        private readonly ApplicationDbContext _context;

        public CartItemService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CartItem> GetCartItemByIdAsync(Guid id)
        {
            return await _context.CartItems
                .Include(ci => ci.MenuItem)
                .FirstOrDefaultAsync(ci => ci.Id == id);
        }

        public async Task<List<CartItem>> GetCartItemsByCartIdAsync(Guid cartId)
        {
            return await _context.CartItems
                .Where(ci => ci.CartId == cartId)
                .Include(ci => ci.MenuItem)
                .ToListAsync();
        }

        public async Task<CartItem> GetCartItemByMenuItemAndCartAsync(Guid menuItemId, Guid cartId)
        {
            return await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.MenuItemId == menuItemId && ci.CartId == cartId);
        }

        public async Task<CartItem> AddCartItemAsync(CartItem cartItem)
        {
            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();
            return cartItem;
        }

        public async Task<CartItem> UpdateCartItemAsync(CartItem cartItem)
        {
            _context.CartItems.Update(cartItem);
            await _context.SaveChangesAsync();
            return cartItem;
        }

        public async Task<bool> DeleteCartItemAsync(Guid id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem == null) return false;

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CartItem> UpdateQuantityAsync(Guid cartItemId, int newQuantity)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem == null) return null;

            cartItem.Quantity = newQuantity;
            _context.CartItems.Update(cartItem);
            await _context.SaveChangesAsync();
            return cartItem;
        }

        public async Task<bool> ClearCartAsync(Guid cartId)
        {
            var cartItems = await _context.CartItems
                .Where(ci => ci.CartId == cartId)
                .ToListAsync();

            if (!cartItems.Any()) return false;

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}   