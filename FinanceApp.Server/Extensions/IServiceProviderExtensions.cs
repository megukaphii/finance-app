using FinanceApp.Data.Interfaces;
using FinanceApp.Server.Exceptions;
using FinanceApp.Server.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceApp.Server.Extensions;

// ReSharper disable once InconsistentNaming
public static class IServiceProviderExtensions
{
	public static object GetValidator<T>(this IServiceProvider serviceProvider) where T : IRequest
	{
		return serviceProvider.GetService<IValidator<T>>() ??
		       throw new ValidatorMissingException(
			       $"{nameof(IValidator<T>)}<{typeof(T).Name}> does not have an implementation");
	}
}