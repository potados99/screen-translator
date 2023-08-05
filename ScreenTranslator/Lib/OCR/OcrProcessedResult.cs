using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenTranslator.Lib.OCR
{
    internal class OcrProcessedResult
    {
        public IReadOnlyList<OcrProcessedParagraph> Paragraphs { get; set; }
    }
}
