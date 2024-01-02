using FinanceApp.Data.Requests.Account;
using FinanceApp.Server.Handlers.Account;
using FinanceApp.Server.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace FinanceApp.ServerTests.Requests.Account;

[TestFixture]
[TestOf(typeof(CreateAccountHandler))]
public class CreateAccountHandlerTest
{
	[SetUp]
	public void Setup()
	{
		_unitOfWork = Substitute.For<IUnitOfWork>();
		_client = Substitute.For<IClient>();
		_handler = new(_unitOfWork);
	}

	private CreateAccountHandler _handler = null!;
	private IUnitOfWork _unitOfWork = null!;
	private IClient _client = null!;

	[Test]
	public async Task HandleAsync_ShouldInteractWithCorrectMethods()
	{
		// Arrange
		CreateAccount request = new()
		{
			Name = new() { Value = "Making Bank" },
			Description = new() { Value = "A Test Bank Account" }
		};

		// Act
		await _handler.HandleAsync(request, _client);

		// Assert
		await _unitOfWork.Received(1).Repository<Data.Models.Account>().AddAsync(Arg.Any<Data.Models.Account>());
		_unitOfWork.Received(1).SaveChanges();
		await _client.Received(1).Send(Arg.Any<CreateAccountResponse>());
	}
}