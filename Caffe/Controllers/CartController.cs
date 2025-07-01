using Microsoft.AspNetCore.Mvc;
using Caffe.Models;
using Caffe.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caffe.Models.Dto;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Caffe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IMenuItemService _menuItemService;
        private readonly ICartItemService _cartItemService;

        public CartController(
            ICartService cartService,
            IMenuItemService menuItemService,
            ICartItemService cartItemService)
        {
            _cartService = cartService;
            _menuItemService = menuItemService;
            _cartItemService = cartItemService;
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Получить корзину по идентификатору", Description = "Возвращает информацию о корзине по идентификатору.")]
        [SwaggerResponse(200, "Корзина успешно возвращена", typeof(CartDto))]
        [SwaggerResponse(404, "Корзина не найдена")]
        public async Task<ActionResult<CartDto>> GetCartById(Guid id)
        {
            var cart = await _cartService.GetCartByIdAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            var cartItems = await _cartItemService.GetCartItemsByCartIdAsync(cart.Id);

            var cartDto = new CartDto
            {
                Id = cart.Id,
                UserId = cart.user_id,
                TotalPrice = cart.totalPrice,
               // OrderId = cart.Order?.Id,
                Items = cartItems?
                    .Where(item => item.MenuItem != null)
                    .Select(item => new MenuCartItemDto
                    {
                        Id = item.MenuItem?.Id ?? Guid.Empty,
                        Title = item.MenuItem?.title ?? "Название не указано",
                        Description = item.MenuItem?.description ?? string.Empty,
                        Price = item.MenuItem?.price ?? 0,
                        ImageUrl = item.MenuItem?.img,
                        IsAvailable = item.MenuItem?.is_availble ?? false,
                        Category = item.MenuItem?.category ?? 0,
                        Quantity = item.Quantity
                    })
                    .ToList() ?? new List<MenuCartItemDto>()
            };

            return Ok(cartDto);
        }

        [HttpGet("user/{userId}")]
        [SwaggerOperation(Summary = "Получить корзину пользователя", Description = "Возвращает корзину пользователя по идентификатору.")]
        [SwaggerResponse(200, "Корзина успешно возвращена", typeof(CartDto))]
        [SwaggerResponse(404, "Корзина не найдена")]
        public async Task<ActionResult<CartDto>> GetCartByUserId(Guid userId)
        {
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                return NotFound("Корзина не найдена");
            }

            var cartItems = await _cartItemService.GetCartItemsByCartIdAsync(cart.Id);

            var cartDto = new CartDto
            {
                Id = cart.Id,
                UserId = cart.user_id,
                TotalPrice = cart.totalPrice,
               // OrderId = cart.Order?.Id,
                Items = cartItems?
                    .Where(item => item.MenuItem != null)
                    .Select(item => new MenuCartItemDto
                    {
                        Id = item.MenuItem?.Id ?? Guid.Empty,
                        Title = item.MenuItem?.title ?? "Название не указано",
                        Description = item.MenuItem?.description ?? string.Empty,
                        Price = item.MenuItem?.price ?? 0,
                        ImageUrl = item.MenuItem?.img,
                        IsAvailable = item.MenuItem?.is_availble ?? false,
                        Category = item.MenuItem?.category ?? 0,
                        Quantity = item.Quantity
                    })
                    .ToList() ?? new List<MenuCartItemDto>()
            };

            return Ok(cartDto);
        }

       
        [HttpPost("user/{userId}/add-item")]
        [SwaggerOperation(Summary = "Добавить товар в корзину", Description = "Добавляет товар в корзину пользователя.")]
        [SwaggerResponse(201, "Товар успешно добавлен", typeof(CartDto))]
        [SwaggerResponse(400, "Ошибка при добавлении товара")]
        
        public async Task<ActionResult<CartDto>> AddItemToCart(Guid userId, AddToCartDto addToCartDto)
        {
            if (addToCartDto.Quantity <= 0)
            {
                return BadRequest("Количество должно быть больше нуля");
            }

            var cart = await _cartService.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                return BadRequest("Не удалось создать корзину");
            }

            var menuItem = await _menuItemService.GetMenuItemByIdAsync(addToCartDto.MenuItemId);
            if (menuItem == null)
            {
                return NotFound("Товар не найден");
            }

            if (!menuItem.is_availble)
            {
                return BadRequest("Товар временно недоступен");
            }

            try
            {
                var cartItem = await _cartService.AddItemToCartAsync(
                    cart.Id,
                    menuItem.Id,
                    addToCartDto.Quantity,
                    menuItem.price);

                await _cartService.UpdateCartTotalAsync(cart.Id);

                var updatedCart = await _cartService.GetCartByIdAsync(cart.Id);
                return Ok(MapToCartDto(updatedCart));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Conflict("Произошел конфликт при обновлении данных. Попробуйте еще раз.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Произошла внутренняя ошибка сервера");
            }
        }

        private CartDto MapToCartDto(Cart cart)
        {
            return new CartDto
            {
                Id = cart.Id,
                UserId = cart.user_id,
                TotalPrice = cart.totalPrice,
                Items = cart.Items?.Select(item => new MenuCartItemDto
                {
                    Id = item.MenuItem?.Id ?? Guid.Empty,
                    Title = item.MenuItem?.title ?? string.Empty,
                    Price = item.MenuItem?.price ?? 0,
                    Quantity = item.Quantity,
                    ImageUrl = item.MenuItem?.img
                }).ToList() ?? new List<MenuCartItemDto>()
            };
        }
        [HttpDelete("user/{userId}/remove-item/{itemId}")]
        [SwaggerOperation(Summary = "Удалить товар из корзины", Description = "Удаляет товар из корзины по его идентификатору.")]
        [SwaggerResponse(200, "Товар успешно удален", typeof(CartDto))]
        [SwaggerResponse(404, "Товар не найден")]
        public async Task<ActionResult<CartDto>> RemoveItemFromCart(Guid userId, Guid itemId)
        {
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                return NotFound("Корзина не найдена для пользователя");
            }

            var cartItem = await _cartItemService.GetCartItemByIdAsync(itemId);
            if (cartItem == null || cartItem.CartId != cart.Id)
            {
                return NotFound("Товар не найден в корзине");
            }

            try
            {
                await _cartItemService.DeleteCartItemAsync(itemId);

                await _cartService.UpdateCartTotalAsync(cart.Id);

                var updatedCart = await _cartService.GetCartByIdAsync(cart.Id);
                return Ok(MapToCartDto(updatedCart));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Произошла ошибка при удалении товара из корзины");
            }
        }

        [HttpDelete("user/{userId}/clear")]
        [SwaggerOperation(Summary = "Очистить корзину", Description = "Удаляет все товары из корзины.")]
        [SwaggerResponse(200, "Корзина очищена")]
        [SwaggerResponse(404, "Корзина не найдена")]
        public async Task<ActionResult<CartDto>> ClearCart(Guid userId)
        {
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                return NotFound("Корзина не найдена для пользователя");
            }

            // Удаляем все элементы корзины
            foreach (var item in cart.Items.ToList())
            {
                await _cartItemService.DeleteCartItemAsync(item.Id);
            }

            cart.Items.Clear();
            cart.totalPrice = 0;

            var updatedCart = await _cartService.UpdateCartAsync(cart);

            var cartDto = new CartDto
            {
                Id = updatedCart.Id,
                UserId = updatedCart.user_id,
                TotalPrice = updatedCart.totalPrice,
                //OrderId = updatedCart.Order?.Id,
                Items = new List<MenuCartItemDto>()
            };

            return Ok(cartDto);
        }
    }
}