﻿using System.Linq.Expressions;
using FinanceApp.Data.Requests.Account;
using FinanceApp.Data.Utility;
using FinanceApp.Server.Handlers.Account;
using FinanceApp.Server.Interfaces;
using NSubstitute;

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

		_unitOfWork.Repository<Data.Models.Account>().AnyAsync(Arg.Any<Expression<Func<Data.Models.Account, bool>>>()).Returns(true);

		_handler = new(_unitOfWork);
	}

	private IClient _client = null!;
	private IUnitOfWork _unitOfWork = null!;
	private SelectAccountHandler _handler = null!;

	[Test]
	public async Task HandleAsync_ShouldInteractWithCorrectMethods()
	{
		SelectAccount request = new()
		{
			Id = new() { Value = 1 }
		};

		await _handler.HandleAsync(request, _client);

		await _unitOfWork.Repository<Data.Models.Account>().Received().AnyAsync(Arg.Any<Expression<Func<Data.Models.Account, bool>>>());
		await _client.Received().Send(Arg.Is<SelectAccountResponse>(r => r.Success));
		Assert.That(_client.Session.AccountId, Is.EqualTo(1L));
	}
}