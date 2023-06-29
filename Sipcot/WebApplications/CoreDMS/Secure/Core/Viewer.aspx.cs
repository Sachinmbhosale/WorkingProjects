using System;

namespace Lotex.EnterpriseSolutions.WebUI.Secure.Core
{
    public partial class Viewer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //  string DocId = Request.QueryString["f"].ToString();
            //  DataTable Doc = CommonData.ViewDocument(DocId);
            Response.ContentType = "application/pdf";
            Response.Clear();

            if (Request.QueryString["tagid"] != null)
            {
                Response.TransmitFile(Request.QueryString["tagid"].ToString());
            }
            else
            {

                Response.TransmitFile(Request.QueryString["f"].ToString());

            }

            Response.End();
        }
    }
}