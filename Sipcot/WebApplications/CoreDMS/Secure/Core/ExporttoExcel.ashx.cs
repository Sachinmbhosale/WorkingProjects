using System;
using System.Web;


namespace Lotex.EnterpriseSolutions.WebUI.Secure.Core
{
    /// <summary>
    /// Summary description for ExporttoExcel
    /// </summary>
    public class ExporttoExcel : IHttpHandler
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
            string Orgfilename = context.Request.QueryString["Orgfilename"];            
            Response.BufferOutput = true;
            string zipName = String.Format(Orgfilename, DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"));

            Response.ContentType = "application/vnd.ms-excel";
            Response.AppendHeader("content-disposition", "attachment; filename=" + Orgfilename);
           
            //string sPath = context.Session["zipFilePath"] as string;
            string sPath = context.Request.QueryString["path"];
            byte[] data = System.IO.File.ReadAllBytes(sPath);
            //System.IO.File.Delete(sPath);
            Response.OutputStream.Write(data, 0, data.Length);

            Response.End();

        }
    }
}