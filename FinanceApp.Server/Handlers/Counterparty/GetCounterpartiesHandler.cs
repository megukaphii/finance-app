using FinanceApp.Data.Requests.Counterparty;
using FinanceApp.Server.Interfaces;

namespace FinanceApp.Server.Handlers.Counterparty;

public class GetCounterpartiesHandler : IRequestHandler<GetCounterparties>
{
	public GetCounterpartiesHandler(IUnitOfWork unitOfWork) => UnitOfWork = unitOfWork;

	public IUnitOfWork UnitOfWork { get; }

	public async Task HandleAsync(GetCounterparties request, IClient client)
	{
		using (UnitOfWork) {
			List<Data.Models.Counterparty> counterparties =
				await UnitOfWork.Repository<Data.Models.Counterparty>().AllAsync();

			GetCounterpartiesResponse response = new()
			{
				Success = true,
				Counterparties = counterparties
			};

			await client.Send(response);
		}
	}
}