/* ============================================================================  
Author     : 
Create date: 
===============================================================================  
** Change History   
** Date:          Author:            Issue ID               Description:  
** ----------------------------------------------------------------------------
** 03 Dec 2013          Mandatory   (UMF)                   Set Mandatory for index fields
 * 28/02/2015   Sabina              DMS5-3384               Add dataentryId to querystring to enable coordinate textbox
 * 28/02/2015   Sabina              DMS5-3383               add new five fields into Stagefieldedit
 * 28/02/2015   Sabina              DMS5-3426               Get the coordinates on image dragging
 * 28/02/2015   Sabina              DMS5-3421               Navigation in page dropdownlist
 * 28/02/2015   Sabina              DMS5-3433               Save new coordinate values to stagefields
 * 28/02/2015   Sabina              DMS5-3484               Capture coordinates for formtype
 * 03/19/2015  Sabina               DMS5-3579               Bringing a new checkbox to denote form type dataentry requires with image or without image
 * 03/19/2015  Sabina               DMS5-3582               Design-Stage field creation/edit page
 * 04/07/2015   Sabina              DMS5-3847               binding image and page dropdown for capturing coordinates modification after code review
 * 09/04/2015   gokul               DMS04-3877              adding check box
 * 10/04/2015   Sabina              DMS04-3895	            fix in validation of stage edit fields
 * 11 Apr 2015  Yogeesha            DMS5-3904               Load background images directly from template folder 
 * 27/02/2015   Sharath             DMS5-3422               Save the Stage Feild Type to database
 * 17 Apr 2015  Gokul               DMS5-3946                Applying rights an permissions in all pages as per user group rights!
 * 20 Apr 2015  sharath             DMS5-3972               Previous fields are displayed in the Execute formula area when creating a new formula field
 * 20 Apr 2015  sharath             DMS5-3971               One field cannot be used to in tow or more formulas    
 * 08 Apr 2015  Sabina              DMS5-4122               Button not enabled in workflow pages
 * 02 Jul 2015  Sharath             DMS5-4423               Data Captured Form usage
 * 10/08/2015   sharath             DMSENH6-4732            Deletion options within Workflow
=============================================================================== */
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
using System.Web.UI.HtmlControls;
using System.IO;
namespace Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin
{
    public partial class ManageStageFeildsEdit : PageBase
    {
        WorkflowStageField objStageField = new WorkflowStageField();
        //added newly
        WorkflowStage objStage = new WorkflowStage();
        HtmlTable tbl = new HtmlTable();
        string referredIds = string.Empty;
        public string pageRights = string.Empty; /* DMS5-3946 A */
       
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["DynamicControls"] != null)
            {
                HtmlTable Controls = Session["DynamicControls"] as HtmlTable;
                PlaceHolderFormula.Controls.Add(Controls);
            }
            CheckAuthentication();
            pageRights = GetPageRights("CONFIGURATION");/* DMS5-3946 A,DMS5-4122M added parameter configuration */

            // DMSENH6-4732 BS
            //Disable buttons if the workflow is confirmed
            if ((Session["workflowConfirmed"]) != null && (Session["workflowConfirmed"]).Equals("Yes"))
            {
                DisableControls();
            }
            // DMSENH6-4732 BE
       
            string dataEntryType = string.Empty;
            //Start: ------------------SitePath Details ----------------------
            Label sitePath = (Label)Master.FindControl("SitePath");
            sitePath.Text = "WorkFlow >> Process >> Workflow >> Stages >> Fields >> Add/Edit Fields";
            //End: ------------------SitePath Details ------------------------

