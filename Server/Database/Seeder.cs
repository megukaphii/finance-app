namespace Server.Database;

internal static class Seeder {
	internal static void SeedDB() {
		using (Database db = new Database()) {
			string sql =
			@"
				INSERT INTO Transactions (
					Value
				)
				VALUES (
					120
				);
			";
			db.ExecuteNonQuery(sql, Parameters.Empty);
		}
	}
}