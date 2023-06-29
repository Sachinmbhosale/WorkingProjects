/* ===================================================================================================================================
 * Author     : 
 * Create date: 
 * Description:  
======================================================================================================================================  
 * Change History   
 * Date:            Author:             Issue ID            Description:  
 * ----------       -------------       ----------          ------------------------------------------------------------------
 * 27 Apr 2015      Yogeesha Naik       DMS5-4052	        Menu - Re factor menu implementation
====================================================================================================================================== */

using System;
using System.Linq;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreBL;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;

namespace Lotex.EnterpriseSolutions.WebUI
{
    public class MasterPageBase : System.Web.UI.MasterPage
    {
        public string GetCompanyLogoPath()
        {
            if (Session["LoginOrgLogoPath"] == null)
            {
                GetCompanyInfo();
            }
            return (Convert.ToString(Session["LoginOrgLogoPath"]));
        }
        public string GetCompanyName()
        {
            if (Session["LoginOrgName"] == null)
            {
                GetCompanyInfo();
            }
            return (Convert.ToString(Session["LoginOrgName"]));

        }
        public bool GetCompanyInfo()
        {
            string logoServerPath = string.Empty;
            try
            {
                //To get company logo pah

                string loginOrgCode = string.Empty;
                string logoImageName = string.Empty;
                string loginOrgName = string.Empty;
                //Get Users Login Name
                if (Session["LoginOrgCode"] != null)
                {
                    loginOrgCode = Convert.ToString(Session["LoginOrgCode"]);
                }
                else if (Session["LoggedUser"] != null)
                {
                    UserBase loginUser = (UserBase)Session["LoggedUser"];
                    loginOrgCode = loginUser.LoginOrgCode;
                }
                if (loginOrgCode != string.Empty)
                {
                    SecurityBL bl = new SecurityBL();
                    if (bl.GetLogoImageName(loginOrgCode.ToLower(), ref logoImageName, ref loginOrgName))
                    {
                        if (logoImageName != string.Empty)
                        {
                            logoServerPath = "~/Assets/Logo/" + logoImageName;
                        }

                    }
                    if (logoServerPath == string.Empty || !System.IO.File.Exists(Server.MapPath(logoServerPath)))
                    {
                        logoServerPath = "~/Assets/Logo/Company_logo.jpg";
                    }
                    if (loginOrgName == string.Empty)
                    {
                        loginOrgName = "Writers Corporation";
                    }
                    Session["LoginOrgLogoPath"] = logoServerPath;
                    Session["LoginOrgName"] = loginOrgName;
                }
                else
                {
                    //Response.Redirect("~/Accounts/Logout.aspx");
                }
            }
            catch
            {
            }
            return false;
        }

        //DMS5-4052 BS - New methods created in common place to render menus
        #region Menu

        public void LoadMenus(Menu menuControl, int applicationId = 1)
        {
            DataTable dtMenus = new DataTable();
            DataSet dsMenus = GetMenuData();

            string url = string.Empty;
            if (applicationId.Equals(1))
                url = "~/Workflow/WorkflowAdmin/WorkFlowHome.aspx"; // redirect to this url if menus not exists for 1 i.e., DMS
            else if (applicationId.Equals(2))
                url = "~/secure/Home.aspx"; // redirect to this url if menus not exists for 2 i.e., Workflow

            if (dsMenus.Tables.Cast<DataTable>()
                    .Any(table => table.Rows.Count != 0))
            {
                dtMenus = dsMenus.Tables[0].Select("ApplicationId = " + applicationId).Length > 0
                    ? dsMenus.Tables[0].Select("ApplicationId = " + applicationId)
                    .CopyToDataTable()
                    : new DataTable();
                if (dtMenus.Rows.Count > 2) // By default one row will be there as an Tab item
                    LoadMenus(dtMenus, menuControl);
                // if menus exists for another application redirect to same
                else
                {
                    applicationId = 2;
                    if (dsMenus.Tables[0].Select("ApplicationId = " + applicationId).Length > 0)
                        Response.Redirect("~/Workflow/WorkflowAdmin/WorkFlowHome.aspx", false);
                }
            }
        }

        private DataSet GetMenuData()
        {
            DataSet dsTemp = new DataSet();
            if (Session["User_MenuData"] != null)
            {
                dsTemp = (DataSet)Session["User_MenuData"];
            }
            else
            {
                UserBase user = (UserBase)Session["LoggedUser"];
                GroupBL bl = new GroupBL();
                dsTemp = bl.GetMenuItemDatasetByGroupId(user.GroupId, string.Empty, "GetMenuPermissionByUserId", user.LoginOrgId.ToString(), user.LoginToken);
                Session["User_MenuData"] = dsTemp;
            }
            return dsTemp;
        }

        private void LoadMenus(DataTable table, Menu menuControl)
        {
            var mainMenus = table.Select("(menu_parent_id is null or menu_parent_id = 0)");

            foreach (DataRow row in mainMenus)
            {
                string menuName = row["menu_name"].ToString();

                //bind only if menu has child or url 
                if (table.Select("len(menu_url)>2 or menu_parent_id = '" + row["menu_id"].ToString() + "'").Length > 0)
                {
                    string iconPath = string.Empty;

                    if (!menuName.Length.Equals(0))
                        iconPath = "~/Assets/Skin/Images/Menu/" + menuName + ".png";

                    if (!File.Exists(Server.MapPath(iconPath)))
                        iconPath = string.Empty;

                    MenuItem menuItem = new MenuItem(menuName, row["menu_id"].ToString(), iconPath);
                    menuItem.NavigateUrl = row["menu_url"].ToString();

                    if (!(menuItem.NavigateUrl.Length > 1))
                        menuItem.Selectable = false;


                    menuControl.Items.Add(menuItem);
                    AddChildItems(table, menuItem);
                }
            }
        }

        private void AddChildItems(DataTable table, MenuItem menuItem)
        {
            foreach (DataRow row in table.Select("menu_parent_id=" + menuItem.Value))
            {
                string menuName = row["menu_name"].ToString();
                string iconPath = string.Empty;

                if (!menuName.Length.Equals(0))
                    iconPath = "~/Assets/Skin/Images/Menu/" + menuName + ".png";

                if (!File.Exists(Server.MapPath(iconPath)))
                    iconPath = string.Empty;

                MenuItem childItem = new MenuItem(menuName, row["menu_id"].ToString(), iconPath);

                childItem.NavigateUrl = row["menu_url"].ToString();
                menuItem.ChildItems.Add(childItem);
                AddChildItems(table, childItem);
            }
        }

        #endregion
        //DMS5-4052 BE
    }
}
