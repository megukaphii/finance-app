﻿using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Utility;

namespace FinanceApp.Data.Requests.Counterparty;

public class GetCounterpartiesHandler : IRequestHandler<GetCounterparties>
{
	public GetCounterpartiesHandler(IUnitOfWork unitOfWork) => UnitOfWork = unitOfWork;

	public IUnitOfWork UnitOfWork { get; }

	public async Task HandleAsync(GetCounterparties request, Client client)
	{
		using (UnitOfWork) {
			List<Models.Counterparty> counterparties = await UnitOfWork.Repository<Models.Counterparty>().AllAsync();

			GetCounterpartiesResponse response = new()
			{
				Counterparties = counterparties,
				Success = true
			};

			await client.Send(response);
		}
	}
}