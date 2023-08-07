using System.Collections.Generic;
using System.Drawing;

namespace ScreenTranslator.Lib.OCR
{
    internal class OcrProcessedParagraph
    {
        public IReadOnlyList<OcrProcessedLine> Lines { get; init; }

        public Rectangle BoundingRect { get; init; }
    }
}
