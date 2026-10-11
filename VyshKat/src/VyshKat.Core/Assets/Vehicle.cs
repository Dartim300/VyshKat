namespace VyshKat.Core.Assets;

public abstract class Vehicle : Asset
{
    public const int MinSimplicity = 1;
    public const int MaxSimplicity = 10;

    protected Vehicle(string inventoryNumber, string name, int conditionPercent, int beginnerSimplicity)
        : base(inventoryNumber, name, conditionPercent)
    {
        BeginnerSimplicity = Guard.InRange(beginnerSimplicity, MinSimplicity, MaxSimplicity, nameof(beginnerSimplicity));
    }

    public int BeginnerSimplicity { get; }
}
