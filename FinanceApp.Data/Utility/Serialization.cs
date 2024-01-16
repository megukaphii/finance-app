using System.Text.Json;
using System.Text.Json.Serialization;
using FinanceApp.Data.Interfaces;

namespace FinanceApp.Data.Utility;

public static class Serialization
{
	public const string Eof = "<EOF>";
	public const string Error = "<ERROR>";

	public static string SerializeRequest<T>(T value) where T : IRequest
	{
		return T.Flag + Serialize(value);
	}

	public static string Serialize<T>(T value)
	{
		string json = JsonSerializer.Serialize(value, new JsonSerializerOptions
		{
			ReferenceHandler = ReferenceHandler.IgnoreCycles
		});
		return json + Eof;
	}

	public static object? Deserialize(string json, Type returnType)
	{
		string stripped = StripEof(json);
		return JsonSerializer.Deserialize(stripped, returnType);
	}

	public static T? Deserialize<T>(string json)
	{
		string stripped = StripEof(json);
		return JsonSerializer.Deserialize<T>(stripped);
	}

	private static string StripEof(string message)
	{
		return message.Replace(Eof, "");
	}
}