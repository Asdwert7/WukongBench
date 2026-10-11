using System.Diagnostics;

namespace WukongBench.Services;

public static class ProcessRunner
{
    // TODO(WIN): ПРОВЕРЬ КАКОЙ exe ЗАПУСКАТЬ 
    // TODO(WIN): ЕСЛИ ПРОЦУСС СРАЗУ ЗАВЕРШАЕТСЯ ТО ЗАПУСКАТЬ ЧЕРЕЗ steam://rungameid/3132990 И ИСКАТЬ ПРОЦЕСС ПО ИМЕНИ

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
    public static void Stop(Process process)
    {
        if (process.HasExited)
            return;
        
        process.Kill(entireProcessTree: true);
        process.WaitForExit();
    }
    
}