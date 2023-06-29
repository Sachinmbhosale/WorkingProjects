using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WorkflowBLL.Classes;
using WorkflowBAL;
using Lotex.EnterpriseSolutions.CoreBE;
using System.Data;
using OfficeConverter;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;

namespace Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin
{
    public partial class ManageStudio : PageBase
    {
        WorkflowProcess objProcess = new WorkflowProcess();
        Workflows objWorkflow = new Workflows();
        ManageWorkflowStudio objStudio = new ManageWorkflowStudio();
        WorkflowStage objStage = new WorkflowStage();
        WorkflowUserMapping objUserMapping = new WorkflowUserMapping();
        WorkflowStageStatus objStatus = new WorkflowStageStatus();
        WorkflowNotification objNotification = new WorkflowNotification();

        string unConfirmedProcess = "GetAllActiveProcess";


        protected void Page_Load(object sender, EventArgs e)
        {
            
            if (!IsPostBack)
            {
                #region ProcessSection
                BindOrganizationDropDown();
                BindProcessTab(ReptUnConfirmedProcess, unConfirmedProcess);
                #endregion

                #region Workflow
                BindWorkflowProjectDropDown();
                BindSortOrderToListBox();
                #endregion

                #region Stages
               LoadDataEntryTypes();
                #endregion

               
            }
            
        }

        #region Process Section

        protected void BindOrganizationDropDown()
        {
            try
            {
                Logger.Trace("Binding organisation dropdown started(Drag and Drop)", Session["LoggedUserId"].ToString());
                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objWorkflow.LoginToken = loginUser.LoginToken;
                objWorkflow.LoginOrgId = loginUser.LoginOrgId;
                objResult = objWorkflow.ManageWorkflows(objWorkflow, "BindOrganizationDropDown");
                ddlOrganization.DataSource = objResult.dsResult;
                ddlOrganization.DataTextField = "ORGS_vName";
                ddlOrganization.DataValueField = "ORGS_iId";
                ddlOrganization.DataBind();
                Logger.Trace("Binding organisation dropdown finished(Drag and Drop)", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Binding organisation dropdown exception(Drag and Drop)", Session["LoggedUserId"].ToString());
            }

        }

        protected void BindProcessTab(Repeater rept, string action)
        {
            try
            {
                Logger.Trace("Binding active process to accordian started (Drag and Drop)", Session["LoggedUserId"].ToString());
                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objStudio.LoginToken = loginUser.LoginToken;
                objStudio.LoginOrgId = loginUser.LoginOrgId;
                objResult = objStudio.ManageStudio(objStudio, action);
                if (objResult.dsResult.Tables.Count > 0 && objResult.dsResult.Tables[0].Rows.Count > 0)
                {
                    rept.DataSource = objResult.dsResult.Tables[0];
                    rept.DataBind();
                }
                Logger.Trace("Binding active process to accordian finished (Drag and Drop)", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Binding active process to accordian exception (Drag and Drop)" + ex.Message.ToString(), Session["LoggedUserId"].ToString());
            }


        }

        protected void btnProcessEdit_Click(object sender, EventArgs e)
        {
            try
            {
               
                Logger.Trace("Process controls binding edit mode started(click on process (drag and drop))", Session["LoggedUserId"].ToString());
                lblMessageProcess.Text = string.Empty;
                chkProcessActive.Enabled = true;
                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objStudio.LoginToken = loginUser.LoginToken;
                objStudio.LoginOrgId = loginUser.LoginOrgId;
                Session["ProcessId"] = objStudio.ProcessId = Convert.ToInt32(hiddenProcessId.Value);
                objResult = objStudio.ManageStudio(objStudio, "GetProcessDetails");
                BindOrganizationDropDown();
                if (objResult.dsResult.Tables.Count > 0 && objResult.dsResult.Tables[0].Rows.Count > 0)
                {
                    DataRow dataRow = objResult.dsResult.Tables[0].Rows[0];
                    ddlOrganization.SelectedValue = Convert.ToString(dataRow["OrgName"]);
                    txtProcessName.Text = Convert.ToString(dataRow["ProcessName"]).Trim();
                    txtProcessDescription.Text = Convert.ToString(dataRow["ProcessDescription"]).Trim();
                    chkProcessActive.Checked = Convert.ToBoolean(dataRow["Active"]);

                    BindUsers(GetAllUsers());
                }
                if (hdnActionProcess.Value == "EditProcess")
                {
                    //Building workflows based on process 
                    objResult = objStudio.ManageStudio(objStudio, "GetWorkflowByProcessId");

                    if (objResult.dsResult.Tables.Count > 0 && objResult.dsResult.Tables[0].Rows.Count > 0)
                    {
                        int Count = objResult.dsResult.Tables[0].Rows.Count;
                        //Generateing workflow buttons based on process
                        Dictionary<int, string> dictionary = new Dictionary<int, string>();
                        foreach (DataRow row in objResult.dsResult.Tables[0].Rows)
                        {
                            int buttonId = Convert.ToInt32(row["ID"]);
                            string buttonName = Convert.ToString(row["Name"]);
                            dictionary.Add(buttonId, buttonName);
                        }

                        var values = string.Join(", ", dictionary.Select(m => m.Key + "-" + m.Value).ToArray());
                        ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "BuildWorkflows", "BuildWorkflows( '" + values + "');", true);
                    }
                    else
                    {
                        var values = string.Empty;
                        ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "BuildWorkflows", "BuildWorkflows( '" + values + "');", true);
                    }

                }
                Logger.Trace("Process controls binding edit mode finished(click on process (drag and drop))", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Click on process exception (drag and drop)" + ex.Message.ToString(), Session["LoggedUserId"].ToString());
            }
            pnlProcess.Style.Add("display", "block");
            pnlWorkflow.Style.Add("display", "none");
            pnlStages.Style.Add("display", "none");
            pnlStagesAdd.Style.Add("display", "none");         
            pnlStatus.Style.Add("display", "none");
            pnlStatusAdd.Style.Add("display", "none");
            pnlNotifications.Style.Add("display", "none");

        }

        protected void btnProcessCancel_Click(object sender, EventArgs e)
        {
            
            pnlProcess.Style.Add("display", "none");
            ClearProcessControls();
           
        }

        private void ClearProcessControls()
        {
            BindOrganizationDropDown();
            ddlOrganization.SelectedIndex.Equals(0);
            txtProcessName.Text = string.Empty;
            txtProcessDescription.Text = string.Empty;
        }

        protected void btnProcessSave_Click(object sender, EventArgs e)
        {

            string action = hdnActionProcess.Value;
            DBResult objResult = new DBResult();

            try
            {
                Logger.Trace("Process saving started (drag and drop)", Session["LoggedUserId"].ToString());
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objProcess.LoginToken = loginUser.LoginToken;
                objProcess.LoginOrgId = loginUser.LoginOrgId;

                objProcess.WfProcessName = txtProcessName.Text.Trim();
                objProcess.WfProcessDescription = txtProcessDescription.Text.Trim();
                objProcess.WfProcessIsActive = chkProcessActive.Checked;
                objProcess.WfProcessOrgId = Convert.ToInt32(ddlOrganization.SelectedValue.ToString());

              

                if (hdnActionProcess.Value == "EditProcess")
                {
                    Session["ProcessId"] = objProcess.WfProcessId = Convert.ToInt32(Session["ProcessId"]);
                }
                    objProcess.WfProcessIsDeleted = chkProcessActive.Checked ? false : true;
                  
                   
                objResult = objProcess.ManageProcess(objProcess, action);
                handleDBResult(objResult, lblMessageProcess);
                chkProcessActive.Checked = true;

                //only if success
                if (objResult.ErrorState == 0)
                {
                   
                   // ClearProcessControls();
                    // BindProcessGridview(GetProcess());
                    hdnErrorStatus.Value = "";

                 

                    if (hdnActionProcess.Value == "AddProcess")
                    {
                        
                        hiddenProcessSaveStatus.Value = "Yes";
                       // DMSENH6-5217
                        BindProcessTab(ReptUnConfirmedProcess, unConfirmedProcess);
                        int buttonId = 0;
                        if (objResult.dsResult.Tables.Count > 0 && objResult.dsResult.Tables[0].Rows.Count > 0)
                        {
                            string values = ReturnProcessDBValues(objResult.dsResult.Tables[0]);
                             buttonId = Convert.ToInt32(objResult.dsResult.Tables[0].Rows[0]["ProcessID"]);                            
                            ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "BuildProcess", "BuildProcess( '" + values + "');", true);
                        }
                       Session["ProcessId"]= hiddenProcessId.Value = buttonId.ToString();

                        //hdnActionProcess.Value = "EditProcess";
                    }

                    if (hdnActionProcess.Value == "EditProcess")
                    {
                       
                         objStudio.LoginToken = loginUser.LoginToken;
                        objStudio.LoginOrgId = loginUser.LoginOrgId;
                        objStudio.ProcessId = Convert.ToInt32(Session["ProcessId"]);
                            objResult = objStudio.ManageStudio(objStudio, "GetProcessById");
                            if (objResult.dsResult.Tables.Count > 0 && objResult.dsResult.Tables[0].Rows.Count > 0)
                            {
                                string values = ReturnProcessDBValues(objResult.dsResult.Tables[0]);
                                // DMSENH6-5217
                                BindProcessTab(ReptUnConfirmedProcess, unConfirmedProcess);
                                ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "BuildProcess", "BuildProcess( '" + values + "');", true);
                            }
                        
                    }
                    //handling process owners
                    DisplaySelectedEntityDetails(GetSelectedEntityDetails());
                    BindUsers(GetAllUsers());  

                }
                else
                {
                    try
                    {
                    
                        ddlOrganization.SelectedValue = hiddenOrganizationId.Value;
                       
                    }
                    catch
                    {
                    }
                   
                }
                Logger.Trace("Process saving finished (drag and drop)", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Process saving exception (drag and drop)" + ex.Message.ToString(), Session["LoggedUserId"].ToString());
            }
            pnlProcess.Style.Add("display", "block");

        }

