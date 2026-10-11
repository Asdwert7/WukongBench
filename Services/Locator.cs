using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace WukongBench.Services;

public static class Locator
{
    const string AppId = "3132990";
    // Относительный путь
    static readonly string ExeRelativePath = Path.Combine("b1", "Binaries", "Win64", "b1-Win64-Shipping.exe");
    static readonly string IniRelativePath = Path.Combine("b1", "Saved", "Config", "Windows", "GameUserSettings.ini");

    static string GetSteamPath()
    {
        using RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam");
        string? path = (string?)key?.GetValue("SteamPath");

        if(string.IsNullOrEmpty(path))
            throw new InvalidOperationException("Не найден Стим!, Может установишь его?");
        return path.Replace('/', '\\');
    }
    // Steam может держать игры в нескольких папках, для этого libraryfolders.vdf смотрим 
    static List<string> GetLibraries(string steamPath)
    {
        var reZZZult = new List<string> {steamPath};

        string vdf = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf");

        if (File.Exists(vdf))
        {
            foreach(Match m in Regex.Matches(File.ReadAllText(vdf), "\"path\"\\s+\"([^\"]+)\""))// спасибо квен
            {
                string lib = m.Groups[1].Value.Replace(@"\\", @"\");
                if (!reZZZult.Contains(lib, StringComparer.OrdinalIgnoreCase))
                    reZZZult.Add(lib);
            }
        }
        return reZZZult;
    }
    static string? ReadValue(string text, string key)
    {
        Match m = Regex.Match(text, $"\"{key}\"\\s+\"([^\"]+)\"");
        return m.Success ? m.Groups[1].Value : null;
    }

    public static BenchmarkInstall Find()
    {
        string steamPath = GetSteamPath();


        foreach(string lib in GetLibraries(steamPath))
        {
            string manifest = Path.Combine(lib , "steamapps", $"appmanifest_{AppId}.acf");
            if (!File.Exists(manifest))
                continue;
            
            string? installDirectory = ReadValue(File.ReadAllText(manifest), "installdir");
            if (installDirectory == null)
                continue;
            
            string root = Path.Combine(lib, "steamapps", "common", installDirectory);
            if (Directory.Exists(root))
                return new BenchmarkInstall(root, Path.Combine(root, ExeRelativePath), Path.Combine(root,IniRelativePath));
        }
        throw new InvalidOperationException("Benchmark инструмент не найден. Установи в стим его");
    }
}








//https://steamdb.info/app/3132990/info/
/*
Идея: Начниаем со спика из одной папки, которая основная папка Steam, игра там может лежать так что придется проверить
libraryfolders.vdf - файл стима, в нем будет путь типа "path" "D:\\SteamLibrary"
потом проверяем места, подходяшие под шаблон, возвращаем коллецию совпадений

Шаблон мне квен подсказал: 
- \s+ — один или больше пробелов или табов. В файле между ключом и значением стоят табы.
- " — открывающая кавычка значения.
- ([^"]+) — скобки запоминают то, что внутри. Внутри: любые символы, кроме кавычки, один или больше раз. Это и есть путь.



*/