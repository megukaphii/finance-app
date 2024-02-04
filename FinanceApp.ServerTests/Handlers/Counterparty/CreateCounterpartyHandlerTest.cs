using FinanceApp.Data.Requests.Counterparty;
using FinanceApp.Server;
using FinanceApp.Server.Handlers.Counterparty;
using FinanceApp.Server.Interfaces;
using FinanceApp.Server.Utility;
using FinanceApp.ServerTests.Helpers;
using NSubstitute;

namespace FinanceApp.ServerTests.Handlers.Counterparty;

[TestFixture]
[TestOf(typeof(CreateCounterpartyHandler))]
public class CreateCounterpartyHandlerTest
{
	[SetUp]
	public void SetUp()
	{
		FinanceAppContext context = _databaseFactory.CreateNewDatabase();

		_client = Substitute.For<IClient>();

		UnitOfWork unitOfWork = new(context);
		_handler = new(unitOfWork);
	}

	private readonly InMemoryDatabaseFactory _databaseFactory = new();
	private CreateCounterpartyHandler _handler = null!;
	private IClient _client = null!;

	[Test]
	public async Task CreateCounterpartyHandler_HandleAsync_CreatesCounterpartySuccessfully()
	{
		Data.Models.Counterparty expected = new() { Id = 1, Name = "NewName" };
		CreateCounterparty request = new() { Name = new() { Value = "NewName" } };

		await _handler.HandleAsync(request, _client);

		FinanceAppContext context = _databaseFactory.GetExistingDatabase();
		UnitOfWork unitOfWork = new(context);
		Data.Models.Counterparty? result =
			await unitOfWork.Repository<Data.Models.Counterparty>().FindAsync(1L);

		Assert.That(expected, Is.EqualTo(result));
		await _client.Received()
			.Send(Arg.Is<CreateCounterpartyResponse>(response => response.Success && expected.Id == response.Id));
	}
}