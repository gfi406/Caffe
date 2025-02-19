using Caffe;
using Caffe.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

public class DatabaseInitializer
{
    private readonly ApplicationDbContext _context;

    public DatabaseInitializer(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task InitializeAsync()
    {
        // Применяем миграции, если они есть
        await _context.Database.MigrateAsync();

        // Проверяем, есть ли уже данные
        if (!_context.Users.Any() && !_context.MenuItems.Any() && !_context.Carts.Any() && !_context.Orders.Any())
        {
            var random = new Random();

            // Генерация пользователей
            var users = new[]
            {
                new User
                {
                    is_admin = true,
                    is_active = true,
                    name = "Admin User",
                    email = "admin@caffe.com",
                    password = "password", // В реальном проекте не хранить пароль в открытом виде
                    phone = "123456789"
                },
                new User
                {
                    is_admin = false,
                    is_active = true,
                    name = "John Doe",
                    email = "john.doe@caffe.com",
                    password = "password", // В реальном проекте не хранить пароль в открытом виде
                    phone = "987654321"
                }
            };

            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync(); // Сохраняем пользователей

            // Генерация товаров меню
            var menuItems = new[]
            {
                new MenuItem
                {
                    is_availble = true,
                    title = "Espresso",
                    description = "Strong black coffee",
                    img = "/images/espresso.jpg",
                    price = 150
                },
                new MenuItem
                {
                    is_availble = true,
                    title = "Latte",
                    description = "Coffee with steamed milk",
                    img = "/images/latte.jpg",
                    price = 200
                },
                new MenuItem
                {
                    is_availble = true,
                    title = "Cappuccino",
                    description = "Coffee with steamed milk and foam",
                    img = "/images/cappuccino.jpg",
                    price = 220
                }
            };

            await _context.MenuItems.AddRangeAsync(menuItems);
            await _context.SaveChangesAsync(); // Сохраняем товары меню

            // Генерация корзины для пользователя
            var userJohn = users.First(u => u.email == "john.doe@caffe.com");
            var cart = new Cart
            {
                User = userJohn,
                user_id = userJohn.Id,
                Items = new[] { menuItems.First(m => m.title == "Latte") }.ToList(),
                totalPrice = 200
            };

            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync(); // Сохраняем корзину

            // Генерация заказа для пользователя
            var order = new Order
            {
                Cart = cart,
                CartId = cart.Id,
                User = userJohn,
                user_id = userJohn.Id,
                status = "Pending",
                orderNumber = 1001,
                paymentMethod = "Credit Card"
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync(); // Сохраняем заказ
        }
    }
}
