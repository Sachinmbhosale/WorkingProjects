using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

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