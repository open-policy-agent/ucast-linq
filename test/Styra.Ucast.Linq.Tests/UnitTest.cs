// This file contains basic unit tests for the library.
namespace Styra.Ucast.Linq.Tests;

// Field operations, across all JSON primitive types (null, bool, int, double, string), with expected results from equivalent LINQ queries.
public class UnitTestFieldExprs
{
    private static readonly List<UnitTestDataSource.HydrologyData> testdata = UnitTestDataSource.GetTestHydrologyData();
    private static readonly MappingConfiguration<UnitTestDataSource.HydrologyData> mapping = new(prefix: "data");

    [Theory]
    [MemberData(nameof(EqTestData))]
    public void TestEq(UCASTNode node, List<UnitTestDataSource.HydrologyData> expected)
    {
        var result = testdata.AsQueryable().ApplyUCASTFilter(node, mapping).ToList();
        Assert.Equivalent(expected, result, true);
    }

    [Theory]
    [MemberData(nameof(NeTestData))]
    public void TestNe(UCASTNode node, List<UnitTestDataSource.HydrologyData> expected)
    {
        var result = testdata.AsQueryable().ApplyUCASTFilter(node, mapping).ToList();
        Assert.Equivalent(expected, result, true);
    }

    [Theory]
    [MemberData(nameof(GtTestData))]
    public void TestGt(UCASTNode node, List<UnitTestDataSource.HydrologyData> expected)
    {
        var result = testdata.AsQueryable().ApplyUCASTFilter(node, mapping).ToList();
        Assert.Equivalent(expected, result, true);
    }

    [Theory]
    [MemberData(nameof(GeTestData))]
    public void TestGe(UCASTNode node, List<UnitTestDataSource.HydrologyData> expected)
    {
        var result = testdata.AsQueryable().ApplyUCASTFilter(node, mapping).ToList();
        Assert.Equivalent(expected, result, true);
    }

    [Theory]
    [MemberData(nameof(LtTestData))]
    public void TestLt(UCASTNode node, List<UnitTestDataSource.HydrologyData> expected)
    {
        var result = testdata.AsQueryable().ApplyUCASTFilter(node, mapping).ToList();
        Assert.Equivalent(expected, result, true);
    }

    [Theory]
    [MemberData(nameof(LeTestData))]
    public void TestLe(UCASTNode node, List<UnitTestDataSource.HydrologyData> expected)
    {
        var result = testdata.AsQueryable().ApplyUCASTFilter(node, mapping).ToList();
        Assert.Equivalent(expected, result, true);
    }

    [Theory]
    [MemberData(nameof(InTestData))]
    public void TestIn(UCASTNode node, List<UnitTestDataSource.HydrologyData> expected)
    {
        var result = testdata.AsQueryable().ApplyUCASTFilter(node, mapping).ToList();
        Assert.Equivalent(expected, result, true);
    }

