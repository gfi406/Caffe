using Microsoft.AspNetCore.Mvc;
using Caffe.Models;
using Caffe.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caffe.Models.Dto;

namespace Caffe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemController : ControllerBase
    {
        private readonly IMenuItemService _menuItemService;

        public MenuItemController(IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        [HttpGet]
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
                IsAvailable = item.is_availble
            }).ToList();

            return Ok(menuItemDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MenuItemDto>> GetMenuItemById(Guid id)
        {
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
                IsAvailable = menuItem.is_availble
            };

            return Ok(menuItemDto);
        }

        [HttpPost]
        public async Task<ActionResult<MenuItemDto>> AddMenuItem(MenuItemCreateUpdateDto menuItemCreateDto)
        {
            var menuItem = new MenuItem
            {
                title = menuItemCreateDto.Title,
                description = menuItemCreateDto.Description,
                price = menuItemCreateDto.Price,
                img = menuItemCreateDto.ImageUrl,
                is_availble = menuItemCreateDto.IsAvailable
            };

            var createdMenuItem = await _menuItemService.AddMenuItemAsync(menuItem);

            var menuItemDto = new MenuItemDto
            {
                Id = createdMenuItem.Id,
                Title = createdMenuItem.title,
                Description = createdMenuItem.description,
                Price = createdMenuItem.price,
                ImageUrl = createdMenuItem.img,
                IsAvailable = createdMenuItem.is_availble
            };

            return CreatedAtAction(nameof(GetMenuItemById), new { id = menuItemDto.Id }, menuItemDto);
        }

        [HttpPut("{id}")]
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

            var updatedMenuItem = await _menuItemService.UpdateMenuItemAsync(existingMenuItem);

            var menuItemDto = new MenuItemDto
            {
                Id = updatedMenuItem.Id,
                Title = updatedMenuItem.title,
                Description = updatedMenuItem.description,
                Price = updatedMenuItem.price,
                ImageUrl = updatedMenuItem.img,
                IsAvailable = updatedMenuItem.is_availble
            };

            return Ok(menuItemDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenuItem(Guid id)
        {
            var menuItem = await _menuItemService.GetMenuItemByIdAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            await _menuItemService.DeleteMenuItemAsync(id);
            return NoContent();
        }
    }
}
