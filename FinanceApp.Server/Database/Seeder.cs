using FinanceApp.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Server.Database;

internal static class Seeder
{
    private static readonly FinanceAppContext DB = null!;

	internal static async void SeedDB() {
		string sql =
		@"
			INSERT INTO Transactions (
			    CounterpartyId,
				Value
			)
			VALUES (
			    1,
				120
			);
		";
        await DB.Database.ExecuteSqlRawAsync(sql);
    }
}