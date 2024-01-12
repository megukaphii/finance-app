using FinanceApp.Data.Requests.Account;
using FinanceApp.Server.Handlers.Account;
using FinanceApp.Server.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace FinanceApp.ServerTests.Handlers.Account;

[TestFixture]
[TestOf(typeof(CreateAccountHandler))]
public class CreateAccountHandlerTest
{
	[SetUp]
	public void SetUp()
	{
		_client = Substitute.For<IClient>();
		_unitOfWork = Substitute.For<IUnitOfWork>();
		_handler = new(_unitOfWork);
	}

	private IClient _client = null!;
	private IUnitOfWork _unitOfWork = null!;
	private CreateAccountHandler _handler = null!;

	[Test]
	public async Task HandleAsync_ShouldInteractWithCorrectMethods()
	{
		CreateAccount request = new()
		{
			Name = new() { Value = "Making Bank" },
			Description = new() { Value = "A Test Bank Account" }
		};

		await _handler.HandleAsync(request, _client);

		await _unitOfWork.Repository<Data.Models.Account>().Received().AddAsync(Arg.Any<Data.Models.Account>());
		_unitOfWork.Received().SaveChanges();
		await _client.Received().Send(Arg.Is<CreateAccountResponse>(r => r.Success));
	}
}