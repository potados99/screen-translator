using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Ocr;

namespace ScreenTranslator.Lib.OCR
{
    internal class WindowsOcrResultProcessor
    {
        public OcrProcessedResult Process(OcrResult result)
        {
            var duplicatedStartPorints = result.Lines
                .Select(l => l.Words.First().BoundingRect.Left)
                .GroupBy(x => x)
                .Where(g => g.Count() > 1)
                .Select(y => y.Key)
                .ToList();

            var groupedLines = duplicatedStartPorints.Select(sp => result.Lines.Where(l => l.Words.First().BoundingRect.Left == sp));

            var paragraphsWithLines = groupedLines.Select(g => BuildParagraph(g));

            return new OcrProcessedResult
            {
                Paragraphs = paragraphsWithLines.ToList()
            };
        }

        public OcrProcessedParagraph BuildParagraph(IEnumerable<OcrLine> lines)
        {
            return new OcrProcessedParagraph
            {
                Lines = lines.Select(l => new OcrProcessedLine
                {
                    Text = l.Text,
                    Words = l.Words.Select(w => new OcrProcessedWord
                    {
                        Text = w.Text,
                        BoundingRect = w.BoundingRect.ToSystemDrawingRect()
                    }).ToList(),
                    BoundingRect = ContainingRect(l.Words.Select(w => w.BoundingRect.ToSystemDrawingRect()))
                }).ToList(),
                BoundingRect = ContainingRect(lines.SelectMany(l => l.Words.Select(w => w.BoundingRect.ToSystemDrawingRect())))
            };
        }

        public Rectangle ContainingRect(IEnumerable<Rectangle> rects)
        {
            var lefts = rects.Select(r => r.Left);
            var tops = rects.Select(r => r.Top);
            var rights = rects.Select(r => r.Right);
            var bottoms = rects.Select(r => r.Bottom);

            return Rectangle.FromLTRB(
                lefts.Min(),
                tops.Min(),
                rights.Max(),
                bottoms.Max()
                );
        }
    }
}
