using VyshKat.Core.Abstractions;
using VyshKat.Core.Assets;

namespace VyshKat.Tests.Core;

public class AssetTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithoutInventoryNumber_Throws(string number)
    {
        Assert.Throws<ArgumentException>(() => TestAssets.Bicycle(number: number));
    }

    [Fact]
    public void Constructor_WithoutName_Throws()
    {
        Assert.Throws<ArgumentException>(() => new KickScooter("VK-SC-0001", "", 90, 7));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void Vehicle_SimplicityOutsideOneToTen_Throws(int simplicity)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => TestAssets.Bicycle(simplicity: simplicity));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Asset_ConditionOutsideZeroToHundred_Throws(int condition)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => TestAssets.Helmet(condition: condition));
    }

    [Fact]
    public void EnergyConsumer_NegativeConsumption_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => TestAssets.ElectricScooter(kwh: -0.1m));
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new DockStation("VK-DCK-0001", "Док", 100, 10, -1m));
    }

    [Fact]
    public void Constructor_TrimsInventoryNumberAndName()
    {
        var bike = new Bicycle("  VK-BK-0007 ", " Classic ", 90, 8);

        Assert.Equal("VK-BK-0007", bike.InventoryNumber);
        Assert.Equal("Classic", bike.Name);
    }

    [Fact]
    public void ElectricTransportAndStations_AreEnergyConsumers_MuscleTransportAndHelmetsAreNot()
    {
        Assert.IsAssignableFrom<IEnergyConsumer>(TestAssets.ElectricScooter());
        Assert.IsAssignableFrom<IEnergyConsumer>(TestAssets.ElectricBike());
        Assert.IsAssignableFrom<IEnergyConsumer>(TestAssets.Cabinet());
        Assert.IsAssignableFrom<IEnergyConsumer>(new DockStation("VK-DCK-0001", "Док", 100, 10, 2m));

        Assert.False(IsEnergyConsumer(TestAssets.Bicycle()));
        Assert.False(IsEnergyConsumer(new KickScooter("VK-SC-0001", "Kick", 90, 9)));
        Assert.False(IsEnergyConsumer(TestAssets.Helmet()));
    }

    [Fact]
    public void AllAssets_AreInventoriable_WithKindName()
    {
        Asset[] assets =
        [
            TestAssets.ElectricScooter(), TestAssets.ElectricBike(), TestAssets.Bicycle(),
            new KickScooter("VK-SC-0001", "Kick", 90, 9), TestAssets.Helmet(), TestAssets.Cabinet(),
            new DockStation("VK-DCK-0001", "Док", 100, 10, 2m),
        ];

        Assert.All(assets, a => Assert.IsAssignableFrom<IInventoriable>(a));
        Assert.Equal(assets.Length, assets.Select(a => a.KindName).Distinct().Count());
    }

    [Fact]
    public void Helmet_SizeIsNormalized_AndRequired()
    {
        Assert.Equal("M", TestAssets.Helmet().Size);
        Assert.Throws<ArgumentException>(() => new Helmet("VK-HLM-0002", "Шлем", 100, ""));
    }

    [Fact]
    public void Stations_RejectInvalidSlotCount()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ChargingCabinet("VK-CHG-0001", "Шкаф", 100, 0, 1m));
        Assert.Throws<ArgumentOutOfRangeException>(() => new DockStation("VK-DCK-0001", "Док", 100, 101, 1m));
    }

    [Fact]
    public void ToString_ContainsNumberKindAndName()
    {
        Assert.Equal("VK-BK-0001 — Велосипед «Тестовый велосипед»", TestAssets.Bicycle().ToString());
    }

    private static bool IsEnergyConsumer(Asset asset) => asset is IEnergyConsumer;
}
