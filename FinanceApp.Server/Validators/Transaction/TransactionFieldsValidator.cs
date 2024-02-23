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

	public async Task<bool> ValidateAsync(ITransactionFields request)
	{
		bool success = true;

		if (!await UnitOfWork.Repository<Data.Models.Counterparty>()
			     .AnyAsync(counterparty => counterparty.Id == request.Counterparty.Value)) {
			request.Counterparty.Error =
				$"Counterparty with {nameof(Data.Models.Counterparty.Id)} of {request.Counterparty.Value} does not exist";
			success = false;
		}

		return success;
	}
}