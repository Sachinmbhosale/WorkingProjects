/* ============================================================================  
Author     : Robbin Thomas
Create date: 
===============================================================================  
** Change History   
** Date:          Author:            Issue ID           Description:  
** ----------   -------------       ----------          ----------------------------
** 03 Mar 2015  Yogeesha Naik        Bug DMS04-3429D    Removed parentId check as sub dropdown was not filling
 * 11 Apr 2015  Sabina               DMS5-3897          Fullstop(.) is not allowed in a string data type field, causes issue in data entry
 * 27/02/2015   Sharath              DMS5-3424          Auto Calculate Age on Current Date Selection
 *                                   DMS5-3425          Dual Data Entry Functionality
 *                                   DMS5-3473          Calculate year diffrence between user input and refer date field
 *                                   DMS5-3474          Formula Calculation
 *                                   DMS5-3627          Document/Image Uplaod
 * 17 Apr 2015  Yogeesha Naik        DMS5-3947          Rewritten most of the methods related to dynamic controls (made generic)
 *17 Apr 2015      Gokul             DMS5-3946          Applying rights an permissions in all pages as per user group rights!
 *08 Apr 2015     Sabina             DMS5-4122          Button not enabled in workflow pages
 *02 June 2015  Sharath              DMS5-4331          Disabling of Stage fields not working  
 *02 June 2015  Sharath              DMS5-4332          A completed workflow should not be editable or reopened once last stage has Closed/Rejected.
 *08 June 2015  Sharath              DMS5-4263          The age calculate fields are inherited but the values get changed in the next stage
 *23 June 2015  Sharath              DMS5-4388          Once user submitted the task items immediately show next one for processing rather going back to Dashboard (Priority configuration to pick items of same stage(last processed), FIFO, LIFO ).
 *01 Jul  2015  Sharath              DMS5-4423          Data Captured Form usage
 *08 Jul  2015  Sharath              DMS5-4398          Workitem ID needs to be displayed in the data entry page
 *15 Jul 2015   Sharath              DMSENH6-4640       Closure of activities on Workflow  
 *24/07/2015    Sharath              DMSENH6-4716       On setting workitem to Closed status and Submitting it, error page is displayed
 * 08-26-2015   gokuldas.palapatta   DMSENH6-4776       Data Captured Form usage
 * 20-10-2015   Harikrishna.k   Task WDMS-S-5156        After clicking on history tab image is disappearing. And there is no history getting generated.
 * 22-10-2015   gokuldas.palapatta   DMSENH6-5158       Doc file is not opening in the viewer workflow
=============================================================================== */

using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WorkflowBAL;
using WorkflowBLL.Classes;
using System.Data;
using Lotex.EnterpriseSolutions.CoreBE;
using OfficeConverter;
using Ionic.Zip;
using System.IO;
using System.Collections.Generic;
using AjaxControlToolkit;
using System.Configuration;

namespace Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin
{
    public partial class WorkflowDataEntry : PageBase
    {
        WorkflowDataEntryBLL ObjDataEntry = new WorkflowDataEntryBLL();
        public string pageRights = string.Empty; /* DMS5-3946 A */

        //To update the annoted pages in annotation table
        protected DataTable FormulaFieldData
        {
            get
            {
                if (ViewState["FormulaFieldData"] != null)
                {
                    return (DataTable)ViewState["FormulaFieldData"];
                }
                else
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("ReferredFieldId", typeof(string));
                    dt.Columns.Add("ReferredFieldName", typeof(string));
                    dt.Columns.Add("ReferredFieldIdList", typeof(string));
                    dt.Columns.Add("FormulaFieldId", typeof(string));
                    dt.Columns.Add("Formula", typeof(string));

                    return dt;
                }
            }
            set { ViewState["FormulaFieldData"] = value; }
        }

