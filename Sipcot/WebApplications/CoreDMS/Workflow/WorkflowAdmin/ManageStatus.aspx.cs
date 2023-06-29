using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WorkflowBAL;
using WorkflowBLL.Classes;
using Lotex.EnterpriseSolutions.CoreBE;
using System.Data;


/*
=============================================================================  
** File			: USP_Workflow_ManageStageStatuses
** Author		: Robin Thomas / Pavana
** Creation On	: 10 Oct 2014
** Description	: To manage stage Statuses
===============================================================================  
 * ===============================================================================  
** Change History   
** Date:          Author:            Issue ID                         Description:  
** ----------   -------------       ----------                  ----------------------------

*08 Apr 2015     Sabina              DMS5-4122       Button not enabled in workflow pages
*21 May 2015     Sharath             DMS5-4268       We require the Delete option for Unwanted Stages & Status
*02 Jun 2015     Sharath             DMS5-4332       A completed workflow should not be editable or reopened once last stage has Closed/Rejected.
*10/08/2015      sharath             DMSENH6-4732    Deletion options within Workflow
=============================================================================== */

namespace Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin
{
    public partial class ManageStatus : PageBase
    {

        #region GLOBAL DECLARATION

        WorkflowStageStatus objStatus = new WorkflowStageStatus();
        public string pageRights = string.Empty; /* DMS5-3946 A */

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            CheckAuthentication();
            pageRights = GetPageRights("CONFIGURATION");/* DMS5-3946 A,DMS5-4122M added parameter configuration */

            // DMSENH6-4732 BS
            //Disable buttons if the workflow is confirmed
            if ((Session["workflowConfirmed"]) != null && (Session["workflowConfirmed"]).Equals("Yes"))
            {
                DisableControls();
            }
            // DMSENH6-4732 BE

            //Start: ------------------SitePath Details ----------------------
            Label sitePath = (Label)Master.FindControl("SitePath");
            sitePath.Text = "WorkFlow >> Process >> Workflow >> Stage >> Manage Statuses";
            //End: ------------------SitePath Details ------------------------

            if (!Page.IsPostBack)
            {
                try
                {
                    ApplyPageRights(pageRights, this.Form.Controls); /* DMS5-3946 A */
                    if (Request.QueryString["ProcessId"] != null)
                    {
                        // Store process id in view state for later use
                        Session["ProcessId"] = Decrypt(HttpUtility.UrlDecode(Request.QueryString["ProcessId"]));
                        Session["WorkflowId"] = Decrypt(HttpUtility.UrlDecode(Request.QueryString["WorkflowId"]));
                        Session["StageId"] = Decrypt(HttpUtility.UrlDecode(Request.QueryString["StageId"]));

                        Session["StageName"] = Decrypt(HttpUtility.UrlDecode(Request.QueryString["StageName"]));

                        lblCurrentStageNameHeaderValue.Text = Session["StageName"].ToString();

                        GetAllActiveStages();
                    }
                    else
                    {
                        throw new Exception("Querystring empty.");
                    }

                }
                catch
                {
                }

                BindStatusGridview(GetAllstatus());
            }

        }

        #region "Stages"

        private DBResult GetAllstatus()
        {
            int ProcessId = Convert.ToInt32(Session["ProcessId"]);
            int WorkflowId = Convert.ToInt32(Session["WorkflowId"]);
            int StageId = Convert.ToInt32(Session["StageId"]);

            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            objStatus.LoginToken = loginUser.LoginToken;
            objStatus.LoginOrgId = loginUser.LoginOrgId;

            objStatus.WorkflowStageStatusesProcessId = ProcessId;
            objStatus.WorkflowStageStatusesWorkFlowId = WorkflowId;
            objStatus.WorkflowStageStatusesStageId = StageId;


            return objStatus.ManageWorkflowStageStatuses(objStatus, "GetAllStageStatuses");

        }


