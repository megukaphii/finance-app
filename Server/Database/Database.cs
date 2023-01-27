using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Data;

namespace Server.Database;

public class Database : IDisposable {
	private SqliteConnection DB = new SqliteConnection("Data Source=test.db");

	public Database() {
		OpenConnection();
	}

	public Database OpenConnection() {
		DB.Open();
		return this;
	}

	public List<T> ExecuteReader<T>(string sql, Parameters vars) where T : new() /* Generally better design for model classes */ {
		var command = DB.CreateCommand();
		command.CommandText = sql;
		command.Parameters.AddRange(vars.ToSqliteParameterEnumerable());

		List<T> result = new();

		using (var reader = command.ExecuteReader()) {
			/*// Field count is the no. of columns, not rows? Though, hm. I guess we need a 2D array or something?
			// GetValues() obtains the column values of the current row. It's parameter is an object array of values
			// to be copied into.
			// Ah, yep, got it.
			// Hm, so, do we use GetValues and parse, or GetFieldType and GetFieldValue<T>? Probably the latter?
			// The latter does sound better, yeah hahahahah*/
			var type = typeof(T);
			var properties = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

			while (reader.Read()) {
				var t = new T();

				for (int i = 0; i < reader.FieldCount; i++) {
					Type fieldType = reader.GetFieldType(i);

					/*// How use Type?
					// But, isn't T in this context, the object, i.e. Transaction, while GetFieldValue<T> wants a T
					// Right, I forgot. We need a model class.
					// Oh yeah, one of those. What are they?
					// Basically a class with public getter and setter properties. Those properties are to be written to by this
					// reader and read from by the user.
					// Now, time for the usual reflection BS.
					// Oh dear God.
					// Hhahahahahah lol

					// Generally we only want public properties. We can tweak filter parameters with enum values as we see fit later.
					// ...also cos I can't be bothered thinking about it lol
					// Aha, fair enough.
					// Ah, I've placed it inside the loop. Whoops.

					// I feel like yours is a better way of doing this...
					// Maybe, but my brain melted in the process thinking about a couple of edge cases LOL
					// So your brain survived this hahahahahahaha
					// Oh dear, was it truly worth it for the code to look pretty?
					// On par for the course ;)*/

					var fieldName = reader.GetName(i);
					var field = properties.SingleOrDefault(p => p.Name == fieldName);

					// TODO: To throw or not to throw if the local class type does not contain property
					// corresponding to the incoming data from the database? Your call, Cory.
					if (field != null) {
						field.SetValue(t, reader.GetValue(i)); // TODO: Async
					} else {
						// Throw here if you want to throw when property does not exist in class
						throw new Exception($"{type.Name} doesn't contain field {fieldName}");
					}
				}
				result.Add(t);
			}
		}

		return result;
	}

	private T ParseObject<T>() where T : new() {
		// TODO: lil' refactor
		return new T();
	}

	public Database ExecuteNonQuery(string sql, Parameters vars) {
		var command = DB.CreateCommand();
		command.CommandText = sql;
		command.Parameters.AddRange(vars.ToSqliteParameterEnumerable());
		command.ExecuteNonQuery();
		return this;
	}

	public void Dispose() {
		DB.Close();
	}
}
