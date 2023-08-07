using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Windows.Globalization;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage.Streams;
using ScreenTranslator.Lib.Drawing;
using ScreenTranslator.Lib.OCR;

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

            Read().ContinueWith(t => new Drawer(this).Draw(t.Result));
        }
        
        private async Task<OcrProcessedResult> Read()
        {
            var language = new Language("ko");
            if (!OcrEngine.IsLanguageSupported(language))
            {
                throw new Exception($"{language.LanguageTag} is not supported in this system.");
            }

            var bitmap = await GetScreenshot();
            var engine = OcrEngine.TryCreateFromLanguage(language);
            var ocrResult = await engine.RecognizeAsync(bitmap).AsTask();

            return new WindowsOcrResultProcessor().Process(ocrResult);
        }
        
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
    }
}