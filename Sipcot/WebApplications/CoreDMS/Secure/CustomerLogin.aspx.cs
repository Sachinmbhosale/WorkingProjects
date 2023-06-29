using System;
using System.Web.UI.WebControls;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreBL;

namespace Lotex.EnterpriseSolutions.WebUI
{
    public partial class CustomerLogin : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CheckAuthentication();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            if (loginUser.UserParentOrgId != 0)
            {
                LogOutAndRedirectionWithErrorMessge("You are not authorised to use this fucntionality");
            }

            else
            {
                SecurityBL bl = new SecurityBL();
                SearchFilter filter = new SearchFilter();
                Results res = bl.SearchOrgs(filter, Actions.SearchOrgs.ToString(), loginUser.LoginOrgId.ToString(), loginUser.LoginToken);
                if (res.Orgs == null)
                {
                    divMsg.InnerHtml = CoreMessages.GetMessages(Actions.SearchOrgs.ToString(), ActionStatus.NODATA.ToString());
                }
                else
                {

                    foreach (Org org in res.Orgs)
                    {
                        if (org.OrgParentId > 0)
                        {
                            cmbCustomers.Items.Add(new ListItem(org.OrgName, org.OrgId.ToString() + "|" + org.OrgCode));
                        }
                    }
                    if (cmbCustomers.Items.Count == 0)
                    {
                        cmbCustomers.Enabled = false;
                        divMsg.InnerText = "No active customers are available.";
                        btnSubmit.Enabled = false;
                    }
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            //need to login to the Customer portal as per the customer selected
            string selOrgData = cmbCustomers.SelectedItem.Value;
            string[] selOrgDataArray = selOrgData.Split('|');
            if (selOrgDataArray.Length == 2)
            {
                int orgid = Convert.ToInt32(selOrgDataArray[0]);
                string orgCode = selOrgDataArray[1];
                string action = Actions.ValidateUserExplicitly.ToString();
                SecurityBL bl = new SecurityBL(); // TODO: Initialize to an appropriate value
                User writerUser = new User();
                writerUser.UserName = loginUser.UserName;
                writerUser.LoginOrgCode = orgCode;
                string LoginToken = Guid.NewGuid().ToString();
                writerUser.LoginToken = LoginToken;
                Results rs = bl.ValidateUserManagement(ref writerUser, action);
                if (rs.ActionStatus == ActionStatus.SUCCESS.ToString())
                {
                    writerUser.LoginOrgId = rs.UserData.LoginOrgId;
                    action = Actions.UserLogin.ToString();
                    rs = bl.LoggedUserManagement(ref writerUser, action);
                    if (rs.ActionStatus == ActionStatus.SUCCESS.ToString())
                    {
                        InitialiseSessionVariables();
                        Session["LoginOrgCode"] = orgCode;
                        Session["LoggedUser"] = rs.UserData;
                        Session["LoggedUserId"] = rs.UserData.UserId;
                        Session["OrgID"] = loginUser.LoginOrgId.ToString();
                        Session["LoggedUserName"] = rs.UserData.UserName;
                        Response.Redirect("~/Secure/Home.aspx");
                    }
                }
            }
        }
    }
}
