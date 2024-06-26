﻿using FinanceApp.Data.RequestPatterns.Account;
using FinanceApp.Server.Interfaces;
using FinanceApp.Server.Utility;

namespace FinanceApp.Server.Validators.Account;

public class AccountFieldsValidator : IValidator<IAccountFields>
{
	// TODO - Are these cached (I don't think so)? Should they be cached (wouldn't hurt)?
	private static readonly int MinNameLength = PropertyHelpers.GetMinLength((Data.Models.Account a) => a.Name);
	private static readonly int MaxNameLength = PropertyHelpers.GetMaxLength((Data.Models.Account a) => a.Name);
	private static readonly int MinDescriptionLength = PropertyHelpers.GetMinLength((Data.Models.Account a) => a.Description);
	private static readonly int MaxDescriptionLength = PropertyHelpers.GetMaxLength((Data.Models.Account a) => a.Description);

	public Task<bool> ValidateAsync(IAccountFields request)
	{
		bool success = true;

		if (request.Name.Value.Length < MinNameLength) {
			request.Name.Error =
				$"{nameof(request.Name)} should be more than {MinNameLength} characters";
			success = false;
		} else if (request.Name.Value.Length > MaxNameLength) {
			request.Name.Error =
				$"{nameof(request.Name)} should be less than {MaxNameLength} characters";
			success = false;
		}

		if (request.Description.Value.Length < MinDescriptionLength) {
			request.Description.Error =
				$"{nameof(request.Description)} should be more than {MinDescriptionLength} characters";
			success = false;
		} else if (request.Description.Value.Length > MaxDescriptionLength) {
			request.Description.Error =
				$"{nameof(request.Description)} should be less than {MaxDescriptionLength} characters";
			success = false;
		}

		return Task.FromResult(success);
	}
}