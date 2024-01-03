using FinanceApp.Data.Requests.Account;
using FinanceApp.Server.Handlers.Account;
using FinanceApp.Server.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace FinanceApp.ServerTests.Handlers.Account;

[TestFixture]
[TestOf(typeof(GetAccountsHandler))]
public class GetAccountsHandlerTest
{
	[SetUp]
	public void SetUp()
	{
		_unitOfWork = Substitute.For<IUnitOfWork>();
		_client = Substitute.For<IClient>();
		_handler = new(_unitOfWork);
	}

	private GetAccountsHandler _handler = null!;
	private IUnitOfWork _unitOfWork = null!;
	private IClient _client = null!;

	[TestCase(1)]
	[TestCase(10)]
	[TestCase(long.MaxValue)]
	public async Task Test_HandleAsync(long page)
	{
		GetAccounts request = new()
		{
			Page = new() { Value = page }
		};

		await _handler.HandleAsync(request, _client);

		// TODO - Update when we add actual pagination
		await _unitOfWork.Received(1).Repository<FinanceApp.Data.Models.Account>().AllAsync();
		await _client.Received(1).Send(Arg.Any<GetAccountsResponse>());
	}
}