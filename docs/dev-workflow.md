# General Development Workflow

The process can coarsely be split between OPA and C# sides. In practice, there will be some co-design where a dev may want a certain field in the model to map to a certain name in the Rego policy, and vice versa. Below is the rough process most users will find useful for spinning up a C# application that uses this library.

## OPA side

 1. The user writes a [data filtering policy][data-filters] in Rego. ([Docs on writing data filter policies][writing-data-filters])
    - These policies might be unlike others you've written before in Rego.
      This is because they are not evaluated to generate decisions (as is typical for Rego rules).
      Instead, these policies are *partially evaluated*, and the resulting simplified rules are compiled down into [Universal Conditions AST (UCAST)][ucast] expression trees.
 1. The user loads the policy into an OPA server instance.

### Mapping names between LINQ and Rego

It is important that the data filtering policy is written with correct names for model properties that will be used on the C# side.
By default, when mapping an object's properties to Rego refs, the mapper assumes `snake_case` for everything.

Example:
```csharp
// Assuming we map this class to have the prefix "hydro"
// Ex: MappingConfiguration<HydrologyData>(prefix: "hydro");
public class HydrologyData
{
    public int Id { get; set; } // => hydro.id
    public Guid Uuid { get; set; } // => hydro.uuid
    public string? Name { get; set; } // => hydro.name
    public DateTime LastUpdated { get; set; } // => hydro.last_updated
    public bool FloodStage { get; set; } // => hydro.flood_stage
    public double WaterLevelMeters { get; set; } // => hydro.water_level_meters
    public double? FlowRateMinute { get; set; } // => hydro.flow_rate_minute
}
```

Note: The above mapping assumes fields with identical spelling except for capitalization are **not** present in the model.
If you have such a case, you will want to look into using the `namesToProperties` field in the `MappingConfiguration` constructor to control how those properties are mapped.

## C# side

The process is: building the field mapping, querying OPA for a UCAST value, and then plugging in the UCAST at the LINQ query site.
The steps for doing those things looks like:

 1. This library's `MappingConfiguration<T>` type is used to create a mapping from properties of the model object type to field references in the UCAST sent back from OPA. This allows a Rego policy to refer to specific model fields by name. (e.g. `hydro.water_level_meters`)
 1. The C# program then makes an HTTP request to the OPA server's [Compile API][opa-compile-api] endpoint, to get the [UCAST][ucast] expression tree that describes the data filters.
 1. This library's `QueryableExtensions.ApplyUCASTFilter()` LINQ operator can then be used to build a LINQ [`Expression`][csharp-linq-expression] tree as part of a larger LINQ query, using the `UCASTNode` hierarchy, and the `MappingConfiguration` from earlier.
 1. LINQ then handles JIT compiling the LINQ query into whatever format makes sense for the underlying data source. (e.g. SQL queries for an EF Core model query.)

   [data-filters]: https://www.openpolicyagent.org/docs/filtering
   [writing-data-filters]: https://www.openpolicyagent.org/docs/filtering/fragment
   [opa-compile-api]: https://www.openpolicyagent.org/docs/rest-api#compile-api
   [ucast]: https://www.openpolicyagent.org/docs/filtering/ucast-syntax
   [csharp-linq-expression]: https://learn.microsoft.com/en-us/dotnet/api/system.linq.expressions.expression