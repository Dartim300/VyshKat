namespace VyshKat.Core.Inspection;

public sealed record InspectionResult(bool Passed, string Reason)
{
    public static InspectionResult Pass(string reason = "Техосмотр пройден") => new(true, reason);

    public static InspectionResult Fail(string reason) => new(false, reason);
}
