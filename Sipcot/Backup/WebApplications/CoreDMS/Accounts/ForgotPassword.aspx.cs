using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreBL;
using System.IO;

namespace Lotex.EnterpriseSolutions.WebUI
{
    public partial class ForgotPassword : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["LoginOrgCode"] != null)
                {
                    hdnLoginOrgName.Value = Session["LoginOrgCode"].ToString();
                }
                txtUsername.Focus();
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            divMsg.InnerHtml = string.Empty;
            SecurityBL bl = new SecurityBL();
            SearchFilter filter = new SearchFilter();
            string action = "GetUserDataToSendPassword";
            try
            {
                string userName = txtUsername.Text.ToUpper();
                int loginOrgId = Convert.ToInt32(hdnLoginOrgId.Value);
                Results res = bl.GetUsersByUserNameOrgId(action, userName, string.Empty, loginOrgId);
                if (res.Users != null && res.Users.Count > 0)
                {
                    if (res.Users[0].UserId > 0)
                    {
                        res.Users[0].LoginWebsiteUrl = hdnOrglink.Value + Session["LoginOrgCode"].ToString();
                        if (SendMessage(res.Users[0], action))
                        {
                            res.ActionStatus = ActionStatus.SUCCESS.ToString();
                        }
                        else
                        {
                            res.ActionStatus = "MAILFAILED";
                        }
                    }
                    else
                    {
                        res.ActionStatus = "ERROR";
                    }
                }
                else
                {
                    res.ActionStatus = "ERROR";
                }
                if (res.ActionStatus == ActionStatus.SUCCESS.ToString())
                {
                    divMsg.Style.Add("color", "green");
                    divMsg.InnerHtml = CoreMessages.GetMessages(action, res.ActionStatus) + res.Users[0].EmailId;
                }
                else
                {

                    divMsg.InnerHtml = CoreMessages.GetMessages(action, res.ActionStatus);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        protected void btnClose_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Accounts/Login.aspx?org=" + hdnLoginOrgName.Value);
        }


    }
}
