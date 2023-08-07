using System.Collections.Generic;

namespace ScreenTranslator.Lib.OCR
{
    public class OcrProcessedResult
    {
        public IReadOnlyList<OcrProcessedParagraph> Paragraphs { get; set; }
    }
}
