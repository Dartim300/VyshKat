using VyshKat.Core.Assets;

namespace VyshKat.Core.Inspection;

public interface IServiceCenter
{
    InspectionResult Inspect(Asset asset);
}
