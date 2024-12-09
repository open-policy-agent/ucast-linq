# Styra UCAST integration for LINQ

[![License](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)
[![NuGet Version](https://img.shields.io/nuget/v/Styra.Ucast.Linq?style=flat&color=%2324b6e0)](https://www.nuget.org/packages/Styra.Ucast.Linq/)

> [!IMPORTANT]
> The reference documentation for this library is available at https://styrainc.github.io/ucast-linq


## Installation

### Nuget

```bash
dotnet add package Styra.Ucast.Linq
```


## Example Usage

Let's assume that we have a collection of random integers, and wish to filter them with a LINQ query using multiple criteria:
```csharp
using System;
using System.Linq;

public record SimpleRecord(int Value);

var numbers = new int[] { -1523, 1894, -456, 789, -1002, 345, -1789, 567, 1234, -890, 123, -1456, 1678, -234, 567, -1890, 901, -345, 1567, -789 };
var collection = numbers.Select(n => new SimpleRecord(n)).ToList();
var results = collection.Where(x => x.Value >= 1500 || (x.Value < 400 && (x.Value > 0 || x.Value < -1500)))
                        .OrderBy(x => x.Value)
                        .ToList();

Console.WriteLine(string.Join("\n", results));
```

<details>

<summary>Output</summary>

```csharp
SimpleRecord { Value = -1890 }
SimpleRecord { Value = -1789 }
SimpleRecord { Value = -1523 }
SimpleRecord { Value = 123 }
SimpleRecord { Value = 345 }
SimpleRecord { Value = 1567 }
SimpleRecord { Value = 1678 }
SimpleRecord { Value = 1894 }
```

</details>

Using this library, the same filters can be constructed dynamically using UCAST expressions (which can be deserialized from JSON):
```csharp
using System;
using System.Linq;
using Styra.Ucast.Linq;

public record SimpleRecord(int Value);

var conditions = new UCASTNode { Type = "compound", Op = "or", Value = new List<UCASTNode>{
    new UCASTNode { Type = "field", Op = "ge", Field = "r.value", Value = 1500 },
    new UCASTNode { Type = "compound", Op = "and", Value = new List<UCASTNode>{
        new UCASTNode { Type = "field", Op = "lt", Field = "r.value", Value = 400 },
        new UCASTNode { Type = "compound", Op = "or", Value = new List<UCASTNode>{
            new UCASTNode { Type = "field", Op = "gt", Field = "r.value", Value = 0 },
            new UCASTNode { Type = "field", Op = "lt", Field = "r.value", Value = -1500 },
        } },
    } },
} };

var numbers = new int[] { -1523, 1894, -456, 789, -1002, 345, -1789, 567, 1234, -890, 123, -1456, 1678, -234, 567, -1890, 901, -345, 1567, -789 };
var collection = numbers.Select(n => new SimpleRecord(n)).ToList();
var results = collection.AsQueryable()
                        .ApplyUCASTFilter(conditions, QueryableExtensions.BuildDefaultMapperDictionary<SimpleRecord>("r"))
                        .OrderBy(x => x.Value)
                        .ToList();

Console.WriteLine(string.Join("\n", results));
```

<details>

<summary>Output</summary>

```csharp
SimpleRecord { Value = -1890 }
SimpleRecord { Value = -1789 }
SimpleRecord { Value = -1523 }
SimpleRecord { Value = 123 }
SimpleRecord { Value = 345 }
SimpleRecord { Value = 1567 }
SimpleRecord { Value = 1678 }
SimpleRecord { Value = 1894 }
```

</details>


## Community

For questions, discussions and announcements related to Styra products, services and open source projects, please join
the Styra community on [Slack](https://communityinviter.com/apps/styracommunity/signup)!
