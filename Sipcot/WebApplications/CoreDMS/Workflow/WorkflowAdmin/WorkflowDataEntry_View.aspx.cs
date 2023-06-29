
/* ============================================================================  
Author     : Robbin Thomas
Create date: 
===============================================================================  
** Change History   
** Date:          Author:            Issue ID           Description:  
** ----------   -------------       ----------          ----------------------------
*17 Apr 2015      Gokul               DMS5-3946       Applying rights an permissions in all pages as per user group rights!
 *08 Apr 2015     Sabina              DMS5-4122       Button not enabled in workflow pages
=============================================================================== */

using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WorkflowBAL;
using WorkflowBLL.Classes;
using System.Data;
using System.Web.UI.HtmlControls;
using Lotex.EnterpriseSolutions.CoreBE;
using OfficeConverter;
using Ionic.Zip;
using System.IO;

namespace Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin
{
    public partial class WorkFlowDataEntry_View : PageBase
    {
        WorkflowDataEntryBLL ObjDataEntry = new WorkflowDataEntryBLL();
        public string pageRights = string.Empty; /* DMS5-3946 A */        
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
                    string ProcessId, WorkflowId, StageId, FieldDataId, PageMode = string.Empty;
                    ProcessId = Decrypt(HttpUtility.UrlDecode(Request.QueryString["ProcessId"]));
                    WorkflowId = Decrypt(HttpUtility.UrlDecode(Request.QueryString["WorkflowId"]));
                    StageId = Decrypt(HttpUtility.UrlDecode(Request.QueryString["StageId"]));
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
                        DDLStatus.Enabled = false;
                    }
                    else
                    {
                        btnSave.Visible = true;
                        btnCancel.Visible = true;
                        btnClose.Visible = false;
                    }

