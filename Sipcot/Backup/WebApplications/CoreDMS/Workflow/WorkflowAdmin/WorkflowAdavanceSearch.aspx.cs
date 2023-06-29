

/* ============================================================================  
Author     : 
Create date: 
===============================================================================  
** Change History   
** Date:          Author:            Issue ID            Description:  
** ----------   -------------       ----------    ----------------------------
*17 Apr 2015   Gokul                DMS5-3946         Applying rights an permissions in all pages as per user group rights!   
7/7/2015      Sharath               DMS5-4388         If Process, Workflow, Stage contains only one item then by default select it
17 Jul 2015   Sharath               DMSENH6-4638      Search & Retrieve improvization
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
using Lotex.Common;

namespace Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin
{
    public partial class WorkflowAdavanceSearch : PageBase
    {
        WorkflowAdvanceSearchBLL objAdvanceSearch = new WorkflowAdvanceSearchBLL();
        public string pageRights = string.Empty; /* DMS5-3946 A */
       
        public int ControlCount = 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            
            lblMessage.Text = "";
            CheckAuthentication();           
            pageRights = GetPageRights();/* DMS5-3946 A */
            txtStartDate.Attributes.Add("readonly", "readonly");
            txtEndDate.Attributes.Add("readonly", "readonly");

            //Start: ------------------SitePath Details ----------------------
            Label sitePath = (Label)Master.FindControl("SitePath");
            sitePath.Text = "WorkFlow Tasks >> Advance Search";
            //End: ------------------SitePath Details ------------------------

            if (!IsPostBack)
            {
                ApplyPageRights(pageRights, this.Form.Controls); /* DMS5-3946 A */
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objAdvanceSearch.LoginToken = loginUser.LoginToken;
                objAdvanceSearch.LoginOrgId = loginUser.LoginOrgId;
                DBResult objQueryConditions = objAdvanceSearch.ManageWorkflowAdvanceSearch(objAdvanceSearch, "GetQueryConditions");
                Session["QueryConditions"] = objQueryConditions;
                BindprocessDropDown();
                BindSavedQueriesDropDown();

                ListItem li = new ListItem("---Select---", "0");
                ddlWorkflow.Items.Clear();
                ddlWorkflow.Items.Add(li);

                ddlStage.Items.Clear();
                ddlStage.Items.Add(li);

                ddlStatus.Items.Clear();
                ddlStatus.Items.Add(li);

                if (Session["dtQueryClause"] != null)
                {
                    Session.Remove("dtQueryClause");
                }
            }

            RecreateQueryClause();
        }

        private void RecreateQueryClause()
        {
            string ObjectType = string.Empty;
            DataTable dtQueryClause = new DataTable();
            if (Session["dtQueryClause"] != null)
            {
                dtQueryClause = (DataTable)Session["dtQueryClause"];
                ControlCount = 0;
                foreach (DataRow drTemp in dtQueryClause.Rows)
                {
                    ControlCount++;
                    ObjectType = drTemp["ObjectType"].ToString();
                    CreateQueryClause(ObjectType, drTemp, ControlCount);
                }
            }
        }




        protected void btnAddClause_Click(object sender, EventArgs e)
        {
            CheckAuthentication();
            string ObjectType = "TextBox";

            DataTable dtQueryClause = new DataTable();
            DataRow dr;
            if (Session["dtQueryClause"] != null)
            {
                dtQueryClause = (DataTable)Session["dtQueryClause"];

                dr = dtQueryClause.NewRow();
                dr["BoolOperator"] = "";
                dr["FieldName"] = "";
                dr["QueryCondition"] = "";
                dr["FieldValue"] = "";
                dr["ObjectType"] = "TextBox";

                dtQueryClause.Rows.Add(dr);
                Session["dtQueryClause"] = dtQueryClause;
                ControlCount++;
                CreateQueryClause(ObjectType, dr, ControlCount);

            }
            else
            {

                dtQueryClause.Columns.Add("BoolOperator");
                dtQueryClause.Columns.Add("FieldName");
                dtQueryClause.Columns.Add("QueryCondition");
                dtQueryClause.Columns.Add("FieldValue");
                dtQueryClause.Columns.Add("ObjectType");

                dr = dtQueryClause.NewRow();
                dr["BoolOperator"] = "";
                dr["FieldName"] = "";
                dr["QueryCondition"] = "";
                dr["FieldValue"] = "";
                dr["ObjectType"] = "TextBox";

                dtQueryClause.Rows.Add(dr);
                Session["dtQueryClause"] = dtQueryClause;
                CreateQueryClause(ObjectType, dr, 1);
            }


        }

        protected void btnRemoveClause_Click(object sender, EventArgs e)
        {
            CheckAuthentication();
            if (Session["dtQueryClause"] != null)
            {
                DataTable dtQueryClause = new DataTable();
                dtQueryClause = (DataTable)Session["dtQueryClause"];


                for (int cnt = 1; cnt <= dtQueryClause.Rows.Count; cnt++)
                {
                    if (cnt == ControlCount)
                    {
                        Table panelrow = (Table)PanelQueryHolder.FindControl("Panel_Row_" + ControlCount.ToString());

                        PanelQueryHolder.Controls.Remove(panelrow);
                    }
                }
                dtQueryClause.Rows.RemoveAt(dtQueryClause.Rows.Count - 1);

                if (dtQueryClause.Rows.Count > 0)
                {
                    Session["dtQueryClause"] = dtQueryClause;
                }
                else
                {
                    Session.Remove("dtQueryClause");
                }
            }
        }

        private void CreateQueryClause(string ObjectType, DataRow dr, int controlId)
        {
            CheckAuthentication();


            Table tbl = new Table();
            TableRow tr = new TableRow();
            TableCell cell = new TableCell();
            tbl.Width = Unit.Pixel(560);
            tbl.ID = "Panel_Row_" + controlId.ToString();

            DropDownList ddlBoolOperator = new DropDownList();
            DBResult dbrQueryConditions = (DBResult)Session["QueryConditions"];
            ddlBoolOperator.ID = "ddlBoolOperator_Row_" + controlId.ToString();
            if (dbrQueryConditions.dsResult.Tables[0].Rows.Count > 0 && dbrQueryConditions.dsResult.Tables.Count > 0)
            {
                ddlBoolOperator.DataSource = dbrQueryConditions.dsResult.Tables[0];
                ddlBoolOperator.DataTextField = "WorkflowBoolOperator_vName";
                ddlBoolOperator.DataValueField = "WorkflowBoolOperator_iId";
                ddlBoolOperator.DataBind();
                ddlBoolOperator.Width = Unit.Pixel(100);

                //disable dropdown for first count
                if (controlId == 1) //DMSENH6-4638
                {
                    ddlBoolOperator.Enabled = false;
                }

                if (dr["BoolOperator"].ToString() != "")
                {
                    try
                    {
                        ddlBoolOperator.SelectedValue = dr["BoolOperator"].ToString().Trim();
                    }
                    catch 
                    {

                    }
                }
            }
            DropDownList ddlQueryCondition = new DropDownList();
            ddlQueryCondition.ID = "ddlQueryCondition_Row_" + controlId.ToString();
            if (dbrQueryConditions.dsResult.Tables[1].Rows.Count > 0 && dbrQueryConditions.dsResult.Tables.Count > 0)
            {
                ddlQueryCondition.DataSource = dbrQueryConditions.dsResult.Tables[1];
                ddlQueryCondition.DataTextField = "WorkflowQueryCondition_vName";
                ddlQueryCondition.DataValueField = "WorkflowQueryCondition_iId";
                ddlQueryCondition.DataBind();
                ddlQueryCondition.Width = Unit.Pixel(120);

                if (dr["QueryCondition"].ToString() != "")
                {
                    try
                    {
                        ddlQueryCondition.SelectedValue = dr["QueryCondition"].ToString().Trim();
                    }
                    catch 
                    {

                    }
                }
            }
            DropDownList ddlFieldNames = new DropDownList();
            ddlFieldNames.ID = "ddlFieldNames_Row_" + controlId.ToString();
            DBResult dbrFields = (DBResult)Session["FieldNames"];
            if (dbrFields.dsResult.Tables.Count > 0 && dbrFields.dsResult.Tables[0].Rows.Count > 0)
            {
                ddlFieldNames.DataSource = dbrFields.dsResult.Tables[0];
                ddlFieldNames.DataTextField = "WorkflowStageFields_vName";
                ddlFieldNames.DataValueField = "WorkflowStageFields_iId";
                ddlFieldNames.Width = Unit.Pixel(150);
                ddlFieldNames.DataBind();
                ddlFieldNames.AutoPostBack = true;
                ddlFieldNames.SelectedIndexChanged += new EventHandler(ddlQueryCondition_SelectedIndexChanged);

                if (dr["FieldName"].ToString() != "")
                {
                    try
                    {
                        ddlFieldNames.SelectedValue = dr["FieldName"].ToString().Trim();
                    }
                    catch 
                    {

                    }
                }

            }

            DropDownList ddlFieldValue = new DropDownList();
            ddlFieldValue.ID = "ddlFieldValue_Row_" + controlId.ToString();
            ddlFieldValue.Width = Unit.Pixel(130);
            TextBox txtFieldValue = new TextBox();
            txtFieldValue.ID = "txtFieldValue_Row_" + controlId.ToString();
            txtFieldValue.Width = Unit.Pixel(120);

            if (ObjectType == "TextBox")
            {
                ddlFieldValue.Visible = false;
                txtFieldValue.Visible = true;

                if (dr["FieldValue"].ToString() != "")
                {
                    try
                    {
                        txtFieldValue.Text = dr["FieldValue"].ToString().Trim();
                    }
                    catch
                    {

                    }
                }
            }
            else
            {
                txtFieldValue.Visible = false;
                ddlFieldValue.Visible = true;

                if (dr["FieldValue"].ToString() != "")
                {
                    try
                    {
                        ddlFieldValue.SelectedValue = dr["FieldValue"].ToString().Trim();
                    }
                    catch
                    {

                    }
                }
            }

            cell.Controls.Add(ddlBoolOperator);
            cell.Controls.Add(ddlFieldNames);
            cell.Controls.Add(ddlQueryCondition);
            cell.Controls.Add(ddlFieldValue);
            cell.Controls.Add(txtFieldValue);

            tr.Controls.Add(cell);
            tbl.Controls.Add(tr);
            PanelQueryHolder.Controls.Add(tbl);

            ddlQueryCondition_SelectedIndexChanged(ddlFieldNames, new EventArgs());

        }

        protected void ddlQueryCondition_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckAuthentication();
            try
            {
                if (Session["FieldNames"] != null)
                {
                    DBResult queryFields = (DBResult)Session["FieldNames"];

                    string ddlCurrentRowId = ((DropDownList)sender).ID.ToString();
                    string ddlSelectedValue = ((DropDownList)sender).SelectedValue;
                    string strMasterType = "0";
                    string strDBFieldType = "";
                    string strObjectType = "";
                    string CurrentRow = ddlCurrentRowId.Substring(ddlCurrentRowId.LastIndexOf("_") + 1);

                    if (queryFields.dsResult != null && queryFields.dsResult.Tables != null && queryFields.dsResult.Tables.Count > 0)
                    {
                        if (ddlSelectedValue != "0")
                        {
                            foreach (DataRow dr in queryFields.dsResult.Tables[0].Rows)
                            {
                                if (dr["WorkflowStageFields_iId"].ToString() == ddlSelectedValue)
                                {
                                    strMasterType = dr["WorkflowStageFields_iMasterType"].ToString();
                                    strDBFieldType = dr["WorkflowStageFields_vDBType"].ToString();
                                    strObjectType = dr["WorkflowStageFields_cObjType"].ToString();
                                    
                                    break;
                                }
                            }
                        }
                        else
                        {
                            strMasterType = "0";
                        }
                    }

                    DropDownList ddlFieldValue = (DropDownList)PanelQueryHolder.FindControl("ddlFieldValue_Row_" + CurrentRow);
                    TextBox txtFieldValue = (TextBox)PanelQueryHolder.FindControl("txtFieldValue_Row_" + CurrentRow);
                    if (strMasterType != "0")
                    {
                        if (strObjectType == "DropDown")
                        {
                            txtFieldValue.Visible = false;
                            ddlFieldValue.Visible = true;
                        }
                        else if (strObjectType == "TextBox")
                        {
                            ddlFieldValue.Visible = false;
                            txtFieldValue.Visible = true;
                        }

                        DBResult dbrMasterValues = GetMasterValues(Convert.ToInt32(strMasterType));

                        ddlFieldValue.DataSource = dbrMasterValues.dsResult;
                        ddlFieldValue.DataTextField = "WorkflowMasterValues_vName";
                        ddlFieldValue.DataValueField = "WorkflowMasterValues_iId";
                        ddlFieldValue.DataBind();
                    }
                    else
                    {
                        if (strObjectType == "DropDown")
                        {
                            txtFieldValue.Visible = false;
                            ddlFieldValue.Visible = true;
                        }
                        else if (strObjectType == "TextBox")
                        {
                            ddlFieldValue.Visible = false;
                            txtFieldValue.Visible = true;
                            txtFieldValue.ReadOnly = false;

                            AjaxControlToolkit.CalendarExtender cal = (AjaxControlToolkit.CalendarExtender)PanelQueryHolder.FindControl("Cal_" + txtFieldValue.ID);
                            if (cal != null)
                            {
                                PanelQueryHolder.Controls.Remove(cal);
                            }

                            if (strDBFieldType == "Date")
                            {
                                //AjaxControlToolkit.CalendarExtender Cal = new AjaxControlToolkit.CalendarExtender();
                                //Cal.Format = "dd/MM/yyyy";
                                //txtFieldValue.ReadOnly = true;
                                //Cal.TargetControlID = txtFieldValue.ID;
                                //Cal.ID = "Cal_" + txtFieldValue.ID;
                                //PanelQueryHolder.Controls.Add(Cal);
                            }
                        }
                    }

                }
            }
            catch
            {

            }
        }

        private string GetQueryClauseXML()
        {
            string strQueryClauseXML = string.Empty;
            CheckAuthentication();
            try
            {
                if (Session["dtQueryClause"] != null)
                {

                    DBResult queryFields = (DBResult)Session["FieldNames"];

                    DataTable dtQueryClause = (DataTable)Session["dtQueryClause"];

                    if (dtQueryClause.Rows.Count > 0)
                    {
                        int slNo = 0;
                        foreach (DataRow dr in dtQueryClause.Rows)
                        {
                            slNo++;

                            DropDownList ddlBoolOperator = (DropDownList)PanelQueryHolder.FindControl("ddlBoolOperator_Row_" + slNo.ToString());
                            DropDownList ddlQueryCondition = (DropDownList)PanelQueryHolder.FindControl("ddlQueryCondition_Row_" + slNo.ToString());
                            DropDownList ddlFieldNames = (DropDownList)PanelQueryHolder.FindControl("ddlFieldNames_Row_" + slNo.ToString());
                            DropDownList ddlFieldValue = (DropDownList)PanelQueryHolder.FindControl("ddlFieldValue_Row_" + slNo.ToString());
                            TextBox txtFieldValue = (TextBox)PanelQueryHolder.FindControl("txtFieldValue_Row_" + slNo.ToString());

                            string strFieldValue = string.Empty;
                            string strMasterType = "0";
                            string strObjectType = string.Empty;
                            if (ddlFieldNames.SelectedValue != "0")
                            {


                                foreach (DataRow drTemp in queryFields.dsResult.Tables[0].Rows)
                                {
                                    if (drTemp["WorkflowStageFields_iId"].ToString() == ddlFieldNames.SelectedValue)
                                    {
                                        strMasterType = drTemp["WorkflowStageFields_iMasterType"].ToString();
                                        break;
                                    }
                                }

                                if (strMasterType != "0")
                                {
                                    strFieldValue = ddlFieldValue.SelectedValue;
                                }
                                else
                                {
                                    strFieldValue = txtFieldValue.Text;
                                }

                            }
                            else
                            {
                                strFieldValue = "";
                            }

                            strObjectType = strMasterType != "0" ? "DropDown" : "TextBox";

                            strQueryClauseXML += "<Clause> " +
                                "<BoolOperator> " + ddlBoolOperator.SelectedValue + " </BoolOperator>" +
                            "<QueryCondition>" + ddlQueryCondition.SelectedValue + "</QueryCondition>" +
                            "<FieldName> " + ddlFieldNames.SelectedValue + " </FieldName>" +
                            "<FieldValue>" + strFieldValue + "</FieldValue>" +
                            "<ObjectType>" + strObjectType + "</ObjectType>" +
                            "</Clause>";


                        }
                    }

                }
            }
            catch 
            {

            }

            return "<QueryClause>" + strQueryClauseXML + "</QueryClause>";
        }

        protected DBResult GetMasterValues(int MasterType)
        {

            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            objAdvanceSearch.LoginToken = loginUser.LoginToken;
            objAdvanceSearch.LoginOrgId = loginUser.LoginOrgId;

            objAdvanceSearch.WorkflowAdvanceMasterTypeId = MasterType;
            objResult = objAdvanceSearch.ManageWorkflowAdvanceSearch(objAdvanceSearch, "BindFieldValues");

            return objResult;

        }

        protected void BindprocessDropDown()
        {
            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            objAdvanceSearch.LoginToken = loginUser.LoginToken;
            objAdvanceSearch.LoginOrgId = loginUser.LoginOrgId;
            objResult = objAdvanceSearch.ManageWorkflowAdvanceSearch(objAdvanceSearch, "BindProcessSearchDropDown");
            //DMS5-4388  BS
            DropdownBinder.BindDropDown(ddlProcess, objResult.dsResult, "WorkflowProcess_iId", "WorkflowProcess_vName");

            //Calling method to select value if table has only one item
            if (ddlProcess.Items.Count.Equals(2))
            {
                ddlProcess.Items[1].Selected = true;
                ddlProcess_SelectedIndexChanged(ddlProcess, new EventArgs());
            }
            //DMS5-4388  BE
        }

        protected void BindSavedQueriesDropDown()
        {
            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            objAdvanceSearch.LoginToken = loginUser.LoginToken;
            objAdvanceSearch.LoginOrgId = loginUser.LoginOrgId;
            objResult = objAdvanceSearch.ManageWorkflowAdvanceSearch(objAdvanceSearch, "BindSavedQueriesDropDown");
            ddlQueries.DataSource = objResult.dsResult;
            ddlQueries.DataTextField = "WorkflowQuery_vQueryName";
            ddlQueries.DataValueField = "WorkflowQuery_iId";
            ddlQueries.DataBind();

        }


        protected void ddlProcess_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckAuthentication();

            if (ddlProcess.SelectedValue != "0")
            {
                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objAdvanceSearch.LoginToken = loginUser.LoginToken;
                objAdvanceSearch.LoginOrgId = loginUser.LoginOrgId;
                Session["ProcessId"] = ddlProcess.SelectedValue;
                objAdvanceSearch.WorkflowAdvanceSearchProcessId = Convert.ToInt32(Session["ProcessId"]);
                objResult = objAdvanceSearch.ManageWorkflowAdvanceSearch(objAdvanceSearch, "BindWorkflowSearchDropDown");
                ddlWorkflow.DataSource = objResult.dsResult;
                ddlWorkflow.DataTextField = "Workflow_vName";
                ddlWorkflow.DataValueField = "Workflow_iId";
                ddlWorkflow.DataBind();

                Session.Remove("FieldNames");

                //DMS5-4388  BS
                //Load Workflow dropdown based on process ID
                DropdownBinder.BindDropDown(ddlWorkflow, objResult.dsResult, "Workflow_iId", "Workflow_vName");

                //Calling method to select value if table has only one item
                if (ddlWorkflow.Items.Count.Equals(2))
                {
                    ddlWorkflow.Items[1].Selected = true;
                    ddlWorkflow_SelectedIndexChanged(ddlWorkflow, new EventArgs());
                    btnSearch_Click(sender, e);
                }
                //DMS5-4388  BE

                ListItem li = new ListItem("---Select---", "0");
                ddlStage.Items.Clear();
                ddlStage.Items.Add(li);

                ddlStatus.Items.Clear();
                ddlStatus.Items.Add(li);
            }
            else
            {
                Session["ProcessId"] = "0";
                ListItem li = new ListItem("---Select---", "0");
                ddlWorkflow.Items.Clear();
                ddlWorkflow.Items.Add(li);

                ddlStage.Items.Clear();
                ddlStage.Items.Add(li);

                ddlStatus.Items.Clear();
                ddlStatus.Items.Add(li);

                txtStartDate.Text = string.Empty;
                txtEndDate.Text = string.Empty;

                GridSearchResult.DataSource = null;
                GridSearchResult.DataBind();
            }

            PanelQueryHolder.Controls.Clear();
            Session.Remove("dtQueryClause");
        }

        protected void ddlStage_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckAuthentication();
            if (ddlStage.SelectedValue != "0")
            {
                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objAdvanceSearch.LoginToken = loginUser.LoginToken;
                objAdvanceSearch.LoginOrgId = loginUser.LoginOrgId;
                objAdvanceSearch.WorkflowAdvanceSearchProcessId = Convert.ToInt32(Session["ProcessId"]);
                objAdvanceSearch.WorkflowAdvanceSearchWorkflowId = Convert.ToInt32(Session["WorkflowId"]);
                Session["StageId"] = ddlStage.SelectedValue;
                objAdvanceSearch.WorkflowAdvanceSearchStageId = Convert.ToInt32(Session["StageId"]);
                objResult = objAdvanceSearch.ManageWorkflowAdvanceSearch(objAdvanceSearch, "BindStatusDropDown");
                ddlStatus.DataSource = objResult.dsResult;
                ddlStatus.DataTextField = "WorkflowStageStatus_vName";
                ddlStatus.DataValueField = "WorkflowStageStatus_iId";
                ddlStatus.DataBind();

                Session["FieldNames"] = GetFieldNames();
                Session["StageId"] = ddlStage.SelectedValue;
            }
            else
            {
                Session["StageId"] = "0";
                ListItem li = new ListItem("---Select---", "0");
                ddlStatus.Items.Clear();
                ddlStatus.Items.Add(li);
            }

            PanelQueryHolder.Controls.Clear();
            Session.Remove("dtQueryClause");
        }

        protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckAuthentication();
            Session["StatusId"] = ddlStatus.SelectedValue;
        }

        protected void ddlWorkflow_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckAuthentication();
            if (ddlWorkflow.SelectedValue != "0")
            {
                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objAdvanceSearch.LoginToken = loginUser.LoginToken;
                objAdvanceSearch.LoginOrgId = loginUser.LoginOrgId;
                Session["WorkflowId"] = ddlWorkflow.SelectedValue;
                objAdvanceSearch.WorkflowAdvanceSearchProcessId = Convert.ToInt32(Session["ProcessId"]);
                objAdvanceSearch.WorkflowAdvanceSearchWorkflowId = Convert.ToInt32(Session["WorkflowId"]);
                objResult = objAdvanceSearch.ManageWorkflowAdvanceSearch(objAdvanceSearch, "BindSearchStageDropDown");
                ddlStage.DataSource = objResult.dsResult;
                ddlStage.DataTextField = "WorkflowStage_vDisplayName";
                ddlStage.DataValueField = "WorkflowStage_iId";
                ddlStage.DataBind();

                Session["FieldNames"] = GetFieldNames();
                //DMS5-4388  BS
                if (!(ddlWorkflow.SelectedValue.Equals(0)))
                {
                    ddlStage_SelectedIndexChanged(ddlStage, new EventArgs());
                }
                //DMS5-4388  BE

                ListItem li = new ListItem("---Select---", "0");
                ddlStatus.Items.Clear();
                ddlStatus.Items.Add(li);
            }
            else
            {
                Session["WorkflowId"] = "0";
                ListItem li = new ListItem("---Select---", "0");
                ddlStage.Items.Clear();
                ddlStage.Items.Add(li);

                ddlStatus.Items.Clear();
                ddlStatus.Items.Add(li);

                txtStartDate.Text = string.Empty;
                txtEndDate.Text = string.Empty;
            }

            PanelQueryHolder.Controls.Clear();
            Session.Remove("dtQueryClause");
        }



        protected void ddlQueries_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckAuthentication();
            if (ddlQueries.SelectedValue != "0")
            {
                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objAdvanceSearch.LoginToken = loginUser.LoginToken;
                objAdvanceSearch.LoginOrgId = loginUser.LoginOrgId;
                string ObjectType = string.Empty;
                objAdvanceSearch.WorkflowAdvanceQueryId = Convert.ToInt32(ddlQueries.SelectedValue);
                objResult = objAdvanceSearch.ManageWorkflowAdvanceSearch(objAdvanceSearch, "GetQueryClause");
                if (objResult.dsResult != null && objResult.dsResult.Tables.Count > 0 && objResult.dsResult.Tables[0].Rows.Count > 0)
                {

                    DataRow currentRow = objResult.dsResult.Tables[0].Rows[0];

                    try
                    {
                        ddlProcess.SelectedValue = currentRow["WorkflowQuery_iProcessId"].ToString();
                        ddlProcess_SelectedIndexChanged(sender, e);
                        ddlWorkflow.SelectedValue = currentRow["WorkflowQuery_iWorkflowId"].ToString();
                        ddlWorkflow_SelectedIndexChanged(sender, e);
                        ddlStage.SelectedValue = currentRow["WorkflowQuery_iStageId"].ToString();
                        ddlStage_SelectedIndexChanged(sender, e);
                        ddlStatus.SelectedValue = currentRow["WorkflowQuery_iStatusId"].ToString();
                        txtStartDate.Text = currentRow["WorkflowQuery_vStartDate"].ToString();
                        txtEndDate.Text = currentRow["WorkflowQuery_vEndDate"].ToString();
                        Session["FieldNames"] = GetFieldNames();
                    }
                    catch (Exception ex)
                    {
                        lblMessage.Text = ex.Message.ToString();
                    }
                }
                if (objResult.dsResult != null && objResult.dsResult.Tables.Count > 1 && objResult.dsResult.Tables[1].Rows.Count > 0)
                {
                    ControlCount = 0;
                    foreach (DataRow currentRow in objResult.dsResult.Tables[1].Rows)
                    {
                        ObjectType = currentRow["ObjectType"].ToString();
                        ControlCount++;
                        CreateQueryClause(ObjectType, currentRow, ControlCount);
                    }

                    Session["dtQueryClause"] = objResult.dsResult.Tables[1];
                }
            }
            else
            {
                PanelQueryHolder.Controls.Clear();
                ddlProcess.SelectedValue = "0";

                ddlWorkflow.Items.Clear();
                ListItem li = new ListItem("---Select---", "0");
                ddlWorkflow.Items.Add(li);

                ddlStage.Items.Clear();
                ddlStage.Items.Add(li);

                ddlStatus.Items.Clear();
                ddlStatus.Items.Add(li);

                txtStartDate.Text = string.Empty;
                txtEndDate.Text = string.Empty;
                Session.Remove("dtQueryClause");
            }

        }

        protected DBResult GetFieldNames()
        {
            DBResult objResult = new DBResult();
            if (Session["ProcessId"] != null && Session["WorkflowId"] != null)
            {

                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objAdvanceSearch.LoginToken = loginUser.LoginToken;
                objAdvanceSearch.LoginOrgId = loginUser.LoginOrgId;
                objAdvanceSearch.WorkflowAdvanceSearchProcessId = Convert.ToInt32(Session["ProcessId"]);
                objAdvanceSearch.WorkflowAdvanceSearchWorkflowId = Convert.ToInt32(Session["WorkflowId"]);
                objAdvanceSearch.WorkflowAdvanceSearchStageId = Convert.ToInt32(Session["StageId"]);
                if (Session["StageId"] != null && (string)Session["StageId"] != "0")
                {
                    objResult = objAdvanceSearch.ManageWorkflowAdvanceSearch(objAdvanceSearch, "GetStageFields");
                }
                else
                {
                    objResult = objAdvanceSearch.ManageWorkflowAdvanceSearch(objAdvanceSearch, "GetWorkflowFields");
                }

            }
            Session.Remove("StageId");
            return objResult;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            CheckAuthentication();
            string Query = string.Empty;



            int ProcessId = Convert.ToInt32(Session["ProcessId"]);
            int WorkflowId = Convert.ToInt32(Session["WorkflowId"]);
            Session["StageId"] = ddlStage.SelectedValue;
            int StageId = Convert.ToInt32(Session["StageId"]);
            Session["StatusId"] = ddlStatus.SelectedValue;
            int StatusId = Convert.ToInt32(Session["StatusId"]);

            string startdateTemp = txtStartDate.Text.Trim();
            string enddateTemp = txtEndDate.Text.Trim();
            string startdate = string.Empty;
            string enddate = string.Empty;


            try
            {
                if (startdateTemp != "")
                {
                    string stDateVal = startdateTemp.Replace(":", "/").Replace(" ", "/");
                    string dayfield = stDateVal.Split('/')[0];
                    string monthfield = stDateVal.Split('/')[1];
                    string yearfield = stDateVal.Split('/')[2];

                    startdate = yearfield + "-" + monthfield + "-" + dayfield;
                }
            }
            catch
            {
                startdate = "";
            }

            try
            {
                if (enddateTemp != "")
                {
                    string stDateVal = enddateTemp.Replace(":", "/").Replace(" ", "/");
                    string dayfield = stDateVal.Split('/')[0];
                    string monthfield = stDateVal.Split('/')[1];
                    string yearfield = stDateVal.Split('/')[2];

                    enddate = yearfield + "-" + monthfield + "-" + dayfield;
                }
            }
            catch
            {
                enddate = "";
            }


            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            objAdvanceSearch.LoginToken = loginUser.LoginToken;
            objAdvanceSearch.LoginOrgId = loginUser.LoginOrgId;
            objAdvanceSearch.WorkflowAdvanceSearchProcessId = ProcessId;
            objAdvanceSearch.WorkflowAdvanceSearchWorkflowId = WorkflowId;
            objAdvanceSearch.WorkflowAdvanceSearchStageId = StageId;
            objAdvanceSearch.WorkflowAdvanceSearchStatusId = StatusId;


            Query = "SELECT fd.WorkflowStageFieldData_iID AS [FieldData Id] " +
            ", fd.WorkflowStageFieldData_iProcessID  AS [Process Id] " +
            ", wp.WorkflowProcess_vName AS [Process Name] " +
            ", fd.WorkflowStageFieldData_iWorkFlowID AS [Workflow Id] " +
            ", w.Workflow_vName AS [Workflow Name] " +
            ", fd.WorkflowStageFieldData_iStageID	 AS [Stage Id] " +
            ", wst.WorkflowStage_vDisplayName AS [Stage Name] " +
            ", fd.WorkflowStageFieldData_iStatusID AS [Status ID] " +
            ", ISNULL(wstt.WorkflowStageStatus_vName,'-') AS [Status Name] " +
            ", fd.WorkflowStageFieldData_vOriginFileName AS [Original File Name] " +
            ", fd.WorkflowStageFieldData_vDocName AS [Document Name] " +
            ", dm.DOCTYPEMASTER_vName AS [DMS PRojectName] " +
            ", fd.WorkflowStageFieldData_vDocPhysicalPath AS[Path] " +
            ", fd.WorkflowStageFieldData_iUploadRecordID AS DMSID" +
            " FROM WorkflowStageFieldData fd " +
            "INNER JOIN WorkflowProcess wp ON wp.WorkflowProcess_iId= fd.WorkflowStageFieldData_iProcessID " +
            "INNER JOIN Workflows w ON w.Workflow_iId= fd.WorkflowStageFieldData_iWorkFlowID " +
            "INNER JOIN WorkflowStages wst ON wst.WorkflowStage_iId = fd.WorkflowStageFieldData_iStageID " +
            "LEFT OUTER JOIN WorkflowStageStatuses wstt ON wstt.WorkflowStageStatus_iId = fd.WorkflowStageFieldData_iStatusID " +
            "LEFT OUTER JOIN DOCTYPEMASTER dm ON dm.DOCTYPEMASTER_iID = w.Workflow_iDMSProjectId " +
            "WHERE fd.WorkflowStageFieldData_iOrgID = " + loginUser.LoginOrgId.ToString() + " " +
            "AND ( ISNULL(" + ProcessId.ToString() + ",0) = 0 OR fd.WorkflowStageFieldData_iProcessID = " + ProcessId.ToString() + ") " +
            "AND ( ISNULL(" + WorkflowId.ToString() + ",0) = 0 OR fd.WorkflowStageFieldData_iWorkFlowID = " + WorkflowId.ToString() + ") " +
            "AND ( ISNULL(" + StageId.ToString() + ",0) = 0 OR fd.WorkflowStageFieldData_iStageID = " + StageId.ToString() + ") " +
            "AND ( ISNULL(" + StatusId.ToString() + ",0) = 0 OR fd.WorkflowStageFieldData_iStatusID = " + StatusId.ToString() + ") " +
            "AND ( ISNULL('" + startdate.ToString() + "','') = '' OR  fd.WorkflowStageFieldData_dUploadedOn >= '" + startdate.ToString() + "') " +
            "AND ( ISNULL('" + enddate.ToString() + "','') = '' OR  fd.WorkflowStageFieldData_dUploadedOn <= '" + enddate.ToString() + "') ";

            objAdvanceSearch.WorkflowAdvanceSearchDynamicQuery = Query;

            string xml = GetQueryClauseXML();
            objAdvanceSearch.WorkflowAdvanceQueryClause = xml;

            objResult = objAdvanceSearch.ManageWorkflowAdvanceSearch(objAdvanceSearch, "ExecuteDynamicQuery");
            GridSearchResult.DataSource = objResult.dsResult.Tables[0];
            GridSearchResult.DataBind();
        }


        protected void GridSearchResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //// Hide unwanted rows

            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[1].Style.Add("display", "none");//Field Data Id
                e.Row.Cells[2].Style.Add("display", "none");//Process Id
                e.Row.Cells[4].Style.Add("display", "none");//Workflow Id
                e.Row.Cells[6].Style.Add("display", "none");//Stage Id
                e.Row.Cells[8].Style.Add("display", "none");//Status Id
                e.Row.Cells[13].Style.Add("display", "none");//path

                e.Row.Cells[0].Style.Add("width", "20px");
                e.Row.Cells[3].Style.Add("width", "80px");
                e.Row.Cells[5].Style.Add("width", "80px");
                e.Row.Cells[7].Style.Add("width", "80px");
                e.Row.Cells[9].Style.Add("width", "80px");
                e.Row.Cells[10].Style.Add("width", "80px");
                e.Row.Cells[11].Style.Add("width", "80px");
                e.Row.Cells[12].Style.Add("width", "80px");
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string ProcessId = HttpUtility.UrlEncode(Encrypt(e.Row.Cells[2].Text)); //ProcessId
                string WorkflowId = HttpUtility.UrlEncode(Encrypt(e.Row.Cells[4].Text)); //WorkflowId
                string StageId = HttpUtility.UrlEncode(Encrypt(e.Row.Cells[6].Text)); //StageId
                string FeildDataId = HttpUtility.UrlEncode(Encrypt(e.Row.Cells[1].Text)); //FieldDataId
                string DMSID = e.Row.Cells[14].Text; //WorkflowStageFieldData_iUploadRecordID
                string docPhysicalPath = e.Row.Cells[13].Text; //WorkflowStageFieldData_iUploadRecordID

                string QueryString = "?ProcessId=" + ProcessId + "&WorkflowId=" + WorkflowId + "&StageId=" + StageId + "&FieldDataId=" + FeildDataId + "&PageMode=" + "ViewMode";
                string URL = "WorkflowDataEntry_View.aspx" + QueryString;

                e.Row.Attributes.Add("ondblclick", " openMyModal('" + URL + "'); return false;");

                LinkButton lnkView = (LinkButton)e.Row.FindControl("lnkView");
                lnkView.Attributes.Add("onclick", " openMyModal('" + URL + "'); return false;");

                LinkButton lnkDocDownload = (LinkButton)e.Row.FindControl("lnkDocDownload");
                if (DMSID == "0")
                {
                    lnkDocDownload.Visible = false;
                }
                else
                {
                    lnkDocDownload.Visible = true;
                    string Filepath = beforedot(docPhysicalPath) + ".zip";

                    lnkDocDownload.Attributes.Add("onclick", " openMyWindow('DownloadDocument.aspx?FilePath=" + HttpUtility.UrlEncode(Filepath) + "'); return false;");
                    //lnkDocDownload.PostBackUrl = "../../Secure/Core/DocumentDownloadDetails.aspx?id=" + DMSID + "&docid=0&depid=0&active=1&PageNo=1&MainTagId=0&SubTagId=0&Search=''&Page=DocumentDownload&Watermark=''";
                }

            }

        }

        protected void SaveQueryName(object sender, EventArgs e)
        {
            CheckAuthentication();
            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            objAdvanceSearch.LoginToken = loginUser.LoginToken;
            objAdvanceSearch.LoginOrgId = loginUser.LoginOrgId;
            Session["ProcessId"] = ddlProcess.SelectedValue;
            objAdvanceSearch.WorkflowAdvanceSearchProcessId = Convert.ToInt32(Session["ProcessId"]);
            Session["WorkflowId"] = ddlWorkflow.SelectedValue;
            objAdvanceSearch.WorkflowAdvanceSearchWorkflowId = Convert.ToInt32(Session["WorkflowId"]);
            Session["StageId"] = ddlStage.SelectedValue;
            objAdvanceSearch.WorkflowAdvanceSearchStageId = Convert.ToInt32(Session["StageId"]);
            Session["StatusId"] = ddlStatus.SelectedValue;
            objAdvanceSearch.WorkflowAdvanceSearchStatusId = Convert.ToInt32(Session["StatusId"]);
            objAdvanceSearch.WorkflowAdvanceQueryName = txtQuery.Text.Trim();
            objAdvanceSearch.WorkflowAdvanceSearchQueryStartDate = txtStartDate.Text.Trim();
            objAdvanceSearch.WorkflowAdvanceSearchQueryEndDate = txtEndDate.Text.Trim();
            string xml = GetQueryClauseXML();
            objAdvanceSearch.WorkflowAdvanceQueryClause = xml;
            objResult = objAdvanceSearch.ManageWorkflowAdvanceSearch(objAdvanceSearch, "SaveQuery");
            handleDBResult(objResult);

            int selIndex = 0;
            if (objResult.ErrorState == 0)
            {
                BindSavedQueriesDropDown();
                int cnt = 0;
                foreach (ListItem li in ddlQueries.Items)
                {
                    if (li.Text == txtQuery.Text.Trim())
                    {
                        selIndex = cnt;
                        break;
                    }
                    cnt++;
                }

                ddlQueries.SelectedIndex = selIndex;
            }

        }

        protected void btnSaveQuery_Click(object sender, EventArgs e)
        {
            CheckAuthentication();
            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            objAdvanceSearch.LoginToken = loginUser.LoginToken;
            objAdvanceSearch.LoginOrgId = loginUser.LoginOrgId;
            string xml = GetQueryClauseXML();
            objAdvanceSearch.WorkflowAdvanceQueryClause = xml;
            Session["ProcessId"] = ddlProcess.SelectedValue;
            objAdvanceSearch.WorkflowAdvanceSearchProcessId = Convert.ToInt32(Session["ProcessId"]);
            Session["WorkflowId"] = ddlWorkflow.SelectedValue;
            objAdvanceSearch.WorkflowAdvanceSearchWorkflowId = Convert.ToInt32(Session["WorkflowId"]);
            Session["StageId"] = ddlStage.SelectedValue;
            objAdvanceSearch.WorkflowAdvanceSearchStageId = Convert.ToInt32(Session["StageId"]);
            Session["StatusId"] = ddlStatus.SelectedValue;
            objAdvanceSearch.WorkflowAdvanceSearchStatusId = Convert.ToInt32(Session["StatusId"]);
            objAdvanceSearch.WorkflowAdvanceQueryId = Convert.ToInt32(ddlQueries.SelectedValue);
            objAdvanceSearch.WorkflowAdvanceSearchQueryStartDate = txtStartDate.Text.Trim();
            objAdvanceSearch.WorkflowAdvanceSearchQueryEndDate = txtEndDate.Text.Trim();
            objResult = objAdvanceSearch.ManageWorkflowAdvanceSearch(objAdvanceSearch, "EditQuery");
            handleDBResultEdit(objResult);
        }


        private void handleDBResult(DBResult objResult)
        {
            lblPopUpErrorMessage.Text = "";
            lblMessage.Text = "";
            if (objResult.ErrorState == 0)
            {
                // Success
                if (objResult.Message.Length > 0)
                    lblPopUpErrorMessage.Text = (objResult.Message);
                lblMessage.Text = (objResult.Message);
                hdnSaveResult.Value = "Success";
            }
            else if (objResult.ErrorState == -1)
            {
                hdnSaveResult.Value = "ERROR";
                // Warning
                if (objResult.Message.Length > 0)
                {
                    lblPopUpErrorMessage.Text = (objResult.Message);

                }
            }
            else if (objResult.ErrorState == 1)
            {
                hdnSaveResult.Value = "ERROR";
                // Error
                if (objResult.Message.Length > 0)
                {
                    lblPopUpErrorMessage.Text = (objResult.Message);

                }
            }
        }

        private void handleDBResultEdit(DBResult objResult)
        {
            lblPopUpErrorMessage.Text = "";
            lblMessage.Text = "";
            if (objResult.ErrorState == 0)
            {
                // Success
                if (objResult.Message.Length > 0)
                    lblMessage.Text = (objResult.Message);
                hdnSaveResult.Value = "Success";
            }
            else if (objResult.ErrorState == -1)
            {
                hdnSaveResult.Value = "ERROR";
                // Warning
                if (objResult.Message.Length > 0)
                {
                    lblMessage.Text = (objResult.Message);

                }
            }
            else if (objResult.ErrorState == 1)
            {
                hdnSaveResult.Value = "ERROR";
                // Error
                if (objResult.Message.Length > 0)
                {
                    lblMessage.Text = (objResult.Message);

                }
            }
        }


        protected void GridSearchResult_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            CheckAuthentication();
            GridSearchResult.PageIndex = e.NewPageIndex;
            btnSearch_Click(sender, e);
        }

        protected void btnRemoveAllClause_Click(object sender, EventArgs e)
        {
            CheckAuthentication();
            PanelQueryHolder.Controls.Clear();
            if (Session["dtQueryClause"] != null)
            {
                DataTable dtQueryClause = new DataTable();
                dtQueryClause = (DataTable)Session["dtQueryClause"];

                dtQueryClause.Rows.Clear();

                Session["dtQueryClause"] = dtQueryClause;
            }
        }

    }
}