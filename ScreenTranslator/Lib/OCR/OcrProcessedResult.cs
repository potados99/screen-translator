using System.Collections.Generic;

namespace ScreenTranslator.Lib.OCR
{
    internal class OcrProcessedResult
    {
        public IReadOnlyList<OcrProcessedParagraph> Paragraphs { get; set; }
    }
}
