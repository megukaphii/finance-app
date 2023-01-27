namespace Server.Database;

internal static class Migration {
	internal static void RefreshTables() {
		DropTables();
		CreateTablesIfNotExists();
	}

	private static void CreateTablesIfNotExists()
    {
		using (Database db = new Database()) {
			string sql =
			@"
				CREATE TABLE IF NOT EXISTS Transactions (
					ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
					Value INTEGER
				);
			";
			db.ExecuteNonQuery(sql, Parameters.Empty);
        }
    }

    private static void DropTables() {
		using (Database db = new Database()) {
			string sql = 
			@"
				DROP TABLE IF EXISTS Transactions
			";
			db.ExecuteNonQuery(sql, Parameters.Empty);
		}
	}
}
