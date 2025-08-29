using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OpenPolicyAgent.Ucast.Linq;

public static class EnumerableExtension
{
    /// <summary>
    /// Allows shallow cloning an object without requiring serializing and
    /// deserializing the object.
    /// </summary>
    /// <typeparam name="T">The type of the source object.</typeparam>
    /// <param name="source">The object to clone.</param>
    /// <returns>A shallow clone of the source object.</returns>
    /// <exception cref="ArgumentException"></exception>
    private static T ShallowClone<T>(T source)
    {
        if (source == null) return default!; // Don't care, currently.

        Type type = typeof(T);

        // Handle primitive types and strings.
        if (type.IsPrimitive || type == typeof(string))
        {
            return source;
        }

        // Handle reference types.
        if (type.IsClass)
        {
            object clone = Activator.CreateInstance(type)!; // Should never be null by this point.
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (FieldInfo field in fields)
            {
                field.SetValue(clone, field.GetValue(source));
            }

            return (T)clone;
        }

        // Handle value types (structs).
        if (type.IsValueType)
        {
            return source;
        }

        throw new ArgumentException($"Unable to clone type {type.Name}");
    }

    /// <summary>
    /// Mask fields on a single object. This is how individual objects are masked in a collection.
    /// </summary>
    /// <typeparam name="T">The type of the source object.</typeparam>
    /// <param name="source">The object whose fields will be masked.</param>
    /// <param name="maskingRules">A dictionary mapping UCAST field names to column masking functions.</param>
    /// <param name="config">A name mapping config, allowing easy translation of UCAST field names to object fields.</param>
    /// <returns>A shallow clone of the source object, with all masks applied.</returns>
    private static T MaskElement<T>(this T source, Dictionary<string, MaskingFunc> maskingRules, MappingConfiguration<T> config)
    {
        T result = ShallowClone(source);
        foreach (var kv in maskingRules)
        {
            var name = kv.Key;
            var maskingFunc = kv.Value;
            // Future: Plug this value into masking functions that use the original column's value, (e.g. hash functions).
            // var currentValue = config.GetPropertyByName(name, result);
            if (maskingFunc.Replace is not null)
            {
                config.SetPropertyByName(name, ref result, maskingFunc.Replace?.Value);
            }
        }
        return result;
    }

    /// <summary>
    /// Masks fields on every element in a collection, using a Dictionary of masks, and a name mapping.
    /// </summary>
    /// <typeparam name="T">The type of the source collection's elements.</typeparam>
    /// <param name="source">The collection whose elements' fields will be masked.</param>
    /// <param name="maskingRules">A dictionary mapping UCAST field names to column masking functions.</param>
    /// <param name="config">A name mapping config, allowing easy translation of UCAST field names to object fields.</param>
    /// <returns>A new collection, containing shallow clones of the source collection's object, with all masks applied to each object.</returns>
    public static IEnumerable<T> MaskElements<T>(this IEnumerable<T> source, Dictionary<string, MaskingFunc>? maskingRules, MappingConfiguration<T> config)
    {
        if (maskingRules is null)
        {
            return source;
        }
        IEnumerable<T> result = source.Select(x => x.MaskElement(maskingRules, config));
        return result;
    }

    /// <summary>
    /// Masks fields on every element in a collection, using a nested Dictionary of masks, and a name mapping.
    /// </summary>
    /// <typeparam name="T">The type of the source collection's elements.</typeparam>
    /// <param name="source">The collection whose elements' fields will be masked.</param>
    /// <param name="maskingRules">A dictionary mapping UCAST field names to column masking functions.</param>
    /// <param name="config">A name mapping config, allowing easy translation of UCAST field names to object fields.</param>
    /// <returns>A new collection, containing shallow clones of the source collection's object, with all masks applied to each object.</returns>
    public static IEnumerable<T> MaskElements<T>(this IEnumerable<T> source, Dictionary<string, Dictionary<string, MaskingFunc>>? maskingRules, MappingConfiguration<T> config)
    {
        if (maskingRules is null)
        {
            return source;
        }
        // Prealloc the flattened masking rules dictionary.
        var flattenedRules = new Dictionary<string, MaskingFunc>(maskingRules.Sum(kv => kv.Value.Count));

        foreach (var tableKV in maskingRules)
        {
            foreach (var columnKV in tableKV.Value)
            {
                flattenedRules[tableKV.Key + "." + columnKV.Key] = columnKV.Value;
            }
        }

        IEnumerable<T> result = source.Select(x => x.MaskElement(flattenedRules, config));
        return result;
    }
}
