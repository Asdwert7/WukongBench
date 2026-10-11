using System.Runtime.Versioning;
using WukongBench.Services;

[assembly: SupportedOSPlatform("windows")]

BenchmarkInstall install = Locator.Find();

Console.WriteLine($"Папа: {install.Root}");
Console.WriteLine($"Exe: {install.ExePath} [{(File.Exists(install.ExePath) ? "ага, есть exe" : "НЕЕЕЕТ.exe")}]");
Console.WriteLine($"Ini: {install.IniPath} [{(File.Exists(install.IniPath) ? "ага, есть ini" : "НЕЕЕЕТ.ini")}]");


Console.WriteLine();


Console.WriteLine("Файлы .exe в корне папки: ");
foreach(string exe in Directory.GetFiles(install.Root, "*.exe"))
    Console.WriteLine($" {Path.GetFileName(exe)}");