using FinanceApp.Data.Requests.Counterparty;
using FinanceApp.Server.Interfaces;

namespace FinanceApp.Server.Handlers.Counterparty;

public class CreateCounterpartyHandler : IRequestHandler<CreateCounterparty>
{
	public CreateCounterpartyHandler(IUnitOfWork unitOfWork) => UnitOfWork = unitOfWork;

	public IUnitOfWork UnitOfWork { get; }

	public async Task HandleAsync(CreateCounterparty request, IClient client)
	{
		using (UnitOfWork) {
			Data.Models.Counterparty created = new()
			{
				Name = request.Name.Value
			};
			await UnitOfWork.Repository<Data.Models.Counterparty>().AddAsync(created);
			UnitOfWork.SaveChanges();

			CreateCounterpartyResponse response = new()
			{
				Success = true,
				Id = created.Id
			};

			await client.Send(response);
		}
	}
}