using VyshKat.Core.Assets;

namespace VyshKat.Core.Inspection;

public sealed class ServiceCenter : IServiceCenter
{
    public const int DefaultMinConditionPercent = 70;

    private readonly int _minConditionPercent;

    public ServiceCenter(int minConditionPercent = DefaultMinConditionPercent)
    {
        if (minConditionPercent is < Asset.MinCondition or > Asset.MaxCondition)
        {
            throw new ArgumentOutOfRangeException(nameof(minConditionPercent));
        }

        _minConditionPercent = minConditionPercent;
    }

    public InspectionResult Inspect(Asset asset)
    {
        ArgumentNullException.ThrowIfNull(asset);

        return asset.ConditionPercent >= _minConditionPercent
            ? InspectionResult.Pass($"Состояние {asset.ConditionPercent}% — допуск в парк")
            : InspectionResult.Fail(
                $"Состояние {asset.ConditionPercent}% ниже допустимых {_minConditionPercent}%");
    }
}
