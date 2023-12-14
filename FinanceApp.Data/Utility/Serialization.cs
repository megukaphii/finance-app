using System.Text.Json;
using System.Text.Json.Serialization;
using FinanceApp.Data.Interfaces;

namespace FinanceApp.Data.Utility;

public static class Serialization
{
	public const string Eof = "<EOF>";
	public const string Error = "<ERROR>";

	public static string Serialize<T>(T value) where T : IRequest
	{
		string json = JsonSerializer.Serialize(value, new JsonSerializerOptions
		{
			ReferenceHandler = ReferenceHandler.IgnoreCycles
		});
		return T.Flag + json + Eof;
	}

	public static string Serialize(IResponse value)
	{
		string json = JsonSerializer.Serialize(value, new JsonSerializerOptions
		{
			ReferenceHandler = ReferenceHandler.IgnoreCycles
		});
		return json + Eof;
	}

	public static object? Deserialize(string json, Type returnType)
	{
		string stripped = json.Replace(Eof, "");
		return JsonSerializer.Deserialize(stripped, returnType);
	}

	public static T? Deserialize<T>(string json)
	{
		return JsonSerializer.Deserialize<T>(json);
	}
}