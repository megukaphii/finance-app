using FinanceApp.Data.Requests.Counterparty;
using FinanceApp.Server;
using FinanceApp.Server.Handlers.Counterparty;
using FinanceApp.Server.Interfaces;
using FinanceApp.Server.Utility;
using FinanceApp.ServerTests.Helpers;
using NSubstitute;

namespace FinanceApp.ServerTests.Handlers.Counterparty;

[TestFixture]
[TestOf(typeof(UpdateCounterpartyHandler))]
public class UpdateCounterpartyHandlerTest
{
	private IClient _mockClient = null!;

	[SetUp]
	public void SetUp()
	{
		_mockClient = Substitute.For<IClient>();
	}

	[Test]
	public async Task HandleAsync_SuccessfulUpdate_ReturnsSuccessResponse()
	{
		InMemoryDatabaseFactory databaseFactory = new();
		FinanceAppContext context = databaseFactory.CreateNewDatabase();
		UnitOfWork unitOfWork = new(context);
		UpdateCounterpartyHandler handler = new(unitOfWork);
		Data.Models.Counterparty mockCounterparty = new() { Id = 1, Name = "OldName" };
		await unitOfWork.Repository<Data.Models.Counterparty>().AddAsync(mockCounterparty);
		unitOfWork.SaveChanges();
		UpdateCounterparty request = new()
		{
			Id = new() { Value = 1 },
			Name = new() { Value = "NewName" }
		};

		await handler.HandleAsync(request, _mockClient);
		context = databaseFactory.GetExistingDatabase();
		unitOfWork = new(context);
		Data.Models.Counterparty? updatedCounterparty =
			await unitOfWork.Repository<Data.Models.Counterparty>().FindAsync(mockCounterparty.Id);

		Assert.That(request.Name.Value, Is.EqualTo(updatedCounterparty?.Name));
		await _mockClient.Received().Send(Arg.Is<UpdateCounterpartyResponse>(response => response.Success));
	}
}