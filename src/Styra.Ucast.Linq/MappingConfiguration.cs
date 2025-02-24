using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Styra.Ucast.Linq;

public class NameToLINQExpressionConfiguration : Dictionary<string, Func<ParameterExpression, Expression>>;

/// <summary>
/// This type helps wrap up the complexities of building LINQ expressions
/// filtering, and for generating dynamic property lookups.
/// </summary>
/// <typeparam name="T">The type to build a name mapping config over.</typeparam>
public class MappingConfiguration<T>
{
    // Cache of generated mappings, so we don't have to reconstruct them every time.
    protected Dictionary<string, string> nameMappings = new(typeof(T).GetProperties().Length);
    protected Dictionary<string, Func<ParameterExpression, Expression>> linqMappingsCache = new(typeof(T).GetProperties().Length);
    protected string namePrefix = typeof(T).Name.ToLower();

    /// <summary>
    ///   <para>
    ///   Constructs a MappingConfiguration object that maps UCAST
    ///   property names to lambda functions.<br />
    ///   The lambda functions allow late-binding a LINQ data source into LINQ
    ///   Property expression lookups, which are used extensively when building
    ///   conditions over EF Core models.
    ///   </para>
    ///   <para>
    ///   When deciding on names for data source properties, we follow a small set
    ///   of default construction rules:
    ///   <list type="bullet">
    ///    <item>Example.Id -> "example.id"</item>
    ///    <item>Example.LastUpdated -> "example.last_updated"</item>
    ///   </list>
    ///   </para>
    /// </summary>
    /// <param name="namesToProperties">A dictionary, mapping UCAST field names to property lookups in the object.</param>
    /// <param name="prefix">Name of the LINQ data source, as it will appear in UCAST field references. Used as a prefix for the generated property mappings.</param>
    public MappingConfiguration(Dictionary<string, string>? namesToProperties = null, string? prefix = null)
    {
        namePrefix = prefix ?? namePrefix;
        foreach (var property in typeof(T).GetProperties())
        {
            var snakeCasedProperty = property.Name.ToSnakeCase();
            snakeCasedProperty = string.IsNullOrEmpty(namePrefix) ? snakeCasedProperty : $"{namePrefix}.{snakeCasedProperty}";
            nameMappings[snakeCasedProperty] = property.Name;
            linqMappingsCache[snakeCasedProperty] = param => Expression.Property(param, property.Name);
        }
        if (namesToProperties is not null)
        {
            foreach (var mapping in namesToProperties)
            {
                // Split string on '.' characters, and build out the Expression.Property() chain accordingly.
                var parts = mapping.Value.Split('.');
                var startIdx = parts[0] == prefix ? 0 : 1;
                linqMappingsCache[mapping.Key] = param => Expression.Property(param, parts[startIdx]);
                for (var i = startIdx + 1; i < parts.Length; i++)
                {
                    var part = parts[i];
                    linqMappingsCache[mapping.Key] = param => Expression.Property(linqMappingsCache[mapping.Key](param), part);
                }
            }
        }
    }

    /// <summary>
    /// Returns all stored UCAST to LINQ mappings.
    /// </summary>
    /// <returns>
    /// A dictionary where the keys are UCAST field names, and the values are LINQ lambdas.
    /// </returns>
    public Dictionary<string, Func<ParameterExpression, Expression>> GetLinqMappings()
    {
        return new(linqMappingsCache);
    }

    /// <summary>
    /// Indexer method providing direct access, as if this class were a dictionary.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Func<ParameterExpression, Expression> this[string name]
    {
        get
        {
            return linqMappingsCache[name];
        }
    }

    /// <summary>
    /// Retrieves an object property dynamically, given the UCAST field name.
    /// </summary>
    /// <returns>
    /// The value stored in the field, or else null.
    /// </returns>
    /// <remarks>
    /// Uses reflection, so this method may be very slow.
    /// </remarks>
    public object? GetPropertyByName(string name, ref T source)
    {
        if (source is not null && nameMappings.TryGetValue(name, out var accessor))
        {
            return source.GetType().GetProperty(accessor)?.GetValue(source);
        }
        return null;
    }

