using FinanceApp.Data.Interfaces;

namespace FinanceApp.Data.Requests.Account;

public class SelectAccountHandler : IRequestHandler<SelectAccount>
{
	public SelectAccountHandler(IUnitOfWork unitOfWork) => UnitOfWork = unitOfWork;

	public IUnitOfWork UnitOfWork { get; }

	public async Task HandleAsync(SelectAccount request, IClient client)
	{
		using (UnitOfWork) {
			client.Session.Account = (await UnitOfWork.Repository<Models.Account>().FindAsync(request.Id.Value))!;
			SelectAccountResponse response = new()
			{
				Success = true
			};

			await client.Send(response);
		}
	}
}