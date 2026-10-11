using VyshKat.Core.Abstractions;
using VyshKat.Core.Assets;
using VyshKat.Core.Policies;

namespace VyshKat.Core.Fleet;

public sealed class FleetReportService
{
    private readonly IFleetRepository _repository;
    private readonly IBeginnerPolicy _beginnerPolicy;

    public FleetReportService(IFleetRepository repository, IBeginnerPolicy beginnerPolicy)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _beginnerPolicy = beginnerPolicy ?? throw new ArgumentNullException(nameof(beginnerPolicy));
    }

    public FleetCountReport GetCountReport()
    {
        var assets = _repository.GetAll();

        var byKind = assets
            .GroupBy(a => a.KindName)
            .OrderBy(g => g.Key, StringComparer.CurrentCulture)
            .ToDictionary(g => g.Key, g => g.Count());

        return new FleetCountReport(
            Total: assets.Count,
            Vehicles: assets.OfType<Vehicle>().Count(),
            Equipment: assets.OfType<Equipment>().Count(),
            ByKind: byKind);
    }

    public EnergyReport GetEnergyReport()
    {
        var consumers = _repository.GetAll().OfType<IEnergyConsumer>().ToList();
        return new EnergyReport(consumers.Sum(c => c.DailyEnergyKwh), consumers.Count);
    }

    public IReadOnlyList<Vehicle> GetBeginnerFriendlyVehicles() =>
        _repository.GetAll()
            .OfType<Vehicle>()
            .Where(_beginnerPolicy.IsSuitableForBeginner)
            .OrderByDescending(v => v.BeginnerSimplicity)
            .ThenBy(v => v.InventoryNumber, StringComparer.Ordinal)
            .ToList();

    public IReadOnlyList<IInventoriable> GetInventory() =>
        _repository.GetAll()
            .OrderBy(a => a.InventoryNumber, StringComparer.Ordinal)
            .ToList<IInventoriable>();
}
