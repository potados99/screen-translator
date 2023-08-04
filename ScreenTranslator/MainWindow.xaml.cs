using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.Globalization;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage;
using Windows.Storage.Streams;
using Brushes = System.Windows.Media.Brushes;
using Font = System.Drawing.Font;
using Color = System.Drawing.Color;
using FlowDirection = System.Windows.FlowDirection;
using FontFamily = System.Drawing.FontFamily;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace ScreenTranslator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>  
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Read().ContinueWith(_ => Draw());
        }

        private OcrResult Result;

        private async Task<SoftwareBitmap> GetScreenshot()
        {
            var screenLeft = 0;
            var screenTop = 0;
            var screenWidth = Screen.PrimaryScreen.Bounds.Width;
            var screenHeight = Screen.PrimaryScreen.Bounds.Height;

            using var bitmap = new Bitmap((int) screenWidth, (int) screenHeight);
            using var g = Graphics.FromImage(bitmap);
            g.CopyFromScreen((int) screenLeft, (int) screenTop, 0, 0, bitmap.Size);

            // bitmap.Save("Capture.png", ImageFormat.Png);

            using var stream = new InMemoryRandomAccessStream();
            bitmap.Save(stream.AsStream(),
                ImageFormat.Jpeg); //choose the specific image format by your own bitmap source
            var decoder = await BitmapDecoder.CreateAsync(stream);
            return await decoder.GetSoftwareBitmapAsync();
        }

        private async Task Read()
        {
            var language = new Language("ko");
            if (!OcrEngine.IsLanguageSupported(language))
            {
                throw new Exception($"{language.LanguageTag} is not supported in this system.");
            }

            var bitmap = await GetScreenshot();
            var engine = OcrEngine.TryCreateFromLanguage(language);
            var ocrResult = await engine.RecognizeAsync(bitmap).AsTask();

            Result = ocrResult;
        }


        private void Draw()
        {
            var g = Graphics.FromHwnd(IntPtr.Zero);
            var fontFamily = "Arial";

            foreach (var line in Result.Lines)
            {
                foreach (var word in line.Words)
                {
                    var text = word.Text;
                    var rect = word.BoundingRect;

                    g.FillRectangle(
                        new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(128, 0, 0, 0)),
                        new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height)
                    );

                    var fontSize = GetProperFontSize(text, (int)rect.Height, fontFamily);
                    TextRenderer.DrawText(
                        g,
                        text,
                        new Font(fontFamily, fontSize),
                        new Point((int)rect.X, (int)rect.Y),
                        Color.Yellow
                    );
                }
            }
        }

        private int GetProperFontSize(string text, int designatedHeight, string typeface)
        {
            var maxFontSize = 1;
            var ft = new FormattedText(
                text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(typeface),
                maxFontSize,
                Brushes.Black,
                VisualTreeHelper.GetDpi(this).PixelsPerDip
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
                else
                {
                    maxFontSize++;
                }
            }

            return Math.Max(maxFontSize - 6, 1);
        }
    }
}