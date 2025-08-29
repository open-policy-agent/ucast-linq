using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace OpenPolicyAgent.Ucast.Linq;

public class NameToLINQExpressionConfiguration : Dictionary<string, Func<ParameterExpression, Expression>>;

/// <summary>
/// This type helps wrap up the complexities of building LINQ expressions
/// filtering, and for generating dynamic property lookups.
/// </summary>
/// <typeparam name="T">The type to build a name mapping config over.</typeparam>
public class MappingConfiguration<T>
{
    // Cache of generated mappings, so we don't have to reconstruct them every time.
    protected Dictionary<string, string> baseNameMappings = GetUCASTToPropertyNamesMapping();
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
        foreach (var kv in baseNameMappings)
        {
            var key = $"{namePrefix}.{kv.Key}";
            nameMappings[key] = kv.Value;
            linqMappingsCache[key] = param => Expression.Property(param, kv.Value);
        }
        if (namesToProperties is not null)
        {
            foreach (var mapping in namesToProperties)
            {
                nameMappings[mapping.Key] = nameMappings[mapping.Value];
                // Split string on '.' characters, and build out the Expression.Property() chain accordingly.
                var parts = mapping.Value.Split('.');

                // Build up the linq expression by wrapping each successive member access.
                // Example params when evaluated for "ticket.customer.id": (ParameterExpression, {"ticket"}, {"customer", "id"})
                linqMappingsCache[mapping.Key] = param => NestedPropertyAccessFromParts(param, parts[..1], parts[1..]);
            }
        }
    }

    // baseExpr will be a ParameterExpression normally at the top-level.
    public Expression NestedPropertyAccessFromParts(Expression baseExpr, string[] seen, string[] remaining)
    {
        // Base case, no indexing left to do!
        if (remaining.Length == 0)
        {
            return baseExpr;
        }
        // Recursive case: Pull off a property, and dive down to the next level.
        var indexer = nameMappings[string.Join(".", [.. seen, remaining[0]])].Split(".").Last();
        return NestedPropertyAccessFromParts(Expression.Property(baseExpr, indexer), [.. seen, remaining[0]], remaining[1..]);
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
    /// Returns the stored UCAST field name to property name mappings.
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, string> GetNameMappings()
    {
        return new(nameMappings);
    }

    /// <summary>
    /// Indexer method providing direct access, as if this class were a dictionary.
    /// </summary>
    /// <param name="name">The UCAST field name to look up.</param>
    /// <returns></returns>
    public Func<ParameterExpression, Expression> this[string name]
    {
        get
        {
            return linqMappingsCache[name];
        }
    }

    /// <summary>
    /// Removes a mapping, given the UCAST field name.
    /// </summary>
    /// <param name="name">The UCAST field name to attempt to remove.</param>
    /// <returns></returns>
    public bool Remove(string name)
    {
        nameMappings.Remove(name);
        return linqMappingsCache.Remove(name);
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
            return GetPropertyValue(source, accessor);
        }
        return null;
    }

    private static object? GetPropertyValue(object? src, string? propName)
    {
        if (src == null) return null;
        if (propName == null) return null;

        if (propName.Contains('.')) // complex type, nested
        {
            var temp = propName.Split(['.'], 2);
            var outer = GetPropertyValue(src, temp[0]);
            return GetPropertyValue(outer, temp[1]);
        }
        else
        {
            var prop = src.GetType().GetProperty(propName);
            return prop?.GetValue(src, null);
        }
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

    protected static bool IsClassOrStruct(Type type)
    {
        return type.IsClass || (type.IsValueType && !type.IsPrimitive && !type.IsEnum);
    }

    /// <summary>
    /// Generates a "raw" UCAST -> property names mapping for the type.
    /// Expands down one level, to pick up properties of struct member fields.
    /// </summary>
    /// <returns>The dictionary mapping UCAST field names to property names of the object.</returns>
    protected static Dictionary<string, string> GetUCASTToPropertyNamesMapping()
    {
        var properties = typeof(T).GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
        Dictionary<string, string> result = new(properties.Length);

        foreach (var property in properties)
        {
            var snakeCasedProperty = property.Name.ToSnakeCase();
            result[snakeCasedProperty] = property.Name;

            Type propType = property.PropertyType;
            if (IsClassOrStruct(propType))
            {
                var memberPropertyMapping = GetUCASTToPropertyNamesMapping(propType);
                foreach (var kv in memberPropertyMapping)
                {
                    result[snakeCasedProperty + "." + kv.Key] = property.Name + "." + kv.Value;
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Variant of GetUCASTToPropertyNamesMapping, allowing one to provide a type directly as a parameter.
    /// </summary>
    /// <param name="type">The type to generate UCAST field names for.</param>
    /// <returns>The dictionary mapping UCAST field names to property names of the type.</returns>
    protected static Dictionary<string, string> GetUCASTToPropertyNamesMapping(Type type)
    {
        var properties = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
        Dictionary<string, string> result = new(properties.Length);

        foreach (var property in properties)
        {
            var snakeCasedProperty = property.Name.ToSnakeCase();
            result[snakeCasedProperty] = property.Name;
        }

        return result;
    }
}

/// <summary>
/// Adds extensions to the name mapping logic, specific to Entity Framework Core model classes.
/// </summary>
/// <typeparam name="T">The type to build a name mapping config over.</typeparam>
public class EFCoreMappingConfiguration<T> : MappingConfiguration<T>
{
    protected new Dictionary<string, string> baseNameMappings = GetUCASTToPropertyNamesMapping();

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
    public EFCoreMappingConfiguration(Dictionary<string, string>? namesToProperties = null, string? prefix = null) : base(null, prefix)
    {
        namePrefix = prefix ?? namePrefix;
        foreach (var kv in baseNameMappings)
        {
            var key = $"{namePrefix}.{kv.Key}";
            nameMappings[key] = kv.Value;
            linqMappingsCache[key] = param => Expression.Property(param, kv.Value);
        }
        if (namesToProperties is not null)
        {
            foreach (var mapping in namesToProperties)
            {
                nameMappings[mapping.Key] = nameMappings[mapping.Value];
                // Split string on '.' characters, and build out the Expression.Property() chain accordingly.
                var parts = mapping.Value.Split('.');

                // Build up the linq expression by wrapping each successive member access.
                // Example params when evaluated for "ticket.customer.id": (ParameterExpression, {"ticket"}, {"customer", "id"})
                linqMappingsCache[mapping.Key] = param => NestedPropertyAccessFromParts(param, parts[..1], parts[1..]);
            }
        }
    }

    /// <summary>
    /// Generates a "raw" UCAST -> property names mapping for the type.
    /// This version is updated to handle Navigation types that are common with EF Core.
    /// </summary>
    /// <returns>The dictionary mapping UCAST field names to property names of the object.</returns>
    private static new Dictionary<string, string> GetUCASTToPropertyNamesMapping()
    {
        var originals = MappingConfiguration<T>.GetUCASTToPropertyNamesMapping();
        var result = new Dictionary<string, string>(originals.Count);
        foreach (var kv in originals)
        {
            var key = kv.Key;
            // Strip off the "Navigation" / "_navigation" suffix.
            if (key.Contains("_navigation"))
            {
                key = key.Replace("_navigation", "");
            }
            result[key] = kv.Value;
        }

        return result;
    }
}

