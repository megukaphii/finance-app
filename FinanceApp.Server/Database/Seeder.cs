using FinanceApp.Abstractions;
using FinanceApp.Extensions.Sqlite;

namespace FinanceApp.Server.Database;

internal static class Seeder {
	internal static void SeedDB() {
		using (SqliteDatabase db = new SqliteDatabase()) {
			string sql =
			@"
				INSERT INTO Transactions (
					Value
				)
				VALUES (
					120
				);
			";
			db.ExecuteNonQuery(sql, ParameterCollection.Empty);
		}
	}
}