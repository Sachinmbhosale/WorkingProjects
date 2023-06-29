/* ============================================================================  
Author     : 
Create date: 
===============================================================================  
** Change History   
** Date:          Author:            Issue ID            Description:  
** ------------------------------------------------------------------------------
** 03 Dec 2013          Mandatory   (UMF)                Set Mandatory for index fields
 *
 * Modified
 * 04/03/2015       Sabina              DMS5-3457       Add dataentryId to querystring
 * 03/25/2015		Sabina.k.v			DMS5-3586	    Validation on dataentry
 * 16/04/2015       Gokuldas.Palapata   DMS5-3919       Workflow Filter - Data and dropdown filter as per user mapping
 * 17 Apr 2015      Gokul               DMS5-3946       Applying rights an permissions in all pages as per user group rights!
 * 19 Apr 2015      Yogeesha Naik       DMS5-3956       Dataentry page is not loaded on selecting workitem and clicking on the Goto Data Entry button
 * 01-07-2015       Gokuldas.Palapatta  DMS5-4422       New task creation for an independent Workflow should be allowed at all levels & should follow the hierarchy/rule as defined in the system
 * 01-07-2015       Gokuldas.Palapatta  DMS5-4421       New task creation (ability to create a new task/record) for a DMS-based workflow should not be allowed
 * 3/7/2015         Sharath             DMS5-4388       If Process, Workflow, Stage contains only one item then by default select it
 * 15 Jul 2015      sharath             DMSENH6-4640    Closure of activities on Workflow    
 * =============================================================================== */
using System;
using System.Web;
using System.Web.UI.WebControls;
using Lotex.EnterpriseSolutions.CoreBE;
using WorkflowBAL;
using WorkflowBLL.Classes;
using Lotex.Common;

namespace Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin
{
    public partial class WorkflowPendingList : PageBase
    {

        WorkflowPendingListBLL ObjPendigList = new WorkflowPendingListBLL();
        WorkflowStage objStage = new WorkflowStage();
        public string pageRights = string.Empty; /* DMS5-3946 A */      

        protected void Page_Load(object sender, EventArgs e)
        {
            CheckAuthentication();
            pageRights = GetPageRights("WorkflowPendingList");/* DMS5-3946 A */
          
            if (!IsPostBack)
            {
                ApplyPageRights(pageRights, ButtonPanel.Controls); /* DMS5-3946 A */

                //Start: ------------------SitePath Details ----------------------
                Label sitePath = (Label)Master.FindControl("SitePath");
                sitePath.Text = "WorkFlow Tasks >> Task Items";
                //End: ------------------SitePath Details ------------------------

                BindprocessDropDown();

                ListItem li = new ListItem("---Select---", "0");           

                ddlWorkflow.Items.Clear();
                ddlWorkflow.Items.Add(li);           

                ddlStage.Items.Clear();
                ddlStage.Items.Add(li);

                ddlStatus.Items.Clear();
                ddlStatus.Items.Add(li);

                btnGotoDataEntry.Visible = false;
                btnAddNew.Visible = false;

                if (Session["PendingListProcessId"] != null)
                {
                    ddlProcess.SelectedValue = Session["PendingListProcessId"].ToString();
                    if (ddlProcess.SelectedValue != "0")
                    {
                        ddlProcess_SelectedIndexChanged(ddlProcess, new EventArgs());
                    }
                }

                if (Session["PendingListWorkflowId"] != null)
                {
                    ddlWorkflow.SelectedValue = Session["PendingListWorkflowId"].ToString();
                    if (ddlWorkflow.SelectedValue != "0")
                    {
                        ddlWorkflow_SelectedIndexChanged(ddlWorkflow, new EventArgs());
                    }
                }

                if (Session["PendingListStageId"] != null)
                {
                    ddlStage.SelectedValue = Session["PendingListStageId"].ToString();
                    if (ddlStage.SelectedValue != "0")
                    {
                        ddlStage_SelectedIndexChanged(ddlStage, new EventArgs());
                    }
                }

                if (Session["PendingListStatusId"] != null)
                {
                    ddlStatus.SelectedValue = Session["PendingListStatusId"].ToString();
                }

                BindPendingListGrid(Getpendinglist());
            }
        }

