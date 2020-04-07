using System;
using System.Drawing;
using System.IO;
using System.Linq;
using ImageMagick;

namespace OCRService.Utils
{
    public static class ImageHelper
    {
        public static Bitmap ConvertPdf(byte[] pdfBytes)
        {
            var settings = new MagickReadSettings();
            // Settings the density to 300 dpi will create an image with a better quality
            settings.Density = new Density(300);
            settings.FrameIndex = 0; // First page
            settings.FrameCount = 1; // Number of pages

            using (var imageCollection = new MagickImageCollection())
            {
                // Add all the pages of the pdf file to the collection
                imageCollection.Read(pdfBytes, settings);
                var img = imageCollection.Single();
                return img.ToBitmap();
            }
        }

        public static Bitmap ConvertBase64String(string base64String)
        {
            var byteBuffer = Convert.FromBase64String(base64String.Replace("data:image/png;base64,", ""));
            using (var memoryStream = new MemoryStream(byteBuffer))
            {
                return new Bitmap(memoryStream);
            }
        }

        public static Bitmap ConvertImage(byte[] imageBytes)
        {
            using (var memoryStream = new MemoryStream(imageBytes))
            {
                return new Bitmap(memoryStream);
            }
        }
    }
}