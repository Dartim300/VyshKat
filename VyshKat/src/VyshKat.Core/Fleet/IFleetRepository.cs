using VyshKat.Core.Assets;

namespace VyshKat.Core.Fleet;

public interface IFleetRepository
{
    void Add(Asset asset);

    bool Contains(string inventoryNumber);

    IReadOnlyCollection<Asset> GetAll();
}
