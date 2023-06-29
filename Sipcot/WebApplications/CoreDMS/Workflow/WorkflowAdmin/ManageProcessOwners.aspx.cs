using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lotex.EnterpriseSolutions.CoreBE;
using WorkflowBAL;
using WorkflowBLL.Classes;

/*
=============================================================================  
** File			: USP_Workflow_ManageStageStatuses
** Author		: Robin Thomas 
** Creation On	: 10 Oct 2014
** Description	: To manage stage Statuses
===============================================================================  
======================================================================================================================================  
 * Change History   
 * Date:            Author:             Issue ID            Description:  
 * -----------------------------------------------------------------------------------------------------------------------------------

 * 17 Apr 2015      Gokul               DMS5-3946       Applying rights an permissions in all pages as per user group rights!
 * 08 Apr 2015      Sabina              DMS5-4122       Button not enabled in workflow pages 
====================================================================================================================================== */


namespace Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin
{
    public partial class ManageProcessOwners : PageBase
    {
        WorkflowUserMapping objUserMapping = new WorkflowUserMapping();
        public string pageRights = string.Empty; /* DMS5-3946 A */
        protected void Page_Load(object sender, EventArgs e)
        {
            CheckAuthentication();

            //Start: ------------------SitePath Details ----------------------
            Label sitePath = (Label)Master.FindControl("SitePath");
            sitePath.Text = "WorkFlow >> Process >> Manage Process Owners";
            //End: ------------------SitePath Details ------------------------

            pageRights = GetPageRights("CONFIGURATION");/* DMS5-3946 A,DMS5-4122M added parameter configuration */
            if (!Page.IsPostBack)
            {
                try
                {
                    if (Request.QueryString["ProcessId"] != null)
                    {

                        ApplyPageRights(pageRights, this.Form.Controls); /* DMS5-3946 A */
                        // Store process id in view state for later use
                        Session["ProcessId"] = Decrypt(HttpUtility.UrlDecode(Request.QueryString["ProcessId"]));
                        Session["ProcessName"] = Decrypt(HttpUtility.UrlDecode(Request.QueryString["ProcessName"]));

                        lblCurrentStageNameHeaderValue.Text = Session["ProcessName"].ToString();


                        btnAddUser_ProcessOwner.Attributes.Add("onclick", "javascript:return validateSelectedItem('" + lstAvailableUsers_ProcessOwner.ClientID.ToString() + "','Please select a user name to assign');");
                        btnRemoveUser_ProcessOwner.Attributes.Add("onclick", "javascript:return validateSelectedItem('" + lstAssignedUsers_ProcessOwner.ClientID.ToString() + "','Please select a user name to remove');");
                    }
                    else
                    {
                        throw new Exception("Querystring empty.");
                    }

                    DisplaySelectedEntityDetails(GetSelectedEntityDetails());
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                BindUsers(GetAllUsers());

            }
        }

        private DBResult GetAllUsers()
        {
            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            objUserMapping.LoginToken = loginUser.LoginToken;
            objUserMapping.LoginOrgId = loginUser.LoginOrgId;
            objUserMapping.WfUserMappingAction = "GetAllUsers";
            objUserMapping.WfUserMappingProcessId = Convert.ToInt32( Session["ProcessId"].ToString());
            objUserMapping.WfUserMappingWorkflowId = 0;
            objUserMapping.WfUserMappingStageId = 0;

            return objUserMapping.GetAllWorkflowUserMapping(objUserMapping);
        }

        private DBResult GetSelectedEntityDetails()
        {
            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            objUserMapping.LoginToken = loginUser.LoginToken;
            objUserMapping.LoginOrgId = loginUser.LoginOrgId;
            objUserMapping.WfUserMappingAction = "GetSelectedEntityDetails";
            objUserMapping.WfUserMappingProcessId = Convert.ToInt32(Session["ProcessId"].ToString());
            objUserMapping.WfUserMappingWorkflowId = 0;
            objUserMapping.WfUserMappingStageId = 0;

            return objUserMapping.GetAllWorkflowUserMapping(objUserMapping);
        }

        protected void BindUsers(DBResult objResult)
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
        }

        protected void DisplaySelectedEntityDetails(DBResult objResult)
        {

            //Display process details if minimum 1 result sets are available
            if (objResult.dsResult != null && objResult.dsResult.Tables.Count > 0)
            {
                string strCurrentProcessName = objResult.dsResult.Tables[0].Rows[0]["WorkflowProcess_vName"].ToString();

                lblProcessOwner_CurrentProcessValue.Text = strCurrentProcessName;
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

        protected void btnAddUser_ProcessOwner_Click(object sender, EventArgs e)
        {
            CheckAuthentication();
            try
            {

                string selUserID = lstAvailableUsers_ProcessOwner.SelectedItem.Value;

                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objUserMapping.LoginToken = loginUser.LoginToken;
                objUserMapping.LoginOrgId = loginUser.LoginOrgId;
                objUserMapping.WfUserMappingAction = "SaveUser";
                objUserMapping.WfUserMappingProcessId = Convert.ToInt32(Session["ProcessId"].ToString());

                objUserMapping.WfUserMappingCategoryId = 1001; //process owner
                objUserMapping.WfUserMappingGroupId = 0;
                objUserMapping.WfUserMappingStageId = 0;
                objUserMapping.WfUserMappingWorkflowId = 0;
                objUserMapping.WfUserMappingUserId = Convert.ToInt32(selUserID);


                objUserMapping.ManageWorkflowUserMapping(objUserMapping);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            BindUsers(GetAllUsers());

            hdnCurrentPanel.Value = "0";
        }

        protected void btnRemoveUser_ProcessOwner_Click(object sender, EventArgs e)
        {
            CheckAuthentication();
            try
            {
                string selUserID = lstAssignedUsers_ProcessOwner.SelectedItem.Value;

                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objUserMapping.LoginToken = loginUser.LoginToken;
                objUserMapping.LoginOrgId = loginUser.LoginOrgId;
                objUserMapping.WfUserMappingAction = "DeleteUser";
                objUserMapping.WfUserMappingProcessId = Convert.ToInt32(Session["ProcessId"].ToString());

                objUserMapping.WfUserMappingCategoryId = 1001; //process owner
                objUserMapping.WfUserMappingGroupId = 0;
                objUserMapping.WfUserMappingStageId = 0;
                objUserMapping.WfUserMappingWorkflowId = 0;
                objUserMapping.WfUserMappingUserId = Convert.ToInt32(selUserID);


                objUserMapping.ManageWorkflowUserMapping(objUserMapping);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            BindUsers(GetAllUsers());

            hdnCurrentPanel.Value = "0";
        }

        protected void btnGoBacktoStage_Click(object sender, EventArgs e)
        {
            CheckAuthentication();

            Response.Redirect("~/Workflow/WorkflowAdmin/ManageProcess.aspx");
        }
    }
}