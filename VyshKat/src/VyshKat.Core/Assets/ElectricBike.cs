using VyshKat.Core.Abstractions;

namespace VyshKat.Core.Assets;

public sealed class ElectricBike : Vehicle, IEnergyConsumer
{
    public ElectricBike(
        string inventoryNumber, string name, int conditionPercent, int beginnerSimplicity, decimal dailyEnergyKwh)
        : base(inventoryNumber, name, conditionPercent, beginnerSimplicity)
    {
        DailyEnergyKwh = Guard.NonNegative(dailyEnergyKwh, nameof(dailyEnergyKwh));
    }

    public decimal DailyEnergyKwh { get; }

    public override string KindName => "Электровелосипед";
}
