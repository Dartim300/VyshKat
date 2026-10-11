namespace VyshKat.Core.Assets;

public sealed class Bicycle : Vehicle
{
    public Bicycle(string inventoryNumber, string name, int conditionPercent, int beginnerSimplicity)
        : base(inventoryNumber, name, conditionPercent, beginnerSimplicity)
    {
    }

    public override string KindName => "Велосипед";
}
