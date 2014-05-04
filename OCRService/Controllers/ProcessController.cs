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
        public string Get(string path)
        {
            System.Net.WebClient objWebClient = new System.Net.WebClient();
            var targetFile = HostingEnvironment.MapPath(@"~/PicturesToProcess/") + Path.GetRandomFileName() + Path.GetExtension(path);
            objWebClient.DownloadFile(path, targetFile);
            Bitmap picture = new Bitmap(targetFile);
            string result = Process(picture);
            picture.Dispose();
            File.Delete(targetFile);
            return result;
        }

        private string Process(Bitmap picture)
        {
            using (var engine = new TesseractEngine(HostingEnvironment.MapPath(@"~/tessdata"), "slk", EngineMode.Default))
            {
                using (var pix = PixConverter.ToPix(picture))
                {
                    using (var page = engine.Process(pix))
                    {
                        return page.GetText();
                    }
                }
            }
        }
    }
}
