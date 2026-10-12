using System.Diagnostics;
using WukongBench.Services;
using Windows.Media.Ocr;

Console.OutputEncoding = System.Text.Encoding.UTF8;

if (args.Length > 0 && args[0] == "--ocr-dump")
{
    await OcrDump.RunAsync();
    return;
}

ScreenCapture.EnableDpiAwareness();
OcrEngine engine = ScreenText.CreateEngine();

BenchmarkInstall install = Locator.Find();
Console.WriteLine($"Benchmark Tool найден: {install.Root}");

SettingsProfile cpuProfile = Profiles.Cpu();
SettingsProfile gpuProfile = Profiles.Gpu();

SettingsFile settings = new SettingsFile(install.IniPath);
settings.Backup();

try
{
    BenchmarkResult cpuResult = await RunTestAsync(cpuProfile, install, engine);
    BenchmarkResult gpuResult = await RunTestAsync(gpuProfile, install, engine);

    string report = ReportBuilder.Build(cpuResult, cpuProfile, gpuResult, gpuProfile);
    Console.WriteLine();
    Console.WriteLine(report);
    Console.WriteLine($"Отчёт сохранён: {ReportBuilder.Save(report)}");
}
catch (Exception ex)
{
    Console.WriteLine($"ОШИБКА ({ex.GetType().Name}): {ex.Message}");
}
finally
{
    settings.Restore();
    Console.WriteLine("Исходные настройки восстановлены.");
}

Console.WriteLine();
Console.WriteLine("Нажми Enter, чтобы закрыть окно...");
Console.ReadLine();

static async Task<BenchmarkResult> RunTestAsync(SettingsProfile profile, BenchmarkInstall install, OcrEngine engine)
{
    Console.WriteLine($"[{profile.Name}] применяю настройки...");
    profile.Apply(install.IniPath);

    ResultWatcher watcher = new ResultWatcher();
    watcher.Snapshot();

    Console.WriteLine($"[{profile.Name}] запускаю Benchmark Tool...");
    ProcessRunner.Start(install.ExePath);
    WindowHelper.MinimizeConsole();

    try
    {
        Console.WriteLine($"[{profile.Name}] пропускаю заставку и жду меню...");
        Func<string, bool> isMenu = text => text.StartsWith("Тест быстродейств", StringComparison.Ordinal);

        double cornerX = (double)MenuPoints.CornerOffsetPx / ScreenCapture.GetWidth();
        double cornerY = (double)MenuPoints.CornerOffsetPx / ScreenCapture.GetHeight();

        bool menu = false;
        for (int i = 0; i < 60 && !menu; i++)
        {
            Mouse.Click(cornerX, cornerY);
            menu = await ScreenClicker.WaitAndClickAsync(engine, isMenu, TimeSpan.FromSeconds(3));
        }

        if (!menu)
        {
            ScreenClicker.PrintLastLines();
            throw new TimeoutException("Главное меню так и не появилось.");
        }

        Console.WriteLine($"[{profile.Name}] жду окно подтверждения...");
        if (!await ScreenClicker.WaitAndClickAsync(engine,
                text => text.StartsWith("Подтверд", StringComparison.Ordinal),
                TimeSpan.FromSeconds(15)))
        {
            Console.WriteLine($"[{profile.Name}] кнопку не распознал, кликаю по координатам");
            ScreenClicker.PrintLastLines();
            Mouse.Click(MenuPoints.ConfirmX, MenuPoints.ConfirmY);
        }

        Console.WriteLine($"[{profile.Name}] жду результат (до 10 минут)...");
        BenchmarkResult result = watcher.WaitForResult(TimeSpan.FromMinutes(10));

        Console.WriteLine($"[{profile.Name}] готово: avg FPS = {result.FpsAvg}");
        return result;
    }
    finally
    {
        ProcessRunner.StopAll();
        WindowHelper.RestoreConsole();
    }
}