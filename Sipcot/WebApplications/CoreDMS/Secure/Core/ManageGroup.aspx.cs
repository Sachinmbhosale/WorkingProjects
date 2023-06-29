/* ============================================================================  
Author     : 
Create date: 
Description: New Organization Creation
===============================================================================  
** Change History   
** Date:          Author:            Issue ID                  Description:  
** ----------   -------------       ----------           ----------------------------
** 
 * 16 Apr 15    Sabina              DMS5-3929 	        Adding a new checkbox list control inside manage org page
 * 16 Apr 15    Sabina              DMS5-3932           Populating rights in User role page based on available application selection
 * 19 Apr 15    Yogeesha Naik       DMS5-3935           Change Master page dynamically
 * 28 Apr 15    Yogeesha Naik       DMS5-4055	        Pages (rights) added to workflow coming under DMS and vice versa 
 *                                                      (removed server side list index change methods, handled in javascript)
=============================================================================== */
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreBL;

namespace Lotex.EnterpriseSolutions.WebUI
{
    public partial class ManageGroup : PageBase
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
                    chkSelectAllPages.Attributes.Add("onclick", "TogglePageSelection();");
                    chkSelectAllRights.Attributes.Add("onclick", "ToggleRightsSelection();");
                    string action = Request.QueryString["action"].ToString();
                    string PageName = string.Empty;
                    if (action.ToLower() == "add")
                    {
                        PageName = "USER_GROUP_ADD";
                        hdnAction.Value = "AddGroup";
                        lblHeading.Text = "Add New Role";
                        hdnCurrentGroupId.Value = "0";
                        btnsearchagain.Visible = false;
                    }
                    else if (action.ToLower() == "edit")
                    {
                        PageName = "USER_GROUP_SEARCH";
                        if (Request.QueryString["id"] == null)
                        {
                            LogOutAndRedirectionWithErrorMessge(string.Empty);
                        }
                        else
                        {
                            hdnCurrentGroupId.Value = Request.QueryString["id"].ToString();
                            hdnAction.Value = "EditGroup";
                            lblHeading.Text = "Edit Role";
                            GetGroupWithId(loginUser.LoginOrgId.ToString(), loginUser.LoginToken);
                        }
                    }

                    //Bind Applications  DMS5-3929M
                    BindApplications(loginUser);


                    //Get Rights   DMS5-3932D  moved to lstApplication selected index changed               
                    //Results rs = (new GroupBL()).GetAppRights();
                    //if (rs.Items != null)
                    //{
                    //    lstRights.Items.Clear();
                    //    foreach (Item item in rs.Items)
                    //    {
                    //        lstRights.Items.Add(new ListItem(item.Value, item.Value));
                    //    }
                    //}
                }
            }
        }
        // DMS5-3929M the action is replaced with new action to get application based on organizationApplication table
        //no other change only replaced action
        private void BindApplications(UserBase loginUser)
        {
            Results result = (new GroupBL()).GetApps("GetApplicationMapping", loginUser.LoginOrgId, loginUser.LoginToken);
            if (result.Items != null)
            {
                lstApplications.Items.Clear();
                foreach (Item item in result.Items)
                {
                    lstApplications.Items.Add(new ListItem(item.Value, item.Key));
                }
            }
        }

        public void GetGroupWithId(string loginOrgId, string loginToken)
        {
            GroupBL bl = new GroupBL();
            SearchFilter filter = new SearchFilter();
            try
            {
                filter.CurrGroupId = Convert.ToInt32(hdnCurrentGroupId.Value);

                Results res = bl.SearchGroups(filter, hdnAction.Value, loginOrgId, loginToken);
                if (res.ActionStatus == "SUCCESS" && res.Groups != null)
                {
                    Group grp = res.Groups[0];
                    txtGroupName.Text = grp.GroupName;
                    txtDescription.Text = grp.Description;
                    GetGroupRightsWithGroupId(loginOrgId, loginToken);
                }
            }
            catch (Exception ex)
            {
                divMsg.InnerHtml = CoreMessages.GetMessages(hdnAction.Value, "ERROR", ex.Message.ToString());
            }
        }
        public void GetGroupRightsWithGroupId(string loginOrgId, string loginToken)
        {

            GroupBL bl = new GroupBL();
            SearchFilter filter = new SearchFilter();
            try
            {
                Logger.Trace("GetGroupRightsWithGroupId started", "TraceStatus");
                int currentGroupId = Convert.ToInt32(hdnCurrentGroupId.Value);
                string action = "GetGroupRights";
                Results res = bl.GetGroupRightsWithGroupId(currentGroupId, action, loginOrgId, loginToken);
                if (res.ActionStatus == "SUCCESS" && res.Groups != null && res.Groups[0].groupRights != null)
                {
                    string rightsText = string.Empty;
                    Group grp = res.Groups[0];
                    string pagename = string.Empty;
                    int i = 0;
                    foreach (GroupRights grpRights in grp.groupRights)
                    {
                        if (pagename != grpRights.Pagename)
                        {
                            pagename = string.Empty;
                        }
                        if (pagename == string.Empty)
                        {
                            i = i + 1;
                            pagename = grpRights.Pagename;
                            rightsText += "#" + i.ToString() + "|" + pagename + "|" + grpRights.Rights;
                        }
                        else
                        {
                            rightsText += "|" + grpRights.Rights;
                        }
                    }
                    if (rightsText != string.Empty)
                    {
                    }
                    rightsText = rightsText.TrimStart('#');
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowGroupRights", "ShowGroupRights('" + rightsText + "');", true);
                }
                Logger.Trace("GetGroupRightsWithGroupId Finished", "TraceStatus");
            }
            catch (Exception ex)
            {
                Logger.TraceErrorLog("function GetGroupRightsWithGroupId caught ex;" + ex.Message.ToString());
                divMsg.InnerHtml = CoreMessages.GetMessages(hdnAction.Value, "ERROR");
            }
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Secure/Core/SearchGroup.aspx", true);
        }

        protected void btnsearchagain_Click(object sender, EventArgs e)
        {
            string SearchCriteria = Request.QueryString["Search"] != null ? Request.QueryString["Search"].ToString() : string.Empty;
            Response.Redirect("SearchGroup.aspx?Search=" + SearchCriteria);

        }

        protected void lstApplications_SelectedIndexChanged(object sender, EventArgs e)
        {
            GroupBL bl = new GroupBL();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            int count = 0;
            string CommaSeparatedApplicationIds = string.Empty;
            foreach (ListItem item in lstApplications.Items)
            {
                if (item.Selected)
                {
                    CommaSeparatedApplicationIds += item.Value + ",";
                    count++;
                }
            }
            if (count < lstApplications.Items.Count)
            {
                chkApplicationSelect.Checked = false;
            }

            //Get Pages   
            lstPages.Items.Clear();
            Results rs = bl.GetAppPages(loginUser.LoginParentOrgId, CommaSeparatedApplicationIds);
            if (rs.Items != null)
            {
                foreach (Item item in rs.Items)
                {
                    lstPages.Items.Add(new ListItem(item.Value, item.Value));
                }


                GetGroupRightsWithGroupId(hdnLoginOrgId.Value, hdnLoginToken.Value);
            }

            //Get Rights   DMS5-3932M                
            Results rst = (new GroupBL()).GetAppRights(CommaSeparatedApplicationIds);
            if (rst.Items != null)
            {
                lstRights.Items.Clear();
                foreach (Item Ritem in rst.Items)
                {
                    lstRights.Items.Add(new ListItem(Ritem.Value, Ritem.Value));
                }
            }
        }

    }
}
