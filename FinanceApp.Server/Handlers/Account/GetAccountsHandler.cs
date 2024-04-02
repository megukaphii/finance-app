using FinanceApp.Data.Requests.Account;
using FinanceApp.Server.Interfaces;

namespace FinanceApp.Server.Handlers.Account;

public class GetAccountsHandler : IRequestHandler<GetAccounts>
{
	public GetAccountsHandler(IUnitOfWork unitOfWork) => UnitOfWork = unitOfWork;

	public IUnitOfWork UnitOfWork { get; }

	public async Task HandleAsync(GetAccounts request, IClient client)
	{
		using (UnitOfWork) {
			List<Data.Models.Account> accounts = await UnitOfWork.Repository<Data.Models.Account>().AllAsync();

			GetAccountsResponse response = new()
			{
				Success = true,
				Accounts = accounts
			};

			await client.Send(response);
		}
	}
}