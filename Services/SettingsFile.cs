namespace WukongBench.Services;

public class SettingsFile
{
    readonly string _initpath;
    readonly string _backupPath;

    public SettingsFile(string iniPath)
    {
        _initpath = iniPath;
        _backupPath = iniPath + ".bak";
    }

    public void Backup()
    {
        if (!File.Exists(_initpath))
            throw new InvalidOperationException("GameUserSettings.ini не найден. Запусти пж Benchmark Tool все там пройди и попробуй еще раз меня запустить");
        
        if (!File.Exists(_backupPath))
            File.Copy(_initpath, _backupPath);
    }

    public void Restore()
    {
        if (!File.Exists(_backupPath))
            return;
        
        // TODO(WIN): ПРОВЕРЬ ПЕРЕЗАПИСЫВАЕТ ЛИ ЭТА ХРЕНЬ INI ПРИ ЗАКРЫТИИ
        File.Copy(_backupPath, _initpath, overwrite: true);
        File.Delete(_backupPath);
    }
}