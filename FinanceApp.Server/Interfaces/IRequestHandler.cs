﻿using FinanceApp.Data.Interfaces;

namespace FinanceApp.Server.Interfaces;

public interface IRequestHandler<in TRequest> where TRequest : IRequest
{
	protected IUnitOfWork UnitOfWork { get; }

	Task HandleAsync(TRequest request, IClient client);
}