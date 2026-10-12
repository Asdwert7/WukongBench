using System.Text.Json;

namespace WukongBench.Services;

public static class ResultParser
{
    private static readonly string[] SettingKeys =
    {
        "QualityLevel", "ImageQuality", "ViewDistance", "AntiAliasing", "PostProcessing",
        "ShadowQuality", "TextureQuality", "MaterialQuality", "VegetationQuality",
        "MotionBlur", "Rtx", "Dlss", "InsertFrame", "Dx12"
    };

    public static BenchmarkResult ParseFile(string path)
    {
        return Parse(File.ReadAllText(path));
    }

    public static BenchmarkResult Parse(string json)
    {
        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;

        BenchmarkResult result = new BenchmarkResult
        {
            FpsAvg = ReadDouble(root, "FPSAvg"),
            FpsMin = ReadDouble(root, "FPSMin"),
            FpsMax = ReadDouble(root, "FPSMax"),
            Fps95 = ReadDouble(root, "FPS95"),

            CpuModel = ReadString(root, "CPUModel"),
            GpuModel = ReadString(root, "GPUModel"),
            GpuDriver = ReadString(root, "GpuDriverVer"),
            VideoMemSize = ReadString(root, "VideoMemSize"),
            SysMem = ReadString(root, "SysMem"),
            SysVer = ReadString(root, "SysVer"),
            ScreenResolution = ReadString(root, "ScreenResolution"),

            RawJson = json
        };

        foreach (string key in SettingKeys)
            result.Settings[key] = ReadString(root, key);

        return result;
    }

    static double ReadDouble(JsonElement root, string name)
    {
        if (!root.TryGetProperty(name, out JsonElement value))
            throw new InvalidOperationException($"В результатах нет поля {name}.");

        return value.GetDouble();
    }

    // Если поля нет чтоб не упало я заставил просто вернуть понятную надпись, ну типа неизвестно лучше чем падение
    static string ReadString(JsonElement root, string name)
    {
        if (!root.TryGetProperty(name, out JsonElement value))
            return "неизвестно";

        return value.ToString();
    }
}