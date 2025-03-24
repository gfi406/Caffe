using Microsoft.AspNetCore.Mvc;
using Caffe.Models;
using Caffe.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caffe.Models.Dto;
using Swashbuckle.AspNetCore.Annotations;
using static System.Net.Mime.MediaTypeNames;
using System;
using System.Drawing;
using System.IO;

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
                UserIcon = user.UserIcon,
                CartId = user.Cart?.Id,
                OrderIds = user.Orders?.Select(o => o.Id).ToList() ?? new List<Guid>()
            };
            byte[] imageBytes = user.UserIcon;
            string outputPath = "path_to_save_image.png";
            SaveImageFromBytes(imageBytes, outputPath);
            return Ok(userDto);
        }

        [HttpPost("login")]
        [SwaggerOperation(Summary = "Вход пользователя", Description = "Аутентификация пользователя по email и паролю.")]
        [SwaggerResponse(200, "Вход выполнен успешно", typeof(UserDto))]
        [SwaggerResponse(401, "Неверный email или пароль")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            // Предполагается, что в IUserService есть метод для проверки учетных данных
            var user = await _userService.AuthenticateUserAsync(loginDto.Email, loginDto.Password);

            if (user == null)
            {
                return Unauthorized("Неверный email или пароль");
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.name,
                Email = user.email,
                Phone = user.phone,
                IsAdmin = user.is_admin,
                IsActive = user.is_active,
                UserIcon = user.UserIcon,
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
                password = userCreateDto.Password, 
                phone = userCreateDto.Phone,
                is_admin = userCreateDto.IsAdmin,
                is_active = userCreateDto.IsActive,
                
            };

            var createdUser = await _userService.AddUserAsync(user);

            var userDto = new UserDto
            {
                Id = createdUser.Id,
                Name = createdUser.name,
                Email = createdUser.email,
                Phone = createdUser.phone,
                IsAdmin = createdUser.is_admin,
                IsActive = createdUser.is_active,
                
            };

            return CreatedAtAction(nameof(GetUserById), new { id = userDto.Id }, userDto);
        }

        [HttpPost("{id}/upload-icon")]
        [SwaggerOperation(Summary = "Загрузить иконку пользователя", Description = "Загружает иконку пользователя в формате изображения (.jpg, .png).")]
        [SwaggerResponse(200, "Файл успешно загружен")]
        [SwaggerResponse(400, "Ошибка загрузки файла")]
        public async Task<IActionResult> UploadUserIcon(Guid id, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Файл не выбран");
            }

            // Разрешенные типы
            var allowedExtensions = new List<string> { ".jpg", ".jpeg", ".png" };
            var allowedContentTypes = new List<string> { "image/jpeg", "image/png" };

            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            var contentType = file.ContentType.ToLower();

            if (!allowedExtensions.Contains(fileExtension) || !allowedContentTypes.Contains(contentType))
            {
                return BadRequest("Неверный формат файла. Разрешены только .jpg и .png");
            }

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("Пользователь не найден");
            }

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                user.UserIcon = memoryStream.ToArray();
                user.FileName = file.FileName;
                user.ContentType = file.ContentType;
            }

            await _userService.UpdateUserAsync(user);
            return Ok("Файл загружен");
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
        public static void SaveImageFromBytes(byte[] imageBytes, string outputPath)
        {
            // Проверяем, что массив байтов не пустой
            if (imageBytes == null || imageBytes.Length == 0)
            {
                throw new ArgumentException("The byte array is empty or null.", nameof(imageBytes));
            }

            // Создаем поток из байтов
            using (var ms = new MemoryStream(imageBytes))
            {
                try
                {
                    // Указываем полное имя класса Image для работы с изображениями
                    var image = System.Drawing.Image.FromStream(ms);

                    // Сохраняем изображение в файл
                    image.Save(outputPath);
                }
                catch (Exception ex)
                {
                    throw new Exception("An error occurred while saving the image.", ex);
                }
            }
        }
    }
}
