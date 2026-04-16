using ClearSettle.Application.UseCases;
using ClearSettle.Domain.Interfaces;
using ClearSettle.Infrastructure.Data;
using ClearSettle.Infrastructure.Data.Repositories;
using ClearSettle.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var connectionString = "Server=localhost,1434;Database=ClearSettleDb;User Id=sa;Password=ClearSettle@2026!;TrustServerCertificate=True;";
        services.AddDbContext<SettlementDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped<ITradeRepository, TradeRepository>();
        services.AddScoped<RegisterPendingTradeUseCase>();

        services.AddHostedService<TradeWorker>();
    })
    .Build();

await host.RunAsync();