        protected void btnSave_Click(object sender, EventArgs e)
        {
            CheckAuthentication();
            string action = "AddStage";
            lblMessage.Text = null;

            DBResult objResult = new DBResult();
            try
            {
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objStatus.LoginToken = loginUser.LoginToken;
                objStatus.LoginOrgId = loginUser.LoginOrgId;

                objStatus.WorkflowStageStatusesName = txtStatusName.Text.Trim();
                objStatus.WorkflowStageStatusesDescription = txtStatusDescription.Text.Trim();
                objStatus.WorkflowStageStatusesIsActive = chkActive.Checked;
                string strSaveStatus = hdnSaveStatus.Value;
                objStatus.WorkflowStageStatusCurrentstatusId = Convert.ToInt32(hdncurrentstatusid.Value);
                objStatus.WorkflowStageStatusesConfirmMsgOnSubmit = txtConfirmMsgOnSubmit.Text.Trim();
                objStatus.WorkflowStageStatusesConfirmOnSubmit = chkConfirmOnSubmit.Checked;
                objStatus.WorkflowStageStatusesSendNotification = chkSendNotification.Checked;
                objStatus.WorkflowStageStatusesMoveToStageId = Convert.ToInt32(ddl_Stages.SelectedValue);
                if (strSaveStatus == "Save Changes")
                {

                    objStatus.WorkflowStageStatusesMasterId = Convert.ToInt32(hdnStatusId.Value);
                    objStatus.WorkflowStageStatusesStageId = Convert.ToInt32(Session["StageId"]);
                    objStatus.WorkflowStageStatusesWorkFlowId = Convert.ToInt32(Session["WorkflowId"]);
                    objStatus.WorkflowStageStatusesProcessId = Convert.ToInt32(Session["ProcessId"]);
                    objStatus.WorkflowStageStatusesIsActive = chkActive.Checked ? true : false;
                    action = "EditStageStatuses";
                }


                objResult = objStatus.ManageWorkflowStageStatuses(objStatus, action);
                handleDBResult(objResult);

                //only if success
                if (objResult.ErrorState == 0)
                {
                    BindStatusGridview(GetAllstatus());
                    clearControls();
                    hdnErrorStatus.Value = "";
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
                    lblMessage.Text = (objResult.Message);

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

        private void clearControls()
        {
            txtStatusName.Text = string.Empty;
            txtStatusDescription.Text = string.Empty;
            chkActive.Checked = false;
            //lblMessage.Text = string.Empty;
        }

        protected void BindStatusGridview(DBResult objResult)
        {
            if (objResult.dsResult != null)
            {
                if (objResult.dsResult.Tables.Count > 0)
                {
                    // First table contains mapped stages

                    gridStage.DataSource = objResult.dsResult.Tables[0];
                    gridStage.DataBind();

                    //Multilanguage implementation for Grid headers
                    UserBase loginUser = (UserBase)Session["LoggedUser"];
                    if (loginUser.LanguageCode != "en-US")
                    {
                        WorkFlowCommonFns.SetGridHeaderForMultiLanguage(ref gridStage, "STAGESTATUS", loginUser.LoginToken, loginUser.LoginOrgId, loginUser.LanguageID, loginUser.UserId);
                    }
                }

                if (objResult.dsResult.Tables.Count > 1)
                {
                    // Second table contains available stages from master stages
                    Session["GridstageMasterData"] = objResult.dsResult.Tables[1];
                    gridStageMaster.DataSource = objResult.dsResult.Tables[1];
                    gridStageMaster.DataBind();

                    //Multilanguage implementation for Grid headers
                    UserBase loginUser = (UserBase)Session["LoggedUser"];
                    if (loginUser.LanguageCode != "en-US")
                    {
                        WorkFlowCommonFns.SetGridHeaderForMultiLanguage(ref gridStageMaster, "STATUSLIST", loginUser.LoginToken, loginUser.LoginOrgId, loginUser.LanguageID, loginUser.UserId);
                    }
                }
            }
        }



        #endregion
        protected void btCancelStageEditDiv_Click(object sender, EventArgs e)
        {
            CheckAuthentication();
        }


        protected void btnOk_Click(object sender, EventArgs e)
        {
            CheckAuthentication();
            string CommaSeparatedStatusIds = string.Empty;

            foreach (GridViewRow row in gridStageMaster.Rows)
            {
                CheckBox chk = row.FindControl("ChkStageMaster") as CheckBox;

                if (chk.Checked)
                {
                    CommaSeparatedStatusIds += row.Cells[1].Text + ","; //StatusId                   
                }
            }

            DBResult objResult = new DBResult();

            UserBase loginUser = (UserBase)Session["LoggedUser"];
            objStatus.LoginToken = loginUser.LoginToken;
            objStatus.LoginOrgId = loginUser.LoginOrgId;

            int ProcessId = Convert.ToInt32(Session["ProcessId"]);
            int WorkflowId = Convert.ToInt32(Session["WorkflowId"]);
            int StageId = Convert.ToInt32(Session["StageId"]);

            objStatus.WorkflowStageStatusesWorkFlowId = WorkflowId;
            objStatus.WorkflowStageStatusesStageId = StageId;
            objStatus.WorkflowStageStatusesProcessId = ProcessId;
            objStatus.CommaSeparatedStatusIds = CommaSeparatedStatusIds;

            //insert into status table

            objResult = objStatus.ManageWorkflowStageStatuses(objStatus, "AddStageStatuses");


            BindStatusGridview(GetAllstatus());
        }




        protected void gridStatusMaster_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            //hide Unwanted columns
            if (e.Row.Cells.Count > 1)
            {
                //hide Unwanted columns
                e.Row.Cells[1].Style.Add("display", "none"); //[status id]


                e.Row.Cells[0].Style.Add("width", "20px");
                e.Row.Cells[2].Style.Add("width", "100px"); //[Status Name] 
                e.Row.Cells[3].Style.Add("width", "200px"); //[Status Description]

                //DMS5-4332 BS 
                // Html encode
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    foreach (TableCell cell in e.Row.Cells)
                        if (cell.Text.Length > 0)
                            cell.Text = HttpUtility.HtmlDecode(cell.Text);
                }
                ////DMS5-4332 BE
            }
        }

        protected void gridStage_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //hide Unwanted columns
            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
            {
                //hide Unwanted columns
                e.Row.Cells[2].Style.Add("display", "none"); //[CurrstageStatusesId]
                e.Row.Cells[3].Style.Add("display", "none"); //[[StatusMasterId]]
                e.Row.Cells[6].Style.Add("display", "none"); //[StageId]
                e.Row.Cells[12].Style.Add("display", "none"); //[WorkflowId]
                e.Row.Cells[13].Style.Add("display", "none"); //[ProcessId]
                e.Row.Cells[14].Style.Add("display", "none"); //[OrgId] 
                e.Row.Cells[8].Style.Add("display", "none"); //[[ActiveStatus]]
                e.Row.Cells[15].Style.Add("display", "none"); //[ConfirmationStatus]
                e.Row.Cells[16].Style.Add("display", "none"); //[NotificationStatus]
                e.Row.Cells[17].Style.Add("display", "none"); //[Move To Stage Id]

                e.Row.Cells[0].Style.Add("width", "20px");  //[Edit]
                e.Row.Cells[1].Style.Add("width", "20px");  //[Serial No]
                e.Row.Cells[4].Style.Add("width", "70px"); //[Status Name]
                e.Row.Cells[5].Style.Add("width", "200px"); //[Status Description]
                e.Row.Cells[7].Style.Add("width", "40px"); //[Active]

                e.Row.Cells[9].Style.Add("width", "40px");  //[Need Confirmation on Submit]
                e.Row.Cells[10].Style.Add("width", "70px"); //[Confirm Message]
                e.Row.Cells[11].Style.Add("width", "40px"); //[Send Notification]
                e.Row.Cells[18].Style.Add("width", "80px"); //[Move To Stage]
                e.Row.Cells[19].Style.Add("width", "40px"); //[Final Status]

            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center; //edit
                e.Row.Cells[7].HorizontalAlign = HorizontalAlign.Center;//[Active]
                e.Row.Cells[9].HorizontalAlign = HorizontalAlign.Center; //Need Confirmation on Submit
                e.Row.Cells[11].HorizontalAlign = HorizontalAlign.Center;//[Send Notification]
                e.Row.Cells[19].HorizontalAlign = HorizontalAlign.Center;//[Final Status]
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

        private void GetAllActiveStages()
        {
            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            WorkflowStage objStage = new WorkflowStage();

            objStage.LoginToken = loginUser.LoginToken;
            objStage.LoginOrgId = loginUser.LoginOrgId;
            objStage.ProcessId = Convert.ToInt32(Session["ProcessId"]);
            objStage.WorkflowId = Convert.ToInt32(Session["WorkflowId"]);
            objStage.StageId = Convert.ToInt32(Session["StageId"]); ;
            objResult = objStage.ManageWorkflowStages(objStage, "GetOtherActiveStages");

            ddl_Stages.DataSource = objResult.dsResult;
            ddl_Stages.DataValueField = "WorkflowStage_iId";
            ddl_Stages.DataTextField = "WorkflowStage_vDisplayName";
            ddl_Stages.DataBind();

        }

        protected void gridStageMaster_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            CheckAuthentication();
            gridStageMaster.PageIndex = e.NewPageIndex;
            gridStageMaster.DataSource = (DataTable)Session["GridstageMasterData"];
            gridStageMaster.DataBind();

            hdnErrorStatus.Value = "PAGE_CHANGE";

        }

        protected void gridStage_PageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            CheckAuthentication();
            gridStage.PageIndex = e.NewPageIndex;
            BindStatusGridview(GetAllstatus());

        }

