using System.Text;

namespace WukongBench.Services;

public static class ReportBuilder
{
    public static string Build(BenchmarkResult cpu, SettingsProfile cpuProfile,
                               BenchmarkResult gpu, SettingsProfile gpuProfile)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("ХАРАКТЕРИСТИКИ ПК");
        sb.AppendLine($"Процессор:   {cpu.CpuModel}");
        sb.AppendLine($"Видеокарта:  {gpu.GpuModel}");
        sb.AppendLine($"Драйвер:     {gpu.GpuDriver}");
        sb.AppendLine($"Видеопамять: {gpu.VideoMemSize}");
        sb.AppendLine($"ОЗУ:         {cpu.SysMem}");
        sb.AppendLine($"Система:     {cpu.SysVer}");
        sb.AppendLine($"Экран:       {cpu.ScreenResolution}");

        AppendTest(sb, "ТЕСТ CPU", cpu, cpuProfile);
        AppendTest(sb, "ТЕСТ GPU", gpu, gpuProfile);

        return sb.ToString();
    }
    public static string Save(string text)
    {
        string fileName = $"report_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
        string path = Path.Combine(Directory.GetCurrentDirectory(), fileName);
        File.WriteAllText(path, text, Encoding.UTF8);
        return path;
    }

    private static void AppendTest(StringBuilder sb, string title, BenchmarkResult result, SettingsProfile profile)
    {
        sb.AppendLine();
        sb.AppendLine($"{title}");
        sb.AppendLine($"Профиль: {profile.Name} ({profile.Description})");
        sb.AppendLine($"FPS средний: {result.FpsAvg}");
        sb.AppendLine($"FPS мин:     {result.FpsMin}");
        sb.AppendLine($"FPS макс:    {result.FpsMax}");
        sb.AppendLine($"FPS 95%:     {result.Fps95}");
        sb.AppendLine("Настройки:");

        foreach (KeyValuePair<string, string> pair in profile.UISettings)
            sb.AppendLine($"  {pair.Key.PadRight(20)} {pair.Value}");
    }
}

// Собрано клаудом