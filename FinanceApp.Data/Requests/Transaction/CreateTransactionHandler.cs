using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Utility;

namespace FinanceApp.Data.Requests.Transaction;

public class CreateTransactionHandler : IRequestHandler<CreateTransaction>
{
	public CreateTransactionHandler(IUnitOfWork unitOfWork) => UnitOfWork = unitOfWork;

	public IUnitOfWork UnitOfWork { get; }

	public async Task HandleAsync(CreateTransaction request, Client client)
	{
		using (UnitOfWork) {
			Models.Counterparty? counterparty = request.Counterparty.Value;
			if (request.Counterparty.Value.Id == 0) {
				counterparty = await UnitOfWork.Repository<Models.Counterparty>()
					               .FirstOrDefaultAsync(
						               temp => temp.Name == request.Counterparty.Value.Name);
				if (counterparty is null) {
					await UnitOfWork.Repository<Models.Counterparty>().AddAsync(request.Counterparty.Value);
					counterparty = request.Counterparty.Value;
				}
			}

			if (!client.Session.IsAccountSet()) {
				// TODO - Make sure this is handled properly! Probably doesn't currently work
				CreateTransactionResponse response = new()
				{
					Id = 0,
					Success = false
				};

				await client.Send(response);
			} else {
				Models.Transaction created = new()
				{
					Account = client.Session.Account,
					Counterparty = counterparty,
					Value = request.Value.Value
				};
				UnitOfWork.AttachAccount(created.Account);
				await UnitOfWork.Repository<Models.Transaction>().AddAsync(created);
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