using VyshKat.Core.Assets;

namespace VyshKat.Core.Fleet;

public sealed record AcceptanceResult(Asset Asset, bool Accepted, string Reason);
