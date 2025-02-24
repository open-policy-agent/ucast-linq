// This file contains basic unit tests for the library.
namespace Styra.Ucast.Linq.Tests;

// Field operations, across all JSON primitive types (null, bool, int, double, string), with expected results from equivalent LINQ queries.
public class UnitTestFieldExprs
{
    private static readonly List<UnitTestDataSource.HydrologyData> testdata = UnitTestDataSource.GetTestHydrologyData();

    [Theory]
    [MemberData(nameof(EqTestData))]
    public void TestEq(UCASTNode node, List<UnitTestDataSource.HydrologyData> expected)
    {
        var result = testdata.AsQueryable().ApplyUCASTFilter(node, UnitTestDataSource.HydrologyDataMapping).ToList();
        Assert.Equivalent(expected, result, true);
    }

    [Theory]
    [MemberData(nameof(NeTestData))]
    public void TestNe(UCASTNode node, List<UnitTestDataSource.HydrologyData> expected)
    {
        var result = testdata.AsQueryable().ApplyUCASTFilter(node, UnitTestDataSource.HydrologyDataMapping).ToList();
        Assert.Equivalent(expected, result, true);
    }

    [Theory]
    [MemberData(nameof(GtTestData))]
    public void TestGt(UCASTNode node, List<UnitTestDataSource.HydrologyData> expected)
    {
        var result = testdata.AsQueryable().ApplyUCASTFilter(node, UnitTestDataSource.HydrologyDataMapping).ToList();
        Assert.Equivalent(expected, result, true);
    }

    [Theory]
    [MemberData(nameof(GeTestData))]
    public void TestGe(UCASTNode node, List<UnitTestDataSource.HydrologyData> expected)
    {
        var result = testdata.AsQueryable().ApplyUCASTFilter(node, UnitTestDataSource.HydrologyDataMapping).ToList();
        Assert.Equivalent(expected, result, true);
    }

    [Theory]
    [MemberData(nameof(LtTestData))]
    public void TestLt(UCASTNode node, List<UnitTestDataSource.HydrologyData> expected)
    {
        var result = testdata.AsQueryable().ApplyUCASTFilter(node, UnitTestDataSource.HydrologyDataMapping).ToList();
        Assert.Equivalent(expected, result, true);
    }

    [Theory]
    [MemberData(nameof(LeTestData))]
    public void TestLe(UCASTNode node, List<UnitTestDataSource.HydrologyData> expected)
    {
        var result = testdata.AsQueryable().ApplyUCASTFilter(node, UnitTestDataSource.HydrologyDataMapping).ToList();
        Assert.Equivalent(expected, result, true);
    }

    [Theory]
    [MemberData(nameof(InTestData))]
    public void TestIn(UCASTNode node, List<UnitTestDataSource.HydrologyData> expected)
    {
        var result = testdata.AsQueryable().ApplyUCASTFilter(node, UnitTestDataSource.HydrologyDataMapping).ToList();
        Assert.Equivalent(expected, result, true);
    }

    [Theory]
    [MemberData(nameof(NinTestData))]
    public void TestNin(UCASTNode node, List<UnitTestDataSource.HydrologyData> expected)
    {
        var result = testdata.AsQueryable().ApplyUCASTFilter(node, UnitTestDataSource.HydrologyDataMapping).ToList();
        Assert.Equivalent(result, expected);
    }

