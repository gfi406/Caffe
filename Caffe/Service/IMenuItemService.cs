using Caffe.Models;

namespace Caffe.Service
{
    public interface IMenuItemService
    {
        Task<List<MenuItem>> GetMenuItemsAsync();
        Task<MenuItem> GetMenuItemByIdAsync(Guid id);
        Task<MenuItem> AddMenuItemAsync(MenuItem menuItem);
        Task<MenuItem> UpdateMenuItemAsync(MenuItem menuItem);
        Task DeleteMenuItemAsync(Guid id);
    }
}
