﻿using Microsoft.AspNetCore.Mvc;
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
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Получить всех пользователей", Description = "Возвращает информацию всех пользователях.")]
        [SwaggerResponse(200, "Пользователь успешно возвращен", typeof(UserDto))]
        [SwaggerResponse(404, "Пользователь не найден")]
        public async Task<ActionResult<List<UserDto>>> GetUsers()
        {
            var users = await _userService.GetUsersAsync();
            var userDtos = users.Select(user => new UserDto
            {
                Id = user.Id,
                Name = user.name,
                Email = user.email,
                Phone = user.phone,
                IsAdmin = user.is_admin,
                IsActive = user.is_active,
                CartId = user.Cart?.Id,
                OrderIds = user.Orders?.Select(o => o.Id).ToList() ?? new List<Guid>()
            }).ToList();

            return Ok(userDtos);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Получить пользователя идентификатору", Description = "Возвращает информацию о пользователе по идентификатору.")]
        [SwaggerResponse(200, "Пользователь успешно возвращен", typeof(UserDto))]
        [SwaggerResponse(404, "Пользователь не найден")]
        public async Task<ActionResult<UserDto>> GetUserById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.name,
                Email = user.email,
                Phone = user.phone,
                IsAdmin = user.is_admin,
                IsActive = user.is_active,
                CartId = user.Cart?.Id,
                OrderIds = user.Orders?.Select(o => o.Id).ToList() ?? new List<Guid>()
            };

            return Ok(userDto);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Добавить пользователя", Description = "Добавляет нового пользователя в систему.")]
        [SwaggerResponse(200, "Пользователь успешно добавлен", typeof(UserDto))]
        [SwaggerResponse(404, "Пользователь не найден")]
        public async Task<ActionResult<UserDto>> AddUser(UserCreateUpdateDto userCreateDto)
        {
            var user = new User
            {
                name = userCreateDto.Name,
                email = userCreateDto.Email,
                password = userCreateDto.Password, // Note: In a real app, this should be hashed
                phone = userCreateDto.Phone,
                is_admin = userCreateDto.IsAdmin,
                is_active = userCreateDto.IsActive
            };

            var createdUser = await _userService.AddUserAsync(user);

            var userDto = new UserDto
            {
                Id = createdUser.Id,
                Name = createdUser.name,
                Email = createdUser.email,
                Phone = createdUser.phone,
                IsAdmin = createdUser.is_admin,
                IsActive = createdUser.is_active
            };

            return CreatedAtAction(nameof(GetUserById), new { id = userDto.Id }, userDto);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Изменить пользователя идентификатору", Description = "Изменяет информацию о пользователе по идентификатору.")]
        [SwaggerResponse(200, "Пользователь успешно изменен", typeof(UserDto))]
        [SwaggerResponse(404, "Пользователь не найден")]
        public async Task<ActionResult<UserDto>> UpdateUser(Guid id, UserCreateUpdateDto userUpdateDto)
        {
            var existingUser = await _userService.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            existingUser.name = userUpdateDto.Name;
            existingUser.email = userUpdateDto.Email;
            existingUser.phone = userUpdateDto.Phone;
            existingUser.is_admin = userUpdateDto.IsAdmin;
            existingUser.is_active = userUpdateDto.IsActive;

            // Only update password if it's provided
            if (!string.IsNullOrEmpty(userUpdateDto.Password))
            {
                existingUser.password = userUpdateDto.Password; // Should be hashed in production
            }

            var updatedUser = await _userService.UpdateUserAsync(existingUser);

            var userDto = new UserDto
            {
                Id = updatedUser.Id,
                Name = updatedUser.name,
                Email = updatedUser.email,
                Phone = updatedUser.phone,
                IsAdmin = updatedUser.is_admin,
                IsActive = updatedUser.is_active,
                CartId = updatedUser.Cart?.Id,
                OrderIds = updatedUser.Orders?.Select(o => o.Id).ToList() ?? new List<Guid>()
            };

            return Ok(userDto);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Удалить пользователя", Description = "Удаляет пользователя по его идентификатору.")]
        [SwaggerResponse(200, "Пользователь успешно удален")]
        [SwaggerResponse(404, "Пользователь не найден")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            await _userService.DeleteUserAsync(id);
            return NoContent();
        }
    }
}
