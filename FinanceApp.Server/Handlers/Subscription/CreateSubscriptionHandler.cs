using FinanceApp.Data.Requests.Subscription;
using FinanceApp.Server.Extensions;
using FinanceApp.Server.Interfaces;

namespace FinanceApp.Server.Handlers.Subscription;

public class CreateSubscriptionHandler : IRequestHandler<CreateSubscription>
{
	public CreateSubscriptionHandler(IUnitOfWork unitOfWork) => UnitOfWork = unitOfWork;

	public IUnitOfWork UnitOfWork { get; }

	public async Task HandleAsync(CreateSubscription request, IClient client)
	{
		using (UnitOfWork) {
			CreateSubscriptionResponse response;
			if (client.Session.IsAccountSet()) {
				Data.Models.Account account = (await UnitOfWork.Repository<Data.Models.Account>().FindAsync(client.Session.AccountId))!;
				Data.Models.Counterparty counterparty =
					(await UnitOfWork.Repository<Data.Models.Counterparty>().FindAsync(request.Counterparty.Value))!;
				Data.Models.Subscription created = new()
				{
					Account = account,
					Counterparty = counterparty,
					Name = request.Name.Value,
					Value = request.Value.Value,
					FrequencyCounter = request.FrequencyCounter.Value,
					FrequencyMeasure = request.FrequencyMeasure.Value,
					StartDate = request.StartDate.Value,
					EndDate = request.EndDate.Value
				};
				await UnitOfWork.Repository<Data.Models.Subscription>().AddAsync(created);
				if (request.StartDate.Value == DateTime.Today) {
					Data.Models.Transaction initialTransaction = new()
					{
						Account = account,
						Counterparty = counterparty,
						Value = request.Value.Value,
						Timestamp = request.StartDate.Value
					};
					await UnitOfWork.AddTransaction(initialTransaction);
				}
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