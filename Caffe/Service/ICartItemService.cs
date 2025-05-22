using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Caffe.Models;

namespace Caffe.Service
{
    public interface ICartItemService
    {
        Task<CartItem> GetCartItemByIdAsync(Guid id);
        Task<List<CartItem>> GetCartItemsByCartIdAsync(Guid cartId);
        Task<CartItem> GetCartItemByMenuItemAndCartAsync(Guid menuItemId, Guid cartId);
        Task<CartItem> AddCartItemAsync(CartItem cartItem);
        Task<CartItem> UpdateCartItemAsync(CartItem cartItem);
        Task<bool> DeleteCartItemAsync(Guid id);
        Task<CartItem> UpdateQuantityAsync(Guid cartItemId, int newQuantity);     
        Task<bool> ClearCartAsync(Guid cartId);
    }
}