    public static IEnumerable<object[]> EqTestData()
    {
        yield return new object[] { new UCASTNode { Type = "field", Op = "eq", Field = "data.name", Value = null }, testdata.Where(d => d.Name == null).ToList() };
        yield return new object[] { new UCASTNode { Type = "field", Op = "eq", Field = "data.name", Value = "Lake Beta" }, testdata.Where(d => d.Name == "Lake Beta").ToList() };
        yield return new object[] { new UCASTNode { Type = "field", Op = "eq", Field = "data.id", Value = 2 }, testdata.Where(d => d.Id == 2).ToList() };
        yield return new object[] { new UCASTNode { Type = "field", Op = "eq", Field = "data.id", Value = (long)2 }, testdata.Where(d => d.Id == 2).ToList() };
        yield return new object[] { new UCASTNode { Type = "field", Op = "eq", Field = "data.flood_stage", Value = true }, testdata.Where(d => d.FloodStage).ToList() };
        yield return new object[] { new UCASTNode { Type = "field", Op = "eq", Field = "data.water_level_meters", Value = 5.8 }, testdata.Where(d => d.WaterLevelMeters == 5.8).ToList() };
    }

    public static IEnumerable<object[]> NeTestData()
    {
        yield return new object[] { new UCASTNode { Type = "field", Op = "ne", Field = "data.name", Value = null }, testdata.Where(d => d.Name != null).ToList() };
        yield return new object[] { new UCASTNode { Type = "field", Op = "ne", Field = "data.name", Value = "Lake Beta" }, testdata.Where(d => d.Name != "Lake Beta").ToList() };
        yield return new object[] { new UCASTNode { Type = "field", Op = "ne", Field = "data.id", Value = 2 }, testdata.Where(d => d.Id != 2).ToList() };
        yield return new object[] { new UCASTNode { Type = "field", Op = "ne", Field = "data.id", Value = (long)2 }, testdata.Where(d => d.Id != 2).ToList() };
        yield return new object[] { new UCASTNode { Type = "field", Op = "ne", Field = "data.flood_stage", Value = true }, testdata.Where(d => !d.FloodStage).ToList() };
        yield return new object[] { new UCASTNode { Type = "field", Op = "ne", Field = "data.water_level_meters", Value = 5.8 }, testdata.Where(d => d.WaterLevelMeters != 5.8).ToList() };
    }

    public static IEnumerable<object[]> GtTestData()
    {
        yield return new object[] { new UCASTNode { Type = "field", Op = "gt", Field = "data.id", Value = 2 }, testdata.Where(d => d.Id > 2).ToList() };
        yield return new object[] { new UCASTNode { Type = "field", Op = "gt", Field = "data.id", Value = (long)2 }, testdata.Where(d => d.Id > 2).ToList() };
        yield return new object[] { new UCASTNode { Type = "field", Op = "gt", Field = "data.water_level_meters", Value = 5.8 }, testdata.Where(d => d.WaterLevelMeters > 5.8).ToList() };
    }

    public static IEnumerable<object[]> GeTestData()
    {
        yield return new object[] { new UCASTNode { Type = "field", Op = "ge", Field = "data.id", Value = 2 }, testdata.Where(d => d.Id >= 2).ToList() };
        yield return new object[] { new UCASTNode { Type = "field", Op = "ge", Field = "data.id", Value = (long)2 }, testdata.Where(d => d.Id >= 2).ToList() };
        yield return new object[] { new UCASTNode { Type = "field", Op = "ge", Field = "data.water_level_meters", Value = 5.8 }, testdata.Where(d => d.WaterLevelMeters >= 5.8).ToList() };
    }

    public static IEnumerable<object[]> LtTestData()
    {
        yield return new object[] { new UCASTNode { Type = "field", Op = "lt", Field = "data.id", Value = 2 }, testdata.Where(d => d.Id < 2).ToList() };
        yield return new object[] { new UCASTNode { Type = "field", Op = "lt", Field = "data.id", Value = (long)2 }, testdata.Where(d => d.Id < 2).ToList() };
        yield return new object[] { new UCASTNode { Type = "field", Op = "lt", Field = "data.water_level_meters", Value = 5.8 }, testdata.Where(d => d.WaterLevelMeters < 5.8).ToList() };
    }

