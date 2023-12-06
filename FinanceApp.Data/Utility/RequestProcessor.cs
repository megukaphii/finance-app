using FinanceApp.Data.Interfaces;

namespace FinanceApp.Data.Utility;

public class RequestProcessor : IRequestProcessor
{
	private readonly IServiceProvider _serviceProvider;

	public RequestProcessor(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

	public async Task ProcessAsync(IRequest request, Client client)
	{
		Type type = typeof(IRequestHandler<>).MakeGenericType(request.GetType());
		dynamic handler = _serviceProvider.GetService(type) ??
		                  throw new InvalidOperationException(
			                  $"Could not find appropriate request handler for {type.GenericTypeArguments.First().Name}");
		await handler.HandleAsync((dynamic)request, client);
	}
}