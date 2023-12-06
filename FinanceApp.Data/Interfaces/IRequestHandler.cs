using FinanceApp.Data.Utility;

namespace FinanceApp.Data.Interfaces;

public interface IRequestHandler<in TRequest> where TRequest : IRequest
{
	protected IUnitOfWork UnitOfWork { get; }

	Task HandleAsync(TRequest request, Client client);
}