using Microsoft.AspNetCore.Mvc;
using Caffe.Models;
using Caffe.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Caffe.Models.Dto;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.Annotations;

namespace Caffe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemController : ControllerBase
    {
        private readonly IMenuItemService _menuItemService;
        private readonly IDatabase _redisDatabase;

        public MenuItemController(IMenuItemService menuItemService, IDatabase redisDatabase)
        {
            _menuItemService = menuItemService;
            //_redisDatabase = redisConnection.GetDatabase();
            _redisDatabase = redisDatabase;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Получить меню", Description = "Возвращает меню.")]
        [SwaggerResponse(200, "Список меню успешно возвращен", typeof(IEnumerable<MenuItemDto>))]
        [SwaggerResponse(500, "Внутренняя ошибка сервера")]
        public async Task<ActionResult<List<MenuItemDto>>> GetMenuItems()
        {
            var menuItems = await _menuItemService.GetMenuItemsAsync();
            var menuItemDtos = menuItems.Select(item => new MenuItemDto
            {
                Id = item.Id,
                Title = item.title,
                Description = item.description,
                Price = item.price,
                ImageUrl = item.img,
                IsAvailable = item.is_availble,
                Category = item.category
            }).ToList();

            return Ok(menuItemDtos);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Получить позицию меню идентификатору", Description = "Возвращает информацию о позиции меню по идентификатору.")]
        [SwaggerResponse(200, "Позиция меню успешно возвращена", typeof(MenuItemDto))]
        [SwaggerResponse(404, "Позиция меню не найдена")]
        public async Task<ActionResult<MenuItemDto>> GetMenuItemById(Guid id)
        {
            string cacheKey = $"menuItem:{id}";
            var cachedMenuItem = await _redisDatabase.StringGetAsync(cacheKey);

            if (!cachedMenuItem.IsNullOrEmpty)
            {
                return Ok(JsonSerializer.Deserialize<MenuItemDto>(cachedMenuItem));
            }

            var menuItem = await _menuItemService.GetMenuItemByIdAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            var menuItemDto = new MenuItemDto
            {
                Id = menuItem.Id,
                Title = menuItem.title,
                Description = menuItem.description,
                Price = menuItem.price,
                ImageUrl = menuItem.img,
                IsAvailable = menuItem.is_availble,
                Category = menuItem.category
            };

            await _redisDatabase.StringSetAsync(cacheKey, JsonSerializer.Serialize(menuItemDto), TimeSpan.FromMinutes(10));
            return Ok(menuItemDto);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Добавить новую позицию меню", Description = "Добавляет новую позицию меню в систему.")]
        [SwaggerResponse(201, "Позиция меню успешно добавлен", typeof(MenuItemDto))]
        [SwaggerResponse(400, "Ошибка при добавлении позиции меню")]
        public async Task<ActionResult<MenuItemDto>> AddMenuItem(MenuItemCreateUpdateDto menuItemCreateDto)
        {
            var menuItem = new MenuItem
            {
                title = menuItemCreateDto.Title,
                description = menuItemCreateDto.Description,
                price = menuItemCreateDto.Price,
                img = menuItemCreateDto.ImageUrl,
                is_availble = menuItemCreateDto.IsAvailable,
                category = menuItemCreateDto.Category
            };

            var createdMenuItem = await _menuItemService.AddMenuItemAsync(menuItem);
            var menuItemDto = new MenuItemDto
            {
                Id = createdMenuItem.Id,
                Title = createdMenuItem.title,
                Description = createdMenuItem.description,
                Price = createdMenuItem.price,
                ImageUrl = createdMenuItem.img,
                IsAvailable = createdMenuItem.is_availble,
                Category = createdMenuItem.category
            };

            await _redisDatabase.StringSetAsync($"menuItem:{menuItemDto.Id}", JsonSerializer.Serialize(menuItemDto), TimeSpan.FromMinutes(10));
            return CreatedAtAction(nameof(GetMenuItemById), new { id = menuItemDto.Id }, menuItemDto);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Обновить информацию позиции меню", Description = "Обновляет данные о позиции меню по его идентификатору.")]
        [SwaggerResponse(200, "Позиция меню успешно обновлена", typeof(MenuItemDto))]
        [SwaggerResponse(400, "Некорректные данные")]
        [SwaggerResponse(404, "Позиция меню не найдена")]
        public async Task<ActionResult<MenuItemDto>> UpdateMenuItem(Guid id, MenuItemCreateUpdateDto menuItemUpdateDto)
        {
            var existingMenuItem = await _menuItemService.GetMenuItemByIdAsync(id);
            if (existingMenuItem == null)
            {
                return NotFound();
            }

            existingMenuItem.title = menuItemUpdateDto.Title;
            existingMenuItem.description = menuItemUpdateDto.Description;
            existingMenuItem.price = menuItemUpdateDto.Price;
            existingMenuItem.img = menuItemUpdateDto.ImageUrl;
            existingMenuItem.is_availble = menuItemUpdateDto.IsAvailable;
            existingMenuItem.category = menuItemUpdateDto.Category;

            var updatedMenuItem = await _menuItemService.UpdateMenuItemAsync(existingMenuItem);

            var menuItemDto = new MenuItemDto
            {
                Id = updatedMenuItem.Id,
                Title = updatedMenuItem.title,
                Description = updatedMenuItem.description,
                Price = updatedMenuItem.price,
                ImageUrl = updatedMenuItem.img,
                IsAvailable = updatedMenuItem.is_availble,
                Category = updatedMenuItem.category
            };

            return Ok(menuItemDto);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Удалить позицию", Description = "Удаляет позицию по его идентификатору.")]
        [SwaggerResponse(200, "Позиция успешно удалена")]
        [SwaggerResponse(404, "Позиция не найдена")]
        public async Task<IActionResult> DeleteMenuItem(Guid id)
        {
            var menuItem = await _menuItemService.GetMenuItemByIdAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            await _menuItemService.DeleteMenuItemAsync(id);
            await _redisDatabase.KeyDeleteAsync($"menuItem:{id}");
            return NoContent();
        }
    }
}