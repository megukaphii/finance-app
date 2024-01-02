using FinanceApp.Data.Models;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.Server.Interfaces;
using FinanceApp.Server.Utility;

namespace FinanceApp.Server.Validators;

public class SingleTransactionValidator : IValidator<ISingleTransaction>
{
	private const decimal MinValue = decimal.MinValue + 1;
	private const decimal MaxValue = decimal.MaxValue - 1;
	private static readonly int MinCounterpartyNameLength = PropertyHelpers.GetMinLength((Counterparty c) => c.Name);
	private static readonly int MaxCounterpartyNameLength = PropertyHelpers.GetMaxLength((Counterparty c) => c.Name);

	public Task<bool> ValidateAsync(ISingleTransaction request)
	{
		bool failure = false;
		switch (request.Value.Value) {
			case < MinValue:
				request.Value.Error = $"{nameof(request.Value)} should be greater than {MinValue}";
				failure = true;
				break;
			case > MaxValue:
				request.Value.Error = $"{nameof(request.Value)} should be less than {MaxValue}";
				failure = true;
				break;
		}

		if (request.Counterparty.Value.Name.Length < MinCounterpartyNameLength) {
			request.Counterparty.Error = $"{nameof(request.Counterparty)} name length should be" +
			                             $" more than {MinCounterpartyNameLength} characters";
			failure = true;
		} else if (request.Counterparty.Value.Name.Length > MaxCounterpartyNameLength) {
			request.Counterparty.Error = $"{nameof(request.Counterparty)} name length should be" +
			                             $" less than {MaxCounterpartyNameLength} characters";
			failure = true;
		}

		return Task.FromResult(!failure);
	}
}