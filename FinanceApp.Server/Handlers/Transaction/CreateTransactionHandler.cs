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
			Data.Models.Counterparty? counterparty = request.Counterparty.Value;
			if (request.Counterparty.Value.Id == 0) {
				counterparty = await UnitOfWork.Repository<Data.Models.Counterparty>()
					               .FirstOrDefaultAsync(
						               temp => temp.Name == request.Counterparty.Value.Name);
				if (counterparty is null) {
					await UnitOfWork.Repository<Data.Models.Counterparty>().AddAsync(request.Counterparty.Value);
					counterparty = request.Counterparty.Value;
				}
			}

			if (!client.Session.IsAccountSet()) {
				CreateTransactionResponse response = new()
				{
					Id = 0,
					Success = false
				};

				await client.Send(response);
			} else {
				Data.Models.Transaction created = new()
				{
					Account = client.Session.Account,
					Counterparty = counterparty,
					Value = request.Value.Value
				};
				UnitOfWork.AttachAccount(created.Account);
				await UnitOfWork.Repository<Data.Models.Transaction>().AddAsync(created);
				UnitOfWork.SaveChanges();

				CreateTransactionResponse response = new()
				{
					Id = created.Id,
					Success = true
				};

				await client.Send(response);
			}
		}
	}
}