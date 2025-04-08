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
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly IOrderItemService _orderItemService;
        private readonly ICartItemService _cartItemService;

        public OrderController(
            IOrderService orderService,
            ICartService cartService,
            IOrderItemService orderItemService,
            ICartItemService cartItemService)
        {
            _orderService = orderService;
            _cartService = cartService;
            _orderItemService = orderItemService;
            _cartItemService = cartItemService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Получить все заказы", Description = "Возвращает информацию о всех заказах.")]
        [SwaggerResponse(200, "Заказы успешно возвращены", typeof(List<OrderDto>))]
        [SwaggerResponse(404, "Заказы не найдены")]
        public async Task<ActionResult<List<OrderDto>>> GetAllOrders()
        {
            var orders = await _orderService.GetOrdersAsync();
            var orderDtos = new List<OrderDto>();

            foreach (var order in orders)
            {
                var orderItems = await _orderItemService.GetOrderItemsByOrderIdAsync(order.Id);
                orderDtos.Add(MapToOrderDto(order, orderItems));
            }

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

            var orderItems = await _orderItemService.GetOrderItemsByOrderIdAsync(order.Id);
            return Ok(MapToOrderDto(order, orderItems));
        }

        [HttpGet("user/{userId}")]
        [SwaggerOperation(Summary = "Получить заказы пользователя", Description = "Возвращает информацию о заказах пользователя по идентификатору пользователя.")]
        [SwaggerResponse(200, "Заказы успешно возвращены", typeof(List<OrderDto>))]
        [SwaggerResponse(404, "Заказы не найдены")]
        public async Task<ActionResult<List<OrderDto>>> GetOrdersByUserId(Guid userId)
        {
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            var orderDtos = new List<OrderDto>();

            foreach (var order in orders)
            {
                var orderItems = await _orderItemService.GetOrderItemsByOrderIdAsync(order.Id);
                orderDtos.Add(MapToOrderDto(order, orderItems));
            }

            return Ok(orderDtos);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Создать заказ", Description = "Создает новый заказ из текущей корзины.")]
        [SwaggerResponse(201, "Заказ успешно создан", typeof(OrderDto))]
        [SwaggerResponse(400, "Невозможно создать заказ с пустой корзиной")]
        [SwaggerResponse(404, "Корзина не найдена")]
        public async Task<ActionResult<OrderDto>> CreateOrder(OrderCreateDto orderCreateDto)
        {
            var cart = await _cartService.GetCartByIdAsync(orderCreateDto.CartId);
            if (cart == null)
            {
                return NotFound("Корзина не найдена");
            }

            var cartItems = await _cartItemService.GetCartItemsByCartIdAsync(cart.Id);
            if (cartItems == null || !cartItems.Any())
            {
                return BadRequest("Невозможно создать заказ с пустой корзиной");
            }

            var order = new Order
            {
                CartId = orderCreateDto.CartId,
                user_id = cart.user_id,
                status = "В обработке",
                paymentMethod = orderCreateDto.PaymentMethod,
                orderNumber = GenerateOrderNumber()
            };

            var createdOrder = await _orderService.AddOrderAsync(order);
            await _orderItemService.CreateOrderItemsFromCartItems(createdOrder.Id, cart.Id);

            // Очищаем корзину
            await _cartItemService.ClearCartAsync(cart.Id);
            cart.totalPrice = 0;
            await _cartService.UpdateCartAsync(cart);

            var orderItems = await _orderItemService.GetOrderItemsByOrderIdAsync(createdOrder.Id);
            var orderDto = MapToOrderDto(createdOrder, orderItems);

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

            var orderItems = await _orderItemService.GetOrderItemsByOrderIdAsync(updatedOrder.Id);
            return Ok(MapToOrderDto(updatedOrder, orderItems));
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

            // Удаляем связанные OrderItems
            await _orderItemService.DeleteOrderItemsByOrderIdAsync(id);
            await _orderService.DeleteOrderAsync(id);

            return NoContent();
        }

        private OrderDto MapToOrderDto(Order order, List<OrderItem> orderItems)
        {
            return new OrderDto
            {
                Id = order.Id,
                CartId = order.CartId,
                UserId = order.user_id,
                Status = order.status,
                OrderNumber = order.orderNumber,
                PaymentMethod = order.paymentMethod,
                TotalPrice = orderItems.Sum(oi => oi.PriceAtOrderTime * oi.Quantity),
                CreatedAt = order.CreatedAt,
                Items = orderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    MenuItemId = oi.MenuItemId,
                    Title = oi.MenuItem?.title ?? "Неизвестный товар",
                    Description = oi.MenuItem?.description ?? string.Empty,
                    Price = oi.PriceAtOrderTime,
                    ImageUrl = oi.MenuItem?.img,
                    Quantity = oi.Quantity
                }).ToList()
            };
        }

        private string GenerateOrderNumber()
        {
            return $"{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";
        }
    }
}