        protected void BindprocessDropDown()
        {
            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            WorkflowAdvanceSearchBLL objAdvanceSearch = new WorkflowAdvanceSearchBLL()
            {
                LoginToken = loginUser.LoginToken,
                LoginOrgId = loginUser.LoginOrgId
            };
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

       

        protected void btnGotoDataEntry_Click(object sender, EventArgs e)
        {
            CheckAuthentication();

            //DMS5-3956 BS -- Removed old code
            foreach (GridViewRow row in GridPendingList.Rows)
            {
                RadioButton chk = row.FindControl("RowSelector") as RadioButton;

                if (chk.Checked)
                {
                    DBResult objResult = new DBResult();

                    objStage.ProcessId = Convert.ToInt32(row.Cells[2].Text); //ProcessId
                    objStage.WorkflowId = Convert.ToInt32(row.Cells[4].Text); //WorkflowId
                    objStage.StageId = Convert.ToInt32(row.Cells[6].Text); //StageId

                    string processId = HttpUtility.UrlEncode(Encrypt(Convert.ToString(objStage.ProcessId)));
                    string workflowId = HttpUtility.UrlEncode(Encrypt(Convert.ToString(objStage.WorkflowId))); //WorkflowId
                    string stageId = HttpUtility.UrlEncode(Encrypt(Convert.ToString(objStage.StageId))); //StageId

                    string feildDataId = HttpUtility.UrlEncode(Encrypt(row.Cells[1].Text)); //FeildDataId
                    string dataEntryType = HttpUtility.UrlEncode(Encrypt(row.Cells[14].Text)); // Data Entry Type

                    //DMSENH6-4640 BS
                    string isFinalStatus = string.Empty;
                    if (row.Cells[17].Text.Equals("Yes"))
                    {
                         isFinalStatus = HttpUtility.UrlEncode(Encrypt(row.Cells[17].Text)); // Is workitem has reached (Final Status) 
                    }
                    //DMSENH6-4640 BE

                    //DMS5-3457A check the type of dataentry and divert based on it                    
                    string URL = "?ProcessId=" + processId + "&WorkflowId=" + workflowId + "&StageId=" + stageId + "&FieldDataId=" + feildDataId + "&DataEntryType=" + dataEntryType + "&IsFinalStatus="  + isFinalStatus;

                    // Call database to check fields or co-ordinates not available
                    UserBase loginUser = (UserBase)Session["LoggedUser"];
                    objStage.LoginOrgId = loginUser.LoginOrgId;
                    objStage.LoginToken = loginUser.LoginToken;
                    objResult = objStage.ManageWorkflowStages(objStage, "CheckMappingOfStage");

                    string dataEntryMode = row.Cells[14].Text; // Data Entry Type
                    if (dataEntryMode == "Normal")
                    {
                        Response.Redirect("~/Workflow/WorkflowAdmin/WorkflowDataEntry.aspx" + URL, false);
                    }

                    // If Fields or co-ordinates not available do not redirect
                    if (objResult.ErrorState == 0)
                    {
                        if (dataEntryMode == "Guided")
                        {
                            Response.Redirect("~/Workflow/WorkflowAdmin/ManageGuidedDataEntry.aspx" + URL, false);
                        }
                        else if (dataEntryMode == "Form")
                        {
                            Response.Redirect("~/Workflow/WorkflowAdmin/ManageFormDataEntry.aspx" + URL, false);
                        }
                    }
                    else
                    {
                        lblMessage.Text = objResult.Message;
                    }
                }
            }
            //DMS5-3956BE

        }

        protected void GridPendingList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            CheckAuthentication();
            GridPendingList.PageIndex = e.NewPageIndex;
            BindPendingListGrid(Getpendinglist());

        }
        private DBResult Getpendinglist()
        {
            DBResult result = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            ObjPendigList.LoginOrgId = loginUser.LoginOrgId;
            ObjPendigList.LoginToken = loginUser.LoginToken;

            try
            {
                ObjPendigList.ProcessId = Convert.ToInt32(ddlProcess.SelectedValue);
                ObjPendigList.WorkFlowId = Convert.ToInt32(ddlWorkflow.SelectedValue);
                ObjPendigList.StageId = Convert.ToInt32(ddlStage.SelectedValue);
                ObjPendigList.StatusId = Convert.ToInt32(ddlStatus.SelectedValue);

                Session["PendingListProcessId"] = ddlProcess.SelectedValue;
                Session["PendingListWorkflowId"] = ddlWorkflow.SelectedValue;
                Session["PendingListStageId"] = ddlStage.SelectedValue;
                Session["PendingListStatusId"] = ddlStatus.SelectedValue;
            }
            catch
            {
            }
            return result = ObjPendigList.ManageWorkflowPendingList(ObjPendigList, "BindPendingListGrid");

        }

