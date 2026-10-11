using VyshKat.Core.Assets;

namespace VyshKat.Tests;

internal static class TestAssets
{
    public static ElectricScooter ElectricScooter(
        string number = "VK-ESC-0001", int condition = 90, int simplicity = 7, decimal kwh = 0.5m) =>
        new(number, "Тестовый электросамокат", condition, simplicity, kwh);

    public static ElectricBike ElectricBike(
        string number = "VK-EBK-0001", int condition = 90, int simplicity = 5, decimal kwh = 0.7m) =>
        new(number, "Тестовый электровелосипед", condition, simplicity, kwh);

    public static Bicycle Bicycle(string number = "VK-BK-0001", int condition = 90, int simplicity = 8) =>
        new(number, "Тестовый велосипед", condition, simplicity);

    public static Helmet Helmet(string number = "VK-HLM-0001", int condition = 100) =>
        new(number, "Тестовый шлем", condition, "m");

    public static ChargingCabinet Cabinet(string number = "VK-CHG-0001", int condition = 100, decimal kwh = 5m) =>
        new(number, "Тестовый зарядный шкаф", condition, 12, kwh);
}
