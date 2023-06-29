using System.Web;

namespace Lotex.EnterpriseSolutions.WebUI.Secure.Core
{
    /// <summary>
    /// Summary description for PDFDownloader
    /// </summary>
    public class PDFDownloader : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {

            PDFDownload(context);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public void PDFDownload(HttpContext context)
        {
            //context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World");
          
            // string fileName = "BUSProjectCard.pdf";
            string sPath = context.Request.QueryString["filePath"];
            string Orgfilename = context.Request.QueryString["Orgfilename"];
            string Fileext = System.IO.Path.GetExtension(sPath);
            string Filename=System.IO.Path.GetFileName(sPath);
            string filePath = sPath;
            context.Response.Clear();
            context.Response.ContentType = "application/pdf";
            context.Response.AddHeader("Content-Disposition", "attachment; filename=" + Orgfilename+ Fileext);
            context.Response.TransmitFile(sPath);
            context.Response.End();
        }

       // filePath

        //public void PDFDownload(HttpContext context)
        //{


        //        context.Response.ClearContent();
        //        context.Response.ClearHeaders();
        //        context.Response.ContentType = "application/pdf";
        //        context.Response.AddHeader("Content-Disposition", "attachment");
        //        context.Response.TransmitFile(context.Request.RawUrl);
        //        context.Response.Flush();
        //        context.Response.Clear();
        //        context.ApplicationInstance.CompleteRequest();

        //}

    }
}