        private void SaveNewWorkItem()
        {
            DBResult result = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            ObjPendigList.LoginOrgId = loginUser.LoginOrgId;
            ObjPendigList.LoginToken = loginUser.LoginToken;

            try
            {
                ObjPendigList.ProcessId = Convert.ToInt32(ddlProcess.SelectedValue);
                ObjPendigList.WorkFlowId = Convert.ToInt32(ddlWorkflow.SelectedValue);
                ObjPendigList.StageId = Convert.ToInt32(ddlStage.SelectedValue); /* DMS5-4422  A  */
                //ObjPendigList.StageId = 0;
                ObjPendigList.StatusId = 0;

                Session["ProcessId"] = ddlProcess.SelectedValue;
                Session["WorkflowId"] = ddlWorkflow.SelectedValue;
            }
            catch
            {
            }
            result = ObjPendigList.ManageWorkflowPendingList(ObjPendigList, "AddNewWorkItem");

            if (result.ErrorState == 0)
            {
                BindPendingListGrid(Getpendinglist());
            }
            else
            {
                handleDBResult(result);
            }
        }

        private void handleDBResult(DBResult objResult)
        {
            lblMessage.Text = "";
            if (objResult.ErrorState == 0)
            {
                // Success
                if (objResult.Message.Length > 0)
                {
                    lblMessage.Text = (objResult.Message);
                }
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

        protected void BindPendingListGrid(DBResult result)
        {
            if (result.dsResult.Tables.Count > 0 && result.dsResult != null)
            {
                GridPendingList.DataSource = result.dsResult.Tables[0];
                GridPendingList.DataBind();
                btnGotoDataEntry.Visible = true;
                btnAddNew.Visible = true;
            }

        }

        protected void GridPendingList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //// Hide unwanted rows

            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
            {
                //e.Row.Cells[1].Style.Add("display", "none");//Field Data Id
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
                e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center; //select radio button
                //DMS5-3956 A  -- Removed old code and used below javascript
                e.Row.Attributes.Add("ondblclick", "GoToDataEntry(this);");
            }
        }

        protected void ddlProcess_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                CheckAuthentication();
                ListItem li = new ListItem("---Select---", "0"); /*DMS5-3919 A */
                if (ddlProcess.SelectedValue != "0")
                {
                    DBResult objResult = new DBResult();
                    UserBase loginUser = (UserBase)Session["LoggedUser"];
                    WorkflowAdvanceSearchBLL objAdvanceSearch = new WorkflowAdvanceSearchBLL();
                    objAdvanceSearch.LoginToken = loginUser.LoginToken;
                    objAdvanceSearch.LoginOrgId = loginUser.LoginOrgId;
                    Session["ProcessId"] = ddlProcess.SelectedValue;
                    objAdvanceSearch.WorkflowAdvanceSearchProcessId = Convert.ToInt32(Session["ProcessId"]);
                    objResult = objAdvanceSearch.ManageWorkflowAdvanceSearch(objAdvanceSearch, "BindWorkflowSearchDropDown");
                    ddlWorkflow.DataSource = objResult.dsResult;
                    ddlWorkflow.DataTextField = "Workflow_vName";
                    ddlWorkflow.DataValueField = "Workflow_iId";
                    ddlWorkflow.DataBind();

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


                    // ddlStage.Items.Clear();/*DMS5-3919 A */
                    //ddlStage.Items.Add(li);/*DMS5-3919 A */

                    ddlStatus.Items.Clear();/*DMS5-3919 A */
                    ddlStatus.Items.Add(li);/*DMS5-3919 A */
                }
                else
                {
                    /*DMS5-3919 D */
                    ddlWorkflow.Items.Clear();
                    ddlWorkflow.Items.Add(li);

                    ddlStage.Items.Clear();
                    ddlStage.Items.Add(li);

                    ddlStatus.Items.Clear();
                    ddlStatus.Items.Add(li);

                    GridPendingList.DataSource = null;
                    GridPendingList.DataBind();

                }
            }
            catch(Exception ex)
            {
                lblMessage.Text = ex.Message.ToString();
            }
        }


