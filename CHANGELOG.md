# Change Log

All notable changes to this project will be documented in this file. This
project adheres to [Semantic Versioning](http://semver.org/).

## 0.8.0

This release adds initial support for [GUID](https://learn.microsoft.com/en-us/dotnet/api/system.guid) types in data filter queries. It also includes several small dependency bumps, and documentation improvements.

### GUID Support

GUIDs are now supported in for `eq`, `ne`, `in`, and `nin` operators in UCAST.
This is helpful when dealing with database model classes that have a GUID primary key, among other use cases.

Support is implemented by having the LINQ `Expression` tree builder functions detect cases where GUID-typed properties in the base LINQ object are being compared against strings in the UCAST expression tree, and then converting those strings to appropriate GUID values before attempting comparisons.

#### GUID Support Example

Assuming we have a model class with a `Guid` property/field named `Uuid`, the following sequence is roughly how a Rego expression to match a particular GUID would be translated down into LINQ `Expression`s.

Rego expression:
```rego
item.uuid == "123e4567-e89b-12d3-a456-426614174000"
```

Translated to UCAST:
```json
{
  "type": "field",
  "operator": "eq",
  "field": "item.uuid",
  "value": "123e4567-e89b-12d3-a456-426614174000"
}
```

Translated to LINQ `Expressions`:
```csharp
Expression.Equal(
  Expression.Property(/* <ParameterExpression> */, "Uuid"), // Property lookup
  Expression.Constant(new GUID("123e4567-e89b-12d3-a456-426614174000")) // The constant from the policy
)
```


## 0.7.0, 0.7.1

These releases contain release engineering improvements, and no significant code or dependency changes.


## 0.6.0

This release includes additional constructor options for the `UCASTNode` class. Previously, no constructor was defined, which interfered with initialization of its `required` members in derived classes. Now, two options are available: a default parameterless constructor, and a fully-parametrized constructor.


## 0.5.0

This release includes a bugfix for numeric type handling.

Previously, you could not mix numeric types in UCAST expressions beyond a few basic integer types.
We now support mixing and matching integer and floating-point types, and will automatically convert values in expressions to use the larger/higher-precision type as needed.

This means you can now run an altered version of the README example with mixed numeric types, and it will work as expected:

```csharp
int[] numbers = { -1523, 1894, -456, 789, -1002, 345, -1789, 567, 1234, -890, 123, -1456, 1678, -234, 567, -1890, 901, -345, 1567, -789 };
List<SimpleRecord> collection = [.. numbers.Select(n => new SimpleRecord(n))];
var expected = collection.Where(x => x.Value >= 1500 || (x.Value < 400 && (x.Value > 0 || x.Value < -1500))).OrderBy(x => x.Value).ToList();
var conditions = new UCASTNode{
    Type = "compound",
    Op = "or",
    Value = new List<UCASTNode>{
        new() { Type = "field", Op = "ge", Field = "r.value", Value = 1500.0f }, // float
        new() { Type = "compound", Op = "and", Value = new List<UCASTNode>{
            new() { Type = "field", Op = "lt", Field = "r.value", Value = 400L }, // long
            new() { Type = "compound", Op = "or", Value = new List<UCASTNode>{
                new() { Type = "field", Op = "gt", Field = "r.value", Value = 0 }, // int
                new() { Type = "field", Op = "lt", Field = "r.value", Value = -1500.0d }, //double
            } },
        } },
    }
};
var result = collection.AsQueryable().ApplyUCASTFilter(conditions, new MappingConfiguration<SimpleRecord>(prefix: "r")).OrderBy(x => x.Value).ToList();
Assert.Equivalent(expected, result, true);
```


## 0.4.0

This release includes support for using nested objects for describing column masking rules.

Example:
```json
{
  "masks": {
    "table": {
      "column1": {"replace": {"value": 2}},
      "column2": {"replace": {"value": "foo"}},
    }
  }
}
```

The above JSON object describe a set of column masking rules for `table.colum1` and `table.column2`, without needing the strings concatenated in advance with `.` characters between them.


## 0.3.1

This release includes a bugfix for name mapping in the `EFCoreMappingConfiguration` class, ensuring that property names already have the `_navigation` suffix truncated before it attempts to map user-provided UCAST field names to object properties.


## 0.3.0

This release includes a complete rework of how name mapping and LINQ property lookups are generated, fixing several pernicious bugs around name mapping.


## 0.2.0

This release includes numerous small cleanups and refactors, and adds initial support for column masking of datasets!

### Column masking

To mask a particular field out of a collection, there is a new `EnumerableExtensions.MaskElements<T>` method, which takes a collection, a Dictionary of column masking rules, and a `MappingConfiguration`, and produces a shallow-cloned copy of the collection where every object's fields have been masked according to the masking rules.

### Breaking changes

The `Dictionary<string, Func<ParameterExpression, Expression>>` type used for LINQ mappings has been switched out for a more powerful `MappingConfiguration` class, which allows reusing the name mappings for both data filtering, and column masking.


## 0.1.0

This release is an release engineering tests, aimed at sorting out automated publishing of a Github Release and NuGet package.
