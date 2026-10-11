namespace VyshKat.App.Ui;

public sealed class EndOfInputException : Exception
{
    public EndOfInputException()
        : base("Ввод завершён.")
    {
    }
}
