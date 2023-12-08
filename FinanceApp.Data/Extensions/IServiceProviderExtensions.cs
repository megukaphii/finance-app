using FinanceApp.Data.Exceptions;
using FinanceApp.Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceApp.Data.Extensions;

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