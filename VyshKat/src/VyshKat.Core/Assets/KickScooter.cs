namespace VyshKat.Core.Assets;

public sealed class KickScooter : Vehicle
{
    public KickScooter(string inventoryNumber, string name, int conditionPercent, int beginnerSimplicity)
        : base(inventoryNumber, name, conditionPercent, beginnerSimplicity)
    {
    }

    public override string KindName => "Самокат";
}
