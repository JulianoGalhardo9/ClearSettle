using ClearSettle.Application.UseCases;
using ClearSettle.Domain.Interfaces;
using ClearSettle.Infrastructure.Data;
using ClearSettle.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SettlementDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<ITradeRepository, TradeRepository>();
builder.Services.AddScoped<ClearSettle.Infrastructure.Messaging.RabbitMqPublisher>();
builder.Services.AddTransient<RegisterPendingTradeUseCase>();

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddOpenApiDocument(configure =>
{
    configure.Title = "ClearSettle API";
    configure.Version = "v1";
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi(); 
    app.UseSwaggerUi(); 
}

app.UseAuthorization();
app.UseCors("AllowReactApp");
app.MapControllers();

app.Run();