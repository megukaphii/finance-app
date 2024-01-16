﻿using FinanceApp.Data.Interfaces;
using FinanceApp.Server.Interfaces;

namespace FinanceApp.Server.Utility;

public class ValidatorResolver : IValidatorResolver
{
	private readonly IServiceProvider _serviceProvider;

	public ValidatorResolver(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	public IValidator<T> GetValidator<T>() where T : IRequest
	{
		Type type = typeof(IValidator<>).MakeGenericType(typeof(T));
		IValidator<T> validator = (IValidator<T>?)_serviceProvider.GetService(type) ??
		                          throw new InvalidOperationException(
			                          $"Could not find appropriate validator for {typeof(T).Name}");
		return validator;
	}
}