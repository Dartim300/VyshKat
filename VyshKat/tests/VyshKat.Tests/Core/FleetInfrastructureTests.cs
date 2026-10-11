using VyshKat.Core.Fleet;

namespace VyshKat.Tests.Core;

public class FleetInfrastructureTests
{
    [Fact]
    public void Repository_RejectsDuplicateNumber_IgnoringCase()
    {
        var repository = new InMemoryFleetRepository();
        repository.Add(TestAssets.Bicycle("VK-BK-0001"));

        Assert.True(repository.Contains("vk-bk-0001"));
        Assert.Throws<InvalidOperationException>(() => repository.Add(TestAssets.Helmet("vk-bk-0001")));
    }

    [Fact]
    public void Repository_GetAll_ReturnsSnapshot()
    {
        var repository = new InMemoryFleetRepository();
        var snapshot = repository.GetAll();

        repository.Add(TestAssets.Bicycle());

        Assert.Empty(snapshot);
        Assert.Single(repository.GetAll());
    }

    [Fact]
    public void Repository_AddNull_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new InMemoryFleetRepository().Add(null!));
    }

    [Fact]
    public void Generator_NumbersEachPrefixSeparately()
    {
        var generator = new InventoryNumberGenerator();

        Assert.Equal("VK-ESC-0001", generator.Next("ESC"));
        Assert.Equal("VK-ESC-0002", generator.Next("esc"));
        Assert.Equal("VK-HLM-0001", generator.Next("HLM"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Generator_BlankPrefix_Throws(string prefix)
    {
        Assert.Throws<ArgumentException>(() => new InventoryNumberGenerator().Next(prefix));
    }
}
