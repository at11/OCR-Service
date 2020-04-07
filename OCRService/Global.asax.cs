using System.Web.Hosting;
using System.Web.Http;
using ImageMagick;

namespace OCRService
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            MagickNET.SetGhostscriptDirectory(HostingEnvironment.MapPath(@"~/ghostscript"));
        }
    }
}
