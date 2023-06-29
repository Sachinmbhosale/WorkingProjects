/* ============================================================================  
Author     : Joby
Create date: 
Description:  
===============================================================================  
** Change History   
** Date:          Author:            Issue ID                       Description:  
** ----------   -------------       ----------                ----------------------------
16 Nov 2013     Pratheesh A         Enhancement 2 (DUEN)       Adding Domain User Details
18 Nov 2013     Pratheesh A         Wrtier DMS ENHSMT 2-1695   Domain Id check implemeted, Now domain ID sending to DB
16 Apr 2015     Sabina              DMS5-3935                   Login management:redirecting to either workflow home page or DMS based on application access
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
using System.Data;
using System.DirectoryServices;
using System.Text;

namespace Lotex.EnterpriseSolutions.WebUI
{
    public partial class Login : PageBase
    {
        UserBL BL = new UserBL();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FillCapctha();
                if (Request.QueryString["msg"] != null)
                {
                    divMsg.InnerHtml = Request.QueryString["msg"].ToString();
                    try
                    {
                        FormsAuthentication.SignOut();
                    }
                    catch
                    {

                    }
                    InitialiseSessionVariables();
                    if (Request.QueryString["Org"] == null) //OrgCode
                    {
                        divMsg.InnerHtml = "Please verify the link.";
                    }
                    else
                    {
                        string loginOrgCode = Request.QueryString["Org"].ToString().Trim();
                        hdnLoginOrgCode.Value = loginOrgCode.ToLower();
                        Session["LoginOrgCode"] = hdnLoginOrgCode.Value;
                        LoadDomain();

                    }

                }
                else if (Session["LoggedUser"] == null)
                {
                    InitialiseSessionVariables();
                    txtUsername.Focus();
                    if (Request.QueryString["Org"] == null) //OrgCode
                    {
                        divMsg.InnerHtml = "Please verify the link.";
                    }
                    else
                    {
                        string loginOrgCode = Request.QueryString["Org"].ToString().Trim();
                        hdnLoginOrgCode.Value = loginOrgCode.ToLower();
                        Session["LoginOrgCode"] = hdnLoginOrgCode.Value;
                        LoadDomain();
                    }
                }
                else
                {
                    Response.Redirect("~/secure/Home.aspx", false);
                }
            }
        }
        /// <summary>
        /// To validate login & Get user information for a valid user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (hdnCapta.Value.ToString() == txtCaptcha.Text)
            {
                divMsg.InnerHtml = string.Empty;
                SecurityBL bl = new SecurityBL();
                User loginUser = new User();
                Results rs = new Results();
                try
                {
                    loginUser.UserName = txtUsername.Text.ToUpper();
                    loginUser.LoginOrgId = hdnLoginOrgId.Value.Length > 0 ? Convert.ToInt32(hdnLoginOrgId.Value) : 0;
                    loginUser.LoginToken = hdnLoginToken.Value;
                    if (loginUser.LoginOrgId < 1 || loginUser.LoginToken.Trim() == string.Empty)
                    {
                        divMsg.InnerHtml = CoreMessages.GetMessages("InvalidToken", string.Empty);
                    }

                    if (chkDomainUser.Checked == true && checkDomainuser() == false) // DUEN - Add
                    {
                        chkDomainUser.Checked = true;
                        divMsg.InnerHtml = "Invalid Domain User Name or Password";
                        return;
                    }

                    if (loginUser.LoginOrgId > 0)
                        rs = bl.LoggedUserManagement(ref loginUser, "UserLogin");

                    if (rs.UserData != null && rs.UserData.UserId > 0)
                    {
                        rs.UserData.LoginOrgCode = hdnLoginOrgCode.Value;
                        //newly added for session issue Resolved
                        HttpCookie Orgcode = new HttpCookie("Orgcode");
                        Orgcode.Value = hdnLoginOrgCode.Value;
                        Response.Cookies.Add(Orgcode);
                        //newly added for session issue Resolved
                        Session["LoggedUser"] = rs.UserData;
                        Session["LoggedUserId"] = rs.UserData.UserId;
                        Session["OrgID"] = loginUser.LoginOrgId.ToString();
                        Session["LoggedUserName"] = rs.UserData.UserName;
                        Session["LanguageID"] = rs.UserData.LanguageID.ToString();
                        Session["LanguageCode"] = rs.UserData.LanguageCode.ToString();
                        Session["OutofOffice"] = rs.UserData.OutofOffice.ToString();
                        Session["LoginOrgCode"] = hdnLoginOrgCode.Value;

                        FormsAuthentication.RedirectFromLoginPage(loginUser.UserName, false);

                        if (rs.ActionStatus == "EXPAIRED")
                        {

                            Response.Redirect("~/secure/Core/ChanagePassword.aspx", false);
                        }
                        else
                        {
                            // In master page menus will be validated and redirected to DMS / Workflow 
                            Response.Redirect("~/secure/Home.aspx", false);
                        }
                        
                        //if (loginUser.ConfidentialityAgreement)
                        //{
                        //Response.Redirect("~/secure/Home.aspx");
                        //}
                        //else
                        //{
                        //    Response.Redirect("~/secure/User/ConfidentialityAgreement.aspx");
                        //}
                    }
                    else
                    {
                        divMsg.InnerHtml = rs.Message;
                        //added by sabina to focus the txtusername when no value in textbox
                        if (string.IsNullOrEmpty(divMsg.InnerHtml))
                        {
                            txtUsername.Focus();
                        }
                    }

                }
                catch (Exception ex)
                {
                    divMsg.InnerHtml = "An error occurred, please try again. If problem exists please contact Administrator";
                    Logger.TraceErrorLog("Login.aspx - btnSubmit_Click () :" + ex.Message.ToString());
                }
            }
            else
            {
                divMsg.InnerHtml = "Invalid CaptaCode";
                FillCapctha();
            }
        }

        void FillCapctha()
        {
            try
            {
                Random random = new Random();
                string combination = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
                StringBuilder captcha = new StringBuilder();
                for (int i = 0; i < 6; i++)
                    captcha.Append(combination[random.Next(combination.Length)]);
                hdnCapta.Value = captcha.ToString();
                Session["captcha"] = captcha.ToString();
                imgCaptcha.ImageUrl = "GenerateCaptcha.aspx?" + DateTime.Now.Ticks.ToString();
                txtCaptcha.Text = "";
            }
            catch
            {
                throw;
            }
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Close Window", "window.opener = 'x'; window.close();", true);
        }
        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            FillCapctha();
        }
        protected void LoadDomain() // DUEN - Add
        {

            try
            {
                Logger.Trace("Started Loading Domain Name Drop Down", "0");

                DataTable dsDomain = BL.GetDomains(0, "GetDomainNamewithOrgCode", "0", "0", Request.QueryString["Org"].ToString().Trim());

                if (dsDomain.Rows.Count > 0)
                {
                    drpDomain.Items.Clear();
                    drpDomain.DataSource = dsDomain;
                    drpDomain.DataTextField = "TextField";
                    drpDomain.DataValueField = "ValueField";
                    drpDomain.DataBind();
                }
                else
                {
                    drpDomain.Items.Clear();
                    drpDomain.Items.Add(new ListItem("--Select--", "0"));
                }

                Logger.Trace("Finished Loading Domain Name Drop Down", "0");

            }
            catch (Exception ex)
            {
                Logger.TraceErrorLog(ex.ToString());
            }

        }

        public bool checkDomainuser()// DUEN - Add
        {
            try
            {
                string strEntry = @"LDAP://" + drpDomain.SelectedItem.Text.Trim();
                DirectoryEntry objDirectoryEntry = new DirectoryEntry(strEntry);
                DirectorySearcher objDirectorySearcher = null;
                objDirectoryEntry.Username = txtUsername.Text.Trim();
                objDirectoryEntry.Password = txtPassword.Text.Trim();
                objDirectorySearcher = new DirectorySearcher(objDirectoryEntry);
                objDirectorySearcher.Filter = "sAMAccountName=" + txtUsername.Text.Trim();
                objDirectorySearcher.SearchScope = SearchScope.Subtree;
                objDirectorySearcher.PropertiesToLoad.Add("cn");

                try
                {
                    SearchResult myresult = objDirectorySearcher.FindOne();
                    if (myresult == null)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                catch (Exception ex) //Incase of User Name or Password Wrong --Logon failure: unknown user name or bad password.
                {
                    Logger.TraceErrorLog(ex.ToString());
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.TraceErrorLog(ex.ToString());
                return false;
            }
        }

        protected void btnrefreshcapta_Click(object sender, EventArgs e)
        {

            divMsg.InnerHtml = hdnmessage.Value.ToString();
            FillCapctha();
        }
    }
}
