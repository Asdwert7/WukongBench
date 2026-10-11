using System.Text.Json;

namespace WukongBench.Services;

public class ResultWatcher
{
    readonly string _folder;
    readonly HashSet<string> _knownFiles = new HashSet<string>();

    public ResultWatcher()
    {
        _folder = Path.Combine(Path.GetTempPath(), "b1", "BenchMarkHistory", "Tool");
    }

    // До запуска
    public void Snapshot()
    {
        _knownFiles.Clear();

        if(!Directory.Exists(_folder))
            return;
        foreach (string file in Directory.GetFiles(_folder))
            _knownFiles.Add(file);
    }

    // ждем файл и разбираем
    public BenchmarkResult WaitForResult(TimeSpan timeout)
    {
        DateTime deadline = DateTime.Now + timeout;
        while (DateTime.Now < deadline)
        {
            string? newFile = MyFindNewFile(); // my func
            if (newFile != null)
            {
                BenchmarkResult? rezzz = MyTryParse(newFile);
                if (rezzz != null)
                    return rezzz;
            }
            Thread.Sleep(2000);
        }
        throw new TimeoutException($"Чет не появился результат, прошло {timeout.TotalMinutes} мин");
    }

    string? MyFindNewFile()
    {
        if (!Directory.Exists(_folder))
            return null;
        foreach(string f in Directory.GetFiles(_folder))
        {
            if (!_knownFiles.Contains(f))
                return f;
        }
        return null;
    }
    BenchmarkResult? MyTryParse(string f)
    {
        try
        {
            return ResultParser.ParseFile(f);
        }
        catch(IOException)
        {
            return null; // файл занят
        }
        catch (JsonException)
        {
            return null; // не доконца записан фвайл 
        }
    }
}