namespace VyshKat.App.Ui;

public sealed class DemoScenario
{
    private readonly FileImporter _importer;
    private readonly ReportPrinter _printer;
    private readonly TextWriter _output;

    public DemoScenario(FileImporter importer, ReportPrinter printer, TextWriter output)
    {
        _importer = importer ?? throw new ArgumentNullException(nameof(importer));
        _printer = printer ?? throw new ArgumentNullException(nameof(printer));
        _output = output ?? throw new ArgumentNullException(nameof(output));
    }

    public static string DefaultDataPath => Path.Combine(AppContext.BaseDirectory, "demo.csv");

    public void Run(string path)
    {
        _output.WriteLine($"Демонстрация: приём техники из файла {Path.GetFileName(path)}");
        if (!_importer.Import(path))
        {
            return;
        }

        _output.WriteLine();
        _printer.PrintCounts();
        _output.WriteLine();
        _printer.PrintEnergy();
        _output.WriteLine();
        _printer.PrintBeginnerVehicles();
        _output.WriteLine();
        _printer.PrintInventory();
    }
}
