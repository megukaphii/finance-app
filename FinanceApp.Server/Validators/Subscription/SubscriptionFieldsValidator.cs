using FinanceApp.Data.RequestPatterns.Subscription;
using FinanceApp.Server.Interfaces;
using FinanceApp.Server.Utility;

namespace FinanceApp.Server.Validators.Subscription;

public class SubscriptionFieldsValidator : IValidator<ISubscriptionFields>
{
	private static readonly int MinNameLength = PropertyHelpers.GetMinLength((Data.Models.Subscription a) => a.Name);
	private static readonly int MaxNameLength = PropertyHelpers.GetMaxLength((Data.Models.Subscription a) => a.Name);
	private static readonly int MinFrequencyCounterValue =
		PropertyHelpers.GetMinValue((Data.Models.Subscription a) => a.FrequencyCounter);
	private static readonly int MaxFrequencyCounterValue =
		PropertyHelpers.GetMaxValue((Data.Models.Subscription a) => a.FrequencyCounter);
	public SubscriptionFieldsValidator(IUnitOfWork unitOfWork) => UnitOfWork = unitOfWork;

	private IUnitOfWork UnitOfWork { get; }

	public async Task<bool> ValidateAsync(ISubscriptionFields request)
	{
		bool success = true;

		if (!await UnitOfWork.Repository<Data.Models.Counterparty>()
			     .AnyAsync(counterparty => counterparty.Id == request.Counterparty.Value)) {
			request.Counterparty.Error =
				$"Counterparty with {nameof(Data.Models.Counterparty.Id)} of {request.Counterparty.Value} does not exist";
			success = false;
		}

		if (request.Name.Value.Length < MinNameLength) {
			request.Name.Error = $"{nameof(request.Name)} should be more than {MinNameLength} characters";
			success = false;
		} else if (request.Name.Value.Length > MaxNameLength) {
			request.Name.Error = $"{nameof(request.Name)} should be less than {MaxNameLength} characters";
			success = false;
		}

		if (request.FrequencyCounter.Value < MinFrequencyCounterValue) {
			request.FrequencyCounter.Error =
				$"{nameof(request.FrequencyCounter)} should be greater than {MinFrequencyCounterValue}";
			success = false;
		} else if (request.FrequencyCounter.Value > MaxFrequencyCounterValue) {
			request.FrequencyCounter.Error =
				$"{nameof(request.FrequencyCounter)} should be less than {MaxFrequencyCounterValue}";
			success = false;
		}

		if (request.StartDate.Value >= request.EndDate.Value && request.EndDate.Value != DateTime.UnixEpoch) {
			request.StartDate.Error = $"{nameof(request.StartDate)} should be before {nameof(request.EndDate)}";
			success = false;
		}

		return success;
	}
}