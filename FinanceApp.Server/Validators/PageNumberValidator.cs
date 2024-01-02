using FinanceApp.Data.RequestPatterns;
using FinanceApp.Server.Interfaces;

namespace FinanceApp.Server.Validators;

public class PageNumberValidator : IValidator<IPageNumber>
{
	private const long MinPage = 0;
	// This is just to stop it complaining about an unreachable switch case. Probably change it later.
	private const long MaxPage = long.MaxValue - 1;

	public Task<bool> ValidateAsync(IPageNumber request)
	{
		bool failure = false;
		switch (request.Page.Value) {
			case < MinPage:
				request.Page.Error = $"{nameof(request.Page)} should be greater than {MinPage}";
				failure = true;
				break;
			case > MaxPage:
				request.Page.Error = $"{nameof(request.Page)} should be less than {MaxPage}";
				failure = true;
				break;
		}

		return Task.FromResult(!failure);
	}
}