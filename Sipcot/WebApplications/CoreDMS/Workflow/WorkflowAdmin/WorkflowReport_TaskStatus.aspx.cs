/* ============================================================================  
Author     : 
Create date: 
===============================================================================  
** Change History   
** Date:          Author:            Issue ID                         Description:  
** ----------   -------------       ----------                  ----------------------------
*17 Apr 2015      Gokul               DMS5-3946               Applying rights an permissions in all pages as per user group rights!   
 7/7/2015         Sharath             DMS5-4388               If Process, Workflow, Stage contains only one item then by default select it
*11/3/2015        harikrishna.k       Task DMSENH6-5215       unwanted records coming in grid as per status
=============================================================================== */

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lotex.EnterpriseSolutions.CoreBE;
using WorkflowBAL;
using WorkflowBLL.Classes;
using Lotex.Common;

namespace Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin
{

    public partial class WorkflowReport_TaskStatus : PageBase
    {
        public string pageRights = string.Empty; /* DMS5-3946 A */        
        private string curProcess = string.Empty;
        private string prevProcess = string.Empty;
        private string curWorkflow = string.Empty;
        private string prevWorkflow = string.Empty;

        WorkflowReports ObjReport = new WorkflowReports();
        protected void Page_Load(object sender, EventArgs e)
        {
            CheckAuthentication();           
            pageRights = GetPageRights();/* DMS5-3946 A */
            if (!IsPostBack)
            {
                ApplyPageRights(pageRights, this.Form.Controls); /* DMS5-3946 A */
                //Start: ------------------SitePath Details ----------------------
                Label sitePath = (Label)Master.FindControl("SitePath");
                sitePath.Text = "WorkFlow Reports >> Task Status Reports";
                //End: ------------------SitePath Details ------------------------

                if (Session["ProcessId"] != null)
                {
                    Session.Remove("ProcessId");
                }

                BindprocessDropDown();

                ListItem li = new ListItem("---Select---", "0");
                ddlWorkflow.Items.Clear();
                ddlWorkflow.Items.Add(li);

                ddlStage.Items.Clear();
                ddlStage.Items.Add(li);

                ddlStatus.Items.Clear();
                ddlStatus.Items.Add(li);
            }
        }

        protected void BindprocessDropDown()
        {
            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            WorkflowAdvanceSearchBLL objAdvanceSearch = new WorkflowAdvanceSearchBLL();
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

        
        private DBResult GetReport()
        {
            DBResult result = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            ObjReport.LoginOrgId = loginUser.LoginOrgId;
            ObjReport.LoginToken = loginUser.LoginToken;

            try
            {
                ObjReport.ProcessId = Convert.ToInt32(ddlProcess.SelectedValue);
                ObjReport.WorkFlowId = Convert.ToInt32(ddlWorkflow.SelectedValue);
                ObjReport.StageId = Convert.ToInt32(ddlStage.SelectedValue);
                ObjReport.StatusId = Convert.ToInt32(ddlStatus.SelectedValue);
                ObjReport.StatusName = Convert.ToString(ddlStatus.SelectedItem);
                

                Session["ProcessId"] = ddlProcess.SelectedValue;
                Session["WorkflowId"] = ddlWorkflow.SelectedValue;
                Session["StageId"] = ddlStage.SelectedValue;
                Session["StatusId"] = ddlStatus.SelectedValue;

            }
            catch
            {
            }
                         
                return result = ObjReport.ManageWorkflowReports(ObjReport, "ReportTaskStatusSummary");          
        }
        // Task DMSENH6-5215 BS A
        private void Message(Control cntrl, string msg)
        {
            string script = "<script>alert('" + msg + "');</script>";
            ScriptManager.RegisterStartupScript(cntrl, this.GetType(), "JSCR", script, false);
            //this.RegisterClientScriptBlock("alertMsg", "<script>alert('" + msg + "');</script>");
        }
        // Task DMSENH6-5215 BE A

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
                GridReport.DataSource = result.dsResult.Tables[0];
                GridReport.DataBind();
            }
            //Task DMSENH6-5215 BS A
            else
            {
                GridReport.DataSource = null;
                GridReport.DataBind();
            }
            //Task DMSENH6-5215 BE A
 
        }

        protected void GridReport_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                curProcess = e.Row.Cells[0].Text.ToString(); //ProcessName
                curWorkflow = e.Row.Cells[1].Text.ToString(); //WorkflowName

                if (curProcess == prevProcess)
                {
                    e.Row.Cells[0].Text = "";
                }
                else
                {
                    prevProcess = curProcess;
                }

                if (curWorkflow == prevWorkflow)
                {
                    e.Row.Cells[1].Text = "";
                }
                else
                {
                    prevWorkflow = curWorkflow;
                }

                for (int i = 3; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].HorizontalAlign = HorizontalAlign.Center;
                    if ((e.Row.Cells[i].Text.Trim() == "0") || (e.Row.Cells[i].Text.Trim() == "") || (e.Row.Cells[i].Text.Trim() == "&nbsp;"))
                    {
                        e.Row.Cells[i].Text = "-";
                    }
                    else if (Convert.ToInt32(e.Row.Cells[i].Text.Length) > 0)
                    {
                        e.Row.Cells[i].ForeColor = System.Drawing.Color.Red;
                    }
                    else if (Convert.ToInt32(e.Row.Cells[i].Text.Length) <= 0)
                    {
                        e.Row.Cells[i].Text = "-";
                    }
                }
                
            }
        }

        protected void ddlProcess_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckAuthentication();
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
            }
            else
            {
                ListItem li = new ListItem("---Select---", "0");
                ddlWorkflow.Items.Clear();
                ddlWorkflow.Items.Add(li);

                ddlStage.Items.Clear();
                ddlStage.Items.Add(li);

                ddlStatus.Items.Clear();
                ddlStatus.Items.Add(li);

                GridReport.DataSource = null;
                GridReport.DataBind();
            }
        }

        protected void ddlWorkflow_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckAuthentication();
            if (ddlWorkflow.SelectedValue != "0")
            {
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

        protected void ddlStage_SelectedIndexChanged(object sender, EventArgs e)
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

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            CheckAuthentication();
            BindPendingListGrid(GetReport());
        }

        protected void GridReport_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            CheckAuthentication();
            GridReport.PageIndex = e.NewPageIndex;
            BindPendingListGrid(GetReport());

        }
    }
}