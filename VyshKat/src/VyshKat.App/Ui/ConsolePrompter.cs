using System.Globalization;

namespace VyshKat.App.Ui;

public sealed class ConsolePrompter
{
    private readonly TextReader _input;
    private readonly TextWriter _output;
    private readonly bool _strict;

    public ConsolePrompter(TextReader input, TextWriter output)
        : this(input, output, strict: false)
    {
    }

    private ConsolePrompter(TextReader input, TextWriter output, bool strict)
    {
        _input = input ?? throw new ArgumentNullException(nameof(input));
        _output = output ?? throw new ArgumentNullException(nameof(output));
        _strict = strict;
    }

    public static ConsolePrompter ForAnswers(IEnumerable<string> answers) =>
        new(new StringReader(string.Join('\n', answers)), TextWriter.Null, strict: true);

    internal bool HasUnreadInput => _input.Peek() != -1;

    public string AskText(string label)
    {
        while (true)
        {
            var answer = Ask(label);
            if (answer.Length > 0)
            {
                return answer;
            }

            Reject(label, answer, "Значение не может быть пустым.");
        }
    }

    public string AskOptionalText(string label) => Ask(label);

    public int AskInt(string label, int min, int max)
    {
        while (true)
        {
            var answer = Ask($"{label} [{min}–{max}]");
            if (int.TryParse(answer, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
                && value >= min && value <= max)
            {
                return value;
            }

            Reject(label, answer, $"Введите целое число от {min} до {max}.");
        }
    }

    public decimal AskDecimal(string label, decimal min, decimal max)
    {
        while (true)
        {
            var answer = Ask(label).Replace(',', '.');
            if (decimal.TryParse(answer, NumberStyles.Number, CultureInfo.InvariantCulture, out var value)
                && value >= min && value <= max)
            {
                return value;
            }

            Reject(label, answer,
                $"Введите число от {min.ToString(CultureInfo.InvariantCulture)} до {max.ToString(CultureInfo.InvariantCulture)}.");
        }
    }

    private string Ask(string label)
    {
        _output.Write($"{label}: ");
        var line = _input.ReadLine() ?? throw new EndOfInputException();
        return line.Trim();
    }

    private void Reject(string label, string answer, string hint)
    {
        if (_strict)
        {
            throw new FormatException($"{label}: недопустимое значение «{answer}». {hint}");
        }

        _output.WriteLine($"  {hint}");
    }
}
