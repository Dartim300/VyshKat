using System.Text;
using Microsoft.Extensions.DependencyInjection;
using VyshKat.App;
using VyshKat.App.Ui;

try
{
    // Кириллица в консоли Windows; ставится до того, как Console.Out попадёт в контейнер.
    Console.OutputEncoding = Encoding.UTF8;
}
catch (IOException)
{
    // Консоль не поддерживает смену кодировки — оставляем как есть.
}

using var provider = new ServiceCollection()
    .AddVyshKat(Console.In, Console.Out)
    .BuildServiceProvider();

if (!args.Contains("--no-demo", StringComparer.OrdinalIgnoreCase))
{
    var dataPath = args.FirstOrDefault(a => !a.StartsWith("--", StringComparison.Ordinal)) ?? DemoScenario.DefaultDataPath;
    provider.GetRequiredService<DemoScenario>().Run(dataPath);
}

provider.GetRequiredService<ConsoleMenu>().Run();
