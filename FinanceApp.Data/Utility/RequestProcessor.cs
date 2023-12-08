using System.Text;
using System.Text.Json;
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

	public async Task ProcessAsync(IRequest request, Client client)
	{
		if (await IsValidAsync((dynamic)request)) {
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

	private static async Task SendErrorResponseAsync<TRequest>(Stream stream, TRequest validatedRequest)
		where TRequest : IRequest
	{
		string strResponse = JsonSerializer.Serialize(validatedRequest);
		byte[] message = Encoding.UTF8.GetBytes(Serialization.Error + strResponse + Serialization.Eof);
		await stream.WriteAsync(message);
		await stream.FlushAsync();
	}
}