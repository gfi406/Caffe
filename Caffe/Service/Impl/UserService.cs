using Caffe.Models;
using Microsoft.EntityFrameworkCore;

namespace Caffe.Service.Impl
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Cart)           // Включаем корзину пользователя
                .Include(u => u.Orders)         // Включаем заказы пользователя
                .ToListAsync();
        }

        public async Task<User> AuthenticateUserAsync(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            // Найти пользователя по email
            var user = await _context.Users
                .Include(u => u.Cart)
                .Include(u => u.Orders)
                .FirstOrDefaultAsync(u => u.email == email && u.is_active);

           
            if (user == null || user.password != password) 
            {
                return null;
            }

            return user;
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            return await _context.Users
                .Include(u => u.Cart)
                .Include(u => u.Orders)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Cart)
                .Include(u => u.Orders)
                .FirstOrDefaultAsync(u => u.email == email);
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(Guid userId)
        {
            return await _context.Orders
                .Include(o => o.Cart)
                .Include(o => o.User)
                .Where(o => o.user_id == userId)
                .ToListAsync();
        }

        public async Task<Cart> GetCartByUserIdAsync(Guid userId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.user_id == userId);
        }
    }
}
