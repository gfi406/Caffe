using Caffe;
using Caffe.Service;
using Caffe.Service.Impl;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Добавление контекста БД
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Добавление контроллеров
builder.Services.AddControllers();

// Добавление CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Регистрация сервисов
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IMenuItemService, MenuItemService>(); // Исправлена кириллическая буква

// Swagger для тестирования API
builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();

// Инициализация базы данных
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        dbContext.Database.CanConnect();
        Console.WriteLine("Connection is Ok");
        var initializer = new DatabaseInitializer(dbContext);
        await initializer.InitializeAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        throw new Exception("Connection error");
    }
}



// Применение CORS
app.UseCors("AllowAll");

// Маршрутизация
app.UseRouting();

// Авторизация (если нужна)
// app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => "Hello World!");

app.Run();