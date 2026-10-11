using VyshKat.App.Ui;

namespace VyshKat.Tests.App;

public class ConsolePrompterTests
{
    [Theory]
    [InlineData("0,5", 0.5)]
    [InlineData("0.5", 0.5)]
    [InlineData(" 12 ", 12)]
    public void AskDecimal_AcceptsCommaAndDot(string answer, double expected)
    {
        var prompter = new ConsolePrompter(new StringReader(answer), new StringWriter());

        Assert.Equal((decimal)expected, prompter.AskDecimal("кВт·ч", 0m, 100m));
    }

    [Fact]
    public void AskInt_RepeatsUntilValueInRange()
    {
        var output = new StringWriter();
        var prompter = new ConsolePrompter(new StringReader(TestApp.Lines("0", "11", "x", "6")), output);

        Assert.Equal(6, prompter.AskInt("Простота", 1, 10));
        Assert.Equal(3, output.ToString().Split("Введите целое число от 1 до 10.").Length - 1);
    }

    [Fact]
    public void AskText_TrimsAnswer()
    {
        var prompter = new ConsolePrompter(new StringReader("  Модель  "), new StringWriter());

        Assert.Equal("Модель", prompter.AskText("Модель"));
    }

    [Fact]
    public void EndOfInput_Throws()
    {
        var prompter = new ConsolePrompter(new StringReader(string.Empty), new StringWriter());

        Assert.Throws<EndOfInputException>(() => prompter.AskText("Модель"));
    }

    [Fact]
    public void AskOptionalText_AllowsEmptyAnswer()
    {
        var prompter = new ConsolePrompter(new StringReader(TestApp.Lines("")), new StringWriter());

        Assert.Equal(string.Empty, prompter.AskOptionalText("Путь"));
    }

    [Fact]
    public void ForAnswers_ReadsAnswersInOrder()
    {
        var prompter = ConsolePrompter.ForAnswers(["Модель", "95", "0,5"]);

        Assert.Equal("Модель", prompter.AskText("Модель"));
        Assert.Equal(95, prompter.AskInt("Состояние", 0, 100));
        Assert.Equal(0.5m, prompter.AskDecimal("кВт·ч", 0m, 10m));
    }

    [Theory]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("101")]
    public void ForAnswers_InvalidValue_ThrowsInsteadOfAskingAgain(string answer)
    {
        var prompter = ConsolePrompter.ForAnswers([answer, "50"]);

        Assert.Throws<FormatException>(() => prompter.AskInt("Состояние", 0, 100));
    }
}
