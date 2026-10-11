namespace VyshKat.Core.Assets;

public abstract class Equipment : Asset
{
    protected Equipment(string inventoryNumber, string name, int conditionPercent)
        : base(inventoryNumber, name, conditionPercent)
    {
    }
}
