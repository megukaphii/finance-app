using FinanceApp.Server.Classes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FinanceApp.Server;

public static class Program
{
    private static IHost _host = null!;

    private static async Task Main()
    {
        AppDomain.CurrentDomain.ProcessExit += Exit;

        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((_, _) => { })
            .ConfigureServices((_, services) =>
            {
                services.AddMemoryCache();
                services.AddHostedService<FinanceServer>();
            })
            .Build();

        CancellationTokenSource source = new();
        CancellationToken token = source.Token;

        await _host.StartAsync(token);
        await _host.WaitForShutdownAsync(token);
    }

    private static async void Exit(object? eventArgs, EventArgs args)
    {
        await _host.StopAsync();
    }
}