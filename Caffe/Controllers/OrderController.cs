using Microsoft.AspNetCore.Mvc;
using Caffe.Models;
using Caffe.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caffe.Models.Dto;
using Swashbuckle.AspNetCore.Annotations;

namespace Caffe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;

        public OrderController(IOrderService orderService, ICartService cartService)
        {
            _orderService = orderService;
            _cartService = cartService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Получить все заказы", Description = "Возвращает информацию о всех заказах.")]
        [SwaggerResponse(200, "Заказы успешно возвращены", typeof(List<OrderDto>))]
        [SwaggerResponse(404, "Заказы не найдены")]
        public async Task<ActionResult<List<OrderDto>>> GetAllOrders()
        {
            var orders = await _orderService.GetOrdersAsync();
            var orderDtos = orders.Select(order => new OrderDto
            {
                Id = order.Id,
                CartId = order.CartId,
                UserId = order.user_id,
                Status = order.status,
                OrderNumber = order.orderNumber,
                PaymentMethod = order.paymentMethod,
                TotalPrice = order.Cart?.totalPrice,
                CreatedAt = order.CreatedAt,
                Items = order.Cart?.Items?.Select(item => new MenuCartItemDto
                {
                    Id = item.MenuItem.Id,
                    Title = item.MenuItem.title,
                    Description = item.MenuItem.description,
                    Price = item.MenuItem.price,
                    ImageUrl = item.MenuItem.img,
                    IsAvailable = item.MenuItem.is_availble,
                    Quantity = item.Quantity
                }).ToList() ?? new List<MenuCartItemDto>()
            }).ToList();

            return Ok(orderDtos);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Получить заказ по идентификатору", Description = "Возвращает информацию о заказе по идентификатору.")]
        [SwaggerResponse(200, "Заказ успешно возвращен", typeof(OrderDto))]
        [SwaggerResponse(404, "Заказ не найден")]
        public async Task<ActionResult<OrderDto>> GetOrderById(Guid id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            var orderDto = new OrderDto
            {
                Id = order.Id,
                CartId = order.CartId,
                UserId = order.user_id,
                Status = order.status,
                OrderNumber = order.orderNumber,
                PaymentMethod = order.paymentMethod,
                TotalPrice = order.Cart?.totalPrice,
                CreatedAt = order.CreatedAt,
                Items = order.Cart?.Items?.Select(item => new MenuCartItemDto
                {
                    Id = item.MenuItem.Id,
                    Title = item.MenuItem.title,
                    Description = item.MenuItem.description,
                    Price = item.MenuItem.price,
                    ImageUrl = item.MenuItem.img,
                    IsAvailable = item.MenuItem.is_availble,
                    Quantity = item.Quantity
                }).ToList() ?? new List<MenuCartItemDto>()
            };

            return Ok(orderDto);
        }

        [HttpGet("user/{userId}")]
        [SwaggerOperation(Summary = "Получить заказы пользователя", Description = "Возвращает информацию о заказах пользователя по идентификатору пользователя.")]
        [SwaggerResponse(200, "Заказы успешно возвращены", typeof(List<OrderDto>))]
        [SwaggerResponse(404, "Заказы не найдены")]
        public async Task<ActionResult<List<OrderDto>>> GetOrdersByUserId(Guid userId)
        {
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            var orderDtos = orders.Select(order => new OrderDto
            {
                Id = order.Id,
                CartId = order.CartId,
                UserId = order.user_id,
                Status = order.status,
                OrderNumber = order.orderNumber,
                PaymentMethod = order.paymentMethod,
                TotalPrice = order.Cart?.totalPrice,
                CreatedAt = order.CreatedAt,
                Items = order.Cart?.Items?.Select(item => new MenuCartItemDto
                {
                    Id = item.MenuItem.Id,
                    Title = item.MenuItem.title,
                    Description = item.MenuItem.description,
                    Price = item.MenuItem.price,
                    ImageUrl = item.MenuItem.img,
                    IsAvailable = item.MenuItem.is_availble,
                    Quantity = item.Quantity
                }).ToList() ?? new List<MenuCartItemDto>()
            }).ToList();

            return Ok(orderDtos);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Создать заказ", Description = "Добавляет заказ в систему.")]
        [SwaggerResponse(200, "Заказ успешно создан", typeof(OrderDto))]
        [SwaggerResponse(400, "Невозможно создать заказ с пустой корзиной")]
        [SwaggerResponse(404, "Корзина не найдена")]
        public async Task<ActionResult<OrderDto>> CreateOrder(OrderCreateDto orderCreateDto)
        {
            var cart = await _cartService.GetCartByIdAsync(orderCreateDto.CartId);
            if (cart == null)
            {
                return NotFound("Корзина не найдена");
            }

            if (cart.Items == null || !cart.Items.Any())
            {
                return BadRequest("Невозможно создать заказ с пустой корзиной");
            }

            var order = new Order
            {
                CartId = orderCreateDto.CartId,
                user_id = cart.user_id,
                status = "В обработке", // Начальный статус
                paymentMethod = orderCreateDto.PaymentMethod,
                orderNumber = new Random().Next(10000, 99999)
            };

            var createdOrder = await _orderService.AddOrderAsync(order);

            // Связываем заказ с корзиной
            cart.Order = createdOrder;
            await _cartService.UpdateCartAsync(cart);

            var orderDto = new OrderDto
            {
                Id = createdOrder.Id,
                CartId = createdOrder.CartId,
                UserId = createdOrder.user_id,
                Status = createdOrder.status,
                OrderNumber = createdOrder.orderNumber,
                PaymentMethod = createdOrder.paymentMethod,
                TotalPrice = cart.totalPrice,
                CreatedAt = createdOrder.CreatedAt,
                Items = cart.Items.Select(item => new MenuCartItemDto
                {
                    Id = item.MenuItem.Id,
                    Title = item.MenuItem.title,
                    Description = item.MenuItem.description,
                    Price = item.MenuItem.price,
                    ImageUrl = item.MenuItem.img,
                    IsAvailable = item.MenuItem.is_availble,
                    Quantity = item.Quantity
                }).ToList()
            };

            return CreatedAtAction(nameof(GetOrderById), new { id = orderDto.Id }, orderDto);
        }

        [HttpPut("{id}/status")]
        [SwaggerOperation(Summary = "Обновить статус заказа", Description = "Обновляет статус заказа.")]
        [SwaggerResponse(200, "Статус заказа успешно обновлен", typeof(OrderDto))]
        [SwaggerResponse(404, "Заказ не найден")]
        public async Task<ActionResult<OrderDto>> UpdateOrderStatus(Guid id, OrderStatusUpdateDto statusUpdateDto)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.status = statusUpdateDto.Status;
            var updatedOrder = await _orderService.UpdateOrderAsync(order);

            var orderDto = new OrderDto
            {
                Id = updatedOrder.Id,
                CartId = updatedOrder.CartId,
                UserId = updatedOrder.user_id,
                Status = updatedOrder.status,
                OrderNumber = updatedOrder.orderNumber,
                PaymentMethod = updatedOrder.paymentMethod,
                TotalPrice = updatedOrder.Cart?.totalPrice,
                CreatedAt = updatedOrder.CreatedAt,
                Items = updatedOrder.Cart?.Items?.Select(item => new MenuCartItemDto
                {
                    Id = item.MenuItem.Id,
                    Title = item.MenuItem.title,
                    Description = item.MenuItem.description,
                    Price = item.MenuItem.price,
                    ImageUrl = item.MenuItem.img,
                    IsAvailable = item.MenuItem.is_availble,
                    Quantity = item.Quantity
                }).ToList() ?? new List<MenuCartItemDto>()
            };

            return Ok(orderDto);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Удалить заказ", Description = "Удаляет заказ по его идентификатору.")]
        [SwaggerResponse(204, "Заказ успешно удален")]
        [SwaggerResponse(404, "Заказ не найден")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            await _orderService.DeleteOrderAsync(id);
            return NoContent();
        }
    }
}