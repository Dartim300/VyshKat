using Microsoft.Extensions.DependencyInjection;
using VyshKat.App.Ui;

namespace VyshKat.Tests.App;

public class ConsoleMenuTests
{
    [Fact]
    public void AcceptElectricScooter_ThenReports_ShowItEverywhere()
    {
        var output = Run(TestApp.Lines(
            "1", "3", "ВышКат E-Lite", "95", "7", "0,45",
            "3", "4", "5", "6", "0"));

        Assert.Contains("Акт приёмки, инвентарный номер: VK-ESC-0001", output);
        Assert.Contains("[ПРИНЯТ] VK-ESC-0001 — Электросамокат «ВышКат E-Lite»", output);
        Assert.Contains("Всего: 1 (транспорт: 1, вещи: 0)", output);
        Assert.Contains("Суммарно за сутки: 0.45 кВт·ч (потребителей: 1)", output);
        Assert.Contains("простота 7/10", output);
        Assert.Contains("VK-ESC-0001    ВышКат E-Lite", output);
        Assert.Contains("До встречи!", output);
    }

    [Theory]
    [InlineData(new[] { "1", "1", "Kick", "90", "9" }, "[ПРИНЯТ] VK-SC-0001")]
    [InlineData(new[] { "1", "2", "Classic", "90", "8" }, "[ПРИНЯТ] VK-BK-0001")]
    [InlineData(new[] { "1", "4", "Volt", "90", "5", "0.6" }, "[ПРИНЯТ] VK-EBK-0001")]
    [InlineData(new[] { "2", "1", "Шлем", "100", "m" }, "[ПРИНЯТ] VK-HLM-0001")]
    [InlineData(new[] { "2", "2", "Док", "100", "10", "2.4" }, "[ПРИНЯТ] VK-DCK-0001")]
    [InlineData(new[] { "2", "3", "Шкаф", "100", "20", "6" }, "[ПРИНЯТ] VK-CHG-0001")]
    public void EveryRegisteredKind_CanBeAcceptedFromMenu(string[] answers, string expected)
    {
        var output = Run(TestApp.Lines([.. answers, "0"]));

        Assert.Contains(expected, output);
    }

    [Fact]
    public void WornAsset_IsRejectedByRealServiceCenter()
    {
        var output = Run(TestApp.Lines("1", "2", "Старый велосипед", "50", "8", "6", "0"));

        Assert.Contains("[ОТКЛОНЁН] VK-BK-0001", output);
        Assert.Contains("ниже допустимых 70%", output);
        Assert.Contains("На балансе пока ничего нет.", output);
    }

    [Fact]
    public void FakeServiceCenter_InjectedIntoMenu_DecidesAcceptance()
    {
        var fake = new FakeServiceCenter(pass: false);

        var output = Run(TestApp.Lines("2", "1", "Новый шлем", "100", "L", "0"), fake);

        Assert.Contains("[ОТКЛОНЁН] VK-HLM-0001", output);
        Assert.Contains("fake: отказ", output);
        Assert.Single(fake.Inspected);
    }

    [Fact]
    public void EmptyFleet_ReportsSayNothingYet()
    {
        var output = Run(TestApp.Lines("5", "6", "0"));

        Assert.Contains("Подходящего транспорта нет.", output);
        Assert.Contains("На балансе пока ничего нет.", output);
    }

    [Fact]
    public void InvalidAnswers_AreAskedAgain()
    {
        var output = Run(TestApp.Lines("abc", "9", "1", "3", "", "Модель", "200", "95", "7", "-1", "0.5", "0"));

        Assert.Contains("Введите целое число от 0 до 7.", output);
        Assert.Contains("Значение не может быть пустым.", output);
        Assert.Contains("Введите целое число от 0 до 100.", output);
        Assert.Contains("Введите число от 0 до 1000.", output);
        Assert.Contains("[ПРИНЯТ] VK-ESC-0001", output);
    }

