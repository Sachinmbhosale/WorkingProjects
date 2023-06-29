/* ============================================================================  
Author     : Joby
Create date: 
Description: 
===============================================================================  
** Change History   
** Date:          Author:            Issue ID                   Description:  
** ----------   -------------       ----------                  ----------------------------
15 Nov 2013     Pratheesh A         Enhancement 2 (DUEN)        Adding Domain User Details
18 Nov 2013     Pratheesh A         Wrtier DMS ENHSMT 2-1692    Reset Password Enabling / Disabling in Edit
19 Apr 2015    Yogeesha Naik        DMS5-3935                   Change Master page dynamically 
=============================================================================== */

using System;
using System.Web.UI.WebControls;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreBL;
using System.Data;

namespace Lotex.EnterpriseSolutions.WebUI
{
    public partial class ManageUser : PageBase
    {

        UserBase BE = new UserBase();
        UserBL BL = new UserBL();
        Results Results = new Results();
        string FinalResult = string.Empty;

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
            //this.MasterPageFile = MasterPage;
            if (!IsPostBack)
            {
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
                hdnLoginToken.Value = loginUser.LoginToken;
                if (Request.QueryString["action"] == null)
                {
                    LogOutAndRedirectionWithErrorMessge(string.Empty);
                }
                else
                {
                    LoadDomain();
                    
                    GetGroups(loginUser.LoginOrgId.ToString(), loginUser.LoginToken);
                    GetDepartments(loginUser.LoginOrgId.ToString(), loginUser.LoginToken);
                    string action = Request.QueryString["action"].ToString();
                    txtUserEmailId.Attributes.Add("onkeyup", "enable();");
                    txtUserName.Attributes.Add("onkeyup", "enable();");
                    if (action.ToLower() == "add")
                    {
                       
                        hdnAction.Value = "AddUser";
                        lblHeading.Text = "Add New User";
                        hdnCurrentUserId.Value = "0";
                        btnresetpassword.Attributes.Add("class", "HiddenButton");
                        btnsearchagain.Attributes.Add("class", "HiddenButton");
                        chkDomainUser.Checked = false;

                    }
                    else if (action.ToLower() == "edit")
                    {
                        
                        if (Request.QueryString["id"] == null)
                        {
                            LogOutAndRedirectionWithErrorMessge(string.Empty);
                        }
                        else
                        {
                            if (Request.QueryString["Search"] != null)
                            {
                                btnsearchagain.Attributes.Add("class", "btnsearchagain");
                            }

                            hdnCurrentUserId.Value = Request.QueryString["id"].ToString();
                            hdnAction.Value = "EditUser";
                            lblHeading.Text = "Edit User";
                            GetUserWithID(loginUser.LoginOrgId.ToString(), loginUser.LoginToken);
                        }
                    }
                    else if (action.ToLower() == "edittempuser")
                    {
                        
                        if (Request.QueryString["id"] == null)
                        {
                            LogOutAndRedirectionWithErrorMessge(string.Empty);
                        }
                        else
                        {
                            hdnCurrentUserId.Value = Request.QueryString["id"].ToString();
                            hdnAction.Value = "EditTempUser";
                            lblHeading.Text = "Edit User";
                            drpDepartment.SelectionMode = ListSelectionMode.Single;
                            GetTempUserWithID(loginUser.LoginOrgId.ToString(), loginUser.LoginToken);


                        }
                    }
                    string pageRights = GetPageRights();
                    hdnPageRights.Value = pageRights;
                    ApplyPageRights(pageRights, this.Form.Controls);
                }
                chkSelectAllDepartment.Attributes.Add("onclick", "ToggleDepartmentSelection();");
            }
        }
        public void GetUserWithID(string loginOrgId, string loginToken)
        {
            UserBL bl = new UserBL();
            SearchFilter filter = new SearchFilter();
            try
            {
                filter.CurrUserId = Convert.ToInt32(hdnCurrentUserId.Value);
                //user authentication

                Results res = bl.SearchUsers(filter, hdnAction.Value, loginOrgId, loginToken);
                if (res.ActionStatus == "SUCCESS" && res.Users != null)
                {
                    User user = res.Users[0];
                    if (user.UserName.ToLower() == "administrator")
                    {
                        txtUserName.Enabled = false;
                        txtFirstName.Enabled = false;
                        txtLastName.Enabled = false;
                        txtDescription.Enabled = false;
                        drpDepartment.Enabled = false;
                        drpUserGroup.Enabled = false;
                        chkActive.Enabled = false;
                        chkDomainUser.Enabled = false; // DUEN - Add
                        drpDomain.Enabled = false;
                        chkSelectAllDepartment.Enabled = false;
                    }
                    txtUserName.Text = user.UserName;
                    txtDescription.Text = user.Description;
                    txtFirstName.Text = user.FirstName;
                    txtLastName.Text = user.LastName;
                    chkActive.Checked = user.Active;
                    chkDomainUser.Checked = user.DomainUser; // DUEN - Add
                    if (user.DomainUser == true)
                    {
                        btnresetpassword.Attributes.Add("class", "HiddenButton");
                    }
                    else
                    {
                        btnresetpassword.Attributes.Add("class", "btnresetpassword");
                    }
                    txtUserEmailId.Text = user.EmailId;
                    txtMobileNo.Text = user.MobileNo;
                    foreach (ListItem item in drpUserGroup.Items)
                    {
                        if (item.Value == user.GroupId.ToString())
                        {
                            item.Selected = true;
                        }
                    }
                    string[] Departments = user.DepartmentIdsForUserCreattion.ToString().Split(',');


                    drpDepartment.SelectionMode = ListSelectionMode.Multiple;

                    for (int i = 0; i < drpDepartment.Items.Count; i++)
                    {
                        foreach (string item in Departments)
                        {

                            if (drpDepartment.Items[i].Value == item)
                            {
                                drpDepartment.Items[i].Selected = true;
                            }
                        }

                    }

                    foreach (ListItem item in drpDomain.Items) // DUEN - Add
                    {
                        if (item.Text == user.DomainName.ToString())
                        {
                            item.Selected = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                divMsg.InnerHtml = CoreMessages.GetMessages(hdnAction.Value, "ERROR");
                Logger.Trace("Exception:" + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        public void GetTempUserWithID(string loginOrgId, string loginToken)
        {
            UserBL bl = new UserBL();
            SearchFilter filter = new SearchFilter();
            try
            {
                filter.CurrUserId = Convert.ToInt32(hdnCurrentUserId.Value);
                //user authentication

                Results res = bl.SearchUsers(filter, hdnAction.Value, loginOrgId, loginToken);
                if (res.ActionStatus == "SUCCESS" && res.Users != null)
                {
                    User user = res.Users[0];
                    txtUserName.Text = user.UserName;
                    txtDescription.Text = user.Description;
                    txtFirstName.Text = user.FirstName;
                    txtLastName.Text = user.LastName;
                    chkActive.Checked = user.Active;
                    chkDomainUser.Checked = user.DomainUser;
                    btnresetpassword.Attributes.Add("class", "HiddenButton");
                    btnsearchagain.Attributes.Add("class", "HiddenButton");
                    txtUserEmailId.Text = user.EmailId;
                    txtMobileNo.Text = user.MobileNo;
                    foreach (ListItem item in drpUserGroup.Items)
                    {
                        if (item.Value == user.GroupId.ToString())
                        {
                            item.Selected = true;
                        }
                    }
                    string[] Departments = user.DepartmentIdsForUserCreattion.ToString().Split(',');
                    foreach (string item in Departments)
                    {
                        drpDepartment.SelectedValue = item;
                    }
                    foreach (ListItem item in drpDomain.Items)
                    {
                        if (item.Text == user.DomainName.ToString())
                        {
                            item.Selected = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                divMsg.InnerHtml = CoreMessages.GetMessages(hdnAction.Value, "ERROR");
                Logger.Trace("Exception:" + ex.Message, Session["LoggedUserId"].ToString());
            }
        }


        public void GetGroups(string loginOrgId, string loginToken)
        {
            GroupBL bl = new GroupBL();
            SearchFilter filter = new SearchFilter();
            try
            {
                string action = "SearchGroups";
                Results res = bl.SearchGroups(filter, action, loginOrgId, loginToken);

                if (res.ActionStatus == "SUCCESS")
                {
                    drpUserGroup.Items.Add(new ListItem("<Select>", "0"));
                    if (res.Groups != null)
                    {
                        foreach (Group grp in res.Groups)
                        {
                            drpUserGroup.Items.Add(new ListItem(grp.GroupName, grp.GroupId.ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                divMsg.InnerHtml = CoreMessages.GetMessages(hdnAction.Value, "ERROR");
                Logger.Trace("Exception:" + ex.Message, Session["LoggedUserId"].ToString());
            }
        }
        public void GetDepartments(string loginOrgId, string loginToken)
        {
            DepartmentBL bl = new DepartmentBL();
            SearchFilter filter = new SearchFilter();
            try
            {
                string action = "SearchDepartments";
                Results res = bl.SearchDepartments(filter, action, loginOrgId, loginToken);

                if (res.ActionStatus == "SUCCESS")
                {
                    if (res.Departments != null)
                    {
                        foreach (Department dp in res.Departments)
                        {
                            drpDepartment.Items.Add(new ListItem(dp.DepartmentName, dp.DepartmentId.ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                divMsg.InnerHtml = CoreMessages.GetMessages(hdnAction.Value, "ERROR");
                Logger.Trace("Exception" + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            btnSave.Enabled = true;
            string userPass = hdnCredentials.Value;
            string orglink = hdnOrglink.Value;
            divMsg.InnerHtml = string.Empty;
            Results res = new Results();
            string action = "AddUserSentMail";
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            try
            {
                Logger.Trace("btnSubmit_Click started", Session["LoggedUserId"].ToString());
                User user = new User();
                user.UserName = txtUserName.Text.Trim().ToLower();
                user.Password = userPass;

                user.FirstName = txtFirstName.Text.Trim();
                user.LastName = txtLastName.Text.Trim();
                user.OrgEmailId = System.Configuration.ConfigurationManager.AppSettings["SuperAdminEmail"].ToString();
                user.EmailId = txtUserEmailId.Text;
                user.LoginOrgName = loginUser.LoginOrgName;

                user.LoginWebsiteUrl = orglink + loginUser.LoginOrgCode;

                if (SendMessage(user, action))
                {
                    res.ActionStatus = "SUCCESS";
                }
                else
                {
                    res.ActionStatus = "MAILFAILED";
                }
                if (res.ActionStatus == "SUCCESS")
                {
                    divMsg.Style.Add("color", "green");
                    divMsg.InnerHtml = CoreMessages.GetMessages(action, res.ActionStatus) + user.EmailId;
                }
                else
                {
                    divMsg.InnerHtml = CoreMessages.GetMessages(action, res.ActionStatus);
                }
                Logger.Trace("btnSubmit_Click finished", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception" + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        protected void btnresetpassword_Click(object sender, EventArgs e)
        {
            try
            {
                divMsg.InnerHtml = "";
                User user = new CoreBE.User();
                user.UserId = Convert.ToInt32(hdnCurrentUserId.Value);
                user.UserName = txtUserName.Text;
                Results = BL.PasswordReset(user, "PasswordReset", hdnLoginOrgId.Value, hdnLoginToken.Value);
                divMsg.InnerHtml = Results.Message;
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception:" + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        protected void btnsearchagain_Click(object sender, EventArgs e)
        {
            string SearchCriteria = Request.QueryString["Search"] != null ? Request.QueryString["Search"].ToString() : string.Empty;
            Response.Redirect("SearchUser.aspx?Search=" + SearchCriteria);
        }

        protected void LoadDomain() // DUEN - Add
        {
            try
            {
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                Logger.Trace("Started Loading Domain Name Drop Down", Session["LoggedUserId"].ToString());

                DataTable dsDomain = BL.GetDomains(loginUser.UserOrgId, "GetDomainNamewithUserID", hdnLoginOrgId.Value, hdnLoginToken.Value);

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

                Logger.Trace("Finished Loading Domain Name Drop Down", Session["LoggedUserId"].ToString());

            }
            catch (Exception ex)
            {
                Logger.Trace("Exception:"+ex.Message, Session["LoggedUserId"].ToString());
            }

        }
    }
}
