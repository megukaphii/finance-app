using FinanceApp.DatabaseInterfaces;
using FinanceApp.Extensions.Sqlite;

namespace Server.Data;

public class Transaction {
    // MONEY MONEY MONEY MONEY MONEY TRANSACTION MONEY MONEY I believe that's how banks work

    public long ID { get; set; }
    public long Value { get; set; }

	public static List<Transaction> All() {
		using (SqliteDatabase db = new SqliteDatabase()) {
			string sql =
			@"
				SELECT *
				FROM Transactions
			";
			return db.ExecuteReader<Transaction>(sql, ParameterCollection.Empty);
		}
	}

	public override string ToString() {
		return $"Transaction ID: {ID}, Value: {Value}";
	}

	public void ReadFromDatabase() {
		/*using (var connection = new SqliteConnection("Data Source=test.db")) {
			connection.Open();

			var command = connection.CreateCommand();
			command.CommandText =
			@"
				SELECT *
				FROM transactions
				WHERE id = $id
			";

			command.Parameters.AddWithValue("$id", ID);

			using (var reader = command.ExecuteReader()) {
				while (reader.Read()) {
					var name = reader.GetString(0);

					Console.WriteLine($"Hello, {name}!");
				}
			}
		}*/
	}
}