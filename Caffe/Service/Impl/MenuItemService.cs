using Caffe.Models;
using Microsoft.EntityFrameworkCore;

namespace Caffe.Service.Impl
{
    public class MenuItemService : IMenuItemService
    {
        private readonly ApplicationDbContext _context;

        public MenuItemService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MenuItem>> GetMenuItemsAsync()
        {
            return await _context.MenuItems.ToListAsync();
        }

        public async Task<MenuItem> GetMenuItemByIdAsync(Guid id)
        {
            return await _context.MenuItems
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<MenuItem> AddMenuItemAsync(MenuItem menuItem)
        {
            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();
            return menuItem;
        }

        public async Task<MenuItem> UpdateMenuItemAsync(MenuItem menuItem)
        {
            _context.MenuItems.Update(menuItem);
            await _context.SaveChangesAsync();
            return menuItem;
        }

        public async Task DeleteMenuItemAsync(Guid id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem != null)
            {
                _context.MenuItems.Remove(menuItem);
                await _context.SaveChangesAsync();
            }
        }
    }
}
