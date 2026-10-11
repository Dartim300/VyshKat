namespace VyshKat.Core.Fleet;

public sealed class InventoryNumberGenerator
{
    private readonly Dictionary<string, int> _counters = new(StringComparer.OrdinalIgnoreCase);

    public string Next(string prefix)
    {
        if (string.IsNullOrWhiteSpace(prefix))
        {
            throw new ArgumentException("Префикс обязателен.", nameof(prefix));
        }

        var key = prefix.Trim().ToUpperInvariant();
        var next = _counters.GetValueOrDefault(key) + 1;
        _counters[key] = next;
        return $"VK-{key}-{next:D4}";
    }
}
