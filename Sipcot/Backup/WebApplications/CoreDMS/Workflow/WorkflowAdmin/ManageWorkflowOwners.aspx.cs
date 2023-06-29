using System;
using System.Collections.Generic;
using System.Linq;
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
===============================================================================  
** Change History   
** Date:          Author:            Issue ID                         Description:  
** ----------   -------------       ----------                  ----------------------------

 *17 Apr 2015   Gokul        DMS5-3946       Applying rights an permissions in all pages as per user group rights!   
 *08 Apr 2015   Sabina       DMS5-4122       Button not enabled in workflow pages
=============================================================================== */
namespace Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin
{
    public partial class ManageWorkflowOwners : PageBase
    {
        WorkflowUserMapping objUserMapping = new WorkflowUserMapping();
        public string pageRights = string.Empty; /* DMS5-3946 A */
       
        protected void Page_Load(object sender, EventArgs e)
        {
            CheckAuthentication();

            pageRights = GetPageRights("CONFIGURATION");/* DMS5-3946 A,DMS5-4122M added parameter configuration */
            //Start: ------------------SitePath Details ----------------------
            Label sitePath = (Label)Master.FindControl("SitePath");
            sitePath.Text = "WorkFlow >> Process >> Workflow >> Manage Workflow Owners";
            //End: ------------------SitePath Details ------------------------

            if (!Page.IsPostBack)
            {
                try
                {

                    ApplyPageRights(pageRights, this.Form.Controls); /* DMS5-3946 A */
                    if (Request.QueryString["ProcessId"] != null)
                    {
                        // Store process id in view state for later use
                        Session["ProcessId"] = Decrypt(HttpUtility.UrlDecode(Request.QueryString["ProcessId"]));
                        Session["WorkflowId"] = Decrypt(HttpUtility.UrlDecode(Request.QueryString["WorkflowId"]));
                        Session["WorkflowName"] = Decrypt(HttpUtility.UrlDecode(Request.QueryString["WorkflowName"]));

                        lblCurrentStageNameHeaderValue.Text = Session["WorkflowName"].ToString();

                        btnAddUser_WorkflowOwner.Attributes.Add("onclick", "javascript:return validateSelectedItem('" + lstAvailableUsers_WorkflowOwner.ClientID.ToString() + "','Please select a user name to assign');");
                        btnRemoveUser_WorkflowOwner.Attributes.Add("onclick", "javascript:return validateSelectedItem('" + lstAssignedUsers_WorkflowOwner.ClientID.ToString() + "','Please select a user name to remove');");
                    }
                    else
                    {
                        throw new Exception("Querystring empty.");
                    }

                    DisplaySelectedEntityDetails(GetSelectedEntityDetails());
                }
                catch
                {
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
            objUserMapping.WfUserMappingWorkflowId = Convert.ToInt32(Session["WorkflowId"].ToString());
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
            objUserMapping.WfUserMappingWorkflowId = Convert.ToInt32(Session["WorkflowId"].ToString());
            objUserMapping.WfUserMappingStageId = 0;

            return objUserMapping.GetAllWorkflowUserMapping(objUserMapping);
        }

        protected void BindUsers(DBResult objResult)
        {
            //revert the if conditions at the end

            //fill workflow owner details if minimum 4 result sets are available
            if (objResult.dsResult != null && objResult.dsResult.Tables.Count > 3)
            {
                lstAvailableUsers_WorkflowOwner.DataTextField = "USERFULLNAME";
                lstAvailableUsers_WorkflowOwner.DataValueField = "USERS_iId";
                lstAvailableUsers_WorkflowOwner.DataSource = objResult.dsResult.Tables[3];
                lstAvailableUsers_WorkflowOwner.DataBind();

                lstAssignedUsers_WorkflowOwner.DataTextField = "USERFULLNAME";
                lstAssignedUsers_WorkflowOwner.DataValueField = "USERS_iId";
                lstAssignedUsers_WorkflowOwner.DataSource = objResult.dsResult.Tables[2];
                lstAssignedUsers_WorkflowOwner.DataBind();
            }

       }

        protected void DisplaySelectedEntityDetails(DBResult objResult)
        {

            //Display process details if minimum 1 result sets are available
            if (objResult.dsResult != null && objResult.dsResult.Tables.Count > 0)
            {
                string strCurrentProcessName = objResult.dsResult.Tables[0].Rows[0]["WorkflowProcess_vName"].ToString();

                lblWorkFlowOwner_CurrentProcessValue.Text = strCurrentProcessName;
            }

            //fill workflow  details if minimum 2 result sets are available
            if (objResult.dsResult != null && objResult.dsResult.Tables.Count > 1)
            {
                string strCurrentCurrentWorkFlowName = objResult.dsResult.Tables[1].Rows[0]["Workflow_vName"].ToString();

                lblWorkFlowOwner_CurrentWorkFlowValue.Text = strCurrentCurrentWorkFlowName;
            }

            //fill stage  details if minimum 3 result sets are available
            if (objResult.dsResult != null && objResult.dsResult.Tables.Count > 2)
            {
                string strCurrentStageName = objResult.dsResult.Tables[2].Rows[0]["WorkflowStage_vDisplayName"].ToString();
            }

        }

        protected void btnAddUser_WorkflowOwner_Click(object sender, EventArgs e)
        {
            CheckAuthentication();
            try
            {
                string selUserID = lstAvailableUsers_WorkflowOwner.SelectedItem.Value;

                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objUserMapping.LoginToken = loginUser.LoginToken;
                objUserMapping.LoginOrgId = loginUser.LoginOrgId;
                objUserMapping.WfUserMappingAction = "SaveUser";
                objUserMapping.WfUserMappingProcessId = Convert.ToInt32(Session["ProcessId"].ToString());
                objUserMapping.WfUserMappingWorkflowId = Convert.ToInt32(Session["WorkflowId"].ToString());

                objUserMapping.WfUserMappingCategoryId = 1002; //Workflow owner
                objUserMapping.WfUserMappingGroupId = 0;
                objUserMapping.WfUserMappingStageId = 0;
                objUserMapping.WfUserMappingUserId = Convert.ToInt32(selUserID);


                objUserMapping.ManageWorkflowUserMapping(objUserMapping);
            }
            catch 
            {
            }
            BindUsers(GetAllUsers());

        }

        protected void btnRemoveUser_WorkflowOwner_Click(object sender, EventArgs e)
        {
            CheckAuthentication();
            try
            {
                string selUserID = lstAssignedUsers_WorkflowOwner.SelectedItem.Value;

                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objUserMapping.LoginToken = loginUser.LoginToken;
                objUserMapping.LoginOrgId = loginUser.LoginOrgId;
                objUserMapping.WfUserMappingAction = "DeleteUser";
                objUserMapping.WfUserMappingProcessId = Convert.ToInt32(Session["ProcessId"].ToString());
                objUserMapping.WfUserMappingWorkflowId = Convert.ToInt32(Session["WorkflowId"].ToString());

                objUserMapping.WfUserMappingCategoryId = 1002; //workflow owner
                objUserMapping.WfUserMappingGroupId = 0;
                objUserMapping.WfUserMappingStageId = 0;
                objUserMapping.WfUserMappingUserId = Convert.ToInt32(selUserID);


                objUserMapping.ManageWorkflowUserMapping(objUserMapping);
            }
            catch
            {
            }
            BindUsers(GetAllUsers());

        }

        protected void btnGoBacktoStage_Click(object sender, EventArgs e)
        {
            CheckAuthentication();

            string ProcessId = Session["ProcessId"].ToString(); //ProcessId
            ProcessId = HttpUtility.UrlEncode(Encrypt(ProcessId));
            string QueryString = "?ProcessId=" + ProcessId;

            Response.Redirect("~/Workflow/WorkflowAdmin/ManageWorkflow.aspx" + QueryString);
        }
    }
}