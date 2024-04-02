using FinanceApp.Data.Models;
using FinanceApp.Server.Interfaces;

namespace FinanceApp.Server.Extensions;

// ReSharper disable once InconsistentNaming
public static class IUnitOfWorkExtensions
{
	public static async Task AddTransaction(this IUnitOfWork unitOfWork, Transaction transaction)
	{
		await unitOfWork.Repository<Transaction>().AddAsync(transaction);
		Account account = (await unitOfWork.Repository<Account>().FindAsync(transaction.Account.Id))!;
		account.Value += transaction.Value;
	}
}