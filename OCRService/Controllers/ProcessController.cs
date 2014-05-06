using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Hosting;
using System.Web.Http;
using Tesseract;
using System.Drawing;
using System.IO;

namespace OCRService.Controllers
{
    public class ProcessController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage Url([FromBody]string path)
        {
            try
            {
                System.Net.WebClient objWebClient = new System.Net.WebClient();
                var targetFile = HostingEnvironment.MapPath(@"~/PicturesToProcess/") + Path.GetRandomFileName() + Path.GetExtension(path);
                objWebClient.DownloadFile(path, targetFile);
                Bitmap picture = new Bitmap(targetFile);
                string result = Process(picture);
                picture.Dispose();
                File.Delete(targetFile);
                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                resp.Content = new StringContent(result);
                return resp;
            }
            catch { throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, new Exception("Error"))); }
        }

        [HttpPost]
        public HttpResponseMessage Encoded([FromBody]string pictureString)
        {
            try
            {
                pictureString=pictureString.Replace("data:image/png;base64,","");
                Bitmap picture = Base64StringToBitmap(pictureString);
                string result = Process(picture);
                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                resp.Content = new StringContent(result);
                return resp;
            }
            catch { throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, new Exception("Error"))); }
        }

        private string Process(Bitmap picture)
        {
            using (var engine = new TesseractEngine(HostingEnvironment.MapPath(@"~/tessdata"), "slk", EngineMode.Default))
            {
                engine.DefaultPageSegMode = PageSegMode.SingleBlock;
                
                using (var pix = PixConverter.ToPix(picture))
                {
                    using (var page = engine.Process(pix))
                    {
                        return page.GetText();
                    }
                }
            }
        }

        public Bitmap Base64StringToBitmap(string base64String)
        {
            Bitmap bmpReturn = null;

            byte[] byteBuffer = Convert.FromBase64String(base64String);
            MemoryStream memoryStream = new MemoryStream(byteBuffer);

            memoryStream.Position = 0;

            bmpReturn = (Bitmap)Bitmap.FromStream(memoryStream);

            memoryStream.Close();
            memoryStream = null;
            byteBuffer = null;

            return bmpReturn;
        }
    }
}
