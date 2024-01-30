using FinanceApp.Data;
using FinanceApp.Data.Requests.Counterparty;
using FinanceApp.Server;
using FinanceApp.Server.Handlers.Counterparty;
using FinanceApp.Server.Interfaces;
using FinanceApp.Server.Utility;
using FinanceApp.ServerTests.Extensions;
using FinanceApp.ServerTests.Helpers;
using NSubstitute;

namespace FinanceApp.ServerTests.Handlers.Counterparty;

[TestFixture]
[TestOf(typeof(GetCounterpartiesHandler))]
public class GetCounterpartiesHandlerTest
{
	[SetUp]
	public void SetUp()
	{
		_client = Substitute.For<IClient>();
	}

	private IClient _client = null!;

	[Test]
	public async Task HandleAsync_ReturnsCounterparties()
	{
		FinanceAppContext context = InMemoryDatabaseFactory.CreateNewDatabase();
		context.LoadCounterparties();
		UnitOfWork unitOfWork = new(context);
		GetCounterpartiesHandler handler = new(unitOfWork);
		List<Data.Models.Counterparty> expectedCounterparties = DatabaseSeeder.Counterparties.ToList();
		GetCounterparties request = new()
		{
			Page = new() { Value = 1 }
		};

		await handler.HandleAsync(request, _client);

		await _client.Received().Send(Arg.Is<GetCounterpartiesResponse>(r =>
			r.Success && r.Counterparties.SequenceEqual(expectedCounterparties)));
	}

	[Test]
	public async Task HandleAsync_ReturnsEmptyList()
	{
		FinanceAppContext context = InMemoryDatabaseFactory.CreateNewDatabase();
		UnitOfWork unitOfWork = new(context);
		GetCounterpartiesHandler handler = new(unitOfWork);
		GetCounterparties request = new()
		{
			Page = new() { Value = 1 }
		};

		await handler.HandleAsync(request, _client);

		await _client.Received().Send(Arg.Is<GetCounterpartiesResponse>(r =>
			r.Success && r.Counterparties.SequenceEqual(new List<Data.Models.Counterparty>())));
	}
}