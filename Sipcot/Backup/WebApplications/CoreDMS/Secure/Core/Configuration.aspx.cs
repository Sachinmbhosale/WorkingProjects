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
    public partial class Configuration : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //CheckAuthentication();
            }
        }

        protected void btnExit_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Account/Login.aspx");
        }
        /// <summary>
        /// This function re-set the securit key.
        /// This is recomented only at the stage of Initial Configuration.
        /// </summary>
        /// <returns></returns>
        protected bool ReConfigureSecurityKey()
        {
            //SecurityBL bl = new SecurityBL();
            //int resultCount = bl.ReConfigureSecurityKey();
            //if (resultCount > 0)
            //{
            //    ScriptManager.RegisterStartupScript(this, this.GetType(), "Success Alert", "alert('Re-Configure SecurityKey is completed successfully');", true);
            //}
            //else
            //{
            //    ScriptManager.RegisterStartupScript(this, this.GetType(), "Error Alert", "alert('Re-Configure SecurityKey is failed');", true);
            //    return false;
            //}
            return true;
        }
        /// <summary>
        /// To check the DB Connectivity is there.
        /// </summary>
        /// <returns></returns>
        protected bool DatabaseCheck()
        {
            bool status = false;
            SecurityBL bl = new SecurityBL();
            int resultCount =0;// bl.DatabaseCheck();
            if (resultCount > 0)
            {
                status =  true;
            }            
            return status;
        }
        /// <summary>
        /// Execute DB check
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDBCheck_Click(object sender, EventArgs e)
        {
            if (DatabaseCheck())
            {
                //lblDBCheck.Text = "Success";
                //btnCreateKey.Visible = true;
                //lblCreateKey.Text = "Pending";
                //lblCreateKey.Visible = true;
            }
        }
        /// <summary>
        /// Execute Security Key Creation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnReSetScurityKey_Click(object sender, EventArgs e)
        {
            if (ReConfigureSecurityKey())
            {
                //lblCreateKey.Text = "Success";
                //btnGlobalOrg.Visible = true;
                //lblGlobalOrg.Text = "Pending";
                //lblGlobalOrg.Visible = true;
            }

        }
        /// <summary>
        /// To add a global customer(One time activity
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCreateGlobalOrg_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Secure/Admin/ManageCustomer.aspx");
        }

        protected void btnReConfigureSecurityKey_Click(object sender, EventArgs e)
        {

        }

        protected void btnClose_Click(object sender, EventArgs e)
        {

        }

   
    }
}
