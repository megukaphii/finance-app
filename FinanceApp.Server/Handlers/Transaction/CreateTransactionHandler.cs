using FinanceApp.Data.Requests.Transaction;
using FinanceApp.Server.Interfaces;

namespace FinanceApp.Server.Handlers.Transaction;

public class CreateTransactionHandler : IRequestHandler<CreateTransaction>
{
	public CreateTransactionHandler(IUnitOfWork unitOfWork) => UnitOfWork = unitOfWork;

	public IUnitOfWork UnitOfWork { get; }

	public async Task HandleAsync(CreateTransaction request, IClient client)
	{
		using (UnitOfWork) {
			if (!client.Session.IsAccountSet()) {
				CreateTransactionResponse response = new()
				{
					Success = false,
					Id = 0
				};

				await client.Send(response);
			} else {
				UnitOfWork.AttachAccount(client.Session.Account);
				Data.Models.Transaction created = new()
				{
					Account = client.Session.Account,
					Counterparty = (await UnitOfWork.Repository<Data.Models.Counterparty>()
						                .FindAsync(request.Counterparty.Value))!,
					Value = request.Value.Value,
					Timestamp = request.Timestamp.Value
				};
				await UnitOfWork.Repository<Data.Models.Transaction>().AddAsync(created);
				client.Session.Account.Value += created.Value;
				UnitOfWork.SaveChanges();

				CreateTransactionResponse response = new()
				{
					Success = true,
					Id = created.Id
				};

				await client.Send(response);
			}
		}
	}
}