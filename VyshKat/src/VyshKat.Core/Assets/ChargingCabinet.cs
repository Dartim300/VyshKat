using VyshKat.Core.Abstractions;

namespace VyshKat.Core.Assets;

public sealed class ChargingCabinet : Equipment, IEnergyConsumer
{
    public ChargingCabinet(string inventoryNumber, string name, int conditionPercent, int slots, decimal dailyEnergyKwh)
        : base(inventoryNumber, name, conditionPercent)
    {
        Slots = Guard.InRange(slots, 1, 100, nameof(slots));
        DailyEnergyKwh = Guard.NonNegative(dailyEnergyKwh, nameof(dailyEnergyKwh));
    }

    public int Slots { get; }

    public decimal DailyEnergyKwh { get; }

    public override string KindName => "Зарядный шкаф";
}
