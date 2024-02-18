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

	public static int GetMinValue<TModel, TProperty>(Expression<Func<TModel, TProperty>> property)
	{
		Tuple<int, int> result = GetRange(property);
		return result.Item1;
	}

	public static int GetMaxValue<TModel, TProperty>(Expression<Func<TModel, TProperty>> property)
	{
		Tuple<int, int> result = GetRange(property);
		return result.Item2;
	}

	private static Tuple<int, int> GetRange<TModel, TProperty>(Expression<Func<TModel, TProperty>> property)
	{
		Type type = property.Parameters.First().Type;
		RangeAttribute? attribute = type.GetProperty(GetName(property))?.GetCustomAttribute<RangeAttribute>();
		Tuple<int, int> result = new((int)(attribute?.Minimum ?? 0), (int)(attribute?.Maximum ?? 0));
		return result;
	}

	private static string GetName<TModel, TProperty>(Expression<Func<TModel, TProperty>> property)
	{
		MemberExpression memberExpression = (MemberExpression)property.Body;
		return memberExpression.Member.Name;
	}
}