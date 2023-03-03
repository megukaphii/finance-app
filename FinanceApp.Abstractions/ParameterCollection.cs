using System.Collections;
using System.Data;

namespace FinanceApp.Abstractions;

public class ParameterCollection : IEnumerable<Parameter> {
	private List<Parameter> parameters = new List<Parameter>();

	private static ParameterCollection empty = new();
	public static ParameterCollection Empty => empty;

	public ParameterCollection() {}

	public ParameterCollection(IEnumerable<Parameter> parameters) {
		this.parameters.AddRange(parameters);
	}

	public List<T> ConvertParameters<T>(Func<Parameter, T> sqlParameterAdapterFactory) {
		return parameters.Select(p => sqlParameterAdapterFactory(p)).ToList();
	}

	public IEnumerator<Parameter> GetEnumerator() {
		return parameters.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}

	public void Add(Parameter p) => parameters.Add(p);
}

public class Parameter {
	public SqlDbType type;
	public string name;
	public object value;

	public Parameter(SqlDbType type, string name, object value) {
		this.type = type;
		this.name = name;
		this.value = value;
	}
}