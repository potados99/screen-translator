using System.Collections.Generic;
using System.Drawing;

namespace ScreenTranslator.Lib.OCR
{
    internal class OcrProcessedLine
    {
        public IReadOnlyList<OcrProcessedWord> Words { get; init; }

        public Rectangle BoundingRect { get; init; }

        public string Text { get; init; }
    }
}
