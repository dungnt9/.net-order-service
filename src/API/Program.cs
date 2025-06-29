using Microsoft.EntityFrameworkCore;
using Application;
using Infrastructure;
using Infrastructure.Persistence;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    try
    {
        Console.WriteLine("🚀 Starting MySQL...");
        Process.Start(new ProcessStartInfo("docker-compose", "up -d") { UseShellExecute = false });
        await Task.Delay(10000); // Wait 10 seconds for OrderService
        Console.WriteLine("✅ MySQL started!");
    }
    catch { Console.WriteLine("⚠️ Run 'docker-compose up -d' manually"); }
}

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Application & Infrastructure services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// CORS for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Dev")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Auto migrate database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.EnsureDeletedAsync();
    await context.Database.EnsureCreatedAsync();
    
    if (!context.Orders.Any())
    {
        var sqlFile = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "data.sql");
        if (File.Exists(sqlFile))
        {
            var sql = await File.ReadAllTextAsync(sqlFile);
            await context.Database.ExecuteSqlRawAsync(sql);
            Console.WriteLine("✅ Data loaded from data.sql!");
        }
    }
}

app.Run();