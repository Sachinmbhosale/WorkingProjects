using System;
using Lotex.EnterpriseSolutions.CoreBE;

namespace Lotex.EnterpriseSolutions.WebUI
{
    public partial class DocumentTypes : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CheckAuthentication();
            if (!IsPostBack)
            {
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
                hdnLoginToken.Value = loginUser.LoginToken;
                hdnPageId.Value = "0";
                string pageRights = GetPageRights();
                hdnPageRights.Value = pageRights;
                ApplyPageRights(pageRights, this.Form.Controls);
            }

        }

    }

}