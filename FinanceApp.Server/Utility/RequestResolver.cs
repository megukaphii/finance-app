﻿using System.Reflection;
using FinanceApp.Data.Interfaces;
using FinanceApp.Data.Utility;
using FinanceApp.Server.Exceptions;

namespace FinanceApp.Server.Utility;

public static class RequestResolver
{
	private static List<Type> RequestTypes { get; } = new();

	public static T? Deserialize<T>(string message) where T : IRequest
	{
		message = message.Replace(T.Flag, "");
		return Serialization.Deserialize<T>(message);
	}

	public static IRequest GetRequest(string message)
	{
		CacheRequestTypes();

		foreach (Type t in RequestTypes) {
			PropertyInfo? flagProperty = t.GetProperty(nameof(IRequest.Flag));
			string flag = (string)flagProperty?.GetValue(null)!;
			if (flag != string.Empty && message.StartsWith(flag))
				try {
					IRequest request = (IRequest?)Serialization.Deserialize(message.Replace(flag, ""), t) ??
					                   throw new InvalidRequestException($"Could not deserialize message: {message}");

					return request;
				} catch (Exception e) {
					throw new InvalidRequestException($"Could not deserialize message: {message}", e);
				}
		}

		throw new InvalidMessageException($"No flag exists for message: {message}");
	}

	private static void CacheRequestTypes()
	{
		if (RequestTypes.Count == 0)
			RequestTypes.AddRange(AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(assembly => assembly.GetTypes())
				.Where(t =>
				{
					IEnumerable<Type> allInterfaces = t.GetInterfaces();
					IEnumerable<Type> immediateInterfaces =
						allInterfaces.Except(allInterfaces.SelectMany(immediateInterface =>
							immediateInterface.GetInterfaces()));

					return typeof(IRequest).IsAssignableFrom(t) &&
					       t != typeof(IRequest) &&
					       !immediateInterfaces.Contains(typeof(IRequest));
				}));
	}
}