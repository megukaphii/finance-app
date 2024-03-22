using FinanceApp.Data.Requests.Account;
using FinanceApp.Server.Interfaces;

namespace FinanceApp.Server.Handlers.Account;

public class SelectAccountHandler : IRequestHandler<SelectAccount>
{
	public SelectAccountHandler(IUnitOfWork unitOfWork) => UnitOfWork = unitOfWork;

	public IUnitOfWork UnitOfWork { get; }

	public async Task HandleAsync(SelectAccount request, IClient client)
	{
		using (UnitOfWork) {
			SelectAccountResponse response;
			if (await UnitOfWork.Repository<Data.Models.Account>().AnyAsync(account => account.Id == request.Id.Value)) {
				client.Session.AccountId = request.Id.Value;
				response = new()
				{
					Success = true
				};
			} else {
				response = new()
				{
					Success = false
				};
			}

			await client.Send(response);
		}
	}
}