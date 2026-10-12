using System.Drawing;
using System.Drawing.Imaging;
using Windows.Globalization;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage.Streams;

namespace WukongBench.Services;

public static class ScreenText
{
    public static OcrEngine CreateEngine()
    {
        OcrEngine? engine = OcrEngine.TryCreateFromLanguage(new Language("ru"));

        if (engine == null)
            engine = OcrEngine.TryCreateFromUserProfileLanguages();

        if (engine == null)
            throw new InvalidOperationException("OCR недоступен: ЯЗЫК МЕНЯЙ!!!! на РУССКИЙ🇷🇺");

        return engine;
    }

    public static async Task<List<TextLine>> ReadAsync(OcrEngine engine, Bitmap bitmap)
    {
        SoftwareBitmap softwareBitmap = await ToSoftwareBitmapAsync(bitmap);
        OcrResult result = await engine.RecognizeAsync(softwareBitmap);

        List<TextLine> lines = new List<TextLine>();

        foreach (OcrLine line in result.Lines)
        {
            double left = double.MaxValue;
            double top = double.MaxValue;
            double right = 0;
            double bottom = 0;

            foreach (OcrWord word in line.Words)
            {
                left = Math.Min(left, word.BoundingRect.X);
                top = Math.Min(top, word.BoundingRect.Y);
                right = Math.Max(right, word.BoundingRect.X + word.BoundingRect.Width);
                bottom = Math.Max(bottom, word.BoundingRect.Y + word.BoundingRect.Height);
            }

            lines.Add(new TextLine
            {
                Text = line.Text,
                X = left,
                Y = top,
                Width = right - left,
                Height = bottom - top
            });
        }

        return lines;
    }

    private static async Task<SoftwareBitmap> ToSoftwareBitmapAsync(Bitmap bitmap)
    {
        using MemoryStream memory = new MemoryStream();
        bitmap.Save(memory, ImageFormat.Png);
        memory.Position = 0;

        using IRandomAccessStream stream = memory.AsRandomAccessStream();
        BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
        return await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
    }
}