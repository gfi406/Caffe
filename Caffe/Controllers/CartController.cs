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
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IMenuItemService _menuItemService;

        public CartController(ICartService cartService, IMenuItemService menuItemService)
        {
            _cartService = cartService;
            _menuItemService = menuItemService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CartDto>> GetCartById(Guid id)
        {
            var cart = await _cartService.GetCartByIdAsync(id);
            if (cart == null)
            {
                return NotFound();
            }

            var cartDto = new CartDto
            {
                Id = cart.Id,
                UserId = cart.user_id,
                TotalPrice = cart.totalPrice,
                OrderId = cart.Order?.Id,
                Items = cart.Items?.Select(item => new MenuItemDto
                {
                    Id = item.Id,
                    Title = item.title,
                    Description = item.description,
                    Price = item.price,
                    ImageUrl = item.img,
                    IsAvailable = item.is_availble
                }).ToList() ?? new List<MenuItemDto>()
            };

            return Ok(cartDto);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<CartDto>> GetCartByUserId(Guid userId)
        {
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                return NotFound();
            }

            var cartDto = new CartDto
            {
                Id = cart.Id,
                UserId = cart.user_id,
                TotalPrice = cart.totalPrice,
                OrderId = cart.Order?.Id,
                Items = cart.Items?.Select(item => new MenuItemDto
                {
                    Id = item.Id,
                    Title = item.title,
                    Description = item.description,
                    Price = item.price,
                    ImageUrl = item.img,
                    IsAvailable = item.is_availble
                }).ToList() ?? new List<MenuItemDto>()
            };

            return Ok(cartDto);
        }

        [HttpPost("user/{userId}/add-item")]
        public async Task<ActionResult<CartDto>> AddItemToCart(Guid userId, AddToCartDto addToCartDto)
        {
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                return NotFound("Cart not found for user");
            }

            var menuItem = await _menuItemService.GetMenuItemByIdAsync(addToCartDto.MenuItemId);
            if (menuItem == null)
            {
                return NotFound("Menu item not found");
            }

            // Add the item to cart (this is simplified, in real app you might need to handle quantities)
            if (cart.Items == null)
            {
                cart.Items = new List<MenuItem>();
            }

            for (int i = 0; i < addToCartDto.Quantity; i++)
            {
                cart.Items.Add(menuItem);
            }

            // Recalculate total price
            cart.totalPrice = cart.Items.Sum(item => item.price);

            var updatedCart = await _cartService.UpdateCartAsync(cart);

            var cartDto = new CartDto
            {
                Id = updatedCart.Id,
                UserId = updatedCart.user_id,
                TotalPrice = updatedCart.totalPrice,
                OrderId = updatedCart.Order?.Id,
                Items = updatedCart.Items?.Select(item => new MenuItemDto
                {
                    Id = item.Id,
                    Title = item.title,
                    Description = item.description,
                    Price = item.price,
                    ImageUrl = item.img,
                    IsAvailable = item.is_availble
                }).ToList() ?? new List<MenuItemDto>()
            };

            return Ok(cartDto);
        }

        [HttpDelete("user/{userId}/remove-item/{itemId}")]
        public async Task<ActionResult<CartDto>> RemoveItemFromCart(Guid userId, Guid itemId)
        {
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                return NotFound("Cart not found for user");
            }

            // Find and remove the item from cart
            var itemToRemove = cart.Items?.FirstOrDefault(i => i.Id == itemId);
            if (itemToRemove == null)
            {
                return NotFound("Item not found in cart");
            }

            cart.Items.Remove(itemToRemove);

            // Recalculate total price
            cart.totalPrice = cart.Items.Sum(item => item.price);

            var updatedCart = await _cartService.UpdateCartAsync(cart);

            var cartDto = new CartDto
            {
                Id = updatedCart.Id,
                UserId = updatedCart.user_id,
                TotalPrice = updatedCart.totalPrice,
                OrderId = updatedCart.Order?.Id,
                Items = updatedCart.Items?.Select(item => new MenuItemDto
                {
                    Id = item.Id,
                    Title = item.title,
                    Description = item.description,
                    Price = item.price,
                    ImageUrl = item.img,
                    IsAvailable = item.is_availble
                }).ToList() ?? new List<MenuItemDto>()
            };

            return Ok(cartDto);
        }

        [HttpDelete("user/{userId}/clear")]
        public async Task<ActionResult<CartDto>> ClearCart(Guid userId)
        {
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                return NotFound("Cart not found for user");
            }

            cart.Items = new List<MenuItem>();
            cart.totalPrice = 0;

            var updatedCart = await _cartService.UpdateCartAsync(cart);

            var cartDto = new CartDto
            {
                Id = updatedCart.Id,
                UserId = updatedCart.user_id,
                TotalPrice = updatedCart.totalPrice,
                OrderId = updatedCart.Order?.Id,
                Items = new List<MenuItemDto>()
            };

            return Ok(cartDto);
        }
    }
}
