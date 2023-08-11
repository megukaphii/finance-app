using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;

namespace FinanceApp.Abstractions;

public abstract class Eloquent
{
    // TODO - Would like this to be private and not nullable? But the parser is the issue with this.
    public IDatabase? Database;
    public bool ExistsOnDb;
    private long _id;
    private static readonly List<Tuple<PropertyInfo, SqlDbType>> DBColumnProperties = new();

    [Column("ID")]
    public long ID
    {
        get => _id;
        set
        {
            if (!ExistsOnDb)
            {
                _id = value;
            }
            else
            {
                throw new Exception("Cannot assign ID to existing DB entry");
            }
        }
    }

    public Eloquent Save()
    {
        return ExistsOnDb ? Update() : Insert();
    }

    protected abstract Eloquent Update();

    protected abstract Eloquent Insert();

    protected static ParameterCollection GetDBParams<T>(Eloquent obj) where T : Eloquent
    {
        CacheDBColumnProperties<T>();

        ParameterCollection parameters = new();
        foreach (Tuple<PropertyInfo, SqlDbType> col in DBColumnProperties.Where(c => c.Item1.Name != "ID"))
        {
            parameters.Add(new Parameter(col.Item2, $"${col.Item1.Name}", col.Item1.GetValue(obj)));
        }

        return parameters;
    }

    private static void CacheDBColumnProperties<T>() where T : Eloquent
    {
        if (DBColumnProperties.Count != 0) return;

        List<PropertyInfo> properties = GetDBColumnPropertyInfos<T>();
        List<SqlDbType> types = GetDBColumnTypes(properties);
        for (int i = 0; i < properties.Count; i++)
        {
            DBColumnProperties.Add(new Tuple<PropertyInfo, SqlDbType>(properties[i], types[i]));
        }
    }

    private static List<PropertyInfo> GetDBColumnPropertyInfos<T>() where T : Eloquent
    {
        Type type = typeof(T);

        return (from prop in type.GetProperties()
                let nameAttr = (ColumnAttribute?)prop
                    .GetCustomAttributes(typeof(ColumnAttribute), true)
                    .FirstOrDefault()
                where nameAttr?.Name != null
                select prop)
            .Where(prop => prop.Name != "ID")
            .ToList();
    }

    private static List<SqlDbType> GetDBColumnTypes(IEnumerable<PropertyInfo> properties)
    {
        return properties.Select(col => Type.GetTypeCode(col.PropertyType))
            .Select(typeCode => typeCode switch
            {
                TypeCode.Int64 => SqlDbType.Int,
                TypeCode.Boolean => SqlDbType.Bit,
                _ => SqlDbType.Text
            })
            .ToList();
    }
}