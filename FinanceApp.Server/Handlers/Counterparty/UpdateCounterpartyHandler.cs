using FinanceApp.Data.Requests.Counterparty;
using FinanceApp.Server.Interfaces;

namespace FinanceApp.Server.Handlers.Counterparty;

public class UpdateCounterpartyHandler : IRequestHandler<UpdateCounterparty>
{
	public UpdateCounterpartyHandler(IUnitOfWork unitOfWork) => UnitOfWork = unitOfWork;

	public IUnitOfWork UnitOfWork { get; }

	public async Task HandleAsync(UpdateCounterparty request, IClient client)
	{
		using (UnitOfWork) {
			Data.Models.Counterparty counterparty = (await UnitOfWork.Repository<Data.Models.Counterparty>().FindAsync(request.Id.Value))!;
			counterparty.Name = request.Name.Value;
			UnitOfWork.SaveChanges();

			UpdateCounterpartyResponse response = new()
			{
				Success = true
			};

			await client.Send(response);
		}
	}
}