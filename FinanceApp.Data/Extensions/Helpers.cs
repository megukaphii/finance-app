using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace FinanceApp.Data.Extensions;

public static class Helpers
{
	public static int GetPropertyMaxLength<TModel, TProperty>(Expression<Func<TModel, TProperty>> property)
	{
		Type type = property.Parameters.First().Type;
		return type.GetProperty(GetPropertyName(property))?.GetCustomAttribute<MaxLengthAttribute>()?.Length ?? 0;
	}

	private static string GetPropertyName<TModel, TProperty>(Expression<Func<TModel, TProperty>> property)
    {
        MemberExpression memberExpression = (MemberExpression)property.Body;
        return memberExpression.Member.Name;
    }
}