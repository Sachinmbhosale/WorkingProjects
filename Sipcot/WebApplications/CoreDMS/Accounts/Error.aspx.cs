using System;

namespace Lotex.EnterpriseSolutions.WebUI
{
    public partial class Error : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string orgCode = Session["LoginOrgCode"] != null ? Session["LoginOrgCode"].ToString() : string.Empty;
            LogOutAndRedirection(orgCode,"SessionOut");
        }
    }
}
