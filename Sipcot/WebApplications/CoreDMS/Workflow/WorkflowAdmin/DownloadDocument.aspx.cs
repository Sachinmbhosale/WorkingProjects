using System;
using System.Web;

namespace Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin
{
    public partial class DownloadDocument : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string Filepath =  HttpUtility.UrlDecode(Request.QueryString["FilePath"].ToString());
            Response.AppendHeader("Content-Disposition", "attachment;filename=" + Filepath);
            Response.TransmitFile(Filepath);
            Response.Flush();
            Response.End();
        }
    }
}