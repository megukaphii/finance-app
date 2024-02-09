using FinanceApp.Data.RequestPatterns.Transaction;
using FinanceApp.Server.Interfaces;

namespace FinanceApp.Server.Validators.Transaction;

public class TransactionFieldsValidator : IValidator<ITransactionFields>
{
	public TransactionFieldsValidator(IUnitOfWork unitOfWork)
	{
		UnitOfWork = unitOfWork;
	}

	private IUnitOfWork UnitOfWork { get; }

	private const decimal MinValue = decimal.MinValue + 1;
	private const decimal MaxValue = decimal.MaxValue - 1;

	public async Task<bool> ValidateAsync(ITransactionFields request)
	{
		bool success = true;

		switch (request.Value.Value) {
			case < MinValue:
				request.Value.Error = $"{nameof(request.Value)} should be greater than {MinValue}";
				success = false;
				break;
			case > MaxValue:
				request.Value.Error = $"{nameof(request.Value)} should be less than {MaxValue}";
				success = false;
				break;
		}

		if (!await UnitOfWork.Repository<Data.Models.Counterparty>()
			     .AnyAsync(counterparty => counterparty.Id == request.Counterparty.Value)) {
			request.Counterparty.Error =
				$"Counterparty with {nameof(request.Counterparty.Value)} of {request.Counterparty.Value} does not exist";
			success = false;
		}

		return success;
	}
}