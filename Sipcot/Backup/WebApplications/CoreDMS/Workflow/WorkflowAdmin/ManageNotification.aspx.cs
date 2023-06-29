/* ===================================================================================================================================
 * Author     : 
 * Create date: 
 * Description:  
======================================================================================================================================  
 * Change History   
 * Date:            Author:             Issue ID            Description:  
 * -----------------------------------------------------------------------------------------------------------------------------------

 * 17 Apr 2015      Gokul               DMS5-3946       Applying rights an permissions in all pages as per user group rights!
 * 08 Apr 2015      Sabina              DMS5-4122       Button not enabled in workflow pages
 * 10/08/2015    sharath             DMSENH6-4732            Deletion options within Workflow
====================================================================================================================================== */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WorkflowBLL.Classes;
using Lotex.EnterpriseSolutions.CoreBE;
using WorkflowBAL;
using System.Data;
using System.Drawing;


namespace Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin
{
    public partial class ManageNotification : PageBase
    {
        #region GLOBAL DECLARATION

        WorkflowNotification objNotification = new WorkflowNotification();
        public string pageRights = string.Empty; /* DMS5-3946 A */
       
        #endregion
      
        protected void Page_Load(object sender, EventArgs e)
        {
            CheckAuthentication();

            //Start: ------------------SitePath Details ----------------------
            Label sitePath = (Label)Master.FindControl("SitePath");
            sitePath.Text = "WorkFlow >> Process >> Workflow >> Stage >> Manage Notifications";
            //End: ------------------SitePath Details ------------------------

            //Disable buttons if the workflow is confirmed
            if ((Session["workflowConfirmed"]) != null && (Session["workflowConfirmed"]).Equals("Yes"))
            {
                DisableControls();
            }
            // DMSENH6-4732 BE

            pageRights = GetPageRights("CONFIGURATION");/* DMS5-3946 A,DMS5-4122M added parameter configuration */
            if (!Page.IsPostBack)
            {

                try
                {
                    ApplyPageRights(pageRights, this.Form.Controls); /* DMS5-3946 A */
                    if (Request.QueryString["StageId"] != null)
                    {
                        // Store process id in view state for later use
                        Session["ProcessId"] =Decrypt(HttpUtility.UrlDecode(Request.QueryString["ProcessId"]));
                        Session["WorkflowId"] =Decrypt(HttpUtility.UrlDecode(Request.QueryString["WorkflowId"]));
                        Session["StageId"] = Decrypt(HttpUtility.UrlDecode(Request.QueryString["StageId"]));

                        Session["StageName"] = Decrypt(HttpUtility.UrlDecode(Request.QueryString["StageName"]));

                        lblCurrentStageNameHeaderValue.Text = Session["StageName"].ToString();

                        GetStatus();
                        GetCategory();
                    }
                    else
                    {
                        throw new Exception("Querystring empty.");
                    }

                }
                catch
                {
                }

                BindNotificationGridview(GetNotification());

            }

        }
        private DBResult GetNotification()
        {
            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            objNotification.LoginToken = loginUser.LoginToken;
            objNotification.LoginOrgId = loginUser.LoginOrgId;
            objNotification.NotificationProcessId = Convert.ToInt32(Session["ProcessId"]);
            objNotification.NotificationWorkflowId = Convert.ToInt32(Session["WorkflowId"]);
            objNotification.NotificationStageId = Convert.ToInt32(Session["StageId"]);
            return objNotification.ManageNotificationConfiguration(objNotification, "GetAllNotifications");

        }

        protected void btnAddNotification_Click(object sender, EventArgs e)
        {

        }

        private void GetStatus()
        {
            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            objNotification.LoginToken = loginUser.LoginToken;
            objNotification.LoginOrgId = loginUser.LoginOrgId;
            objNotification.NotificationProcessId = Convert.ToInt32(Session["ProcessId"]);
            objNotification.NotificationWorkflowId = Convert.ToInt32(Session["WorkflowId"]);
            objNotification.NotificationStageId = Convert.ToInt32(Session["StageId"]);
            objResult = objNotification.ManageNotificationConfiguration(objNotification, "GetallStatusoftheStage");

            ddl_Status.DataSource = objResult.dsResult;
            ddl_Status.DataValueField = "WorkflowStageStatus_iId";
            ddl_Status.DataTextField = "WorkflowStageStatus_vName";
            ddl_Status.DataBind();

        }

        private void GetCategory()
        {
            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            objNotification.LoginToken = loginUser.LoginToken;
            objNotification.LoginOrgId = loginUser.LoginOrgId;
            objNotification.NotificationProcessId = Convert.ToInt32(Session["ProcessId"]);
            objNotification.NotificationWorkflowId = Convert.ToInt32(Session["WorkflowId"]);
            objNotification.NotificationStageId = Convert.ToInt32(Session["StageId"]);
            objResult = objNotification.ManageNotificationConfiguration(objNotification, "GetNotificationCategory");

            ddl_Category.DataSource = objResult.dsResult;
            ddl_Category.DataValueField = "GEN_vDescription";
            ddl_Category.DataTextField = "GEN_vDescription";
            ddl_Category.DataBind();
        }
        protected void BindNotificationGridview(DBResult objResult)
        {

            gridNotification.DataSource = objResult.dsResult.Tables[0];
            gridNotification.DataBind();

            //Multilanguage implementation for Grid headers
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            if (loginUser.LanguageCode != "en-US")
            {
                WorkFlowCommonFns.SetGridHeaderForMultiLanguage(ref gridNotification, "NOTIFICATION", loginUser.LoginToken, loginUser.LoginOrgId, loginUser.LanguageID, loginUser.UserId);
            }

        }

