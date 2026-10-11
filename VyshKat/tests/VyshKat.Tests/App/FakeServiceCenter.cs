using VyshKat.Core.Assets;
using VyshKat.Core.Inspection;

namespace VyshKat.Tests.App;

internal sealed class FakeServiceCenter : IServiceCenter
{
    private readonly bool _pass;

    public FakeServiceCenter(bool pass)
    {
        _pass = pass;
    }

    public List<Asset> Inspected { get; } = [];

    public InspectionResult Inspect(Asset asset)
    {
        Inspected.Add(asset);
        return _pass ? InspectionResult.Pass("fake: допуск") : InspectionResult.Fail("fake: отказ");
    }
}
