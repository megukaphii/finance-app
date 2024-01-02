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
			List<Data.Models.Transaction> transactions =
				await UnitOfWork.Repository<Data.Models.Transaction>()
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