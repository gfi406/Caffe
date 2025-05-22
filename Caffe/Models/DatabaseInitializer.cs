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
        
        await _context.Database.MigrateAsync();

        
        if (!_context.Users.Any() && !_context.MenuItems.Any() && !_context.Carts.Any() && !_context.Orders.Any())
        {
            var random = new Random();

            
            var users = new[]
            {                
                new User
                {
                    is_admin = false,
                    is_active = true,
                    name = "John Doe",
                    email = "john.doe@caffe.com",
                    password = "password",
                    phone = "987654321"
                }
            };

            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync(); 

            var menuItems = new[]
            {
                new MenuItem
                {
                    is_availble = true,
                    title = "Espresso",
                    description = "Strong black coffee",
                    img = "",
                    price = 150,
                    category = FoodCategory.Beverage 

                },
                new MenuItem
                {
                    is_availble = true,
                    title = "Latte",
                    description = "Coffee with steamed milk",
                    img = "",
                    price = 200,
                    category = FoodCategory.Beverage
                },
                new MenuItem
                {
                    is_availble = true,
                    title = "Cappuccino",
                    description = "Coffee with steamed milk and foam",
                    img = "",
                    price = 220,
                    category = FoodCategory.Beverage
                }
            };

            await _context.MenuItems.AddRangeAsync(menuItems);
            await _context.SaveChangesAsync(); 

           
            var userJohn = users.First(u => u.email == "john.doe@caffe.com");
            var cart = new Cart
            {
                User = userJohn,
                user_id = userJohn.Id,
               
            };

            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync(); 

            
        }
    }
}
