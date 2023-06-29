/* ===================================================================================================================================
 * Author     : 
 * Create date: 
 * Description:  
======================================================================================================================================  
 * Change History   
 * Date:            Author:             Issue ID            Description:  
 * ----------       -------------       ----------          ------------------------------------------------------------------
 * 04 Apr 2015      Sabina              DMS5-3933	        Dynamic menu creation in workflow
 * 18 Apr 2015      Yogeesha Naik       DMS5-3933	        Menu filtering and redirecting page 
 * 27 Apr 2015      Yogeesha Naik       DMS5-4052	        Menu - Re factor menu implementation
====================================================================================================================================== */

using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lotex.EnterpriseSolutions.CoreBE;
using WorkflowBAL;
using WorkflowBLL.Classes;
using System.Collections;
using System.Data;
using Lotex.EnterpriseSolutions.CoreBL;

namespace Lotex.EnterpriseSolutions.WebUI
{
    public partial class WorkflowAdmin : MasterPageBase
    {
        WorkflowLanguages objLanguages = new WorkflowLanguages();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["LoggedUser"] == null)
                {
                    Response.Redirect("~/Accounts/LogOut.aspx");
                }
                else
                {
                    imgLogo.Src = Convert.ToString(Session["LoginOrgLogoPath"]);
                    imgLogo.Alt = Convert.ToString(Session["LoginOrgName"]);

                    lblCompanyName.Text = Convert.ToString(Session["LoginOrgName"]);
                }

               

                if (!Page.IsPostBack)
                {
                    LoadMenus(mnuMain, 2); // DMS5-4052 M - Removed old methods and created new.
                    BindLanguages(GetLanguages());
                }
                UserBase user = (UserBase)Session["LoggedUser"];
                lblUser.Text = "User Name : " + user.FirstName + " " + user.LastName;
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception: " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        private DBResult GetLanguages()
        {
            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            objLanguages.LoginToken = loginUser.LoginToken;
            objLanguages.LoginOrgId = loginUser.LoginOrgId;
            objLanguages.WfLanguagesAction = string.Empty;
            return objLanguages.GetAllLanguages(objLanguages);
        }


        protected void BindLanguages(DBResult objResult)
        {
            try
            {
                if (objResult.dsResult != null && objResult.dsResult.Tables.Count > 0)
                {
                    ddlLanguages.DataTextField = "WorkFlowLanguages_vLanguageText";
                    ddlLanguages.DataValueField = "LanguageCodePlusID";
                    ddlLanguages.DataSource = objResult.dsResult.Tables[0];
                    ddlLanguages.DataBind();
                }

                if (Session["LanguageCode"] != null && Session["LanguageID"] != null
                    && Session["LanguageCode"].ToString() != string.Empty
                    && Session["LanguageID"].ToString() != string.Empty)
                {
                    ddlLanguages.SelectedValue = Session["LanguageCode"] + "(" + Session["LanguageID"] + ")";
                }
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception: " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Accounts/LogOut.aspx?msg=Logout", false);
        }

        protected void ddlLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

                string strLangId = ddlLanguages.SelectedValue.Split('(')[1].ToString();
                strLangId = strLangId.Substring(0, strLangId.Length - 1);

                string strLangCode = ddlLanguages.SelectedValue.Split('(')[0].ToString();

                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objLanguages.LoginToken = loginUser.LoginToken;
                objLanguages.LoginOrgId = loginUser.LoginOrgId;
                objLanguages.WfLanguagesAction = "SaveUserLanguagePreference";
                objLanguages.WfLanguagesId = Convert.ToInt32(strLangId);
                objLanguages.SaveUserLanguagePreference(objLanguages);

                Session["LanguageID"] = strLangId;
                Session["LanguageCode"] = strLangCode;
                loginUser.LanguageID = Convert.ToInt32(strLangId);
                loginUser.LanguageCode = strLangCode;
                Session["LoggedUser"] = loginUser;
                Response.Redirect(Request.Url.ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception: " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

    }
}