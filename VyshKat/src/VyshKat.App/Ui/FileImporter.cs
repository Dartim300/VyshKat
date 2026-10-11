using VyshKat.App.Catalog;
using VyshKat.Core.Fleet;

namespace VyshKat.App.Ui;

public sealed class FileImporter
{
    private readonly IReadOnlyList<AssetKind> _kinds;
    private readonly InventoryNumberGenerator _numbers;
    private readonly FleetAcceptanceService _acceptance;
    private readonly ReportPrinter _printer;
    private readonly TextWriter _output;

    public FileImporter(
        IEnumerable<AssetKind> kinds,
        InventoryNumberGenerator numbers,
        FleetAcceptanceService acceptance,
        ReportPrinter printer,
        TextWriter output)
    {
        _kinds = kinds?.ToList() ?? throw new ArgumentNullException(nameof(kinds));
        _numbers = numbers ?? throw new ArgumentNullException(nameof(numbers));
        _acceptance = acceptance ?? throw new ArgumentNullException(nameof(acceptance));
        _printer = printer ?? throw new ArgumentNullException(nameof(printer));
        _output = output ?? throw new ArgumentNullException(nameof(output));
    }

    public bool Import(string path)
    {
        if (!File.Exists(path))
        {
            _output.WriteLine($"Файл не найден: {path}");
            return false;
        }

        var lines = File.ReadAllLines(path);
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (line.Length == 0 || line.StartsWith('#'))
            {
                continue;
            }

            try
            {
                ImportLine(line);
            }
            catch (EndOfInputException)
            {
                _output.WriteLine($"  [ОШИБКА, строка {i + 1}] Не хватает полей.");
            }
            catch (Exception ex) when (ex is FormatException or ArgumentException)
            {
                _output.WriteLine($"  [ОШИБКА, строка {i + 1}] {ex.Message}");
            }
        }

        return true;
    }

    private void ImportLine(string line)
    {
        var fields = line.Split(';').Select(f => f.Trim()).ToArray();
        var kind = _kinds.FirstOrDefault(k => string.Equals(k.InventoryPrefix, fields[0], StringComparison.OrdinalIgnoreCase))
            ?? throw new FormatException($"Неизвестный вид «{fields[0]}».");

        var answers = ConsolePrompter.ForAnswers(fields.Skip(1));
        var asset = kind.Create(_numbers.Next(kind.InventoryPrefix), answers);
        if (answers.HasUnreadInput)
        {
            throw new FormatException("Лишние поля в строке.");
        }

        _printer.PrintAcceptance(_acceptance.Accept(asset));
    }
}
