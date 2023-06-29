/* ============================================================================  
Author     : 
Create date: 
===============================================================================  
** Change History   
** Date:          Author:            Issue ID                         Description:  
** ----------   -------------       ----------                  ----------------------------
*17 Apr 2015   Gokul        DMS5-3946       Applying rights an permissions in all pages as per user group rights!   
 *08 Apr 2015  Sabina       DMS5-4122       Button not enabled in workflow pages 
 17 june 2015  Sharath		DMS5-4382       User experience for Workflow Stage user needs improvement
 28 july 2015  Sharath		DMSENH6-4732    Deletion options within Workflow
 *25/08/2015   Sharath      DMSENH6-4795    Clone - copy a defined Workflow  
 *25/8/2015   Gokul         DMSENH6-4776    Data Captured Form usage
 *31/08/2015  Sharath       DMSENH6-4924    Once a workflow is confirmed, the "Confirm" button in the preview popup is not disabled
=============================================================================== */

using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WorkflowBLL.Classes;
using Lotex.EnterpriseSolutions.CoreBE;
using WorkflowBAL;
using System.Web.Services;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;
using System.Web.Configuration;
using Lotex.Common;

namespace Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin
{
    public partial class ManageWorkflow : PageBase
    {
        #region GLOBAL DECLARATION

        Workflows objWorkflow = new Workflows();
        public string pageRights = string.Empty; /* DMS5-3946 A */

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            CheckAuthentication();
            pageRights = GetPageRights("CONFIGURATION");/* DMS5-3946 A,DMS5-4122M added parameter configuration */
            //Start: ------------------SitePath Details ----------------------
            Label sitePath = (Label)Master.FindControl("SitePath");
            sitePath.Text = "WorkFlow >> Process >> Manage Workflows";
            //End: ------------------SitePath Details ------------------------


            if (!Page.IsPostBack)
            {
                ApplyPageRights(pageRights, this.Form.Controls); /* DMS5-3946 A */
                if (Request.QueryString["ProcessId"] != null)
                {
                    // Store process id in view state for later use
                    // ViewState["ProcessId"] = Decrypt(HttpUtility.UrlDecode(Request.QueryString["ProcessId"]));
                    Session["ProcessId"] = Decrypt(HttpUtility.UrlDecode(Request.QueryString["ProcessId"]));
                    // Get the available workflow details for given processid and bind to grid
                    int iCurrentProcessID = Convert.ToInt32(Session["ProcessId"]);
                    BindWorkflowGridview(GetWorkflow(iCurrentProcessID));
                    BindSortOrderToListBox();
                    BindProjectDropDown();
                    BindOrganizationDropDown();


                    //Fill textbox clone same as workflow name selection
                    ddlWorkflowClone.Attributes.Add("onchange", "FillWorkflowName();");
                }
                else
                {
                    throw new Exception("Querystring empty.");
                }
            }

        }



        #region "Workflow"

        protected void BindProjectDropDown()
        {
            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            objWorkflow.LoginToken = loginUser.LoginToken;
            objWorkflow.LoginOrgId = loginUser.LoginOrgId;
            objWorkflow.WorkFlowProcessId = Convert.ToInt32(Session["ProcessId"].ToString());
            objResult = objWorkflow.ManageWorkflows(objWorkflow, "BindDMSProjectDropDown");
            ddlDmsProject.DataSource = objResult.dsResult;
            ddlDmsProject.DataTextField = "DOCTYPEMASTER_vName";
            ddlDmsProject.DataValueField = "DOCTYPEMASTER_iID";
            ddlDmsProject.DataBind();

        }



        //To load sort order to listbox
        //DMS5-4382 BS
        protected void BindSortOrderToListBox()
        {
            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            objWorkflow.LoginToken = loginUser.LoginToken;
            objWorkflow.LoginOrgId = loginUser.LoginOrgId;
            objWorkflow.WorkFlowProcessId = Convert.ToInt32(Session["ProcessId"].ToString());
            objResult = objWorkflow.ManageWorkflows(objWorkflow, "BindSortOrderToListBox");
            if (objResult.dsResult.Tables[0].Rows.Count > 0)
            {
                lstSortOrder.DataSource = objResult.dsResult;
                lstSortOrder.DataTextField = "SortOrder";
                lstSortOrder.DataValueField = "SortOrderId";
                lstSortOrder.DataBind();
            }



        }
        //DMS5-4382 BE

