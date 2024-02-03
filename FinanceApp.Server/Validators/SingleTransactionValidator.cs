using FinanceApp.Data.Models;
using FinanceApp.Data.RequestPatterns;
using FinanceApp.Server.Interfaces;

namespace FinanceApp.Server.Validators;

public class SingleTransactionValidator : IValidator<ISingleTransaction>
{
	public SingleTransactionValidator(IUnitOfWork unitOfWork)
	{
		UnitOfWork = unitOfWork;
	}

	private IUnitOfWork UnitOfWork { get; }

	private const decimal MinValue = decimal.MinValue + 1;
	private const decimal MaxValue = decimal.MaxValue - 1;
	/*private static readonly int MinCounterpartyNameLength = PropertyHelpers.GetMinLength((Counterparty c) => c.Name);
	private static readonly int MaxCounterpartyNameLength = PropertyHelpers.GetMaxLength((Counterparty c) => c.Name);*/

	public async Task<bool> ValidateAsync(ISingleTransaction request)
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

		if (!await UnitOfWork.Repository<Counterparty>()
			     .AnyAsync(counterparty => counterparty.Id == request.Counterparty.Value)) {
			request.Counterparty.Error =
				$"Counterparty with {nameof(request.Counterparty.Value)} of {request.Counterparty.Value} does not exist";
			failure = true;
		}

		/*if (request.Counterparty.Value.Name.Length < MinCounterpartyNameLength) {
			request.Counterparty.Error = $"{nameof(request.Counterparty)} name length should be" +
			                             $" more than {MinCounterpartyNameLength} characters";
			failure = true;
		} else if (request.Counterparty.Value.Name.Length > MaxCounterpartyNameLength) {
			request.Counterparty.Error = $"{nameof(request.Counterparty)} name length should be" +
			                             $" less than {MaxCounterpartyNameLength} characters";
			failure = true;
		}*/

		return !failure;
	}
}