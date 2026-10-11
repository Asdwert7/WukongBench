namespace WukongBench.Services;


public class BenchmarkInstall
{
    public string Root {get;set;}
    public string ExePath {get;set;}
    public string IniPath {get;set;}

    public BenchmarkInstall(string root,string exePath, string iniPath)
    {
        Root = root;
        ExePath = exePath;
        IniPath = iniPath;
    }
}