    public static IEnumerable<object[]> LeTestData()
    {
        yield return new object[] { new UCASTNode { Type = "field", Op = "le", Field = "data.id", Value = 2 }, testdata.Where(d => d.Id <= 2).ToList() };
        yield return new object[] { new UCASTNode { Type = "field", Op = "le", Field = "data.id", Value = (long)2 }, testdata.Where(d => d.Id <= 2).ToList() };
        yield return new object[] { new UCASTNode { Type = "field", Op = "le", Field = "data.water_level_meters", Value = 5.8 }, testdata.Where(d => d.WaterLevelMeters <= 5.8).ToList() };
    }

    public static IEnumerable<object[]> InTestData()
    {
        {
            yield return new object[] { new UCASTNode { Type = "field", Op = "in", Field = "data.name", Value = new List<object>() { null! } }, testdata.Where(d => new List<object>() { null! }.Contains(d.Name!)).ToList() };
            yield return new object[] { new UCASTNode { Type = "field", Op = "in", Field = "data.name", Value = new List<object>() { "River Alpha", "Lake Beta" } }, testdata.Where(d => new List<object> { "River Alpha", "Lake Beta" }.Contains(d.Name!)).ToList() };
            yield return new object[] { new UCASTNode { Type = "field", Op = "in", Field = "data.id", Value = new List<object>() { 2, 5 } }, testdata.Where(d => new List<object>() { 2, 5 }.Contains(d.Id)).ToList() };
            yield return new object[] { new UCASTNode { Type = "field", Op = "in", Field = "data.id", Value = new List<object>() { (long)2, (long)5 } }, testdata.Where(d => new List<object>() { (long)2, (long)5 }.Contains((long)d.Id)).ToList() };
            yield return new object[] { new UCASTNode { Type = "field", Op = "in", Field = "data.flood_stage", Value = new List<object>() { true } }, testdata.Where(d => new List<object>() { true }.Contains(d.FloodStage)).ToList() };
            yield return new object[] { new UCASTNode { Type = "field", Op = "in", Field = "data.water_level_meters", Value = new List<object>() { 2.5, 5.8 } }, testdata.Where(d => new List<object>() { 2.5, 5.8 }.Contains(d.WaterLevelMeters)).ToList() };
        }
    }

    public static IEnumerable<object[]> NinTestData()
    {
        {
            yield return new object[] { new UCASTNode { Type = "field", Op = "nin", Field = "data.name", Value = new List<object>() { null! } }, testdata.Where(d => !new List<object>() { null! }.Contains(d.Name!)).ToList() };
            yield return new object[] { new UCASTNode { Type = "field", Op = "nin", Field = "data.name", Value = new List<object>() { "River Alpha", "Lake Beta" } }, testdata.Where(d => !new List<object> { "River Alpha", "Lake Beta" }.Contains(d.Name!)).ToList() };
            yield return new object[] { new UCASTNode { Type = "field", Op = "nin", Field = "data.id", Value = new List<object>() { 2, 5 } }, testdata.Where(d => !new List<object>() { 2, 5 }.Contains(d.Id)).ToList() };
            yield return new object[] { new UCASTNode { Type = "field", Op = "nin", Field = "data.id", Value = new List<object>() { (long)2, (long)5 } }, testdata.Where(d => !new List<object>() { (long)2, (long)5 }.Contains((long)d.Id)).ToList() };
            yield return new object[] { new UCASTNode { Type = "field", Op = "nin", Field = "data.flood_stage", Value = new List<object>() { true } }, testdata.Where(d => !new List<object>() { true }.Contains(d.FloodStage)).ToList() };
            yield return new object[] { new UCASTNode { Type = "field", Op = "nin", Field = "data.water_level_meters", Value = new List<object>() { 2.5, 5.8 } }, testdata.Where(d => !new List<object>() { 2.5, 5.8 }.Contains(d.WaterLevelMeters)).ToList() };
        }
    }
}

