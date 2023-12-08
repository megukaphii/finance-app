using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Models;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.Data.Utility;

namespace FinanceApp.Data.Validators;

public class SingleAccountValidator : IValidator<ISingleAccount>
{
	// TODO - Are these cached (I don't think so)? Should they be cached (wouldn't hurt)?
	private static readonly int MinNameLength = Helpers.GetPropertyMinLength((Account a) => a.Name);
	private static readonly int MaxNameLength = Helpers.GetPropertyMaxLength((Account a) => a.Name);
	private static readonly int MinDescriptionLength = Helpers.GetPropertyMinLength((Account a) => a.Description);
	private static readonly int MaxDescriptionLength = Helpers.GetPropertyMaxLength((Account a) => a.Description);

	public Task<bool> ValidateAsync(ISingleAccount request)
	{
		bool failure = false;
		if (request.Name.Value.Length < MinNameLength) {
			request.Name.Error =
				$"{nameof(request.Name)} should be more than {MinNameLength} characters";
			failure = true;
		} else if (request.Name.Value.Length > MaxNameLength) {
			request.Name.Error =
				$"{nameof(request.Name)} should be less than {MaxNameLength} characters";
			failure = true;
		}

		if (request.Description.Value.Length < MinDescriptionLength) {
			request.Description.Error =
				$"{nameof(request.Description)} should be more than {MinDescriptionLength} characters";
			failure = true;
		} else if (request.Description.Value.Length > MaxDescriptionLength) {
			request.Description.Error =
				$"{nameof(request.Description)} should be less than {MaxDescriptionLength} characters";
			failure = true;
		}

		return Task.FromResult(!failure);
	}
}