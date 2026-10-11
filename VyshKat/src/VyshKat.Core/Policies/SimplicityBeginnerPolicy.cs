using VyshKat.Core.Assets;

namespace VyshKat.Core.Policies;

public sealed class SimplicityBeginnerPolicy : IBeginnerPolicy
{
    public const int DefaultThreshold = 6;

    private readonly int _threshold;

    public SimplicityBeginnerPolicy(int threshold = DefaultThreshold)
    {
        if (threshold is < Vehicle.MinSimplicity or > Vehicle.MaxSimplicity)
        {
            throw new ArgumentOutOfRangeException(nameof(threshold));
        }

        _threshold = threshold;
    }

    public bool IsSuitableForBeginner(Vehicle vehicle)
    {
        ArgumentNullException.ThrowIfNull(vehicle);
        return vehicle.BeginnerSimplicity >= _threshold;
    }
}
