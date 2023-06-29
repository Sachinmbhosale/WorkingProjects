using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lotex.EnterpriseSolutions.WebUI
{
    /// <summary>
    /// Summary description for GenericHandler
    /// </summary>
    public class GenericHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                string f = context.Request.QueryString.Get("f");
                string FileType = string.Empty;
                if (f.IndexOf(".tif") > -1 || f.IndexOf(".pdf") > -1)
                {
                    FileType = context.Request.QueryString.Get("f").IndexOf(".tif") > -1 ? "tif" : "pdf";
                    context.Response.Clear();
                    if (FileType.ToLower() == "tif")
                        context.Response.ContentType = "image/tiff";
                    else
                        context.Response.ContentType = "Application/pdf";
                    context.Response.WriteFile(f);
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
                else if (f.IndexOf(".jpeg") > -1 || f.IndexOf(".jpg") > -1 || f.IndexOf(".bmp") > -1 || f.IndexOf(".gif") > -1 || f.IndexOf(".png") > -1)
                {
                    context.Response.Clear();
                    context.Response.ContentType = "image/jpeg";
                    context.Response.WriteFile(f);
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
                //string l = context.Request.QueryString.Get"t"("l");
                //string TodayDate = DateTime.Today.ToString("yyyy-MM-dd");
                //string TempFolder = System.Configuration.ConfigurationManager.AppSettings[l];
                //f = @TempFolder + f;
               
            }
            catch 
            {
                throw;
            }
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}