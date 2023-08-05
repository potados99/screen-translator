using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
