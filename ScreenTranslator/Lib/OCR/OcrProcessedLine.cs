using System.Collections.Generic;
using System.Drawing;

namespace ScreenTranslator.Lib.OCR
{
    public class OcrProcessedLine
    {
        public IReadOnlyList<OcrProcessedWord> Words { get; init; }

        public Rectangle BoundingRect { get; init; }

        public string Text { get; init; }
    }
}
