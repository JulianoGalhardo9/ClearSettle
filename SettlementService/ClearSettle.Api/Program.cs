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
builder.Services.AddTransient<RegisterPendingTradeUseCase>();

builder.Services.AddControllers();

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
app.MapControllers();

app.Run();