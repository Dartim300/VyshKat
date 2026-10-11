namespace VyshKat.Core.Assets;

public sealed class Helmet : Equipment
{
    public Helmet(string inventoryNumber, string name, int conditionPercent, string size)
        : base(inventoryNumber, name, conditionPercent)
    {
        Size = Guard.NotBlank(size, nameof(size), "Размер шлема обязателен.").ToUpperInvariant();
    }

    public string Size { get; }

    public override string KindName => "Шлем";
}
