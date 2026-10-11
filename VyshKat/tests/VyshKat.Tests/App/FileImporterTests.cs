using Microsoft.Extensions.DependencyInjection;
using VyshKat.App.Ui;
using VyshKat.Core.Fleet;

namespace VyshKat.Tests.App;

public class FileImporterTests
{
    [Fact]
    public void ValidLines_GoThroughServiceCenter_AndGetInventoryNumbers()
    {
        var fake = new FakeServiceCenter(pass: true);
        var path = TestApp.CsvFile(
            "SC;Kick;90;9",
            "BK;Classic;90;8",
            "ESC;E-Lite;95;7;0.45",
            "EBK;Volt;90;5;0.6",
            "HLM;Шлем;100;m",
            "DCK;Док;100;10;2.4",
            "CHG;Шкаф;97;20;6");

        var (output, provider) = Import(path, fake);

        Assert.Equal(7, fake.Inspected.Count);
        Assert.Equal(7, provider.GetRequiredService<IFleetRepository>().GetAll().Count);
        Assert.Contains("[ПРИНЯТ] VK-SC-0001", output);
        Assert.Contains("[ПРИНЯТ] VK-CHG-0001", output);
    }

    [Fact]
    public void CommentsAndBlankLines_AreSkipped()
    {
        var path = TestApp.CsvFile("# комментарий", "", "   ", "BK;Classic;90;8");

        var (output, provider) = Import(path);

        Assert.Single(provider.GetRequiredService<IFleetRepository>().GetAll());
        Assert.DoesNotContain("ОШИБКА", output);
    }

    [Fact]
    public void KindCode_IsCaseInsensitive()
    {
        var (output, _) = Import(TestApp.CsvFile("esc;E-Lite;95;7;0.45"));

        Assert.Contains("[ПРИНЯТ] VK-ESC-0001", output);
    }

    [Theory]
    [InlineData("MONO;Моноколесо;90;5;0.3", "Неизвестный вид «MONO»")]
    [InlineData("BK;Classic;90;11", "Простота для новичка: недопустимое значение «11»")]
    [InlineData("ESC;E-Lite;95;7;много", "Суточный расход энергии, кВт·ч: недопустимое значение «много»")]
    [InlineData("SC;Kick;90", "Не хватает полей.")]
    [InlineData("SC;Kick;90;9;лишнее", "Лишние поля в строке.")]
    [InlineData("HLM;;100;M", "Модель: недопустимое значение")]
    public void BrokenLine_IsReportedWithLineNumber_AndNotAccepted(string line, string expected)
    {
        var (output, provider) = Import(TestApp.CsvFile("# заголовок", line));

        Assert.Contains($"[ОШИБКА, строка 2] {expected}", output);
        Assert.Empty(provider.GetRequiredService<IFleetRepository>().GetAll());
    }

    [Fact]
    public void BrokenLine_DoesNotStopTheRestOfTheFile()
    {
        var path = TestApp.CsvFile("BK;Classic;90;11", "BK;Classic;90;8");

        var (output, provider) = Import(path);

        Assert.Contains("[ОШИБКА, строка 1]", output);
        Assert.Single(provider.GetRequiredService<IFleetRepository>().GetAll());
    }

    [Fact]
    public void MissingFile_ReturnsFalse()
    {
        var writer = new StringWriter();
        using var provider = TestApp.Build(string.Empty, writer);

        var imported = provider.GetRequiredService<FileImporter>().Import("нет-такого-файла.csv");

        Assert.False(imported);
        Assert.Contains("Файл не найден", writer.ToString());
    }

    private static (string Output, ServiceProvider Provider) Import(string path, FakeServiceCenter? serviceCenter = null)
    {
        var writer = new StringWriter();
        var provider = TestApp.Build(string.Empty, writer, serviceCenter);

        Assert.True(provider.GetRequiredService<FileImporter>().Import(path));
        return (writer.ToString(), provider);
    }
}
