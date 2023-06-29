
/* ============================================================================  
Author     : Joby
Create date: 
Description:  
===============================================================================  
** Change History   
** Date:          Author:            Issue ID                       Description:  
** ----------   -------------       ----------                ----------------------------

 * 17 Apr 2015     Gokuldas.p              DMS5-3935          Login management:redirecting to either workflow home page or DMS based on application access
 * 28 Apr 2015     Yogeesha Naik           DMS5-3935          Re written dynamic master page swapping code
=============================================================================== */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Web.Security;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreBL;

namespace Lotex.EnterpriseSolutions.WebUI
{
    public partial class ChangePassword : PageBase
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
        // DMS5-3935 BE
      
        protected void Page_Load(object sender, EventArgs e)
        {
            // txtOldPassword.Attributes.Add("type", "password");
            txtOldPassword.Attributes.Add("autocomplete", "off");
            // txtNewPassword.Attributes.Add("type", "password");
            //  txtRetryNewPassord.Attributes.Add("type", "password");

            CheckAuthentication();

            if (!IsPostBack)
            {
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                if (loginUser.IsDomainUser == true)
                {
                    Response.Redirect("~/Secure/Home.aspx");
                }
                hdnUserName.Value = loginUser.UserName;
                hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
                hdnLoginToken.Value = loginUser.LoginToken;

                //txtOldPassword.Text = string.Empty;
                //txtOldPassword.Focus();
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            // In master page menus will be validated and redirected to DMS / Workflow 
            Response.Redirect("~/secure/Home.aspx", false);
        }
    }
}
