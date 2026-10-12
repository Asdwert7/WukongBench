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

        SetResolution(p,1280,720,50);
        return p;
    }

    public static SettingsProfile Gpu()
    {
        SettingsProfile p = CreateQuality("GPU", 5);
        p.Description = "Максимальная графика, трассировка и генерация кадров выключены -  нагрузка уходит в видеокарту";

        // Я вырублю если нет rtx то тест свалится
        p.UISettings["Rtx"] = "0";
        p.UISettings["InsertFrame"] = "0";
        // Апскейлер: 0=FSR, 1=XeSS, 2=DLSS. Выключить нельзя, при масштабе 100% XeSS работает без апскейла.
        p.UISettings["SuperResolutionSampling"] = "1";

        SetResolution(p, ScreenCapture.GetWidth(), ScreenCapture.GetHeight(), 100);
        return p;
    }

    // outW x outH — "Разрешение экрана"; scale — масштаб рендера в процентах.
    // Разрешение рендера = выход * масштаб (так пишет тестер у меня сам).
    static void SetResolution(SettingsProfile p, int outW, int outH, int scale)
    {
        int renderW = outW * scale / 100;
        int renderH = outH * scale / 100;

        p.Lines["ResolutionSizeX"] = outW.ToString();
        p.Lines["ResolutionSizeY"] = outH.ToString();
        p.Lines["LastUserConfirmedResolutionSizeX"] = outW.ToString();
        p.Lines["LastUserConfirmedResolutionSizeY"] = outH.ToString();

        p.Lines["DesiredScreenWidth"] = renderW.ToString();
        p.Lines["DesiredScreenHeight"] = renderH.ToString();
        p.Lines["LastUserConfirmedDesiredScreenWidth"] = renderW.ToString();
        p.Lines["LastUserConfirmedDesiredScreenHeight"] = renderH.ToString();

        p.UISettings["ImageQuality"] = renderH.ToString();
        p.Lines["sg.ResolutionQuality"] = scale.ToString();
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