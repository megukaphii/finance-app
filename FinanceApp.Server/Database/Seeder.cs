using FinanceApp.Abstractions;
using FinanceApp.Extensions.Sqlite;

namespace FinanceApp.Server.Database;

internal static class Seeder
{
    public static IDatabase? DB;

	internal static void SeedDB() {
		string sql =
		@"
			INSERT INTO Transactions (
				Value
			)
			VALUES (
				120
			);
		";
		DB?.ExecuteNonQuery(sql, ParameterCollection.Empty);
	}
}