// Compound operations, with expected results from equivalent LINQ queries.
public class UnitTestCompoundExprs
{
    private static readonly List<UnitTestDataSource.HydrologyData> testdata = UnitTestDataSource.GetTestHydrologyData();

    [Theory]
    [MemberData(nameof(AndTestData))]
    public void TestAnd(UCASTNode node, List<UnitTestDataSource.HydrologyData> expected)
    {
        var result = testdata.AsQueryable().ApplyUCASTFilter(node, UnitTestDataSource.HydrologyDataMapping).ToList();
        Assert.Equivalent(expected, result, true);
    }

    [Theory]
    [MemberData(nameof(OrTestData))]
    public void TestOr(UCASTNode node, List<UnitTestDataSource.HydrologyData> expected)
    {
        var result = testdata.AsQueryable().ApplyUCASTFilter(node, UnitTestDataSource.HydrologyDataMapping).ToList();
        Assert.Equivalent(expected, result, true);
    }

    [Theory]
    [MemberData(nameof(AndNestedTestData))]
    public void TestAndNested(UCASTNode node, List<UnitTestDataSource.HydrologyData> expected)
    {
        var result = testdata.AsQueryable().ApplyUCASTFilter(node, UnitTestDataSource.HydrologyDataMapping).ToList();
        Assert.Equivalent(expected, result, true);
    }

    [Theory]
    [MemberData(nameof(OrNestedTestData))]
    public void TestOrNested(UCASTNode node, List<UnitTestDataSource.HydrologyData> expected)
    {
        var result = testdata.AsQueryable().ApplyUCASTFilter(node, UnitTestDataSource.HydrologyDataMapping).ToList();
        Assert.Equivalent(expected, result, true);
    }

    public static IEnumerable<object[]> AndTestData()
    {
        yield return new object[] { new UCASTNode { Type = "compound", Op = "and", Value = new List<UCASTNode>{
            new() { Type = "field", Op = "eq", Field = "data.name", Value = "Lake Beta" },
            new() { Type = "field", Op = "eq", Field = "data.flood_stage", Value = true },
        } }, testdata.Where(d => d.Name == "Lake Beta" && d.FloodStage).ToList() };
    }

    public static IEnumerable<object[]> OrTestData()
    {
        yield return new object[] { new UCASTNode { Type = "compound", Op = "or", Value = new List<UCASTNode>{
            new() { Type = "field", Op = "eq", Field = "data.name", Value = "Lake Beta" },
            new() { Type = "field", Op = "eq", Field = "data.flood_stage", Value = false },
        } }, testdata.Where(d => d.Name == "Lake Beta" || !d.FloodStage).ToList() };
    }

    public static IEnumerable<object[]> AndNestedTestData()
    {
        yield return new object[] { new UCASTNode { Type = "compound", Op = "and", Value = new List<UCASTNode>{
            new() { Type = "field", Op = "eq", Field = "data.id", Value = 2 },
            new() { Type = "compound", Op = "or", Value = new List<UCASTNode>{
                new() { Type = "field", Op = "ge", Field = "data.water_level_meters", Value = 2.5 },
                new() { Type = "field", Op = "eq", Field = "data.flood_stage", Value = false },
            } },

        } }, testdata.Where(d => d.Id == 2 && (d.WaterLevelMeters >= 2.5 || !d.FloodStage)).ToList() };
    }

    public static IEnumerable<object[]> OrNestedTestData()
    {
        yield return new object[] { new UCASTNode { Type = "compound", Op = "or", Value = new List<UCASTNode>{
            new() { Type = "field", Op = "eq", Field = "data.id", Value = 2 },
            new() { Type = "compound", Op = "and", Value = new List<UCASTNode>{
                new() { Type = "field", Op = "ge", Field = "data.water_level_meters", Value = 2.5 },
                new() { Type = "field", Op = "eq", Field = "data.flood_stage", Value = false },
            } },

        } }, testdata.Where(d => d.Id == 2 || (d.WaterLevelMeters >= 2.5 && !d.FloodStage)).ToList() };
    }
}

