/* ============================================================================  
Author     : Joby
Create date: 
Description: 
===============================================================================  
** Change History   
** Date:          Author:            Issue ID                   Description:  
** ----------   -------------       ----------                  ----------------------------
16 06 2015     Gokuldas.Palapatta   DMSENH6-4655      Audit Trails & Logs

=============================================================================== */



using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lotex.EnterpriseSolutions.CoreBL;
using Lotex.EnterpriseSolutions.CoreBE;

namespace Lotex.EnterpriseSolutions.WebUI
{
    public partial class LogOut : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //string orName = Session["LoginOrgCode"] != null ? Session["LoginOrgCode"].ToString() : string.Empty;
                string orName = Request.Cookies["Orgcode"].Value != null ? Request.Cookies["Orgcode"].Value.ToString() : string.Empty;


                if (Request.QueryString["msg"].ToString() == "Logout")
                {
                    TraceLogout();
                    LogOutAndRedirection(orName, "You have logged out successfully.");
                }
                else if (Request.QueryString["msg"].ToString() == "ForceLogout")
                {
                    TraceLogout();
                    LogOutAndRedirection(orName, "You have been logged out due to inactivity.");
                }
                else if (Request.QueryString["msg"].ToString() == "sessionout")
                {
                    divMsg.InnerHtml = "Your session has been timedout. Please relogin to continue.";
                }
            }


        }
       /* DMSENH6-4655 BS*/
        protected void TraceLogout()
        {
            try
            {
                UserBL UserBL = new UserBL();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                DateTime time = DateTime.Now;             // Use current time.
                string format = "MMM d yyyy hh:mm tt";   // Use this format.

                UserBL.TraceLoginLogout(loginUser.UserId, "User " + loginUser.UserName + " Logged out on " + time.ToString(format), "User");
            }
            catch { }
        }
       /*  DMSENH6-4655 BD*/
    }
}
