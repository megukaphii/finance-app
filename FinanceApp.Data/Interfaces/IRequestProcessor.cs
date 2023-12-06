using FinanceApp.Data.Utility;

namespace FinanceApp.Data.Interfaces;

public interface IRequestProcessor
{
	Task ProcessAsync(IRequest request, Client client);
}