        private DBResult GetWorkflow(int iProcessId)
        {
            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            objWorkflow.LoginToken = loginUser.LoginToken;
            objWorkflow.LoginOrgId = loginUser.LoginOrgId;

            //Make Clone button visible to only superadmin
            if (loginUser.GroupId.Equals(1) && (loginUser.GroupName.Equals("Super Admin")))
            {
                btnClone.Visible = true;
            }

            objWorkflow.WorkFlowProcessId = iProcessId;
            return objWorkflow.ManageWorkflows(objWorkflow, "GetAllWorkflows");
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {

            CheckAuthentication();
            string action = "AddWorkflow";
            int iCurrentProcessID = Convert.ToInt32(Session["ProcessId"].ToString());

            try
            {
                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objWorkflow.LoginToken = loginUser.LoginToken;
                objWorkflow.LoginOrgId = loginUser.LoginOrgId;
                objWorkflow.WorkflowDMSProjectId = Convert.ToInt32(ddlDmsProject.SelectedValue);
                objWorkflow.WorkFlowProcessId = iCurrentProcessID;
                objWorkflow.WorkFlowName = txtWorkflowName.Text.Trim();
                objWorkflow.WorkFlowDescription = txtWorkflowDescription.Text.Trim();
                objWorkflow.WorkflowIsActive = chkActive.Checked;
                objWorkflow.WorkflowPriority = lstSortOrder.SelectedItem.Text;
                string strSaveStatus = hdnSaveStatus.Value;

                if (strSaveStatus == "Save Changes")
                {
                    objWorkflow.WorkflowId = Convert.ToInt32(hiddenWorkflowId.Value);
                    objWorkflow.WorkflowIsDeleted = chkActive.Checked ? false : true;
                    action = "EditWorkflow";
                }

                // Save update datbase and get the result
                objResult = objWorkflow.ManageWorkflows(objWorkflow, action);
                handleDBResult(objResult);

                //only if success
                if (objResult.ErrorState == 0)
                {
                    clearControls();
                    // BInd result to gridview
                    BindWorkflowGridview(GetWorkflow(Convert.ToInt32(Session["ProcessId"].ToString())));
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
            txtWorkflowName.Text = string.Empty;
            txtWorkflowDescription.Text = string.Empty;
            chkActive.Checked = false;
        }

        protected void BindWorkflowGridview(DBResult objResult)
        {
            if (objResult.dsResult != null && objResult.dsResult.Tables.Count > 0)
            {
                gridWorkflow.DataSource = objResult.dsResult.Tables[0];
                gridWorkflow.DataBind();
                btnGotoStage.Enabled = true;

                //Multilanguage implementation for Grid headers
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                if (loginUser.LanguageCode != "en-US")
                {
                    WorkFlowCommonFns.SetGridHeaderForMultiLanguage(ref gridWorkflow, "WORKFLOW", loginUser.LoginToken, loginUser.LoginOrgId, loginUser.LanguageID, loginUser.UserId);
                }
            }
            else
            {
                btnGotoStage.Enabled = false;
            }
        }

        #endregion



        protected void btnGotoStage_Click(object sender, EventArgs e)
        {
            CheckAuthentication();
            foreach (GridViewRow row in gridWorkflow.Rows)
            {
                RadioButton chk = row.FindControl("RowSelector") as RadioButton;

                if (chk.Checked)
                {
                    string ProcessId = Session["ProcessId"].ToString(); //ProcessId
                    ProcessId = HttpUtility.UrlEncode(Encrypt(ProcessId));
                    string WorkflowId = Encrypt(row.Cells[3].Text); //WorkflowId
                    string workflowConfirmed = string.Empty;//WorkflowConfirmed
                    string QueryString = "?ProcessId=" + ProcessId + "&WorkflowId=" + (WorkflowId) + "&WorkflowConfirmed=" + workflowConfirmed;

                    Response.Redirect("~/Workflow/WorkflowAdmin/ManageStages.aspx" + QueryString);
                }
            }
        }

        protected void gridWorkflow_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Hide unwanted columns
            if (e.Row.RowType == DataControlRowType.Header || e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[0].Style.Add("width", "25px"); //edit
                e.Row.Cells[4].Style.Add("width", "40px"); // name
                e.Row.Cells[5].Style.Add("width", "180px"); //desc
                e.Row.Cells[7].Style.Add("width", "50px"); //DmsProject name
                e.Row.Cells[8].Style.Add("width", "20px"); //active
                e.Row.Cells[2].Style.Add("width", "20px"); //slno
                e.Row.Cells[13].Style.Add("width", "20px"); //Confirmation Status
                e.Row.Cells[1].Style.Add("display", "none"); //totalcount
                e.Row.Cells[3].Style.Add("display", "none");// wfid
                e.Row.Cells[9].Style.Add("display", "none");//Activestatus
                e.Row.Cells[10].Style.Add("display", "none");//Org ID
                e.Row.Cells[11].Style.Add("display", "none");//Docmaster Id
                e.Row.Cells[6].Style.Add("display", "none"); //org name (no more required)
                e.Row.Cells[12].Style.Add("display", "none");//SortOrder
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center; //edit
                e.Row.Cells[8].HorizontalAlign = HorizontalAlign.Center; //active

                string ProcessId = Session["ProcessId"].ToString(); //ProcessId
                string WorkflowId = Encrypt(e.Row.Cells[3].Text); //WorkflowId
                string WorkflowName = Encrypt(e.Row.Cells[4].Text); //WorkflowName
                string workflowConfirmed = HttpUtility.UrlEncode(Encrypt(e.Row.Cells[13].Text)); //WorkflowConfirmation             

                ProcessId = HttpUtility.UrlEncode(Encrypt(ProcessId));
                string QueryString = "?ProcessId=" + ProcessId + "&WorkflowId=" + (WorkflowId) + "&WorkflowConfirmed=" + workflowConfirmed;
                string URL = "ManageStages.aspx" + QueryString;

                // Redirect on row double click
                e.Row.Attributes.Add("ondblclick", "window.location.href ='" + URL + "'");
                LinkButton lnkManageWorkflowOwners = (LinkButton)e.Row.FindControl("lnkManageWorkflowOwners");
                lnkManageWorkflowOwners.PostBackUrl = "~/Workflow/WorkflowAdmin/ManageWorkflowOwners.aspx" + QueryString + "&WorkflowName=" + WorkflowName;

            }
        }

        protected void btnGoBacktoProcess_Click(object sender, EventArgs e)
        {
            CheckAuthentication();
            Response.Redirect("~/Workflow/WorkflowAdmin/ManageProcess.aspx");
        }

        protected void gridWorkflow_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            CheckAuthentication();
            gridWorkflow.PageIndex = e.NewPageIndex;
            BindWorkflowGridview(GetWorkflow((Convert.ToInt32(Session["ProcessId"]))));

        }

        // DMSENH6-4732 BS
        //Confirmation of workflow
        protected void btnConfirm_Click(object sender, EventArgs e)
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
                BindWorkflowGridview(GetWorkflow(objWorkflow.WorkFlowProcessId));
            }
        }

        //To find count of stages,users,status for thre particular workflow
        protected DataSet BindWorkflowStagesCount()
        {

            DBResult objResult = new DBResult();
            objWorkflow = GetDefalutParameters();
            //Display Stagecount,Feild count,statuscount,usercount for the particular workflow and process
            objResult = objWorkflow.ManageWorkflows(objWorkflow, "WorkflowStagesCounts");
            string WorkFlowCount = string.Empty;
            if (objResult.dsResult.Tables.Count > 0 && objResult.dsResult.Tables[0].Rows.Count > 0)
            {

                WorkFlowCount =  " Number of stages  : " + Convert.ToString(objResult.dsResult.Tables[0].Rows[0]["StageCount"] + " \\n");
                WorkFlowCount += " Number of users   : " + Convert.ToString(objResult.dsResult.Tables[0].Rows[0]["UserCount"] + " \\n");
                WorkFlowCount += " Number of fields  : " + Convert.ToString(objResult.dsResult.Tables[0].Rows[0]["FieldCount"] + " \\n");
                WorkFlowCount += " Number of status  : " + Convert.ToString(objResult.dsResult.Tables[0].Rows[0]["StatusCount"] + " \\n");
                hdnWorkFlowCount.Value = WorkFlowCount;

            }
            return objResult.dsResult;
        }

        protected void btnPreview_Click(object sender, EventArgs e)
        {

            string WorkflowId = GetWorkflowId();
            Session["WorkflowId"] = Convert.ToInt32(WorkflowId);
            BindWorkflowStagesCount();

            //call drawChart method to load chart
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Success", "google.load('visualization', '1', { packages: ['orgchart'], callback: drawChart });", true);
        }

        protected Workflows GetDefalutParameters()
        {
            int iCurrentProcessID = Convert.ToInt32(Session["ProcessId"].ToString());
            int WorkflowId = Convert.ToInt32(Session["WorkflowId"].ToString());
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            objWorkflow.LoginToken = loginUser.LoginToken;
            objWorkflow.LoginOrgId = loginUser.LoginOrgId;
            objWorkflow.WorkFlowProcessId = iCurrentProcessID;
            objWorkflow.WorkflowId = Convert.ToInt32(WorkflowId);
            return objWorkflow;
        }

        private string GetWorkflowId()
        {
            string WorkflowId = string.Empty;
            string WorkflowConfirmed = string.Empty;
            foreach (GridViewRow row in gridWorkflow.Rows)
            {
                //Find the Radio button control
                RadioButton rb = (RadioButton)row.FindControl("RowSelector");
                if (rb.Checked.Equals(true))
                {
                    WorkflowId = row.Cells[3].Text;//WorkflowId                    

                    //DMSENH6-4924 BS
                    WorkflowConfirmed = row.Cells[13].Text;//WorkflowConfirmed
                    if (WorkflowConfirmed.Equals("Confirmed"))
                    {
                        hdnconfirmed.Value = WorkflowConfirmed;
                    }
                    else
                    {
                        hdnconfirmed.Value = WorkflowConfirmed;

                    }
                    //DMSENH6-4924 BS
                }
            }

            return WorkflowId;

        }

        //Pictorial Chart
        [WebMethod]
        public static List<object> GetChartData()
        {
            return new ManageWorkflow().GetPictorialData();
        }

        protected List<object> GetPictorialData()
        {

            IDataReader reader;
            List<object> chartData = new List<object>();
            objWorkflow = GetDefalutParameters();
            reader = (IDataReader)objWorkflow.GetChartData(objWorkflow, "GetChartData");

            while (reader.Read())
            {
                chartData.Add(new object[]
             {
                 reader["Id"], reader["Name"], reader["ParentId"]
             });

            }

            reader.Close();


            return chartData;
        }

        // DMSENH6-4732 BE

        //DMSENH6-4795 BS
        //Bind organization drop down for Current logged user
        protected void BindOrganizationDropDown()
        {
            try
            {

                ListItem li = new ListItem("---Select---", "0");
                ddlProcessClone.Items.Clear();
                ddlProcessClone.Items.Add(li);

                ddlWorkflowClone.Items.Clear();
                ddlWorkflowClone.Items.Add(li);


                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objWorkflow.LoginToken = loginUser.LoginToken;
                objWorkflow.LoginOrgId = loginUser.LoginOrgId;
                objResult = objWorkflow.ManageWorkflows(objWorkflow, "BindOrganizationDropDown");
                ddlOrganizationClone.DataSource = objResult.dsResult;
                ddlOrganizationClone.DataTextField = "ORGS_vName";
                ddlOrganizationClone.DataValueField = "ORGS_iId";
                ddlOrganizationClone.DataBind();
            }
            catch (Exception ex)
            {
                lblMessageGrid.Text = ex.Message.ToString();
            }

        }

        //Bind process drop down which are active
        protected void BindProcessCloneDropDown()
        {

            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            objWorkflow.LoginToken = loginUser.LoginToken;
            objWorkflow.LoginOrgId = loginUser.LoginOrgId;
            objWorkflow.WorkFlowSelectedOrgId = Convert.ToInt32(ddlOrganizationClone.SelectedValue);
            objResult = objWorkflow.ManageWorkflows(objWorkflow, "BindProcessCloneDropDown");
            ddlProcessClone.DataSource = objResult.dsResult;
            ddlProcessClone.DataTextField = "WorkflowProcess_vName";
            ddlProcessClone.DataValueField = "WorkflowProcess_iId";
            ddlProcessClone.DataBind();
        }

        //Bind Workflow drop down which are active
        protected void BindWorkflowCloneDropDown()
        {
            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            objWorkflow.LoginToken = loginUser.LoginToken;
            objWorkflow.LoginOrgId = loginUser.LoginOrgId;
            objWorkflow.WorkFlowProcessId = Convert.ToInt32(ddlProcessClone.SelectedValue);
            objWorkflow.WorkFlowSelectedOrgId = Convert.ToInt32(ddlOrganizationClone.SelectedValue);
            objResult = objWorkflow.ManageWorkflows(objWorkflow, "BindWorkflowCloneDropDown");
            ddlWorkflowClone.DataSource = objResult.dsResult;
            ddlWorkflowClone.DataTextField = "Workflow_vName";
            ddlWorkflowClone.DataValueField = "Workflow_iId";
            ddlWorkflowClone.DataBind();
        }

        //To clone the selected workflow
        protected void btnOKCloneClick(object sender, EventArgs e)
        {
            try
            {
                int ProcessID = Convert.ToInt32(Session["ProcessId"].ToString());
                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objWorkflow.LoginToken = loginUser.LoginToken;
                objWorkflow.LoginOrgId = loginUser.LoginOrgId;
                objWorkflow.WorkFlowProcessId = ProcessID;
                objWorkflow.WorkflowId = Convert.ToInt32(ddlWorkflowClone.SelectedValue);
                objWorkflow.WorkFlowName = txtWorkflowRename.Text.Trim();
                objResult = objWorkflow.ManageWorkflows(objWorkflow, "CloneWorkflow");
                BindWorkflowGridview(GetWorkflow(ProcessID));
                lblMessageClone.Text = "Workflow " + ddlWorkflowClone.SelectedItem.Text + " cloned sucessfully .";
                lblMessageClone.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message.ToString();

            }

        }

        protected void ddlProcessClone_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Load Workflow related to selected process
            BindWorkflowCloneDropDown();

        }

        //DMSENH6-4795 BE

        protected void ddlOrganizationClone_SelectedIndexChanged(object sender, EventArgs e)
        {

            //Load Process related to selected organization
            BindProcessCloneDropDown();
        }

        //DMSENH6-4776 BS
        //Navigate to ManageWorkFlowPDFtemplate page
        protected void btnAddPdfcontrols_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in gridWorkflow.Rows)
            {
                RadioButton chk = row.FindControl("RowSelector") as RadioButton;

                if (chk.Checked)
                {
                    string ProcessId = Session["ProcessId"].ToString(); //ProcessId
                    ProcessId = HttpUtility.UrlEncode(Encrypt(ProcessId));
                    string WorkflowId = Encrypt(row.Cells[3].Text); //WorkflowId
                    string workflowConfirmed = string.Empty;//WorkflowConfirmed
                    string QueryString = "?ProcessId=" + ProcessId + "&WorkflowId=" + (WorkflowId) + "&WorkflowConfirmed=" + workflowConfirmed;

                    Response.Redirect("~/Workflow/WorkflowAdmin/ManageWorkFlowPDFtemplate.aspx" + QueryString);
                }
            }
        }
        //DMSENH6-4776 BE

    }
}