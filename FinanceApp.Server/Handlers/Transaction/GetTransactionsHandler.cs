using FinanceApp.Data.Requests.Transaction;
using FinanceApp.Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Server.Handlers.Transaction;

public class GetTransactionsHandler : IRequestHandler<GetTransactions>
{
	public GetTransactionsHandler(IUnitOfWork unitOfWork) => UnitOfWork = unitOfWork;

	public IUnitOfWork UnitOfWork { get; }

	public async Task HandleAsync(GetTransactions request, IClient client)
	{
		using (UnitOfWork) {
			GetTransactionsResponse response;
			if (client.Session.IsAccountSet()) {
				List<Data.Models.Transaction> transactions =
					await UnitOfWork.Repository<Data.Models.Transaction>()
						.Include(transaction => transaction.Counterparty)
						.Where(transaction => transaction.Account.Id.Equals(client.Session.AccountId))
						.ToListAsync();

				response = new()
				{
					Success = true,
					Transactions = transactions,
					Value = (await UnitOfWork.Repository<Data.Models.Account>().FindAsync(client.Session.AccountId))!.Value
				};
			} else {
				response = new()
				{
					Success = false,
					Transactions = new(),
					Value = 0
				};
			}

			await client.Send(response);
		}
	}
}