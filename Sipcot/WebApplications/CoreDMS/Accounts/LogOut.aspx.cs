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
using System.Web;
using Lotex.EnterpriseSolutions.CoreBL;
using Lotex.EnterpriseSolutions.CoreBE;
using System.Collections;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace Lotex.EnterpriseSolutions.WebUI
{
    public partial class LogOut : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                UserBL UserBL = new UserBL();
                UserBase loginUser = (UserBase)Session["LoggedUser"];

                checkConcurrentUser(loginUser.LoginToken);

                ClearApplicationCache();

                Session.Clear();
                Session.RemoveAll();
                Session.Abandon();


                if (Request.Cookies["ASP.NET_SessionId"] != null)
                {
                    Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                    Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
                }

                //if (Request.Cookies["AuthToken"] != null)
                //{
                //    Response.Cookies["AuthToken"].Value = string.Empty;
                //    Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
                //}


                // Code disables caching by browser.
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                Response.Cache.SetNoStore();

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

                //UserBL.TraceLoginLogout(loginUser.UserId, "User " + loginUser.UserName + " Logged out on " + time.ToString(format), "User");
            }
            catch { }
        }


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

            //to clear all data for SSO
            Session.RemoveAll();
            Session.Clear();
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Cache.SetNoServerCaching();
            HttpContext.Current.Response.Cache.SetNoStore();
        }


        protected void checkConcurrentUser(string logintoken)
        {
            UserBL UserBL = new UserBL();
            UserBase loginUser = (UserBase)Session["LoggedUser"];

            string connectionstring = ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString.ToString();
            SqlConnection con = new SqlConnection(connectionstring);
            con.Open();
            SqlCommand cmd = new SqlCommand("ConcurrentUserlogout", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Logintoken", logintoken);
            //cmd.Parameters["@count"].Direction = ParameterDirection.Output;  
          //  cmd.Parameters.Add("@count", SqlDbType.VarChar, 20).Direction = ParameterDirection.Output;
            //st = Convert.ToInt32(cmd.ExecuteScalar());
            cmd.ExecuteNonQuery();

            con.Close();




        }


        /*  DMSENH6-4655 BD*/
    }
}
