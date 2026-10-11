using VyshKat.Core.Inspection;

namespace VyshKat.Tests.Core;

public class ServiceCenterTests
{
    [Theory]
    [InlineData(70, true)]
    [InlineData(100, true)]
    [InlineData(69, false)]
    [InlineData(0, false)]
    public void Inspect_DefaultThreshold_AcceptsFromSeventyPercent(int condition, bool expected)
    {
        var center = new ServiceCenter();

        var result = center.Inspect(TestAssets.ElectricScooter(condition: condition));

        Assert.Equal(expected, result.Passed);
        Assert.Contains($"{condition}%", result.Reason);
    }

    [Fact]
    public void Inspect_AppliesToEquipmentToo()
    {
        var center = new ServiceCenter();

        Assert.False(center.Inspect(TestAssets.Helmet(condition: 40)).Passed);
        Assert.True(center.Inspect(TestAssets.Helmet(condition: 100)).Passed);
    }

    [Fact]
    public void Inspect_CustomThreshold_IsRespected()
    {
        var center = new ServiceCenter(minConditionPercent: 95);

        Assert.False(center.Inspect(TestAssets.Bicycle(condition: 94)).Passed);
        Assert.True(center.Inspect(TestAssets.Bicycle(condition: 95)).Passed);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Constructor_InvalidThreshold_Throws(int threshold)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ServiceCenter(threshold));
    }

    [Fact]
    public void Inspect_Null_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new ServiceCenter().Inspect(null!));
    }

    [Fact]
    public void InspectionResult_Factories_SetVerdict()
    {
        Assert.True(InspectionResult.Pass().Passed);
        Assert.False(InspectionResult.Fail("брак").Passed);
        Assert.Equal("брак", InspectionResult.Fail("брак").Reason);
    }
}
