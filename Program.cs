using System.Diagnostics;
using System.Runtime.Versioning;
using WukongBench.Services;

[assembly: SupportedOSPlatform("windows")]

Console.OutputEncoding = System.Text.Encoding.UTF8;

BenchmarkInstall install = Locator.Find();
Console.WriteLine($"Benchmark Tool найден: {install.Root}");

SettingsProfile cpuProfile = Profiles.Cpu();
SettingsProfile gpuProfile = Profiles.Gpu();

SettingsFile settings = new SettingsFile(install.IniPath);
settings.Backup();

try
{
    BenchmarkResult cpuResult = RunTest(cpuProfile, install);
    BenchmarkResult gpuResult = RunTest(gpuProfile, install);

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

static BenchmarkResult RunTest(SettingsProfile profile, BenchmarkInstall install)
{
    Console.WriteLine($"[{profile.Name}] применяю настройки...");
    profile.Apply(install.IniPath);

    ResultWatcher watcher = new ResultWatcher();
    watcher.Snapshot();

    Console.WriteLine($"[{profile.Name}] запускаю Benchmark Tool...");
    Process process = ProcessRunner.Start(install.ExePath);

    try
    {
        // TODO(WIN): вместо фиксированной паузы ждать появления окна и меню.
        Console.WriteLine($"[{profile.Name}] жду загрузки меню...");
        Thread.Sleep(TimeSpan.FromSeconds(30));

        Console.WriteLine($"[{profile.Name}] нажимаю \"Тест быстродействия\"...");
        Mouse.Click(MenuPoints.BenchmarkX, MenuPoints.BenchmarkY);

        // TODO(WIN): подобрать паузу до появления окна подтверждения.
        Thread.Sleep(TimeSpan.FromSeconds(2));

        Console.WriteLine($"[{profile.Name}] подтверждаю запуск...");
        Mouse.Click(MenuPoints.ConfirmX, MenuPoints.ConfirmY);

        Console.WriteLine($"[{profile.Name}] жду результат (до 10 минут)...");
        BenchmarkResult result = watcher.WaitForResult(TimeSpan.FromMinutes(10));

        Console.WriteLine($"[{profile.Name}] готово: avg FPS = {result.FpsAvg}");
        return result;
    }
    finally
    {
        ProcessRunner.Stop(process);
    }
}