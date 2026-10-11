using VyshKat.Core.Abstractions;

namespace VyshKat.Core.Assets;

public abstract class Asset : IInventoriable
{
    public const int MinCondition = 0;
    public const int MaxCondition = 100;

    protected Asset(string inventoryNumber, string name, int conditionPercent)
    {
        InventoryNumber = Guard.NotBlank(inventoryNumber, nameof(inventoryNumber), "Инвентарный номер обязателен.");
        Name = Guard.NotBlank(name, nameof(name), "Наименование обязательно.");
        ConditionPercent = Guard.InRange(conditionPercent, MinCondition, MaxCondition, nameof(conditionPercent));
    }

    public string InventoryNumber { get; }

    public string Name { get; }

    public int ConditionPercent { get; }

    public abstract string KindName { get; }

    public override string ToString() => $"{InventoryNumber} — {KindName} «{Name}»";
}
