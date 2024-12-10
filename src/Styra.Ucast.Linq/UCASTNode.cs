using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Styra.Ucast.Linq;

/// <summary>
/// The UCASTNode class is used to represent UCAST conditions trees.<br />
/// Nodes come in three types:
///   <list type="bullet">
///    <item>Field-level conditions (Supported operators: <c>eq</c>, <c>ne</c>, <c>gt</c>, <c>ge</c>, <c>lt</c>, <c>le</c>, <c>in</c>, <c>nin</c>)</item>
///    <item>Document-level conditions (Supported operators: None)</item>
///    <item>Compound conditions (Supported operators: <c>and</c>, <c>or</c>)</item>
///   </list>
/// Unsupported operators or nonexistent field names will cause exceptions
/// to be thrown at query evaluation time.
/// </summary>
public class UCASTNode
{
    /// <summary>
    /// The type of UCAST condition.<br />
    /// Must be one of: <c>"field"</c>, <c>"document"</c>, <c>"compound"</c>
    /// </summary>
    [JsonProperty("type")]
    public required string Type;

    /// <summary>
    /// The operator this node is applying.<br />
    /// Operators by node type:
    ///   <list type="bullet">
    ///    <item>Field-level conditions: <c>"eq"</c>, <c>"ne"</c>, <c>"gt"</c>, <c>"ge"</c>, <c>"lt"</c>, <c>"le"</c>, <c>"in"</c>, <c>"nin"</c></item>
    ///    <item>Document-level conditions: None</item>
    ///    <item>Compound conditions <c>"and"</c>, <c>"or"</c></item>
    ///   </list>
    /// </summary>
    [JsonProperty("operator")]
    public required string Op;

    /// <summary>
    /// <para>The field in the record type to look up.</para>
    /// <example>
    /// Example:
    /// <code>
    /// // Declare a record or class type:
    /// public record Example(int Value);
    ///
    /// // We can later look up a specific value for the field:
    /// var condition = new UCASTNode { Type = "field", Op = "eq", Field = "example.value", Value = 5 };
    /// </code>
    /// </example>
    /// </summary>
    [JsonProperty("field")]
    public string? Field;

    /// <summary>
    /// The Value field can be one of 3x cases:
    ///   <list type="bullet">
    ///    <item>Field-level condition, primitive type: Used for the <c>"eq"</c>, <c>"ne"</c>, <c>"gt"</c>, <c>"ge"</c>, <c>"lt"</c>, <c>"le"</c> operators.</item>
    ///    <item>Field-level condition, array of primitive types: Used for the <c>"in"</c>, <c>"nin"</c> operators.</item>
    ///    <item>Compound condition, array of <c>UCASTNode</c>: Used to represent child conditions for <c>"and"</c>, <c>"or"</c> operators.</item>
    ///   </list>
    /// </summary>
    [JsonProperty("value")]
    [JsonConverter(typeof(UCASTNodeValueConverter))]
    public object? Value; // Either another string, or a List<UCASTNode>.
}

public class UCASTNodeValueConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(object);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        JToken token = JToken.Load(reader);
        if (token.Type == JTokenType.Array)
        {
            JArray array = (JArray)token;
            if (array.Children().All(item => item.Type == JTokenType.Object))
            {
                return array.ToObject<List<UCASTNode>>()!;
            }
        }

        return token.ToObject<object>()!;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}
