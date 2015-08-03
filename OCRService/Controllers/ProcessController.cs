using System;
using System.Net;
using System.Net.Http;
using System.Web.Hosting;
using System.Web.Http;
using OCRService.Utils;
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
                var objWebClient = new WebClient();
                var targetFile = HostingEnvironment.MapPath(@"~/FilesToProcess/") + Path.GetRandomFileName() + Path.GetExtension(path);
                objWebClient.DownloadFile(path, targetFile);
                var picture = new Bitmap(targetFile);
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
                pictureString = pictureString.Replace("data:image/png;base64,", "");
                Bitmap picture = ImageHelper.Base64StringToBitmap(pictureString);
                string result = Process(picture);
                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                resp.Content = new StringContent(result);
                return resp;
            }
            catch { throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, new Exception("Error"))); }
        }

        [HttpPost]
        public HttpResponseMessage Pdf([FromBody]string path)
        {
            try
            {
                var objWebClient = new WebClient();
                var targetFile = HostingEnvironment.MapPath(@"~/FilesToProcess/") + Path.GetRandomFileName() + Path.GetExtension(path);
                objWebClient.DownloadFile(path, targetFile);

                var picture = ImageHelper.ExtractImageFromPdf(targetFile);
                string result = Process(picture);
                picture.Dispose();
                File.Delete(targetFile);
                var resp = new HttpResponseMessage(HttpStatusCode.OK);
                resp.Content = new StringContent(result);
                return resp;
            }
            catch { throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, new Exception("Error"))); }
        }

        private string Process(Bitmap picture)
        {
            using (var engine = new TesseractEngine(HostingEnvironment.MapPath(@"~/tessdata"), "slk"))
            {
                engine.DefaultPageSegMode = PageSegMode.SingleBlock;

                using (var page = engine.Process(picture))
                {

                    return page.GetText();
                }
            }
        }
    }
}