    /// <summary>
    /// Sets an object property dynamically, given the UCAST field name and a value.
    /// </summary>
    /// <remarks>
    /// Uses reflection, so it may be very slow.
    /// </remarks>
    public void SetPropertyByName(string name, ref T source, object? value)
    {
        if (source is not null && nameMappings.TryGetValue(name, out var accessor))
        {
            var prop = source.GetType().GetProperty(accessor);
            prop?.SetValue(source, value);
        }
    }
}
/// <summary>
/// Adds extensions to the name mapping logic, specific to Entity Framework Core model classes.
/// </summary>
/// <typeparam name="T">The type to build a name mapping config over.</typeparam>
public class EFCoreMappingConfiguration<T> : MappingConfiguration<T>
{
    /// <summary>
    ///   <para>
    ///   Constructs an EFCoreMappingConfiguration object that maps UCAST
    ///   property names to lambda functions.<br />
    ///   The lambda functions allow late-binding a LINQ data source into LINQ
    ///   Property expression lookups, which are used extensively when building
    ///   conditions over EF Core models.
    ///   </para>
    ///   <para>
    ///   When deciding on names for data source properties, we follow a small set
    ///   of default construction rules:
    ///   <list type="bullet">
    ///    <item>Example.Id -> "example.id"</item>
    ///    <item>Example.LastUpdated -> "example.last_updated"</item>
    ///    <item>Example.UserNavigation.Id -> "example.user.id"</item>
    ///   </list>
    ///   </para>
    /// </summary>
    /// <param name="namesToProperties">A dictionary, mapping UCAST field names to property lookups in the object.</param>
    /// <param name="prefix">Name of the LINQ data source, as it will appear in UCAST field references. Used as a prefix for the generated property mappings.</param>
    public EFCoreMappingConfiguration(Dictionary<string, string> namesToProperties, string? prefix = null) : base(namesToProperties)
    {
        var properties = typeof(T).GetProperties();
        foreach (var property in properties)
        {
            var propertyName = property.Name;
            // Normal properties, or a property just named "navigation" (case invariant) should be processed normally.
            if (!propertyName.EndsWith("Navigation") || propertyName.ToLower() == "Navigation")
            {
                propertyName = string.IsNullOrEmpty(prefix) ? propertyName.ToSnakeCase() : $"{prefix}.{propertyName.ToSnakeCase()}";
                nameMappings[propertyName] = property.Name;
                linqMappingsCache[propertyName] = param => Expression.Property(param, property.Name);
                continue;
            }
            // Implicit else: Properties with the "Navigation" suffix are
            // usually ORM tooling for foreign key/entity lookups in EF Core. We
            // indirect one level, and enumerate the non-Navigation properties
            // of that type.
            propertyName = property.Name[..^"Navigation".Length];
            propertyName = string.IsNullOrEmpty(prefix) ? propertyName.ToSnakeCase() : $"{prefix}.{propertyName.ToSnakeCase()}";

            Type memberType = property.PropertyType;
            var memberProperties = memberType.GetProperties();
            foreach (var memberProp in memberProperties)
            {
                // Skip cases like "Ticket.CustomerNavigation.TenantNavigation".
                if (memberProp.Name.EndsWith("Navigation") && !memberProp.Name.Equals("Navigation", StringComparison.CurrentCultureIgnoreCase))
                {
                    continue;
                }
                var memberPropertyName = memberProp.Name.ToSnakeCase();
                linqMappingsCache[$"{propertyName}.{memberPropertyName}"] = param => Expression.Property(Expression.Property(param, property.Name), memberPropertyName);
            }
        }
        if (namesToProperties is not null)
        {
            foreach (var mapping in namesToProperties)
            {
                // Split string on '.' characters, and build out the Expression.Property() chain accordingly.
                var parts = mapping.Value.Split('.');
                var startIdx = parts[0] == prefix ? 0 : 1;
                linqMappingsCache[mapping.Key] = param => Expression.Property(param, parts[startIdx]);
                for (var i = startIdx + 1; i < parts.Length; i++)
                {
                    var part = parts[i];
                    linqMappingsCache[mapping.Key] = param => Expression.Property(linqMappingsCache[mapping.Key](param), part);
                }
            }
        }
    }
}

