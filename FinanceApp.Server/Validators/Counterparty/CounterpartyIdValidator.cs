using FinanceApp.Data.RequestPatterns.Counterparty;
using FinanceApp.Server.Interfaces;

namespace FinanceApp.Server.Validators.Counterparty;

public class CounterpartyIdValidator
{
	public CounterpartyIdValidator(IUnitOfWork unitOfWork)
	{
		UnitOfWork = unitOfWork;
	}

	private IUnitOfWork UnitOfWork { get; }

	public async Task<bool> ValidateAsync(ICounterpartyFull request)
	{
		bool success = true;

		if (!await UnitOfWork.Repository<Data.Models.Counterparty>().AnyAsync(counterparty => counterparty.Id == request.Id.Value)) {
			request.Id.Error = $"Counterparty with {nameof(request.Id)} of {request.Id} does not exist";
			success = false;
		}

		return success;
	}
}