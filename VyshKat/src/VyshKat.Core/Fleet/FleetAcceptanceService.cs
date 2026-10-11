using VyshKat.Core.Assets;
using VyshKat.Core.Inspection;

namespace VyshKat.Core.Fleet;

public sealed class FleetAcceptanceService
{
    private readonly IFleetRepository _repository;
    private readonly IServiceCenter _serviceCenter;

    public FleetAcceptanceService(IFleetRepository repository, IServiceCenter serviceCenter)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _serviceCenter = serviceCenter ?? throw new ArgumentNullException(nameof(serviceCenter));
    }

    public AcceptanceResult Accept(Asset asset)
    {
        ArgumentNullException.ThrowIfNull(asset);

        if (_repository.Contains(asset.InventoryNumber))
        {
            return new AcceptanceResult(
                asset, false, $"Инвентарный номер {asset.InventoryNumber} уже на балансе");
        }

        var inspection = _serviceCenter.Inspect(asset);
        if (!inspection.Passed)
        {
            return new AcceptanceResult(asset, false, $"Техосмотр не пройден: {inspection.Reason}");
        }

        _repository.Add(asset);
        return new AcceptanceResult(asset, true, inspection.Reason);
    }
}
