using FinanceApp.Data.RequestPatterns;
using FinanceApp.Data.Requests.Account;
using FinanceApp.Data.Requests.Counterparty;
using FinanceApp.Data.Requests.Transaction;
using FinanceApp.Server.Handlers.Account;
using FinanceApp.Server.Handlers.Counterparty;
using FinanceApp.Server.Handlers.Transaction;
using FinanceApp.Server.Interfaces;
using FinanceApp.Server.Utility;
using FinanceApp.Server.Validators;
using Microsoft.EntityFrameworkCore;
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

				services.AddSingleton<IValidatorResolver, ValidatorResolver>();
				services.AddSingleton<IRequestProcessor, RequestProcessor>();

				services.AddTransient<DbContext, FinanceAppContext>();
				services.AddTransient<IUnitOfWork, UnitOfWork>();

				services.AddTransient<IValidator<IAccountId>, AccountIdValidator>();
				services.AddTransient<IValidator<IPageNumber>, PageNumberValidator>();
				services.AddTransient<IValidator<ISingleAccount>, SingleAccountValidator>();
				services.AddTransient<IValidator<ISingleCounterparty>, SingleCounterpartyValidator>();
				services.AddTransient<IValidator<ISingleTransaction>, SingleTransactionValidator>();

				services.AddTransient<IRequestHandler<CreateAccount>, CreateAccountHandler>();
				services.AddTransient<IRequestHandler<GetAccounts>, GetAccountsHandler>();
				services.AddTransient<IRequestHandler<SelectAccount>, SelectAccountHandler>();
				services.AddTransient<IRequestHandler<GetCounterparties>, GetCounterpartiesHandler>();
				services.AddTransient<IRequestHandler<UpdateCounterparty>, UpdateCounterpartyHandler>();
				services.AddTransient<IRequestHandler<CreateTransaction>, CreateTransactionHandler>();
				services.AddTransient<IRequestHandler<GetTransactions>, GetTransactionsHandler>();
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