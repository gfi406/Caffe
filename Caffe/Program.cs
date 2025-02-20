using Caffe;
using Caffe.Service;
using Caffe.Service.Impl;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
Console.WriteLine("Started");

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

//Connect Redis
try
{
    string redisConnectionString = Environment.GetEnvironmentVariable("REDIS_URL") ?? "localhost:6379,abortConnect=false";
    var connection = ConnectionMultiplexer.Connect(redisConnectionString);
    builder.Services.AddSingleton<IConnectionMultiplexer>(connection);
    Console.WriteLine("Redis ok");
}
catch (Exception ex)
{
    Console.WriteLine($"Error connecting to Redis: {ex.Message}");
}
//try
//{
//    var connection = ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false");
//    builder.Services.AddSingleton<IConnectionMultiplexer>(connection);
//    Console.WriteLine("Redis ok");
//}
//catch (Exception ex)
//{
//    Console.WriteLine($"Error connecting to Redis: {ex.Message}");
//}

//Swager
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.DocInclusionPredicate((docName, apiDesc) => true);
});


// Регистрация сервисов
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IMenuItemService, MenuItemService>(); // Исправлена кириллическая буква

// Swagger для тестирования API
builder.Services.AddEndpointsApiExplorer();


Console.WriteLine("Building");
var app = builder.Build();
Console.WriteLine("Building ok");
app.UseRouting();
Console.WriteLine("Routing ok");


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


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// Применение CORS
app.UseCors("AllowAll");

// Маршрутизация
app.UseRouting();

// Авторизация (если нужна)
// app.UseAuthorization();

app.MapControllers();
Console.WriteLine("Controllers ok");

Console.WriteLine("App running");
app.MapGet("/", () => "Hello World!");

app.Run();
Console.WriteLine("App Run!");