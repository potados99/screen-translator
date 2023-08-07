using System;
using ScreenTranslator.Lib.OCR;

namespace ScreenTranslator.Lib.Drawing;

public class Drawer
{
    public Drawer(System.Windows.Media.Visual visual)
    {
        Visual = visual;
    }

    private System.Windows.Media.Visual Visual { get; }

    public void Draw(OcrProcessedResult result)
    {
        var g = System.Drawing.Graphics.FromHwnd(IntPtr.Zero);
        var fontFamily = "Arial";

        foreach (var paragraph in result.Paragraphs)
        {
            FillRect(g, paragraph.BoundingRect, System.Drawing.Color.FromArgb(128, 0, 255, 0));

            foreach (var line in paragraph.Lines)
            {
                FillRect(g, line.BoundingRect, System.Drawing.Color.FromArgb(128, 0, 0, 255));

                foreach (var word in line.Words)
                {
                    FillRect(g, word.BoundingRect, System.Drawing.Color.FromArgb(128, 255, 0, 0));

                    var text = word.Text;
                    var rect = word.BoundingRect;

                    var fontSize = GetProperFontSize(text, (int) rect.Height, fontFamily);
                    System.Windows.Forms.TextRenderer.DrawText(
                        g,
                        text,
                        new System.Drawing.Font(fontFamily, fontSize),
                        new System.Drawing.Point((int) rect.X, (int) rect.Y),
                        System.Drawing.Color.Yellow
                    );
                }
            }
        }
    }

    private void FillRect(System.Drawing.Graphics g, System.Drawing.Rectangle rect, System.Drawing.Color color)
    {
        g.FillRectangle(
            new System.Drawing.SolidBrush(color),
            new System.Drawing.Rectangle(
                rect.X,
                rect.Y,
                rect.Width,
                rect.Height)
        );
    }

    private int GetProperFontSize(string text, int designatedHeight, string typeface)
    {
        var maxFontSize = 1;
        var ft = new System.Windows.Media.FormattedText(
            text,
            System.Globalization.CultureInfo.CurrentCulture,
            System.Windows.FlowDirection.LeftToRight,
            new System.Windows.Media.Typeface(typeface),
            maxFontSize,
            System.Windows.Media.Brushes.Black,
            System.Windows.Media.VisualTreeHelper.GetDpi(this.Visual).PixelsPerDip
        );
        while (true)
        {
            ft.SetFontSize(maxFontSize);
            if (ft.Height > designatedHeight)
            {
                //Too large! Maxmimum size found one step before
                maxFontSize--;
                break;
            }

            maxFontSize++;
        }

        return Math.Max(maxFontSize - 3, 1);
    }
}