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

		if (!await UnitOfWork.Repository<Data.Models.Counterparty>()
			     .AnyAsync(counterparty => counterparty.Id == request.Counterparty.Value)) {
			request.Counterparty.Error =
				$"Counterparty with {nameof(request.Counterparty.Value)} of {request.Counterparty.Value} does not exist";
			failure = true;
		}

		return !failure;
	}
}