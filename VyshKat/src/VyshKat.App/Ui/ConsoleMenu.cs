using VyshKat.App.Catalog;

namespace VyshKat.App.Ui;

public sealed class ConsoleMenu
{
    private readonly ConsolePrompter _prompter;
    private readonly TextWriter _output;
    private readonly IntakeDialog _intake;
    private readonly ReportPrinter _printer;
    private readonly FileImporter _importer;

    public ConsoleMenu(
        ConsolePrompter prompter,
        TextWriter output,
        IntakeDialog intake,
        ReportPrinter printer,
        FileImporter importer)
    {
        _prompter = prompter ?? throw new ArgumentNullException(nameof(prompter));
        _output = output ?? throw new ArgumentNullException(nameof(output));
        _intake = intake ?? throw new ArgumentNullException(nameof(intake));
        _printer = printer ?? throw new ArgumentNullException(nameof(printer));
        _importer = importer ?? throw new ArgumentNullException(nameof(importer));
    }

    public void Run()
    {
        _output.WriteLine();
        _output.WriteLine("ВышКат — учёт парка. «Доедем до дедлайна»");

        try
        {
            while (true)
            {
                PrintMenu();
                var choice = _prompter.AskInt("Пункт меню", 0, 7);
                _output.WriteLine();

                if (choice == 0)
                {
                    _output.WriteLine("До встречи!");
                    return;
                }

                Execute(choice);
            }
        }
        catch (EndOfInputException)
        {
            _output.WriteLine();
            _output.WriteLine("Ввод завершён, выходим.");
        }
    }

    private void Execute(int choice)
    {
        switch (choice)
        {
            case 1:
                _intake.Run(AssetGroup.Transport);
                break;
            case 2:
                _intake.Run(AssetGroup.Equipment);
                break;
            case 3:
                _printer.PrintCounts();
                break;
            case 4:
                _printer.PrintEnergy();
                break;
            case 5:
                _printer.PrintBeginnerVehicles();
                break;
            case 6:
                _printer.PrintInventory();
                break;
            case 7:
                ImportFromFile();
                break;
        }
    }

    private void ImportFromFile()
    {
        var path = _prompter.AskOptionalText("Путь к файлу (Enter — demo.csv)");
        _importer.Import(path.Length > 0 ? path : DemoScenario.DefaultDataPath);
    }

    private void PrintMenu()
    {
        _output.WriteLine();
        _output.WriteLine("1. Принять транспорт");
        _output.WriteLine("2. Принять вещь на баланс");
        _output.WriteLine("3. Отчёт: количество единиц");
        _output.WriteLine("4. Отчёт: суммарное энергопотребление");
        _output.WriteLine("5. Транспорт для новичков");
        _output.WriteLine("6. Инвентарная ведомость");
        _output.WriteLine("7. Загрузить технику из файла");
        _output.WriteLine("0. Выход");
    }
}
