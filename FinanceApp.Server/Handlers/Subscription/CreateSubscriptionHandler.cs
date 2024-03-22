using FinanceApp.Data.Requests.Subscription;
using FinanceApp.Server.Interfaces;

namespace FinanceApp.Server.Handlers.Subscription;

public class CreateSubscriptionHandler : IRequestHandler<CreateSubscription>
{
	public CreateSubscriptionHandler(IUnitOfWork unitOfWork) => UnitOfWork = unitOfWork;

	public IUnitOfWork UnitOfWork { get; }

	public async Task HandleAsync(CreateSubscription request, IClient client)
	{
		// TODO - Create new transaction if subscription starts today
		using (UnitOfWork) {
			CreateSubscriptionResponse response;
			if (client.Session.IsAccountSet()) {
				Data.Models.Subscription created = new()
				{
					Account = (await UnitOfWork.Repository<Data.Models.Account>().FindAsync(client.Session.AccountId))!,
					Counterparty = (await UnitOfWork.Repository<Data.Models.Counterparty>().FindAsync(request.Counterparty.Value))!,
					Name = request.Name.Value,
					Value = request.Value.Value,
					FrequencyCounter = request.FrequencyCounter.Value,
					FrequencyMeasure = request.FrequencyMeasure.Value,
					StartDate = request.StartDate.Value,
					EndDate = request.EndDate.Value
				};
				await UnitOfWork.Repository<Data.Models.Subscription>().AddAsync(created);
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