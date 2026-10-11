using VyshKat.Core.Assets;

namespace VyshKat.Core.Policies;

public interface IBeginnerPolicy
{
    bool IsSuitableForBeginner(Vehicle vehicle);
}
