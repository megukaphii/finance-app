using FinanceApp.Server.Classes;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

IHost host = Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration((_, configBuilder) =>
    {

    })
    .ConfigureServices((_, services) =>
    {
        services.AddSingleton<IServer, FinanceServer>();
    })
.Build();

IServer server = host.Services.GetRequiredService<IServer>();

await server.Start();