using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Caffe.Models;

namespace Caffe.Service
{
    public interface ICartItemService
    {
        /// <summary>
        /// Получить элемент корзины по ID
        /// </summary>
        Task<CartItem> GetCartItemByIdAsync(Guid id);

        /// <summary>
        /// Получить все элементы корзины по ID корзины
        /// </summary>
        Task<List<CartItem>> GetCartItemsByCartIdAsync(Guid cartId);

        /// <summary>
        /// Получить элемент корзины по ID товара и ID корзины
        /// </summary>
        Task<CartItem> GetCartItemByMenuItemAndCartAsync(Guid menuItemId, Guid cartId);

        /// <summary>
        /// Добавить новый элемент в корзину
        /// </summary>
        Task<CartItem> AddCartItemAsync(CartItem cartItem);

        /// <summary>
        /// Обновить существующий элемент корзины
        /// </summary>
        Task<CartItem> UpdateCartItemAsync(CartItem cartItem);

        /// <summary>
        /// Удалить элемент корзины
        /// </summary>
        Task<bool> DeleteCartItemAsync(Guid id);

        /// <summary>
        /// Обновить количество товара в элементе корзины
        /// </summary>
        Task<CartItem> UpdateQuantityAsync(Guid cartItemId, int newQuantity);

        /// <summary>
        /// Очистить корзину (удалить все элементы)
        /// </summary>
        Task<bool> ClearCartAsync(Guid cartId);
    }
}