        private string ReturnProcessDBValues(DataTable Dt)
        {
            //Binding process which are active only
            string values = string.Empty;
            try
            {
                Logger.Trace("Process retrieving database values started (drag and drop)", Session["LoggedUserId"].ToString());
               
                if (Dt.Rows.Count > 0)
                {
                    int Count = Dt.Rows.Count;
                    //Generateing workflow buttons based on process
                    Dictionary<int, string> dictionary = new Dictionary<int, string>();
                    foreach (DataRow row in Dt.Rows)
                    {
                        int buttonId = Convert.ToInt32(row["ProcessID"]);
                        string buttonName = Convert.ToString(row["ProcessName"]);
                        dictionary.Add(buttonId, buttonName);
                    }

                    values = string.Join(", ", dictionary.Select(m => m.Key + "-" + m.Value).ToArray());

                }
                else
                {
                    values = string.Empty;

                }
                Logger.Trace("Process retrieving database values started (drag and drop)", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            { Logger.Trace("Process retrieving database values exception (drag and drop)" + ex.Message.ToString(), Session["LoggedUserId"].ToString()); }

            return values;
        }

        private void handleDBResult(DBResult objResult, Label errorMessageLabel)
        {
            if (objResult.ErrorState == 0)
            {
                // Success
                if (objResult.Message.Length > 0)
                {
                    errorMessageLabel.Text = (objResult.Message);

                }
            }
            else if (objResult.ErrorState == -1)
            {
                // Warning
                if (objResult.Message.Length > 0)
                {
                    errorMessageLabel.Text = (objResult.Message);

                }
            }
            else if (objResult.ErrorState == 1)
            {
                // Error
                if (objResult.Message.Length > 0)
                {
                    errorMessageLabel.Text = (objResult.Message);

                }
            }
        }


        #endregion

        #region Workflow Section

        protected void BindWorkflowProjectDropDown()
        {
            try
            {
                Logger.Trace("Workflow project dropdown binding started (drag and drop)" , Session["LoggedUserId"].ToString());
                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objWorkflow.LoginToken = loginUser.LoginToken;
                objWorkflow.LoginOrgId = loginUser.LoginOrgId;
                objWorkflow.WorkFlowProcessId = Convert.ToInt32(Session["ProcessId"]);
                objResult = objWorkflow.ManageWorkflows(objWorkflow, "BindDMSProjectDropDown");
                ddlDmsProject.DataSource = objResult.dsResult;
                ddlDmsProject.DataTextField = "DOCTYPEMASTER_vName";
                ddlDmsProject.DataValueField = "DOCTYPEMASTER_iID";
                ddlDmsProject.DataBind();
                Logger.Trace("Workflow project dropdown binding finished (drag and drop)", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            { Logger.Trace("Workflow project dropdown binding exception (drag and drop)" + ex.Message.ToString(), Session["LoggedUserId"].ToString()); }

        }

        protected void btnWorkflowSave_Click(object sender, EventArgs e)
        {

            // CheckAuthentication();
            string action = hdnActionWorkflow.Value;
            try
            {
                Logger.Trace("Workflow saving started (drag and drop)", Session["LoggedUserId"].ToString());
                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objWorkflow.LoginToken = loginUser.LoginToken;
                objWorkflow.LoginOrgId = loginUser.LoginOrgId;
                objWorkflow.WorkflowDMSProjectId = Convert.ToInt32(ddlDmsProject.SelectedValue);
                if (Session["ProcessId"] != null)
                objWorkflow.WorkFlowProcessId = Convert.ToInt32(Session["ProcessId"]);
                objWorkflow.WorkFlowName = txtWorkflowName.Text.Trim();
                objWorkflow.WorkFlowDescription = txtWorkflowDescription.Text.Trim();
                objWorkflow.WorkflowIsActive = ChkActiveWorkflow.Checked;
                objWorkflow.WorkflowPriority = lstSortOrder.SelectedItem.Text;
               
                if (hdnActionWorkflow.Value == "EditWorkflow")
                {
                   
                  int workflowId =   Convert.ToInt32(hiddenWorkflowId.Value);
                  Session["WorkflowId"] = workflowId;
                    objWorkflow.WorkflowId = workflowId == 0 ? 0 : workflowId;
                    objWorkflow.WorkflowIsDeleted = ChkActiveWorkflow.Checked ? false : true;
                    action = hdnActionWorkflow.Value;
                }

                // Save update datbase and get the result
                objResult = objWorkflow.ManageWorkflows(objWorkflow, action);
                handleDBResult(objResult, lblMessageWorkflow);
               

                //only if success
                if (objResult.ErrorState == 0)
                {

                    if (hdnActionWorkflow.Value == "AddWorkflow")
                    {
                        hiddenWorkflowSaveStatus.Value = "Yes";
                        Session["WorkflowId"] = objResult.dsResult.Tables[0].Rows[0]["workflowID"].ToString();
                    }
                    //WorkflowOwners.Enabled = false;
                    objStudio.LoginToken = loginUser.LoginToken;
                    objStudio.LoginOrgId = loginUser.LoginOrgId;
                    objStudio.ProcessId = Convert.ToInt32(Session["ProcessId"]);
                    objResult = objStudio.ManageStudio(objStudio, "GetWorkflowByProcessId");
                    //clearControls();

                 
                    if (objResult.dsResult.Tables.Count > 0 && objResult.dsResult.Tables[0].Rows.Count > 0)
                    {
                      string values =  ReturnDBValues(objResult.dsResult.Tables[0]);
                        ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "BuildWorkflows", "BuildWorkflows( '" + values + "');", true);
                    }
                 


                    //handling process owners
                    DisplaySelectedEntityDetails(GetSelectedEntityDetails());
                    BindUsers(GetAllUsers());


                    //make button color green on sucessful save
                    ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "GetBackWorkflow", "GetBackWorkflow();", true);
                  
                    hdnErrorStatus.Value = "";

                    hdnActionWorkflow.Value = "EditWorkflow";
                    Logger.Trace("Workflow saving finished (drag and drop)", Session["LoggedUserId"].ToString());
                }
               
            }
            catch (Exception ex)
            {
                Logger.Trace("Workflow saving exception (drag and drop)" + ex.Message.ToString(), Session["LoggedUserId"].ToString());
            }
            showworkflowpanel();

        }

        protected void btnWorkflowEdit_Click(object sender, EventArgs e)
        {
            try
            {
                Logger.Trace("Workflow edit started (drag and drop)" , Session["LoggedUserId"].ToString());
                                lblMessageWorkflow.Text = string.Empty;
                                ChkActiveWorkflow.Enabled = true;
                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objStudio.LoginToken = loginUser.LoginToken;
                objStudio.LoginOrgId = loginUser.LoginOrgId;
                objStudio.ProcessId = Convert.ToInt32(Session["ProcessId"]);
                Session["WorkflowId"] = objStudio.WorkflowId = Convert.ToInt32(hiddenWorkflowId.Value);
                objResult = objStudio.ManageStudio(objStudio, "GetWorkflowDetails");
                if (objResult.dsResult.Tables.Count > 0 && objResult.dsResult.Tables[0].Rows.Count > 0)
                {
                    DataRow dataRow = objResult.dsResult.Tables[0].Rows[0];
                    BindWorkflowProjectDropDown(); BindSortOrderToListBox();
                    if (dataRow["Priority"] != null && Convert.ToString(dataRow["Priority"]) != string.Empty)                        
                        lstSortOrder.Items.FindByText(Convert.ToString(dataRow["Priority"])).Selected = true;
                    if (dataRow["ProjectName"] != null && Convert.ToString(dataRow["ProjectName"]) != string.Empty)
                        ddlDmsProject.SelectedItem.Text = Convert.ToString(dataRow["ProjectName"]);
                    txtWorkflowName.Text = Convert.ToString(dataRow["WorkflowName"]).Trim();
                    txtWorkflowDescription.Text = Convert.ToString(dataRow["WorkflowDescription"]).Trim();
                    ChkActiveWorkflow.Checked = Convert.ToBoolean(dataRow["Active"]);

                    BindUsers(GetAllUsers());
                    Session["WorkflowConfirmed"] = Convert.ToBoolean(dataRow["Confirmed"]);
                }

                if (Session["WorkflowConfirmed"].Equals(true))
                {
                    DisableConfirmButtons();

                }
                else { EnableConfirmButtons(); }

                objResult = objStudio.ManageStudio(objStudio, "GetStagesByWorkflowId");

                if (objResult.dsResult.Tables.Count > 0 && objResult.dsResult.Tables[0].Rows.Count > 0)
                {
                    string values = ReturnDBValues(objResult.dsResult.Tables[0]);
                    ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "BuildStages", "BuildStages( '" + values + "');", true);
                }
                else
                {
                    var values = string.Join(",", string.Empty);
                    ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "BuildStages", "BuildStages( '" + values + "');", true);
                }
                Logger.Trace("Workflow edit finished (drag and drop)", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            { Logger.Trace("Workflow edit exception (drag and drop)" + ex.Message.ToString(), Session["LoggedUserId"].ToString()); }
            showworkflowpanel();
        }

        private void DisableConfirmButtons()
        {
            btnWorkflowSave.Enabled = false;
            btnStageSave.Enabled = false;
            btnStageOk.Enabled = false;
            btnStageDelete.Enabled = false;
            btnStatusSave.Enabled = false;
            btnStatusOk.Enabled = false;
            btnStatusDelete.Enabled = false;
            btnNotificationSave.Enabled = false;
        }

        private void EnableConfirmButtons()
        {
            btnWorkflowSave.Enabled = true;
            btnStageSave.Enabled = true;
            btnStageOk.Enabled = true;
            btnStageDelete.Enabled = true;
            btnStatusSave.Enabled = true;
            btnStatusOk.Enabled = true;
            btnStatusDelete.Enabled = true;
            btnNotificationSave.Enabled = true;
        }
        private void showworkflowpanel()
        {
            pnlProcess.Style.Add("display", "none");
            pnlStages.Style.Add("display", "none");
            pnlStagesAdd.Style.Add("display", "none");
            pnlStatus.Style.Add("display", "none");
            pnlStatusAdd.Style.Add("display", "none");
            pnlWorkflow.Style.Add("display", "block");
            pnlNotifications.Style.Add("display", "none");
        }

        //Bind sort orderList
        protected void BindSortOrderToListBox()
        {
            try
            {
                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objWorkflow.LoginToken = loginUser.LoginToken;
                objWorkflow.LoginOrgId = loginUser.LoginOrgId;
                if (Session["ProcessId"] != null)
                    objWorkflow.WorkFlowProcessId = Convert.ToInt32(Session["ProcessId"].ToString());
                objResult = objWorkflow.ManageWorkflows(objWorkflow, "BindSortOrderToListBox");
                if (objResult.dsResult.Tables[0].Rows.Count > 0)
                {
                    lstSortOrder.DataSource = objResult.dsResult;
                    lstSortOrder.DataTextField = "SortOrder";
                    lstSortOrder.DataValueField = "SortOrderId";
                    lstSortOrder.DataBind();
                    if(hdnActionWorkflow.Value == "AddWorkflow")
                    lstSortOrder.SelectedIndex = 1;
                }

            }
            catch (Exception ex)
            { }

        }

        #endregion

        #region Status Section

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

        protected void gridStatusMaster_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {

            gridStatusMaster.PageIndex = e.NewPageIndex;
            gridStatusMaster.DataSource = (DataTable)Session["GridstageMasterData"];
            gridStatusMaster.DataBind();

            hdnErrorStatus.Value = "PAGE_CHANGE";

        }

        protected void btnStatusEdit_Click(object sender, EventArgs e)
        {
            try
            {
                lblStatusMessage.Text = string.Empty;
                ChkStatusActive.Enabled = true;
                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objStudio.LoginToken = loginUser.LoginToken;
                objStudio.LoginOrgId = loginUser.LoginOrgId;
                objStudio.ProcessId = Convert.ToInt32(Session["ProcessId"]);
                objStudio.WorkflowId = Convert.ToInt32(Session["WorkflowId"]);
                objStudio.StageId = Convert.ToInt32(hiddenStageId.Value);
                objStudio.StatusId = Convert.ToInt32(hiddenStatusId.Value);
                objResult = objStudio.ManageStudio(objStudio, "GetStatusDetails");
                if (objResult.dsResult.Tables.Count > 0 && objResult.dsResult.Tables[0].Rows.Count > 0)
                {
                    DataRow dataRow = objResult.dsResult.Tables[0].Rows[0];
                    txtStatusName.Text = Convert.ToString(dataRow["Status Name"]).Trim();
                    txtStatusDescription.Text = Convert.ToString(dataRow["Status Description"]).Trim();
                    ChkStatusActive.Checked = Convert.ToBoolean(dataRow["Active"]);
                    chkSendNotification.Checked = Convert.ToBoolean(dataRow["Send Notification"]);
                    chkConfirmOnSubmit.Checked = Convert.ToBoolean(dataRow["Need Confirmation on Submit"]);
                    txtConfirmMsgOnSubmit.Text = Convert.ToString(dataRow["Confirmation Message"]).Trim();
                    GetAllActiveStages();
                    if (dataRow["Move To Stage"] != null && Convert.ToString(dataRow["Move To Stage"]) != string.Empty)
                        ddl_Stages.Items.FindByText(Convert.ToString(dataRow["Move To Stage"])).Selected = true;
                       

                }
            }
            catch (Exception ex)
            { }
            ShowStatusPanel();
        }

        protected void btnStatusOk_Click(object sender, EventArgs e)
        {
            try
            {
                CheckAuthentication();
                string CommaSeparatedStatusNames = string.Empty;
                string CommaSeparatedStatusIds = string.Empty;

                foreach (GridViewRow row in gridStatusMasterAdd.Rows)
                {
                    CheckBox chk = row.FindControl("ChkStageMaster") as CheckBox;

                    if (chk.Checked)
                    {
                        CommaSeparatedStatusIds += (row.Cells[1].Text + ",").Trim(); //StatusId
                        CommaSeparatedStatusNames += (row.Cells[2].Text + ",").Trim(); //StatusName  
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

                if (objResult.ErrorState == 0)
                {
                    hdnActionStatus.Value = "EditStatus";
                    objStudio = new ManageWorkflowStudio();
                    objStudio.LoginToken = loginUser.LoginToken;
                    objStudio.LoginOrgId = loginUser.LoginOrgId;
                    objStudio.ProcessId = Convert.ToInt32(Session["ProcessId"]);
                    objStudio.WorkflowId = Convert.ToInt32(Session["WorkflowId"]);
                    objStudio.StageId = Convert.ToInt32(hiddenStageId.Value);
                    objResult = objStudio.ManageStudio(objStudio, "GetStageInformation");
                }
                if (objResult.dsResult.Tables.Count > 0 && objResult.dsResult.Tables[0].Rows.Count > 0)
                {
                    BindStatusGridview(GetAllstatus());
                    string values = ReturnDBValues(objResult.dsResult.Tables[0]);
                    values = Regex.Replace(values, "<.*?>", String.Empty);
                    UncheckAllCheckboxes(gridStatusMasterAdd, "ChkStageMaster");
                    ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "BuildAddStatus", "BuildAddStatus( '" + values + "');", true);
                }
              
              
               
               
            }
            catch (Exception ex)
            { }


        }

        protected void BindStatusGridview(DBResult objResult)
        {
            if (objResult.dsResult != null)
            {
                if (objResult.dsResult.Tables.Count > 1)
                {
                    // Second table contains available stages from master stages
                    Session["GridstageMasterData"] = objResult.dsResult.Tables[1];
                    gridStatusMasterAdd.DataSource = objResult.dsResult.Tables[1];
                    gridStatusMasterAdd.DataBind();

                    //show status add panel
                    pnlProcess.Style.Add("display", "none");
                    pnlStages.Style.Add("display", "none");
                    pnlStagesAdd.Style.Add("display", "none");
                    pnlWorkflow.Style.Add("display", "none");
                    pnlNotifications.Style.Add("display", "none");
                    pnlStatus.Style.Add("display", "none");
                    pnlStatusAdd.Style.Add("display", "block");

                }
            }
        }

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

        protected void gridStatusMasterAdd_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            CheckAuthentication();
            gridStatusMasterAdd.PageIndex = e.NewPageIndex;
            gridStatusMasterAdd.DataSource = (DataTable)Session["GridstageMasterData"];
            gridStatusMasterAdd.DataBind();

            hdnErrorStatus.Value = "PAGE_CHANGE";

        }

        private void ShowStatusPanel()
        {
            pnlProcess.Style.Add("display", "none");
            pnlStages.Style.Add("display", "none");
            pnlStagesAdd.Style.Add("display", "none");
            pnlWorkflow.Style.Add("display", "none");
            pnlNotifications.Style.Add("display", "none");
            pnlStatusAdd.Style.Add("display", "none");
            pnlStatus.Style.Add("display", "block");
        }

        protected void btnStatusCancel_Click(object sender, EventArgs e)
        {
            ClosePanels();
        }

        protected void btnStatusAddCancel_Click(object sender, EventArgs e)
        {
            ClosePanels();
        }
       

        protected void btnStatusAdd_Click(object sender, EventArgs e)
        {
            BindStatusGridview(GetAllstatus());
        }

        private void GetAllActiveStages()
        {
            try
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
            catch (Exception ex)
            { }

        }

        protected void btnStatusSave_Click(object sender, EventArgs e)
        {
            WorkflowStageStatus objStatus = new WorkflowStageStatus();
            string action = "AddStage";
            lblStatusMessage.Text = null;
            
            DBResult objResult = new DBResult();
            try
            {
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objStatus.LoginToken = loginUser.LoginToken;
                objStatus.LoginOrgId = loginUser.LoginOrgId;

                objStatus.WorkflowStageStatusesName = txtStatusName.Text.Trim();
                objStatus.WorkflowStageStatusesDescription = txtStatusDescription.Text.Trim();
                objStatus.WorkflowStageStatusesIsActive = ChkStatusActive.Checked;

                objStatus.WorkflowStageStatusCurrentstatusId = Convert.ToInt32(hiddenStatusId.Value);
                objStatus.WorkflowStageStatusesConfirmMsgOnSubmit = txtConfirmMsgOnSubmit.Text.Trim();
                objStatus.WorkflowStageStatusesConfirmOnSubmit = chkConfirmOnSubmit.Checked;
                objStatus.WorkflowStageStatusesSendNotification = chkSendNotification.Checked;
                objStatus.WorkflowStageStatusesMoveToStageId = Convert.ToInt32(ddl_Stages.SelectedValue);
                if (hdnActionStatus.Value == "EditStatus")
                {

                    objStatus.WorkflowStageStatusesMasterId = Convert.ToInt32(hiddenStatusId.Value);
                    objStatus.WorkflowStageStatusesStageId = Convert.ToInt32(Session["StageId"]);
                    objStatus.WorkflowStageStatusesWorkFlowId = Convert.ToInt32(Session["WorkflowId"]);
                    objStatus.WorkflowStageStatusesProcessId = Convert.ToInt32(Session["ProcessId"]);
                    objStatus.WorkflowStageStatusesIsActive = ChkStatusActive.Checked ? true : false;
                    action = "EditStageStatuses";
                }


                objResult = objStatus.ManageWorkflowStageStatuses(objStatus, action);
                handleDBResult(objResult, lblStatusMessage);
                
                //only if success
                if (objResult.ErrorState == 0)
                {
                    hdnActionStatus.Value = "EditStatus";
                    objStudio.LoginToken = loginUser.LoginToken;
                    objStudio.LoginOrgId = loginUser.LoginOrgId;
                    objStudio.ProcessId = Convert.ToInt32(Session["ProcessId"]);
                    objStudio.WorkflowId = Convert.ToInt32(Session["WorkflowId"]);
                    objStudio.StageId = Convert.ToInt32(hiddenStageId.Value);
                    objResult = objStudio.ManageStudio(objStudio, "GetStageInformation");
              
                if (objResult.dsResult.Tables.Count > 0 && objResult.dsResult.Tables[0].Rows.Count > 0)
                {
                    BindStatusGridview(GetAllstatus());                  
                    string values = ReturnDBValues(objResult.dsResult.Tables[0]);
                    values = Regex.Replace(values, "<.*?>", String.Empty);                
                    ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "BuildAddStatus", "BuildAddStatus( '" + values + "');", true);
                }
                else
                {
                    string values = string.Empty;
                    ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "BuildAddStatus", "BuildAddStatus( '" + values + "');", true);
                }


                }
               
               
            }
            catch (Exception ex)
            {
                
            }
            ShowStatusPanel();
        }

