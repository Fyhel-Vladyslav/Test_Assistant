using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_Assistant.ImageProcessorModels
{
    public class ScreenshotProcessor
    {
        private static Rectangle _screenBounds = Screen.PrimaryScreen.Bounds;

        public string TakeScreenshot(int xStart = 0, int yStart = 0, int xEnd = 0, int yEnd = 0)
        {
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
                    graphics.CopyFromScreen(xEnd, yEnd, xStart, yStart, new Size(xEnd - xStart, yEnd - yStart));
                }
             
                // Save the bitmap to the specified file path
                string screenshotsFilePath = $".\\..\\..\\..\\TempImages\\{DateTime.Now:MM-dd_HH-mm-ss}.png"; // Define the path to save screenshots
                bitmap.Save(screenshotsFilePath, ImageFormat.Png);
                return screenshotsFilePath;
            }
        }
    }
}
