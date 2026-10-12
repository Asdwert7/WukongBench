using System.Drawing;
using Windows.Media.Ocr;

namespace WukongBench.Services;

public static class ScreenClicker
{
    // Че OCR видел для диагностики 
    private static List<TextLine> _lastLines = new List<TextLine>();

    // true — нашёл и кликнул, false — не дождался за timeout.
    public static async Task<bool> WaitAndClickAsync(OcrEngine engine, Func<string, bool> match, TimeSpan timeout)
    {
        DateTime deadline = DateTime.Now + timeout;

        while (DateTime.Now < deadline)
        {
            using Bitmap bitmap = ScreenCapture.Capture();
            List<TextLine> lines = await ScreenText.ReadAsync(engine, bitmap);
            _lastLines = lines;

            foreach (TextLine line in lines)
            {
                if (match(line.Text.Trim()))
                {
                    double x = (line.X + line.Width / 2) / bitmap.Width;
                    double y = (line.Y + line.Height / 2) / bitmap.Height;

                    // Текст мог только начать проявляться — даём экрану секунду "ожить"
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    Console.WriteLine($"    OCR нашёл \"{line.Text}\", кликаю");
                    Mouse.Click(x, y);
                    return true;
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(1));
        }

        return false;
    }
    public static void PrintLastLines()
    {
        Console.WriteLine("    Что OCR видел на экране:");
        foreach (TextLine line in _lastLines)
            Console.WriteLine($"      [{line.X:0},{line.Y:0}] {line.Text}");
    }
}
//StartsWith(..., StringComparison.Ordinal) — строгое посимвольное сравнение с