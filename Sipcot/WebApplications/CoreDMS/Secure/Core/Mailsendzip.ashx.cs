using System;
using System.Web;

namespace Lotex.EnterpriseSolutions.WebUI
{
    /// <summary>
    /// Summary description for Mailsendzip
    /// </summary>
    public class Mailsendzip : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("Hello World");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public void test(HttpResponse Response
            , HttpContext context)
        {
            Response.BufferOutput = true;
            string zipName = String.Format("Zip_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"));

            Response.ContentType = "application/zip";
            Response.AddHeader("content-disposition", "attachment; filename=" + zipName);
            //string sPath = context.Session["zipFilePath"] as string;
            string sPath = context.Request.QueryString["path"];
            byte[] data = System.IO.File.ReadAllBytes(sPath);

            string mailto = context.Request.QueryString["detail"].Split('~')[0];
            string message = context.Request.QueryString["detail"].Split('~')[1];
            string subject = context.Request.QueryString["detail"].Split('~')[2];

            //System.IO.File.Delete(sPath);
            Response.OutputStream.Write(data, 0, data.Length);

            Response.End();

        }
    }
}