        private void clearStatusControls()
        {
            txtStatusName.Text = string.Empty;
            txtStatusDescription.Text = string.Empty;
            ChkStatusActive.Checked = false;
            //lblMessage.Text = string.Empty;
        }


        protected void btnWorkflowCancel_Click(object sender, EventArgs e)
        {
          

            pnlWorkflow.Style.Add("display", "none");
            ClosePanels();
        }

        private void ClosePanels()
        {

            pnlNotifications.Style.Add("display", "none");

            pnlProcess.Style.Add("display", "none");

            pnlStages.Style.Add("display", "none");

            pnlStagesAdd.Style.Add("display", "none");

            pnlStatus.Style.Add("display", "none");

            pnlStatusAdd.Style.Add("display", "none");

            pnlWorkflow.Style.Add("display", "none");
        }

        protected void btnStatusDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DBResult objResult = new DBResult();
                lblStatusMessage.Text = string.Empty;
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objStatus.LoginToken = loginUser.LoginToken;
                objStatus.LoginOrgId = loginUser.LoginOrgId;

                int ProcessId = Convert.ToInt32(Session["ProcessId"]);
                int WorkflowId = Convert.ToInt32(Session["WorkflowId"]);
                int StageId = Convert.ToInt32(Session["StageId"]);
                int StatusId = Convert.ToInt32(hiddenStatusId.Value);

