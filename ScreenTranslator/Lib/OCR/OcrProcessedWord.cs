using System.Drawing;

namespace ScreenTranslator.Lib.OCR
{
    internal class OcrProcessedWord
    {
        public Rectangle BoundingRect { get; init; }

        public string Text { get; init; }
    }
}
