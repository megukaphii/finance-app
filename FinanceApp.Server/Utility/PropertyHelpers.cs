using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace FinanceApp.Server.Utility;

public static class PropertyHelpers
{
	public static int GetMinLength<TModel, TProperty>(Expression<Func<TModel, TProperty>> property)
	{
		Type type = property.Parameters.First().Type;
		return type.GetProperty(GetName(property))?.GetCustomAttribute<MinLengthAttribute>()?.Length ?? 0;
	}

	public static int GetMaxLength<TModel, TProperty>(Expression<Func<TModel, TProperty>> property)
	{
		Type type = property.Parameters.First().Type;
		return type.GetProperty(GetName(property))?.GetCustomAttribute<MaxLengthAttribute>()?.Length ?? 0;
	}

	private static string GetName<TModel, TProperty>(Expression<Func<TModel, TProperty>> property)
	{
		MemberExpression memberExpression = (MemberExpression)property.Body;
		return memberExpression.Member.Name;
	}
}