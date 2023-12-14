using FinanceApp.Data.Interfaces;

namespace FinanceApp.Data.Requests.Account;

public class CreateAccountHandler : IRequestHandler<CreateAccount>
{
	public CreateAccountHandler(IUnitOfWork unitOfWork) => UnitOfWork = unitOfWork;

	public IUnitOfWork UnitOfWork { get; }

	public async Task HandleAsync(CreateAccount request, IClient client)
	{
		using (UnitOfWork) {
			Models.Account created = new()
			{
				Name = request.Name.Value,
				Description = request.Description.Value,
				Value = 0
			};
			await UnitOfWork.Repository<Models.Account>().AddAsync(created);
			UnitOfWork.SaveChanges();

			CreateAccountResponse response = new()
			{
				Id = created.Id,
				Success = true
			};

			await client.Send(response);
		}
	}
}