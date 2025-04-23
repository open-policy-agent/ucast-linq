using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Styra.Ucast.Linq;

public static class QueryableExtensions
{
    private static readonly Dictionary<Type, int> NumericTypePrecedence = new()
    {
        { typeof(byte), 1 },
        { typeof(short), 2 },
        { typeof(int), 3 },
        { typeof(long), 4 },
        { typeof(float), 5 },
        { typeof(double), 6 },
        { typeof(decimal), 7 }
    };

    public static bool IsNumericType(Type t)
    {
        return NumericTypePrecedence.TryGetValue(t, out int _);
    }

    public static Type GetHigherPrecedenceNumericType(Type a, Type b)
    {
        if (a == b) { return a; }

        if (NumericTypePrecedence.TryGetValue(a, out int aPrecedence) &&
            NumericTypePrecedence.TryGetValue(b, out int bPrecedence))
        {
            return aPrecedence > bPrecedence ? a : b;
        }

        throw new InvalidOperationException($"Cannot determine precedence between {a} and {b}");
    }

    /// <summary>
    /// Builds a LINQ Lambda Expression from the UCAST tree, and then invokes
    /// it under a LINQ Where expression on some queryable data source.<br />
    /// In our case, this *should* usually be an EF Core ORM model.
    /// </summary>
    /// <param name="source">LINQ data source (same type as <typeparamref name="T"/>).</param>
    /// <param name="root">The top-level UCAST node to build a LINQ Expression tree from.</param>
    /// <param name="mapper">Dictionary mapping UCAST property names to lambdas that generate LINQ Expressions.</param>
    /// <returns>Result, an <see cref="IQueryable{T}" />.</returns>
    public static IQueryable<T> ApplyUCASTFilter<T>(this IQueryable<T> source, UCASTNode root, MappingConfiguration<T> mapper)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var expression = BuildExpression<T>(root, parameter, mapper);
        return source.Where(Expression.Lambda<Func<T, bool>>(expression, parameter));
    }

    /// <summary>
    /// The entry point for recursively constructing a LINQ Expression tree
    /// from a given UCASTNode.
    /// </summary>
    /// <param name="node">Current UCAST node in the conditions tree.</param>
    /// <param name="parameter">LINQ data source (same type as <typeparamref name="T"/>).</param>
    /// <param name="mapper">Dictionary mapping UCAST property names to lambdas that generate LINQ Expressions.</param>
    /// <returns>Result, a LINQ Expression.</returns>
    public static Expression BuildExpression<T>(UCASTNode node, ParameterExpression parameter, MappingConfiguration<T> mapper)
    {
        // Switch expression:
        return node.Type.ToLower() switch
        {
            "field" => BuildFieldExpression<T>(node, parameter, mapper),
            "document" => BuildFieldExpression<T>(node, parameter, mapper), // TODO: Fix this to provide actual document-level operations once we have any.
            "compound" => BuildCompoundExpression<T>(node, parameter, mapper),
            _ => throw new ArgumentException($"Unknown node type: {node.Type}"),
        };
    }

    /// <summary>
    /// Constructs a field-level UCAST condition using LINQ Expressions.<br />
    /// Most operators of interest in UCAST field-level conditionsare represented
    /// as BinaryExpression types.
    /// </summary>
    /// <remarks>
    /// Some typecasts (such as Int32 upcasting to Int64) are detected and
    /// included in the LINQ Expression tree automatically, to ensure that
    /// the binary expressions won't fail at runtime due to type mismatches
    /// between operands.
    /// </remarks>
    /// <typeparam name="T">The LINQ data source type.</typeparam>
    /// <param name="node">Current UCAST node in the conditions tree.</param>
    /// <param name="parameter">LINQ data source (usually same type as <typeparamref name="T"/>).</param>
    /// <param name="mapper">Dictionary mapping UCAST property names to lambdas that generate LINQ Expressions.</param>
    /// <returns>Result, a LINQ Expression (Usually a BinaryExpression).</returns>
    private static Expression BuildFieldExpression<T>(UCASTNode node, ParameterExpression parameter, MappingConfiguration<T> mapper)
    {
        var property = mapper[node.Field!](parameter); // Note: This will throw a KeyNotFoundException if the field name does not exist.
        // We pull out the in/nin operators early, due to different args handling.
        return node.Op.ToLower() switch
        {
            "in" => BuildFieldInExpression<T>(node, property, mapper),
            "nin" => Expression.Not(BuildFieldInExpression<T>(node, property, mapper)),
            _ => BuildFieldExpressionFromProperty<T>(node, property, mapper),
        };
    }

    /// <summary>
    /// The inner logic of <c>BuildFieldExpression</c>.
    /// </summary>
    /// <typeparam name="T">The LINQ</typeparam>
    /// <param name="node">Current UCAST node in the conditions tree.</param>
    /// <param name="property">Derefered property lookup expression on the LINQ data source.</param>
    /// <param name="mapper">Dictionary mapping UCAST property names to lambdas that generate LINQ Expressions.</param>
    /// <returns>Result, a LINQ Expression (Usually a BinaryExpression).</returns>
    /// <exception cref="ArgumentException">Thrown when arguments are of incompatible types.</exception>
    private static Expression BuildFieldExpressionFromProperty<T>(UCASTNode node, Expression property, MappingConfiguration<T> mapper)
    {
        Expression value = Expression.Constant(node.Value);

        Type lhsType = property.Type;
        Type rhsType = value.Type;
        if (lhsType != rhsType)
        {
            if (IsNumericType(lhsType) && IsNumericType(rhsType))
            {
                var exprType = GetHigherPrecedenceNumericType(lhsType, rhsType);

                if (lhsType != exprType)
                {
                    property = Expression.Convert(property, exprType);
                }
                else if (rhsType != exprType)
                {
                    value = Expression.Convert(value, exprType);
                }
            }
        }

        // Switch expression:
        return node.Op.ToLower() switch
        {
            "eq" => Expression.Equal(property, value),
            "ne" => Expression.NotEqual(property, value),
            "gt" => Expression.GreaterThan(property, value),
            "ge" => Expression.GreaterThanOrEqual(property, value),
            "gte" => Expression.GreaterThanOrEqual(property, value),
            "lt" => Expression.LessThan(property, value),
            "le" => Expression.LessThanOrEqual(property, value),
            "lte" => Expression.LessThanOrEqual(property, value),
            _ => throw new ArgumentException($"Unknown operator: {node.Op}"),
        };
    }

    /// <summary>
    /// Constructs a compound LINQ condition, implementing the "in" operator from UCAST.<br />
    /// Builds a collection of field-level equality checks, and then binds them together under an aggregate LINQ expression.
    /// </summary>
    /// <param name="node">Current UCAST node in the conditions tree.</param>
    /// <param name="property">LINQ data source (usually same type as <typeparamref name="T"/>).</param>
    /// <param name="mapper">Dictionary mapping UCAST property names to lambdas that generate LINQ Expressions.</param>
    /// <returns>Result, an aggregate LINQ Expression.</returns>
    private static Expression BuildFieldInExpression<T>(UCASTNode node, Expression property, MappingConfiguration<T> mapper)
    {
        if (node.Value is null)
        {
            throw new NullReferenceException();
        }
        var eq = new UCASTNode("field", "eq", node.Field);
        var childValues = (List<object>)node.Value;


        // Iterate over all children, and determine highest-precedent type among
        // them. Convert LHS value if needed. RHS type conversions will happen
        // automatically during expression building later.
        Type lhsType = property.Type;
        if (IsNumericType(lhsType))
        {
            IEnumerable<Type> childTypes = childValues.Select(x => Expression.Constant(x).Type);
            var highestPrecedentType = lhsType;
            foreach (var childType in childTypes)
            {
                highestPrecedentType = GetHigherPrecedenceNumericType(highestPrecedentType, childType);
            }
            if (lhsType != highestPrecedentType)
            {
                property = Expression.Convert(property, highestPrecedentType);
            }
        }

        IEnumerable<Expression> fieldChecks = new List<Expression>(childValues.Count);
        foreach (var value in childValues)
        {
            eq.Value = value;
            fieldChecks = fieldChecks.Append(BuildFieldExpressionFromProperty(eq, property, mapper));
        }

        return fieldChecks.Aggregate(Expression.OrElse);
    }

    /// <summary>
    /// Constructs a compound UCAST condition.<br />
    /// Recursively constructs its child conditions, then binds the child
    /// nodes together with a LINQ aggregate operation.
    /// </summary>
    /// <param name="node">Current UCAST node in the conditions tree.</param>
    /// <param name="parameter">LINQ data source (same type as <typeparamref name="T"/>).</param>
    /// <param name="mapper">Dictionary mapping UCAST property names to lambdas that generate LINQ Expressions.</param>
    /// <returns>Result, an aggregate LINQ Expression.</returns>
    private static Expression BuildCompoundExpression<T>(UCASTNode node, ParameterExpression parameter, MappingConfiguration<T> mapper)
    {
        if (node.Value is null)
        {
            throw new NullReferenceException();
        }
        // TODO: Detect wrong types, and/or empty child condition lists.
        var childNodes = (List<UCASTNode>)node.Value;
        var childExpressions = childNodes.Select(child => BuildExpression<T>(child, parameter, mapper));

        // Switch expression:
        return node.Op.ToLower() switch
        {
            "and" => childExpressions.Aggregate(Expression.AndAlso),
            "or" => childExpressions.Aggregate(Expression.OrElse),
            _ => throw new ArgumentException($"Unknown compound operator: {node.Op}"),
        };
    }
}
