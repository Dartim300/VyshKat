using VyshKat.Core.Policies;

namespace VyshKat.Tests.Core;

public class SimplicityBeginnerPolicyTests
{
    [Theory]
    [InlineData(1, false)]
    [InlineData(5, false)]
    [InlineData(6, true)]
    [InlineData(10, true)]
    public void DefaultPolicy_AllowsBeginnersFromSimplicitySix(int simplicity, bool expected)
    {
        var policy = new SimplicityBeginnerPolicy();

        Assert.Equal(expected, policy.IsSuitableForBeginner(TestAssets.Bicycle(simplicity: simplicity)));
    }

    [Fact]
    public void CustomThreshold_IsRespected()
    {
        var strict = new SimplicityBeginnerPolicy(threshold: 9);

        Assert.False(strict.IsSuitableForBeginner(TestAssets.Bicycle(simplicity: 8)));
        Assert.True(strict.IsSuitableForBeginner(TestAssets.Bicycle(simplicity: 9)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void Threshold_OutsideScale_Throws(int threshold)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new SimplicityBeginnerPolicy(threshold));
    }

    [Fact]
    public void NullVehicle_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new SimplicityBeginnerPolicy().IsSuitableForBeginner(null!));
    }
}
