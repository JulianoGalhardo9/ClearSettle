using ClearSettle.Application.UseCases;
using ClearSettle.Domain.Interfaces;
using ClearSettle.Infrastructure.Data;
using ClearSettle.Infrastructure.Data.Repositories;
using ClearSettle.Worker;
using Microsoft.EntityFrameworkCore;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var connectionString = "Server=localhost,1434;Database=ClearSettleDb;User Id=sa;Password=ClearSettle@2026!;TrustServerCertificate=True;";
        services.AddDbContext<SettlementDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped<ITradeRepository, TradeRepository>();
        services.AddScoped<RegisterPendingTradeUseCase>();

        services.AddHostedService<TradeWorker>();

        services.AddScoped<ProcessSettlementUseCase>();

        services.AddHostedService<SettlementJobWorker>();
    })
    .Build();

await host.RunAsync();