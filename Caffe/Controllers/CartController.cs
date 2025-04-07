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

            var cartDto = new CartDto
            {
                Id = cart.Id,
                UserId = cart.user_id,
                TotalPrice = cart.totalPrice,
                OrderId = cart.Order?.Id,
                Items = cart.Items?.Select(item => new MenuCartItemDto
                {
                    Id = item.MenuItem.Id,
                    Title = item.MenuItem.title,
                    Description = item.MenuItem.description,
                    Price = item.MenuItem.price,
                    ImageUrl = item.MenuItem.img,
                    IsAvailable = item.MenuItem.is_availble,
                    Category = item.MenuItem.category,
                    Quantity = item.Quantity
                }).ToList() ?? new List<MenuCartItemDto>()
            };

            return Ok(cartDto);
        }

        [HttpGet("user/{userId}")]
        [SwaggerOperation(Summary = "Получить корзину пользователя", Description = "Возвращает корзину пользователя по идентификатору.")]
        [SwaggerResponse(200, "Корзина успешно возвращена", typeof(CartDto))]
        [SwaggerResponse(404, "Корзина не найдена")]
        public async Task<ActionResult<CartDto>> GetCartByUserId(Guid userId)
        {
            // Получаем корзину без элементов (только основную информацию)
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                return NotFound("Корзина не найдена");
            }

            // Получаем элементы корзины с полной информацией о товарах
            var cartItems = await _cartItemService.GetCartItemsByCartIdAsync(cart.Id);

            // Маппим в DTO с защитой от null
            var cartDto = new CartDto
            {
                Id = cart.Id,
                UserId = cart.user_id,
                TotalPrice = cart.totalPrice,
                OrderId = cart.Order?.Id,
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

        //[HttpPost("user/{userId}/add-item")]
        //[SwaggerOperation(Summary = "Добавить товар в корзину", Description = "Добавляет товар в корзину пользователя.")]
        //[SwaggerResponse(201, "Товар успешно добавлен", typeof(CartDto))]
        //[SwaggerResponse(400, "Ошибка при добавлении товара")]
        //public async Task<ActionResult<CartDto>> AddItemToCart(Guid userId, AddToCartDto addToCartDto)
        //{
        //    var cart = await _cartService.GetCartByUserIdAsync(userId);
        //    if (cart == null)
        //    {
        //        return NotFound("Корзина не найдена для пользователя");
        //    }

        //    var menuItem = await _menuItemService.GetMenuItemByIdAsync(addToCartDto.MenuItemId);
        //    if (menuItem == null)
        //    {
        //        return NotFound("Товар не найден");
        //    }

        //    // Проверяем, есть ли уже такой товар в корзине
        //    var existingCartItem = cart.Items?.FirstOrDefault(i => i.MenuItemId == addToCartDto.MenuItemId);

        //    if (existingCartItem != null)
        //    {
        //        // Увеличиваем количество существующего товара
        //        existingCartItem.Quantity += addToCartDto.Quantity;
        //    }
        //    else
        //    {
        //        // Создаем новый элемент корзины
        //        var newCartItem = new CartItem
        //        {
        //            CartId = cart.Id,
        //            MenuItemId = menuItem.Id,
        //            Quantity = addToCartDto.Quantity
        //        };

        //        if (cart.Items == null)
        //        {
        //            cart.Items = new List<CartItem>();
        //        }

        //        cart.Items.Add(newCartItem);
        //    }

        //    // Пересчитываем общую стоимость
        //    cart.totalPrice = cart.Items.Sum(item => item.MenuItem.price * item.Quantity);

        //    var updatedCart = await _cartService.UpdateCartAsync(cart);

        //    var cartDto = new CartDto
        //    {
        //        Id = updatedCart.Id,
        //        UserId = updatedCart.user_id,
        //        TotalPrice = updatedCart.totalPrice,
        //        OrderId = updatedCart.Order?.Id,
        //        Items = updatedCart.Items?.Select(item => new MenuCartItemDto
        //        {
        //            Id = item.MenuItem.Id,
        //            Title = item.MenuItem.title,
        //            Description = item.MenuItem.description,
        //            Price = item.MenuItem.price,
        //            ImageUrl = item.MenuItem.img,
        //            IsAvailable = item.MenuItem.is_availble,
        //            Category = item.MenuItem.category,
        //            Quantity = item.Quantity
        //        }).ToList() ?? new List<MenuCartItemDto>()
        //    };

        //    return Ok(cartDto);
        //}

        //[HttpDelete("user/{userId}/remove-item/{itemId}")]
        //[SwaggerOperation(Summary = "Удалить товар из корзины", Description = "Удаляет товар из корзины по его идентификатору.")]
        //[SwaggerResponse(200, "Товар успешно удален")]
        //[SwaggerResponse(404, "Товар не найден")]
        //public async Task<ActionResult<CartDto>> RemoveItemFromCart(Guid userId, Guid itemId)
        //{
        //    var cart = await _cartService.GetCartByUserIdAsync(userId);
        //    if (cart == null)
        //    {
        //        return NotFound("Корзина не найдена для пользователя");
        //    }

        //    // Находим и удаляем товар из корзины
        //    var itemToRemove = cart.Items?.FirstOrDefault(i => i.Id == itemId);
        //    if (itemToRemove == null)
        //    {
        //        return NotFound("Товар не найден в корзине");
        //    }

        //    cart.Items.Remove(itemToRemove);
        //    await _cartItemService.DeleteCartItemAsync(itemId);

        //    // Пересчитываем общую стоимость
        //    cart.totalPrice = cart.Items?.Sum(item => item.MenuItem.price * item.Quantity) ?? 0;

        //    var updatedCart = await _cartService.UpdateCartAsync(cart);

        //    var cartDto = new CartDto
        //    {
        //        Id = updatedCart.Id,
        //        UserId = updatedCart.user_id,
        //        TotalPrice = updatedCart.totalPrice,
        //        OrderId = updatedCart.Order?.Id,
        //        Items = updatedCart.Items?.Select(item => new MenuCartItemDto
        //        {
        //            Id = item.MenuItem.Id,
        //            Title = item.MenuItem.title,
        //            Description = item.MenuItem.description,
        //            Price = item.MenuItem.price,
        //            ImageUrl = item.MenuItem.img,
        //            IsAvailable = item.MenuItem.is_availble,
        //            Category = item.MenuItem.category,
        //            Quantity = item.Quantity
        //        }).ToList() ?? new List<MenuCartItemDto>()
        //    };

        //    return Ok(cartDto);
        //}
        [HttpPost("user/{userId}/add-item")]
        [SwaggerOperation(Summary = "Добавить товар в корзину", Description = "Добавляет товар в корзину пользователя.")]
        [SwaggerResponse(201, "Товар успешно добавлен", typeof(CartDto))]
        [SwaggerResponse(400, "Ошибка при добавлении товара")]
        
        public async Task<ActionResult<CartDto>> AddItemToCart(Guid userId, AddToCartDto addToCartDto)
        {
            // Валидация входных данных
            if (addToCartDto.Quantity <= 0)
            {
                return BadRequest("Количество должно быть больше нуля");
            }

            // Получаем или создаем корзину
            var cart = await _cartService.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                return BadRequest("Не удалось создать корзину");
            }

            // Получаем товар из меню
            var menuItem = await _menuItemService.GetMenuItemByIdAsync(addToCartDto.MenuItemId);
            if (menuItem == null)
            {
                return NotFound("Товар не найден");
            }

            // Проверяем доступность товара
            if (!menuItem.is_availble)
            {
                return BadRequest("Товар временно недоступен");
            }

            try
            {
                // Добавляем товар в корзину
                var cartItem = await _cartService.AddItemToCartAsync(
                    cart.Id,
                    menuItem.Id,
                    addToCartDto.Quantity,
                    menuItem.price);

                // Обновляем общую стоимость корзины
                await _cartService.UpdateCartTotalAsync(cart.Id);

                // Возвращаем обновленную корзину
                var updatedCart = await _cartService.GetCartByIdAsync(cart.Id);
                return Ok(MapToCartDto(updatedCart));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Обработка конфликта параллельного доступа
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
                OrderId = updatedCart.Order?.Id,
                Items = new List<MenuCartItemDto>()
            };

            return Ok(cartDto);
        }
    }
}