/* ===================================================================================================================================
 * Author     : 
 * Create date: 
 * Description:  
======================================================================================================================================  
 * Change History   
 * Date:            Author:             Issue ID            Description:  
 * ----------       -------------       ----------          ------------------------------------------------------------------
 * 21 Mar 2015      Yogeesha Naik       DMS04-3459	        Renamed page name (original aspx page name) 
 * 18 Apr 2015      Yogeesha Naik       DMS5-3933	        Menu filtering and redirecting page
 * 27 Apr 2015      Yogeesha Naik       DMS5-4052	        Menu - Re factor menu implementation
====================================================================================================================================== */

using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Lotex.EnterpriseSolutions.CoreBE;
using System.Collections;

namespace Lotex.EnterpriseSolutions.WebUI
{
    public partial class DefaultlMaster : MasterPageBase
    {
        #region Page Laod
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LoggedUser"] == null)
            {
                Response.Redirect("~/Accounts/LogOut.aspx");
            }
            else
            {
               imgLogo.Src = GetCompanyLogoPath();
               imgLogo.Alt = GetCompanyName();
                UserBase user = (UserBase)Session["LoggedUser"];
                lblUser.Text = "Hi! " + user.FirstName + " " + user.LastName;

                if (!IsPostBack)
                {

                    LoadMenus(mnuMain); // DMS5-4052 M - Removed old methods and created new.
                }
            }

            TodayDate.Text = DateTime.Now.ToString("yyyy-MM-dd hh:mm:tt");

            ClearApplicationCache();

            //Disable the enter key for logout. Explicit click only
            btnLogout.Attributes.Add("onclick", "javascript: return swallowEnter();");
        }
        #endregion

        public void ClearApplicationCache()
        {
            List<string> keys = new List<string>();


            // retrieve application Cache enumerator
            IDictionaryEnumerator enumerator = Cache.GetEnumerator();


            // copy all keys that currently exist in Cache
            while (enumerator.MoveNext())
            {
                keys.Add(enumerator.Key.ToString());
            }


            // delete every key from cache
            for (int i = 0; i < keys.Count; i++)
            {
                Cache.Remove(keys[i]);
            }
        }

        protected void mnuMain_MenuItemClick(object sender, MenuEventArgs e)
        {
            Session["CurrentReportSubMenu"] = null;
        }

        protected void lnkChangepassword_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Default/ChangePassword.aspx", false);
        }

        public void RedirectToAccessDeniedPage()
        {
            Response.Redirect("~/Secure/UnAuthorised.aspx");
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Accounts/LogOut.aspx?msg=Logout", false);
        }
    }
}
