using FinanceApp.Data.Requests.Transaction;
using FinanceApp.Server.Extensions;
using FinanceApp.Server.Interfaces;

namespace FinanceApp.Server.Handlers.Transaction;

public class CreateTransactionHandler : IRequestHandler<CreateTransaction>
{
	public CreateTransactionHandler(IUnitOfWork unitOfWork) => UnitOfWork = unitOfWork;

	public IUnitOfWork UnitOfWork { get; }

	public async Task HandleAsync(CreateTransaction request, IClient client)
	{
		using (UnitOfWork) {
			CreateTransactionResponse response;
			if (client.Session.IsAccountSet()) {
				Data.Models.Transaction created = new()
				{
					Account = (await UnitOfWork.Repository<Data.Models.Account>().FindAsync(client.Session.AccountId))!,
					Counterparty = (await UnitOfWork.Repository<Data.Models.Counterparty>().FindAsync(request.Counterparty.Value))!,
					Value = request.Value.Value,
					Timestamp = request.Timestamp.Value
				};
				await UnitOfWork.AddTransaction(created);
				UnitOfWork.SaveChanges();

				response = new()
				{
					Success = true,
					Id = created.Id
				};
			} else {
				response = new()
				{
					Success = false,
					Id = 0
				};
			}

			await client.Send(response);
		}
	}
}