    [Fact]
    public void EndOfInput_ExitsGracefully()
    {
        var output = Run(TestApp.Lines("1", "3", "Модель"));

        Assert.Contains("Ввод завершён, выходим.", output);
    }

    [Fact]
    public void ImportMenuItem_LoadsFileGivenByUser()
    {
        var path = TestApp.CsvFile("ESC;Импортный самокат;95;7;0.5", "HLM;Импортный шлем;100;S");

        var output = Run(TestApp.Lines("7", path, "6", "0"));

        Assert.Contains("[ПРИНЯТ] VK-ESC-0001 — Электросамокат «Импортный самокат»", output);
        Assert.Contains("[ПРИНЯТ] VK-HLM-0001 — Шлем «Импортный шлем»", output);
        Assert.Contains("VK-HLM-0001    Импортный шлем", output);
    }

    [Fact]
    public void ImportMenuItem_EmptyPath_LoadsDemoFile()
    {
        var output = Run(TestApp.Lines("7", "", "3", "0"));

        Assert.Contains("Всего: 7 (транспорт: 4, вещи: 3)", output);
    }

    [Fact]
    public void ImportMenuItem_MissingFile_IsReported()
    {
        var output = Run(TestApp.Lines("7", "нет-такого-файла.csv", "0"));

        Assert.Contains("Файл не найден: нет-такого-файла.csv", output);
    }

    [Fact]
    public void DemoScenario_WithShippedDemoFile_ShowsAllScenarios()
    {
        var writer = new StringWriter();
        using var provider = TestApp.Build(string.Empty, writer);

        provider.GetRequiredService<DemoScenario>().Run(DemoScenario.DefaultDataPath);
        var output = writer.ToString();

        Assert.Contains("[ПРИНЯТ] VK-ESC-0001", output);
        Assert.Contains("[ОТКЛОНЁН] VK-ESC-0002", output);
        Assert.Contains("[ОТКЛОНЁН] VK-HLM-0002", output);
        Assert.Equal(3, CountOccurrences(output, "[ОШИБКА, строка"));
        Assert.Contains("Всего: 7 (транспорт: 4, вещи: 3)", output);
        Assert.Contains("Суммарно за сутки: 9.45 кВт·ч (потребителей: 4)", output);

        var beginners = output[output.IndexOf("Транспорт для новичков:", StringComparison.Ordinal)..output.IndexOf("Инвентарная ведомость:", StringComparison.Ordinal)];
        var kick = beginners.IndexOf("VK-SC-0001", StringComparison.Ordinal);
        var bicycle = beginners.IndexOf("VK-BK-0001", StringComparison.Ordinal);
        var scooter = beginners.IndexOf("VK-ESC-0001", StringComparison.Ordinal);
        Assert.True(kick >= 0 && kick < bicycle && bicycle < scooter);
        Assert.DoesNotContain("VK-EBK-0001", beginners);
    }

    [Fact]
    public void DemoScenario_MissingFile_PrintsNoReports()
    {
        var writer = new StringWriter();
        using var provider = TestApp.Build(string.Empty, writer);

        provider.GetRequiredService<DemoScenario>().Run("нет-такого-файла.csv");

        Assert.Contains("Файл не найден", writer.ToString());
        Assert.DoesNotContain("Количество единиц", writer.ToString());
    }

    private static string Run(string input, FakeServiceCenter? serviceCenter = null)
    {
        var writer = new StringWriter();
        using var provider = TestApp.Build(input, writer, serviceCenter);

        provider.GetRequiredService<ConsoleMenu>().Run();
        return writer.ToString();
    }

    private static int CountOccurrences(string text, string value)
    {
        var count = 0;
        for (var i = text.IndexOf(value, StringComparison.Ordinal); i >= 0; i = text.IndexOf(value, i + 1, StringComparison.Ordinal))
        {
            count++;
        }

        return count;
    }
}
