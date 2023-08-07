using System.Linq;
using Windows.Media.Ocr;

namespace ScreenTranslator.Lib.OCR;

public static class OcrLineExtensions
{
    public static System.Drawing.Rectangle ContainingRect(this OcrLine line)
    {
        return line.Words.Select(w => w.BoundingRect.ToSystemDrawingRect()).ContainingRect();
    }
}