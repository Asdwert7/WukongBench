namespace WukongBench.Services;

public class BenchmarkResult
{
    public double FpsAvg { get; set; }
    public double FpsMin { get; set; }
    public double FpsMax { get; set; }
    public double Fps95 { get; set; }

    public string CpuModel { get; set; } = "";
    public string GpuModel { get; set; } = "";
    public string GpuDriver { get; set; } = "";
    public string VideoMemSize { get; set; } = "";
    public string SysMem { get; set; } = "";
    public string SysVer { get; set; } = "";
    public string ScreenResolution { get; set; } = "";

    public string RawJson { get; set; } = "";
}