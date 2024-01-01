using FinanceApp.Data.Extensions;
using FinanceApp.Data.Interfaces;

namespace FinanceApp.Data.Utility;

public class RequestProcessor : IRequestProcessor
{
	private readonly IServiceProvider _serviceProvider;
	private readonly IValidatorResolver _validatorResolver;

	public RequestProcessor(IServiceProvider serviceProvider, IValidatorResolver validatorResolver)
	{
		_serviceProvider = serviceProvider;
		_validatorResolver = validatorResolver;
	}

	public async Task ProcessAsync<T>(T request, IClient client) where T : IRequest
	{
		if (await IsValidAsync(request)) {
			Type type = typeof(IRequestHandler<>).MakeGenericType(request.GetType());
			dynamic handler = _serviceProvider.GetService(type) ??
			                  throw new InvalidOperationException(
				                  $"Could not find appropriate request handler for {type.GenericTypeArguments.First().Name}");

			await handler.HandleAsync((dynamic)request, client);
		} else {
			await SendErrorResponseAsync(client.Stream, request);
		}
	}

	private Task<bool> IsValidAsync<T>(T request) where T : IRequest
	{
		IValidator<T> validator = _validatorResolver.GetValidator<T>();
		return validator.ValidateAsync(request);
	}

	private static Task SendErrorResponseAsync(Stream stream, IRequest failedRequest)
	{
		// TODO - I don't think this works??? The <Error> flag is never added?
		return stream.SendRequestAsync(failedRequest);
	}
}