using Caffe;
using Caffe.Service;
using Caffe.Service.Impl;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
Console.WriteLine("Started");

//Мусор для миграций 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});


try
{
    // var connection = ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false");
    // for container
    var connection = ConnectionMultiplexer.Connect("redis:6379,abortConnect=false");
    builder.Services.AddSingleton<IConnectionMultiplexer>(connection);
    builder.Services.AddScoped<IDatabase>(sp =>
    sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase());
    Console.WriteLine("✅ Redis connected!");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error connecting to Redis: {ex.Message}");
}


builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.DocInclusionPredicate((docName, apiDesc) => true);
});

builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IMenuItemService, MenuItemService>();
builder.Services.AddScoped<ICartItemService, CartItemService>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();

builder.Services.AddEndpointsApiExplorer();

Console.WriteLine("Building...");
var app = builder.Build();
Console.WriteLine("✅ Building complete!");

app.UseRouting();
Console.WriteLine("✅ Routing configured!");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        dbContext.Database.Migrate();
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
//app.UseSwaggerUI();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Notes API");
    options.RoutePrefix = string.Empty;  
});


app.UseCors("AllowAll");

app.UseRouting();

app.MapControllers();
Console.WriteLine("✅ Controllers initialized!");

Console.WriteLine("🚀 App is running!");
app.MapGet("/", () => "Hello World!");

app.Run();

