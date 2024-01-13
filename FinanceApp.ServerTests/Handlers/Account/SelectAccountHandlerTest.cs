using FinanceApp.Data.Requests.Account;
using FinanceApp.Server.Handlers.Account;
using FinanceApp.Server.Interfaces;
using FinanceApp.Server.Utility;
using NSubstitute;
using NUnit.Framework;

namespace FinanceApp.ServerTests.Handlers.Account;

[TestFixture]
[TestOf(typeof(SelectAccountHandler))]
public class SelectAccountHandlerTests
{
	[SetUp]
	public void SetUp()
	{
		_unitOfWork = Substitute.For<IUnitOfWork>();
		_client = Substitute.For<IClient>();
		Session session = Substitute.For<Session>();
		_client.Session.Returns(session);

		_account = new()
		{
			Name = "Test",
			Description = "Test Description",
			Value = 100
		};
		_unitOfWork.Repository<Data.Models.Account>()
			.FindAsync(Arg.Any<long>())!
			.Returns(_account);

		_handler = new(_unitOfWork);
	}

	private IClient _client = null!;
	private IUnitOfWork _unitOfWork = null!;
	private SelectAccountHandler _handler = null!;
	private Data.Models.Account _account = null!;

	[Test]
	public async Task HandleAsync_ShouldInteractWithCorrectMethods()
	{
		SelectAccount request = new()
		{
			Id = new() { Value = 1 }
		};

		await _handler.HandleAsync(request, _client);

		await _unitOfWork.Repository<Data.Models.Account>().Received().FindAsync(request.Id.Value);
		await _client.Received().Send(Arg.Is<SelectAccountResponse>(r => r.Success));
		Assert.That(_client.Session.Account, Is.EqualTo(_account));
	}
}