public class UnitTestREADMEExample
{
    public record SimpleRecord(int Value);

    [Fact]
    public void TestREADMEExample()
    {
        int[] numbers = { -1523, 1894, -456, 789, -1002, 345, -1789, 567, 1234, -890, 123, -1456, 1678, -234, 567, -1890, 901, -345, 1567, -789 };
        List<SimpleRecord> collection = [.. numbers.Select(n => new SimpleRecord(n))];
        var expected = collection.Where(x => x.Value >= 1500 || (x.Value < 400 && (x.Value > 0 || x.Value < -1500))).OrderBy(x => x.Value).ToList();
        var conditions = new UCASTNode
        {
            Type = "compound",
            Op = "or",
            Value = new List<UCASTNode>{
            new() { Type = "field", Op = "ge", Field = "r.value", Value = 1500 },
            new() { Type = "compound", Op = "and", Value = new List<UCASTNode>{
                new() { Type = "field", Op = "lt", Field = "r.value", Value = 400 },
                new() { Type = "compound", Op = "or", Value = new List<UCASTNode>{
                    new() { Type = "field", Op = "gt", Field = "r.value", Value = 0 },
                    new() { Type = "field", Op = "lt", Field = "r.value", Value = -1500 },
                } },
            } },
        }
        };
        var result = collection.AsQueryable().ApplyUCASTFilter(conditions, new MappingConfiguration<SimpleRecord>(prefix: "r")).OrderBy(x => x.Value).ToList();
        Assert.Equivalent(expected, result, true);
    }
}

public class UnitTestMasking
{
    private static List<UnitTestDataSource.HydrologyData> testdata = UnitTestDataSource.GetTestHydrologyData();

    [Fact]
    public void TestMaskingReplace()
    {
        Dictionary<string, MaskingFunc> maskingRules = new()
        {
            { "data.name", new MaskingFunc() { Replace = new() { Value = "***" } } },
        };

        var maskedList = testdata.MaskElements(maskingRules, UnitTestDataSource.HydrologyDataMapping);
        Assert.All(maskedList, item => Assert.Equal("***", item.Name));
    }
}

// AI-generated, used to provide a dataset for LINQ queries.
public class UnitTestDataSource
{
    public static readonly MappingConfiguration<HydrologyData> HydrologyDataMapping = new(prefix: "data");

    public class HydrologyData
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool FloodStage { get; set; }
        public double WaterLevelMeters { get; set; }
        public double? FlowRateMinute { get; set; }
    }

    public static List<HydrologyData> GetTestHydrologyData()
    {
        return [
            new HydrologyData { Id = 1, Name = "River Alpha", LastUpdated = new DateTime(2024, 12, 10, 8, 30, 0), FloodStage = false, WaterLevelMeters = 2.5, FlowRateMinute = 100.5 },
            new HydrologyData { Id = 2, Name = "Lake Beta", LastUpdated = new DateTime(2024, 12, 9, 15, 45, 0), FloodStage = true, WaterLevelMeters = 5.8, FlowRateMinute = null },
            new HydrologyData { Id = 3, Name = "Stream Gamma", LastUpdated = new DateTime(2024, 12, 8, 12, 0, 0), FloodStage = false, WaterLevelMeters = 0.75, FlowRateMinute = 25.3 },
            new HydrologyData { Id = 4, Name = "Reservoir Delta", LastUpdated = new DateTime(2024, 12, 7, 9, 15, 0), FloodStage = false, WaterLevelMeters = 15.2, FlowRateMinute = 500.0 },
            new HydrologyData { Id = 5, Name = null, LastUpdated = new DateTime(2024, 12, 6, 18, 30, 0), FloodStage = true, WaterLevelMeters = 3.1, FlowRateMinute = 75.8 }
        ];
    }
}


