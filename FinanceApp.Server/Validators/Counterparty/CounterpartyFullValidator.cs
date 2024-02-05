using FinanceApp.Data.RequestPatterns.Counterparty;
using FinanceApp.Server.Interfaces;

namespace FinanceApp.Server.Validators.Counterparty;

public class CounterpartyFullValidator : IValidator<ICounterpartyFull>
{
	public CounterpartyFullValidator(IUnitOfWork unitOfWork)
	{
		UnitOfWork = unitOfWork;
	}

	private IUnitOfWork UnitOfWork { get; }

	public async Task<bool> ValidateAsync(ICounterpartyFull request)
	{
		bool success = true;

		CounterpartyFieldsValidator fieldsValidator = new();
		success &= await fieldsValidator.ValidateAsync(request);
		CounterpartyIdValidator idValidator = new(UnitOfWork);
		success &= await idValidator.ValidateAsync(request);

		return success;
	}
}