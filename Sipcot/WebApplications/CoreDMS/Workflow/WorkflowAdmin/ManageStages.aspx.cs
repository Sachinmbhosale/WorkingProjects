/* ============================================================================  
Author     : 
Create date: 
===============================================================================  
** Change History   
** Date:          Author:            Issue ID                         Description:  
** ----------   -------------       ----------                  ----------------------------
** 03 Dec 2013          Mandatory   (UMF)                Set Mandatory for index fields
 *
 * Modified
 * 25/02/2015   Sabina              DMS5-3363               Save the Stage Data
 *                                  DMS5-3362               Upload Stage Template
 *                                  DMS5-3361               Load DataEntry Types Dropdownlist
 * 28/02/2015                       DMS5-3384               Add dataentryId to querystring to enable coordinate textbox
 * 17/03/2015                       DMS5-3578               Bringing warning message on changing the dataentry type in Stage edit page
 * 07/04/2015                       DMS5-3845               Uploading template in stage edit change in configuration after code review
 *                                  DMS5-3846               function to create temporary and original folders in stage edit
 *17 Apr 2015   Gokul               DMS5-3946               Applying rights an permissions in all pages as per user group rights!   
 *08 Apr 2015   Sabina              DMS5-4122               Button not enabled in workflow pages
 **21 May 2015  Sharath             DMS5-4268               We require the Delete option for Unwanted Stages & Status
 *05 jul 2015   sharath             DMS5-4397               Deleted stages cause the Workitem ID to be incremented in the number of times the stage was added.
 *10/08/2015    sharath             DMSENH6-4732            Deletion options within Workflow
 *
=============================================================================== */

using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WorkflowBAL;
using WorkflowBLL.Classes;
using Lotex.EnterpriseSolutions.CoreBE;
using System.IO;
using OfficeConverter;
using System.Configuration;


namespace Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin
{
    public partial class ManageStages : PageBase
    {
        #region GLOBAL DECLARATION

        WorkflowStage objStage = new WorkflowStage();
        public string pageRights = string.Empty; /* DMS5-3946 A */
        
        #endregion

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

      
        protected void Page_Load(object sender, EventArgs e)
        {
            CheckAuthentication();          
            pageRights = GetPageRights("CONFIGURATION");/* DMS5-3946 A,DMS5-4122M added parameter configuration */

            // DMSENH6-4732 BS
            //Disable buttons if the workflow is confirmed
          string  workflowConfirmed = Decrypt(HttpUtility.UrlDecode(Request.QueryString["WorkflowConfirmed"]));
          if ((workflowConfirmed) != null && (workflowConfirmed).Equals("Confirmed"))
          {
              Session["workflowConfirmed"] = "Yes";
              DisableControls();
          }
          else
          {
              Session["workflowConfirmed"] = null;
          }
            // DMSENH6-4732 BE
          
            //Start: ------------------SitePath Details ----------------------
            Label sitePath = (Label)Master.FindControl("SitePath");
            sitePath.Text = "WorkFlow >> Process >> Workflow >> Manage Stages";
            //End: ------------------SitePath Details ------------------------

           //Disable ok button after first click while associating stages
            //To avoid duplicated entry of stages on ok button click
            btnOk.Attributes.Add("onclick", "javascript:" + btnOk.ClientID + ".disabled=true;" + ClientScript.GetPostBackEventReference(btnOk, ""));// DMS5-4397

          

            if (!Page.IsPostBack)
            {
                ApplyPageRights(pageRights, Page.Controls); /* DMS5-3946 A */
                string ProcessId, WorkflowId;
                ProcessId = Decrypt(HttpUtility.UrlDecode(Request.QueryString["ProcessId"]));
                WorkflowId = Decrypt(HttpUtility.UrlDecode(Request.QueryString["WorkflowId"]));
                // Validate querystring and handle page redirection
                if (ProcessId == null)
                {
                    Response.Redirect("~/Workflow/WorkflowAdmin/ManageProcess.aspx");
                }
                else if (WorkflowId == null)
                {

                    string QueryString = "?ProcessId=" + (ProcessId);
                    Response.Redirect("~/Workflow/WorkflowAdmin/ManageWorkflow.aspx" + QueryString);
                }
                else
                {
                    ViewState["ProcessId"] = ProcessId;
                    ViewState["WorkflowId"] = WorkflowId;
                    ViewState["WorkflowConfirmed"] = workflowConfirmed;
                }
                //binding the grid with stage details
                BindStagesGridview(GetAllStages());
                // DMS5-3361
                LoadDataEntryTypes();
                //Enable the browse button based on dataentry type
                ddlDataEntry.Attributes.Add("onchange", "EnableControl();");
            }
        }

