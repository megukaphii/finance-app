using Microsoft.Data.Sqlite;
using System.Collections;
using System.Data;

namespace Server.Database;

public class Parameters : IEnumerable<Parameter> {
	// So yeah, do we just make this a wrapper for SqlParameter or something to that effect? Do we even need a wrapper?
	// I mean honestly we can do either way. Whichever one you want
	// Well, I did hear something about wrapping dependencies and stuff, though I imagine that was more in referene to external libraries, but having more control over it probably wouldn't hurt anyway?
	// Thanos: Fine, I'll do it myself. Haha, yes, indeed. So, will you be Thanos in this situation? I think probably, yes, Thanos was right.
	// Hearing you being a Thanos supporter is like hearing someone you know actually be a Trump supporter the whole time. You don't exactly expect it.
	// Cory: Moider Aha, oh no. I mean, MatPat did say that too. So how do we create a wrapper class like that? Lets see...
	private List<Parameter> _parameters = new();
	public static Parameters Empty { get { return new Parameters(); } }

	public IEnumerable<SqliteParameter> ToSqliteParameterEnumerable() {
		// This presents a bit of a problem...
		// Do you want this to be iterable in a foreach loop iterating over SqliteParameters?
		// Well, the usage is command.Parameters = or command.Parameters.Add or command.Parameters.AddRange, and given that the
		// accesibilty level of the collection is a problem, I feel the next simplest option is to return an IEnumberale and use
		// .AddRange()
		// The easiest way to do this is to expose the internal SqliteParameter member in Parameter: perhaps by using a get only property.
		// Then you can use LINQ in this function. *wow emoji* Imagine being so extra as to typing out "emoji"
		// Well, sadly, I'm not intimately familiar with any codes for said symbol, so. Nor am I aware if it exists in UTF-8 at all.
		// Yeah probably not lol I don't know any either
		return _parameters.Select(p => p.SqliteParameter);
	}

	public Parameters AddParameter(Parameter param) {
		_parameters.Add(param);
		return this;
	}

	public Parameters AddParameter(SqlDbType type, string key, object value) {
		_parameters.Add(new Parameter(type, key, value));
		return this;
	}

	public IEnumerator<Parameter> GetEnumerator() {
		return _parameters.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}
}
