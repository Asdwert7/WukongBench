using System.Diagnostics;

namespace WukongBench.Services;

public static class ProcessRunner
{
    public static Process Start(string exePath)
    {
        var info = new ProcessStartInfo
        {
            FileName = exePath,
            WorkingDirectory = Path.GetDirectoryName(exePath) ?? "",
            UseShellExecute = false
        };

        Process? process = Process.Start(info);
        if (process == null)
            throw new InvalidOperationException("Не удалось запустить вот: " + exePath);
        return process;
    }
    private static readonly string[] GameProcessNames = { "b1-Win64-Shipping", "b1_benchmark" };

    // Закрывает ВСЕ процессы игры по имени — неважно, кто их запустил (мы или Steam)
    public static void StopAll()
    {
        foreach (string name in GameProcessNames)
        {
            foreach (Process p in Process.GetProcessesByName(name))
            {
                try
                {
                    p.Kill(entireProcessTree: true);
                    p.WaitForExit(10000);
                }
                catch (Exception ex)
                {
                    // Процесс мог завершиться сам между поиском и Kill
                    Console.WriteLine($"Не удалось закрыть {name}: {ex.Message}");
                }
                finally
                {
                    p.Dispose();
                }
            }
        }
    }
    public static void Stop(Process process)
    {
        if (process.HasExited)
            return;
        
        process.Kill(entireProcessTree: true);
        process.WaitForExit();
    }
    
}