/* ===================================================================================================================================
 * Author     : 
 * Create date: 
 * Description:  
======================================================================================================================================  
 * Change History   
 * Date:            Author:             Issue ID            Description:  
 * ----------       -------------       ----------          ------------------------------------------------------------------
 * 16/4/2015      sabina                DMS5-3935           Login management:redirecting to either workflow home page or DMS based on application access
 *    
====================================================================================================================================== */

using System;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreBL;

namespace Lotex.EnterpriseSolutions.WebUI
{
    public partial class Home : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CheckAuthentication();
            //DMS5-3935A
            MasterPage = "~/SecureMaster.Master";
            if (!IsPostBack)
            {
                UserBase loginUser = (UserBase)Session["LoggedUser"];

                lblUserName.Text = loginUser.FirstName + " " + loginUser.LastName;
                //lblOrgName.Text = "for " + loginUser.LoginOrgName;
                lblDay.Text = loginUser.UserLoggedInTime.ToShortDateString();
                lblTime.Text = loginUser.UserLoggedInTime.ToShortTimeString();
                if (loginUser.UserLoggedInTime == loginUser.UserLastLoggedInTime)
                {
                    lblLastLoggedDay.Text = " -- ";
                    lblLastLoggedTime.Text = string.Empty;
                }
                else
                {
                    lblLastLoggedDay.Text = loginUser.UserLastLoggedInTime.ToShortDateString();
                    lblLastLoggedTime.Text = loginUser.UserLastLoggedInTime.ToShortTimeString();
                }
                if (loginUser.LoginParentOrgId == 0)
                {
                    lnkCustomerLogin.Visible = true;
                    tdDashboard.Visible = true;
                    tdCustomerData.Visible = false;
                }
                else
                {
                    lblDashboard.Text = string.Empty;
                    lnkCustomerLogin.Visible = false;
                    tdDashboard.Visible = false;
                    tdCustomerData.Visible = true;
                    GetCustomerWithID(loginUser.LoginOrgId, loginUser.LoginToken);
                }
            }
        }

        public void GetCustomerWithID(int loginOrgId, string loginToken)
        {
            SecurityBL bl = new SecurityBL();
            SearchFilter filter = new SearchFilter();
            string action = Actions.SearchOrgs.ToString();
            try
            {
                filter.CurrOrgId = loginOrgId;// Here Current Organisation is same as loginOrganisation

                Results res = bl.GetOrgById(filter, action, loginOrgId.ToString(), loginToken);
                if (res.ActionStatus == "SUCCESS" && res.Orgs != null)
                {
                    Org org = res.Orgs[0];
                    lblDashboard.Text = "Welcome to " + org.OrgGreeting;
                    lblCustomerName.Text = org.OrgDetails;
                    lblAddress.Text = org.OrgAddress;
                    lblOrgEmail.Text = org.OrgEmailId;
                    lblPhoneNo.Text = org.PhoneNo;
                    lblFaxNo.Text = org.FaxNo;
                    lblContactPerson.Text = org.ContactPerson;
                    lblContactMobile.Text = org.ContactMobile;
                }
            }
            catch (Exception ex)
            {
                divMsg.InnerHtml = CoreMessages.GetMessages(action, "ERROR", ex.Message.ToString());
            }
        }

        protected void lnkCustomerLogin_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Secure/CustomerLogin.aspx");
        }

        protected void lnkEditCustomer_Click(object sender, EventArgs e)
        {
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            Response.Redirect("~/Secure/Core/ManageOrg.aspx?action=edit&id=" + loginUser.LoginOrgId);
        }
    }
}
