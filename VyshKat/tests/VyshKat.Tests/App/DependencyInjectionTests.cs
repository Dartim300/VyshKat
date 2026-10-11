using Microsoft.Extensions.DependencyInjection;
using Moq;
using VyshKat.App.Catalog;
using VyshKat.App.Ui;
using VyshKat.Core.Assets;
using VyshKat.Core.Fleet;
using VyshKat.Core.Inspection;
using VyshKat.Core.Policies;

namespace VyshKat.Tests.App;

public class DependencyInjectionTests
{
    [Fact]
    public void Container_ResolvesWholeApplication_WithDefaultImplementations()
    {
        using var provider = TestApp.Build(string.Empty, new StringWriter());

        Assert.NotNull(provider.GetRequiredService<ConsoleMenu>());
        Assert.NotNull(provider.GetRequiredService<DemoScenario>());
        Assert.NotNull(provider.GetRequiredService<FileImporter>());
        Assert.IsType<ServiceCenter>(provider.GetRequiredService<IServiceCenter>());
        Assert.IsType<SimplicityBeginnerPolicy>(provider.GetRequiredService<IBeginnerPolicy>());
        Assert.Same(provider.GetRequiredService<IFleetRepository>(), provider.GetRequiredService<IFleetRepository>());
    }

    [Fact]
    public void Container_RegistersAllAssetKinds_WithUniquePrefixes()
    {
        using var provider = TestApp.Build(string.Empty, new StringWriter());

        var kinds = provider.GetServices<AssetKind>().ToList();

        Assert.Equal(4, kinds.Count(k => k.Group == AssetGroup.Transport));
        Assert.Equal(3, kinds.Count(k => k.Group == AssetGroup.Equipment));
        Assert.Equal(kinds.Count, kinds.Select(k => k.InventoryPrefix).Distinct().Count());
    }

    [Fact]
    public void ReplacedServiceCenter_Fake_RejectsEvenPerfectAsset()
    {
        var fake = new FakeServiceCenter(pass: false);
        using var provider = TestApp.Build(string.Empty, new StringWriter(), fake);
        var acceptance = provider.GetRequiredService<FleetAcceptanceService>();
        var perfect = TestAssets.ElectricScooter(condition: 100);

        var result = acceptance.Accept(perfect);

        Assert.False(result.Accepted);
        Assert.Contains("fake: отказ", result.Reason);
        Assert.Same(perfect, Assert.Single(fake.Inspected));
        Assert.Empty(provider.GetRequiredService<IFleetRepository>().GetAll());
    }

    [Fact]
    public void ReplacedServiceCenter_Mock_AcceptsAssetThatRealCenterWouldReject()
    {
        var mock = new Mock<IServiceCenter>();
        mock.Setup(c => c.Inspect(It.IsAny<Asset>())).Returns(InspectionResult.Pass("mock: допуск"));
        using var provider = TestApp.Build(string.Empty, new StringWriter(), mock.Object);
        var worn = TestAssets.Helmet(condition: 10);

        Assert.False(new ServiceCenter().Inspect(worn).Passed);

        var result = provider.GetRequiredService<FleetAcceptanceService>().Accept(worn);

        Assert.True(result.Accepted);
        Assert.Contains(worn, provider.GetRequiredService<IFleetRepository>().GetAll());
        mock.Verify(c => c.Inspect(worn), Times.Once);
    }
}
