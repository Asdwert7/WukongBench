namespace WukongBench.Services;


public class SettingsProfile
{
    public string Name {get; set;} = "";
    public string Description {get; set;} = "";

    // Значения внутри UISettingData
    public Dictionary<string,string> UISettings {get;} = new ();

    // Обычные строки типа как пример sg.ShadowQuality
    public Dictionary<string,string> Lines {get;} = new ();

    public void Apply(string iniPath)
    {
        var editor = new IniEditor(iniPath);

        foreach(var pair in UISettings)
            editor.SetUiSetting(pair.Key, pair.Value);
        
        foreach (var pair in Lines)
            editor.SetLine(pair.Key,pair.Value);
        
        editor.Save(iniPath);
    }
}