        #region "Stages"

        private DBResult GetAllStages()
        {
            int ProcessId = Convert.ToInt32(ViewState["ProcessId"]);
            int WorkflowId = Convert.ToInt32(ViewState["WorkflowId"]);
            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            objStage.LoginToken = loginUser.LoginToken;
            objStage.LoginOrgId = loginUser.LoginOrgId;
            objStage.ProcessId = ProcessId;
            objStage.WorkflowId = WorkflowId;
            return objStage.ManageWorkflowStages(objStage, "GetAllStages");
        }
       

        //DMS5-3578BS warning message on saving
        protected void btnSave_Click(object sender, EventArgs e)
        {
            Logger.Trace("User clicked save to save stage details.", Session["LoggedUserId"].ToString());

            DBResult objResult = new DBResult();
            int stageFieldsCount = 0;
            int stageDataEntryTypeId = 0;
            string stageDataEntryType = string.Empty;
            string selectedDataEntryType = string.Empty;

            try
            {
                objStage = GetValuesFromSession();

                //selected data entry type id
                objStage.DataEntryId = (ddlDataEntry.SelectedIndex > -1) ? Convert.ToInt32(ddlDataEntry.SelectedValue) : 0;
                selectedDataEntryType = (ddlDataEntry.SelectedIndex > -1) ? ddlDataEntry.SelectedItem.Text : string.Empty;
                Logger.Trace("Selected data entry type: " + selectedDataEntryType, Session["LoggedUserId"].ToString());
                // current data entry type id; taken from grid
                stageDataEntryTypeId = (hdnDataentryId.Value != null && Convert.ToString(hdnDataentryId.Value).Length > 0) ? Convert.ToInt32(hdnDataentryId.Value) : 0;
                Logger.Trace("Selected data entry type Id: " + stageDataEntryTypeId, Session["LoggedUserId"].ToString());

                // Validate file/template is uploaded if data entry type changed
                if (stageDataEntryTypeId != objStage.DataEntryId)
                {

                    if (selectedDataEntryType != "Normal" && UploadedFilePath.Length == 0)
                    {
                        lblMessage.Text = "Please select a template";
                        ScriptManager.RegisterStartupScript(this, typeof(Page), "ReloadDialogbox", "ReloadDialogbox();", true);
                        return;
                    }

                    //DMS5-3363BS   
                    // Get actual data entry type; if it's "guided" or "Form" and user changing to any other type
                    // show warning message to user before saving i.e., "Data location / co-ordinates values will be lost".
                    stageDataEntryType = (hdnDataEntryName.Value != null && Convert.ToString(hdnDataEntryName.Value).Length > 0) ? Convert.ToString(hdnDataEntryName.Value) : string.Empty;
                    Logger.Trace("Actual data entry type before editing the data entry type: " + stageDataEntryType, Session["LoggedUserId"].ToString());
                    if (stageDataEntryType != "Normal")
                    {
                        //get the stagefields count mapped to particular stage which has position(co-odinates).                            
                        objResult = objStage.ManageWorkflowStages(objStage, "CheckMappingOfStage");
                        handleDBResult(objResult);

                        //Warning message when the data entry type is changes from "guided" or "form" to any other type
                        if (objResult.dsResult != null && objResult.dsResult.Tables.Count > 0 && objResult.dsResult.Tables[0].Rows.Count > 0)
                        {
                            stageFieldsCount = Convert.ToInt32(objResult.dsResult.Tables[0].Rows[0]["TotalStageFields"]);
                            Logger.Trace("Total fields with data position captured in the stage: " + stageFieldsCount, Session["LoggedUserId"].ToString());

                            //if fields exits, And current & selected data enrtry type is different show warning message
                            if (stageFieldsCount > 0)
                            {
                                Logger.Trace("Warning message dispalyed due to mapping in stage and stagefields", Session["LoggedUserId"].ToString());
                                //show warning message
                                ScriptManager.RegisterStartupScript(this, typeof(Page), "ConfirmBox", "ConfirmBox();", true);
                            }
                            else
                            {
                                //normal saving
                                ScriptManager.RegisterStartupScript(this, typeof(Page), "SaveFunctionality", "SaveFunctionality();", true);
                            }
                        }
                    }
                    else
                    {
                        //normal saving
                        ScriptManager.RegisterStartupScript(this, typeof(Page), "SaveFunctionality", "SaveFunctionality();", true);
                    }

                }
                else
                {
                    //normal saving
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "SaveFunctionality", "SaveFunctionality();", true);
                }

            }
            catch (Exception ex)
            {
                Logger.Trace("Exception " + ex.Message, Session["LoggedUserId"].ToString());
                lblMessage.Text = ex.Message;
                ScriptManager.RegisterStartupScript(this, typeof(Page), "ReloadDialogbox", "ReloadDialogbox();", true);
            }
        }
        //DMS5-3578BE

