using Caffe;
using Caffe.Models;
using Caffe.Models.Enum;
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
                    img = "https://upload.wikimedia.org/wikipedia/commons/4/45/A_small_cup_of_coffee.JPG",
                    price = 150,
                    category = FoodCategory.Beverage 

                },
                new MenuItem
                {
                    is_availble = true,
                    title = "Latte",
                    description = "Coffee with steamed milk",
                    img = "https://nescafe.ru/sites/default/files/2024-08/GettyImages-1466623971%20%281%29.jpg",
                    price = 200,
                    category = FoodCategory.Beverage
                },
                new MenuItem
                {
                    is_availble = true,
                    title = "Cappuccino",
                    description = "Coffee with steamed milk and foam",
                    img = "https://lorcoffee.com/cdn/shop/articles/Cappuccino-exc.jpg?v=1684870907",
                    price = 220,
                    category = FoodCategory.Beverage
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
               
            };

            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync(); // Сохраняем корзину

            //// Генерация заказа для пользователя
            //var order = new Order
            //{
            //    Cart = cart,
            //    CartId = cart.Id,
            //    User = userJohn,
            //    user_id = userJohn.Id,
            //    status = "Pending",
            //    orderNumber = 1001,
            //    paymentMethod = "Credit Card"
            //};

            //await _context.Orders.AddAsync(order);
            //await _context.SaveChangesAsync(); // Сохраняем заказ
        }
    }
}
