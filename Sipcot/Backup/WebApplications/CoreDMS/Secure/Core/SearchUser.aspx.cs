/* ============================================================================  
Author     : 
Create date: 
Description: New Organization Creation
===============================================================================  
** Change History   
** Date:          Author:            Issue ID                  Description:  
** ----------   -------------       ----------           ----------------------------
 * 19 Apr 15    Yogeesha Naik       DMS5-3935           Change Master page dynamically
=============================================================================== */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreBL;

namespace Lotex.EnterpriseSolutions.WebUI
{
    public partial class SearchUser : PageBase
    {
        

        /* DMS5-3935 BS */
        protected void Page_PreInit(object sender, EventArgs e)
        {
            /* setting master page */
            ChangeMasterPage(MasterPage);
        }

        protected void ChangeMasterPage(string masterPage)
        {
            if (masterPage.Length > 0)
                if (!masterPage.Substring(masterPage.LastIndexOf("/")).Equals(this.Page.MasterPageFile.Substring(this.Page.MasterPageFile.LastIndexOf("/"))))
                    MasterPageFile = masterPage;
        }

        /* DMS5-3935 BE*/
        protected void Page_Load(object sender, EventArgs e)
        {
            CheckAuthentication();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
            hdnLoginToken.Value = loginUser.LoginToken;
            hdnPageId.Value = "0";


            string pageRights = GetPageRights();
            hdnPageRights.Value = pageRights;
            ApplyPageRights(pageRights, this.Form.Controls);
            txtUserName.Focus();
        }
    }
}
