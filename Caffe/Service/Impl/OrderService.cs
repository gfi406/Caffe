using Caffe.Models;
using Microsoft.EntityFrameworkCore;

namespace Caffe.Service.Impl
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Cart)         // Включаем информацию о корзине
                .Include(o => o.User)         // Включаем информацию о пользователе
                .ToListAsync();
        }

        public async Task<Order> GetOrderByIdAsync(Guid id)
        {
            return await _context.Orders
                .Include(o => o.Cart)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order> AddOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order> UpdateOrderAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task DeleteOrderAsync(Guid id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(Guid userId)
        {
            return await _context.Orders
                .Include(o => o.Cart)
                .Include(o => o.User)
                .Where(o => o.user_id == userId)
                .ToListAsync();
        }

        public async Task<List<Order>> GetOrdersByCartIdAsync(Guid cartId)
        {
            return await _context.Orders
                .Include(o => o.Cart)
                .Include(o => o.User)
                .Where(o => o.CartId == cartId)
                .ToListAsync();
        }
    }
}
