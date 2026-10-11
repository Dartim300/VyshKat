using Microsoft.Extensions.DependencyInjection;
using VyshKat.App.Catalog;
using VyshKat.App.Ui;
using VyshKat.Core.Assets;
using VyshKat.Core.Fleet;
using VyshKat.Core.Inspection;
using VyshKat.Core.Policies;

namespace VyshKat.App;

public static class ServiceRegistration
{
    public static IServiceCollection AddVyshKat(this IServiceCollection services, TextReader input, TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(output);

        services.AddSingleton(input);
        services.AddSingleton(output);

        services.AddSingleton<IFleetRepository, InMemoryFleetRepository>();
        services.AddSingleton<IServiceCenter>(_ => new ServiceCenter(ServiceCenter.DefaultMinConditionPercent));
        services.AddSingleton<IBeginnerPolicy>(_ => new SimplicityBeginnerPolicy(SimplicityBeginnerPolicy.DefaultThreshold));
        services.AddSingleton<InventoryNumberGenerator>();
        services.AddSingleton<FleetAcceptanceService>();
        services.AddSingleton<FleetReportService>();

        services.AddSingleton<ConsolePrompter>();
        services.AddSingleton<ReportPrinter>();
        services.AddSingleton<IntakeDialog>();
        services.AddSingleton<FileImporter>();
        services.AddSingleton<DemoScenario>();
        services.AddSingleton<ConsoleMenu>();

        AddAssetKinds(services);
        return services;
    }

    private static void AddAssetKinds(IServiceCollection services)
    {
        services.AddSingleton(new AssetKind("Самокат", "SC", AssetGroup.Transport, (number, p) =>
            new KickScooter(number, p.AskText("Модель"), AskCondition(p), AskSimplicity(p))));

        services.AddSingleton(new AssetKind("Велосипед", "BK", AssetGroup.Transport, (number, p) =>
            new Bicycle(number, p.AskText("Модель"), AskCondition(p), AskSimplicity(p))));

        services.AddSingleton(new AssetKind("Электросамокат", "ESC", AssetGroup.Transport, (number, p) =>
            new ElectricScooter(number, p.AskText("Модель"), AskCondition(p), AskSimplicity(p), AskEnergy(p))));

        services.AddSingleton(new AssetKind("Электровелосипед", "EBK", AssetGroup.Transport, (number, p) =>
            new ElectricBike(number, p.AskText("Модель"), AskCondition(p), AskSimplicity(p), AskEnergy(p))));

        services.AddSingleton(new AssetKind("Шлем", "HLM", AssetGroup.Equipment, (number, p) =>
            new Helmet(number, p.AskText("Модель"), AskCondition(p), p.AskText("Размер (S/M/L/XL)"))));

        services.AddSingleton(new AssetKind("Док-станция", "DCK", AssetGroup.Equipment, (number, p) =>
            new DockStation(number, p.AskText("Модель"), AskCondition(p), AskSlots(p), AskEnergy(p))));

        services.AddSingleton(new AssetKind("Зарядный шкаф", "CHG", AssetGroup.Equipment, (number, p) =>
            new ChargingCabinet(number, p.AskText("Модель"), AskCondition(p), AskSlots(p), AskEnergy(p))));
    }

    private static int AskCondition(ConsolePrompter p) =>
        p.AskInt("Техническое состояние, %", Asset.MinCondition, Asset.MaxCondition);

    private static int AskSimplicity(ConsolePrompter p) =>
        p.AskInt("Простота для новичка", Vehicle.MinSimplicity, Vehicle.MaxSimplicity);

    private static decimal AskEnergy(ConsolePrompter p) =>
        p.AskDecimal("Суточный расход энергии, кВт·ч", 0m, 1000m);

    private static int AskSlots(ConsolePrompter p) => p.AskInt("Число слотов", 1, 100);
}
