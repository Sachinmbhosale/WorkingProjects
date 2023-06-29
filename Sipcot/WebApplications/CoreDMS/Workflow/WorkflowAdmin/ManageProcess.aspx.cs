/* ===================================================================================================================================
 * Author     : 
 * Create date: 
 * Description:  
======================================================================================================================================  
 * Change History   
 * Date:            Author:             Issue ID            Description:  
 * -----------------------------------------------------------------------------------------------------------------------------------

 * 17 Apr 2015      Gokul               DMS5-3946       Applying rights an permissions in all pages as per user group rights!
 * 08 Apr 2015      Sabina              DMS5-4122       Button not enabled in workflow pages
====================================================================================================================================== */




using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WorkflowBLL.Classes;
using Lotex.EnterpriseSolutions.CoreBE;
using WorkflowBAL;
using System.Web.UI.HtmlControls;


namespace Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin
{
    public partial class ManageProcess : PageBase
    {
        #region GLOBAL DECLARATION

        WorkflowProcess objProcess = new WorkflowProcess();
        Workflows objWorkflow = new Workflows();

        public string pageRights = string.Empty; /* DMS5-3946 A */
        

        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            CheckAuthentication();

            //users who are super admin or customer admin only can access to the configuration page.
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            pageRights = GetPageRights("CONFIGURATION");/* DMS5-3946 A,DMS5-4122M added parameter configuration */
            if (!((loginUser.GroupName == "Customer Admin") || (loginUser.GroupName == "Super Admin")))
            {
                ApplyPageRights(pageRights, this.Form.Controls); /* DMS5-3946 A */
                gridProcess.Visible = false;
                btnGotoWorkflow.Visible = false;
                btnAddProcess.Visible = false;

                HtmlMeta meta = new HtmlMeta();
                meta.HttpEquiv = "Refresh";
                meta.Content = "3;url=WorkFlowHome.aspx";
                this.Page.Controls.Add(meta);
                lblMessageGrid.Text = "You do not have sufficient permission to access this page. You will now be redirected to Workflow Home Page in 3 seconds.";

            }


            //Start: ------------------SitePath Details ----------------------
            Label sitePath = (Label)Master.FindControl("SitePath");
            sitePath.Text = "WorkFlow >> Manage Process";
            //End: ------------------SitePath Details ------------------------

            if (!Page.IsPostBack)
            {
                try
                {
                    BindProcessGridview(GetProcess());

                    BindOrganizationDropDown();
                }
                catch (Exception ex)
                {
                    lblMessage.Text = ex.Message.ToString();

                }
            }

        }
        #region "Process"

        protected void BindOrganizationDropDown()
        {
            try
            {
                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objWorkflow.LoginToken = loginUser.LoginToken;
                objWorkflow.LoginOrgId = loginUser.LoginOrgId;
                objResult = objWorkflow.ManageWorkflows(objWorkflow, "BindOrganizationDropDown");
                ddlOrganization.DataSource = objResult.dsResult;
                ddlOrganization.DataTextField = "ORGS_vName";
                ddlOrganization.DataValueField = "ORGS_iId";
                ddlOrganization.DataBind();
            }
            catch (Exception ex)
            {
                lblMessageGrid.Text = ex.Message.ToString();
            }

        }

