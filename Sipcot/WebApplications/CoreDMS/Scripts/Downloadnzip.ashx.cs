using System;
using System.Web;

namespace Lotex.EnterpriseSolutions.WebUI
{
    /// <summary>
    /// Summary description for Downloadnzip
    /// </summary>
    public class Downloadnzip : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            test(context.Response, context);
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
            //System.IO.File.Delete(sPath);
            Response.OutputStream.Write(data, 0, data.Length);

            Response.End();
            
        }


    }

     
}