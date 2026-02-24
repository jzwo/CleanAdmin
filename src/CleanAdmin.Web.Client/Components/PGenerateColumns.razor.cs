using System.Reflection;
using AntDesign;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace CleanAdmin.Web.Client.Components;

public class PGenerateColumns<TItem> : ComponentBase
{
    private static readonly PropertyInfo[] TypeProperties = typeof(TItem).GetProperties();
    private readonly Dictionary<(string PropertyName, Type PropertyType), ColumnTypeInfo> _columnTypeCache = new();

    private sealed record ColumnTypeInfo(
        Type ColumnType,
        PropertyInfo[]? Parameters,
        Func<IFieldColumn>? CreateInstance);

    /// <summary>
    /// Specific the range of the columns that need to display.
    /// </summary>
    [Parameter]
    public Range? Range { get; set; }

    /// <summary>
    /// Hide the columns by the property name.
    /// </summary>
    [Parameter]
    public IEnumerable<string> HideColumnsByName { get; set; } = new List<string>();

    /// <summary>
    /// An action used to define each generated column.
    /// </summary>
    [Parameter]
    public Action<string, IFieldColumn>? Definitions { get; set; }

    /// <summary>
    /// Specify start column index, use it if auto indexes disabled and there are columns before generated ones
    /// </summary>
    [Parameter]
    public int StartColumnIndex { get; set; }

    private ColumnTypeInfo GetColumnTypeInfo(string propertyName, Type propertyType)
    {
        var cacheKey = (propertyName, propertyType);
        if (_columnTypeCache.TryGetValue(cacheKey, out var cached))
        {
            return cached;
        }

        var underlyingType = GetUnderlyingType(propertyType);
        var columnType = typeof(Column<>).MakeGenericType(underlyingType);
        var parameters = columnType.GetProperties()
            .Where(x => x.GetCustomAttribute<ParameterAttribute>() != null)
            .Where(x => !x.Name.IsIn("DataIndex"))
            .ToArray();

        // Create a compiled factory delegate
        var constructor = columnType.GetConstructor(Type.EmptyTypes);

        var result = new ColumnTypeInfo(columnType, parameters, CreateInstance);

        _columnTypeCache[cacheKey] = result;
        return result;

        IFieldColumn CreateInstance() => (IFieldColumn?)constructor?.Invoke(null) ??
                                         throw new InvalidOperationException(
                                             $"Could not create instance of type {columnType.FullName}");
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var i = 0;
        var showProperties = Range != null ? TypeProperties[Range.Value] : TypeProperties;

        var colIndex = StartColumnIndex;
        foreach (var property in showProperties)
        {
            if (HideColumnsByName?.Contains(property.Name) == true) continue;

            var typeInfo = GetColumnTypeInfo(property.Name, property.PropertyType);
            var instance = typeInfo.CreateInstance?.Invoke();
            if (instance == null) throw new InvalidOperationException("Could not create column instance.");
            instance.ColIndex = colIndex;

            colIndex++;

            Definitions?.Invoke(property.Name, instance);

            var attributes = typeInfo.Parameters?
                .Select(x => new KeyValuePair<string, object?>(x.Name, x.GetValue(instance)))
                .Where(x => x.Value != null)
                .Select(x => new KeyValuePair<string, object>(x.Key, x.Value!));

            builder.OpenComponent(++i, typeInfo.ColumnType);
            builder.AddAttribute(++i, "DataIndex", property.Name);
            builder.AddMultipleAttributes(++i, attributes);

            builder.CloseComponent();
        }
    }

    public static Type GetUnderlyingType(Type type)
    {
        Type targetType;
        if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            targetType = Nullable.GetUnderlyingType(type) ?? type;
        }
        else
        {
            targetType = type;
        }

        return targetType;
    }
}
