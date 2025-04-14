# Change Log

All notable changes to this project will be documented in this file. This
project adheres to [Semantic Versioning](http://semver.org/).

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
