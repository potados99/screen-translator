using System.Linq;
using Windows.Media.Ocr;
using ScreenTranslator.Lib.OCR;

namespace ScreenTranslator.Lib.Extensions;

public static class OcrLineExtensions
{
    public static System.Drawing.Rectangle ContainingRect(this OcrLine line)
    {
        return line.Words.Select(w => w.BoundingRect.ToSystemDrawingRect()).ContainingRect();
    }
}