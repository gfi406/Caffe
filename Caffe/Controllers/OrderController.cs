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
                Items = order.Cart?.Items?.Select(item => new MenuItemDto
                {
                    Id = item.Id,
                    Title = item.title,
                    Description = item.description,
                    Price = item.price,
                    ImageUrl = item.img,
                    IsAvailable = item.is_availble
                }).ToList() ?? new List<MenuItemDto>()
            }).ToList();

            return Ok(orderDtos);
        }

        [HttpGet("{id}")]
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
                Items = order.Cart?.Items?.Select(item => new MenuItemDto
                {
                    Id = item.Id,
                    Title = item.title,
                    Description = item.description,
                    Price = item.price,
                    ImageUrl = item.img,
                    IsAvailable = item.is_availble
                }).ToList() ?? new List<MenuItemDto>()
            };

            return Ok(orderDto);
        }

        [HttpGet("user/{userId}")]
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
                Items = order.Cart?.Items?.Select(item => new MenuItemDto
                {
                    Id = item.Id,
                    Title = item.title,
                    Description = item.description,
                    Price = item.price,
                    ImageUrl = item.img,
                    IsAvailable = item.is_availble
                }).ToList() ?? new List<MenuItemDto>()
            }).ToList();

            return Ok(orderDtos);
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder(OrderCreateDto orderCreateDto)
        {
            var cart = await _cartService.GetCartByIdAsync(orderCreateDto.CartId);
            if (cart == null)
            {
                return NotFound("Cart not found");
            }

            if (cart.Items == null || !cart.Items.Any())
            {
                return BadRequest("Cannot create order with empty cart");
            }

            var order = new Order
            {
                CartId = orderCreateDto.CartId,
                user_id = cart.user_id,
                status = "Pending", // Initial status
                paymentMethod = orderCreateDto.PaymentMethod,
                // Order number generation logic could be more sophisticated
                orderNumber = new Random().Next(10000, 99999)
            };

            var createdOrder = await _orderService.AddOrderAsync(order);

            // Associate the order with the cart
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

            return CreatedAtAction(nameof(GetOrderById), new { id = orderDto.Id }, orderDto);
        }

        [HttpPut("{id}/status")]
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
                Items = updatedOrder.Cart?.Items?.Select(item => new MenuItemDto
                {
                    Id = item.Id,
                    Title = item.title,
                    Description = item.description,
                    Price = item.price,
                    ImageUrl = item.img,
                    IsAvailable = item.is_availble
                }).ToList() ?? new List<MenuItemDto>()
            };

            return Ok(orderDto);
        }

        [HttpDelete("{id}")]
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
