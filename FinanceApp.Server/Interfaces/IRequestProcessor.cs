using FinanceApp.Data.Interfaces;

namespace FinanceApp.Server.Interfaces;

public interface IRequestProcessor
{
	Task ProcessAsync<T>(T request, IClient client) where T : IRequest;
}