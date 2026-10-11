using System.Text.RegularExpressions;

namespace WukongBench.Services;

public class IniEditor
{
    string _text;

    public IniEditor(string iniPath)
    {
        _text = File.ReadAllText(iniPath);
    }

    // меняет в UISettingData key и value
    public void SetUiSetting(string key, string value)
    {
        string pattern = $"\\(\"{Regex.Escape(key)}\", \"[^\"]*\"\\)";

        if (!Regex.IsMatch(_text, pattern))
            throw new InvalidOperationException($"Нет ключа {key} в UISettingData");
        
        _text = Regex.Replace(_text, pattern, $"(\"{key}\", \"{value}\")");
    }
    
    public void SetLine(string key, string value)
    {
        string pattern = $"^{Regex.Escape(key)}=[^\\r\\n]*";

        if (!Regex.IsMatch(_text, pattern, RegexOptions.Multiline))
            throw new InvalidOperationException($"нет в файле ini строки {key}=...");
        _text = Regex.Replace(_text, pattern, $"{key}={value}", RegexOptions.Multiline);
    }
    public void Save(string iniPath)
    {
        // TODO(WIN): проверить есть ли BOM для WriteAllText
        // TODO(WIN): нужны ли RayTracing и GSRenderSetting
        File.WriteAllText(iniPath,_text);
    }
}