                    Session["ProcessId"] = ProcessId;
                    Session["WorkflowId"] = WorkflowId;
                    Session["StageId"] = StageId;
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
            }
            catch(Exception ex)
            {
                lblMessage.Text = ex.Message.ToString();
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

        private string GenerateFieldSaveQuery()
        {

            DBResult controlDataset;
            controlDataset = (DBResult)Session["WorkflowControlDetails"];
            DataTable dtCurFields = controlDataset.dsResult.Tables[1];
            DataTable dtPrevFields = controlDataset.dsResult.Tables[2];

            string strTableName = string.Empty;
            string strFieldName = string.Empty;
            string strUpdateQuery = string.Empty;
            string strFieldValue = string.Empty;


            for (int i = 0; i < dtCurFields.Rows.Count; i++)
            {
                strTableName = dtCurFields.Rows[i]["WorkflowStageFields_vTable"].ToString();
                strFieldName = dtCurFields.Rows[i]["WorkflowStageFields_vDBFld"].ToString();

                if (strUpdateQuery.Length == 0)
                {
                    strUpdateQuery = "UPDATE " + strTableName + " SET ";
                }

                if (dtCurFields.Rows[i]["WorkflowStageFields_cObjType"].ToString() == "TextBox")
                {

                    TextBox txtName = (TextBox)this.ControlPlaceHolder_CurStage.FindControl(dtCurFields.Rows[i]["WorkflowStageFields_ControlID"].ToString());
                    if (txtName != null)
                    {
                        strFieldValue = txtName.Text;
                    }

                }
                else if (dtCurFields.Rows[i]["WorkflowStageFields_cObjType"].ToString() == "DropDown")
                {

                    DropDownList ddlField = (DropDownList)this.ControlPlaceHolder_CurStage.FindControl(dtCurFields.Rows[i]["WorkflowStageFields_ControlID"].ToString());
                    if (ddlField != null)
                    {
                        strFieldValue = ddlField.SelectedValue;
                    }

                }
                strUpdateQuery += strFieldName + "='" + strFieldValue + "', ";

            }

            for (int i = 0; i < dtPrevFields.Rows.Count; i++)
            {
                strTableName = dtPrevFields.Rows[i]["WorkflowStageFields_vTable"].ToString();
                strFieldName = dtPrevFields.Rows[i]["WorkflowStageFields_vDBFld"].ToString();

                if (strUpdateQuery.Length == 0)
                {
                    strUpdateQuery = "UPDATE " + strTableName + " SET ";
                }

                if (dtPrevFields.Rows[i]["WorkflowStageFields_cObjType"].ToString() == "TextBox")
                {

                    TextBox txtName = (TextBox)this.ControlPlaceHolder_PrevStage.FindControl(dtPrevFields.Rows[i]["WorkflowStageFields_ControlID"].ToString());
                    if (txtName != null)
                    {
                        strFieldValue = txtName.Text;
                    }

                }
                else if (dtPrevFields.Rows[i]["WorkflowStageFields_cObjType"].ToString() == "DropDown")
                {

                    DropDownList ddlField = (DropDownList)this.ControlPlaceHolder_PrevStage.FindControl(dtPrevFields.Rows[i]["WorkflowStageFields_ControlID"].ToString());
                    if (ddlField != null)
                    {
                        strFieldValue = ddlField.SelectedValue;
                    }
                }

                strUpdateQuery += strFieldName + "='" + strFieldValue + "', ";
            }

            strUpdateQuery += "WorkflowStageFieldData_iStatusID =" + DDLStatus.SelectedValue + ",WorkflowStageFieldData_iProcessID =" + Convert.ToInt32(Session["ProcessId"]) +
                              ",WorkflowStageFieldData_iWorkFlowID =" + Convert.ToInt32(Session["WorkflowId"]) +
                " WHERE WorkflowStageFieldData_iID =" + Session["FieldDataId"].ToString();

            return strUpdateQuery;
        }

        protected void btnSave_Clcik(object sender, EventArgs e)
        {

            DBResult Objresult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            ObjDataEntry.WorkflowDataEntryProcessId = Convert.ToInt32(Session["ProcessId"]);
            ObjDataEntry.WorkflowDataEntryWorkflowId = Convert.ToInt32(Session["WorkflowId"]);
            ObjDataEntry.WorkflowDataEntryStageId = Convert.ToInt32(Session["StageId"]);
            ObjDataEntry.LoginOrgId = loginUser.LoginOrgId;
            ObjDataEntry.LoginToken = loginUser.LoginToken;
            ObjDataEntry.WorkflowDataEntryFieldDataId = Convert.ToInt32(Session["FieldDataId"]);
            string strUpdateQuery = GenerateFieldSaveQuery();
            ObjDataEntry.WorkflowDataEntryXmlData = strUpdateQuery;
            Objresult = ObjDataEntry.ManageWorkflowDataEntry(ObjDataEntry, "SaveFieldData");

            hdnSaveStatus.Value = "";
            Session.Remove("WorkflowControlDetails");
            Response.Redirect("~/Workflow/WorkflowAdmin/WorkflowPendingList.aspx", false);
        }

        protected void BindDropDownList(DropDownList ddl, int masterTypeId)
        {
            DBResult Objresult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            ObjDataEntry.WorkflowDataEntryProcessId = Convert.ToInt32(Session["ProcessId"]);
            ObjDataEntry.WorkflowDataEntryWorkflowId = Convert.ToInt32(Session["WorkflowId"]);
            ObjDataEntry.WorkflowDataEntryStageId = Convert.ToInt32(Session["StageId"]);
            ObjDataEntry.LoginOrgId = loginUser.LoginOrgId;
            ObjDataEntry.LoginToken = loginUser.LoginToken;
            ObjDataEntry.WorkflowDataEntryMasterTypeId = masterTypeId;

            Objresult = ObjDataEntry.ManageWorkflowDataEntry(ObjDataEntry, "GetDropDownData");
            ddl.DataSource = Objresult.dsResult;
            ddl.DataTextField = "WorkflowMasterValues_vName";
            ddl.DataValueField = "WorkflowMasterValues_iId";
            ddl.DataBind();
        }

        protected void BindChildDropDownList(DropDownList ddl, int masterTypeId, int masterTypeVal)
        {
            DBResult Objresult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            ObjDataEntry.WorkflowDataEntryProcessId = Convert.ToInt32(Session["ProcessId"]);
            ObjDataEntry.WorkflowDataEntryWorkflowId = Convert.ToInt32(Session["WorkflowId"]);
            ObjDataEntry.WorkflowDataEntryStageId = Convert.ToInt32(Session["StageId"]);
            ObjDataEntry.LoginOrgId = loginUser.LoginOrgId;
            ObjDataEntry.LoginToken = loginUser.LoginToken;
            ObjDataEntry.WorkflowDataEntryMasterTypeId = masterTypeId;
            ObjDataEntry.WorkflowDataEntryParentId = masterTypeVal;

            Objresult = ObjDataEntry.ManageWorkflowDataEntry(ObjDataEntry, "GetChildDropDownData");
            ddl.DataSource = Objresult.dsResult;
            ddl.DataTextField = "WorkflowMasterValues_vName";
            ddl.DataValueField = "WorkflowMasterValues_iId";
            ddl.DataBind();
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
            ControlPlaceHolder_CurStage.Controls.Clear();
            ControlPlaceHolder_PrevStage.Controls.Clear();

            DBResult Objresult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            string strImagePath = string.Empty;

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
                Objresult = ObjDataEntry.ManageWorkflowDataEntry(ObjDataEntry, "GetWorkflowControlDetailsVIEW");
            }

            if (Objresult.dsResult != null && Objresult.dsResult.Tables.Count > 0 && Objresult.dsResult.Tables[0].Rows.Count > 0)
            {
                Session["WorkflowControlDetails"] = Objresult; //for later use

                DataRow Row = Objresult.dsResult.Tables[0].Rows[0];
                lblProcessDescription.Text = Row["WorkflowProcess_vName"].ToString();
                lblWorkflowDescription.Text = Row["Workflow_vName"].ToString();
                lblStageDescription.Text = Row["WorkflowStage_vDisplayName"].ToString();

                lblUserDescription.Text = loginUser.UserName;
                lblDateofReceiptDescription.Text = Convert.ToDateTime(Objresult.dsResult.Tables[3].Rows[0]["WorkflowStageFieldData_dModifiedOn"].ToString()).ToString("dd-MMM-yyyy hh:mm tt");
            }
            else
            {
                ControlPlaceHolder_CurStage.Visible = false;
            }

            string mandatoryValidationScript = string.Empty;
            string SpecialCharValidationScript = string.Empty;
            string MaxValidationScript = string.Empty;

            if (Objresult.dsResult != null && Objresult.dsResult.Tables.Count > 0 && Objresult.dsResult.Tables[1].Rows.Count > 0)
            {
                Table FieldTable1 = new Table();
                FieldTable1.Enabled = false;

                foreach (DataRow dr in Objresult.dsResult.Tables[1].Rows)
                {
                    TableRow tr = new TableRow();
                    TableCell tc1 = new TableCell();
                    TableCell tc2 = new TableCell();
                    if (dr["WorkflowStageFields_cObjType"].ToString() == "DropDown")
                    {
                        Label lbl = new Label();
                        lbl.ID = dr["WorkflowStageFields_ControlID"].ToString() + "_Label";
                        lbl.Text = dr["WorkflowStageFields_vLabelText"].ToString();
                        tc1.Width = Unit.Pixel(150);
                        tc1.Controls.Add(lbl);

                        DropDownList ddl = new DropDownList();
                        ddl.ID = dr["WorkflowStageFields_ControlID"].ToString();
                        tc2.Controls.Add(ddl);

                        if (dr["WorkflowStageFields_iParentId"].ToString() == "0")
                        {
                            BindDropDownList(ddl, Convert.ToInt32(dr["WorkflowStageFields_iMasterType"].ToString()));
                        }


                        try
                        {
                            ddl.SelectedValue = Objresult.dsResult.Tables[3].Rows[0][dr["WorkflowStageFields_vDBFld"].ToString()].ToString();
                        }
                        catch
                        {
                            ddl.SelectedValue = "0";
                        }

                        if (dr["WorkflowStageFields_bMandatory"].ToString() == "True")
                        {
                            Label lblAstreick = new Label();
                            lblAstreick.ForeColor = System.Drawing.Color.Red;
                            lblAstreick.Text = "&nbsp;*";
                            lblAstreick.Width = Unit.Pixel(50);
                            lblAstreick.Style.Add("font-size", "large");
                            tc2.Controls.Add(lblAstreick);

                            if (mandatoryValidationScript != "")
                            {
                                mandatoryValidationScript += "##";
                            }
                            mandatoryValidationScript += ddl.ClientID.ToString() + "|DropDown";
                        }




                    }

                    else if (dr["WorkflowStageFields_cObjType"].ToString() == "TextBox")
                    {
                        Label lbl = new Label();
                        lbl.ID = dr["WorkflowStageFields_ControlID"].ToString() + "_Label";
                        lbl.Text = dr["WorkflowStageFields_vLabelText"].ToString();
                        tc1.Width = Unit.Pixel(150);
                        tc1.Controls.Add(lbl);

                        TextBox txt = new TextBox();
                        txt.ID = dr["WorkflowStageFields_ControlID"].ToString();
                        tc2.Controls.Add(txt);

                        txt.Text = Objresult.dsResult.Tables[3].Rows[0][dr["WorkflowStageFields_vDBFld"].ToString()].ToString();


                        if (dr["WorkflowStageFields_bMandatory"].ToString() == "True")
                        {
                            Label lblAstreick = new Label();
                            lblAstreick.ForeColor = System.Drawing.Color.Red;
                            lblAstreick.Text = "&nbsp;*";
                            lblAstreick.Width = Unit.Pixel(50);
                            lblAstreick.Style.Add("font-size", "large");
                            tc2.Controls.Add(lblAstreick);

                            if (mandatoryValidationScript != "")
                            {
                                mandatoryValidationScript += "##";
                            }
                            mandatoryValidationScript += txt.ClientID.ToString() + "|TextBox";
                        }

                        if (dr["WorkflowStageFields_vDBType"].ToString() == "String")
                        {
                            if (SpecialCharValidationScript != "")
                            {
                                SpecialCharValidationScript += "##";
                            }
                            SpecialCharValidationScript += txt.ClientID.ToString() + "|TextBox";
                        }
                        else if (dr["WorkflowStageFields_vDBType"].ToString() == "Int")
                        {
                            txt.Attributes.Add("onkeypress", "return onlyNumbers(event,this);");

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

                            if (MaxVal > 0)
                            {
                                if (MaxValidationScript != "")
                                {
                                    MaxValidationScript += "##";
                                }

                                MaxValidationScript += txt.ClientID.ToString() + "|TextBox|" + MinVal.ToString() + "|" + MaxVal.ToString();
                            }
                        }
                        else if (dr["WorkflowStageFields_vDBType"].ToString().Contains("Date"))
                        {
                            //AjaxCalender Extender if Created Fdild is Dob at add new stageFeilds
                            AjaxControlToolkit.CalendarExtender Cal = new AjaxControlToolkit.CalendarExtender();
                            Cal.Format = "dd/MM/yyyy";
                            txt.Attributes.Add("readonly", "readonly");
                            Cal.TargetControlID = txt.ID;
                            tc2.Controls.Add(Cal);
                        }
                        else if (dr["WorkflowStageFields_vDBType"].ToString() == "Boolean")
                        {
                            if (SpecialCharValidationScript != "")
                            {
                                SpecialCharValidationScript += "##";
                            }
                            SpecialCharValidationScript += txt.ClientID.ToString() + "|TextBox";
                        }
                    }

                    tr.Controls.Add(tc1);
                    tr.Controls.Add(tc2);
                    FieldTable1.Rows.Add(tr);

                }

                ControlPlaceHolder_CurStage.Controls.Add(FieldTable1);


                string ControlData_CurStage = string.Empty;
                ControlData_CurStage = GenerateControlData(Objresult.dsResult.Tables[1]);

            }

            else
            {
                table_curStage.Visible = false;
            }


            if (Objresult.dsResult != null && Objresult.dsResult.Tables.Count > 0 && Objresult.dsResult.Tables[2].Rows.Count > 0)
            {
                HtmlTable FieldTable2 = new HtmlTable();
                FieldTable2.Disabled = true;

                foreach (DataRow dr in Objresult.dsResult.Tables[2].Rows)
                {
                    HtmlTableRow tr = new HtmlTableRow();
                    HtmlTableCell tc1 = new HtmlTableCell();
                    HtmlTableCell tc2 = new HtmlTableCell();


                    if ((dr["WorkflowStageFields_cObjType"].ToString() == "DropDown") && (dr["WorkflowStageFields_bDisplay"].ToString() == "True"))
                    {
                        Label lbl = new Label();
                        lbl.ID = dr["WorkflowStageFields_ControlID"].ToString() + "_Label";
                        lbl.Text = dr["WorkflowStageFields_vLabelText"].ToString();
                        tc1.Width = "150px";
                        tc1.Controls.Add(lbl);

                        DropDownList ddl = new DropDownList();
                        ddl.ID = dr["WorkflowStageFields_ControlID"].ToString();
                        if (dr["WorkflowStageFields_bEditable"].ToString() == "True")
                        {
                            ddl.Enabled = true;
                        }
                        else
                        {
                            ddl.Enabled = false;
                        }

                        tc2.Controls.Add(ddl);

                        if (dr["WorkflowStageFields_iParentId"].ToString() == "0")
                        {
                            BindDropDownList(ddl, Convert.ToInt32(dr["WorkflowStageFields_iMasterType"].ToString()));
                        }


                        try
                        {
                            ddl.SelectedValue = Objresult.dsResult.Tables[3].Rows[0][dr["WorkflowStageFields_vDBFld"].ToString()].ToString();
                        }
                        catch 
                        {
                            ddl.SelectedValue = "0";
                        }

                        if (dr["WorkflowStageFields_bMandatory"].ToString() == "True")
                        {
                            Label lblAstreick = new Label();
                            lblAstreick.ForeColor = System.Drawing.Color.Red;
                            lblAstreick.Text = "&nbsp;*";
                            lblAstreick.Width = Unit.Pixel(50);
                            lblAstreick.Style.Add("font-size", "large");
                            tc2.Controls.Add(lblAstreick);

                            if (mandatoryValidationScript != "")
                            {
                                mandatoryValidationScript += "##";
                            }
                            mandatoryValidationScript += ddl.ClientID.ToString() + "|DropDown";
                        }
                    }

                    else if ((dr["WorkflowStageFields_cObjType"].ToString() == "TextBox") && (dr["WorkflowStageFields_bDisplay"].ToString() == "True"))
                    {
                        Label lbl = new Label();
                        lbl.ID = dr["WorkflowStageFields_ControlID"].ToString() + "_Label";
                        lbl.Text = dr["WorkflowStageFields_vLabelText"].ToString();
                        tc1.Width = "150px";
                        tc1.Controls.Add(lbl);

                        TextBox txt = new TextBox();
                        txt.ID = dr["WorkflowStageFields_ControlID"].ToString();
                        if (dr["WorkflowStageFields_bEditable"].ToString() == "True")
                        {
                            txt.Enabled = true;
                        }
                        else
                        {
                            txt.Enabled = false;
                        }

                        tc2.Controls.Add(txt);

                        txt.Text = Objresult.dsResult.Tables[3].Rows[0][dr["WorkflowStageFields_vDBFld"].ToString()].ToString();

                        if (dr["WorkflowStageFields_bMandatory"].ToString() == "True")
                        {
                            Label lblAstreick = new Label();
                            lblAstreick.ForeColor = System.Drawing.Color.Red;
                            lblAstreick.Text = "&nbsp;*";
                            lblAstreick.Width = Unit.Pixel(50);
                            lblAstreick.Style.Add("font-size", "large");
                            tc2.Controls.Add(lblAstreick);

                            if (mandatoryValidationScript != "")
                            {
                                mandatoryValidationScript += "##";
                            }
                            mandatoryValidationScript += txt.ClientID.ToString() + "|TextBox";
                        }

                        if (dr["WorkflowStageFields_vDBType"].ToString() == "String")
                        {
                            if (SpecialCharValidationScript != "")
                            {
                                SpecialCharValidationScript += "##";
                            }
                            SpecialCharValidationScript += txt.ClientID.ToString() + "|TextBox";
                        }
                        else if (dr["WorkflowStageFields_vDBType"].ToString() == "Int")
                        {
                            txt.Attributes.Add("onkeypress", "return onlyNumbers(event,this);");

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

                            if (MaxVal > 0)
                            {
                                if (MaxValidationScript != "")
                                {
                                    MaxValidationScript += "##";
                                }

                                MaxValidationScript += txt.ClientID.ToString() + "|TextBox|" + MinVal.ToString() + "|" + MaxVal.ToString();
                            }
                        }
                        else if (dr["WorkflowStageFields_vDBType"].ToString().Contains("Date"))
                        {
                            //AjaxCalender Extender if Created Fdild is Dob at add new stageFeilds
                            AjaxControlToolkit.CalendarExtender Cal = new AjaxControlToolkit.CalendarExtender();
                            Cal.Format = "dd/MM/yyyy";
                            txt.Attributes.Add("readonly", "readonly");
                            Cal.TargetControlID = txt.ID;
                            tc2.Controls.Add(Cal);
                        }
                        else if (dr["WorkflowStageFields_vDBType"].ToString() == "Boolean")
                        {
                            if (SpecialCharValidationScript != "")
                            {
                                SpecialCharValidationScript += "##";
                            }
                            SpecialCharValidationScript += txt.ClientID.ToString() + "|TextBox";
                        }

                    }

                    tr.Controls.Add(tc1);
                    tr.Controls.Add(tc2);
                    FieldTable2.Rows.Add(tr);

                }

                ControlPlaceHolder_PrevStage.Controls.Add(FieldTable2);

                string ControlData_PrevStage = string.Empty;
                ControlData_PrevStage = GenerateControlData(Objresult.dsResult.Tables[2]);

            }

            else
            {
                table_PrevStage.Visible = false;
            }

            btnSave.Attributes.Add("onclick", "return ValidateMandatoryData('" + mandatoryValidationScript + "','" + SpecialCharValidationScript + "','" + MaxValidationScript + "');");
            btnCancel.Attributes.Add("onclick", "return SetAction('Cancel');");
            if (Objresult.dsResult != null && Objresult.dsResult.Tables.Count > 0 && Objresult.dsResult.Tables[3].Rows.Count > 0)
            {


                strImagePath = Objresult.dsResult.Tables[3].Rows[0]["WorkflowStageFieldData_vDocPhysicalPath"].ToString();
            }

            if (hdnSaveStatus.Value == "")
            {
                try
                {
                    DDLStatus.SelectedValue = Objresult.dsResult.Tables[3].Rows[0]["WorkflowStageFieldData_iStatusID"].ToString();
                }
                catch
                {
                    DDLStatus.SelectedValue = "0";
                }
            }

            if (Objresult.dsResult != null && Objresult.dsResult.Tables.Count > 0 && Objresult.dsResult.Tables[1] != null && Objresult.dsResult.Tables[1].Rows.Count > 0)
            {
                foreach (DataRow dr in Objresult.dsResult.Tables[1].Rows)
                {

                    if ((dr["WorkflowStageFields_cObjType"].ToString() == "DropDown") && (dr["WorkflowStageFields_iChildId"].ToString() != "0"))
                    {
                        DropDownList ddlParent = (DropDownList)this.ControlPlaceHolder_PrevStage.FindControl(dr["WorkflowStageFields_ControlID"].ToString());
                        ddlParent.AutoPostBack = true;
                        ddlParent.SelectedIndexChanged += new EventHandler(ddlFilter_SelectedIndexChanged);
                    }

                    //fill child dropdowns
                    if ((dr["WorkflowStageFields_cObjType"].ToString() == "DropDown") && (dr["WorkflowStageFields_iParentId"].ToString() != "0"))
                    {
                        DropDownList ddlParent = (DropDownList)this.ControlPlaceHolder_CurStage.FindControl("Control_" + dr["WorkflowStageFields_iParentId"].ToString());
                        ddlFilter_SelectedIndexChanged(ddlParent, new EventArgs());

                        //set value of child dropdowns
                        DropDownList ddlchild = (DropDownList)this.ControlPlaceHolder_CurStage.FindControl(dr["WorkflowStageFields_ControlID"].ToString());
                        try
                        {
                            ddlchild.SelectedValue = Objresult.dsResult.Tables[3].Rows[0][dr["WorkflowStageFields_vDBFld"].ToString()].ToString();
                        }
                        catch 
                        {
                            ddlchild.SelectedValue = "0";
                        }
                    }
                }
            }

            if (Objresult.dsResult != null && Objresult.dsResult.Tables.Count > 0 && Objresult.dsResult.Tables[2] != null && Objresult.dsResult.Tables[2].Rows.Count > 0)
            {
                foreach (DataRow dr in Objresult.dsResult.Tables[2].Rows)
                {
                    if ((dr["WorkflowStageFields_cObjType"].ToString() == "DropDown") && (dr["WorkflowStageFields_iChildId"].ToString() != "0"))
                    {
                        DropDownList ddlParent = (DropDownList)this.ControlPlaceHolder_PrevStage.FindControl(dr["WorkflowStageFields_ControlID"].ToString());
                        if (ddlParent != null)
                        {
                            ddlParent.AutoPostBack = true;
                            ddlParent.SelectedIndexChanged += new EventHandler(ddlFilter_SelectedIndexChanged);
                        }
                    }

                    //fill child dropdowns
                    if ((dr["WorkflowStageFields_cObjType"].ToString() == "DropDown") && (dr["WorkflowStageFields_iParentId"].ToString() != "0") && (dr["WorkflowStageFields_bDisplay"].ToString() == "True"))
                    {
                        DropDownList ddlParent = (DropDownList)this.ControlPlaceHolder_PrevStage.FindControl("Control_" + dr["WorkflowStageFields_iParentId"].ToString());
                        ddlFilter_SelectedIndexChanged(ddlParent, new EventArgs());

                        //set value of child dropdowns
                        DropDownList ddlchild = (DropDownList)this.ControlPlaceHolder_PrevStage.FindControl(dr["WorkflowStageFields_ControlID"].ToString());
                        try
                        {
                            ddlchild.SelectedValue = Objresult.dsResult.Tables[3].Rows[0][dr["WorkflowStageFields_vDBFld"].ToString()].ToString();
                        }
                        catch
                        {
                            ddlchild.SelectedValue = "0";
                        }
                    }
                }
            }

            return strImagePath;
        }

        void ddlFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlParent = (DropDownList)sender;
            DropDownList ddlChild = null;

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
                            ddlChild = (DropDownList)this.ControlPlaceHolder_CurStage.FindControl("Control_" + dr["WorkflowStageFields_iChildId"].ToString());
                            ddlChild.SelectedValue = "0";
                            BindChildDropDownList(ddlChild, Convert.ToInt32(dr["WorkflowStageFields_iMasterType"].ToString()), Convert.ToInt32(ddlParent.SelectedValue));
                        }
                    }
                }
                foreach (DataRow dr in Objresult.dsResult.Tables[2].Rows)
                {
                    if (ddlParent != null)
                    {
                        if (dr["WorkflowStageFields_ControlID"].ToString() == ddlParent.ID.ToString())
                        {
                            ddlChild = (DropDownList)this.ControlPlaceHolder_PrevStage.FindControl("Control_" + dr["WorkflowStageFields_iChildId"].ToString());
                            if (ddlChild != null)
                            {
                                ddlChild.SelectedValue = "0";
                                BindChildDropDownList(ddlChild, Convert.ToInt32(dr["WorkflowStageFields_iMasterType"].ToString()), Convert.ToInt32(ddlParent.SelectedValue));
                            }
                        }
                    }
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

        private void LoadImageToViewer(string Action)
        {
            string PdfFullPath = Session["strPDFPath"].ToString();
            string strPDFFolder = beforedot(PdfFullPath);
            string fileExtension = afterdot(PdfFullPath);
            hdnPagesCount.Value = System.IO.Directory.GetFiles(strPDFFolder, "*" + fileExtension).Length.ToString();

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

            string PdfPath = strPDFFolder + "\\" + PageNo.ToString() + fileExtension;

            Logger.Trace("Converting PDF to JPEG.", Session["LoggedUserId"].ToString());
            // Convert pdf to JPEG 
            string CurrentFilepath = new ConvertPdf2Image().ConvertPDFtoHojas(PdfPath, strPDFFolder, PageNo.ToString());

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

    }
}