using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;


namespace Test_Assistant.Processors
{
    public class ImageProcessor
    {
        private static Rectangle _screenBounds = Screen.PrimaryScreen.Bounds;
        public ImageProcessor()
        {

        }

        public string ParseImage(string imagePath)
        {
            var text = "No text found";
            text = ExtractTextFromImage(imagePath);
            return text;
        }


        private string ExtractTextFromImage(string imagePath)
        {
            using (var engine = new TesseractEngine(@".\trainedDataLibs", "eng", EngineMode.Default))
            {
                using (var img = Pix.LoadFromFile(ConvertToBW(imagePath)))
                {
                    using (var page = engine.Process(img))
                    {
                        return page.GetText().Replace("\n", "");
                    }
                }
            }
        }

        public string TakeScreenshot(string screenshotsFolderPath, int xStart = 0, int yStart = 0, int xEnd = 0, int yEnd = 0)
        {
            if (String.IsNullOrEmpty(screenshotsFolderPath))
                screenshotsFolderPath = ".\\TempImages"; // Define the path to save screenshots

            // Ensure the directory exists
            if (!Directory.Exists(screenshotsFolderPath))
                Directory.CreateDirectory(screenshotsFolderPath);

            if(xEnd<xStart)
            {
                int temp = xStart;
                xStart = xEnd;
                xEnd = temp;
            }
            if (yEnd < yStart)
            {
                int temp = yStart;
                yStart = yEnd;
                yEnd = temp;
            }


            // Set default values for xEnd and yEnd if they are not provided
            xEnd = xEnd == 0 ? _screenBounds.Width : xEnd;
            yEnd = yEnd == 0 ? _screenBounds.Height : yEnd;

            // Create a bitmap with the specified size
            using (Bitmap bitmap = new Bitmap(xEnd - xStart, yEnd - yStart))
            {
                // Create a graphics object from the bitmap
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    // Copy the screen content to the bitmap
                    graphics.CopyFromScreen(xStart, yStart, 0, 0, new Size(xEnd - xStart, yEnd - yStart));
                }

                // Save the bitmap to the specified file path
                string screenshotsFilePath = $"{DateTime.Now:MM-dd_HH-mm-ss}.png";
                string screenshotsFullPath = Path.Combine(screenshotsFolderPath, screenshotsFilePath);

                try
                {
                    bitmap.Save(screenshotsFullPath, System.Drawing.Imaging.ImageFormat.Png);
                }
                catch (ExternalException ex)
                {
                    MessageBox.Show("Error saving screenshot");
                    // Log the exception or handle it accordingly
                    Console.WriteLine($"Error saving screenshot: {ex.Message}");
                    return null;
                }

                return screenshotsFullPath;
            }
        }
        private string ConvertToBW(string imagePath)
        {
            Bitmap original = new Bitmap(imagePath);
            Bitmap bwImage = new Bitmap(original.Width, original.Height);

            for (int y = 0; y < original.Height; y++)
            {
                for (int x = 0; x < original.Width; x++)
                {
                    Color pixelColor = original.GetPixel(x, y);
                    int grayValue = (int)(0.3 * pixelColor.R + 0.59 * pixelColor.G + 0.11 * pixelColor.B);
                    Color bwColor = Color.FromArgb(grayValue, grayValue, grayValue);
                    bwImage.SetPixel(x, y, bwColor);
                }
            }
            string screenshotsFilePath = $".\\TempImages\\{DateTime.Now:MM-dd_HH-mm-ss}_BW.png"; // Define the path to save screenshots
            if (screenshotsFilePath != null)
                bwImage.Save(screenshotsFilePath, System.Drawing.Imaging.ImageFormat.Png);
            return screenshotsFilePath;
        }
    }
}
