using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.Globalization;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage;
using Windows.Storage.Streams;
using Color = System.Drawing.Color;

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
            var screenLeft = SystemParameters.VirtualScreenLeft;
            var screenTop = SystemParameters.VirtualScreenTop;
            var screenWidth = SystemParameters.VirtualScreenWidth;
            var screenHeight = SystemParameters.VirtualScreenHeight;

            using var bitmap = new Bitmap((int) screenWidth, (int) screenHeight);
            using var g = Graphics.FromImage(bitmap);
            g.CopyFromScreen((int) screenLeft, (int) screenTop, 0, 0, bitmap.Size);

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
            foreach (var line in Result.Lines)
            {
                foreach (var word in line.Words)
                {
                    g.DrawString(
                        word.Text,
                        new Font("Arial", 14),
                        new SolidBrush(Color.Yellow),
                        (float)word.BoundingRect.X, 
                        (float)word.BoundingRect.Y
                    );
                }
            }
        }
    }
}