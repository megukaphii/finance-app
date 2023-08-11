using System.Reflection;
using Newtonsoft.Json;

namespace FinanceApp.Abstractions;

public interface IRequest
{
	private static IEnumerable<Type> _requestTypes = Array.Empty<Type>();

	public static abstract string Flag { get; }

	public static IRequest GetRequest(string message)
	{
		CacheRequestTypes();

		foreach (Type t in _requestTypes)
		{
			PropertyInfo? property = t.GetProperty(nameof(Flag));
			string flag = (string)property?.GetValue(null)!;
			if (message.StartsWith(flag)) {
				IRequest request = (IRequest)JsonConvert.DeserializeObject(message.Replace(flag, ""), t)!;
				return request;
			}
		}

		throw new Exception($"No valid flag exists for message {message}");
	}

    private static void CacheRequestTypes()
    {
        if (_requestTypes == Array.Empty<Type>())
        {
            _requestTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t => typeof(IRequest).IsAssignableFrom(t) && t != typeof(IRequest));
        }
    }

    public Task Handle(IDatabase database, Stream stream);
}