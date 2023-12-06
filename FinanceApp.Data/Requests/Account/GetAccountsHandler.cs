using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Utility;

namespace FinanceApp.Data.Requests.Account;

public class GetAccountsHandler : IRequestHandler<GetAccounts>
{
	public GetAccountsHandler(IUnitOfWork unitOfWork) => UnitOfWork = unitOfWork;

	public IUnitOfWork UnitOfWork { get; }

	public async Task HandleAsync(GetAccounts request, Client client)
	{
		using (UnitOfWork) {
			List<Models.Account> accounts = await UnitOfWork.Repository<Models.Account>().AllAsync();

			GetAccountsResponse response = new()
			{
				Accounts = accounts,
				Success = true
			};

			await client.Send(response);
		}
	}
}