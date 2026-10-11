using VyshKat.App.Catalog;
using VyshKat.Core.Fleet;

namespace VyshKat.App.Ui;

public sealed class IntakeDialog
{
    private readonly IReadOnlyList<AssetKind> _kinds;
    private readonly ConsolePrompter _prompter;
    private readonly TextWriter _output;
    private readonly InventoryNumberGenerator _numbers;
    private readonly FleetAcceptanceService _acceptance;
    private readonly ReportPrinter _printer;

    public IntakeDialog(
        IEnumerable<AssetKind> kinds,
        ConsolePrompter prompter,
        TextWriter output,
        InventoryNumberGenerator numbers,
        FleetAcceptanceService acceptance,
        ReportPrinter printer)
    {
        _kinds = kinds?.ToList() ?? throw new ArgumentNullException(nameof(kinds));
        _prompter = prompter ?? throw new ArgumentNullException(nameof(prompter));
        _output = output ?? throw new ArgumentNullException(nameof(output));
        _numbers = numbers ?? throw new ArgumentNullException(nameof(numbers));
        _acceptance = acceptance ?? throw new ArgumentNullException(nameof(acceptance));
        _printer = printer ?? throw new ArgumentNullException(nameof(printer));
    }

    public void Run(AssetGroup group)
    {
        var kinds = _kinds.Where(k => k.Group == group).ToList();
        if (kinds.Count == 0)
        {
            _output.WriteLine("Нет доступных видов для приёмки.");
            return;
        }

        _output.WriteLine(group == AssetGroup.Transport ? "Какой транспорт принимаем?" : "Какую вещь принимаем?");
        for (var i = 0; i < kinds.Count; i++)
        {
            _output.WriteLine($"  {i + 1}. {kinds[i].Title}");
        }

        var kind = kinds[_prompter.AskInt("Вид", 1, kinds.Count) - 1];
        var number = _numbers.Next(kind.InventoryPrefix);
        _output.WriteLine($"Акт приёмки, инвентарный номер: {number}");

        try
        {
            var asset = kind.Create(number, _prompter);
            _printer.PrintAcceptance(_acceptance.Accept(asset));
        }
        catch (ArgumentException ex)
        {
            _output.WriteLine($"  Некорректные данные: {ex.Message}");
        }
    }
}
