using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using iTextSharp.text.pdf;

namespace OCRService.Utils
{
    public static class ImageHelper
    {
        public static Bitmap ExtractImageFromPdf(string sourcePdf)
        {
            var pdf = new PdfReader(sourcePdf);

            try
            {
                PdfDictionary pg = pdf.GetPageN(1);
                var res = (PdfDictionary)PdfReader.GetPdfObject(pg.Get(PdfName.RESOURCES));
                var xobj = (PdfDictionary)PdfReader.GetPdfObject(res.Get(PdfName.XOBJECT));

                foreach (PdfName name in xobj.Keys)
                {
                    PdfObject obj = xobj.Get(name);
                    if (!obj.IsIndirect()) continue;
                    var tg = (PdfDictionary)PdfReader.GetPdfObject(obj);
                    var type = (PdfName)PdfReader.GetPdfObject(tg.Get(PdfName.SUBTYPE));
                    if (!PdfName.IMAGE.Equals(type)) continue;
                    var xrefIndex = Convert.ToInt32(((PRIndirectReference)obj).Number.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    var pdfObj = pdf.GetPdfObject(xrefIndex);
                    var pdfStrem = (PdfStream)pdfObj;
                    byte[] bytes = PdfReader.GetStreamBytesRaw((PRStream)pdfStrem);
                    if ((bytes == null)) continue;

                    using (var memStream = new MemoryStream(bytes))
                    {
                        memStream.Position = 0;
                        return new Bitmap(memStream);
                    }
                }
            }
            finally
            {
                pdf.Close();
            }

            return null;
        }

        public static ImageCodecInfo GetImageEncoder(string imageType)
        {
            imageType = imageType.ToUpperInvariant();

            foreach (ImageCodecInfo info in ImageCodecInfo.GetImageEncoders())
            {
                if (info.FormatDescription == imageType)
                {
                    return info;
                }
            }

            return null;
        }

        public static Bitmap Base64StringToBitmap(string base64String)
        {
            byte[] byteBuffer = Convert.FromBase64String(base64String);
            var memoryStream = new MemoryStream(byteBuffer);

            memoryStream.Position = 0;

            var bmpReturn = (Bitmap)Image.FromStream(memoryStream);

            memoryStream.Close();

            return bmpReturn;
        }
    }
}