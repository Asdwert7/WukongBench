namespace WukongBench.Services;

public static class Profiles
{
    // Пункты меню графики
    static readonly string[] UiKeys =
    {
        "ViewDistance", "AntiAliasing", "PostProcessing", "ShadowQuality", "TextureQuality",
        "FxQuality", "MaterialQuality", "VegetationQuality", "GlobalIllumination", "ReflectionQuality"
    };

    // то же но в ScalabilityGroups
    static readonly string[] SgKeys =
    {
        "ViewDistanceQuality", "AntiAliasingQuality", "PostProcessQuality", "ShadowQuality", "TextureQuality",
        "EffectsQuality", "ShadingQuality", "FoliageQuality", "GlobalIlluminationQuality", "ReflectionQuality"
    };

    public static SettingsProfile Cpu()
    {
        SettingsProfile p = CreateQuality("CPU", 1);
        p.Description = "Минимальная графика, трассировка и генерация кадров выключены -  нагрузка уходит в процессор";

        p.UISettings["Rtx"] = "0";

        // TODO(WIN): InsertFrame=0 у этого там 1 наверное 0 но надо проверить
        p.UISettings["InsertFrame"] = "0";

        // TODO(WIN): мин разрешение и мин масштаб заставить его показать
        return p;
    }

    public static SettingsProfile Gpu()
    {
        SettingsProfile p = CreateQuality("GPU", 5);
        p.Description = "Максимальная графика, трассировка и генерация кадров выключены -  нагрузка уходит в видеокарту";

        // Я вырублю если нет rtx то тест свалится
        p.UISettings["Rtx"] = "0";
        // TODO(WIN): InsertFrame=0
        p.UISettings["InsertFrame"] = "0";

        // TODO(WIN): родное разрешение, масштаб рендера 100%
        // и выключение апскейла (FSR).
        return p;
    }

    static SettingsProfile CreateQuality(string name, int level)
    {
        var profile = new SettingsProfile { Name = name };
        
        profile.UISettings["QualityLevel"] = level.ToString();
        foreach (string key in UiKeys)
            profile.UISettings[key] = level.ToString();
        
        foreach (string key in SgKeys)
            profile.Lines["sg." + key] = (level - 1).ToString(); // -1 потомучто по логам было на -1 меньше 

        return profile;
    }
}