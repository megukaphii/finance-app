using FinanceApp.Data.Requests.Account;
using FinanceApp.Server.Handlers.Account;
using FinanceApp.Server.Interfaces;
using FinanceApp.Server.Utility;
using NSubstitute;
using NUnit.Framework;

namespace FinanceApp.ServerTests.Handlers.Account;

public class SelectAccountHandlerTests
{
	private SelectAccountHandler _handler = null!;
	private IUnitOfWork _unitOfWork = null!;
	private IClient _client = null!;
	private Session _session = null!;
	private Data.Models.Account _account = null!;

	[SetUp]
	public void Setup()
	{
		_unitOfWork = Substitute.For<IUnitOfWork>();
		_client = Substitute.For<IClient>();
		_session = Substitute.For<Session>();
		_client.Session.Returns(_session);
		_handler = new(_unitOfWork);

		_account = new()
		{
			Name = "Test",
			Description = "Test Description",
			Value = 100
		};
		_unitOfWork.Repository<Data.Models.Account>()
			.FindAsync(Arg.Any<long>())!
			.Returns(_account);
	}

	[Test]
	public async Task HandleAsync_ShouldInteractWithCorrectMethods()
	{
		SelectAccount request = new()
		{
			Id = new() { Value = 1 }
		};

		await _handler.HandleAsync(request, _client);

		await _unitOfWork.Repository<Data.Models.Account>().Received(1)
			.FindAsync(request.Id.Value);
		await _client.Received(1).Send(Arg.Any<SelectAccountResponse>());
		Assert.That(_client.Session.Account, Is.EqualTo(_account));
	}
}