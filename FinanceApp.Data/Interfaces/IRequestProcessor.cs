namespace FinanceApp.Data.Interfaces;

public interface IRequestProcessor
{
	Task ProcessAsync<T>(T request, IClient client) where T : IRequest;
}