        //To update uploaded file path and upload control Id
        protected DataTable UploadedFiles
        {
            get
            {
                if (Session["UploadedFiles"] != null)
                {
                    return (DataTable)Session["UploadedFiles"];
                }
                else
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("ControlId", typeof(string));
                    dt.Columns.Add("FilePath", typeof(string));
                    return dt;
                }
            }
            set { Session["UploadedFiles"] = value; }
        }

        string mandatoryValidationScript = string.Empty;
        string specialCharValidationScript = string.Empty;
        string rangeValidationScript = string.Empty;
        string updateQuery = string.Empty;

        public string strImagePath = string.Empty;



        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                CheckAuthentication();
                pageRights = GetPageRights("CONFIGURATION");/* DMS5-3946 A,DMS5-4122M added parameter configuration */
                WFPDFViewer.AnnotationToolbarVisible = false;
                if (!IsPostBack)
                {
                    ApplyPageRights(pageRights, this.Form.Controls); /* DMS5-3946 A */
                    //Start: ------------------SitePath Details ----------------------
                    Label sitePath = (Label)Master.FindControl("SitePath");
                    sitePath.Text = "WorkFlow Tasks >> Task Items >> Data Entry";
                    //End: ------------------SitePath Details ------------------------


                    string processId, workflowId, stageId, FieldDataId, PageMode = string.Empty;
                    processId = Decrypt(HttpUtility.UrlDecode(Request.QueryString["ProcessId"]));
                    workflowId = Decrypt(HttpUtility.UrlDecode(Request.QueryString["WorkflowId"]));
                    stageId = Decrypt(HttpUtility.UrlDecode(Request.QueryString["StageId"]));
                    FieldDataId = Decrypt(HttpUtility.UrlDecode(Request.QueryString["FieldDataId"]));


                    if (Request.QueryString["PageMode"] != null)
                    {
                        PageMode = Request.QueryString["PageMode"];
                    }

                    if (PageMode == "ViewMode")
                    {
                        btnSave.Visible = false;
                        btnCancel.Visible = false;
                        btnClose.Visible = true;
                    }
                    else
                    {
                        btnSave.Visible = true;
                        btnCancel.Visible = true;
                        btnClose.Visible = false;
                    }

                    Session["ProcessId"] = processId;
                    Session["WorkflowId"] = workflowId;
                    Session["StageId"] = stageId;
                    Session["FieldDataId"] = FieldDataId;
                    Session["PageMode"] = PageMode;

                    if (Session["WorkflowControlDetails"] != null)
                    {
                        Session.Remove("WorkflowControlDetails");
                    }

                    BindStatusDropDownList();
                    //LoadImageToViewer(string.Empty);
                }

                if (hdnSaveStatus.Value != "Cancel")
                {
                    strImagePath = BindControls();
                    Logger.Trace("Before binding image to viewer " + strImagePath, Session["LoggedUserId"].ToString());
                    if (strImagePath != string.Empty)
                    {
                        if ((!IsPostBack) && (strImagePath != string.Empty))
                        {
                            lblMessageImg.Text = "";
                            WFPDFViewer.Visible = true;
                            ShowImage(strImagePath);
                        }
                    }
                    else
                    {
                        lblMessageImg.Text = "No image is available.";
                        WFPDFViewer.Visible = false;
                    }
                }
                CheckFinalstatus();//Task WDMS-S-5146 A
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message.ToString();
            }
        }
        protected void CheckFinalstatus()
        {
            string Checkstatus;
            DBResult Objresult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            ObjDataEntry.WorkflowDataEntryFieldDataId = Convert.ToInt32(Session["FieldDataId"]);
            ObjDataEntry.LoginOrgId = loginUser.LoginOrgId;
            ObjDataEntry.LoginToken = loginUser.LoginToken;
            Objresult = ObjDataEntry.ManageWorkflowDataEntry(ObjDataEntry, "Checkfinalstatus");
            if (Objresult.dsResult.Tables[0].Rows.Count > 0)
            {
                Checkstatus = Convert.ToString(Objresult.dsResult.Tables[0].Rows[0]["Finalstatus"]);
                if (Checkstatus.ToLower() == "yes")
                {
                    DataEntryControls.Enabled = false;
                    btnSave.Enabled = false;
                    btnsubmitandMail.Visible = true;
                }
            }
        }

        protected void BindStatusDropDownList()
        {
            DBResult Objresult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            ObjDataEntry.WorkflowDataEntryProcessId = Convert.ToInt32(Session["ProcessId"]);
            ObjDataEntry.WorkflowDataEntryWorkflowId = Convert.ToInt32(Session["WorkflowId"]);
            ObjDataEntry.WorkflowDataEntryStageId = Convert.ToInt32(Session["StageId"]);
            ObjDataEntry.LoginOrgId = loginUser.LoginOrgId;
            ObjDataEntry.LoginToken = loginUser.LoginToken;
            Objresult = ObjDataEntry.ManageWorkflowDataEntry(ObjDataEntry, "BindStatusDropDown");
            DDLStatus.DataSource = Objresult.dsResult.Tables[0];
            DDLStatus.DataTextField = "WorkflowStageStatus_vName";
            DDLStatus.DataValueField = "WorkflowStageStatus_iId";
            DDLStatus.DataBind();

        }

        void ddlFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlParent = (DropDownList)sender;
            DropDownList ddlChild = null;
            CheckBoxList ddlchkChild = null;

            if ((hdnCurrentDDL.Value == "") || (ddlParent.ID.ToString() == hdnCurrentDDL.Value))
            {
                DBResult Objresult = new DBResult();
                if (Session["WorkflowControlDetails"] != null)
                {
                    Objresult = (DBResult)Session["WorkflowControlDetails"];
                }

                if (Objresult.dsResult != null && Objresult.dsResult.Tables.Count > 0 && Objresult.dsResult.Tables[1] != null && Objresult.dsResult.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow dr in Objresult.dsResult.Tables[1].Rows)
                    {
                        if (ddlParent != null)
                        {
                            if (dr["WorkflowStageFields_ControlID"].ToString() == ddlParent.ID.ToString())
                            {
                                if (dr["WorkflowStageFields_cObjType"].ToString() == "DropDown")
                                {
                                    if (this.ControlPlaceHolder_CurStage.FindControl("Control_" + dr["WorkflowStageFields_iChildId"].ToString()) != null) //if the control is not made inactive at a later stage
                                    {
                                        if (this.ControlPlaceHolder_CurStage.FindControl("Control_" + dr["WorkflowStageFields_iChildId"].ToString()).GetType().ToString() != "System.Web.UI.WebControls.CheckBoxList")
                                        {
                                            //modify the Parent row with necessary DropDownList type here..
                                            ddlChild = (DropDownList)this.ControlPlaceHolder_CurStage.FindControl("Control_" + dr["WorkflowStageFields_iChildId"].ToString());
                                            new ControlHelper().BindChildDropDownList(ddlChild, Convert.ToInt32(dr["WorkflowStageFields_iMasterType"].ToString()), Convert.ToInt32(ddlParent.SelectedValue == "" ? "0" : ddlParent.SelectedValue));
                                        }
                                        else
                                        {
                                            ddlchkChild = (CheckBoxList)this.ControlPlaceHolder_CurStage.FindControl("Control_" + dr["WorkflowStageFields_iChildId"].ToString());
                                            new ControlHelper().BindChildCheckBoxList(ddlchkChild, Convert.ToInt32(dr["WorkflowStageFields_iMasterType"].ToString()), Convert.ToInt32(ddlParent.SelectedValue == "" ? "0" : ddlParent.SelectedValue));
                                        }
                                    }
                                }
                            }
                        }
                    }

                    foreach (DataRow dr in Objresult.dsResult.Tables[2].Rows)
                    {
                        if (ddlParent != null)
                        {
                            if (dr["WorkflowStageFields_ControlID"].ToString() == ddlParent.ID.ToString())
                            {
                                if (dr["WorkflowStageFields_cObjType"].ToString() == "DropDown")
                                {
                                    if (this.ControlPlaceHolder_PrevStage.FindControl("Control_" + dr["WorkflowStageFields_iChildId"].ToString()) != null) //if the control is not made inactive at a later stage
                                    {
                                        if (this.ControlPlaceHolder_PrevStage.FindControl("Control_" + dr["WorkflowStageFields_iChildId"].ToString()).GetType().ToString() != "System.Web.UI.WebControls.CheckBoxList")
                                        {
                                            //modify the Parent row with necessary DropDownList type here..
                                            ddlChild = (DropDownList)this.ControlPlaceHolder_PrevStage.FindControl("Control_" + dr["WorkflowStageFields_iChildId"].ToString());
                                            new ControlHelper().BindChildDropDownList(ddlChild, Convert.ToInt32(dr["WorkflowStageFields_iMasterType"].ToString()), Convert.ToInt32(ddlParent.SelectedValue == "" ? "0" : ddlParent.SelectedValue));
                                        }
                                        else
                                        {
                                            ddlchkChild = (CheckBoxList)this.ControlPlaceHolder_PrevStage.FindControl("Control_" + dr["WorkflowStageFields_iChildId"].ToString());
                                            new ControlHelper().BindChildCheckBoxList(ddlchkChild, Convert.ToInt32(dr["WorkflowStageFields_iMasterType"].ToString()), Convert.ToInt32(ddlParent.SelectedValue == "" ? "0" : ddlParent.SelectedValue));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (ddlParent.ID.ToString() == hdnCurrentDDL.Value)
                {
                    ScriptManager.RegisterStartupScript(this, typeof(Page), "focuscontrol", "document.getElementById('" + ddlParent.ClientID.ToString() + "').focus();", true);
                }
            }
            if (strImagePath != string.Empty)
            {
                ShowImage(strImagePath);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Session.Remove("WorkflowControlDetails");
            Response.Redirect("~/Workflow/WorkflowAdmin/WorkflowPendingList.aspx", false);
        }

        private void ShowImage(string strFilePath)
        {
            Session["strPDFPath"] = strFilePath;
            LoadImageToViewer(string.Empty);
        }

        /// <summary>
        /// Unzip the file
        /// </summary>
        /// <param name="FilePath"> File full path of zipped file</param>
        public void Unzip(string FilePath)
        {
            try
            {
                // Extract the file to this location
                string ExtractFileLocation = FilePath.Replace(Path.GetFileName(FilePath), string.Empty);
                string FileName = Path.GetFileName(FilePath);

                Logger.Trace("unzip started", Session["LoggedUserId"].ToString());

                using (ZipFile zip = ZipFile.Read(FilePath))
                {
                    zip.ExtractAll(ExtractFileLocation, ExtractExistingFileAction.DoNotOverwrite);
                }
                Logger.Trace("unzip finished", Session["LoggedUserId"].ToString());
            }
            catch (Exception)
            {
                Logger.Trace("Error in unzipping", Session["LoggedUserId"].ToString());
            }
        }

        #region viewer helper methods

        protected void btnCallFromJavascript_Click(object sender, EventArgs e)
        {
            LoadImageToViewer(hdnAction.Value);
        }
        public static void copyDirectory(string Src, string Dst)
        {
            String[] Files;
            if (Dst[Dst.Length - 1] != Path.DirectorySeparatorChar)
                Dst += Path.DirectorySeparatorChar;
            if (!Directory.Exists(Dst)) Directory.CreateDirectory(Dst);
            Files = Directory.GetFileSystemEntries(Src, "*.pdf");
            foreach (string Element in Files)
            {
                try
                {
                    File.Copy(Element, Dst + Path.GetFileName(Element), true);
                }
                catch
                {
                    continue;

                }
            }
        }


        private void LoadImageToViewer(string Action)
        {
            string TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"]; // DMSENH6-5158 A
            string PdfFullPath = Session["strPDFPath"].ToString();
            string strPDFFolder = beforedot(PdfFullPath);
            string fileExtension = afterdot(PdfFullPath);
            string currentTempPath = TempFolder + Guid.NewGuid().ToString() + "\\";  // DMSENH6-5158 A
    
            hdnPagesCount.Value = System.IO.Directory.GetFiles(strPDFFolder, "*" + ".pdf").Length.ToString();

            int pagecount = Convert.ToInt32(hdnPagesCount.Value);
            string src = string.Empty;

            int CurrentPage = 1;

            if (hdnPageNo.Value.Length > 0)
                CurrentPage = Convert.ToInt32(hdnPageNo.Value);

            Logger.Trace("Setting image to viewer. Action: " + hdnAction.Value.ToString() + " Page count: " + pagecount + " Current Page:" + CurrentPage, Session["LoggedUserId"].ToString());

            int PageNo = 0;

            switch (Action.ToUpper())
            {
                case "NEXT":
                    PageNo = CurrentPage + 1;
                    break;
                case "PREVIOUS":
                    PageNo = CurrentPage - 1;
                    break;
                case "GOTO":
                    //PageNo = Convert.ToInt32(ddlpagecount.SelectedValue);
                    break;
                case "FIRST":
                    PageNo = 1;
                    break;
                case "LAST":
                    PageNo = pagecount;
                    break;
                default:
                    PageNo = CurrentPage;
                    break;
            }

            if (PageNo > 0 && PageNo <= pagecount)
            {
                hdnPageNo.Value = PageNo.ToString();
                //WFPDFViewer.ddlpagecount.SelectedValue = PageNo.ToString();
            }
            else
                PageNo = CurrentPage;

            string PdfPath = strPDFFolder + "\\" + PageNo.ToString() + ".pdf";
            // if document path is network path use only '\\' in the beginning. for ex \\171.12.1.22\documents
            PdfPath = PdfPath.Replace("\\\\", "$").Replace("$", @"\");
            Logger.Trace("source file path " + PdfPath, Session["LoggedUserId"].ToString());

            strPDFFolder = strPDFFolder.Replace("\\\\", "$").Replace("$", @"\");
            Logger.Trace("source file path " + strPDFFolder, Session["LoggedUserId"].ToString());

            Logger.Trace("Converting PDF to JPEG.", Session["LoggedUserId"].ToString());
            // Convert pdf to JPEG 
            string CurrentFilepath = new ConvertPdf2Image().ConvertPDFtoHojas(PdfPath, currentTempPath, PageNo.ToString());  // DMSENH6-5158 M

            if (CurrentFilepath.Length > 0)
            {
                src = GetSrc("Handler") + CurrentFilepath;
                src = src.Replace("\\", "//").ToLower();

                Logger.Trace("Setting image to viewer using RegisterStartupScript: " + src, Session["LoggedUserId"].ToString());
                ScriptManager.RegisterStartupScript(this, this.GetType(), "loadImageAndAnnotations", "loadImageToViewer('" + src + "');", true);

                //PDFViewer1.Load += new EventHandler(PDFViewer1.JustPostbakUserControl);
            }
            else
            {
                Logger.Trace("convertPDFToImage method returned file path as empty.", Session["LoggedUserId"].ToString());
            }
        }

        #endregion

        #region methods related to dynamic controls

        private Control FindControlRecursive(Control root, string id)
        {
            if (root.ID == id)
            {
                return root;
            }

            foreach (Control c in root.Controls)
            {
                Control t = FindControlRecursive(c, id);
                if (t != null)
                {
                    return t;
                }
            }

            return null;
        }

        private string GenerateFieldSaveQuery()
        {
            DBResult controlDataset;
            controlDataset = (DBResult)Session["WorkflowControlDetails"];
            DataTable dtCurFields = controlDataset.dsResult.Tables[1];
            DataTable dtPrevFields = controlDataset.dsResult.Tables[2];

            updateQuery = string.Empty;

            //Get update query for current stage fields
            BuildQuery(dtCurFields, ControlPlaceHolder_CurStage);

            //Get update query for previous stage fields
            BuildQuery(dtPrevFields, ControlPlaceHolder_PrevStage);

            updateQuery += "WorkflowStageFieldData_iStatusID =" + DDLStatus.SelectedValue + ",WorkflowStageFieldData_iProcessID =" + Convert.ToInt32(Session["ProcessId"]) +
                              ",WorkflowStageFieldData_iWorkFlowID =" + Convert.ToInt32(Session["WorkflowId"]) +
                " WHERE WorkflowStageFieldData_iID =" + Session["FieldDataId"].ToString();

            return updateQuery;
        }

        protected void btnSave_Clcik(object sender, EventArgs e)
        {
            try
            {
                Logger.Trace("btnSave_Click Started", Session["LoggedUserId"].ToString());
                CheckAuthentication();

                DBResult Objresult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                ObjDataEntry.WorkflowDataEntryProcessId = Convert.ToInt32(Session["ProcessId"]);
                ObjDataEntry.WorkflowDataEntryWorkflowId = Convert.ToInt32(Session["WorkflowId"]);
                ObjDataEntry.WorkflowDataEntryStageId = Convert.ToInt32(Session["StageId"]);
                ObjDataEntry.LoginOrgId = loginUser.LoginOrgId;
                ObjDataEntry.LoginToken = loginUser.LoginToken;
                ObjDataEntry.WorkflowDataEntryFieldDataId = Convert.ToInt32(Session["FieldDataId"]);

                //Copy of files from temp folder to original folder
                if (Directory.Exists(GetTemporaryFolderPath()))
                    ManageFiles.CopyFiles(GetTemporaryFolderPath(), GetOriginalFolderPath());
                ObjDataEntry.WorkflowDataEntryXmlData = GenerateFieldSaveQuery();

                //Validate the mandatory controls have the value
                if (hdnSaveStatus.Value.Equals("Save"))
                {
                    Objresult = ObjDataEntry.ManageWorkflowDataEntry(ObjDataEntry, "SaveFieldData");


                    if (Objresult.ErrorState.Equals(0))
                    {
                        // Clear temporary folder
                        new ManageDirectory().DeleteDirectory(GetTemporaryFolderPath(false));
                    }

                    hdnSaveStatus.Value = "";
                    Session.Remove("WorkflowControlDetails");

                    //DMS5-4388 BS

                    //Reading process,workflow,feild,dataentry mode from database
                    if (Objresult.dsResult.Tables.Count > 0 && Objresult.dsResult.Tables[0].Rows.Count > 0)
                    {
                        DataRow dataRow = Objresult.dsResult.Tables[0].Rows[0];

                        string processId = HttpUtility.UrlEncode(Encrypt(Convert.ToString(dataRow["ProcessId"])));
                        string workflowId = HttpUtility.UrlEncode(Encrypt(Convert.ToString(dataRow["WorkflowId"])));
                        string stageId = HttpUtility.UrlEncode(Encrypt(Convert.ToString(dataRow["StageId"])));
                        string feildDataId = HttpUtility.UrlEncode(Encrypt(Convert.ToString(dataRow["FeildId"])));
                        string dataEntryMode = HttpUtility.UrlEncode(Encrypt(Convert.ToString(dataRow["Data Entry Mode"])));

                        string URL = "?ProcessId=" + processId + "&WorkflowId=" + workflowId + "&StageId=" + stageId + "&FieldDataId=" + feildDataId + "&DataEntryType=" + dataEntryMode;

                        dataEntryMode = Convert.ToString(dataRow["Data Entry Mode"]);

                        if (dataEntryMode == "Normal")
                        {
                            Response.Redirect("~/Workflow/WorkflowAdmin/WorkflowDataEntry.aspx" + URL, false);
                        }
                        else if (dataEntryMode == "Guided")
                        {
                            Response.Redirect("~/Workflow/WorkflowAdmin/ManageGuidedDataEntry.aspx" + URL, false);
                        }
                        else if (dataEntryMode == "Form")
                        {
                            Response.Redirect("~/Workflow/WorkflowAdmin/ManageFormDataEntry.aspx" + URL, false);
                        }


                    }
                }
                //DMS5-4388 BE
                Logger.Trace("btnSave_Click Finished", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message.ToString();
                Logger.Trace("btnSave_Click has Error Check GenerateFieldSaveQuery()", Session["LoggedUserId"].ToString());
            }
        }

        private string GenerateControlData(DataTable dt)
        {
            string strControlData = string.Empty;

            foreach (DataRow dr in dt.Rows)
            {
                if (strControlData != "")
                {
                    strControlData += "#$#";
                }
                strControlData += dr["WorkflowStageFields_iId"].ToString() + "#|#";
                strControlData += dr["WorkflowStageFields_iProcessId"].ToString() + "#|#";
                strControlData += dr["WorkflowStageFields_iWorkflowId"].ToString() + "#|#";
                strControlData += dr["WorkflowStageFields_iStageId"].ToString() + "#|#";
                strControlData += dr["WorkflowStageFields_iOrgId"].ToString() + "#|#";
                strControlData += dr["WorkflowStageFields_vName"].ToString() + "#|#";
                strControlData += dr["WorkflowStageFields_vTable"].ToString() + "#|#";
                strControlData += dr["WorkflowStageFields_vDBFld"].ToString() + "#|#";
                strControlData += dr["WorkflowStageFields_vDBType"].ToString() + "#|#";
                strControlData += dr["WorkflowStageFields_cObjType"].ToString() + "#|#";
                strControlData += dr["WorkflowStageFields_iParentId"].ToString() + "#|#";
                strControlData += dr["WorkflowStageFields_iChildId"].ToString() + "#|#";
                strControlData += dr["WorkflowStageFields_vLabelText"].ToString() + "#|#";
                strControlData += dr["WorkflowStageFields_bActive"].ToString() + "#|#";
                strControlData += dr["WorkflowStageFields_bMandatory"].ToString() + "#|#";
                strControlData += dr["WorkflowStageFields_bDisplay"].ToString() + "#|#";
                strControlData += dr["WorkflowStageFields_bPostback"].ToString() + "#|#";
                strControlData += dr["WorkflowStageFields_iSortOrder"].ToString() + "#|#";
                strControlData += dr["WorkflowStageFields_iMasterType"].ToString() + "#|#";
                strControlData += dr["WorkflowStageFields_ControlID"].ToString();
            }

            return strControlData;
        }

        protected string BindControls()
        {
            string strImagePathLocal = string.Empty;

            //Disable the whole controls if final status is yes
            // DMSENH6-4640 BS
            string isFinalStatus = string.Empty;
            if (Request.QueryString["IsFinalStatus"] != null)// DMSENH6-4716
            {
                isFinalStatus = Decrypt(HttpUtility.UrlDecode(Request.QueryString["IsFinalStatus"]));
            }
            if (isFinalStatus.Equals("Yes"))
            {
                DataEntryControls.Enabled = false;
                btnSave.Enabled = false;
                btnsubmitandMail.Visible = true; /*DMSENH6-4776 A*/
            }
            else
            {
                btnSave.Enabled = true;
                btnsubmitandMail.Visible = false; /*DMSENH6-4776 A*/
            }

            // DMSENH6-4640 BE
            try
            {
                ControlPlaceHolder_CurStage.Controls.Clear();
                ControlPlaceHolder_PrevStage.Controls.Clear();
                ControlPlaceHolder_Information.Controls.Clear();
                DBResult Objresult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];

                if ((Session["WorkflowControlDetails"] != null) && (Session["WorkflowControlDetails"].ToString() != ""))
                {
                    Objresult = (DBResult)Session["WorkflowControlDetails"];
                }
                else
                {
                    ObjDataEntry.WorkflowDataEntryProcessId = Convert.ToInt32(Session["ProcessId"]);
                    ObjDataEntry.WorkflowDataEntryWorkflowId = Convert.ToInt32(Session["WorkflowId"]);
                    ObjDataEntry.WorkflowDataEntryStageId = Convert.ToInt32(Session["StageId"]);
                    ObjDataEntry.LoginOrgId = loginUser.LoginOrgId;
                    ObjDataEntry.LoginToken = loginUser.LoginToken;
                    ObjDataEntry.WorkflowDataEntryFieldDataId = Convert.ToInt32(Session["FieldDataId"]);
                    Objresult = ObjDataEntry.ManageWorkflowDataEntry(ObjDataEntry, "GetWorkflowControlDetailsEDIT");



                    //error
                    if (Objresult.ErrorState != 0)
                    {
                        if (Objresult.Message != null)
                        {
                            lblMessage.Text = Objresult.Message.ToString();
                            btnSave.Enabled = false;
                        }
                    }
                }

                if (Objresult.dsResult != null && Objresult.dsResult.Tables.Count > 0 && Objresult.dsResult.Tables[0].Rows.Count > 0)
                {
                    Session["WorkflowControlDetails"] = Objresult; //for later use

                    DataRow Row = Objresult.dsResult.Tables[0].Rows[0];
                    lblProcessDescription.Text = Row["WorkflowProcess_vName"].ToString();
                    lblWorkflowDescription.Text = Row["Workflow_vName"].ToString();
                    lblStageDescription.Text = Row["WorkflowStage_vDisplayName"].ToString();
                    lblWorkItemID.Text = Convert.ToString(Session["FieldDataId"]); //DMS5-4398

                    lblUserDescription.Text = loginUser.UserName;
                    lblDateofReceiptDescription.Text = Convert.ToDateTime(Objresult.dsResult.Tables[3].Rows[0]["WorkflowStageFieldData_dModifiedOn"].ToString()).ToString("dd-MMM-yyyy hh:mm tt");
                }
                else
                {
                    ControlPlaceHolder_CurStage.Visible = false;
                    ControlPlaceHolder_Information.Visible = false;
                }
                //create and fill cotrols - prev stage
                if (Objresult.dsResult != null && Objresult.dsResult.Tables.Count > 0 && Objresult.dsResult.Tables[2].Rows.Count > 0)
                {
                    //Previous stage control binding
                    RenderControls(Objresult.dsResult.Tables[2], Objresult.dsResult.Tables[3], ControlPlaceHolder_PrevStage);
                }
                else
                {
                    table_PrevStage.Visible = false;
                }
                //create and fill cotrols - current stage
                if (Objresult.dsResult != null && Objresult.dsResult.Tables.Count > 0 && Objresult.dsResult.Tables[1].Rows.Count > 0)
                {
                    RenderControls(Objresult.dsResult.Tables[1], Objresult.dsResult.Tables[3], ControlPlaceHolder_CurStage);
                }
                else
                {
                    table_curStage.Visible = false;
                }

                //  DMS5-4423 BS
                //create and fill cotrols - information
                if (Objresult.dsResult != null && Objresult.dsResult.Tables.Count > 0 && Objresult.dsResult.Tables[4].Rows.Count > 0)
                {
                    //information control binding
                    RenderControls(Objresult.dsResult.Tables[4], Objresult.dsResult.Tables[3], ControlPlaceHolder_Information);
                }
                else
                {
                    table_Information.Visible = false;
                }
                //  DMS5-4423 BE


                if (Objresult.dsResult != null && Objresult.dsResult.Tables.Count > 0 && Objresult.dsResult.Tables[0].Rows.Count > 0)
                {

                    btnSave.Attributes.Add("onclick", "return ValidateMandatoryData('" + mandatoryValidationScript + "','" + specialCharValidationScript + "','" + rangeValidationScript + "');");
                }
                else
                {
                    btnSave.Enabled = false;
                }

                btnCancel.Attributes.Add("onclick", "return SetAction('Cancel');");
                if (Objresult.dsResult != null && Objresult.dsResult.Tables.Count > 0 && Objresult.dsResult.Tables[3].Rows.Count > 0)
                {
                    strImagePathLocal = Objresult.dsResult.Tables[3].Rows[0]["WorkflowStageFieldData_vDocPhysicalPath"].ToString();
                }

                if (hdnSaveStatus.Value == "")
                {
                    try
                    {
                        //DMS5-4332
                        if (!IsPostBack)
                            DDLStatus.SelectedValue = Objresult.dsResult.Tables[3].Rows[0]["WorkflowStageFieldData_iStatusID"].ToString();
                    }
                    catch
                    {
                        DDLStatus.SelectedValue = "0";
                    }
                }

                //select values in the created fields - current stage
                if (Objresult.dsResult != null && Objresult.dsResult.Tables.Count > 0 && Objresult.dsResult.Tables[1] != null && Objresult.dsResult.Tables[1].Rows.Count > 0)
                {
                    InitializeAfterRender(Objresult.dsResult.Tables[1], Objresult.dsResult.Tables[3]);
                }

                //select values in the created fields - prev stage
                if (Objresult.dsResult != null && Objresult.dsResult.Tables.Count > 0 && Objresult.dsResult.Tables[2] != null && Objresult.dsResult.Tables[2].Rows.Count > 0)
                {
                    InitializeAfterRender(Objresult.dsResult.Tables[2], Objresult.dsResult.Tables[3]);
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message.ToString();
            }
            return strImagePathLocal;
        }

        //  DMS5-4423 BS
        protected void btnShowHistory_Click(object sender, EventArgs e)
        {
            try
            {
                BindHistoryDetails();
            }
            catch (Exception ex)
            {
                Logger.Trace("history_Click finished WorkFlowDataEnrtry.cs " + ex, Session["LoggedUserId"].ToString());
            }

        }

        protected void GridHistory_Paging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                GridHistory.PageIndex = e.NewPageIndex;
                BindHistoryDetails();
            }
            catch (Exception ex)
            {

            }
        }

        //to load history gridview
        protected void BindHistoryDetails()
        {

            Logger.Trace("history_Click started WorkFlowDataEnrtry.cs ", Session["LoggedUserId"].ToString());
            DBResult Objresult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            ObjDataEntry.WorkflowDataEntryProcessId = Convert.ToInt32(Session["ProcessId"]);
            ObjDataEntry.WorkflowDataEntryWorkflowId = Convert.ToInt32(Session["WorkflowId"]);
            ObjDataEntry.WorkflowDataEntryStageId = Convert.ToInt32(Session["StageId"]);
            ObjDataEntry.LoginOrgId = loginUser.LoginOrgId;
            ObjDataEntry.LoginToken = loginUser.LoginToken;
            ObjDataEntry.WorkflowDataEntryFieldDataId = Convert.ToInt32(Session["FieldDataId"]);
            Objresult = ObjDataEntry.ManageWorkflowDataEntry(ObjDataEntry, "GetHistoryDetails");

            if (Objresult.dsResult.Tables.Count > 0 && Objresult.dsResult.Tables[0].Rows.Count > 0)
            {
                GridHistory.DataSource = Objresult.dsResult.Tables[0];
                GridHistory.DataBind();
                ModalPopup.Show();
            }

            else
            {
                GridHistory.DataSource = null;
                GridHistory.DataBind();
                ModalPopup.Show();
            }
            LoadImageToViewer(string.Empty);//Task WDMS-S-5156 A
        }
        //  DMS5-4423 BE
        protected void AsyncFileUpload1_UploadedComplete(object sender, AsyncFileUploadEventArgs e)
        {
            string TempFolder = ConfigurationManager.AppSettings["TempWorkFolder"];
            string fileName = string.Empty;
            string savePath = string.Empty;
            string afterDot = string.Empty;
            string dataId = string.Empty;
            try
            {
                Logger.Trace("AsyncFileUpload1_UploadedComplete started WorkFlowDataEnrtry.cs ", Session["LoggedUserId"].ToString());

                dataId = Session["FieldDataId"] as string;

                TempFolder = GetTemporaryFolderPath();

                if (Directory.Exists(TempFolder))
                {
                    new ManageDirectory().DeleteDirectory(TempFolder);
                }

                new ManageDirectory().CreateDirectory(TempFolder);


                if (e.FileSize.Length != 0)
                {
                    lblMessage.Text = string.Empty;
                    afterDot = afterdot(e.FileName);
                    switch (afterDot.ToLower())
                    {
                        case ".doc":
                        case ".docx":
                            fileName = e.FileName.Replace(e.FileName, String.Concat(dataId, afterDot));
                            Logger.Trace("AsyncFileUpload1_UploadedComplete (Workflowdataentry) of MSWord ", Convert.ToString(LoggedUserId));
                            break;
                        case ".ppt":
                        case ".pptx":
                            fileName = e.FileName.Replace(e.FileName, String.Concat(dataId, afterDot));
                            Logger.Trace("AsyncFileUpload1_UploadedComplete (Workflowdataentry) of MSPowerPoint ", Convert.ToString(LoggedUserId));
                            break;
                        case ".xls":
                        case ".xlsx":
                            fileName = e.FileName.Replace(e.FileName, String.Concat(dataId, afterDot));
                            Logger.Trace("AsyncFileUpload1_UploadedComplete (Workflowdataentry) of MSExcel ", Convert.ToString(LoggedUserId));
                            break;
                        case ".tif":
                        case ".tiff":


                        case ".jpg":
                        case ".bmp":
                        case ".jpeg":
                        case ".png":
                        case ".gif":
                        case ".giff":
                            fileName = e.FileName.Replace(e.FileName, String.Concat(dataId, afterDot));
                            Logger.Trace("AsyncFileUpload1_UploadedComplete (Workflowdataentry) of Image2Pdf().tiff2PDF ", Convert.ToString(LoggedUserId));
                            break;
                        case ".pdf":
                            fileName = e.FileName.Replace(e.FileName, String.Concat(dataId, afterDot));
                            Logger.Trace("AsyncFileUpload1_UploadedComplete (Workflowdataentry) of Image2Pdf().ExtractPages ", Convert.ToString(LoggedUserId));
                            break;
                        default:
                            Response.Write("<script>alert('Kindly upload files with .doc,.docx,.ppt,.jpg,.bmp,.jpeg,.png,.gif,.giff,.tif,.tiff,.xlsx,.xls,.ppt,.pptx extensions');</script>");
                            break;
                    }
                    try
                    {
                        if (fileName.Length > 0)
                        {
                            savePath = TempFolder + Path.GetFileName(fileName);
                            ((AsyncFileUpload)sender).SaveAs(savePath);

                            #region Check duplicate and insert into datatable property (UploadedFiles)
                            // Columns: ControlId, FilePath
                            AsyncFileUpload asyncFileUpload = sender as AsyncFileUpload;
                            DataTable dt = UploadedFiles;

                            string find = "ControlId = '" + asyncFileUpload.ID + "'";
                            DataRow[] foundRows = UploadedFiles.Select(find);

                            if (foundRows.Length > 0)
                            {
                                foreach (DataRow DR in dt.Rows)
                                {
                                    if (DR["ControlId"].ToString() == foundRows[0]["ControlId"].ToString())
                                    {
                                        dt.Rows.Remove(DR);
                                    }
                                    break;
                                }
                                dt.AcceptChanges();
                            }
                            dt.Rows.Add(asyncFileUpload.ID, savePath);
                            UploadedFiles = dt;
                            #endregion
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Logger.Trace("Auto Formula  calculation finished WorkFlowDataEnrtry.cs " + ex, Session["LoggedUserId"].ToString());
            }
        }

        //to perform formula calculation on dynamic textbox change
        protected void txt_formulaCalculation(object sender, EventArgs e)
        {
            try
            {
                Logger.Trace("Auto Formula  calculation started WorkFlowDataEnrtry.cs ", Session["LoggedUserId"].ToString());

                TextBox referredField = sender as TextBox;

                // FormulaFieldData columns:  ReferredFieldId, ReferredFieldName, ReferredFieldIdList, FormulaFieldId, Formula
                DataView dvFormulaFieldData = new DataView(FormulaFieldData, "ReferredFieldId = '" + referredField.ID + "'", string.Empty, DataViewRowState.CurrentRows);

                string originalFormula = string.Empty;
                string formulaFieldId = string.Empty;
                List<string> formulaFeilds = new List<string>();
                string txtValue = string.Empty;
                string referredFieldIdList = string.Empty;

                // Loop through formula fields those are referring this (sender) field
                foreach (DataRow dr in dvFormulaFieldData.ToTable().Rows)
                {
                    originalFormula = Convert.ToString(dr["Formula"]);
                    formulaFieldId = Convert.ToString(dr["FormulaFieldId"]);
                    referredFieldIdList = Convert.ToString(dr["ReferredFieldIdList"]);

                    TextBox formulaField = FindControlRecursive(Page, formulaFieldId) as TextBox;

                    //Generate field names list and sort by text length (sorting for replace purpose)
                    formulaFeilds = SplitInputFormula(originalFormula).ToList();
                    IEnumerable<string> query = from word in formulaFeilds
                                                orderby word.Length descending
                                                select word;
                    formulaFeilds = query.ToList();

                    // Loop through field names list; replace field name with its value
                    // say (Num1)+(Num2) => (15)+(10)
                    foreach (string fieldName in formulaFeilds)
                    {
                        // Find field id by field name
                        string fieldId = new DataView(FormulaFieldData, "ReferredFieldName = '" + fieldName + "'", string.Empty, DataViewRowState.CurrentRows)[0]["ReferredFieldId"].ToString();

                        TextBox txtControl = FindControlRecursive(Page, fieldId) as TextBox;
                        if (txtControl != null)
                        {
                            txtValue = txtControl.Text.Trim();
                            if (txtValue.Trim().Length.Equals(0))
                                txtValue = "0";
                            originalFormula = originalFormula.Replace(fieldName, txtValue);
                        }
                    }

                    formulaField.Text = Convert.ToString(EvaluateExpression(originalFormula.Trim()));
                }

                Logger.Trace("Auto Formula  calculation finished WorkFlowDataEnrtry.cs ", Session["LoggedUserId"].ToString());
            }

            catch (Exception ex)
            {
                Logger.Trace("Auto Formula   WorkFlowDataEnrtry.cs , Error " + ex, Session["LoggedUserId"].ToString());
            }
        }

        protected List<string> SplitInputFormula(string input)
        {
            string[] Output = input.Split("/*-+()".ToCharArray());
            List<string> list = new List<string>(Output);
            //To remove empty values in string arrays
            list = list.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();

            return list;
        }

        //To perform Calculation of formula
        private object EvaluateExpression(string eqn)
        {
            var result = (object)null;
            try
            {
                DataTable dt = new DataTable();
                result = dt.Compute(eqn, string.Empty);

            }
            catch (SyntaxErrorException syntaxerror)
            {

            }


            return result;
        }

        //Delete file from folder on delete button click
        protected void btnFileDelete_Click(object sender, EventArgs e)
        {
            Logger.Trace("btnFileDelete_Click Started", Session["LoggedUserId"].ToString());
            try
            {
                LinkButton asyncFileUpload = sender as LinkButton;

                //Check wheather deletin is happening in edit mode or normal save mode
                //if in edit mode delete it from original folder

            }
            catch (DirectoryNotFoundException DirectoryNotfound)
            {
                Logger.Trace("btnFileDelete_Click has ,error =" + DirectoryNotfound, Session["LoggedUserId"].ToString());
            }
        }

        protected string GetOriginalFolderPath()
        {
            string folderPath = ConfigurationManager.AppSettings["WorkFlowDocumentsPath"];
            string processId, workflowId, stageId;
            processId = Session["ProcessId"] as string;
            workflowId = Session["WorkflowId"] as string;
            stageId = Session["StageId"] as string;

            folderPath = folderPath + "\\" + processId + "\\" + workflowId + "\\" + stageId + "\\";
            return folderPath;
        }

        /// <summary>
        /// Returns temporary folder path with id's (token, process, workflow, stage) hierarchy.
        /// </summary>
        /// <param name="includeHierarchy">Default is 'true'. If 'false', returns path only upto token.</param>
        /// <returns></returns>
        protected string GetTemporaryFolderPath(bool includeHierarchy = true)
        {
            string folderPath = ConfigurationManager.AppSettings["TempWorkFolder"];
            string processId, workflowId, stageId;
            UserBase loginUser = (UserBase)Session["LoggedUser"];

            processId = Session["ProcessId"] as string;
            workflowId = Session["WorkflowId"] as string;
            stageId = Session["StageId"] as string;

            if (includeHierarchy)
                folderPath = folderPath + "\\" + loginUser.LoginToken + "\\" + processId + "\\" + workflowId + "\\" + stageId + "\\";
            else
                folderPath = folderPath + "\\" + loginUser.LoginToken;

            return folderPath;
        }

        protected Table RenderControls(DataTable controlsConfig, DataTable controlsData, PlaceHolder placeholder)
        {
            Table controlContainer = new Table();

            try
            {
                //Set data row
                DataRow dataRow = controlsData.Rows[0];

                placeholder.Controls.Clear();

                #region create and fill controls - current stage
                if (controlsConfig.Rows.Count > 0)
                {

                    foreach (DataRow dr in controlsConfig.Rows)
                    {
                        #region variable declaration and initialization
                        string objType = dr["WorkflowStageFields_cObjType"].ToString();
                        string controlID = dr["WorkflowStageFields_ControlID"].ToString();
                        string labelText = dr["WorkflowStageFields_vLabelText"].ToString();

                        int parentId = Convert.ToInt32(dr["WorkflowStageFields_iParentId"]);

                        int masterTypeId = Convert.ToInt32(dr["WorkflowStageFields_iMasterType"]);
                        string dbField = dr["WorkflowStageFields_vDBFld"].ToString();

                        bool mandatory = Convert.ToBoolean(dr["WorkflowStageFields_bMandatory"]);
                        string dbType = dr["WorkflowStageFields_vDBType"].ToString();
                        // DMS5-4331  
                        bool fieldEditable = Convert.ToBoolean(dr["WorkflowStageFields_bEditable"]);
                        string formula = dr["WorkflowStageFields_vFormula"].ToString();
                        string referredFieldIds = dr["WorkflowStageFields_vReferedFieldIds"].ToString();
                        bool remarks = Convert.ToBoolean(dr["WorkflowStageFields_bRemarks"]);

                        string fieldValue = Convert.ToString(dataRow[dbField]);

                        string labelIdSuffix = "_Label";
                        #endregion

                        Logger.Trace("Rendering controls. Object Type: " + objType + ", Control Id: " + controlID, Convert.ToString(LoggedUserId));

                        TableRow tableRow = new TableRow();
                        TableCell tableCell1 = new TableCell();
                        TableCell tableCell2 = new TableCell();
                        TableCell tableCell3 = new TableCell();
                        TableCell tableCell4 = new TableCell();

                        tableCell1.Width = Unit.Pixel(130);
                        tableCell2.Width = Unit.Pixel(150);
                        tableCell4.Width = Unit.Pixel(50);
                        tableCell1.Attributes.Add("valign", "top");


                        #region Dropdown

                        if (objType.Equals(Convert.ToString(FieldTypes.DropDown)))
                        {
                            Label label = new Label();
                            label.ID = controlID + labelIdSuffix;
                            label.Text = labelText;
                            tableCell1.Controls.Add(label);

                            DropDownList ddl = new DropDownList();
                            ddl.ID = controlID;
                            tableCell2.Controls.Add(ddl);

                            //Disable controls if fieldEditable is set to false
                            // DMS5-4331  
                            ddl.Enabled = fieldEditable;


                            //if (parentId.Equals(0))
                            //{
                            new ControlHelper().BindDropDownList(ddl, masterTypeId);
                            //}

                            try
                            {
                                ddl.SelectedValue = fieldValue;
                            }
                            catch
                            {
                                ddl.SelectedValue = "0";
                            }
                        }
                        #endregion

                        #region Multidropdown
                        else if (objType.Equals(Convert.ToString(FieldTypes.MultiDropDown)))
                        {
                            Label label = new Label();
                            label.ID = controlID + labelIdSuffix;
                            label.Text = labelText;
                            tableCell1.Controls.Add(label);

                            CheckBoxList ddl = new CheckBoxList();
                            ddl.ID = controlID;

                            //Disable controls if fieldEditable is set to false
                            // DMS5-4331  
                            ddl.Enabled = fieldEditable;

                            Panel pnl = new Panel();
                            pnl.ID = controlID + "_Panel";
                            pnl.Style.Add("border", "1px solid #bdbcbd");
                            pnl.Style.Add("height", "100px");
                            pnl.Style.Add("width", "198px");
                            pnl.Style.Add("overflow-y", "scroll");

                            pnl.Controls.Add(ddl);
                            tableCell2.Controls.Add(pnl);

                            //if (parentId.Equals(0))
                            //{
                            new ControlHelper().BindCheckBoxList(ddl, masterTypeId);
                            //}

                            try
                            {
                                string strMultiValues = fieldValue;
                                foreach (ListItem li in ddl.Items)
                                {
                                    if (strMultiValues.Split(',').Contains(li.Value.ToString()))
                                    {
                                        li.Selected = true;
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }
                        #endregion

                        #region Textbox
                        else if (objType.Equals(Convert.ToString(FieldTypes.TextBox)))
                        {
                            Label label = new Label();
                            label.ID = controlID + labelIdSuffix;
                            label.Text = labelText;
                            tableCell1.Controls.Add(label);

                            TextBox textbox = new TextBox();
                            textbox.ID = controlID;
                            tableCell2.Controls.Add(textbox);
                            textbox.Text = fieldValue;

                            //Disable controls if fieldEditable is set to false
                            // DMS5-4331  
                            textbox.Enabled = fieldEditable;

                            int MinVal = 0;
                            int MaxVal = 0;
                            try
                            {
                                MinVal = Convert.ToInt32(dr["WorkflowStageFields_iMin"].ToString());
                                MaxVal = Convert.ToInt32(dr["WorkflowStageFields_iMax"].ToString());
                            }
                            catch
                            {
                            }


                            if (dbType.Equals("String"))
                            {
                                if (MaxVal > 0)
                                {
                                    if (rangeValidationScript != "")
                                    {
                                        rangeValidationScript += "##";
                                    }
                                    rangeValidationScript += textbox.ClientID.ToString() + "|" + objType + "|" + MinVal.ToString() + "|" + MaxVal.ToString();
                                }


                            }

                            else if (dbType.Equals("Number"))
                            {
                                textbox.Attributes.Add("onkeypress", "return onlyNumbers(event,this);");

                                if (MaxVal > 0)
                                {
                                    if (rangeValidationScript != "")
                                    {
                                        rangeValidationScript += "##";
                                    }
                                    rangeValidationScript += textbox.ClientID.ToString() + "|" + objType + "|" + MinVal.ToString() + "|" + MaxVal.ToString();
                                }
                            }
                            else if (dbType.Equals("Date"))
                            {
                                //Add Image button next to textbox for calender appearance
                                ImageButton imgButton = new ImageButton();
                                imgButton.ImageUrl = @"..\images\cal.gif";
                                imgButton.ID = controlID + "_imgButton";
                                tableCell3.Controls.Add(imgButton);

                                //AjaxCalender Extender if Created Fdild is Dob at add new stageFeilds
                                AjaxControlToolkit.CalendarExtender Cal = new AjaxControlToolkit.CalendarExtender();
                                Cal.ID = "Cal_" + textbox.ID;
                                Cal.Format = "dd/MM/yyyy";
                                textbox.Attributes.Add("readonly", "readonly");
                                Cal.TargetControlID = textbox.ID;
                                Cal.PopupButtonID = imgButton.ID;
                                tableCell2.Controls.Add(Cal);

                                // DMS5-4331  
                                //Disable controls if fieldEditable is set to false
                                imgButton.Enabled = fieldEditable;


                            }
                        }
                        #endregion

                        //DMS5-3424 BS
                        #region DateCalculation
                        else if (objType.Equals(Convert.ToString(FieldTypes.DateCalculation_CurrentDate))
                            || objType.Equals(Convert.ToString(FieldTypes.DateCalculation_ReferDate)))
                        {

                            try
                            {
                                //Label to diaplay the text of the field name
                                Label label = new Label();
                                label.ID = controlID + labelIdSuffix;
                                label.Text = labelText;
                                tableCell1.Controls.Add(label);

                                //Textbox to render current date
                                TextBox textbox = new TextBox();
                                textbox.ID = controlID;
                                textbox.Attributes.Add("readonly", "readonly");
                                tableCell2.Controls.Add(textbox);

                                //set value from database
                                //DMS5-4263
                                if (!IsPostBack)
                                    textbox.Text = fieldValue;

                                //Disable controls if fieldEditable is set to false
                                // DMS5-4331  
                                textbox.Enabled = fieldEditable;

                                //Add Image button next to textbox for calender appearance
                                ImageButton imgButton = new ImageButton();
                                imgButton.ImageUrl = @"..\images\cal.gif";
                                imgButton.ID = controlID + "_imgButton";
                                tableCell3.Controls.Add(imgButton);
                                //Disable controls if fieldEditable is set to false
                                // DMS5-4331  
                                imgButton.Enabled = fieldEditable;


                                //attach textbox with ajax calender event
                                AjaxControlToolkit.CalendarExtender AutoAge = new AjaxControlToolkit.CalendarExtender();
                                AutoAge.ID = "Cal_" + textbox.ID;
                                AutoAge.Format = "dd/MM/yyyy";
                                AutoAge.TargetControlID = textbox.ID;
                                AutoAge.PopupButtonID = imgButton.ID;
                                tableCell2.Controls.Add(AutoAge);

                                //Label to Display calculated date diffrence
                                Label lblDateDiffrence = new Label();
                                lblDateDiffrence.ID = controlID + "_LblCalculateAge";
                                tableCell2.Controls.Add(lblDateDiffrence);

                                if (objType.Equals(Convert.ToString(FieldTypes.DateCalculation_CurrentDate)))
                                {
                                    //Calling javascript to calculate date with refrence to current date
                                    textbox.Attributes.Add("onchange", "CalculateDifferenceInDate('" + textbox.ID + "', '" + DateTime.Today.ToString("dd/MM/yyyy") + "' ,'" + lblDateDiffrence.ID + "');");

                                    //Call Date Calculation method
                                    if (textbox.Text.Length > 0)
                                        lblDateDiffrence.Text = DateCalculation(textbox.Text, DateTime.Today.ToString("dd/MM/yyyy"));
                                }
                                else
                                {
                                    //DMS5-3473 BS
                                    #region DateCalculation-ReferDate(Refering to Date Feild) logic
                                    //Selecting a row which has matched refered date
                                    var matchingControlId = controlsConfig.Select("WorkflowStageFields_iId = '" + referredFieldIds + "'");
                                    if (matchingControlId.Length != 0)
                                    {
                                        //Getting control Id for the referred field
                                        string referredControlId = Convert.ToString(matchingControlId[0].ItemArray[19]);
                                        //invoke javascript to to do calculation
                                        textbox.Attributes.Add("onchange", "CalculateDifferenceInDate('" + referredControlId + "','" + textbox.ID + "' ,'" + lblDateDiffrence.ID + "');");

                                        // Find referred field and bind onchange event to calculate date difference 
                                        TextBox txtReferredField = FindControlRecursive(controlContainer, referredControlId) as TextBox;
                                        txtReferredField.Attributes.Add("onchange", "CalculateDifferenceInDate('" + txtReferredField.ID + "','" + textbox.ID + "' ,'" + lblDateDiffrence.ID + "');");

                                        //Call Date Calculation method
                                        if (textbox.Text.Length > 0 && txtReferredField.Text.Length > 0)
                                            lblDateDiffrence.Text = DateCalculation(txtReferredField.Text, textbox.Text);
                                    }
                                    #endregion
                                    //DMS5-3473 BE
                                }
                            }

                            catch (Exception ex)
                            {
                                Logger.Trace("Exception: " + ex.Message, Convert.ToString(LoggedUserId));
                            }
                        }

                        #endregion
                        //DMS5-3424 BE

                        // DMS5-3425 BS
                        #region Current Masking of textboxes logic
                        else if (objType.Equals(Convert.ToString(FieldTypes.DualDataEntry_MaskNone))
                            || objType.Equals(Convert.ToString(FieldTypes.DualDataEntry_MaskFirst))
                            || objType.Equals(Convert.ToString(FieldTypes.DualDataEntry_MaskSecond))
                            || objType.Equals(Convert.ToString(FieldTypes.DualDatEntry_MaskBoth)))
                        {
                            try
                            {
                                //Label to diaplay the text of the field name
                                Label label = new Label();
                                label.ID = controlID + labelIdSuffix;
                                label.Text = labelText;
                                tableCell1.Controls.Add(label);

                                TextBox textbox = new TextBox();
                                textbox.ID = dr["WorkflowStageFields_ControlID"].ToString();
                                textbox.Text = fieldValue;
                                tableCell2.Controls.Add(textbox);

                                //Disable controls if fieldEditable is set to false
                                // DMS5-4331  
                                textbox.Enabled = fieldEditable;

                                //Textbox to either mask or not
                                TextBox txtMask = new TextBox();
                                //Retype Textbox to (TextBox2 To match data for textbox one and masking)
                                txtMask.ID = dr["WorkflowStageFields_ControlID"].ToString() + "_Retype";
                                textbox.Attributes.Add("onchange", "return ValidateDataMismatch('" + textbox.ID + "','" + txtMask.ID + "' , '" + label.ID + "');");
                                txtMask.Attributes.Add("onchange", "return ValidateDataMismatch('" + textbox.ID + "','" + txtMask.ID + "', '" + label.ID + "');");
                                txtMask.ToolTip = "Retype " + label.Text;
                                tableCell2.Controls.Add(txtMask);

                                //Disable controls if fieldEditable is set to false
                                // DMS5-4331  
                                txtMask.Enabled = fieldEditable;


                                // MaskNone binding
                                if (objType.Equals(Convert.ToString(FieldTypes.DualDataEntry_MaskNone)))
                                {
                                    txtMask.Text = fieldValue;
                                }

                                //Masking the first textbox if field is MaskOne
                                if (objType.Equals(Convert.ToString(FieldTypes.DualDataEntry_MaskFirst)))
                                {
                                    //binding the textMask Textbox value in password format
                                    textbox.Attributes.Add("value", fieldValue);
                                    txtMask.Text = fieldValue;
                                    textbox.TextMode = TextBoxMode.Password;
                                }
                                //Masking the Second textbox if field is MaskTwo
                                else if (objType.Equals(Convert.ToString(FieldTypes.DualDataEntry_MaskSecond)))
                                {
                                    //binding the textMask Textbox value in password format
                                    txtMask.Attributes.Add("value", fieldValue);
                                    textbox.Text = fieldValue;
                                    txtMask.TextMode = TextBoxMode.Password;
                                }
                                //Masking the Both textbox if field is MaskBoth
                                else if (objType.Equals(Convert.ToString(FieldTypes.DualDatEntry_MaskBoth)))
                                {
                                    textbox.Attributes.Add("value", fieldValue);
                                    txtMask.Attributes.Add("value", fieldValue);
                                    textbox.Text = fieldValue;
                                    //binding the textMask Textbox value in password format
                                    textbox.TextMode = TextBoxMode.Password;
                                    txtMask.TextMode = TextBoxMode.Password;
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Trace("Exception: " + ex.Message, Convert.ToString(LoggedUserId));
                            }
                        }
                        #endregion
                        // DMS5-3425 BE

                        //DMS5-3474  BS   
                        #region Current Stage Calculate Formula logic
                        else if (objType.Equals(Convert.ToString(FieldTypes.CalculateField)))
                        {
                            try
                            {
                                Label label = new Label();
                                label.ID = dr["WorkflowStageFields_ControlID"].ToString() + labelIdSuffix;
                                label.Text = labelText;
                                tableCell1.Controls.Add(label);

                                TextBox CalculateFormula = new TextBox();
                                CalculateFormula.ID = dr["WorkflowStageFields_ControlID"].ToString();
                                CalculateFormula.ToolTip = dr["WorkflowStageFields_vFormula"].ToString();
                                CalculateFormula.Text = fieldValue;
                                tableCell2.Controls.Add(CalculateFormula);

                                //Disable controls if fieldEditable is set to false
                                // DMS5-4331  
                                CalculateFormula.Enabled = fieldEditable;

                                #region Add/attach events to referred fields to calculate formula
                                //Get the control Id's list, in db it's saved like 1001,1002,1003
                                string referedFieldIds = Convert.ToString(dr["WorkflowStageFields_vReferedFieldIds"]);
                                string[] controlsIdList = referedFieldIds.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                                foreach (string controlId in controlsIdList)
                                {
                                    string referredControlId = "Control_" + controlId;
                                    TextBox txtReferredField = FindControlRecursive(controlContainer, referredControlId) as TextBox;
                                    txtReferredField.AutoPostBack = true;
                                    txtReferredField.TextChanged += new EventHandler(txt_formulaCalculation);

                                    //Keep formula fields information in property to calculate formula on field value change
                                    // ReferredFieldId, ReferredFieldName, ReferredFieldIdList, FormulaFieldId, Formula
                                    DataView dv = new DataView(controlsConfig, "WorkflowStageFields_iId='" + controlId + "'  ", "", DataViewRowState.CurrentRows);
                                    string referredFieldName = Convert.ToString(dv.ToTable().Rows[0]["WorkflowStageFields_vName"]);

                                    // Check duplicate and insert into datatable property (FormulaFieldData)
                                    string find = "ReferredFieldId = '" + referredControlId + "' AND  FormulaFieldId = '" + CalculateFormula.ID + "'";
                                    DataRow[] foundRows = FormulaFieldData.Select(find);
                                    if (foundRows.Length.Equals(0))
                                    {
                                        DataTable dt = FormulaFieldData;
                                        dt.Rows.Add(referredControlId, referredFieldName, referedFieldIds, CalculateFormula.ID, CalculateFormula.ToolTip);
                                        FormulaFieldData = dt;
                                    }
                                }
                                #endregion
                            }
                            catch (Exception ex)
                            {
                                Logger.Trace("Exception: " + ex.Message, Convert.ToString(LoggedUserId));
                            }
                        }
                        #endregion
                        //DMS5-3474  BE  

                         //DMS5-3627 BS   
                        #region Current Stage Documnet/image Upload
                        else if (objType.Equals(Convert.ToString(FieldTypes.Document)))
                        {
                            try
                            {
                                Label label = new Label();
                                label.ID = controlID + labelIdSuffix;
                                label.Text = labelText;
                                tableCell1.Controls.Add(label);

                                AsyncFileUpload AsyncFileUpload1 = new AsyncFileUpload();
                                AsyncFileUpload1.UploadedComplete += new EventHandler<AsyncFileUploadEventArgs>(AsyncFileUpload1_UploadedComplete);
                                AsyncFileUpload1.ID = controlID;
                                AsyncFileUpload1.EnableViewState = false;
                                tableCell2.Controls.Add(AsyncFileUpload1);
                                AsyncFileUpload1.ToolTip = fieldValue;
                                AsyncFileUpload1.Width = Unit.Pixel(200);

                                //Disable controls if fieldEditable is set to false
                                // DMS5-4331  
                                AsyncFileUpload1.Enabled = fieldEditable;

                                // Add throber to file upload control
                                Label lblThrobber = new Label();
                                lblThrobber.ID = controlID + "_Throbber";
                                lblThrobber.Text = "<img alt='Loading...' src='/Images/indicator.gif' /></asp:Label>";
                                tableCell2.Controls.Add(lblThrobber);
                                AsyncFileUpload1.ThrobberID = lblThrobber.ID;

                                string url = (fieldValue.Length > 0 ? GetSrc("Handler") + fieldValue : "#");
                                url = url.Replace(@"\", "/");

                                LinkButton lnkViewFile = new LinkButton();
                                lnkViewFile.ID = dr["WorkflowStageFields_ControlID"].ToString() + "_HyperLink";
                                lnkViewFile.Text = "view";
                                lnkViewFile.OnClientClick = url != "#" ? "window.open('" + url + "','_newtab');" : "#";
                                if (fieldValue.Length.Equals(0))
                                    lnkViewFile.Enabled = false;
                                tableCell4.Controls.Add(lnkViewFile);

                                //Disable controls if fieldEditable is set to false
                                // DMS5-4331  
                                lnkViewFile.Enabled = fieldEditable;


                                //LinkButton lnkFileDelete = new LinkButton();
                                //lnkFileDelete.ID = dr["WorkflowStageFields_ControlID"].ToString() + "_LinkButton";
                                //lnkFileDelete.Text = "clear";
                                //lnkFileDelete.Click += new EventHandler(btnFileDelete_Click);
                                //tableCell4.Controls.Add(lnkFileDelete);
                                //lnkFileDelete.CssClass = "btndelete";

                            }
                            catch (Exception ex)
                            {
                                Logger.Trace("Exception: " + ex.Message, Convert.ToString(LoggedUserId));
                            }

                        }
                        //DMS5-3627  BE
                        #endregion
                        //DMS5-3627 BE 

                        #region Add asteric for mandatory fields
                        if (mandatory.Equals(true))
                        {
                            Label lblAstreick = new Label();
                            lblAstreick.ForeColor = System.Drawing.Color.Red;
                            lblAstreick.Text = "*";
                            lblAstreick.Style.Add("font-size", "large");
                            tableCell3.Controls.Add(lblAstreick);

                            //Add control id to mandatory feilds list for mandatory validation later through javascript
                            if (!mandatoryValidationScript.Equals(string.Empty))
                            {
                                mandatoryValidationScript += "##";
                            }
                            mandatoryValidationScript += controlID + "|" + objType;
                        }
                        #endregion

                        #region special character validation;

                        if (dbType.Equals("String") || dbType.Equals("Boolean"))
                        {
                            if (specialCharValidationScript != "")
                            {
                                specialCharValidationScript += "##";
                            }
                            specialCharValidationScript += controlID + "|" + objType;
                        }

                        #endregion

                        tableRow.Controls.Add(tableCell1);
                        tableRow.Controls.Add(tableCell2);
                        tableRow.Controls.Add(tableCell3);
                        tableRow.Controls.Add(tableCell4);
                        controlContainer.Rows.Add(tableRow);

                        if (remarks.Equals(true))
                        {
                            ControlPlaceHolder_PrevStage.Controls.Add(controlContainer);
                        }


                    }

                    placeholder.Controls.Add(controlContainer);




                    Logger.Trace("Rendering controls completed.", Convert.ToString(LoggedUserId));
                }
                #endregion

            }
            catch (Exception ex)
            {
                Logger.Trace("Exception: " + ex.Message, Convert.ToString(LoggedUserId));
                lblMessage.Text = ex.Message.ToString();
            }
            return controlContainer;
        }

        protected void InitializeAfterRender(DataTable controlsConfig, DataTable controlsData)
        {
            //Set data row
            DataRow dataRow = controlsData.Rows[0];

            foreach (DataRow dr in controlsConfig.Rows)
            {
                #region variable declaration and initialization
                string objType = Convert.ToString(dr["WorkflowStageFields_cObjType"]);
                string controlID = Convert.ToString(dr["WorkflowStageFields_ControlID"]);

                int childId = Convert.ToInt32(dr["WorkflowStageFields_iChildId"]);
                int parentId = Convert.ToInt32(dr["WorkflowStageFields_iParentId"]);

                string dbField = Convert.ToString(dr["WorkflowStageFields_vDBFld"]);
                string fieldValue = Convert.ToString(dataRow[dbField]);
                #endregion

                Logger.Trace("Initializing controls after rendering. Object Type: " + objType + ", Control Id: " + controlID, Convert.ToString(LoggedUserId));

                #region dropdown
                if (objType.Equals(Convert.ToString(FieldTypes.DropDown)))
                {
                    if (childId > 0 || childId.Equals(0))
                    {
                        //add auto post back to parent controls - to change the child control values
                        DropDownList ddlParent = (DropDownList)this.ControlPlaceHolder_CurStage.FindControl(controlID);
                        ddlParent.AutoPostBack = true;
                        ddlParent.Attributes.Add("onclick", "return SetDDLControlName('" + ddlParent.ID.ToString() + "');");
                        if (childId > 0)
                            ddlParent.SelectedIndexChanged += new EventHandler(ddlFilter_SelectedIndexChanged);
                    }

                    //fill child dropdowns
                    if (parentId > 0)
                    {
                        DropDownList ddlParent = (DropDownList)this.ControlPlaceHolder_CurStage.FindControl("Control_" + parentId);
                        ddlFilter_SelectedIndexChanged(ddlParent, new EventArgs());

                        //set value of child dropdowns
                        DropDownList ddlchild = (DropDownList)this.ControlPlaceHolder_CurStage.FindControl(controlID);
                        try
                        {
                            ddlchild.SelectedValue = fieldValue.Length > 0 ? fieldValue : "0";
                        }
                        catch
                        {
                            ddlchild.SelectedValue = "0";
                        }
                    }
                }
                #endregion

                #region multidropdowns
                //fill child miltidropdowns
                if (objType.Equals(Convert.ToString(FieldTypes.MultiDropDown)))
                {
                    if (parentId > 0)
                    {
                        DropDownList ddlParent = (DropDownList)this.ControlPlaceHolder_CurStage.FindControl("Control_" + parentId);
                        ddlFilter_SelectedIndexChanged(ddlParent, new EventArgs());

                        //set value of child dropdowns
                        CheckBoxList ddlchild = (CheckBoxList)this.ControlPlaceHolder_CurStage.FindControl(controlID);
                        try
                        {
                            string strMultiValues = fieldValue;
                            foreach (ListItem li in ddlchild.Items)
                            {
                                if (strMultiValues.Split(',').Contains(li.Value.ToString()))
                                {
                                    li.Selected = true;
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                #endregion
            }
            Logger.Trace("Completed initialization of controls.", Convert.ToString(LoggedUserId));
        }

        protected string BuildQuery(DataTable controlsConfig, PlaceHolder placeholder)
        {
            string tableName = string.Empty;
            string fieldName = string.Empty;
            string fieldValue = string.Empty;
            string objType = string.Empty;
            string controlID = string.Empty;

            //for (int i = 0; i < controlsConfig.Rows.Count; i++)
            foreach (DataRow dr in controlsConfig.Rows)
            {
                objType = dr["WorkflowStageFields_cObjType"].ToString();
                tableName = Convert.ToString(dr["WorkflowStageFields_vTable"]);
                fieldName = Convert.ToString(dr["WorkflowStageFields_vDBFld"]);
                controlID = dr["WorkflowStageFields_ControlID"].ToString();

                Logger.Trace("Building update query from controls. Object Type: " + objType + ", Control Id: " + controlID, Convert.ToString(LoggedUserId));

                //Clear value for next iteration
                fieldValue = string.Empty;

                if (updateQuery.Length == 0)
                {
                    updateQuery = "UPDATE " + tableName + " SET ";
                }

                if (objType.Equals(Convert.ToString(FieldTypes.TextBox)))
                {
                    TextBox txtName = (TextBox)placeholder.FindControl(controlID);
                    if (txtName != null)
                    {
                        fieldValue = txtName.Text;
                    }
                }
                else if (objType.Equals(Convert.ToString(FieldTypes.DropDown)))
                {
                    DropDownList ddlField = (DropDownList)placeholder.FindControl(controlID);
                    if (ddlField != null)
                    {
                        fieldValue = ddlField.SelectedValue;
                    }
                }
                else if (objType.Equals(Convert.ToString(FieldTypes.MultiDropDown)))
                {
                    CheckBoxList ddlField = (CheckBoxList)placeholder.FindControl(controlID);
                    if (ddlField != null)
                    {
                        foreach (ListItem li in ddlField.Items)
                        {
                            if (li.Selected)
                            {
                                if (fieldValue != string.Empty)
                                {
                                    fieldValue += ",";
                                }
                                fieldValue += li.Value.ToString();
                            }
                        }
                    }
                }
                //to save document type to database  DMS5-3627
                else if (objType.Equals(Convert.ToString(FieldTypes.Document)))
                {
                    AsyncFileUpload asyncFileUpload = placeholder.FindControl(controlID) as AsyncFileUpload;
                    DataView dvFormulaFieldData = new DataView(UploadedFiles, "ControlId = '" + asyncFileUpload.ID + "'", string.Empty, DataViewRowState.CurrentRows);
                    if (dvFormulaFieldData.Count > 0)
                    {
                        string filePath = Convert.ToString(dvFormulaFieldData[0]["FilePath"]);
                        fieldValue = filePath.Replace(GetTemporaryFolderPath(), GetOriginalFolderPath());
                    }
                }

                   //To Save special feilds to database of current stages    DMS5-3424 , DMS5-3425, DMS5-3473, DMS5-3474
                else if (objType.Equals(Convert.ToString(FieldTypes.DateCalculation_CurrentDate))
                      || objType.Equals(Convert.ToString(FieldTypes.CalculateField))
                      || objType.Equals(Convert.ToString(FieldTypes.DateCalculation_ReferDate))
                      || objType.Equals(Convert.ToString(FieldTypes.DualDataEntry_MaskNone))
                      || objType.Equals(Convert.ToString(FieldTypes.DualDataEntry_MaskFirst))
                      || objType.Equals(Convert.ToString(FieldTypes.DualDataEntry_MaskSecond))
                      || objType.Equals(Convert.ToString(FieldTypes.DualDatEntry_MaskBoth)))
                {
                    TextBox txtName = (TextBox)placeholder.FindControl(controlID);
                    if (txtName != null)
                    {
                        fieldValue = txtName.Text;
                    }
                }
                updateQuery += fieldName + "='" + fieldValue + "', ";
            }
            Logger.Trace("Completed building query. Query: " + updateQuery, Convert.ToString(LoggedUserId));
            return updateQuery;
        }

        /// <summary>
        /// Diffrence in Date Calculation
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns>Diffrence in Years Months and Days between two days</returns>
        protected string DateCalculation(string fromDate, string toDate)
        {
            DateTime date1 = DateTime.ParseExact(fromDate, "dd/MM/yyyy", null);
            DateTime date2 = DateTime.ParseExact(toDate, "dd/MM/yyyy", null);
            int Days = date2.Day - date1.Day;
            int Months = date2.Month - date1.Month;
            int Years = date2.Year - date1.Year;

            if (Months < 0)
            {
                Months += 12;
                Years--;
            }
            return Convert.ToString(Math.Abs(Years) + "  Years  " + Math.Abs(Months) + "  Months  " + Math.Abs(Days) + "  Days  ");
        }

        #endregion

        //DMS5-4332 BS
        protected void DDLStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Logger.Trace("Status changed", Session["LoggedUserId"].ToString());

                hdnStatusMessage.Value = string.Empty;

                DBResult Objresult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                ObjDataEntry.WorkflowDataEntryProcessId = Convert.ToInt32(Session["ProcessId"]);
                ObjDataEntry.WorkflowDataEntryWorkflowId = Convert.ToInt32(Session["WorkflowId"]);
                ObjDataEntry.WorkflowDataEntryStageId = Convert.ToInt32(Session["StageId"]);
                ObjDataEntry.LoginOrgId = loginUser.LoginOrgId;
                ObjDataEntry.LoginToken = loginUser.LoginToken;
                ObjDataEntry.WorkflowDataEntryFieldDataId = Convert.ToInt32(Session["FieldDataId"]);
                ObjDataEntry.WorkflowDataEntryFieldStatusId = Convert.ToInt32(DDLStatus.SelectedValue);

                Objresult = ObjDataEntry.ManageWorkflowDataEntry(ObjDataEntry, "GetConfirmStatusMessage");

                if (Objresult.dsResult.Tables.Count > 0)
                    hdnStatusMessage.Value = Objresult.dsResult.Tables[0].AsEnumerable()
                        .Select(s => s.Field<string>("WorkflowStageStatus_vConfirmMsgOnSubmit")).First();
            }
            catch (Exception ex)
            {
                Logger.Trace("Status changed Ended " + ex.Message.ToString(), Session["LoggedUserId"].ToString());
            }
        }
        //DMS5-4332 BE


    }
}