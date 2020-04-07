using System.Net;
using System.Net.Http;
using System.Web.Hosting;
using System.Web.Http;
using OCRService.Utils;
using Tesseract;
using System.Drawing;

namespace OCRService.Controllers
{
    public class ProcessController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage Image([FromBody] string url)
        {
            using (var objWebClient = new WebClient())
            {
                var bytes = objWebClient.DownloadData(url);
                using (var picture = ImageHelper.ConvertImage(bytes))
                {
                    var result = Process(picture);
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(result)
                    };
                }
            }
        }

        [HttpPost]
        public HttpResponseMessage Encoded([FromBody] string base64)
        {
            using (var picture = ImageHelper.ConvertBase64String(base64))
            {
                var result = Process(picture);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(result)
                };
            }
        }

        [HttpPost]
        public HttpResponseMessage Pdf([FromBody] string url)
        {
            using (var objWebClient = new WebClient())
            {
                var bytes = objWebClient.DownloadData(url);
                using (var picture = ImageHelper.ConvertPdf(bytes))
                {
                    var result = Process(picture);
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(result)
                    };
                }
            }
        }

        private static string Process(Bitmap picture)
        {
            using (var engine = new TesseractEngine(HostingEnvironment.MapPath(@"~/tessdata"), "slk"))
            {
                using (var page = engine.Process(picture))
                {
                    return page.GetText();
                }
            }
        }
    }
}
