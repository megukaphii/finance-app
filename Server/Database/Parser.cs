using FinanceApp.DatabaseInterfaces;
using System.Reflection;

namespace Server.Database;
internal class Parser {
	private readonly IDataReader reader;
	private readonly Type parseInto;
	private readonly PropertyInfo[] properties;

	public Parser(IDataReader reader, Type parseInto, PropertyInfo[] properties) {
		this.reader = reader;
		this.parseInto = parseInto;
		this.properties = properties;
	}

	public List<T> PerformParse<T>() where T : new() {
		List<T> result = new();

		while (reader.Read()) {
			result.Add(ParseObject<T>());
		}

		return result;
	}

	private T ParseObject<T>() where T : new() {
		T t = new();
		for (int i = 0; i < reader.FieldCount; i++) {
			Type fieldType = reader.GetFieldType(i);

			string fieldName = reader.GetName(i);
			PropertyInfo? field = properties.SingleOrDefault(p => p.Name == fieldName);

			if (field != null) {
				field.SetValue(t, reader.GetValue(i));
			} else {
				throw new Exception($"{parseInto.Name} doesn't contain field {fieldName}");
			}
		}
		return t;
	}
}
