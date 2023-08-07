using System.Drawing;

namespace ScreenTranslator.Lib.OCR
{
    public class OcrProcessedWord
    {
        public Rectangle BoundingRect { get; init; }

        public string Text { get; init; }
    }
}