                objStatus.WorkflowStageStatusesWorkFlowId = WorkflowId;
                objStatus.WorkflowStageStatusesStageId = StageId;
                objStatus.WorkflowStageStatusesProcessId = ProcessId;
                objStatus.WorkflowStageStatusCurrentstatusId = StatusId;
                objResult = objStatus.ManageWorkflowStageStatuses(objStatus, "DeleteStageStatuses");
                if (objResult.ErrorState != 0)
                {
                    lblStatusMessage.Text = objResult.Message.ToString();
                    lblStatusMessage.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblStatusMessage.Text = objResult.Message.ToString();
                    BindStatusGridview(GetAllstatus());
                    ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "Removestatus();", "Removestatus('" + StatusId + "');", true);
                }
            }
            catch (Exception ex)
            { }
        }
        #endregion

        #region StagesSection

        public void LoadDataEntryTypes()
        {
            try
            {
                DBResult objResult = new DBResult();
                objStage = GetValuesFromSession();//Get the value from session
                objResult = objStage.ManageWorkflowStages(objStage, "GetAllDataEntryTypes");
                Logger.Trace("DataEntryTypes count : " + objResult.dsResult.Tables[0].Rows.Count, Session["LoggedUserId"].ToString());
                ddlDataEntry.DataSource = objResult.dsResult;
                ddlDataEntry.DataTextField = "GEN_vDescription";
                ddlDataEntry.DataValueField = "GEN_iID";
                ddlDataEntry.DataBind();
                //ddlDataEntry.SelectedIndex = 2;
                //Session["dataEntryTypeName"] = ddlDataEntry.SelectedItem.Text;
            }
            catch (Exception ex)
            { }
        }

        public WorkflowStage GetValuesFromSession()
        {
            try
            {
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objStage.LoginToken = loginUser.LoginToken;
                objStage.LoginOrgId = loginUser.LoginOrgId;
                if (Session["ProcessId"] != null)
                    objStage.ProcessId = Convert.ToInt32(Session["ProcessId"]);
                if (Session["WorkflowId"] != null)
                    objStage.WorkflowId = Convert.ToInt32(Session["WorkflowId"]); ;
                if (hiddenStageId.Value != null && hiddenStageId.Value != string.Empty)
                {
                    objStage.StageId = Convert.ToInt32(hiddenStageId.Value);
                }
            }
            catch (Exception ex)
            { }
            return objStage;
        }

        public string UploadedFilePath
        {
            get
            {
                if (Session["UploadedFilePath"] != null)
                {
                    return Convert.ToString(Session["UploadedFilePath"]);
                }
                else
                    return string.Empty;
            }
            set { Session["UploadedFilePath"] = value; }
        }

        protected void btnStageSave_Click(object sender, EventArgs e)
        {
            objStage = GetValuesFromSession();

            Logger.Trace("User clicked save to save stage details.", Session["LoggedUserId"].ToString());

            DBResult objResult = new DBResult();
            int stageFieldsCount = 0;
            int stageDataEntryTypeId = 0;
            string stageDataEntryType = string.Empty;
            string selectedDataEntryType = string.Empty;

            try
            {
                //objStage = GetValuesFromSession();

                //selected data entry type id
                objStage.DataEntryId = (ddlDataEntry.SelectedIndex > -1) ? Convert.ToInt32(ddlDataEntry.SelectedValue) : 0;
                hdnDataEntryName.Value = selectedDataEntryType = (ddlDataEntry.SelectedIndex > -1) ? ddlDataEntry.SelectedItem.Text : string.Empty;
                Logger.Trace("(Drag and Drop) Selected data entry type: " + selectedDataEntryType, Session["LoggedUserId"].ToString());
                // current data entry type id; taken from grid
                stageDataEntryTypeId = (hdnDataentryId.Value != null && Convert.ToString(hdnDataentryId.Value).Length > 0) ? Convert.ToInt32(hdnDataentryId.Value) : 0;
                Logger.Trace("(Drag and Drop) Selected data entry type Id: " + stageDataEntryTypeId, Session["LoggedUserId"].ToString());

                // Validate file/template is uploaded if data entry type changed
                if (stageDataEntryTypeId != objStage.DataEntryId)
                {

                    if (selectedDataEntryType != "Normal" && UploadedFilePath.Length == 0)
                    {
                        lblMessageStage.Text = "Please select a template";
                        //ScriptManager.RegisterStartupScript(this, typeof(Page), "ReloadDialogbox", "ReloadDialogbox();", true);
                        //return;
                    }

                    //DMS5-3363BS   
                    // Get actual data entry type; if it's "guided" or "Form" and user changing to any other type
                    // show warning message to user before saving i.e., "Data location / co-ordinates values will be lost".
                    stageDataEntryType = (hdnDataEntryName.Value != null && Convert.ToString(hdnDataEntryName.Value).Length > 0) ? Convert.ToString(hdnDataEntryName.Value) : string.Empty;
                    Logger.Trace("(Drag and Drop) Actual data entry type before editing the data entry type: " + stageDataEntryType, Session["LoggedUserId"].ToString());
                    if (stageDataEntryType != "Normal")
                    {
                        //get the stagefields count mapped to particular stage which has position(co-odinates).                            
                        objResult = objStage.ManageWorkflowStages(objStage, "CheckMappingOfStage");
                        handleDBResult(objResult, lblMessageStage);

                        //Warning message when the data entry type is changes from "guided" or "form" to any other type
                        if (objResult.dsResult != null && objResult.dsResult.Tables.Count > 0 && objResult.dsResult.Tables[0].Rows.Count > 0)
                        {
                            stageFieldsCount = Convert.ToInt32(objResult.dsResult.Tables[0].Rows[0]["TotalStageFields"]);
                            Logger.Trace("(Drag and Drop) Total fields with data position captured in the stage: " + stageFieldsCount, Session["LoggedUserId"].ToString());

                            //if fields exits, And current & selected data enrtry type is different show warning message
                            if (stageFieldsCount > 0)
                            {
                                Logger.Trace("(Drag and Drop) Warning message dispalyed due to mapping in stage and stagefields", Session["LoggedUserId"].ToString());
                                //show warning message
                                ScriptManager.RegisterStartupScript(this, typeof(Page), "ConfirmBox", "ConfirmBox();", true);
                            }
                            else
                            {
                                //normal saving
                                ScriptManager.RegisterStartupScript(this, typeof(Page), "SaveStage", "SaveStage();", true);
                            }
                        }
                    }
                    else
                    {
                        //normal saving
                        ScriptManager.RegisterStartupScript(this, typeof(Page), "SaveStage", "SaveStage();", true);
                    }

                }
                else
                {
                    //normal saving
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "SaveStage", "SaveStage();", true);
                }

            }
            catch (Exception ex)
            {
                Logger.Trace("Exception (Drag and Drop) " + ex.Message, Session["LoggedUserId"].ToString());
                lblMessageStage.Text = ex.Message;
                ScriptManager.RegisterStartupScript(this, typeof(Page), "ReloadDialogbox", "ReloadDialogbox();", true);
            }
        }

        protected void btnStageEdit_Click(object sender, EventArgs e)
        {
            try
            {
                Logger.Trace("Stage edit started  (Drag and Drop) " , Session["LoggedUserId"].ToString());
                lblMessageStage.Text = string.Empty;
                StageFileControlPlaceHolder.Visible = false;
                BackgroundImagePlaceHolder.Visible = false;
                TempaltePathPlaceHolder.Visible = false;
                chkStageActive.Enabled = true;
                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objStudio.LoginToken = loginUser.LoginToken;
                objStudio.LoginOrgId = loginUser.LoginOrgId;
                objStudio.ProcessId = Convert.ToInt32(Session["ProcessId"]);
                objStudio.WorkflowId = Convert.ToInt32(Session["WorkflowId"]);
                Session["StageId"] = objStudio.StageId = Convert.ToInt32(hiddenStageId.Value);
                objResult = objStudio.ManageStudio(objStudio, "GetStagesDetails");
                if (objResult.dsResult.Tables.Count > 0 && objResult.dsResult.Tables[0].Rows.Count > 0)
                {
                    DataRow dataRow = objResult.dsResult.Tables[0].Rows[0];


                    Session["StageName"] = txtStageName.Text = Convert.ToString(dataRow["StageName"]).Trim();
                    txtStageDescription.Text = Convert.ToString(dataRow["StageDescription"]).Trim();
                    txtTATDuration.Text = Convert.ToString(dataRow["TATDuration"]).Trim();
                    LoadDataEntryTypes();
                  ddlDataEntry.Items.FindByValue(Convert.ToString(dataRow["DataEntryTypeId"])).Selected = true;
                    Session["DataEntryTypeId"] = hdnDataentryId.Value = Convert.ToString(dataRow["DataEntryTypeId"]);
                    Session["dataEntryTypeName"] = hdnDataEntryName.Value = Convert.ToString(dataRow["DataEntry"]);
                    if (Convert.ToString(Session["dataEntryTypeName"]) == "Guided")
                    {//Guided Data entry
                       
                            StageFileControlPlaceHolder.Visible = true;
                            BackgroundImagePlaceHolder.Visible = false;
                            TempaltePathPlaceHolder.Visible = true;
                    }
                    else if (Convert.ToString(Session["dataEntryTypeName"]) == "Form")
                    {
                        StageFileControlPlaceHolder.Visible = true;
                        BackgroundImagePlaceHolder.Visible = true;
                        TempaltePathPlaceHolder.Visible = true;
                    }
                    lblStageTemplatePath.Text = Convert.ToString(dataRow["Path"]).Trim();
                    chkBackgroundImage.Checked = Convert.ToBoolean(dataRow["ShowBackgroundImage"]);
                    chkStageActive.Checked = Convert.ToBoolean(dataRow["Active"]);

                    BindUsers(GetAllUsers());

                }

                objResult = new DBResult();
                objResult = objStudio.ManageStudio(objStudio, "GetStageInformation");




                //Get Stagefields
                if (objResult.dsResult.Tables.Count > 0 && objResult.dsResult.Tables[1].Rows.Count > 0)
                {
                    string values = ReturnDBValues(objResult.dsResult.Tables[1]);
                    hdnFieldsvalues.Value = values;
                   
                }

                //Get Stage Statuses
                if (objResult.dsResult.Tables.Count > 0 && objResult.dsResult.Tables[0].Rows.Count > 0)
                {
                    string values = ReturnDBValues(objResult.dsResult.Tables[0]);                  
                    hdnstatusvalues.Value = values;
                  
                }

                //Get Notification 
                if (objResult.dsResult.Tables.Count > 0 && objResult.dsResult.Tables[2].Rows.Count > 0)
                {
                    string values = ReturnDBValues(objResult.dsResult.Tables[2]);
                    hdnnotificationvalues.Value = values;
                   
                }
              
                showstagepanel();
                ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "LoadALLStageDetails", "LoadALLStageDetails();", true);
                Logger.Trace("Stage edit finished  (Drag and Drop) ", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex) { Logger.Trace("Stage edit finished  (Drag and Drop) " + ex.Message.ToString(), Session["LoggedUserId"].ToString()); }
        }

        private void showstagepanel()
        {
            pnlProcess.Style.Add("display", "none");
            pnlWorkflow.Style.Add("display", "none");
            pnlStages.Style.Add("display", "block");
            pnlStagesAdd.Style.Add("display", "none");
            pnlStatus.Style.Add("display", "none");           
            pnlStatusAdd.Style.Add("display", "none");
            pnlNotifications.Style.Add("display", "none");
        }

        protected void btnStageSubmit_Click(object sender, EventArgs e)
        {
            // CheckAuthentication();

            DBResult objResult = new DBResult();
            string action = hdnActionStage.Value;
            int dataEntryId = 0;
            string sourceFilePath = string.Empty;
            int processId = 0;
            int workflowId = 0;
            string stageId = string.Empty;
            string stageName = string.Empty;
            string dataEntryTypeId = string.Empty;
            string dataEntryType = string.Empty;

            string stageTemplatePath = hdnStageTemplatePath.Value.Length > 0 ? hdnStageTemplatePath.Value : string.Empty;
            //creating temporary folder and file path with proper folder structure like processid/workflowid/stageid
            string templateTempFolderPath = GetTemplateTemporaryFolderPath();
            //creating original folder and file path with proper folder structure like processid/workflowid/stageid
            string templateOriginalFolderPath = GetTemplateOriginalFolderPath();

            try
            {
                objStage.DisplayName = txtStageName.Text.Trim();
                objStage.Description = txtStageDescription.Text.Trim();
                objStage.Active = chkStageActive.Checked;
                objStage.TatDuration = Convert.ToInt32(txtTATDuration.Text.Trim());
                
                objStage = GetValuesFromSession();
                objStage.DataEntryId = Convert.ToInt32(ddlDataEntry.SelectedValue);
                Session["dataEntryTypeName"] = ddlDataEntry.SelectedItem.Text;
                objStage.bShowBackgroundImage = chkBackgroundImage.Checked;
                //DMS5-3363BS
                objStage.TemplatePath = UploadedFilePath.Length > 0 ? UploadedFilePath.Replace(templateTempFolderPath, templateOriginalFolderPath) : stageTemplatePath; //getting original template path
                Logger.Trace("Template path: " + objStage.TemplatePath, Session["LoggedUserId"].ToString());
                //DMS5-3363BE

                Logger.Trace("Data Entry Id : " + objStage.DataEntryId, Session["LoggedUserId"].ToString());
                if (hdnActionStage.Value == "EditStages")
                {
                    objStage.Deleted = chkStageActive.Checked;
                    action = "EditStage";
                    dataEntryId = Convert.ToInt32(hdnDataentryId.Value);
                }
                Logger.Trace("Action for saving the stage : " + action, Session["LoggedUserId"].ToString());

                //saving stage details
                objResult = objStage.ManageWorkflowStages(objStage, action);
                handleDBResult(objResult, lblMessageStage);//to display message based on result set.
                
                //only if success
                if (objResult.ErrorState == 0)
                {
                   

                    UserBase loginUser = (UserBase)Session["LoggedUser"];
                    objStudio.LoginToken = loginUser.LoginToken;
                    objStudio.LoginOrgId = loginUser.LoginOrgId;
                    objStudio.ProcessId = Convert.ToInt32(Session["ProcessId"]);
                    objStudio.WorkflowId = Convert.ToInt32(Session["WorkflowId"]);
                    objResult = objStudio.ManageStudio(objStudio, "GetStagesByWorkflowId");
                    //clearControls();

                  
                    if (objResult.dsResult.Tables.Count > 0 && objResult.dsResult.Tables[0].Rows.Count > 0)
                    {
                       string values= ReturnDBValues(objResult.dsResult.Tables[0]);
                        ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "BuildStages", "BuildStages( '" + values + "');", true);
                    }
                    


                    if (UploadedFilePath.Length > 0)
                    {
                        //Clear old files
                        Logger.Trace("Deleting original file folder for clearing the olde files in that location : " + templateOriginalFolderPath, Session["LoggedUserId"].ToString());
                        ManageFiles.DeleteFiles(templateOriginalFolderPath);

                        //Copy files from temporary location to original location
                        Logger.Trace("copying files from temporary location to original ", Session["LoggedUserId"].ToString());
                        ManageFiles.CopyFiles(templateTempFolderPath, templateOriginalFolderPath);

                        //To delete the temporary folder and files
                        Logger.Trace("Deleting temporay folder: " + templateTempFolderPath, Session["LoggedUserId"].ToString());
                        new ManageDirectory().DeleteDirectory(templateTempFolderPath);

                        UploadedFilePath = string.Empty;


                    }



                    //checking whether the dataentry type before editing and after editing is same or not
                    if (dataEntryId != objStage.DataEntryId)
                    {
                        dataEntryType = ddlDataEntry.SelectedItem.Text;
                        if (dataEntryType == "Guided" || dataEntryType == "Form")
                        {
                            #region Redirect to fields page to capture fields again
                             processId = Convert.ToInt32(Session["ProcessId"]);
                             workflowId = Convert.ToInt32(Session["WorkflowId"]);
                             stageId = hiddenStageId.Value;
                            stageName = hdnStageName.Value != null ? Convert.ToString(hdnStageName.Value) : string.Empty;
                            dataEntryTypeId = Convert.ToString(objStage.DataEntryId);
                            Session["DataEntryId"] = dataEntryTypeId;
                            ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "ShowFieldProperties", "ShowFieldProperties();", true);
                            #endregion
                        }
                        else
                        {
                            ////Deleting the template which was saved initially in temporary folder
                            new ManageDirectory().DeleteDirectory(templateOriginalFolderPath);
                        }
                    }

                   
                  
                }
                else
                {
                    //delete the saved template from original location id database save fails.
                    new ManageDirectory().DeleteDirectory(templateOriginalFolderPath);

                }
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
                lblMessageStage.ForeColor = System.Drawing.Color.Red;
                lblMessageStage.Text = ex.Message;
            }

            showstagepanel();
        }

        private string GetTemplateTemporaryFolderPath()
        {

            string path = string.Empty;
            string folderPath = ConfigurationManager.AppSettings["TempWorkFolder"];
            //Get data from ViewState            
            objStage = GetValuesFromSession();
            path = folderPath + "/" + objStage.ProcessId + "/" + objStage.WorkflowId + "/" + objStage.StageId;
            return path;
        }

        private string GetTemplateOriginalFolderPath()
        {
            string path = string.Empty;
            string folderPath = ConfigurationManager.AppSettings["StageTemplatePath"];
            //Get data from ViewState            
            objStage = GetValuesFromSession();
            path = folderPath + "/" + objStage.ProcessId + "/" + objStage.WorkflowId + "/" + objStage.StageId;

            return path;
        }

        protected void StageTemplateUpload_UploadedComplete(object sender, AjaxControlToolkit.AsyncFileUploadEventArgs e)
        {
            Logger.Trace("User browsed template for stage.", Session["LoggedUserId"].ToString());

            //Variable declaration
            string filePath = string.Empty;
            string stageTemplateFolderPath = string.Empty;

            string fileExtension = string.Empty;
            string newFileName = string.Empty;

            try
            {
                UploadedFilePath = string.Empty;

                //Get data from ViewState            
                objStage = GetValuesFromSession();

                //Saving template to temporary folder, on save it will be moved to original folder
                if (StageTemplateUpload.HasFile)
                {
                    Logger.Trace("File upload control contains file. File path: " + StageTemplateUpload.FileName, Session["LoggedUserId"].ToString());

                    fileExtension = afterdot(StageTemplateUpload.FileName);

                    stageTemplateFolderPath = GetTemplateTemporaryFolderPath();
                    //Temporary folder path to store uploaded file/template

                    Logger.Trace("Deleting temporary folder path to clear old files if exists. Path: " + stageTemplateFolderPath, Session["LoggedUserId"].ToString());
                    new ManageDirectory().DeleteDirectory(stageTemplateFolderPath);

                    Logger.Trace("Creating temporary folder path to store uploaded file/template. Path: " + stageTemplateFolderPath, Session["LoggedUserId"].ToString());
                    new ManageDirectory().CreateDirectory(stageTemplateFolderPath);
                    if (fileExtension.ToLower() == ".pdf" || fileExtension.ToLower() == ".jpg" || fileExtension.ToLower() == ".jpeg" || fileExtension.ToLower() == ".png")
                    {
                        if (fileExtension.ToLower() == ".pdf")
                        {
                            // Creating full file path to store file/template; prefixed 'temp_' to avoid filename duplicate with splitted pages
                            Logger.Trace("Multiple page file, Renaming as temp_" + objStage.StageId + "." + fileExtension, Session["LoggedUserId"].ToString());
                            //DMS5-3845 Uploading template in stage edit change in configuration after code review
                            //uploading original file name as temp_stageId
                            newFileName = "temp_" + objStage.StageId + fileExtension;
                        }
                        else
                        {
                            Logger.Trace("Single page file, Renaming as 1." + fileExtension, Session["LoggedUserId"].ToString());
                            //if not a pdf upload original file as 1.file extension
                            newFileName = "1" + fileExtension;
                        }

                        filePath = stageTemplateFolderPath + @"/" + newFileName;

                        Logger.Trace("deleting the old pdf files in folder:" + Path.GetDirectoryName(filePath), Session["LoggedUserId"].ToString());
                        ManageFiles.DeleteFiles(Path.GetDirectoryName(filePath), "*.pdf");

                        Logger.Trace("deleting the old jpg files in folder:" + Path.GetDirectoryName(filePath), Session["LoggedUserId"].ToString());
                        ManageFiles.DeleteFiles(Path.GetDirectoryName(filePath), "*.jpg");

                        //saving the template file to temporary folder 
                        Logger.Trace("Saving uploaded file/template to temporary location: " + filePath, Session["LoggedUserId"].ToString());
                        StageTemplateUpload.SaveAs(filePath);

                        //If pdf, split file pages into jpeg; 
                        if (fileExtension.ToLower() == ".pdf")
                        {
                            if (SplitFilePages(filePath, true))
                            {
                                // Delete file after splitting pages into jpeg 
                                Logger.Trace("Deleting original file after completion of file splitting: " + filePath, Session["LoggedUserId"].ToString());
                                File.Delete(filePath);
                            }
                        }


                        //Store path into a session for final save (later on save).
                        UploadedFilePath = filePath;

                        Logger.Trace("User browsed file/template uploaded successfully.", Session["LoggedUserId"].ToString());
                    }
                    else
                    {
                        Logger.Trace("File is not supportted as the file extension is:" + fileExtension, Session["LoggedUserId"].ToString());
                        return;
                    }
                }
                else
                {
                    Logger.Trace("File upload control doesn't contain file.", Session["LoggedUserId"].ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
                //Show alert as label text cannot be updated in AsyncFileUpload control               
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('" + ex.Message + "');", true);

            }
        }
        // split file pages into jpeg
        private bool SplitFilePages(string filePath, bool deleteSource)
        {
            bool success = false;
            string splittedFileFolderPath = Path.GetDirectoryName(filePath);

            if (File.Exists(filePath))
            {
                //Calling method to split files (in pdf format)
                Logger.Trace("Calling method to split files and return splitted pages count.", Session["LoggedUserId"].ToString());
                hdnPagesCount.Value = Convert.ToString(new Image2Pdf().ExtractPages(filePath, splittedFileFolderPath));
                Logger.Trace("Splitting of file is successfully completed. Pages count: " + hdnPagesCount.Value, Session["LoggedUserId"].ToString());

                //Convert splitted files into JPEG
                for (int pageNo = 1; pageNo <= Convert.ToInt32(hdnPagesCount.Value); pageNo++)
                {
                    string source = splittedFileFolderPath + @"\" + pageNo + ".pdf";
                    string destination = splittedFileFolderPath + @"\" + pageNo + ".jpg";
                    Logger.Trace("Converting splitted files into jpg. Source: " + source + " Destination: " + destination, Session["LoggedUserId"].ToString());
                    new ConvertPdf2Image().ConvertPdf2Jpeg(source, destination);
                }

                // Delete 
                if (deleteSource)
                {
                    Logger.Trace("Deleting splitted files(pdf) after completion of converting to jpg. Directory: " + splittedFileFolderPath, Session["LoggedUserId"].ToString());
                    ManageFiles.DeleteFiles(splittedFileFolderPath, "*.pdf");
                }

                success = true;
            }
            else//when file error occurs
            {
                Logger.Trace("File does not exist to split. File path: " + filePath, Session["LoggedUserId"].ToString());
            }
            return success;
        }

        protected void btnStageAdd_Click(object sender, EventArgs e)
        {
            BindStagesGridview(GetAllStages());
        }

        private DBResult GetAllStages()
        {
            int ProcessId = Convert.ToInt32(Session["ProcessId"]);
            int WorkflowId = Convert.ToInt32(Session["WorkflowId"]);
            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            objStage.LoginToken = loginUser.LoginToken;
            objStage.LoginOrgId = loginUser.LoginOrgId;
            objStage.ProcessId = ProcessId;
            objStage.WorkflowId = WorkflowId;
            return objStage.ManageWorkflowStages(objStage, "GetAllStages");
        }

        protected void BindStagesGridview(DBResult objResult)
        {
            if (objResult.dsResult != null)
            {

                if (objResult.dsResult.Tables.Count > 1)
                {
                    // Second table contains available stages from master stages
                    gridStageMaster.DataSource = objResult.dsResult.Tables[1];
                    gridStageMaster.DataBind();
                    //show status add panel
                    pnlProcess.Style.Add("display", "none");
                    pnlStages.Style.Add("display", "none");
                    pnlWorkflow.Style.Add("display", "none");
                    pnlNotifications.Style.Add("display", "none");
                    pnlStatus.Style.Add("display", "none");
                    pnlStatusAdd.Style.Add("display", "none");
                    pnlStagesAdd.Style.Add("display", "block");
                }
            }
        }

        protected void gridStageMaster_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header || e.Row.RowType == DataControlRowType.DataRow)
            {
                //hide Unwanted columns
                e.Row.Cells[0].Style.Add("width", "40px");
                e.Row.Cells[2].Style.Add("width", "150px");
                e.Row.Cells[1].Style.Add("display", "none");
            }

        }

        protected void btnStageAddOk_Click(object sender, EventArgs e)
        {
            try
            {
                lblAddStages.Text = string.Empty;
                CheckAuthentication();
                string CommaSeparatedStageIds = string.Empty;
               
                foreach (GridViewRow row in gridStageMaster.Rows)
                {
                    CheckBox chk = row.FindControl("ChkStageMaster") as CheckBox;
                    if (chk.Checked)
                    {
                        CommaSeparatedStageIds += row.Cells[1].Text + ","; //StageId      
                       
                    }
                }


                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objStage.LoginToken = loginUser.LoginToken;
                objStage.LoginOrgId = loginUser.LoginOrgId;
                objStage.ProcessId = Convert.ToInt32(Session["ProcessId"]);
                objStage.WorkflowId = Convert.ToInt32(Session["WorkflowId"]);
                objStage.CommaSeparatedStageIds = CommaSeparatedStageIds;
               
                //insert into stage table
                objResult = objStage.ManageWorkflowStages(objStage, "AddStage");

                if (objResult.ErrorState != 0)
                {
                    lblAddStages.Text = objResult.Message.ToString();
                }
                else
                {
                    hiddenStageSaveStatus.Value = "Yes";
                    BindStagesGridview(GetAllStages());
                    objStudio = new ManageWorkflowStudio();
                    objStudio.LoginToken = loginUser.LoginToken;
                    objStudio.LoginOrgId = loginUser.LoginOrgId;
                    objStudio.ProcessId = Convert.ToInt32(Session["ProcessId"]);
                    objStudio.WorkflowId = Convert.ToInt32(Session["WorkflowId"]);
                    objResult = objStudio.ManageStudio(objStudio, "GetStagesByWorkflowId");
                }

                if (objResult.dsResult.Tables.Count > 0 && objResult.dsResult.Tables[0].Rows.Count > 0)
                {
                    int buttonId = 0;
                    string buttonName = string.Empty;
                    int Count = objResult.dsResult.Tables[0].Rows.Count;
                    //Generateing workflow buttons based on process
                    Dictionary<int, string> dictionary = new Dictionary<int, string>();
                    foreach (DataRow row in objResult.dsResult.Tables[0].Rows)
                    {
                         buttonId = Convert.ToInt32(row["Id"]);
                         buttonName = Convert.ToString(row["Name"]);
                        dictionary.Add(buttonId, buttonName);
                    }

                    var values = string.Join(",", dictionary.Select(m => m.Key + "-" + m.Value).ToArray());

                    UncheckAllCheckboxes(gridStageMaster,"ChkStageMaster");
                    ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "BuildStages", "BuildStages( '" + values + "');", true);
                }

              

                
            }
            catch (Exception ex)
            { }

        }

        //Uncheck checkbox on end of ok button clcik
        private void UncheckAllCheckboxes(GridView gridView,string checkBoxId)
        {
            try
            {
                foreach (GridViewRow row in gridView.Rows)
                {
                    CheckBox chk = row.FindControl(checkBoxId) as CheckBox;
                    if (chk.Checked)
                    {

                        chk.Checked = false;
                    }
                }
            }
            catch (Exception ex)
            { }

        }

        protected void btnStageCancel_Click(object sender, EventArgs e)
        {
            ClosePanels();
        }

        protected void btnStageAddCancel_Click(object sender, EventArgs e)
        {
            ClosePanels();
        }

        //Delete Stages
        protected void btnStageDelete_Click(object sender, EventArgs e)
        {
            try
            {
                lblMessageStage.Text = string.Empty;
                GetValuesFromSession();               
                DBResult objResult = new DBResult();
                string action = "DeleteStage";               
                objResult = objStage.ManageWorkflowStages(objStage, action);
                if (objResult.ErrorState != 0)
                {
                    lblMessageStage.Text = objResult.Message.ToString();
                    lblMessageStage.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    lblMessageStage.Text = objResult.Message.ToString();
                    BindStagesGridview(GetAllStages());
                    ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "Removestage();", "Removestage('"+objStage.StageId+"');", true);
                }
            }
            catch (Exception ex) { }
        
        }

        protected void ddlDataEntry_SelectedIndexChanged(object sender, EventArgs e)
        {
            StageFileControlPlaceHolder.Visible = false;
            BackgroundImagePlaceHolder.Visible = false;
            TempaltePathPlaceHolder.Visible = false;

            //Form Data entry
            if (ddlDataEntry.SelectedIndex.Equals(0))
            {

                StageFileControlPlaceHolder.Visible = true;
                BackgroundImagePlaceHolder.Visible = true;
                TempaltePathPlaceHolder.Visible = true;
            }
            //Guided Data entry
            if (ddlDataEntry.SelectedIndex.Equals(1))
            {
                StageFileControlPlaceHolder.Visible = true;
                BackgroundImagePlaceHolder.Visible = false;
                TempaltePathPlaceHolder.Visible = true;
            }
        }

        #endregion


        #region ProcessOwners

        private DBResult GetAllUsers()
        {
            try
            {
                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objUserMapping.LoginToken = loginUser.LoginToken;
                objUserMapping.LoginOrgId = loginUser.LoginOrgId;
                objUserMapping.WfUserMappingAction = "GetAllUsers";
                if (Session["ProcessId"] != null)
                    objUserMapping.WfUserMappingProcessId = Convert.ToInt32(Session["ProcessId"]);
                if (Session["WorkflowId"] != null)
                    objUserMapping.WfUserMappingWorkflowId = Convert.ToInt32(Session["WorkflowId"]);
                if (Session["StageId"] != null)
                    objUserMapping.WfUserMappingStageId = Convert.ToInt32(Session["StageId"]);
            }
            catch (Exception ex)
            { }

            return objUserMapping.GetAllWorkflowUserMapping(objUserMapping);
        }

        private DBResult GetSelectedEntityDetails()
        {
            try
            {
                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objUserMapping.LoginToken = loginUser.LoginToken;
                objUserMapping.LoginOrgId = loginUser.LoginOrgId;
                objUserMapping.WfUserMappingAction = "GetSelectedEntityDetails";
                if (Session["ProcessId"] != null)
                    objUserMapping.WfUserMappingProcessId = Convert.ToInt32(Session["ProcessId"]);
                if (Session["WorkflowId"] != null)
                    objUserMapping.WfUserMappingWorkflowId = Convert.ToInt32(Session["WorkflowId"]);
                objUserMapping.WfUserMappingStageId = 0;
            }
            catch (Exception ex)
            { }

            return objUserMapping.GetAllWorkflowUserMapping(objUserMapping);
        }

        protected void BindUsers(DBResult objResult)
        {
            try
            {
                //revert the if conditions at the end
                //fill process owner details if minimum 2 result sets are available
                if (objResult.dsResult != null && objResult.dsResult.Tables.Count > 1)
                {
                    lstAvailableUsers_ProcessOwner.DataTextField = "USERFULLNAME";
                    lstAvailableUsers_ProcessOwner.DataValueField = "USERS_iId";
                    lstAvailableUsers_ProcessOwner.DataSource = objResult.dsResult.Tables[1];
                    lstAvailableUsers_ProcessOwner.DataBind();

                    lstAssignedUsers_ProcessOwner.DataTextField = "USERFULLNAME";
                    lstAssignedUsers_ProcessOwner.DataValueField = "USERS_iId";
                    lstAssignedUsers_ProcessOwner.DataSource = objResult.dsResult.Tables[0];
                    lstAssignedUsers_ProcessOwner.DataBind();

                }
                if (objResult.dsResult != null && objResult.dsResult.Tables.Count > 3)
                {
                    lstAvailableUsers_WorkflowOwner.DataTextField = "USERFULLNAME";
                    lstAvailableUsers_WorkflowOwner.DataValueField = "USERS_iId";
                    lstAvailableUsers_WorkflowOwner.DataSource = objResult.dsResult.Tables[3];
                    lstAvailableUsers_WorkflowOwner.DataBind();

                    lstAssignedUsers_WorkflowOwner.DataTextField = "USERFULLNAME";
                    lstAssignedUsers_WorkflowOwner.DataValueField = "USERS_iId";
                    lstAssignedUsers_WorkflowOwner.DataSource = objResult.dsResult.Tables[2];
                    lstAssignedUsers_WorkflowOwner.DataBind();
                }

                if (objResult.dsResult != null && objResult.dsResult.Tables.Count > 5)
                {
                    lstAvailableUsers_StageOwner.DataTextField = "USERFULLNAME";
                    lstAvailableUsers_StageOwner.DataValueField = "USERS_iId";
                    lstAvailableUsers_StageOwner.DataSource = objResult.dsResult.Tables[5];
                    lstAvailableUsers_StageOwner.DataBind();

                    lstAssignedUsers_StageOwner.DataTextField = "USERFULLNAME";
                    lstAssignedUsers_StageOwner.DataValueField = "USERS_iId";
                    lstAssignedUsers_StageOwner.DataSource = objResult.dsResult.Tables[4];
                    lstAssignedUsers_StageOwner.DataBind();
                }

                //fill stage user details if minimum 8 result sets are available
                if (objResult.dsResult != null && objResult.dsResult.Tables.Count > 7)
                {
                    lstAvailableUsers_StageUser.DataTextField = "USERFULLNAME";
                    lstAvailableUsers_StageUser.DataValueField = "USERS_iId";
                    lstAvailableUsers_StageUser.DataSource = objResult.dsResult.Tables[7];
                    lstAvailableUsers_StageUser.DataBind();

                    lstAssignedUsers_StageUser.DataTextField = "USERFULLNAME";
                    lstAssignedUsers_StageUser.DataValueField = "USERS_iId";
                    lstAssignedUsers_StageUser.DataSource = objResult.dsResult.Tables[6];
                    lstAssignedUsers_StageUser.DataBind();
                }

                //fill stage user details if minimum 10 result sets are available
                if (objResult.dsResult != null && objResult.dsResult.Tables.Count > 9)
                {
                    lstAvailableUsers_StageUserGroup.DataTextField = "GROUPS_vName";
                    lstAvailableUsers_StageUserGroup.DataValueField = "GROUPS_iId";
                    lstAvailableUsers_StageUserGroup.DataSource = objResult.dsResult.Tables[9];
                    lstAvailableUsers_StageUserGroup.DataBind();

                    lstAssignedUsers_StageUserGroup.DataTextField = "GROUPS_vName";
                    lstAssignedUsers_StageUserGroup.DataValueField = "GROUPS_iId";
                    lstAssignedUsers_StageUserGroup.DataSource = objResult.dsResult.Tables[8];
                    lstAssignedUsers_StageUserGroup.DataBind();
                }

                //fill stage notification user details if minimum 12 result sets are available
                if (objResult.dsResult != null && objResult.dsResult.Tables.Count > 11)
                {
                    lstAvailableUsers_NotoficationUser.DataTextField = "USERFULLNAME";
                    lstAvailableUsers_NotoficationUser.DataValueField = "USERS_iId";
                    lstAvailableUsers_NotoficationUser.DataSource = objResult.dsResult.Tables[11];
                    lstAvailableUsers_NotoficationUser.DataBind();

                    lstAssignedUsers_NotoficationUser.DataTextField = "USERFULLNAME";
                    lstAssignedUsers_NotoficationUser.DataValueField = "USERS_iId";
                    lstAssignedUsers_NotoficationUser.DataSource = objResult.dsResult.Tables[10];
                    lstAssignedUsers_NotoficationUser.DataBind();
                }
            }
            catch (Exception ex)
            { }
        }

        protected void DisplaySelectedEntityDetails(DBResult objResult)
        {
            try
            {
                //Display process details if minimum 1 result sets are available
                if (objResult.dsResult != null && objResult.dsResult.Tables.Count > 0)
                {
                    string strCurrentProcessName = objResult.dsResult.Tables[0].Rows[0]["WorkflowProcess_vName"].ToString();

                    //lblProcessOwner_CurrentProcessValue.Text = strCurrentProcessName;
                }

                //fill workflow  details if minimum 2 result sets are available
                if (objResult.dsResult != null && objResult.dsResult.Tables.Count > 1)
                {
                    string strCurrentCurrentWorkFlowName = objResult.dsResult.Tables[1].Rows[0]["Workflow_vName"].ToString();
                }

                //fill stage  details if minimum 3 result sets are available
                if (objResult.dsResult != null && objResult.dsResult.Tables.Count > 2)
                {
                    string strCurrentStageName = objResult.dsResult.Tables[2].Rows[0]["WorkflowStage_vDisplayName"].ToString();
                }
            }
            catch (Exception ex)
            { }

        }

        protected void btnAddUser_ProcessOwner_Click(object sender, EventArgs e)
        {
            //  CheckAuthentication();
            try
            {

                string selUserID = lstAvailableUsers_ProcessOwner.SelectedItem.Value;

                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objUserMapping.LoginToken = loginUser.LoginToken;
                objUserMapping.LoginOrgId = loginUser.LoginOrgId;
                objUserMapping.WfUserMappingAction = "SaveUser";
                if (Session["ProcessId"] != null)
                    objUserMapping.WfUserMappingProcessId = Convert.ToInt32(Session["ProcessId"]);

                objUserMapping.WfUserMappingCategoryId = 1001; //process owner
                objUserMapping.WfUserMappingGroupId = 0;
                objUserMapping.WfUserMappingStageId = 0;
                objUserMapping.WfUserMappingWorkflowId = 0;
                objUserMapping.WfUserMappingUserId = Convert.ToInt32(selUserID);


                objUserMapping.ManageWorkflowUserMapping(objUserMapping);
            }
            catch (Exception ex)
            {
                            }

            BindUsers(GetAllUsers());

            hdnCurrentPanelProcessOwner.Value = "0";
        }

        protected void btnRemoveUser_ProcessOwner_Click(object sender, EventArgs e)
        {
            // CheckAuthentication();
            try
            {
                string selUserID = lstAssignedUsers_ProcessOwner.SelectedItem.Value;

                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objUserMapping.LoginToken = loginUser.LoginToken;
                objUserMapping.LoginOrgId = loginUser.LoginOrgId;
                objUserMapping.WfUserMappingAction = "DeleteUser";
                if (Session["ProcessId"] != null)
                    objUserMapping.WfUserMappingProcessId = Convert.ToInt32(Session["ProcessId"]);

                objUserMapping.WfUserMappingCategoryId = 1001; //process owner
                objUserMapping.WfUserMappingGroupId = 0;
                objUserMapping.WfUserMappingStageId = 0;
                objUserMapping.WfUserMappingWorkflowId = 0;
                objUserMapping.WfUserMappingUserId = Convert.ToInt32(selUserID);


                objUserMapping.ManageWorkflowUserMapping(objUserMapping);
            }
            catch (Exception ex)
            {
              
            }
            BindUsers(GetAllUsers());

            hdnCurrentPanelProcessOwner.Value = "0";
        }

        #endregion

        #region WorkflowOwners

        protected void btnAddUser_WorkflowOwner_Click(object sender, EventArgs e)
        {
            // CheckAuthentication();
            try
            {
                string selUserID = lstAvailableUsers_WorkflowOwner.SelectedItem.Value;

                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objUserMapping.LoginToken = loginUser.LoginToken;
                objUserMapping.LoginOrgId = loginUser.LoginOrgId;
                objUserMapping.WfUserMappingAction = "SaveUser";
                if (Session["ProcessId"] != null)
                    objUserMapping.WfUserMappingProcessId = Convert.ToInt32(Session["ProcessId"].ToString());
                if (Session["WorkflowId"] != null)
                    objUserMapping.WfUserMappingWorkflowId = Convert.ToInt32(Session["WorkflowId"].ToString());

                objUserMapping.WfUserMappingCategoryId = 1002; //Workflow owner
                objUserMapping.WfUserMappingGroupId = 0;
                objUserMapping.WfUserMappingStageId = 0;
                objUserMapping.WfUserMappingUserId = Convert.ToInt32(selUserID);


                objUserMapping.ManageWorkflowUserMapping(objUserMapping);
            }
            catch
            {
            }
            BindUsers(GetAllUsers());

        }

        protected void btnRemoveUser_WorkflowOwner_Click(object sender, EventArgs e)
        {
            // CheckAuthentication();
            try
            {
                string selUserID = lstAssignedUsers_WorkflowOwner.SelectedItem.Value;

                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objUserMapping.LoginToken = loginUser.LoginToken;
                objUserMapping.LoginOrgId = loginUser.LoginOrgId;
                objUserMapping.WfUserMappingAction = "DeleteUser";
                if (Session["ProcessId"] != null)
                    objUserMapping.WfUserMappingProcessId = Convert.ToInt32(Session["ProcessId"].ToString());
                if (Session["WorkflowId"] != null)
                    objUserMapping.WfUserMappingWorkflowId = Convert.ToInt32(Session["WorkflowId"].ToString());

                objUserMapping.WfUserMappingCategoryId = 1002; //workflow owner
                objUserMapping.WfUserMappingGroupId = 0;
                objUserMapping.WfUserMappingStageId = 0;
                objUserMapping.WfUserMappingUserId = Convert.ToInt32(selUserID);


                objUserMapping.ManageWorkflowUserMapping(objUserMapping);
            }
            catch
            {
            }
            BindUsers(GetAllUsers());

        }

        #endregion

        #region StageOwners,Notification Users,Stage Users,User groups

        protected void btnAddUser_StageOwner_Click(object sender, EventArgs e)
        {

            try
            {
                string selUserID = lstAvailableUsers_StageOwner.SelectedItem.Value;

                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objUserMapping.LoginToken = loginUser.LoginToken;
                objUserMapping.LoginOrgId = loginUser.LoginOrgId;
                objUserMapping.WfUserMappingAction = "SaveUser";
                objUserMapping.WfUserMappingProcessId = Convert.ToInt32(Session["ProcessId"].ToString());
                objUserMapping.WfUserMappingWorkflowId = Convert.ToInt32(Session["WorkflowId"].ToString());
                objUserMapping.WfUserMappingStageId = Convert.ToInt32(Session["StageId"].ToString());

                objUserMapping.WfUserMappingCategoryId = 1003; //Stage owner
                objUserMapping.WfUserMappingGroupId = 0;
                objUserMapping.WfUserMappingUserId = Convert.ToInt32(selUserID);


                objUserMapping.ManageWorkflowUserMapping(objUserMapping);
            }
            catch (Exception ex)
            {
                
            }
            BindUsers(GetAllUsers());

            hdnCurrentPanel.Value = "0";
        }

        protected void btnRemoveUser_StageOwner_Click(object sender, EventArgs e)
        {

            try
            {
                string selUserID = lstAssignedUsers_StageOwner.SelectedItem.Value;

                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objUserMapping.LoginToken = loginUser.LoginToken;
                objUserMapping.LoginOrgId = loginUser.LoginOrgId;
                objUserMapping.WfUserMappingAction = "DeleteUser";
                objUserMapping.WfUserMappingProcessId = Convert.ToInt32(Session["ProcessId"].ToString());
                objUserMapping.WfUserMappingWorkflowId = Convert.ToInt32(Session["WorkflowId"].ToString());
                objUserMapping.WfUserMappingStageId = Convert.ToInt32(Session["StageId"].ToString());

                objUserMapping.WfUserMappingCategoryId = 1003; //Stage owner
                objUserMapping.WfUserMappingGroupId = 0;
                objUserMapping.WfUserMappingUserId = Convert.ToInt32(selUserID);


                objUserMapping.ManageWorkflowUserMapping(objUserMapping);
            }
            catch (Exception ex)
            {
                
            }
            BindUsers(GetAllUsers());

            hdnCurrentPanel.Value = "0";
        }

        protected void btnAddUser_StageUser_Click(object sender, EventArgs e)
        {

            try
            {
                string selUserID = lstAvailableUsers_StageUser.SelectedItem.Value;

                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objUserMapping.LoginToken = loginUser.LoginToken;
                objUserMapping.LoginOrgId = loginUser.LoginOrgId;
                objUserMapping.WfUserMappingAction = "SaveUser";
                objUserMapping.WfUserMappingProcessId = Convert.ToInt32(Session["ProcessId"].ToString());
                objUserMapping.WfUserMappingWorkflowId = Convert.ToInt32(Session["WorkflowId"].ToString());
                objUserMapping.WfUserMappingStageId = Convert.ToInt32(Session["StageId"].ToString());

                objUserMapping.WfUserMappingCategoryId = 1004; //Stage user
                objUserMapping.WfUserMappingGroupId = 0;
                objUserMapping.WfUserMappingUserId = Convert.ToInt32(selUserID);


                objUserMapping.ManageWorkflowUserMapping(objUserMapping);
            }
            catch (Exception ex)
            {
               
            }
            BindUsers(GetAllUsers());

            hdnCurrentPanel.Value = "1";
            hdnCurrentSubPanel.Value = "0";
        }

        protected void btnRemoveUser_StageUser_Click(object sender, EventArgs e)
        {

            try
            {
                string selUserID = lstAssignedUsers_StageUser.SelectedItem.Value;

                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objUserMapping.LoginToken = loginUser.LoginToken;
                objUserMapping.LoginOrgId = loginUser.LoginOrgId;
                objUserMapping.WfUserMappingAction = "DeleteUser";
                objUserMapping.WfUserMappingProcessId = Convert.ToInt32(Session["ProcessId"].ToString());
                objUserMapping.WfUserMappingWorkflowId = Convert.ToInt32(Session["WorkflowId"].ToString());
                objUserMapping.WfUserMappingStageId = Convert.ToInt32(Session["StageId"].ToString());

                objUserMapping.WfUserMappingCategoryId = 1004; //Stage user
                objUserMapping.WfUserMappingGroupId = 0;
                objUserMapping.WfUserMappingUserId = Convert.ToInt32(selUserID);


                objUserMapping.ManageWorkflowUserMapping(objUserMapping);
            }
            catch (Exception ex)
            {
                
            }
            BindUsers(GetAllUsers());

            hdnCurrentPanel.Value = "1";
            hdnCurrentSubPanel.Value = "0";
        }

        protected void btnAddUser_StageUserGroup_Click(object sender, EventArgs e)
        {

            try
            {
                string selUserID = lstAvailableUsers_StageUserGroup.SelectedItem.Value;

                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objUserMapping.LoginToken = loginUser.LoginToken;
                objUserMapping.LoginOrgId = loginUser.LoginOrgId;
                objUserMapping.WfUserMappingAction = "SaveUser";
                objUserMapping.WfUserMappingProcessId = Convert.ToInt32(Session["ProcessId"].ToString());
                objUserMapping.WfUserMappingWorkflowId = Convert.ToInt32(Session["WorkflowId"].ToString());
                objUserMapping.WfUserMappingStageId = Convert.ToInt32(Session["StageId"].ToString());

                objUserMapping.WfUserMappingCategoryId = 1005; //Stage usergroup
                objUserMapping.WfUserMappingGroupId = Convert.ToInt32(selUserID);
                objUserMapping.WfUserMappingUserId = 0;


                objUserMapping.ManageWorkflowUserMapping(objUserMapping);
            }
            catch (Exception ex)
            {
               
            }
            BindUsers(GetAllUsers());

            hdnCurrentPanel.Value = "1";
            hdnCurrentSubPanel.Value = "1";
        }

        protected void btnRemoveUser_StageUserGroup_Click(object sender, EventArgs e)
        {

            try
            {
                string selUserID = lstAssignedUsers_StageUserGroup.SelectedItem.Value;

                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objUserMapping.LoginToken = loginUser.LoginToken;
                objUserMapping.LoginOrgId = loginUser.LoginOrgId;
                objUserMapping.WfUserMappingAction = "DeleteUser";
                objUserMapping.WfUserMappingProcessId = Convert.ToInt32(Session["ProcessId"].ToString());
                objUserMapping.WfUserMappingWorkflowId = Convert.ToInt32(Session["WorkflowId"].ToString());
                objUserMapping.WfUserMappingStageId = Convert.ToInt32(Session["StageId"].ToString());

                objUserMapping.WfUserMappingCategoryId = 1005; //Stage usergroup
                objUserMapping.WfUserMappingGroupId = Convert.ToInt32(selUserID); ;
                objUserMapping.WfUserMappingUserId = 0;


                objUserMapping.ManageWorkflowUserMapping(objUserMapping);
            }
            catch (Exception ex)
            {
                
            }
            BindUsers(GetAllUsers());

            hdnCurrentPanel.Value = "1";
            hdnCurrentSubPanel.Value = "1";
        }

        protected void btnAddUser_NotoficationUser_Click(object sender, EventArgs e)
        {

            try
            {
                string selUserID = lstAvailableUsers_NotoficationUser.SelectedItem.Value;

                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objUserMapping.LoginToken = loginUser.LoginToken;
                objUserMapping.LoginOrgId = loginUser.LoginOrgId;
                objUserMapping.WfUserMappingAction = "SaveUser";
                objUserMapping.WfUserMappingProcessId = Convert.ToInt32(Session["ProcessId"].ToString());
                objUserMapping.WfUserMappingWorkflowId = Convert.ToInt32(Session["WorkflowId"].ToString());
                objUserMapping.WfUserMappingStageId = Convert.ToInt32(Session["StageId"].ToString());

                objUserMapping.WfUserMappingCategoryId = 1006; //Stage NotoficationUser
                objUserMapping.WfUserMappingGroupId = 0;
                objUserMapping.WfUserMappingUserId = Convert.ToInt32(selUserID);


                objUserMapping.ManageWorkflowUserMapping(objUserMapping);
            }
            catch (Exception ex)
            {
                
            }
            BindUsers(GetAllUsers());

            hdnCurrentPanel.Value = "2";
            hdnCurrentSubPanel.Value = "0";
        }

        protected void btnRemoveUser_NotoficationUser_Click(object sender, EventArgs e)
        {

            try
            {
                string selUserID = lstAssignedUsers_NotoficationUser.SelectedItem.Value;

                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objUserMapping.LoginToken = loginUser.LoginToken;
                objUserMapping.LoginOrgId = loginUser.LoginOrgId;
                objUserMapping.WfUserMappingAction = "DeleteUser";
                objUserMapping.WfUserMappingProcessId = Convert.ToInt32(Session["ProcessId"].ToString());
                objUserMapping.WfUserMappingWorkflowId = Convert.ToInt32(Session["WorkflowId"].ToString());
                objUserMapping.WfUserMappingStageId = Convert.ToInt32(Session["StageId"].ToString());

                objUserMapping.WfUserMappingCategoryId = 1006; //Stage NotoficationUser
                objUserMapping.WfUserMappingGroupId = 0;
                objUserMapping.WfUserMappingUserId = Convert.ToInt32(selUserID);


                objUserMapping.ManageWorkflowUserMapping(objUserMapping);
            }
            catch (Exception ex)
            {
                
            }
            BindUsers(GetAllUsers());

            hdnCurrentPanel.Value = "2";
            hdnCurrentSubPanel.Value = "0";
        }

        #endregion

        #region StageFieldEdit Section

        protected void btnStageFieldEdit_Click(object sender, EventArgs e)
        {
            Session["FieldId"] = hdnStageFieldId.Value;
            pnlProcess.Style.Add("display", "none");
            pnlWorkflow.Style.Add("display", "none");
            pnlStages.Style.Add("display", "none");



        }

        #endregion

        #region Notification Section

        protected void btnNotificationEdit_Click(object sender, EventArgs e)
        {
            try
            {
                Logger.Trace("Notification edit started  (Drag and Drop) ", Session["LoggedUserId"].ToString());
                lblNotificationMessage.Text = string.Empty;
                chkNotificationActive.Enabled = true;
                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objStudio.LoginToken = loginUser.LoginToken;
                objStudio.LoginOrgId = loginUser.LoginOrgId;
                objStudio.ProcessId = Convert.ToInt32(Session["ProcessId"]);
                objStudio.WorkflowId = Convert.ToInt32(Session["WorkflowId"]);
                objStudio.StageId = Convert.ToInt32(hiddenStageId.Value);
                objStudio.Notificationid = Convert.ToInt32(hdnNotificationId.Value);
                objResult = objStudio.ManageStudio(objStudio, "GetNotificationDetailsByID");
                if (objResult.dsResult.Tables.Count > 0 && objResult.dsResult.Tables[0].Rows.Count > 0)
                {
                    DataRow dataRow = objResult.dsResult.Tables[0].Rows[0];
                    GetStatus(); GetCategory();
                    if (dataRow["Status Name"] != null && Convert.ToString(dataRow["Status Name"]) != string.Empty)
                        ddl_Status.Items.FindByText(Convert.ToString(dataRow["Status Name"])).Selected = true;
                    if (dataRow["Notification Category"] != null && Convert.ToString(dataRow["Notification Category"]) != string.Empty)
                        ddl_Category.SelectedValue = Convert.ToString(dataRow["Notification Category"]);
                    txt_TAT.Text = Convert.ToString(dataRow["TAT (minutes)"]).Trim();
                    chkNotificationActive.Checked = Convert.ToBoolean(dataRow["Active"]);

                }
                Logger.Trace("Notification edit finished  (Drag and Drop) ", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            { Logger.Trace("Notification edit finished  (Drag and Drop) " + ex.Message.ToString() , Session["LoggedUserId"].ToString()); }
            ShowNotificationpanel();

        }

        private void ShowNotificationpanel()
        {
            
            pnlProcess.Style.Add("display", "none");
            pnlStages.Style.Add("display", "none");
            pnlStagesAdd.Style.Add("display", "none");
            pnlWorkflow.Style.Add("display", "none");
            pnlStatus.Style.Add("display", "none");
            pnlStatusAdd.Style.Add("display", "none");
            pnlNotifications.Style.Add("display", "block");
        }

        private DBResult GetNotification()
        {
            try
            {
                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objNotification.LoginToken = loginUser.LoginToken;
                objNotification.LoginOrgId = loginUser.LoginOrgId;
                objNotification.NotificationProcessId = Convert.ToInt32(Session["ProcessId"]);
                objNotification.NotificationWorkflowId = Convert.ToInt32(Session["WorkflowId"]);
                objNotification.NotificationStageId = Convert.ToInt32(Session["StageId"]);
            }
            catch (Exception ex)
            { }
            return objNotification.ManageNotificationConfiguration(objNotification, "GetAllNotifications");

        }

        private void GetStatus()
        {
            try
            {
                lblNotificationMessage.Text = string.Empty;
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
            catch (Exception ex)
            { }

        }

        private void GetCategory()
        {
            try
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
            catch (Exception ex)
            { }
        }

        protected void btnNotificationSave_Click(object sender, EventArgs e)
        {

            string action = "AddNotificationConfiguration";
            DBResult objResult = new DBResult();

            try
            {
                Logger.Trace("Notification save started  (Drag and Drop) ", Session["LoggedUserId"].ToString());
                lblNotificationMessage.Text = string.Empty;
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objNotification.LoginToken = loginUser.LoginToken;
                objNotification.LoginOrgId = loginUser.LoginOrgId;

                objNotification.NotificationProcessId = Convert.ToInt32(Session["ProcessId"]);
                objNotification.NotificationWorkflowId = Convert.ToInt32(Session["WorkflowId"]);
                objNotification.NotificationStageId = Convert.ToInt32(Session["StageId"]);
                objNotification.NotificationStatusId = Convert.ToInt32(ddl_Status.SelectedValue);
                objNotification.NotificatontTATDurationTime = Convert.ToInt32(txt_TAT.Text.Trim());
                objNotification.NotificationCategory = Convert.ToString(ddl_Category.SelectedItem.Text);
                objNotification.NotificationIsActive = chkNotificationActive.Checked;



                if (hdnActionNotification.Value == "EditNotification")
                {
                    objNotification.NotificationId = Convert.ToInt32(hdnNotificationId.Value);
                    action = "EditNotificationConfiguration";
                }

                objResult = objNotification.ManageNotificationConfiguration(objNotification, action);
                handleDBResult(objResult, lblNotificationMessage);
               

                if (objResult.ErrorState == 0)
                {
                    hdnActionNotification.Value = "EditNotification";
                    objStudio = new ManageWorkflowStudio();
                    objStudio.LoginToken = loginUser.LoginToken;
                    objStudio.LoginOrgId = loginUser.LoginOrgId;
                    objStudio.ProcessId = Convert.ToInt32(Session["ProcessId"]);
                    objStudio.WorkflowId = Convert.ToInt32(Session["WorkflowId"]);
                    objStudio.StageId = Convert.ToInt32(hiddenStageId.Value);
                   
                    objResult = objStudio.ManageStudio(objStudio, "GetNotificationDetails");

                    if (objResult.dsResult.Tables.Count > 0 && objResult.dsResult.Tables[0].Rows.Count > 0)
                    {
                        int buttonId = 0;
                        string buttonName = string.Empty;
                        int Count = objResult.dsResult.Tables[0].Rows.Count;
                        //Generateing workflow buttons based on process
                        Dictionary<int, string> dictionary = new Dictionary<int, string>();
                        foreach (DataRow row in objResult.dsResult.Tables[0].Rows)
                        {
                            buttonId = Convert.ToInt32(row["NotificationId"]);
                            buttonName = Convert.ToString(row["Notification Category"]);
                            dictionary.Add(buttonId, buttonName);
                        }

                        var values = string.Join(",", dictionary.Select(m => m.Key + "-" + m.Value).ToArray());


                        ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "BuildNotification", "BuildNotification('" + values + "');", true);
                    }
                    else
                    {
                        string values = string.Empty;
                        ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "BuildNotification", "BuildNotification( '" + values + "');", true);
                    }

                    
                }
                Logger.Trace("Notification save finished  (Drag and Drop) ", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Notification save finished  (Drag and Drop) " + ex.Message.ToString(), Session["LoggedUserId"].ToString());
            }

        }

      
        protected void btnNotificationAdd_Click(object sender, EventArgs e)
        {
            GetStatus();
            GetCategory();
            ShowNotificationpanel();
        }

        protected void btnNotificationCancel_Click(object sender, EventArgs e)
        {
            ClosePanels();
        }
        #endregion


        #region Workflow Confirm

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                DBResult dataCount = new DBResult();
                dataCount.dsResult = BindWorkflowStagesCount();
                int stageCount, userCount, fieldCount, statusCount;

                stageCount = Convert.ToInt32(dataCount.dsResult.Tables[0].Rows[0]["StageCount"]);
                userCount = Convert.ToInt32(dataCount.dsResult.Tables[0].Rows[0]["UserCount"]);
                fieldCount = Convert.ToInt32(dataCount.dsResult.Tables[0].Rows[0]["FieldCount"]);
                statusCount = Convert.ToInt32(dataCount.dsResult.Tables[0].Rows[0]["StatusCount"]);
                if ((stageCount <= 0) || (userCount <= 0) || (fieldCount <= 0) || (statusCount <= 0))
                {
                    string message = "alert('You cant confirm as there are no stage or user or field or status associated with this workflow. \\r\\n " + hdnWorkFlowCount.Value + "')";
                    ScriptManager.RegisterClientScriptBlock((sender as Control), this.GetType(), "alert", message, true);


                }
                else
                {
                    DBResult objResult = new DBResult();
                    objWorkflow = GetDefalutParameters();
                    objResult = objWorkflow.ManageWorkflows(objWorkflow, "ConfirmWorkflow");

                }

                HidePanels();
            }
            catch (Exception ex)
            { }
        }

        private void HidePanels()
        {

            pnlNotifications.Style.Add("display", "none");

            pnlProcess.Style.Add("display", "none");

            pnlStages.Style.Add("display", "none");

            pnlStagesAdd.Style.Add("display", "none");

            pnlStatus.Style.Add("display", "none");

            pnlStatusAdd.Style.Add("display", "none");

            pnlWorkflow.Style.Add("display", "none");
        }
        //To find count of stages,users,status for thre particular workflow
        protected DataSet BindWorkflowStagesCount()
        {
            DBResult objResult = new DBResult();
            try
            {

                objWorkflow = GetDefalutParameters();
                //Display Stagecount,Feild count,statuscount,usercount for the particular workflow and process
                objResult = objWorkflow.ManageWorkflows(objWorkflow, "WorkflowStagesCounts");
                string WorkFlowCount = string.Empty;
                if (objResult.dsResult.Tables.Count > 0 && objResult.dsResult.Tables[0].Rows.Count > 0)
                {

                    WorkFlowCount = " Number of stages  : " + Convert.ToString(objResult.dsResult.Tables[0].Rows[0]["StageCount"] + " \\n");
                    WorkFlowCount += " Number of users   : " + Convert.ToString(objResult.dsResult.Tables[0].Rows[0]["UserCount"] + " \\n");
                    WorkFlowCount += " Number of fields  : " + Convert.ToString(objResult.dsResult.Tables[0].Rows[0]["FieldCount"] + " \\n");
                    WorkFlowCount += " Number of status  : " + Convert.ToString(objResult.dsResult.Tables[0].Rows[0]["StatusCount"] + " \\n");
                    hdnWorkFlowCount.Value = WorkFlowCount;

                }
            }
            catch (Exception ex)
            { }
            return objResult.dsResult;
        }


        protected Workflows GetDefalutParameters()
        {
            try
            {
                int iCurrentProcessID = Convert.ToInt32(Session["ProcessId"].ToString());
                int WorkflowId = Convert.ToInt32(Session["WorkflowId"].ToString());
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objWorkflow.LoginToken = loginUser.LoginToken;
                objWorkflow.LoginOrgId = loginUser.LoginOrgId;
                objWorkflow.WorkFlowProcessId = iCurrentProcessID;
                objWorkflow.WorkflowId = Convert.ToInt32(WorkflowId);
            }
            catch (Exception ex)
            { }
            return objWorkflow;
        }
        #endregion
        
        //Return values drom Db
        private string ReturnDBValues(DataTable Dt)
        {
            //Binding process which are active only
            string values = string.Empty;
            if (Dt.Rows.Count > 0)
            {
                int Count = Dt.Rows.Count;
                //Generateing workflow buttons based on process
                Dictionary<int, string> dictionary = new Dictionary<int, string>();
                foreach (DataRow row in Dt.Rows)
                {
                    int buttonId = Convert.ToInt32(row["ID"]);
                    string buttonName = Convert.ToString(row["Name"]);
                    dictionary.Add(buttonId, buttonName);
                }

                values = string.Join(", ", dictionary.Select(m => m.Key + "-" + m.Value).ToArray());

            }
            else
            {
                values = string.Empty;

            }

            return values;
        }

        protected void ClearField_Click(object sender, EventArgs e)
        {
            Session.Remove("FieldId");
        }
    }
}