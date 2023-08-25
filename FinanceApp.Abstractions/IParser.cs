using System.Reflection;

namespace FinanceApp.Abstractions;

public interface IParser
{
    public List<T> PerformParse<T>() where T : Eloquent, new();
}