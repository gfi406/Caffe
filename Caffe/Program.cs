using Caffe;
using Caffe.Service;
using Caffe.Service.Impl;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
Console.WriteLine("Started");

 //🔹 Получаем и конвертируем строку подключения к PostgreSQL
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (string.IsNullOrEmpty(databaseUrl))
{
    throw new Exception("DATABASE_URL is missing!");
}

var connectionString = ConvertPostgresUrlToConnectionString(databaseUrl);
Console.WriteLine($"🔍 Converted Connection String: {connectionString}");

// 🔹 Добавление контекста БД
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

//Мусор для миграций 
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔹 Добавление контроллеров
builder.Services.AddControllers();

// 🔹 Добавление CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// ✅ Подключение к Redis
//try
//{
//    string redisUrl = Environment.GetEnvironmentVariable("REDIS_URL");
//    Console.WriteLine($"📌 REDIS_URL: {redisUrl}");

//    if (string.IsNullOrEmpty(redisUrl))
//    {
//        throw new Exception("❌ REDIS_URL is missing!");
//    }

//    // Изменяем строку подключения
//    string redisConnectionString = redisUrl.Replace("redis://default@", ""); // Убираем "redis://default@"
//    redisConnectionString += ",abortConnect=false"; // <== Добавляем abortConnect=false

//    Console.WriteLine($"📌 Connecting to Redis: {redisConnectionString}");

//    var connection = ConnectionMultiplexer.Connect(redisConnectionString);
//    builder.Services.AddSingleton<IConnectionMultiplexer>(connection);
//    Console.WriteLine("✅ Redis connected!");
//}
//catch (Exception ex)
//{
//    Console.WriteLine($"❌ Error connecting to Redis: {ex.Message}");
//}


// 🔹 Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.DocInclusionPredicate((docName, apiDesc) => true);
});

// 🔹 Регистрация сервисов
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IMenuItemService, MenuItemService>();

// 🔹 Swagger для тестирования API
builder.Services.AddEndpointsApiExplorer();

Console.WriteLine("Building...");
var app = builder.Build();
Console.WriteLine("✅ Building complete!");

app.UseRouting();
Console.WriteLine("✅ Routing configured!");

// 🔹 Инициализация базы данных
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        dbContext.Database.CanConnect();
        Console.WriteLine("✅ Database connection is OK!");
        var initializer = new DatabaseInitializer(dbContext);
        await initializer.InitializeAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Database Error: {ex.Message}");
        throw new Exception("Connection error");
    }
}


app.UseSwagger();
app.UseSwaggerUI();


// 🔹 Применение CORS
app.UseCors("AllowAll");

// 🔹 Маршрутизация
app.UseRouting();

app.MapControllers();
Console.WriteLine("✅ Controllers initialized!");

Console.WriteLine("🚀 App is running!");
app.MapGet("/", () => "Hello World!");

app.Run();

// 🔹 Функция конвертации строки подключения PostgreSQL
static string ConvertPostgresUrlToConnectionString(string url)
{
    if (string.IsNullOrEmpty(url))
        throw new Exception("DATABASE_URL is empty!");

    var uri = new Uri(url);
    var userInfo = uri.UserInfo.Split(':');

    return $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SslMode=Disable";
}