using System.Reflection;
using FinanceApp.Data.Utility;
using Newtonsoft.Json;

namespace FinanceApp.Data.Interfaces;

public interface IRequest
{
    private static IEnumerable<Type> _requestTypes = Array.Empty<Type>();
    public static readonly string Flag = string.Empty;
    public static readonly Type? Validator = null;

	private class InvalidRequest : IRequest
	{
        public Exception Exception { get; }

        public InvalidRequest(Exception exception) => Exception = exception;

		public Task Handle(FinanceAppContext database, SocketStream client)
		{
			throw new NotImplementedException();
		}
	}

	public bool IsValid()
    {
        if (this is InvalidRequest) return false;

        if (Validator is null) return true;

        if (Validator.BaseType == typeof(IValidator)) {
            IValidator validator = (IValidator)Activator.CreateInstance(Validator)!;
            return validator.Validate(this);
        }

        throw new Exception(
            $"Validator {Validator.Name} for {GetType().Name} is not a child of {nameof(IValidator)}.");
    }

    public static IRequest GetRequest(string message)
    {
        CacheRequestTypes();

        foreach (Type t in _requestTypes) {
            PropertyInfo? property = t.GetProperty(nameof(Flag));
            string flag = (string)property?.GetValue(null)!;
            if (flag != string.Empty && message.StartsWith(flag)) {
                try
                {
                    IRequest? request = (IRequest)JsonConvert.DeserializeObject(message.Replace(flag, ""), t)!;
                    if (request is null) {
                        throw new Exception($"Message {message} does not contain valid {t.Name} properties");
                    }

                    return request;
                }
                catch (Exception e)
                {
                    return new InvalidRequest(e);
                }
            }
        }

        throw new Exception($"No valid flag exists for message {message}");
    }

    private static void CacheRequestTypes()
    {
        if (_requestTypes == Array.Empty<Type>()) {
            _requestTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t =>
                {
                    IEnumerable<Type> allInterfaces = t.GetInterfaces();
                    IEnumerable<Type> immediateInterfaces = allInterfaces.Except(allInterfaces.SelectMany(immediateInterface => immediateInterface.GetInterfaces()));

                    return typeof(IRequest).IsAssignableFrom(t) &&
                           t != typeof(IRequest) &&
                           !immediateInterfaces.Contains(typeof(IRequest));
                });
        }
    }

    public Task Handle(FinanceAppContext database, SocketStream client);
}