            if (Request.QueryString["ProcessId"] != null)
            {
                // Store process id in view state for later use
                Session["ProcessId"] = Decrypt(HttpUtility.UrlDecode(Request.QueryString["ProcessId"].ToString()));
                Session["WorkflowId"] = Decrypt(HttpUtility.UrlDecode(Request.QueryString["WorkflowId"].ToString()));
                Session["StageId"] = Decrypt(HttpUtility.UrlDecode(Request.QueryString["StageId"].ToString()));
                Session["FieldStageId"] = Decrypt(HttpUtility.UrlDecode(Request.QueryString["FieldStageId"].ToString()));

                //DMS5-3384A
                if (Request.QueryString["DataentryId"] != null)
                {
                    Session["DataEntryTypeId"] = Decrypt(HttpUtility.UrlDecode(Request.QueryString["DataentryId"]));
                    objStage = GetValuesFromSession();
                    DBResult dbResult = new DBResult();
                    dbResult = objStage.ManageWorkflowStages(objStage, "GetDataEntryType");
                    if (dbResult.dsResult != null && dbResult.dsResult.Tables.Count > 0 && dbResult.dsResult.Tables[0].Rows.Count > 0)
                    {
                        dataEntryType = Convert.ToString(dbResult.dsResult.Tables[0].Rows[0]["DataEntryType"]);//Get the dataentry type based on querystring
                        Logger.Trace("Dataentry Type of the stage : " + dataEntryType, Session["LoggedUserId"].ToString());
                        Session["dataEntryTypeName"] = dataEntryType;
                        hdnDataType.Value = dataEntryType;
                    }
                }
                if (Request.QueryString["FieldId"] != null)
                {
                    Session["FieldId"] = Decrypt(HttpUtility.UrlDecode(Request.QueryString["FieldId"].ToString()));
                    Session["GridType"] = Decrypt(HttpUtility.UrlDecode(Request.QueryString["GridType"].ToString()));
                }
                else
                {
                    Session["FieldId"] = "0";
                    Session["GridType"] = "";
                }
                if (Request.QueryString["StageName"] != null)
                {
                    Session["StageName"] = Decrypt(HttpUtility.UrlDecode(Request.QueryString["StageName"]));
                }
                lblCurrentStageNameHeaderValue.Text = Session["StageName"].ToString();
            }
            if (!IsPostBack)
            {
                ApplyPageRights(pageRights, this.Form.Controls); /* DMS5-3946 A */
                ClearControls();
                BindDDLMultiValueMain();
                if (Session["GridType"].ToString() == "MainGrid")//StageId from Stagelist Gridview  from ManageStageFeildList.aspx
                {
                    ControlsEditMode();
                    HiddenStageFieldId.Value = (string)Session["FieldId"];
                    btnSave.Visible = true;
                }
                else if (Session["GridType"].ToString() == "InheritedGrid")//FieldId from Stagelist Inherited Gridview from ManageStageFeildList.aspx
                {
                    ControlsEditMode();
                    btnSave.Visible = false;
                    DisableControls();
                }
                //DMS5-3383A
                if (dataEntryType == "Normal")
                {
                    btnCaptureCoordinates.Enabled = false;
                }
                else
                    //DMS5-3904A -- Shifted this from top rigth after if (!IsPostBack)
                    initializeCoordinateCapture();

                DivImage.Visible = false;
            }
            DDLDrop.Attributes.Add("onchange", "ddlDropChange();");//bind the image based on page dropdown change
        }

        protected void ControlsEditMode()
        {
            CheckAuthentication();
            string action = "GetStageFieldsOnEdit";
            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            try
            {
                objStageField.LoginToken = loginUser.LoginToken;
                objStageField.LoginOrgId = loginUser.LoginOrgId;
                objStageField.WorkflowStageFieldsId = Convert.ToInt32(Session["FieldId"].ToString());
                objStageField.WorkflowStageFieldsProcessId = Convert.ToInt32(Session["ProcessId"].ToString());
                objStageField.WorkflowStageFieldsWorkflowId = Convert.ToInt32(Session["WorkflowId"].ToString());
                objStageField.WorkflowStageFieldsStageId = Convert.ToInt32(Session["FieldStageId"].ToString());
                objResult = objStageField.ManageStageFields(objStageField, action);
                Logger.Trace("Retrived the data to bind the controls", Session["LoggedUserId"].ToString());
                if (objResult.dsResult != null && objResult.dsResult.Tables.Count > 0 && objResult.dsResult.Tables[0].Rows.Count > 0)
                {
                    System.Data.DataRow currentRow = objResult.dsResult.Tables[0].Rows[0];
                    txtFieldName.Text = currentRow["WorkflowStageFields_vName"].ToString();
                    if (currentRow["WorkflowStageFields_cObjType"].ToString() == "TextBox")
                    {
                        RadioSingle.Checked = true;
                    }
                    if (currentRow["WorkflowStageFields_cObjType"].ToString() == "DropDown" && Convert.ToInt32(currentRow["WorkflowStageFields_iParentId"]) == 0)
                    {
                        RadioSingleValueMain.Checked = true;
                        DDLMultiValueMain.SelectedValue = currentRow["WorkflowStageFields_iMasterType"].ToString();
                    }
                    if (currentRow["WorkflowStageFields_cObjType"].ToString() == "MultiDropDown" && Convert.ToInt32(currentRow["WorkflowStageFields_iParentId"]) == 0)
                    {
                        RadioMultiValueMain.Checked = true;
                        DDLMultiValueMain.SelectedValue = currentRow["WorkflowStageFields_iMasterType"].ToString();
                    }
                    if (currentRow["WorkflowStageFields_cObjType"].ToString() == "DropDown" && Convert.ToInt32(currentRow["WorkflowStageFields_iParentId"]) != 0)
                    {
                        RadioSingleValueSub.Checked = true;
                        DDLMultiValueSub1.SelectedValue = currentRow["WorkflowStageFields_iParentId"].ToString();
                        BindDDLMultiValueSub();
                        DDLMultiValueSub2.SelectedValue = currentRow["WorkflowStageFields_iMasterType"].ToString();
                    }
                    if (currentRow["WorkflowStageFields_cObjType"].ToString() == "MultiDropDown" && Convert.ToInt32(currentRow["WorkflowStageFields_iParentId"]) != 0)
                    {
                        RadioMultiValueSub.Checked = true;
                        DDLMultiValueSub1.SelectedValue = currentRow["WorkflowStageFields_iParentId"].ToString();
                        BindDDLMultiValueSub();
                        DDLMultiValueSub2.SelectedValue = currentRow["WorkflowStageFields_iMasterType"].ToString();
                    }

                    //Bind values to controls in edit mode for new feild types

                    if (currentRow["WorkflowStageFields_cObjType"].ToString() == Convert.ToString(FieldTypes.DateCalculation_CurrentDate))
                    {
                        RadioCurrentDate.Checked = true;
                        RadioAutoAge.Checked = true;
                    }
                    if (currentRow["WorkflowStageFields_cObjType"].ToString() == Convert.ToString(FieldTypes.DateCalculation_ReferDate))
                    {
                        RadioReferDate.Checked = true;
                        RadioReferDate_CheckedChanged(new object(), new EventArgs());
                        DDLReferDate.Items.FindByValue(currentRow["WorkflowStageFields_vReferedFieldIds"].ToString()).Selected = true;
                        RadioAutoAge.Checked = true;
                    }
                    if (currentRow["WorkflowStageFields_cObjType"].ToString() == Convert.ToString(FieldTypes.DualDataEntry_MaskNone))
                    {
                        RadioDualDataEntryNone.Checked = true;
                        RadioDualDataEntry.Checked = true;
                    }
                    if (currentRow["WorkflowStageFields_cObjType"].ToString() == Convert.ToString(FieldTypes.DualDataEntry_MaskFirst))
                    {
                        RadioDualDataEntryMask1.Checked = true;
                        RadioDualDataEntry.Checked = true;
                    }
                    if (currentRow["WorkflowStageFields_cObjType"].ToString() == Convert.ToString(FieldTypes.DualDataEntry_MaskSecond))
                    {
                        RadioDualDataEntryMask2.Checked = true;
                        RadioDualDataEntry.Checked = true;
                    }
                    if (currentRow["WorkflowStageFields_cObjType"].ToString() == Convert.ToString(FieldTypes.DualDatEntry_MaskBoth))
                    {
                        RadioDualDataEntryMaskBoth.Checked = true;
                        RadioDualDataEntry.Checked = true;
                    }
                    if (currentRow["WorkflowStageFields_cObjType"].ToString() == Convert.ToString(FieldTypes.CalculateField))
                    {
                        RadioAutoCalculateFeild.Checked = true;
                        RadioAutoCalculateFeild_CheckedChanged(RadioAutoCalculateFeild, null);
                        txtFormulaArea.Text = currentRow["WorkflowStageFields_vFormula"].ToString();
                    }
                    if (currentRow["WorkflowStageFields_cObjType"].ToString() == Convert.ToString(FieldTypes.Document))
                    {
                        RadioImageUpload.Checked = true;
                    }
                    if (currentRow["WorkflowStageFields_vDBType"].ToString() == "String")
                    {
                        RadioFeildDataType.SelectedIndex = 0;
                    }
                    else if (currentRow["WorkflowStageFields_vDBType"].ToString() == "Date")
                    {
                        RadioFeildDataType.SelectedIndex = 1;
                    }
                    else if (currentRow["WorkflowStageFields_vDBType"].ToString() == "Int")
                    {
                        RadioFeildDataType.SelectedIndex = 2;
                    }
                    else if (currentRow["WorkflowStageFields_vDBType"].ToString() == "Boolean")
                    {
                        RadioFeildDataType.SelectedIndex = 3;
                    }
                    txtMin.Text = currentRow["WorkflowStageFields_iMin"].ToString();
                    txtMax.Text = currentRow["WorkflowStageFields_iMax"].ToString();
                    if (currentRow["WorkflowStageFields_bActive"].ToString() == "True")
                    {
                        CheckActive.Checked = true;
                    }
                    else
                    {
                        CheckActive.Checked = false;
                    }

                    if (currentRow["WorkflowStageFields_bMandatory"].ToString() == "True")
                    {
                        CheckMandatory.Checked = true;
                    }
                    else
                    {
                        CheckMandatory.Checked = false;
                    }

                    /*DMS04-3877 BS  For checking the check box for edit */
                    if (currentRow["WorkflowStageFields_bShowInDasboard"].ToString() == "True")
                    {
                        chkShowInDashboard.Checked = true;
                    }
                    else
                    {
                        chkShowInDashboard.Checked = false;
                    }
                    /*DMS04-3877 BE */
                    if (currentRow["WorkflowStageFields_bDisplay"].ToString() == "True")
                    {
                        CheckDisplay.Checked = true;
                    }
                    else
                    {
                        CheckDisplay.Checked = false;
                    }

                    if (currentRow["WorkflowStageFields_bEditable"].ToString() == "True")
                    {
                        chkEditable.Checked = true;
                    }
                    else
                    {
                        chkEditable.Checked = false;
                    }

                    //DMS5-4423 BS                   
                     if (currentRow["WorkflowStageFields_bRemarks"].ToString() == "True")
                     {
                         chkRemarks.Checked = true;
                     }
                     else
                     {
                         chkRemarks.Checked = false;
                     }
                     //DMS5-4423 BE
                    txtMandatoryErrorMessage.Text = currentRow["WorkflowStageFields_vShowMsgIfNotFound"].ToString();
                    txtLabelText.Text = currentRow["WorkflowStageFields_vLabelText"].ToString();
                    hdnParentRowId.Value = currentRow["WorkflowStageFields_iParentId"].ToString();

                    //DMS5-3426A bind the coordinate textbox based on the values from the dataset
                    if (hdnDataType.Value != null && Convert.ToString(hdnDataType.Value) != string.Empty)
                    {
                        if (Convert.ToString(hdnDataType.Value) != "Normal")
                        {
                            txtX1.Text = Convert.ToString(currentRow["WorkflowStageFields_iLocationX1"]);
                            txtX2.Text = Convert.ToString(currentRow["WorkflowStageFields_iLocationX2"]);
                            txtY1.Text = Convert.ToString(currentRow["WorkflowStageFields_iLocationY1"]);
                            txtY2.Text = Convert.ToString(currentRow["WorkflowStageFields_iLocationY2"]);
                            txtPageNumber.Text = Convert.ToString(currentRow["WorkflowStageFields_iPageNo"]);
                            hdnResolution.Value = Convert.ToString(currentRow["WorkflowStageFields_iImageWidth"]) + "*" + Convert.ToString(currentRow["WorkflowStageFields_iImageHeight"]);
                            hdnHeight.Value = Convert.ToString(currentRow["WorkflowStageFields_iHeight"]);
                            hdnWidth.Value = Convert.ToString(currentRow["WorkflowStageFields_iWidth"]);
                        }
                        else
                        {
                            btnCaptureCoordinates.Enabled = false;
                        }
                    }
                }
                HiddenFieldPopUp.Value = "EditMode";
                Logger.Trace("All controls values binded properly", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)

            {
                throw ex;
            }
        }
        // DMS5-3433,DMS04-3895
        protected void btnSave_Click(object sender, EventArgs e)
        {
            CheckAuthentication();
            string action = "AddStageFields";
            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            objStageField.LoginToken = loginUser.LoginToken;
            objStageField.LoginOrgId = loginUser.LoginOrgId;
            objStageField.WorkflowStageFieldsName = txtFieldName.Text.Trim();
            if (RadioSingle.Checked == true)
            {
                objStageField.WorkflowStageFieldsObjType = "TextBox";
                foreach (ListItem item in RadioFeildDataType.Items)
                {
                    if (item.Selected)
                    {
                        objStageField.WorkflowStageFieldsDBType = item.ToString();
                        break;
                    }
                }
            }
            else if (RadioSingleValueMain.Checked == true)
            {
                objStageField.WorkflowStageFieldsObjType = "DropDown";
                objStageField.WorkflowStageFieldsDBType = DDLMultiValueMain.SelectedItem.Text;
                objStageField.WorkflowStageFieldsMasterType = Convert.ToInt32(DDLMultiValueMain.SelectedItem.Value.ToString());
                objStageField.WorkflowStageFieldsParentId = 0;
            }
            else if (RadioMultiValueMain.Checked == true)
            {
                objStageField.WorkflowStageFieldsObjType = "MultiDropDown";
                objStageField.WorkflowStageFieldsDBType = DDLMultiValueMain.SelectedItem.Text;
                objStageField.WorkflowStageFieldsMasterType = Convert.ToInt32(DDLMultiValueMain.SelectedItem.Value.ToString());
                objStageField.WorkflowStageFieldsParentId = 0;
            }
            else if (RadioSingleValueSub.Checked == true)
            {
                objStageField.WorkflowStageFieldsObjType = "DropDown";
                objStageField.WorkflowStageFieldsDBType = DDLMultiValueSub2.SelectedItem.Text;
                objStageField.WorkflowStageFieldsMasterType = Convert.ToInt32(DDLMultiValueSub2.SelectedItem.Value.ToString());
                objStageField.WorkflowStageFieldsParentId = Convert.ToInt32(hdnParentRowId.Value);
            }
            else if (RadioMultiValueSub.Checked == true)
            {
                objStageField.WorkflowStageFieldsObjType = "MultiDropDown";
                objStageField.WorkflowStageFieldsDBType = DDLMultiValueSub2.SelectedItem.Text;
                objStageField.WorkflowStageFieldsMasterType = Convert.ToInt32(DDLMultiValueSub2.SelectedItem.Value.ToString());
                objStageField.WorkflowStageFieldsParentId = Convert.ToInt32(hdnParentRowId.Value);
            }


              // DMS5-3422 BS 

            else if (RadioImageUpload.Checked == true)
            {
                objStageField.WorkflowStageFieldsObjType = FieldTypes.Document.ToString();
                objStageField.WorkflowStageFieldsDBType = DBTypes.String.ToString();
            }

                //Feild Type Dual Data Entry
            else if (RadioDualDataEntryNone.Checked == true)
            {
                objStageField.WorkflowStageFieldsObjType = FieldTypes.DualDataEntry_MaskNone.ToString();
                objStageField.WorkflowStageFieldsDBType = DBTypes.String.ToString();
            }
            else if (RadioDualDataEntryMask1.Checked == true)
            {
                objStageField.WorkflowStageFieldsObjType = FieldTypes.DualDataEntry_MaskFirst.ToString();
                objStageField.WorkflowStageFieldsDBType = DBTypes.String.ToString();
            }
            else if (RadioDualDataEntryMask2.Checked == true)
            {
                objStageField.WorkflowStageFieldsObjType = FieldTypes.DualDataEntry_MaskSecond.ToString();
                objStageField.WorkflowStageFieldsDBType = DBTypes.String.ToString();
            }
            else if (RadioDualDataEntryMaskBoth.Checked == true)
            {
                objStageField.WorkflowStageFieldsObjType = FieldTypes.DualDatEntry_MaskBoth.ToString();
                objStageField.WorkflowStageFieldsDBType = DBTypes.String.ToString();
            }
            //Feild Type Auto Date
            else if (RadioCurrentDate.Checked == true)
            {
                objStageField.WorkflowStageFieldsObjType = FieldTypes.DateCalculation_CurrentDate.ToString();
                objStageField.WorkflowStageFieldsDBType = DBTypes.Date.ToString();
            }
            //Feild Type Refer Date
            else if (RadioReferDate.Checked == true)
            {
                objStageField.WorkflowStageFieldsObjType = FieldTypes.DateCalculation_ReferDate.ToString();
                objStageField.WorkflowStageFieldsDBType = DBTypes.Date.ToString();
            }

             //Feild Type Calculate Feild
            else if (RadioAutoCalculateFeild.Checked == true)
            {
                objStageField.WorkflowStageFieldsObjType = FieldTypes.CalculateField.ToString();
                objStageField.WorkflowStageFieldsDBType = DBTypes.Number.ToString();
            }

            // DMS5-3422 BE


            if (Session["FieldId"] != null && Session["FieldId"].ToString() != "0")
            {
                action = "EditStageFields";
                objStageField.WorkflowStageFieldsId = Convert.ToInt32(Session["FieldId"].ToString());
            }
            if (txtMin.Text != string.Empty)
            {
                objStageField.WorkflowStageFields_iMin = Convert.ToInt32(txtMin.Text.Trim());
            }
            if (txtMax.Text != string.Empty)
            {
                objStageField.WorkflowStageFields_iMax = Convert.ToInt32(txtMax.Text.Trim());
            }
            objStageField.WorkflowStageFields_bActive = CheckActive.Checked;
            objStageField.WorkflowStageFields_bMandatory = CheckMandatory.Checked;
            objStageField.WorkflowStageFields_bDisplay = CheckDisplay.Checked;
            objStageField.WorkflowStageFields_bEditable = chkEditable.Checked;
            objStageField.WorkflowStageFields_ShowMsgIfNotFound = txtMandatoryErrorMessage.Text.Trim();
            objStageField.WorkflowStageFieldsLabelText = txtLabelText.Text.Trim();
            objStageField.WorkflowStageFieldsProcessId = Convert.ToInt32(Session["ProcessId"].ToString());
            objStageField.WorkflowStageFieldsWorkflowId = Convert.ToInt32(Session["WorkflowId"].ToString());
            objStageField.WorkflowStageFieldsStageId = Convert.ToInt32(Session["StageId"]);
            objStageField.WorkflowStageFields_bShowInDasboard = chkShowInDashboard.Checked; /*	DMS04-3877 -A */
            //DMS5-4423 BS           
            objStageField.WorkflowStageFields_bShowRemarks = chkRemarks.Checked;
            //DMS5-4423 BE

			objStageField.WorkflowStageFields_vFields = DDLReferDate.SelectedValue.ToString();
           if (txtFormulResult.Text.Trim() != string.Empty)
           {
               objStageField.WorkflowStageFields_vFields = (string)ViewState["FormulaIds"];
		      objStageField.WorkflowStageFields_vFormula = txtFormulaArea.Text.ToString();
 			 }
		    //DMS5-3433A retrive the data from textbox to save the values
            Logger.Trace("retrive the data from textbox to save the values", Session["LoggedUserId"].ToString());
            if (hdnDataType.Value != null && Convert.ToString(hdnDataType.Value) != string.Empty)
            {
                if (Convert.ToString(hdnDataType.Value) != "Normal")
                {
                    objStageField.WorkflowStageFields_iX1 = txtX1.Text != string.Empty ? Convert.ToInt32(txtX1.Text) : 0;
                    objStageField.WorkflowStageFields_iX2 = txtX2.Text != string.Empty ? Convert.ToInt32(txtX2.Text) : 0;
                    objStageField.WorkflowStageFields_iY1 = txtY1.Text != string.Empty ? Convert.ToInt32(txtY1.Text) : 0;
                    objStageField.WorkflowStageFields_iY2 = txtY2.Text != string.Empty ? Convert.ToInt32(txtY2.Text) : 0;
                    objStageField.WorkflowStageFields_iPageNo = txtPageNumber.Text != string.Empty ? Convert.ToInt32(txtPageNumber.Text) : 0;
                    if (hdnResolution.Value != "null*null" && hdnResolution.Value != string.Empty)
                    {
                        String[] resolution = hdnResolution.Value.Split('*');
                        objStageField.WorkflowStageFields_iImageHeight = Convert.ToInt32(resolution[1]);
                        objStageField.WorkflowStageFields_iImageWidth = Convert.ToInt32(resolution[0]);
                    }
                    else
                    {
                        objStageField.WorkflowStageFields_iImageHeight = 0;
                        objStageField.WorkflowStageFields_iImageWidth = 0;
                    }
                    if (hdnHeight.Value == null || hdnHeight.Value == string.Empty)
                    {
                        hdnHeight.Value = "0";
                    }
                    if (hdnWidth.Value == null || hdnWidth.Value == string.Empty)
                    {
                        hdnWidth.Value = "0";
                    }
                    objStageField.WorkflowStageFields_iHeight = hdnHeight.Value != string.Empty ? Convert.ToInt32(hdnHeight.Value) : 0;
                    objStageField.WorkflowStageFields_iWidth = hdnWidth.Value != string.Empty ? Convert.ToInt32(hdnWidth.Value) : 0;
                    Logger.Trace("Dataentry Type of the stage before saving : " + hdnDataType.Value, Session["LoggedUserId"].ToString());
                }
            }

            objResult = objStageField.ManageStageFields(objStageField, action);
            handleDBResult(objResult);
            //only if success
            if (objResult.ErrorState == 0)
            {
                ClearControls();
                Logger.Trace("Save completed successfully: ", Session["LoggedUserId"].ToString());
                string ProcessId = Session["ProcessId"].ToString(); //ProcessId
                string WorkflowId = Session["WorkflowId"].ToString(); //WorkflowId
                string StageId = Session["StageId"].ToString();
                ProcessId = HttpUtility.UrlEncode(Encrypt(ProcessId));
                WorkflowId = HttpUtility.UrlEncode(Encrypt(WorkflowId));
                StageId = HttpUtility.UrlEncode(Encrypt(StageId));
                string Querystring = "?ProcessId=" + ProcessId + "&WorkflowId=" + WorkflowId + "&StageId=" + StageId;
                Session.Remove("FieldId");
                Response.Redirect("~/Workflow/WorkflowAdmin/ManageStageFeildsList.aspx" + Querystring);
            }


        }

        private void ClearControls()
        {
            txtFieldName.Text = string.Empty;
            txtLabelText.Text = string.Empty;
            txtMandatoryErrorMessage.Text = string.Empty;
            txtMax.Text = string.Empty;
            txtMin.Text = string.Empty;
            CheckActive.Checked = true;
            CheckDisplay.Checked = true;
            chkEditable.Checked = true;
            CheckMandatory.Checked = false;
            chkShowInDashboard.Checked = false;  /*DMS04-3877 A*/
            RadioSingleValueMain.Checked = false;
            RadioMultiValueMain.Checked = false;
            RadioSingleValueSub.Checked = false;
            RadioMultiValueSub.Checked = false;
            RadioSingle.Checked = false;
            RadioDualDataEntry.Checked = false;
            RadioDualDataEntryMask1.Checked = false;
            RadioDualDataEntryMask2.Checked = false;
            RadioDualDataEntryMaskBoth.Checked = false;
            RadioDualDataEntryNone.Checked = false;
            RadioAutoAge.Checked = false;
            RadioCurrentDate.Checked = false;
            RadioReferDate.Checked = false;
            foreach (ListItem item in RadioFeildDataType.Items)
            {
                item.Selected = false;
            }
        }

        private void DisableControls()
        {
            txtFieldName.Enabled = false;
            txtLabelText.Enabled = false;
            txtMandatoryErrorMessage.Enabled = false;
            txtMax.Enabled = false;
            txtMin.Enabled = false;
            CheckActive.Enabled = false;
            CheckDisplay.Enabled = false;
            chkEditable.Enabled = false;
            CheckMandatory.Enabled = false;
            chkShowInDashboard.Enabled = false;  /*DMS04-3877 A*/            
            chkRemarks.Enabled = false;    //DMS5-4423 
            RadioSingleValueMain.Enabled = false;
            RadioMultiValueMain.Enabled = false;
            RadioSingleValueSub.Enabled = false;
            RadioMultiValueSub.Enabled = false;
            RadioSingle.Enabled = false;
            RadioFeildDataType.Enabled = false;
            DDLMultiValueMain.Enabled = false;
            DDLMultiValueSub1.Enabled = false;
            DDLMultiValueSub2.Enabled = false;

            RadioAutoAge.Enabled = false;
            RadioAutoCalculateFeild.Enabled = false;
            RadioImageUpload.Enabled = false;
            RadioCurrentDate.Enabled = false;
            RadioReferDate.Enabled = false;
            RadioDualDataEntry.Enabled = false;
            RadioDualDataEntryMask1.Enabled = false;
            RadioDualDataEntryMask2.Enabled = false;
            RadioDualDataEntryMaskBoth.Enabled = false;
            RadioDualDataEntryNone.Enabled = false;

            // DMSENH6-4732 BS
            btnCaptureCoordinates.Enabled = false;
            btnSave.Enabled = false;
            // DMSENH6-4732 BE

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
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            CheckAuthentication();
            string ProcessId = Session["ProcessId"].ToString(); //ProcessId
            string WorkflowId = Session["WorkflowId"].ToString(); //WorkflowId
            string StageId = Session["StageId"].ToString();
            ProcessId = HttpUtility.UrlEncode(Encrypt(ProcessId));
            WorkflowId = HttpUtility.UrlEncode(Encrypt(WorkflowId));
            StageId = HttpUtility.UrlEncode(Encrypt(StageId));
            string Querystring = "?ProcessId=" + ProcessId + "&WorkflowId=" + WorkflowId + "&StageId=" + StageId;
            Session.Remove("FieldId");
            Response.Redirect("~/Workflow/WorkflowAdmin/ManageStageFeildsList.aspx" + Querystring);
        }

        protected void BindDDLMultiValueMain()
        {
            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            objStageField.LoginToken = loginUser.LoginToken;
            objStageField.LoginOrgId = loginUser.LoginOrgId;
            objStageField.WorkflowStageFieldsProcessId = Convert.ToInt32(Session["ProcessId"].ToString());
            objStageField.WorkflowStageFieldsWorkflowId = Convert.ToInt32(Session["WorkflowId"].ToString());
            if (Session["FieldId"] != null && Session["FieldId"].ToString() == "0")
            {
                objStageField.WorkflowStageFieldsStageId = Convert.ToInt32(Session["StageId"].ToString());
            }
            else
            {
                objStageField.WorkflowStageFieldsStageId = Convert.ToInt32(Session["FieldStageId"].ToString());
            }
            objStageField.WorkflowStageFieldsId = Convert.ToInt32(Session["FieldId"].ToString());
            objResult = objStageField.ManageStageFields(objStageField, "BindMultiValueMainDropDown");
            if (objResult.dsResult.Tables != null && objResult.dsResult.Tables[0].Rows.Count > 0)
            {
                DDLMultiValueMain.DataSource = objResult.dsResult.Tables[0];
            }
            DDLMultiValueMain.DataTextField = "WorkflowMasterTypes_vName";
            DDLMultiValueMain.DataValueField = "WorkflowMasterTypes_iId";
            DDLMultiValueMain.DataBind();

            DDLMultiValueSub1.DataSource = objResult.dsResult.Tables[1];
            DDLMultiValueSub1.DataTextField = "WorkflowMasterTypes_vName";
            DDLMultiValueSub1.DataValueField = "WorkflowStageFields_iId"; //assign parent field id. store the data table in session for furure use
            DDLMultiValueSub1.DataBind();
            Session["MultiValueParent"] = objResult.dsResult.Tables[1];
        }

        protected void BindDDLMultiValueSub()
        {
            string selectedParentText = DDLMultiValueSub1.SelectedItem.Text;
            if (Convert.ToInt32(DDLMultiValueSub1.SelectedValue) > 0)
            {
                selectedParentText = selectedParentText.Substring(selectedParentText.IndexOf("[") + 1, selectedParentText.IndexOf("]") - selectedParentText.IndexOf("[") - 1);
            }
            DataTable dt = (DataTable)Session["MultiValueParent"];
            DataRow[] dr = dt.Select("WorkflowStageFields_iId =" + DDLMultiValueSub1.SelectedValue);
            string strMasterType = dr[0]["WorkflowMasterTypes_iId"].ToString();
            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            objStageField.LoginToken = loginUser.LoginToken;
            objStageField.LoginOrgId = loginUser.LoginOrgId;
            objStageField.WorkflowStageFieldsProcessId = Convert.ToInt32(Session["ProcessId"].ToString());
            objStageField.WorkflowStageFieldsWorkflowId = Convert.ToInt32(Session["WorkflowId"].ToString());
            objStageField.WorkflowStageFieldsStageId = Convert.ToInt32(Session["StageId"].ToString());
            objStageField.WorkflowStageFieldsMasterType = Convert.ToInt32(strMasterType);
            objStageField.WorkflowStageFieldsName = selectedParentText; //Parent rows name extracted from the drop down
            objResult = objStageField.ManageStageFields(objStageField, "BindMultiValueSubDropDown");
            DDLMultiValueSub2.DataSource = objResult.dsResult.Tables[0];
            DDLMultiValueSub2.DataTextField = "WorkflowMasterTypes_vName";
            DDLMultiValueSub2.DataValueField = "WorkflowMasterTypes_iId";
            DDLMultiValueSub2.DataBind();
            try
            {
                hdnParentRowId.Value = objResult.dsResult.Tables[1].Rows[0]["WorkflowStageFields_iParentId"].ToString();
            }
            catch
            {
                hdnParentRowId.Value = "0";
            }
        }

        protected void DDLMultiValueSub1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckAuthentication();
            BindDDLMultiValueSub();
        }

 //Load ReferDate DropDown Based on Checked Status
        protected void RadioReferDate_CheckedChanged(object sender, EventArgs e)
{
            try
            {
                if (RadioReferDate.Checked == true)
                {
                    CheckAuthentication();
                    DBResult objResult = new DBResult();
                    UserBase loginUser = (UserBase)Session["LoggedUser"];
                    objStageField.LoginToken = loginUser.LoginToken;
                    objStageField.LoginOrgId = loginUser.LoginOrgId;
                    objStageField.WorkflowStageFieldsProcessId = Convert.ToInt32(Session["ProcessId"].ToString());
                    objStageField.WorkflowStageFieldsWorkflowId = Convert.ToInt32(Session["WorkflowId"].ToString());
                    objStageField.WorkflowStageFieldsStageId = Convert.ToInt32(Session["StageId"]);
                    objResult = objStageField.ManageStageFields(objStageField, "BindAllDateFields");


                    DDLReferDate.DataSource = objResult.dsResult.Tables[0];
                    DDLReferDate.DataTextField = "WorkflowStageFields_vName";
                    DDLReferDate.DataValueField = "WorkflowStageFields_iId";
                    DDLReferDate.DataBind();

                }

                
            }

            catch (Exception ex)
            {
                
            }
        }
		
		protected void RadioAutoCalculateFeild_CheckedChanged(object sender, EventArgs e)
        {
            LoadStageFeildNamesToListBox();
            LoadOperatorsToListBox();
            ModalPopupExtenderFormula.TargetControlID = "RadioAutoCalculateFeild";
            ModalPopupExtenderFormula.Show();

            //DMS5-3972,DMS5-3971 BS
            if (Session["DynamicControls"] != null)
            {
                Session.Remove("DynamicControls");
                PlaceHolderFormula.Controls.Clear();
            }
            //DMS5-3972,DMS5-3971 BE
         
        }
		
		
        //To loada  all stagefeilds with datatype in number
        protected void LoadStageFeildNamesToListBox()
        {
            try
            {
                //if (RadioAutoCalculateFeild.Checked == true)
                //{
                    DBResult objResult = new DBResult();
                    CheckAuthentication();
                    UserBase loginUser = (UserBase)Session["LoggedUser"];
                    objStageField.LoginToken = loginUser.LoginToken;
                    objStageField.LoginOrgId = loginUser.LoginOrgId;
                    objStageField.WorkflowStageFieldsProcessId = Convert.ToInt32(Session["ProcessId"].ToString());
                    objStageField.WorkflowStageFieldsWorkflowId = Convert.ToInt32(Session["WorkflowId"].ToString());
                    objStageField.WorkflowStageFieldsStageId = Convert.ToInt32(Session["StageId"]);
                    objResult = objStageField.ManageStageFields(objStageField, "BindStageFieldNamesToListBox");
                    lstStageFeildNames.DataSource = objResult.dsResult.Tables[0];
                    lstStageFeildNames.DataTextField = "WorkflowStageFields_vName";
                    lstStageFeildNames.DataValueField = "WorkflowStageFields_iId";
                    lstStageFeildNames.DataBind();


                //}
            }
            catch (Exception ex)
            {
              
            }

        }
		   //to load operators to listboxes
        protected void LoadOperatorsToListBox()
        {
            try
            {
                //if (RadioAutoCalculateFeild.Checked == true)
                //{
                    DBResult objResult = new DBResult();
                    objResult = objStageField.ManageStageFields(objStageField, "BindOperatorsToListBox");
                    lstOperators.DataSource = objResult.dsResult.Tables[0];
                    lstOperators.DataTextField = "Operators";
                    lstOperators.DataBind();

                //}
            }
            catch (Exception ex)
			{
               
            }
		}
		
        //To Perform formula
		 protected void TestFormula(object sender, EventArgs e)
        {

             //Validations for input formula
            string formula = txtFormulaArea.Text.Trim();
            lblErrorFormula.Text = string.Empty;
            string[] split = formula.Split(new Char[] { '+', '-', '(' ,')','*','/' },
                                  StringSplitOptions.RemoveEmptyEntries);
                        
            try
            {
                foreach (string word in split)
                {
                    var match = lstStageFeildNames.Items
                        .Cast<object>()
                        .FirstOrDefault(x => x.ToString() == word);

                     referredIds += string.Concat(((ListItem)(match)).Value,",");
                     ViewState["FormulaIds"] = referredIds;
                   
                    if (match == null)
                    {
                        lblErrorFormula.Text = "Please check the formula formed is correct with respect to field names or operators.";
                    }
                    
				}
                
            }
            catch (Exception ex)
            {
			}
          
            if(lblErrorFormula.Text == string.Empty)
            {
            try{
                HtmlTable tbl = new HtmlTable();

                if (txtFormulaArea.Text.Trim().Length > 0)
                {
                   
                    ModalPopupExtenderFormula.Show();
                    //FormulaArea.Style.Add("visibility", "hidden");
                    var inputFormula = txtFormulaArea.Text.Trim();

                    //Split the input formula to generate dynamic controls associate field name
                    List<string> list = new List<string>(SplitInputFormula(inputFormula));

                    //DMS5-3972,DMS5-3971 BS
                    if (Session["DynamicControls"] != null)
                    {
                        Session.Remove("DynamicControls");
                        PlaceHolderFormula.Controls.Clear();
                    }
                    //DMS5-3972,DMS5-3971 BE

                    try
                    {
                        //creating table for alignment and add dynamic controls
                        foreach (string item in list)
                        {
                            HtmlTableRow row = new HtmlTableRow();

                            HtmlTableCell cell1 = new HtmlTableCell();
                            HtmlTableCell cell2 = new HtmlTableCell();
                            row.Cells.Add(cell1);
                            row.Cells.Add(cell2);

                            Label lbl = new Label();
                            cell1.Controls.Add(lbl);
                            lbl.ID = "lbl" + item;
                            lbl.Text = item;

                            TextBox txt = new TextBox();
                            txt.ID = "txt" + item;
                            txt.ViewStateMode = System.Web.UI.ViewStateMode.Enabled;
                            txt.Attributes.Add("onkeypress", "return inputOnlyNumbers(event);");
                            cell2.Controls.Add(txt);

                            tbl.Rows.Add(row);
                        }
                    }
                    catch { }

                    PlaceHolderFormula.Controls.Add(tbl);
                    Session["DynamicControls"] = tbl;
                    
                }
                else
                {
                    ModalPopupExtenderFormula.Show();
                    lblErrorFormula.Text = "Formula is empty.";
                   
                }

            }
          
            catch (Exception exception)
            {
 
            }
            }
            ModalPopupExtenderFormula.Show();
            btnExecute.Enabled = true;
        }
		
        //Executing the formula
		 protected void btnExecute_Click(object sender, EventArgs e)
        {

            try
            {
                ModalPopupExtenderFormula.Show();
                if (txtFormulaArea.Text.Trim().Length > 0)
                {
                    lblErrorFormula.Text = string.Empty;
                    string originalFormula = txtFormulaArea.Text.Trim();
                    List<string> formulaFeilds = SplitInputFormula(originalFormula).ToList();

                    // Sort list by its value length.
                    // Sorting by length is require as replace should not fail.
                    // Ex: Num, Num1, Num12: Num cannot be replaced, need to replace max length value first.

                    IEnumerable<string> query = from word in formulaFeilds
                                                orderby word.Length descending
                                                select word;
                    formulaFeilds = query.ToList();

                    string txtValue = string.Empty;

                    foreach (string item in formulaFeilds)
                    {
                        TextBox txtControlId = FindChildControl(Page, ("txt" + item)) as TextBox;
                        if (txtControlId != null)
                        {

                            txtValue = txtControlId.Text.Trim();
                            originalFormula = originalFormula.Replace(item, txtValue);

                        }
                    }

                    txtFormulResult.Text = Convert.ToString(EvaluateExpression(originalFormula.Trim()));
                  
                }
                else
                {
                    ModalPopupExtenderFormula.Show();
                    lblErrorFormula.Text = "Formula is empty.";
                   
                }
            }
            catch (Exception Ex)
            {
 
            }
           
                  Session.Remove("DynamicControls");
        }
		
		//To perform Calculation of formula
        private object EvaluateExpression(string eqn)
        {
            var result = (object)null;
            try
            {
                DataTable dt = new DataTable();
                result = dt.Compute(eqn, string.Empty);
                lblErrorFormula.Text = string.Empty;
            }
            catch (SyntaxErrorException syntaxerror)
            {
                lblErrorFormula.Text = "Please Check the formula " + "<br/>"  + syntaxerror.Message;
            }


            return result;
        }

        //Split the input formula without spaces
        protected List<string> SplitInputFormula(string input)
        {
            string[] Output = input.Split("/*-+()".ToCharArray());
            List<string> list = new List<string>(Output);
            //To remove empty values in string arrays
            list = list.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();

            return list;
        }

        public static Control FindChildControl(Control start, string id)
        {
            if (start != null)
            {
                Control foundControl;
                foundControl = start.FindControl(id);


                if (foundControl != null)
                {
                    return foundControl;
                }


                foreach (Control c in start.Controls)
                {
                    foundControl = FindChildControl(c, id);
                    if (foundControl != null)
                    {
                        return foundControl;
                    }
                }
            }
            return null;
        }

        //image close click event
        protected void imgClosePopUp_Click(object sender, EventArgs e)
        {
            RadioAutoCalculateFeild.Checked = true;
        }

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
            if (Session["DataEntryTypeId"] != null && Convert.ToString(Session["DataEntryTypeId"]) != string.Empty)
            {
                objStage.DataEntryId = Convert.ToInt32(Session["DataEntryTypeId"]);
            }
            if (Session["ProcessId"] != null && Convert.ToString(Session["ProcessId"]) != string.Empty)
            {
                objStage.ProcessId = Convert.ToInt32(Session["ProcessId"]);
            }
            if (Session["WorkflowId"] != null && Convert.ToString(Session["WorkflowId"]) != string.Empty)
            {
                objStage.WorkflowId = Convert.ToInt32(Session["WorkflowId"]);
            }
            if (Session["StageId"] != null && Convert.ToString(Session["StageId"]) != string.Empty)
            {
                objStage.StageId = Convert.ToInt32(Session["StageId"]);
            }

            return objStage;
        }

        /// <summary>
        /// getting stage field related values from session
        /// </summary>
        /// <returns></returns>
        public WorkflowStageField GetStageFieldValues()
        {
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            objStageField.LoginToken = loginUser.LoginToken;
            objStageField.LoginOrgId = loginUser.LoginOrgId;
            objStageField.WorkflowStageFieldsProcessId = Session["ProcessId"] != null ? Convert.ToInt32(Session["ProcessId"].ToString()) : 0;
            objStageField.WorkflowStageFieldsWorkflowId = Session["WorkflowId"] != null ? Convert.ToInt32(Session["WorkflowId"].ToString()) : 0;
            if (Session["FieldId"] != null && Session["FieldId"].ToString() == "0")
            {
                objStageField.WorkflowStageFieldsStageId = Convert.ToInt32(Session["StageId"].ToString());
            }
            else
            {
                objStageField.WorkflowStageFieldsStageId = Convert.ToInt32(Session["FieldStageId"].ToString());
            }
            objStageField.WorkflowStageFieldsId = Session["FieldId"] != null ? Convert.ToInt32(Session["FieldId"].ToString()) : 0;
            return objStageField;
        }

        /// <summary>
        /// Load page dropdownlist
        /// </summary>
        /// <param name="ds"></param>

        /// <summary>
        /// set the coordinates from hidden fields
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //DMS5-3426BS
        protected void btnClose_Click(object sender, EventArgs e)
        {
            MainDiv.Visible = true;
            MainDiv.Attributes.Add("class", "GVDiv");
            DivImage.Visible = false;
            txtX1.Text = hdnX1.Value;
            txtY1.Text = hdnY1.Value;
            txtX2.Text = hdnX2.Value;
            txtY2.Text = hdnY2.Value;            
            txtPageNumber.Text = hdnPageNo.Value;
        }
        //DMS5-3426BE

        /// <summary>
        /// To load the image based on page selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        // DMS5-3421BS
        protected void btnCallFromJavascript_Click(object sender, EventArgs e)
        {
            int pagecount = Convert.ToInt32(hdnPagesCount.Value);
            string src = string.Empty;
            int CurrentPage = 1;
            if (hdnPageNo.Value.Length > 0)
                CurrentPage = Convert.ToInt32(hdnPageNo.Value);
            Logger.Trace("Setting image to viewer. Action: " + hdnAction.Value.ToString() + " Page count: " + pagecount + " Current Page:" + CurrentPage, Session["LoggedUserId"].ToString());
            int PageNo = 0;
            switch (hdnAction.Value.ToUpper())
            {
                case "NEXT":
                    PageNo = CurrentPage + 1;
                    break;
                case "PREVIOUS":
                    PageNo = CurrentPage - 1;
                    break;
                case "GOTO":
                    // PageNo = Convert.ToInt32(DDLDrop.SelectedValue);
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
                DDLDrop.SelectedValue = PageNo.ToString();
                BindCoodinateImage();
            }
            else
                PageNo = CurrentPage;
        }


        /// <summary>
        /// To load the jpeg image based on page dropdown change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDDLDrop_Click(object sender, EventArgs e)
        {
            int pagecount = Convert.ToInt32(hdnPagesCount.Value);
            int PageNo = 0;
            int CurrentPage = 1;
            CurrentPage = Convert.ToInt32(DDLDrop.SelectedValue);
            PageNo = CurrentPage;
            if (PageNo > 0 && PageNo <= pagecount)
            {
                hdnPageNo.Value = PageNo.ToString();
                BindCoodinateImage(); //load image based on page selected
            }
        }
        //DMS5-3484BS enabling the div on button click
        protected void btnCaptureCoordinates_Click(object sender, EventArgs e)
        {
            DivImage.Visible = true;
            lblStageDescription.Text = (Session["StageName"] != null) ? Convert.ToString(Session["StageName"]) : string.Empty;

            lblFieldDescription.Text = txtFieldName.Text;
            MainDiv.Attributes.Add("class", "mdBG");
        }

        //assigning the coordinate textbox with zero when the button is clicked
        protected void btnCancelCrop_Click(object sender, EventArgs e)
        {
            MainDiv.Visible = true;
            MainDiv.Attributes.Add("class", "GVDiv");
            DivImage.Visible = false;
            txtX1.Text = "0";
            txtX2.Text = "0";
            txtY1.Text = "0";
            txtY2.Text = "0";
            txtPageNumber.Text = "0";
            hdnResolution.Value = string.Empty;
            hdnHeight.Value = string.Empty;
            hdnWidth.Value = string.Empty;
        }
        //DMS5-3484BE      

        // DMS5-3847BS
        private void initializeCoordinateCapture()
        {
            try
            {
                //variable declaration   
                DBResult objResult = new DBResult();

                string templateFilePath = string.Empty;
                string fileExtension = string.Empty;
                string templateFolderPath = string.Empty;
                hdnPageNo.Value = "1";

                //getting values from session
                objStage = GetValuesFromSession();

                //Get the physical path from database based on which we need to create folder for splitting into image.               
                objResult = objStage.ManageWorkflowStages(objStage, "GetStageDetails");
                if (objResult.dsResult != null && objResult.dsResult.Tables.Count > 0 && objResult.dsResult.Tables[0].Rows.Count > 0)
                {
                    templateFilePath = objResult.dsResult.Tables[0].Rows[0]["WorkflowStage_vTemplatePath"].ToString();
                    hdnShowBackgroundImage.Value = objResult.dsResult.Tables[0].Rows[0]["Workflowstage_bShowBackgroundImage"].ToString();
                }

                Logger.Trace("TemplatefilePath from database : " + templateFilePath, Session["LoggedUserId"].ToString());
                fileExtension = afterdot(templateFilePath);
                hdnFielExtension.Value = fileExtension;//inorder to get the extension for loading pages dropdownlist

                //DMS5-3383BS -- Rewritten 
                if (templateFilePath.Length > 0)
                {
                    templateFilePath = templateFilePath.Replace(@"\", "/");
                    templateFolderPath = Path.GetDirectoryName(templateFilePath);
                    hdnTemplateFolderPath.Value = templateFolderPath;
                }
                
                //Find the total pages count 
                if (fileExtension.ToLower() == ".pdf")
                {
                    // if pdf file was uploaded, then it has been splitted and converted to jpg.
                    hdnPagesCount.Value = System.IO.Directory.GetFiles(templateFolderPath, "*.jpg").Length.ToString();
                    fileExtension = ".jpg";
                }
                else
                {
                    hdnPagesCount.Value = "1";
                }
                //DMS5-3383BE

                //Bind the page dropdown 
                objStage.TotalPages = (hdnPagesCount.Value != null && hdnPagesCount.Value.Length > 0) ? Convert.ToInt32(hdnPagesCount.Value) : 1;
                DDLDrop.Items.Clear();
                for (int pageNo = 1; pageNo <= objStage.TotalPages; pageNo++)
                    DDLDrop.Items.Add(new ListItem(Convert.ToString(pageNo), Convert.ToString(pageNo)));

                //Load image
                Logger.Trace("Loading image based on page number for coordinate capturing", Session["LoggedUserId"].ToString());
                BindCoodinateImage();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error occured: " + ex.Message;
                Logger.Trace("Exception: " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        /// <summary>
        /// Bind the image to viewer
        /// </summary>
        private void BindCoodinateImage()
        {
            //DMS5-3383BS - Rewritten
            string fileExtension = string.Empty;
            string imageUrl = string.Empty;
            bool showBackgroundImage = true;
            string imageFolderPath = Convert.ToString(hdnTemplateFolderPath.Value);

            fileExtension = (hdnFielExtension.Value != null && hdnFielExtension.Value.Length > 0) ? Convert.ToString(hdnFielExtension.Value) : string.Empty;
            fileExtension = fileExtension.ToLower() == ".pdf" ? ".jpg" : fileExtension;

            //Using handler path to bind images source            
            imageUrl = GetSrc("Handler") + imageFolderPath + @"\";
            imageUrl = imageUrl + @"/" + hdnPageNo.Value + fileExtension;
            Logger.Trace("imageUrl for binding the viewer: " + imageUrl, Session["LoggedUserId"].ToString());

            if (hdnDataType.Value == "Guided")
            {
                //Load the image from the solution after splitting the file into jpeg                
                divcropbox.Visible = divpreview.Visible = true;
                cropbox.Src = preview.Src = imageUrl;

                divFormCropbox.Visible = divFormPreview.Visible = false;
            }
            else if (hdnDataType.Value == "Form")
            {
                divcropbox.Visible = divpreview.Visible = false;
                divFormCropbox.Visible = divFormPreview.Visible = FormPreview.Visible = true;
                FormCropbox.Src = FormPreview.Src = imageUrl;

                showBackgroundImage = (hdnShowBackgroundImage.Value != null && hdnShowBackgroundImage.Value.Length > 0) ? Convert.ToBoolean(hdnShowBackgroundImage.Value) : true;
                Logger.Trace("status of the back ground image is: " + showBackgroundImage, Session["LoggedUserId"].ToString());
                if (!showBackgroundImage)
                    FormPreview.Attributes.Add("Class", "Image");

            }
            //DMS5-3383BE
        }
        // DMS5-3847BE

    }
}