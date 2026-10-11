using Moq;
using VyshKat.Core.Assets;
using VyshKat.Core.Fleet;
using VyshKat.Core.Policies;

namespace VyshKat.Tests.Core;

public class FleetReportServiceTests
{
    private readonly InMemoryFleetRepository _repository = new();

    [Fact]
    public void EmptyFleet_ReportsZeros()
    {
        var reports = new FleetReportService(_repository, new SimplicityBeginnerPolicy());

        var counts = reports.GetCountReport();
        var energy = reports.GetEnergyReport();

        Assert.Equal(0, counts.Total);
        Assert.Empty(counts.ByKind);
        Assert.Equal(0m, energy.TotalDailyKwh);
        Assert.Equal(0, energy.ConsumersCount);
        Assert.Empty(reports.GetBeginnerFriendlyVehicles());
        Assert.Empty(reports.GetInventory());
    }

    [Fact]
    public void CountReport_SplitsVehiclesAndEquipment_AndGroupsByKind()
    {
        AddAll(
            TestAssets.ElectricScooter("VK-ESC-0001"),
            TestAssets.ElectricScooter("VK-ESC-0002"),
            TestAssets.Bicycle(),
            TestAssets.Helmet(),
            TestAssets.Cabinet());
        var reports = new FleetReportService(_repository, new SimplicityBeginnerPolicy());

        var counts = reports.GetCountReport();

        Assert.Equal(5, counts.Total);
        Assert.Equal(3, counts.Vehicles);
        Assert.Equal(2, counts.Equipment);
        Assert.Equal(2, counts.ByKind["Электросамокат"]);
        Assert.Equal(1, counts.ByKind["Шлем"]);
    }

    [Fact]
    public void EnergyReport_SumsOnlyEnergyConsumers_IncludingEquipment()
    {
        AddAll(
            TestAssets.ElectricScooter(kwh: 0.45m),
            TestAssets.ElectricBike(kwh: 0.6m),
            TestAssets.Cabinet(kwh: 6m),
            TestAssets.Bicycle(),
            TestAssets.Helmet());
        var reports = new FleetReportService(_repository, new SimplicityBeginnerPolicy());

        var energy = reports.GetEnergyReport();

        Assert.Equal(7.05m, energy.TotalDailyKwh);
        Assert.Equal(3, energy.ConsumersCount);
    }

    [Fact]
    public void BeginnerVehicles_UsesPolicy_AndSkipsEquipment()
    {
        AddAll(
            TestAssets.ElectricScooter(simplicity: 7),
            TestAssets.ElectricBike(simplicity: 5),
            TestAssets.Bicycle(simplicity: 9),
            TestAssets.Helmet());
        var reports = new FleetReportService(_repository, new SimplicityBeginnerPolicy());

        var beginners = reports.GetBeginnerFriendlyVehicles();

        Assert.Equal(new[] { "VK-BK-0001", "VK-ESC-0001" }, beginners.Select(v => v.InventoryNumber));
    }

    [Fact]
    public void BeginnerVehicles_DelegatesDecisionToInjectedPolicy()
    {
        AddAll(TestAssets.ElectricScooter(simplicity: 10), TestAssets.Bicycle(simplicity: 1));
        var onlyBicycles = new Mock<IBeginnerPolicy>();
        onlyBicycles.Setup(p => p.IsSuitableForBeginner(It.IsAny<Vehicle>())).Returns<Vehicle>(v => v is Bicycle);
        var reports = new FleetReportService(_repository, onlyBicycles.Object);

        var beginners = reports.GetBeginnerFriendlyVehicles();

        Assert.IsType<Bicycle>(Assert.Single(beginners));
    }

    [Fact]
    public void Inventory_ContainsVehiclesAndEquipment_SortedByNumber()
    {
        AddAll(TestAssets.Helmet("VK-HLM-0001"), TestAssets.ElectricScooter("VK-ESC-0001"), TestAssets.Bicycle("VK-BK-0001"));
        var reports = new FleetReportService(_repository, new SimplicityBeginnerPolicy());

        var inventory = reports.GetInventory();

        Assert.Equal(new[] { "VK-BK-0001", "VK-ESC-0001", "VK-HLM-0001" }, inventory.Select(i => i.InventoryNumber));
        Assert.All(inventory, i => Assert.False(string.IsNullOrWhiteSpace(i.Name)));
    }

    [Fact]
    public void Constructor_NullDependencies_Throw()
    {
        Assert.Throws<ArgumentNullException>(() => new FleetReportService(null!, new SimplicityBeginnerPolicy()));
        Assert.Throws<ArgumentNullException>(() => new FleetReportService(_repository, null!));
    }

    private void AddAll(params Asset[] assets)
    {
        foreach (var asset in assets)
        {
            _repository.Add(asset);
        }
    }
}
