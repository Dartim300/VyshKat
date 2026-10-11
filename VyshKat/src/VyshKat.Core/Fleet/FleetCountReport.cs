namespace VyshKat.Core.Fleet;

public sealed record FleetCountReport(
    int Total,
    int Vehicles,
    int Equipment,
    IReadOnlyDictionary<string, int> ByKind);