        protected void gridNotification_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[1].Style.Add("display", "none");
                e.Row.Cells[2].Style.Add("display", "none");
                e.Row.Cells[7].Style.Add("display", "none");
                e.Row.Cells[8].Style.Add("display", "none");
                e.Row.Cells[9].Style.Add("display", "none");
                e.Row.Cells[10].Style.Add("display", "none");
                e.Row.Cells[11].Style.Add("display", "none");

                e.Row.Cells[0].Style.Add("width", "20px");
                e.Row.Cells[3].Style.Add("width", "70px");
                e.Row.Cells[4].Style.Add("width", "70px");
                e.Row.Cells[5].Style.Add("width", "20px");
                e.Row.Cells[6].Style.Add("width", "20px");
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center; //edit
                e.Row.Cells[5].HorizontalAlign = HorizontalAlign.Center; //active
                e.Row.Cells[6].HorizontalAlign = HorizontalAlign.Center; //TAT
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            CheckAuthentication();
            string action = "AddNotificationConfiguration";
            DBResult objResult = new DBResult();

            try
            {
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objNotification.LoginToken = loginUser.LoginToken;
                objNotification.LoginOrgId = loginUser.LoginOrgId;

                objNotification.NotificationProcessId = Convert.ToInt32(Session["ProcessId"]);
                objNotification.NotificationWorkflowId = Convert.ToInt32(Session["WorkflowId"]);
                objNotification.NotificationStageId = Convert.ToInt32(Session["StageId"]);
                objNotification.NotificationStatusId = Convert.ToInt32(ddl_Status.SelectedValue);
                objNotification.NotificatontTATDurationTime = Convert.ToInt32(txt_TAT.Text.Trim());
                objNotification.NotificationCategory = Convert.ToString(ddl_Category.SelectedItem.Text);
                objNotification.NotificationIsActive = chkActive.Checked;
                string strSaveStatus = hdnSaveStatus.Value;



                if (strSaveStatus == "Save Changes")
                {
                    objNotification.NotificationId = Convert.ToInt32(hdnNotificationid.Value);
                    action = "EditNotificationConfiguration";
                }

                objResult = objNotification.ManageNotificationConfiguration(objNotification, action);
                handleDBResult(objResult);


                if (objResult.ErrorState == 0)
                {
                    clearControls();
                    BindNotificationGridview(GetNotification());
                    //hdnErrorStatus.Value = "";
                }
                else
                {
                    if (strSaveStatus == "Save Changes")
                    {
                        hdnErrorStatus.Value = "EDIT_ERROR";
                    }
                    else
                    {
                        hdnErrorStatus.Value = "ADD_ERROR";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void clearControls()
        {
            txt_TAT.Text = string.Empty;
            //ddl_Category.SelectedValue = string.Empty;
            //ddl_Status.SelectedValue = string.Empty;
            chkActive.Checked = false;
        }

        private void handleDBResult(DBResult objResult)
        {
            if (objResult.ErrorState == 0)
            {
                // Success
                if (objResult.Message.Length > 0)
                    lblMessage.Text = (objResult.Message);
              

            }
            else if (objResult.ErrorState == -1)
            {
                // Warning
                if (objResult.Message.Length > 0)
                {
                    lblMessage.Text =(objResult.Message);
                   
                }
            }
            else if (objResult.ErrorState == 1)
            {
                // Error
                if (objResult.Message.Length > 0)
                {
                    lblMessage.Text = (objResult.Message);
                   
                }
            }
        }

        protected void btnGoBacktoStage_Click(object sender, EventArgs e)
        {
            CheckAuthentication();

            string ProcessId = Session["ProcessId"].ToString(); //ProcessId
            string WorkflowId = Session["WorkflowId"].ToString(); //WorkflowId
            string workflowConfirmed = string.Empty;

            ProcessId = HttpUtility.UrlEncode(Encrypt(ProcessId));
            WorkflowId = HttpUtility.UrlEncode(Encrypt(WorkflowId));
            workflowConfirmed = HttpUtility.UrlEncode(Encrypt(workflowConfirmed));
            string QueryString = "?ProcessId=" + ProcessId + "&WorkflowId=" + (WorkflowId) + "&WorkflowConfirmed=" + workflowConfirmed;

            Response.Redirect("~/Workflow/WorkflowAdmin/ManageStages.aspx" + QueryString);
        }

        protected void gridNotification_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            CheckAuthentication();
            gridNotification.PageIndex = e.NewPageIndex;
            BindNotificationGridview(GetNotification());
        }
        // DMSENH6-4732 BS
        protected void DisableControls()
        {
            btnSave.Enabled = false;
           
        }
        // DMSENH6-4732 BE

    }
}