    [Theory]
    [MemberData(nameof(NinTestData))]
    public void TestNin(UCASTNode node, List<UnitTestDataSource.HydrologyData> expected)
    {
        var result = testdata.AsQueryable().ApplyUCASTFilter(node, mapping).ToList();
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

public class UnitTestFieldExprsWithCustomNameBinding
{
    private static readonly List<UnitTestDataSource.HydrologyData> hydrologyTestData = UnitTestDataSource.GetTestHydrologyData();
    private static readonly List<UnitTestDataSource.Ticket> ticketTestData = UnitTestDataSource.GetTickets();

    [Theory]
    [MemberData(nameof(NinTestData))]
    public void TestNinWithCustomMapping(UCASTNode node, List<UnitTestDataSource.HydrologyData> expected)
    {
        MappingConfiguration<UnitTestDataSource.HydrologyData> mapping = new(new Dictionary<string, string> {
            {"hydro.id", "data.id"},
            {"hydro.name", "data.name"},
            {"hydro.flood_stage", "data.flood_stage"},
            {"hydro.water_level_meters", "data.water_level_meters"},
        }, "data");
        var result = hydrologyTestData.AsQueryable().ApplyUCASTFilter(node, mapping).ToList();
        Assert.Equivalent(result, expected);
    }

    [Theory]
    [MemberData(nameof(NestedNinTestData))]
    public void TestNestedNinWithCustomMapping(UCASTNode node, List<UnitTestDataSource.Ticket> expected)
    {
        MappingConfiguration<UnitTestDataSource.Ticket> mapping = new(new Dictionary<string, string> {
            {"t.id", "ticket.id"},
            {"c.id", "ticket.customer.id"},
            {"c.name", "ticket.customer.name"},
        }, "ticket");
        var result = ticketTestData.AsQueryable().ApplyUCASTFilter(node, mapping).ToList();
        Assert.Equivalent(result, expected);
    }

    [Theory]
    [MemberData(nameof(NinTestData))]
    public void TestNinWithCustomEFCoreMapping(UCASTNode node, List<UnitTestDataSource.HydrologyData> expected)
    {
        EFCoreMappingConfiguration<UnitTestDataSource.HydrologyData> mapping = new(new Dictionary<string, string> {
            {"hydro.id", "data.id"},
            {"hydro.name", "data.name"},
            {"hydro.flood_stage", "data.flood_stage"},
            {"hydro.water_level_meters", "data.water_level_meters"},
        }, "data");
        var result = hydrologyTestData.AsQueryable().ApplyUCASTFilter(node, mapping).ToList();
        Assert.Equivalent(result, expected);
    }

    [Theory]
    [MemberData(nameof(NestedNinTestData))]
    public void TestNestedNinWithCustomEFCoreMapping(UCASTNode node, List<UnitTestDataSource.Ticket> expected)
    {
        EFCoreMappingConfiguration<UnitTestDataSource.Ticket> mapping = new(new Dictionary<string, string> {
            {"t.id", "ticket.id"},
            {"c.id", "ticket.customer.id"},
            {"c.name", "ticket.customer.name"},
            {"u.name", "ticket.user.name"},
        }, "ticket");
        var result = ticketTestData.AsQueryable().ApplyUCASTFilter(node, mapping).ToList();
        Assert.Equivalent(result, expected);
    }

    public static IEnumerable<object[]> NinTestData()
    {
        {
            yield return new object[] { new UCASTNode { Type = "field", Op = "nin", Field = "hydro.name", Value = new List<object>() { null! } }, hydrologyTestData.Where(d => !new List<object>() { null! }.Contains(d.Name!)).ToList() };
            yield return new object[] { new UCASTNode { Type = "field", Op = "nin", Field = "hydro.name", Value = new List<object>() { "River Alpha", "Lake Beta" } }, hydrologyTestData.Where(d => !new List<object> { "River Alpha", "Lake Beta" }.Contains(d.Name!)).ToList() };
            yield return new object[] { new UCASTNode { Type = "field", Op = "nin", Field = "hydro.id", Value = new List<object>() { 2, 5 } }, hydrologyTestData.Where(d => !new List<object>() { 2, 5 }.Contains(d.Id)).ToList() };
            yield return new object[] { new UCASTNode { Type = "field", Op = "nin", Field = "hydro.id", Value = new List<object>() { (long)2, (long)5 } }, hydrologyTestData.Where(d => !new List<object>() { (long)2, (long)5 }.Contains((long)d.Id)).ToList() };
            yield return new object[] { new UCASTNode { Type = "field", Op = "nin", Field = "hydro.flood_stage", Value = new List<object>() { true } }, hydrologyTestData.Where(d => !new List<object>() { true }.Contains(d.FloodStage)).ToList() };
            yield return new object[] { new UCASTNode { Type = "field", Op = "nin", Field = "hydro.water_level_meters", Value = new List<object>() { 2.5, 5.8 } }, hydrologyTestData.Where(d => !new List<object>() { 2.5, 5.8 }.Contains(d.WaterLevelMeters)).ToList() };
        }
    }

    public static IEnumerable<object[]> NestedNinTestData()
    {
        {
            yield return new object[] { new UCASTNode { Type = "field", Op = "nin", Field = "t.id", Value = new List<object>() { 1, 3 } }, ticketTestData.Where(d => !new List<object>() { 1, 3 }.Contains(d.Id)).ToList() };
            yield return new object[] { new UCASTNode { Type = "field", Op = "nin", Field = "ticket.description", Value = new List<object>() { null! } }, ticketTestData.Where(d => !new List<object>() { null! }.Contains(d.Description!)).ToList() };
            yield return new object[] { new UCASTNode { Type = "field", Op = "nin", Field = "c.name", Value = new List<object>() { "John Doe", "Jane Smith" } }, ticketTestData.Where(d => !new List<object> { "John Doe", "Jane Smith" }.Contains(d.Customer.Name!)).ToList() };
            yield return new object[] { new UCASTNode { Type = "field", Op = "nin", Field = "c.id", Value = new List<object>() { 2, 5 } }, ticketTestData.Where(d => !new List<object>() { 2, 5 }.Contains(d.Customer.Id)).ToList() };
            yield return new object[] { new UCASTNode { Type = "field", Op = "nin", Field = "c.id", Value = new List<object>() { (long)2, (long)5 } }, ticketTestData.Where(d => !new List<object>() { (long)2, (long)5 }.Contains((long)d.Customer.Id)).ToList() };
        }
    }
}

// Compound operations, with expected results from equivalent LINQ queries.
public class UnitTestCompoundExprs
{
    private static readonly List<UnitTestDataSource.HydrologyData> testdata = UnitTestDataSource.GetTestHydrologyData();
    private static readonly MappingConfiguration<UnitTestDataSource.HydrologyData> mapping = new(prefix: "data");

    [Theory]
    [MemberData(nameof(AndTestData))]
    public void TestAnd(UCASTNode node, List<UnitTestDataSource.HydrologyData> expected)
    {
        var result = testdata.AsQueryable().ApplyUCASTFilter(node, mapping).ToList();
        Assert.Equivalent(expected, result, true);
    }

    [Theory]
    [MemberData(nameof(OrTestData))]
    public void TestOr(UCASTNode node, List<UnitTestDataSource.HydrologyData> expected)
    {
        var result = testdata.AsQueryable().ApplyUCASTFilter(node, mapping).ToList();
        Assert.Equivalent(expected, result, true);
    }

    [Theory]
    [MemberData(nameof(AndNestedTestData))]
    public void TestAndNested(UCASTNode node, List<UnitTestDataSource.HydrologyData> expected)
    {
        var result = testdata.AsQueryable().ApplyUCASTFilter(node, mapping).ToList();
        Assert.Equivalent(expected, result, true);
    }

    [Theory]
    [MemberData(nameof(OrNestedTestData))]
    public void TestOrNested(UCASTNode node, List<UnitTestDataSource.HydrologyData> expected)
    {
        var result = testdata.AsQueryable().ApplyUCASTFilter(node, mapping).ToList();
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

public class UnitTestTypeConversions
{
    public record IntRecord(int Value);
    public record LongRecord(long Value);
    public record FloatRecord(float Value);
    public record DoubleRecord(double Value);
    public UCASTNode conditions = new()
    {
        Type = "compound",
        Op = "or",
        Value = new List<UCASTNode>{
            new() { Type = "field", Op = "ge", Field = "r.value", Value = 1500.0f },
            new() { Type = "compound", Op = "and", Value = new List<UCASTNode>{
                new() { Type = "field", Op = "lt", Field = "r.value", Value = 400L },
                new() { Type = "compound", Op = "or", Value = new List<UCASTNode>{
                    new() { Type = "field", Op = "gt", Field = "r.value", Value = 0 },
                    new() { Type = "field", Op = "lt", Field = "r.value", Value = -1500.0d },
                } },
            } },
        }
    };

    [Fact]
    public void TestComparisonsVersusIntSource()
    {
        int[] numbers = [-1523, 1894, -456, 789, -1002, 345, -1789, 567, 1234, -890, 123, -1456, 1678, -234, 567, -1890, 901, -345, 1567, -789];
        List<IntRecord> collection = [.. numbers.Select(n => new IntRecord(n))];
        var expected = collection.Where(x => x.Value >= 1500.0f || (x.Value < 400L && (x.Value > 0 || x.Value < -1500.0d))).OrderBy(x => x.Value).ToList();
        var result = collection.AsQueryable().ApplyUCASTFilter(conditions, new MappingConfiguration<IntRecord>(prefix: "r")).OrderBy(x => x.Value).ToList();
        Assert.Equivalent(expected, result, true);
    }

    [Fact]
    public void TestComparisonsVersusLongSource()
    {
        long[] numbers = [-1523, 1894, -456, 789, -1002, 345, -1789, 567, 1234, -890, 123, -1456, 1678, -234, 567, -1890, 901, -345, 1567, -789];
        List<LongRecord> collection = [.. numbers.Select(n => new LongRecord(n))];
        var expected = collection.Where(x => x.Value >= 1500.0f || (x.Value < 400L && (x.Value > 0 || x.Value < -1500.0d))).OrderBy(x => x.Value).ToList();
        var result = collection.AsQueryable().ApplyUCASTFilter(conditions, new MappingConfiguration<LongRecord>(prefix: "r")).OrderBy(x => x.Value).ToList();
        Assert.Equivalent(expected, result, true);
    }

    [Fact]
    public void TestComparisonsVersusFloatSource()
    {
        float[] numbers = [-1523.0f, 1894.0f, -456.0f, 789.0f, -1002.0f, 345.0f, -1789.0f, 567.0f, 1234.0f, -890.0f, 123.0f, -1456.0f, 1678.0f, -234.0f, 567.0f, -1890.0f, 901.0f, -345.0f, 1567.0f, -789.0f];
        List<FloatRecord> collection = [.. numbers.Select(n => new FloatRecord(n))];
        var expected = collection.Where(x => x.Value >= 1500.0f || (x.Value < 400L && (x.Value > 0 || x.Value < -1500.0d))).OrderBy(x => x.Value).ToList();
        var result = collection.AsQueryable().ApplyUCASTFilter(conditions, new MappingConfiguration<FloatRecord>(prefix: "r")).OrderBy(x => x.Value).ToList();
        Assert.Equivalent(expected, result, true);
    }

    [Fact]
    public void TestComparisonsVersusDoubleSource()
    {
        double[] numbers = [-1523.0d, 1894.0d, -456.0d, 789.0d, -1002.0d, 345.0d, -1789.0d, 567.0d, 1234.0d, -890.0d, 123.0d, -1456.0d, 1678.0d, -234.0d, 567.0d, -1890.0d, 901.0d, -345.0d, 1567.0d, -789.0d];
        List<DoubleRecord> collection = [.. numbers.Select(n => new DoubleRecord(n))];
        var expected = collection.Where(x => x.Value >= 1500.0f || (x.Value < 400L && (x.Value > 0 || x.Value < -1500.0d))).OrderBy(x => x.Value).ToList();
        var result = collection.AsQueryable().ApplyUCASTFilter(conditions, new MappingConfiguration<DoubleRecord>(prefix: "r")).OrderBy(x => x.Value).ToList();
        Assert.Equivalent(expected, result, true);
    }
}

public class UnitTestMasking
{
    private static readonly List<UnitTestDataSource.HydrologyData> testdata = UnitTestDataSource.GetTestHydrologyData();
    private static readonly MappingConfiguration<UnitTestDataSource.HydrologyData> mapping = new(prefix: "data");

    [Fact]
    public void TestMaskingReplace()
    {
        Dictionary<string, MaskingFunc> maskingRules = new()
        {
            { "data.name", new MaskingFunc() { Replace = new() { Value = "***" } } },
        };

        var maskedList = testdata.MaskElements(maskingRules, mapping);
        Assert.All(maskedList, item => Assert.Equal("***", item.Name));
    }

    [Fact]
    public void TestMaskingReplaceWithNamesToProperties()
    {
        Dictionary<string, MaskingFunc> maskingRules = new()
        {
            { "hydro.name", new MaskingFunc() { Replace = new() { Value = "***" } } },
        };

        MappingConfiguration<UnitTestDataSource.HydrologyData> mapping = new(new Dictionary<string, string> {
            {"hydro.name", "data.name"},
        }, "data");
        var maskedList = testdata.MaskElements(maskingRules, mapping);
        Assert.All(maskedList, item => Assert.Equal("***", item.Name));
    }

    [Fact]
    public void TestMaskingReplaceWithNestedDictionary()
    {
        Dictionary<string, Dictionary<string, MaskingFunc>> maskingRules = new()
        {
            { "hydro", new() { { "name", new MaskingFunc() { Replace = new() { Value = "***" } } }, } },
        };


        MappingConfiguration<UnitTestDataSource.HydrologyData> mapping = new(new Dictionary<string, string> {
            {"hydro.name", "data.name"},
        }, "data");
        var maskedList = testdata.MaskElements(maskingRules, mapping);
        Assert.All(maskedList, item => Assert.Equal("***", item.Name));
    }
}

public class UnitTestMaskingEFCore
{
    private static readonly List<UnitTestDataSource.HydrologyData> testdata = UnitTestDataSource.GetTestHydrologyData();
    private static readonly EFCoreMappingConfiguration<UnitTestDataSource.HydrologyData> mapping = new(prefix: "data");

    [Fact]
    public void TestMaskingReplace()
    {
        Dictionary<string, MaskingFunc> maskingRules = new()
        {
            { "data.name", new MaskingFunc() { Replace = new() { Value = "***" } } },
        };

        var maskedList = testdata.MaskElements(maskingRules, mapping);
        Assert.All(maskedList, item => Assert.Equal("***", item.Name));
    }

    [Fact]
    public void TestMaskingReplaceWithNamesToProperties()
    {
        Dictionary<string, MaskingFunc> maskingRules = new()
        {
            { "hydro.name", new MaskingFunc() { Replace = new() { Value = "***" } } },
        };

        EFCoreMappingConfiguration<UnitTestDataSource.HydrologyData> mapping = new(new Dictionary<string, string> {
            {"hydro.name", "data.name"},
        }, "data");
        var maskedList = testdata.MaskElements(maskingRules, mapping);
        Assert.All(maskedList, item => Assert.Equal("***", item.Name));
    }

    [Fact]
    public void TestMaskingReplaceWithNestedDictionary()
    {
        Dictionary<string, Dictionary<string, MaskingFunc>> maskingRules = new()
        {
            { "hydro", new() { { "name", new MaskingFunc() { Replace = new() { Value = "***" } } }, } },
        };

        MappingConfiguration<UnitTestDataSource.HydrologyData> mapping = new(new Dictionary<string, string> {
            {"hydro.name", "data.name"},
        }, "data");
        var maskedList = testdata.MaskElements(maskingRules, mapping);
        Assert.All(maskedList, item => Assert.Equal("***", item.Name));
    }
}

// AI-generated, used to provide a dataset for LINQ queries.
public class UnitTestDataSource
{
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

    public class Ticket
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public required Customer Customer { get; set; }
        public bool Resolved { get; set; }
        public User? UserNavigation { get; set; }
    }

    public class Customer
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public DateTime? LastUpdated { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }

    public static List<Ticket> GetTickets()
    {
        return [
            new Ticket { Id = 1, Description = "Issue with water flow", Customer = new Customer { Id = 1, Name = "John Doe" }, Resolved = false },
            new Ticket { Id = 2, Description = "Flooding in basement", Customer = new Customer { Id = 2, Name = "Jane Smith" }, Resolved = true },
            new Ticket { Id = 3, Description = "Water level sensor malfunction", Customer = new Customer { Id = 3, Name = "Alice Johnson" }, Resolved = false },
            new Ticket { Id = 4, Description = "Leak in the main pipe", Customer = new Customer { Id = 4, Name = "Bob Brown" }, Resolved = true },
            new Ticket { Id = 5, Description = null, Customer = new Customer { Id = 5, Name = "Charlie Davis" }, UserNavigation = new User { Id = 5, Name = "Eratosthenes" }, Resolved = false }
        ];
    }
}