        private DBResult GetProcess()
        {
            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            objProcess.LoginToken = loginUser.LoginToken;
            objProcess.LoginOrgId = loginUser.LoginOrgId;
            objProcess.WfProcessOrgId = loginUser.LoginOrgId;
            return objProcess.ManageProcess(objProcess, "GetAllProcess");
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            CheckAuthentication();
            string action = "AddProcess";
            DBResult objResult = new DBResult();

            try
            {
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objProcess.LoginToken = loginUser.LoginToken;
                objProcess.LoginOrgId = loginUser.LoginOrgId;

                objProcess.WfProcessName = txtProcessName.Text.Trim();
                objProcess.WfProcessDescription = txtProcessDescription.Text.Trim();
                objProcess.WfProcessIsActive = chkActive.Checked;
                objProcess.WfProcessOrgId = Convert.ToInt32(ddlOrganization.SelectedValue.ToString());
                string strSaveStatus = hdnSaveStatus.Value;

                if (strSaveStatus == "Save Changes")
                {
                    objProcess.WfProcessId = Convert.ToInt32(hiddenProcessId.Value);
                    objProcess.WfProcessIsDeleted = chkActive.Checked ? false : true;
                    action = "EditProcess";
                }

                objResult = objProcess.ManageProcess(objProcess, action);
                handleDBResult(objResult);

                //only if success
                if (objResult.ErrorState == 0)
                {
                    clearControls();
                    BindProcessGridview(GetProcess());
                    hdnErrorStatus.Value = "";
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
                lblMessageGrid.Text = ex.Message.ToString();
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
            txtProcessName.Text = string.Empty;
            txtProcessDescription.Text = string.Empty;
            chkActive.Checked = false;
        }

        protected void BindProcessGridview(DBResult objResult)
        {
            try
            {
                if (objResult.dsResult != null && objResult.dsResult.Tables.Count > 0)
                {
                    gridProcess.DataSource = objResult.dsResult.Tables[0];
                    gridProcess.DataBind();

                    //Multilanguage implementation for Grid headers
                    UserBase loginUser = (UserBase)Session["LoggedUser"];
                    if (loginUser.LanguageCode != "en-US")
                    {
                        WorkFlowCommonFns.SetGridHeaderForMultiLanguage(ref gridProcess, "PROCESS", loginUser.LoginToken, loginUser.LoginOrgId, loginUser.LanguageID, loginUser.UserId);
                    }

                    btnGotoWorkflow.Enabled = true;
                }
                else
                {
                    btnGotoWorkflow.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                lblMessageGrid.Text = ex.Message.ToString();
            }
        }


        #endregion

        protected void btnAddProcess_Click(object sender, EventArgs e)
        {
            try
            {
                CheckAuthentication();
                BindProcessGridview(GetProcess());
            }
            catch (Exception ex)
            {
                lblMessageGrid.Text = ex.Message.ToString();
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {

        }


        protected void btnGotoWorkflow_Click(object sender, EventArgs e)
        {
            CheckAuthentication();
            foreach (GridViewRow row in gridProcess.Rows)
            {
                RadioButton chk = row.FindControl("RowSelector") as RadioButton;

                if (chk.Checked)
                {
                    string ProcessId = HttpUtility.UrlEncode(Encrypt(row.Cells[3].Text)); //ProcessId
                    string QueryString = "?ProcessId=" + Server.UrlEncode(ProcessId);
                    Response.Redirect("~/Workflow/WorkflowAdmin/ManageWorkflow.aspx" + QueryString);
                }
            }
        }



        protected void gridProcess_PageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            try
            {
                CheckAuthentication();
                gridProcess.PageIndex = e.NewPageIndex;
                BindProcessGridview(GetProcess());
            }
            catch (Exception ex)
            {
                lblMessageGrid.Text = ex.Message.ToString();
            }
        }
        protected void gridProcess_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //// Hide unwanted rows
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
                {
                    e.Row.Cells[9].Style.Add("display", "none"); //orgid
                    e.Row.Cells[8].Style.Add("display", "none"); //activestatus
                    e.Row.Cells[1].Style.Add("display", "none"); //total count
                    e.Row.Cells[3].Style.Add("display", "none"); //processid
                    e.Row.Cells[0].Style.Add("width", "25px");   //edit
                    e.Row.Cells[4].Style.Add("width", "50px"); //processname
                    e.Row.Cells[6].Style.Add("width", "50px"); //org name
                    e.Row.Cells[2].Style.Add("width", "20px"); //slNo
                    e.Row.Cells[5].Style.Add("width", "150px");  //description
                    e.Row.Cells[7].Style.Add("width", "20px"); //active
                }

                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center; //edit
                    e.Row.Cells[7].HorizontalAlign = HorizontalAlign.Center; //active

                    string ProcessId = HttpUtility.UrlEncode(Encrypt(e.Row.Cells[3].Text)); //ProcessId
                    string ProcessName = HttpUtility.UrlEncode(Encrypt(e.Row.Cells[4].Text)); //ProcessName
                    string QueryString = "?ProcessId=" + Server.UrlEncode(ProcessId);
                    string URL = "ManageWorkflow.aspx" + QueryString;

                    e.Row.Attributes.Add("ondblclick", "window.location.href ='" + URL + "'");

                    LinkButton lnkManageProcessOwners = (LinkButton)e.Row.FindControl("lnkManageProcessOwners");
                    lnkManageProcessOwners.PostBackUrl = "~/Workflow/WorkflowAdmin/ManageProcessOwners.aspx" + QueryString + "&ProcessName=" + ProcessName;
                }
            }
            catch (Exception ex)
            {
                lblMessageGrid.Text = ex.Message.ToString();
            }
        }


        protected void btnGotoWorkflowStudio_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Workflow/WorkflowAdmin/ManageStudio.aspx");
        }
    }
}