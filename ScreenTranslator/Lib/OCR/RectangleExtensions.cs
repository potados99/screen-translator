using System.Collections.Generic;
using System.Linq;

namespace ScreenTranslator.Lib.OCR
{
    public static class RectangleExtensions
    {
        public static System.Drawing.Rectangle ToSystemDrawingRect(this Windows.Foundation.Rect rect)
        {
            return new System.Drawing.Rectangle(
                (int)rect.X,
                (int)rect.Y,
                (int)rect.Width,
                (int)rect.Height
                );
        }

        public static System.Drawing.Rectangle ContainingRect(this IEnumerable<System.Drawing.Rectangle> rects)
        {
            var lefts = rects.Select(r => r.Left);
            var tops = rects.Select(r => r.Top);
            var rights = rects.Select(r => r.Right);
            var bottoms = rects.Select(r => r.Bottom);

            return System.Drawing.Rectangle.FromLTRB(
                lefts.Min(),
                tops.Min(),
                rights.Max(),
                bottoms.Max()
            );
        }
    }
}
