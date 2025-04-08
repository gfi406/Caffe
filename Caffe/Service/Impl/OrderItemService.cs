
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caffe.Models;
using Microsoft.EntityFrameworkCore;

namespace Caffe.Service
{
    public class OrderItemService : IOrderItemService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICartItemService _cartItemService;

        public OrderItemService(ApplicationDbContext context, ICartItemService cartItemService)
        {
            _context = context;
            _cartItemService = cartItemService;
        }

        public async Task<OrderItem> AddOrderItemAsync(OrderItem orderItem)
        {
            _context.OrderItems.Add(orderItem);
            await _context.SaveChangesAsync();
            return orderItem;
        }

        public async Task<List<OrderItem>> GetOrderItemsByOrderIdAsync(Guid orderId)
        {
            return await _context.OrderItems
                .Where(oi => oi.OrderId == orderId)
                .Include(oi => oi.MenuItem)
                .ToListAsync();
        }

        public async Task<bool> DeleteOrderItemAsync(Guid id)
        {
            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem == null) return false;

            _context.OrderItems.Remove(orderItem);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<OrderItem>> CreateOrderItemsFromCartItems(Guid orderId, Guid cartId)
        {
            var cartItems = await _cartItemService.GetCartItemsByCartIdAsync(cartId);
            var orderItems = new List<OrderItem>();

            foreach (var cartItem in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = orderId,
                    MenuItemId = cartItem.MenuItemId,
                    Quantity = cartItem.Quantity,
                    PriceAtOrderTime = cartItem.MenuItem.price
                };
                orderItems.Add(orderItem);
            }

            await _context.OrderItems.AddRangeAsync(orderItems);
            await _context.SaveChangesAsync();

            return orderItems;
        }
        public async Task DeleteOrderItemsByOrderIdAsync(Guid orderId)
        {
            var orderItems = await _context.OrderItems
                .Where(oi => oi.OrderId == orderId)
                .ToListAsync();

            _context.OrderItems.RemoveRange(orderItems);
            await _context.SaveChangesAsync();
        }
    }
}