        private void handleDBResult(DBResult objResult)
        {
            if (objResult.ErrorState == 0)
            {
                // Success
                if (objResult.Message.Length > 0)
                {
                    lblErrorMessage.Text = (objResult.Message);
                }
            }
            else if (objResult.ErrorState == -1)
            {
                // Warning
                if (objResult.Message.Length > 0)
                {
                    lblErrorMessage.Text = (objResult.Message);
                }
            }
            else if (objResult.ErrorState == 1)
            {
                // Error
                if (objResult.Message.Length > 0)
                {
                    lblErrorMessage.Text = (objResult.Message);
                }
            }
        }

        private void clearControls()
        {
            txtStageName.Text = string.Empty;
            txtStageDescription.Text = string.Empty;
            chkActive.Checked = false;
        }

        protected void BindStagesGridview(DBResult objResult)
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
                        WorkFlowCommonFns.SetGridHeaderForMultiLanguage(ref gridStage, "WFSTAGE", loginUser.LoginToken, loginUser.LoginOrgId, loginUser.LanguageID, loginUser.UserId);
                    }
                }

                if (objResult.dsResult.Tables.Count > 1)
                {
                    // Second table contains available stages from master stages
                    gridStageMaster.DataSource = objResult.dsResult.Tables[1];
                    gridStageMaster.DataBind();
                    //Multilanguage implementation for Grid headers
                    UserBase loginUser = (UserBase)Session["LoggedUser"];
                    if (loginUser.LanguageCode != "en-US")
                    {
                        WorkFlowCommonFns.SetGridHeaderForMultiLanguage(ref gridStageMaster, "STAGELIST", loginUser.LoginToken, loginUser.LoginOrgId, loginUser.LanguageID, loginUser.UserId);
                    }
                }
            }
        }

        #endregion

        protected void btnOk_Click(object sender, EventArgs e)
        {
           
     
            lblErrorMessage.Text = string.Empty;
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
            objStage.WorkflowId = Convert.ToInt32(ViewState["WorkflowId"]);
            objStage.CommaSeparatedStageIds = CommaSeparatedStageIds;
            objStage.ProcessId = Convert.ToInt32(ViewState["ProcessId"]);
            //insert into stage table
            objResult = objStage.ManageWorkflowStages(objStage, "AddStage");

            if (objResult.ErrorState != 0)
            {
                lblErrorMessage.Text = objResult.Message.ToString();
            }
            BindStagesGridview(GetAllStages());
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

        protected void gridStage_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header || e.Row.RowType == DataControlRowType.DataRow)
            {
                //hide Unwanted columns
                e.Row.Cells[2].Style.Add("display", "none");
                e.Row.Cells[3].Style.Add("display", "none");
                e.Row.Cells[4].Style.Add("display", "none");
                e.Row.Cells[9].Style.Add("display", "none");

                //Added newly hiding the template path and dataentryId
                e.Row.Cells[11].Style.Add("display", "none");
                e.Row.Cells[12].Style.Add("display", "none");
                //e.Row.Cells[14].Style.Add("display", "none"); //Show Background image

                e.Row.Cells[0].Style.Add("width", "40px"); //edit
                e.Row.Cells[1].Style.Add("width", "160px"); //options
                e.Row.Cells[5].Style.Add("width", "40px"); //Slno
                e.Row.Cells[6].Style.Add("width", "100px"); //stage name
                e.Row.Cells[7].Style.Add("width", "160px"); //Description
                e.Row.Cells[8].Style.Add("width", "40px"); //active
                e.Row.Cells[10].Style.Add("width", "80px"); //TAT
                e.Row.Cells[13].Style.Add("width", "100px"); //DataEntryName    
                e.Row.Cells[14].Style.Add("width", "100px"); //Background image    
            }
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center; //edit
                e.Row.Cells[1].HorizontalAlign = HorizontalAlign.Center; //options
                e.Row.Cells[8].HorizontalAlign = HorizontalAlign.Center; //active
                e.Row.Cells[10].HorizontalAlign = HorizontalAlign.Center; //TAT

                string ProcessId = ViewState["ProcessId"].ToString(); //ProcessId
                string WorkflowId = ViewState["WorkflowId"].ToString(); //WorkflowId
                string workflowConfirmed =ViewState["WorkflowConfirmed"].ToString(); //WorkflowConfirmed

                ProcessId = HttpUtility.UrlEncode(Encrypt(ProcessId));
                WorkflowId = HttpUtility.UrlEncode(Encrypt(WorkflowId));
                workflowConfirmed = HttpUtility.UrlEncode(Encrypt(workflowConfirmed));

                string StageId = Encrypt(((System.Data.DataRowView)(e.Row.DataItem)).Row.ItemArray[1].ToString()); //stage id
                string StageName = Encrypt(((System.Data.DataRowView)(e.Row.DataItem)).Row.ItemArray[4].ToString()); //stage name              
                //DMS5-3384A
                string dataEntryTypeId = Encrypt(((System.Data.DataRowView)(e.Row.DataItem)).Row.ItemArray[10].ToString());//To get the dataentry type
                //DMS5-3384M
                string QueryString = "?ProcessId=" + ProcessId + "&WorkflowId=" + WorkflowId + "&StageId=" + StageId + "&StageName=" + StageName + "&dataEntryTypeId=" + dataEntryTypeId + "&WorkflowConfirmed=" + workflowConfirmed; 

                LinkButton lnkManageStageUsers = (LinkButton)e.Row.FindControl("lnkManageStageUsers");
                lnkManageStageUsers.PostBackUrl = "~/Workflow/WorkflowAdmin/ManageStageUsers.aspx" + QueryString;

                LinkButton lnkManageStageFeilds = (LinkButton)e.Row.FindControl("lnkManageStageFeilds");
                lnkManageStageFeilds.PostBackUrl = "~/Workflow/WorkflowAdmin/ManageStageFeildsList.aspx" + QueryString;

                LinkButton lnkManageStageStatuses = (LinkButton)e.Row.FindControl("lnkManageStageStatuses");
                lnkManageStageStatuses.PostBackUrl = "~/Workflow/WorkflowAdmin/ManageStatus.aspx" + QueryString;

                LinkButton lnkNotifications = (LinkButton)e.Row.FindControl("lnkNotifications");
                lnkNotifications.PostBackUrl = "~/Workflow/WorkflowAdmin/ManageNotification.aspx" + QueryString;

            }
        }

        protected void btnGoBacktoWorkflow_Click(object sender, EventArgs e)
        {
            CheckAuthentication();
            string ProcessId = ViewState["ProcessId"].ToString(); //ProcessId
            ProcessId = HttpUtility.UrlEncode(Encrypt(ProcessId));
            string QueryString = "?ProcessId=" + ProcessId;
            Response.Redirect("~/Workflow/WorkflowAdmin/ManageWorkflow.aspx" + QueryString);
        }


        protected void gridStage_PageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            CheckAuthentication();
            gridStage.PageIndex = e.NewPageIndex;
            BindStagesGridview(GetAllStages());
        }

       //DMS5-3362   Upload Stage Template
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
                        Logger.Trace("File is not supportted as the file extension is:"+fileExtension, Session["LoggedUserId"].ToString());
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

        //DMS5-3362BE

        //DMS5-3361BS
        /// <summary>
        /// Load dropdownlist of dataentry types
        /// </summary>
        public void LoadDataEntryTypes()
        {
            DBResult objResult = new DBResult();
            objStage = GetValuesFromSession();//Get the value from session
            objResult = objStage.ManageWorkflowStages(objStage, "GetAllDataEntryTypes");
            Logger.Trace("DataEntryTypes count : " + objResult.dsResult.Tables[0].Rows.Count, Session["LoggedUserId"].ToString());
            ddlDataEntry.DataSource = objResult.dsResult;
            ddlDataEntry.DataTextField = "GEN_vDescription";
            ddlDataEntry.DataValueField = "GEN_iID";
            ddlDataEntry.DataBind();
        }
        //DMS5-3361BE

        /// <summary>
        /// Load the values from session
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        public WorkflowStage GetValuesFromSession()
        {
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            objStage.LoginToken = loginUser.LoginToken;
            objStage.LoginOrgId = loginUser.LoginOrgId;
            objStage.ProcessId = ViewState["ProcessId"] != null ? Convert.ToInt32(ViewState["ProcessId"]) : 0;
            objStage.WorkflowId = ViewState["WorkflowId"] != null ? Convert.ToInt32(ViewState["WorkflowId"]) : 0;
            if (hdnStageId.Value != null && hdnStageId.Value != string.Empty)
            {
                objStage.StageId = Convert.ToInt32(hdnStageId.Value);
            }
            return objStage;
        }
        //DMS5-3363
        //Saving or updating the stage details
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            CheckAuthentication();

            DBResult objResult = new DBResult();
            string action = "AddStage";
            int dataEntryId = 0;
            string sourceFilePath = string.Empty;
            string processId = string.Empty;
            string workflowId = string.Empty;
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
                objStage.Active = chkActive.Checked;
                objStage.TatDuration = Convert.ToInt32(txtTATDuration.Text.Trim());
                string strSaveStatus = hdnSaveStatus.Value;
                objStage = GetValuesFromSession();
                objStage.DataEntryId = Convert.ToInt32(ddlDataEntry.SelectedValue);
                objStage.bShowBackgroundImage = chkBackgroundImage.Checked;
                //DMS5-3363BS
                objStage.TemplatePath = UploadedFilePath.Length > 0 ? UploadedFilePath.Replace(templateTempFolderPath, templateOriginalFolderPath) : stageTemplatePath; //getting original template path
                Logger.Trace("Template path: " + objStage.TemplatePath, Session["LoggedUserId"].ToString());
                //DMS5-3363BE

                Logger.Trace("Data Entry Id : " + objStage.DataEntryId, Session["LoggedUserId"].ToString());
                if (strSaveStatus == "Save Changes")
                {
                    objStage.Deleted = chkActive.Checked;
                    action = "EditStage";
                }
                Logger.Trace("Action for saving the stage : " + action, Session["LoggedUserId"].ToString());

                //saving stage details
                objResult = objStage.ManageWorkflowStages(objStage, action);
                handleDBResult(objResult);//to display message based on result set.

                //only if success
                if (objResult.ErrorState == 0)
                {
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

                    dataEntryId = Convert.ToInt32(hdnDataentryId.Value);

                    //checking whether the dataentry type before editing and after editing is same or not
                    if (dataEntryId != objStage.DataEntryId)
                    {
                        dataEntryType = ddlDataEntry.SelectedItem.Text;
                        if (dataEntryType == "Guided" || dataEntryType == "Form")
                        {
                            #region Redirect to fields page to capture fields again
                            processId = HttpUtility.UrlEncode(Encrypt(Convert.ToString(objStage.ProcessId)));
                            workflowId = HttpUtility.UrlEncode(Encrypt(Convert.ToString(objStage.WorkflowId)));
                            stageId = HttpUtility.UrlEncode(Encrypt(Convert.ToString(objStage.StageId)));
                            //objStage.StageName =hdnStageName.Value != null ? hdnStageName.Value : string.Empty;
                            stageName = hdnStageName.Value != null ? HttpUtility.UrlEncode(Encrypt(Convert.ToString(hdnStageName.Value))) : string.Empty;
                            dataEntryTypeId = HttpUtility.UrlEncode(Encrypt(Convert.ToString(objStage.DataEntryId)));
                            string QueryString = "?ProcessId=" + processId + "&WorkflowId=" + workflowId + "&StageId=" + stageId + "&StageName=" + stageName + "&dataEntryTypeId=" + dataEntryTypeId;

                            Response.Redirect("~/Workflow/WorkflowAdmin/ManageStageFeildsList.aspx" + QueryString);
                            #endregion
                        }
                        else
                        {
                            ////Deleting the template which was saved initially in temporary folder
                            new ManageDirectory().DeleteDirectory(templateOriginalFolderPath);
                        }
                    }

                    //Clear stage edit controls
                    clearControls();
                    lblMessage.Text = string.Empty;
                    //Rebind grid
                    BindStagesGridview(GetAllStages());
                    hdnErrorStatus.Value = "";

                }
                else
                {
                    //delete the saved template from original location id database save fails.
                    new ManageDirectory().DeleteDirectory(templateOriginalFolderPath);

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
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
                lblErrorMessage.ForeColor = System.Drawing.Color.Red;
                lblErrorMessage.Text = ex.Message;
            }
        }

        /*/DMS5-3363BS
        /// <summary>
        /// Copying file from temporary location to oiginal location        
        /// </summary>
        public void CopyTemplateToOriginalLocation(string tempSourcePath, string originalSourcePath)
        {
            string fileExtension = string.Empty;
            string splittedFilesTempPath = string.Empty;
            string splittedFilesOriginalPath = string.Empty;
            int splittedFilesCount = 0;

            try
            {
                if (File.Exists(tempSourcePath))
                {
                    //Copying file from temporary location to original folder
                    Logger.Trace("Copying file from temporary (" + tempSourcePath + ") to original (" + originalSourcePath + ") location.", Session["LoggedUserId"].ToString());
                    new ManageDirectory().FileCopy(tempSourcePath, originalSourcePath);

                    //Copying splitted folder(created on file upload) from temporary folder to original folder

                    if (File.Exists(originalSourcePath))
                    {
                        splittedFilesOriginalPath = beforedot(originalSourcePath);
                        fileExtension = afterdot(tempSourcePath);

                        if (fileExtension.ToLower() == ".pdf")
                        {
                            splittedFilesTempPath = beforedot(tempSourcePath);

                            Logger.Trace("Copying splitted files from temporary (" + splittedFilesTempPath + ") to original (" + splittedFilesOriginalPath + ") location.", Session["LoggedUserId"].ToString());

                            splittedFilesCount = new DirectoryInfo(splittedFilesTempPath).GetFiles().Length;
                            Logger.Trace("Splitted files count in temporary location is: " + splittedFilesCount, Session["LoggedUserId"].ToString());

                            if (splittedFilesCount > 0)
                                new ManageDirectory().CopyDirectory(splittedFilesTempPath, splittedFilesOriginalPath);
                        }
                        else//Incase of images like jpg,png,jpeg
                        {
                            Logger.Trace("Copying single page files/images to original (" + splittedFilesOriginalPath + ") location.", Session["LoggedUserId"].ToString());
                            new ManageDirectory().CreateDirectory(splittedFilesOriginalPath);

                            string originalDestination = splittedFilesOriginalPath + "/1" + fileExtension;
                            new ManageDirectory().FileCopy(originalSourcePath, originalDestination);
                        }

                        //Store file path in hidden variable to save database
                        hdnStageTemplatePath.Value = originalSourcePath;
                        ViewState["checkUpload"] = "Uploaded";

                    }
                    else
                    {
                        Logger.Trace("File does not exist in original location:" + originalSourcePath, Session["LoggedUserId"].ToString());
                    }
                }
                else
                {
                    Logger.Trace("File does not exist in temporary location:" + tempSourcePath, Session["LoggedUserId"].ToString());
                }
            }
            catch
            {
                throw;
            }
        }
        *///DMS5-3363BE


        // DMS5-3846BS
        /// <summary>
        /// Getting the temporary template folder path
        /// </summary>
        /// <returns></returns>
        private string GetTemplateTemporaryFolderPath()
        {
            string path = string.Empty;
            string folderPath = ConfigurationManager.AppSettings["TempWorkFolder"];
            //Get data from ViewState            
            objStage = GetValuesFromSession();
            path = folderPath + "/" + objStage.ProcessId + "/" + objStage.WorkflowId + "/" + objStage.StageId;
            return path;
        }
        /// <summary>
        /// getting the original template folder path
        /// </summary>
        /// <returns></returns>
        private string GetTemplateOriginalFolderPath()
        {
            string path = string.Empty;
            string folderPath = ConfigurationManager.AppSettings["StageTemplatePath"];
            //Get data from ViewState            
            objStage = GetValuesFromSession();
            path = folderPath + "/" + objStage.ProcessId + "/" + objStage.WorkflowId + "/" + objStage.StageId;

            return path;
        }
        // DMS5-3846BE

        //  DMS5-4268 BS
        protected void btnYes_Click(object sender, EventArgs e)
        {
            lblErrorMessage.Text = string.Empty;
             GetValuesFromSession();
            int StageId = Convert.ToInt32(ViewState["StageId"]);
            DBResult objResult = new DBResult();
            string action = "DeleteStage";
            objStage.StageId = StageId;           
            objResult = objStage.ManageWorkflowStages(objStage, action);
            if (objResult.ErrorState != 0)
            {
                lblErrorMessage.Text = objResult.Message.ToString();
                lblErrorMessage.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                lblErrorMessage.Text = objResult.Message.ToString();
                BindStagesGridview(GetAllStages());
            }
        }

        protected void btnDelete_Click(object sender,EventArgs e)
        {
            LinkButton btndetails = sender as LinkButton;
            GridViewRow gvrow = (GridViewRow)btndetails.NamingContainer;
            ViewState["StageId"] = gvrow.Cells[3].Text;
            ViewState["StageName"] = gvrow.Cells[7].Text;
            lblUser.Text = "Are you sure you want to delete " + ViewState["StageName"] + " Stage?";
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