        /* DMS5-4421 BS*/
        protected void CheckWorkFlowMappedWithProjectType()
        {
            

            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            WorkflowAdvanceSearchBLL objAdvanceSearch = new WorkflowAdvanceSearchBLL();
            objAdvanceSearch.LoginToken = loginUser.LoginToken;
            objAdvanceSearch.LoginOrgId = loginUser.LoginOrgId;
            
            objAdvanceSearch.WorkflowAdvanceSearchWorkflowId = Convert.ToInt32(Session["WorkflowId"]);
            objResult = objAdvanceSearch.ManageWorkflowAdvanceSearch(objAdvanceSearch, "GetDMSProjectTypeId");
            if (objResult.dsResult.Tables.Count > 0)
            {
                if (objResult.dsResult.Tables[0].Rows.Count > 0)
                {
                    int DMSProjectId = Convert.ToInt32(objResult.dsResult.Tables[0].Rows[0]["DMSProjectID"]);
                    if (DMSProjectId > 0)
                    {
                        btnAddNew.Enabled = false;

                    }
                    else
                    {
                        btnAddNew.Enabled = true;
                    }
                    ApplyPageRights(pageRights, ButtonPanel.Controls); /* DMS5-3946 A */
                }
            }

        }

        /* DMS5-4421 BE*/
        protected void ddlWorkflow_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                CheckAuthentication();
                if (ddlWorkflow.SelectedValue != "0")
                {
                    CheckWorkFlowMappedWithProjectType();/* DMS5-4421 A*/
                    DBResult objResult = new DBResult();
                    UserBase loginUser = (UserBase)Session["LoggedUser"];
                    WorkflowAdvanceSearchBLL objAdvanceSearch = new WorkflowAdvanceSearchBLL();
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

                    //DMS5-4388  BS
                    if (!(ddlWorkflow.SelectedValue.Equals(0)))
                    {
                        ddlStage_SelectedIndexChanged(ddlStage, new EventArgs());
                    }
                    //DMS5-4388  BE

                }
                else
                {
                    ListItem li = new ListItem("---Select---", "0");
                    ddlStage.Items.Clear();
                    ddlStage.Items.Add(li);

                    ddlStatus.Items.Clear();
                    ddlStatus.Items.Add(li);
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message.ToString();
            }
        }

        protected void ddlStage_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                CheckAuthentication();
                if (ddlStage.SelectedValue != "0")
                {
                    DBResult objResult = new DBResult();
                    UserBase loginUser = (UserBase)Session["LoggedUser"];
                    WorkflowAdvanceSearchBLL objAdvanceSearch = new WorkflowAdvanceSearchBLL();
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
                }
                else
                {
                    ListItem li = new ListItem("---Select---", "0");
                    ddlStatus.Items.Clear();
                    ddlStatus.Items.Add(li);
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message.ToString();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            CheckAuthentication();
            BindPendingListGrid(Getpendinglist());
        }

        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            CheckAuthentication();
            SaveNewWorkItem();
        }
    }
}