using FinanceApp.Data.RequestPatterns.Counterparty;
using FinanceApp.Server.Interfaces;
using FinanceApp.Server.Utility;

namespace FinanceApp.Server.Validators.Counterparty;

public class CounterpartyFullValidator : IValidator<ICounterpartyFull>
{
	public CounterpartyFullValidator(IUnitOfWork unitOfWork)
	{
		UnitOfWork = unitOfWork;
	}

	private static readonly int MinNameLength = PropertyHelpers.GetMinLength((Data.Models.Counterparty a) => a.Name);
	private static readonly int MaxNameLength = PropertyHelpers.GetMaxLength((Data.Models.Counterparty a) => a.Name);

	private IUnitOfWork UnitOfWork { get; }

	public async Task<bool> ValidateAsync(ICounterpartyFull request)
	{
		bool failure = false;
		if (request.Name.Value.Length < MinNameLength) {
			request.Name.Error = $"{nameof(request.Name)} should be more than {MinNameLength} characters";
			failure = true;
		} else if (request.Name.Value.Length > MaxNameLength) {
			request.Name.Error = $"{nameof(request.Name)} should be less than {MaxNameLength} characters";
			failure = true;
		}

		if (!await UnitOfWork.Repository<Data.Models.Counterparty>().AnyAsync(counterparty => counterparty.Id == request.Id.Value)) {
			request.Id.Error = $"Counterparty with {nameof(request.Id)} of {request.Id} does not exist";
			failure = true;
		}

		return !failure;
	}
}