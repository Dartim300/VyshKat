using VyshKat.Core.Assets;

namespace VyshKat.Core.Fleet;

public sealed class InMemoryFleetRepository : IFleetRepository
{
    private readonly Dictionary<string, Asset> _assets = new(StringComparer.OrdinalIgnoreCase);

    public void Add(Asset asset)
    {
        ArgumentNullException.ThrowIfNull(asset);

        if (!_assets.TryAdd(asset.InventoryNumber, asset))
        {
            throw new InvalidOperationException($"Инвентарный номер {asset.InventoryNumber} уже на балансе.");
        }
    }

    public bool Contains(string inventoryNumber) => _assets.ContainsKey(inventoryNumber);

    public IReadOnlyCollection<Asset> GetAll() => _assets.Values.ToList();
}
