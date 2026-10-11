namespace VyshKat.Core.Assets;

internal static class Guard
{
    public static string NotBlank(string? value, string paramName, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(message, paramName);
        }

        return value.Trim();
    }

    public static int InRange(int value, int min, int max, string paramName)
    {
        if (value < min || value > max)
        {
            throw new ArgumentOutOfRangeException(paramName, $"Значение {value} вне диапазона {min}–{max}.");
        }

        return value;
    }

    public static decimal NonNegative(decimal value, string paramName)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(paramName, $"Значение {value} не может быть отрицательным.");
        }

        return value;
    }
}
