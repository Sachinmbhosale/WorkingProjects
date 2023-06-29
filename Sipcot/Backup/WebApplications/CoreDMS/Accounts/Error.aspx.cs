using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lotex.EnterpriseSolutions.CoreBE;

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
