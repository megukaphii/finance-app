using FinanceApp.Data.Requests.Subscription;
using FinanceApp.Server.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Server.Handlers.Subscription;

public class GetSubscriptionsHandler : IRequestHandler<GetSubscriptions>
{
	public GetSubscriptionsHandler(IUnitOfWork unitOfWork) => UnitOfWork = unitOfWork;

	public IUnitOfWork UnitOfWork { get; }

	public async Task HandleAsync(GetSubscriptions request, IClient client)
	{
		using (UnitOfWork) {
			List<Data.Models.Subscription> subscriptions =
				await UnitOfWork.Repository<Data.Models.Subscription>()
					.Include(subscription => subscription.Counterparty)
					.Where(subscription => subscription.Account.Equals(client.Session.Account))
					.ToListAsync();

			GetSubscriptionsResponse response = new()
			{
				Success = true,
				Subscriptions = subscriptions
			};

			await client.Send(response);
		}
	}
}