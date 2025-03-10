using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;


namespace Test_Assistant.ImageProcessorModels
{
    public class ParseImageProcessor
    {
        public ParseImageProcessor()
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
            using (var engine = new TesseractEngine(@"D:\visualStudio\Projects\Clicker_Renome\trainedDataLibs", "eng", EngineMode.Default))
            {
                using (var img = Pix.LoadFromFile(ConvertToBW(imagePath)))
                {
                    using (var page = engine.Process(img))
                    {
                        return page.GetText();
                    }
                }
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
            string screenshotsFilePath = $".\\..\\..\\..\\TempImages\\{DateTime.Now:MM-dd_HH-mm-ss}_BW.png"; // Define the path to save screenshots
            if (screenshotsFilePath != null)
                bwImage.Save(screenshotsFilePath, System.Drawing.Imaging.ImageFormat.Png);
            return screenshotsFilePath;
        }
    }
}
