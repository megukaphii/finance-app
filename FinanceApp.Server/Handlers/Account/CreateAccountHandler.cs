using FinanceApp.Data.Requests.Account;
using FinanceApp.Server.Interfaces;

namespace FinanceApp.Server.Handlers.Account;

public class CreateAccountHandler : IRequestHandler<CreateAccount>
{
	public CreateAccountHandler(IUnitOfWork unitOfWork) => UnitOfWork = unitOfWork;

	public IUnitOfWork UnitOfWork { get; }

	public async Task HandleAsync(CreateAccount request, IClient client)
	{
		using (UnitOfWork) {
			Data.Models.Account created = new()
			{
				Name = request.Name.Value,
				Description = request.Description.Value,
				Value = 0
			};
			await UnitOfWork.Repository<Data.Models.Account>().AddAsync(created);
			UnitOfWork.SaveChanges();

			CreateAccountResponse response = new()
			{
				Success = true,
				Id = created.Id
			};

			await client.Send(response);
		}
	}
}