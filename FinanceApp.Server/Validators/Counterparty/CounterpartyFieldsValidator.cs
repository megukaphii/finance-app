﻿using FinanceApp.Data.RequestPatterns.Counterparty;
using FinanceApp.Server.Interfaces;
using FinanceApp.Server.Utility;

namespace FinanceApp.Server.Validators.Counterparty;

public class CounterpartyFieldsValidator : IValidator<ICounterpartyFields>
{
	private static readonly int MinNameLength = PropertyHelpers.GetMinLength((Data.Models.Counterparty a) => a.Name);
	private static readonly int MaxNameLength = PropertyHelpers.GetMaxLength((Data.Models.Counterparty a) => a.Name);

	public Task<bool> ValidateAsync(ICounterpartyFields request)
	{
		bool failure = false;
		if (request.Name.Value.Length < MinNameLength) {
			request.Name.Error = $"{nameof(request.Name)} should be more than {MinNameLength} characters";
			failure = true;
		} else if (request.Name.Value.Length > MaxNameLength) {
			request.Name.Error = $"{nameof(request.Name)} should be less than {MaxNameLength} characters";
			failure = true;
		}

		return Task.FromResult(!failure);
	}
}