        //  DMS5-4268 BS
        protected void btnYes_Click(object sender, EventArgs e)
        {
            DBResult objResult = new DBResult();

            UserBase loginUser = (UserBase)Session["LoggedUser"];
            objStatus.LoginToken = loginUser.LoginToken;
            objStatus.LoginOrgId = loginUser.LoginOrgId;

            int ProcessId = Convert.ToInt32(Session["ProcessId"]);
            int WorkflowId = Convert.ToInt32(Session["WorkflowId"]);
            int StageId = Convert.ToInt32(Session["StageId"]);
            int StatusId = Convert.ToInt32(ViewState["StatusId"]);

            objStatus.WorkflowStageStatusesWorkFlowId = WorkflowId;
            objStatus.WorkflowStageStatusesStageId = StageId;
            objStatus.WorkflowStageStatusesProcessId = ProcessId;
            objStatus.WorkflowStageStatusCurrentstatusId = StatusId;
            objResult = objStatus.ManageWorkflowStageStatuses(objStatus, "DeleteStageStatuses");
            if (objResult.ErrorState != 0)
            {
                lblErrorMessage.Text = objResult.Message.ToString();
                lblErrorMessage.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                lblErrorMessage.Text = objResult.Message.ToString();
                BindStatusGridview(GetAllstatus());
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            LinkButton btndetails = sender as LinkButton;
            GridViewRow gvrow = (GridViewRow)btndetails.NamingContainer;
            ViewState["StatusId"] = gvrow.Cells[2].Text;
            ViewState["StatusName"] = gvrow.Cells[4].Text;
            lblUser.Text = "Are you sure you want to delete " + ViewState["StatusName"] + " Status?";
            ModalPopup.Show();
        }

        //  DMS5-4268 BE


        // DMSENH6-4732 BS
        protected void DisableControls()
        {
            btnSave.Enabled = false;
            btnOk.Enabled = false;
            btnYes.Enabled = false;
        }
        // DMSENH6-4732 BE
    }
}
