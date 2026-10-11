using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VyshKat.App;
using VyshKat.Core.Inspection;

namespace VyshKat.Tests.App;

internal static class TestApp
{
    public static ServiceProvider Build(string input, TextWriter output, IServiceCenter? serviceCenter = null)
    {
        var services = new ServiceCollection().AddVyshKat(new StringReader(input), output);

        if (serviceCenter is not null)
        {
            services.Replace(ServiceDescriptor.Singleton<IServiceCenter>(serviceCenter));
        }

        return services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
    }

    public static string CsvFile(params string[] lines)
    {
        var path = Path.Combine(Path.GetTempPath(), $"vyshkat-{Guid.NewGuid():N}.csv");
        File.WriteAllLines(path, lines);
        return path;
    }

    public static string Lines(params string[] lines) => string.Join(Environment.NewLine, lines) + Environment.NewLine;
}
