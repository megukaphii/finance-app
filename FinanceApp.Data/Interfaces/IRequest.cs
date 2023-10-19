using System.Reflection;
using FinanceApp.Data.Utility;
using System.Text.Json;

namespace FinanceApp.Data.Interfaces;

public interface IRequest
{
    private static List<Type> RequestTypes { get; } = new();
    public static virtual string Flag => string.Empty;
    public static virtual Type? Validator => null;

	public static bool IsValid<TRequest>(TRequest request) where TRequest : IRequest
    {
        if (typeof(TRequest) == typeof(InvalidRequest)) return false;

        if (TRequest.Validator is null) return true;

        if (TRequest.Validator.BaseType == typeof(IValidator)) {
            IValidator validator = (IValidator)Activator.CreateInstance(TRequest.Validator)!;
            return validator.Validate(request);
        }

        throw new(
            $"Validator {TRequest.Validator.Name} for {typeof(TRequest).Name} is not a child of {nameof(IValidator)}.");
    }

    public static IRequest GetRequest(string message)
    {
        CacheRequestTypes();

        foreach (Type t in RequestTypes) {
            PropertyInfo? property = t.GetProperty(nameof(Flag));
            string flag = (string)property?.GetValue(null)!;
            if (flag != string.Empty && message.StartsWith(flag)) {
                try
                {
                    IRequest request = (IRequest?) JsonSerializer.Deserialize(message.Replace(flag, ""), t) ??
                        throw new($"Message {message} does not contain valid {t.Name} properties");

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
        if (RequestTypes.Count == 0) {
            RequestTypes.AddRange(AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t =>
                {
                    IEnumerable<Type> allInterfaces = t.GetInterfaces();
                    IEnumerable<Type> immediateInterfaces = allInterfaces.Except(allInterfaces.SelectMany(immediateInterface => immediateInterface.GetInterfaces()));

                    return typeof(IRequest).IsAssignableFrom(t) &&
                           t != typeof(IRequest) &&
                           !immediateInterfaces.Contains(typeof(IRequest));
                }));
        }
    }

    public Task HandleAsync(FinanceAppContext database, SocketStream client);
}

public class InvalidRequest : IRequest
{
    public Exception Exception { get; }

    public static string Flag => throw new InvalidDataException();
    public static Type? Validator => throw new InvalidDataException();

    public InvalidRequest(Exception exception) => Exception = exception;

    public Task HandleAsync(FinanceAppContext database, SocketStream client)
    {
        throw new InvalidDataException();
    }
}