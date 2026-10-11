using System.Globalization;
using VyshKat.Core.Fleet;

namespace VyshKat.App.Ui;

public sealed class ReportPrinter
{
    private readonly FleetReportService _reports;
    private readonly TextWriter _output;

    public ReportPrinter(FleetReportService reports, TextWriter output)
    {
        _reports = reports ?? throw new ArgumentNullException(nameof(reports));
        _output = output ?? throw new ArgumentNullException(nameof(output));
    }

    public void PrintAcceptance(AcceptanceResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        var mark = result.Accepted ? "ПРИНЯТ" : "ОТКЛОНЁН";
        _output.WriteLine($"  [{mark}] {result.Asset} — {result.Reason}");
    }

    public void PrintCounts()
    {
        var report = _reports.GetCountReport();

        _output.WriteLine("Количество единиц на балансе:");
        _output.WriteLine($"Всего: {report.Total} (транспорт: {report.Vehicles}, вещи: {report.Equipment})");
        foreach (var (kind, count) in report.ByKind)
        {
            _output.WriteLine($"  {kind}: {count}");
        }
    }

    public void PrintEnergy()
    {
        var report = _reports.GetEnergyReport();

        _output.WriteLine("Энергопотребление парка:");
        _output.WriteLine(
            $"Суммарно за сутки: {FormatKwh(report.TotalDailyKwh)} кВт·ч (потребителей: {report.ConsumersCount})");
    }

    public void PrintBeginnerVehicles()
    {
        var vehicles = _reports.GetBeginnerFriendlyVehicles();

        _output.WriteLine("Транспорт для новичков:");
        if (vehicles.Count == 0)
        {
            _output.WriteLine("  Подходящего транспорта нет.");
            return;
        }

        foreach (var vehicle in vehicles)
        {
            _output.WriteLine($"  {vehicle} — простота {vehicle.BeginnerSimplicity}/10");
        }
    }

    public void PrintInventory()
    {
        var items = _reports.GetInventory();

        _output.WriteLine("Инвентарная ведомость:");
        if (items.Count == 0)
        {
            _output.WriteLine("  На балансе пока ничего нет.");
            return;
        }

        foreach (var item in items)
        {
            _output.WriteLine($"  {item.InventoryNumber,-14} {item.Name}");
        }
    }

    private static string FormatKwh(decimal value) => value.ToString("0.###", CultureInfo.InvariantCulture);
}
