using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using Windows.Globalization;
using Windows.Media.Ocr;

namespace WukongBench.Services;

public static class OcrDump
{
    public static async Task RunAsync()
    {
        ScreenCapture.EnableDpiAwareness();
        OcrEngine engine = ScreenText.CreateEngine();

        Console.WriteLine($"Язык OCR: {engine.RecognizerLanguage.LanguageTag}");
        Console.Write("Все доступные языки OCR:");
        foreach (Language language in OcrEngine.AvailableRecognizerLanguages)
            Console.Write($" {language.LanguageTag}");
        Console.WriteLine();
        Console.WriteLine($"Экран: {ScreenCapture.GetWidth()} x {ScreenCapture.GetHeight()}");

        string folder = Path.Combine(Directory.GetCurrentDirectory(), $"ocr_{DateTime.Now:yyyyMMdd_HHmmss}");
        Directory.CreateDirectory(folder);
        Console.WriteLine($"Сохраняю в: {folder}");

        for (int i = 1; i <= 60; i++)
        {
            using Bitmap bitmap = ScreenCapture.Capture();
            List<TextLine> lines = await ScreenText.ReadAsync(engine, bitmap);

            string name = $"{i:00}_{DateTime.Now:HHmmss}";
            bitmap.Save(Path.Combine(folder, name + ".jpg"), ImageFormat.Jpeg);

            StringBuilder sb = new StringBuilder();
            foreach (TextLine line in lines)
                sb.AppendLine($"[x={line.X:0} y={line.Y:0} w={line.Width:0} h={line.Height:0}] {line.Text}");
            File.WriteAllText(Path.Combine(folder, name + ".txt"), sb.ToString(), Encoding.UTF8);

            Console.WriteLine($"Снимок {name}: строк {lines.Count}");
            foreach (TextLine line in lines)
                Console.WriteLine($"    {line.Text}");

            await Task.Delay(TimeSpan.FromSeconds(5));
        }

        Console.WriteLine("Готово.");
    }
}