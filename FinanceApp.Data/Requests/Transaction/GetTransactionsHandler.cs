using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Utility;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Data.Requests.Transaction;

public class GetTransactionsHandler : IRequestHandler<GetTransactions>
{
	public GetTransactionsHandler(IUnitOfWork unitOfWork) => UnitOfWork = unitOfWork;

	public IUnitOfWork UnitOfWork { get; }

	public async Task HandleAsync(GetTransactions request, Client client)
	{
		using (UnitOfWork) {
			List<Models.Transaction> transactions =
				await UnitOfWork.Repository<Models.Transaction>()
					.IncludeAll(transaction => transaction.Counterparty)
					.Where(transaction => transaction.Account.Equals(client.Session.Account))
					.ToListAsync();

			GetTransactionsResponse response = new()
			{
				Transactions = transactions,
				Success = true
			};

			await client.Send(response);
		}
	}
}