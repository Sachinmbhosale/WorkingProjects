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
===============================================================================  
** Change History   
** Date:          Author:            Issue ID                         Description:  
** ----------   -------------       ----------                  ----------------------------

*17 Apr 2015     Gokul               DMS5-3946       Applying rights an permissions in all pages as per user group rights!   
*08 Apr 2015     Sabina              DMS5-4122       Button not enabled in workflow pages
*10/08/2015      sharath             DMSENH6-4732    Deletion options within Workflow
=============================================================================== */
namespace Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin
{
    public partial class ManageStageUsers : PageBase
    {
        WorkflowUserMapping objUserMapping = new WorkflowUserMapping();
        public string pageRights = string.Empty; /* DMS5-3946 A */


        protected void Page_Load(object sender, EventArgs e)
        {
            CheckAuthentication();
            pageRights = GetPageRights("CONFIGURATION");/* DMS5-3946 A,DMS5-4122M added parameter configuration */
            //Start: ------------------SitePath Details ----------------------
            Label sitePath = (Label)Master.FindControl("SitePath");
            sitePath.Text = "WorkFlow >> Process >> Workflow >> Stage >> Manage Users";
            //End: ------------------SitePath Details ------------------------


            // DMSENH6-4732 BS
            //Disable buttons if the workflow is confirmed
            if ((Session["workflowConfirmed"]) != null && (Session["workflowConfirmed"]).Equals("Yes"))
            {
                DisableControls();
            }
            // DMSENH6-4732 BE

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
                        Session["StageId"] = Decrypt(HttpUtility.UrlDecode(Request.QueryString["StageId"]));
                        Session["StageName"] = Decrypt(HttpUtility.UrlDecode(Request.QueryString["StageName"]));

                        lblCurrentStageNameHeaderValue.Text = Session["StageName"].ToString();


                        //btnAddUser_ProcessOwner.Attributes.Add("onclick", "javascript:return validateSelectedItem('" + lstAvailableUsers_ProcessOwner.ClientID.ToString() + "','Please select a user name to assign');");
                        //btnRemoveUser_ProcessOwner.Attributes.Add("onclick", "javascript:return validateSelectedItem('" + lstAssignedUsers_ProcessOwner.ClientID.ToString() + "','Please select a user name to remove');");
                        //btnAddUser_WorkflowOwner.Attributes.Add("onclick", "javascript:return validateSelectedItem('" + lstAvailableUsers_WorkflowOwner.ClientID.ToString() + "','Please select a user name to assign');");
                        //btnRemoveUser_WorkflowOwner.Attributes.Add("onclick", "javascript:return validateSelectedItem('" + lstAssignedUsers_WorkflowOwner.ClientID.ToString() + "','Please select a user name to remove');");
                        btnAddUser_StageOwner.Attributes.Add("onclick", "javascript:return validateSelectedItem('" + lstAvailableUsers_StageOwner.ClientID.ToString() + "','Please select a user name to assign');");
                        btnRemoveUser_StageOwner.Attributes.Add("onclick", "javascript:return validateSelectedItem('" + lstAssignedUsers_StageOwner.ClientID.ToString() + "','Please select a user name to remove');");
                        btnAddUser_StageUser.Attributes.Add("onclick", "javascript:return validateSelectedItem('" + lstAvailableUsers_StageUser.ClientID.ToString() + "','Please select a user name to assign');");
                        btnRemoveUser_StageUser.Attributes.Add("onclick", "javascript:return validateSelectedItem('" + lstAssignedUsers_StageUser.ClientID.ToString() + "','Please select a user name to remove');");
                        btnAddUser_StageUserGroup.Attributes.Add("onclick", "javascript:return validateSelectedItem('" + lstAvailableUsers_StageUserGroup.ClientID.ToString() + "','Please select a user group name to assign');");
                        btnRemoveUser_StageUserGroup.Attributes.Add("onclick", "javascript:return validateSelectedItem('" + lstAssignedUsers_StageUserGroup.ClientID.ToString() + "','Please select a user group name to remove');");
                        btnAddUser_NotoficationUser.Attributes.Add("onclick", "javascript:return validateSelectedItem('" + lstAvailableUsers_NotoficationUser.ClientID.ToString() + "','Please select a user name to assign');");
                        btnRemoveUser_NotoficationUser.Attributes.Add("onclick", "javascript:return validateSelectedItem('" + lstAssignedUsers_NotoficationUser.ClientID.ToString() + "','Please select a user name to remove');");
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
            objUserMapping.WfUserMappingProcessId = Convert.ToInt32(Session["ProcessId"].ToString());
            objUserMapping.WfUserMappingWorkflowId = Convert.ToInt32(Session["WorkflowId"].ToString());
            objUserMapping.WfUserMappingStageId = Convert.ToInt32(Session["StageId"].ToString());

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
            objUserMapping.WfUserMappingStageId = Convert.ToInt32(Session["StageId"].ToString());

            return objUserMapping.GetAllWorkflowUserMapping(objUserMapping);
        }

        protected void BindUsers(DBResult objResult)
        {
            //revert the if conditions at the end
            //fill process owner details if minimum 2 result sets are available
            //if (objResult.dsResult != null && objResult.dsResult.Tables.Count > 1)
            //{
            //    lstAvailableUsers_ProcessOwner.DataTextField = "USERFULLNAME";
            //    lstAvailableUsers_ProcessOwner.DataValueField = "USERS_iId";
            //    lstAvailableUsers_ProcessOwner.DataSource = objResult.dsResult.Tables[1];
            //    lstAvailableUsers_ProcessOwner.DataBind();

            //    lstAssignedUsers_ProcessOwner.DataTextField = "USERFULLNAME";
            //    lstAssignedUsers_ProcessOwner.DataValueField = "USERS_iId";
            //    lstAssignedUsers_ProcessOwner.DataSource = objResult.dsResult.Tables[0];
            //    lstAssignedUsers_ProcessOwner.DataBind();

            //}

            //fill workflow owner details if minimum 4 result sets are available
            //if (objResult.dsResult != null && objResult.dsResult.Tables.Count > 3)
            //{
            //    lstAvailableUsers_WorkflowOwner.DataTextField = "USERFULLNAME";
            //    lstAvailableUsers_WorkflowOwner.DataValueField = "USERS_iId";
            //    lstAvailableUsers_WorkflowOwner.DataSource = objResult.dsResult.Tables[3];
            //    lstAvailableUsers_WorkflowOwner.DataBind();

            //    lstAssignedUsers_WorkflowOwner.DataTextField = "USERFULLNAME";
            //    lstAssignedUsers_WorkflowOwner.DataValueField = "USERS_iId";
            //    lstAssignedUsers_WorkflowOwner.DataSource = objResult.dsResult.Tables[2];
            //    lstAssignedUsers_WorkflowOwner.DataBind();
            //}

            //fill stage owner details if minimum 6 result sets are available
            if (objResult.dsResult != null && objResult.dsResult.Tables.Count > 5)
            {
                lstAvailableUsers_StageOwner.DataTextField = "USERFULLNAME";
                lstAvailableUsers_StageOwner.DataValueField = "USERS_iId";
                lstAvailableUsers_StageOwner.DataSource = objResult.dsResult.Tables[5];
                lstAvailableUsers_StageOwner.DataBind();

                lstAssignedUsers_StageOwner.DataTextField = "USERFULLNAME";
                lstAssignedUsers_StageOwner.DataValueField = "USERS_iId";
                lstAssignedUsers_StageOwner.DataSource = objResult.dsResult.Tables[4];
                lstAssignedUsers_StageOwner.DataBind();
            }

            //fill stage user details if minimum 8 result sets are available
            if (objResult.dsResult != null && objResult.dsResult.Tables.Count > 7)
            {
                lstAvailableUsers_StageUser.DataTextField = "USERFULLNAME";
                lstAvailableUsers_StageUser.DataValueField = "USERS_iId";
                lstAvailableUsers_StageUser.DataSource = objResult.dsResult.Tables[7];
                lstAvailableUsers_StageUser.DataBind();

                lstAssignedUsers_StageUser.DataTextField = "USERFULLNAME";
                lstAssignedUsers_StageUser.DataValueField = "USERS_iId";
                lstAssignedUsers_StageUser.DataSource = objResult.dsResult.Tables[6];
                lstAssignedUsers_StageUser.DataBind();
            }

            //fill stage user details if minimum 10 result sets are available
            if (objResult.dsResult != null && objResult.dsResult.Tables.Count > 9)
            {
                lstAvailableUsers_StageUserGroup.DataTextField = "GROUPS_vName";
                lstAvailableUsers_StageUserGroup.DataValueField = "GROUPS_iId";
                lstAvailableUsers_StageUserGroup.DataSource = objResult.dsResult.Tables[9];
                lstAvailableUsers_StageUserGroup.DataBind();

                lstAssignedUsers_StageUserGroup.DataTextField = "GROUPS_vName";
                lstAssignedUsers_StageUserGroup.DataValueField = "GROUPS_iId";
                lstAssignedUsers_StageUserGroup.DataSource = objResult.dsResult.Tables[8];
                lstAssignedUsers_StageUserGroup.DataBind();
            }

            //fill stage notification user details if minimum 12 result sets are available
            if (objResult.dsResult != null && objResult.dsResult.Tables.Count > 11)
            {
                lstAvailableUsers_NotoficationUser.DataTextField = "USERFULLNAME";
                lstAvailableUsers_NotoficationUser.DataValueField = "USERS_iId";
                lstAvailableUsers_NotoficationUser.DataSource = objResult.dsResult.Tables[11];
                lstAvailableUsers_NotoficationUser.DataBind();

                lstAssignedUsers_NotoficationUser.DataTextField = "USERFULLNAME";
                lstAssignedUsers_NotoficationUser.DataValueField = "USERS_iId";
                lstAssignedUsers_NotoficationUser.DataSource = objResult.dsResult.Tables[10];
                lstAssignedUsers_NotoficationUser.DataBind();
            }
        }

        protected void DisplaySelectedEntityDetails(DBResult objResult)
        {

            //Display process details if minimum 1 result sets are available
            if (objResult.dsResult != null && objResult.dsResult.Tables.Count > 0)
            {
                string strCurrentProcessName = objResult.dsResult.Tables[0].Rows[0]["WorkflowProcess_vName"].ToString();

                //lblProcessOwner_CurrentProcessValue.Text = strCurrentProcessName;
                //lblWorkFlowOwner_CurrentProcessValue.Text = strCurrentProcessName;
                lblStageOwner_CurrentProcessValue.Text = strCurrentProcessName;
                lblStageUser_CurrentProcessValue.Text = strCurrentProcessName;
                lblStageUserGroup_CurrentProcessValue.Text = strCurrentProcessName;
                lblNotificationUser_CurrentProcessValue.Text = strCurrentProcessName;

            }

            //fill workflow  details if minimum 2 result sets are available
            if (objResult.dsResult != null && objResult.dsResult.Tables.Count > 1)
            {
                string strCurrentCurrentWorkFlowName = objResult.dsResult.Tables[1].Rows[0]["Workflow_vName"].ToString();

                //lblWorkFlowOwner_CurrentWorkFlowValue.Text = strCurrentCurrentWorkFlowName;
                lblStageOwner_CurrentWorkFlowValue.Text = strCurrentCurrentWorkFlowName;
                lblStageUser_CurrentWorkFlowValue.Text = strCurrentCurrentWorkFlowName;
                lblStageUserGroup_CurrentWorkFlowValue.Text = strCurrentCurrentWorkFlowName;
                lblNotificationUser_CurrentWorkFlowValue.Text = strCurrentCurrentWorkFlowName;
            }

            //fill stage  details if minimum 3 result sets are available
            if (objResult.dsResult != null && objResult.dsResult.Tables.Count > 2)
            {
                string strCurrentStageName = objResult.dsResult.Tables[2].Rows[0]["WorkflowStage_vDisplayName"].ToString();

                lblStageOwner_CurrentStageValue.Text = strCurrentStageName;
                lblStageUser_CurrentStageValue.Text = strCurrentStageName;
                lblStageUserGroup_CurrentStageValue.Text = strCurrentStageName;
                lblNotificationUser_CurrentStageValue.Text = strCurrentStageName;

            }

        }

        //protected void btnAddUser_ProcessOwner_Click(object sender, EventArgs e)
        //{
        //    CheckAuthentication();
        //    try
        //    {

        //        string selUserID = lstAvailableUsers_ProcessOwner.SelectedItem.Value;

        //        DBResult objResult = new DBResult();
        //        UserBase loginUser = (UserBase)Session["LoggedUser"];
        //        objUserMapping.LoginToken = loginUser.LoginToken;
        //        objUserMapping.LoginOrgId = loginUser.LoginOrgId;
        //        objUserMapping.WfUserMappingAction = "SaveUser";
        //        objUserMapping.WfUserMappingProcessId = Convert.ToInt32(Session["ProcessId"].ToString());

        //        objUserMapping.WfUserMappingCategoryId = 1001; //process owner
        //        objUserMapping.WfUserMappingGroupId = 0;
        //        objUserMapping.WfUserMappingStageId = 0;
        //        objUserMapping.WfUserMappingWorkflowId = 0;
        //        objUserMapping.WfUserMappingUserId = Convert.ToInt32(selUserID);


        //        objUserMapping.ManageWorkflowUserMapping(objUserMapping);
        //    }
        //    catch (Exception ex)
        //    {
        //    }

        //    BindUsers(GetAllUsers());

        //    hdnCurrentPanel.Value = "0";
        //}

        //protected void btnRemoveUser_ProcessOwner_Click(object sender, EventArgs e)
        //{
        //    CheckAuthentication();
        //    try
        //    {
        //        string selUserID = lstAssignedUsers_ProcessOwner.SelectedItem.Value;

        //        DBResult objResult = new DBResult();
        //        UserBase loginUser = (UserBase)Session["LoggedUser"];
        //        objUserMapping.LoginToken = loginUser.LoginToken;
        //        objUserMapping.LoginOrgId = loginUser.LoginOrgId;
        //        objUserMapping.WfUserMappingAction = "DeleteUser";
        //        objUserMapping.WfUserMappingProcessId = Convert.ToInt32(Session["ProcessId"].ToString());

        //        objUserMapping.WfUserMappingCategoryId = 1001; //process owner
        //        objUserMapping.WfUserMappingGroupId = 0;
        //        objUserMapping.WfUserMappingStageId = 0;
        //        objUserMapping.WfUserMappingWorkflowId = 0;
        //        objUserMapping.WfUserMappingUserId = Convert.ToInt32(selUserID);


        //        objUserMapping.ManageWorkflowUserMapping(objUserMapping);
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    BindUsers(GetAllUsers());

        //    hdnCurrentPanel.Value = "0";
        //}

        //protected void btnAddUser_WorkflowOwner_Click(object sender, EventArgs e)
        //{
        //    CheckAuthentication();
        //    try
        //    {
        //        string selUserID = lstAvailableUsers_WorkflowOwner.SelectedItem.Value;

        //        DBResult objResult = new DBResult();
        //        UserBase loginUser = (UserBase)Session["LoggedUser"];
        //        objUserMapping.LoginToken = loginUser.LoginToken;
        //        objUserMapping.LoginOrgId = loginUser.LoginOrgId;
        //        objUserMapping.WfUserMappingAction = "SaveUser";
        //        objUserMapping.WfUserMappingProcessId = Convert.ToInt32(Session["ProcessId"].ToString());
        //        objUserMapping.WfUserMappingWorkflowId = Convert.ToInt32(Session["WorkflowId"].ToString());

        //        objUserMapping.WfUserMappingCategoryId = 1002; //Workflow owner
        //        objUserMapping.WfUserMappingGroupId = 0;
        //        objUserMapping.WfUserMappingStageId = 0;
        //        objUserMapping.WfUserMappingUserId = Convert.ToInt32(selUserID);


        //        objUserMapping.ManageWorkflowUserMapping(objUserMapping);
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    BindUsers(GetAllUsers());

        //    hdnCurrentPanel.Value = "1";
        //}

        //protected void btnRemoveUser_WorkflowOwner_Click(object sender, EventArgs e)
        //{
        //    CheckAuthentication();
        //    try
        //    {
        //        string selUserID = lstAssignedUsers_WorkflowOwner.SelectedItem.Value;

        //        DBResult objResult = new DBResult();
        //        UserBase loginUser = (UserBase)Session["LoggedUser"];
        //        objUserMapping.LoginToken = loginUser.LoginToken;
        //        objUserMapping.LoginOrgId = loginUser.LoginOrgId;
        //        objUserMapping.WfUserMappingAction = "DeleteUser";
        //        objUserMapping.WfUserMappingProcessId = Convert.ToInt32(Session["ProcessId"].ToString());
        //        objUserMapping.WfUserMappingWorkflowId = Convert.ToInt32(Session["WorkflowId"].ToString());

        //        objUserMapping.WfUserMappingCategoryId = 1002; //workflow owner
        //        objUserMapping.WfUserMappingGroupId = 0;
        //        objUserMapping.WfUserMappingStageId = 0;
        //        objUserMapping.WfUserMappingUserId = Convert.ToInt32(selUserID);


        //        objUserMapping.ManageWorkflowUserMapping(objUserMapping);
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    BindUsers(GetAllUsers());

        //    hdnCurrentPanel.Value = "1";
        //}

        protected void btnAddUser_StageOwner_Click(object sender, EventArgs e)
        {
            CheckAuthentication();
            try
            {
                string selUserID = lstAvailableUsers_StageOwner.SelectedItem.Value;

                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objUserMapping.LoginToken = loginUser.LoginToken;
                objUserMapping.LoginOrgId = loginUser.LoginOrgId;
                objUserMapping.WfUserMappingAction = "SaveUser";
                objUserMapping.WfUserMappingProcessId = Convert.ToInt32(Session["ProcessId"].ToString());
                objUserMapping.WfUserMappingWorkflowId = Convert.ToInt32(Session["WorkflowId"].ToString());
                objUserMapping.WfUserMappingStageId = Convert.ToInt32(Session["StageId"].ToString());

                objUserMapping.WfUserMappingCategoryId = 1003; //Stage owner
                objUserMapping.WfUserMappingGroupId = 0;
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

        protected void btnRemoveUser_StageOwner_Click(object sender, EventArgs e)
        {
            CheckAuthentication();
            try
            {
                string selUserID = lstAssignedUsers_StageOwner.SelectedItem.Value;

                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objUserMapping.LoginToken = loginUser.LoginToken;
                objUserMapping.LoginOrgId = loginUser.LoginOrgId;
                objUserMapping.WfUserMappingAction = "DeleteUser";
                objUserMapping.WfUserMappingProcessId = Convert.ToInt32(Session["ProcessId"].ToString());
                objUserMapping.WfUserMappingWorkflowId = Convert.ToInt32(Session["WorkflowId"].ToString());
                objUserMapping.WfUserMappingStageId = Convert.ToInt32(Session["StageId"].ToString());

                objUserMapping.WfUserMappingCategoryId = 1003; //Stage owner
                objUserMapping.WfUserMappingGroupId = 0;
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

        protected void btnAddUser_StageUser_Click(object sender, EventArgs e)
        {
            CheckAuthentication();
            try
            {
                string selUserID = lstAvailableUsers_StageUser.SelectedItem.Value;

                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objUserMapping.LoginToken = loginUser.LoginToken;
                objUserMapping.LoginOrgId = loginUser.LoginOrgId;
                objUserMapping.WfUserMappingAction = "SaveUser";
                objUserMapping.WfUserMappingProcessId = Convert.ToInt32(Session["ProcessId"].ToString());
                objUserMapping.WfUserMappingWorkflowId = Convert.ToInt32(Session["WorkflowId"].ToString());
                objUserMapping.WfUserMappingStageId = Convert.ToInt32(Session["StageId"].ToString());

                objUserMapping.WfUserMappingCategoryId = 1004; //Stage user
                objUserMapping.WfUserMappingGroupId = 0;
                objUserMapping.WfUserMappingUserId = Convert.ToInt32(selUserID);


                objUserMapping.ManageWorkflowUserMapping(objUserMapping);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            BindUsers(GetAllUsers());

            hdnCurrentPanel.Value = "1";
            hdnCurrentSubPanel.Value = "0";
        }

        protected void btnRemoveUser_StageUser_Click(object sender, EventArgs e)
        {
            CheckAuthentication();
            try
            {
                string selUserID = lstAssignedUsers_StageUser.SelectedItem.Value;

                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objUserMapping.LoginToken = loginUser.LoginToken;
                objUserMapping.LoginOrgId = loginUser.LoginOrgId;
                objUserMapping.WfUserMappingAction = "DeleteUser";
                objUserMapping.WfUserMappingProcessId = Convert.ToInt32(Session["ProcessId"].ToString());
                objUserMapping.WfUserMappingWorkflowId = Convert.ToInt32(Session["WorkflowId"].ToString());
                objUserMapping.WfUserMappingStageId = Convert.ToInt32(Session["StageId"].ToString());

                objUserMapping.WfUserMappingCategoryId = 1004; //Stage user
                objUserMapping.WfUserMappingGroupId = 0;
                objUserMapping.WfUserMappingUserId = Convert.ToInt32(selUserID);


                objUserMapping.ManageWorkflowUserMapping(objUserMapping);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            BindUsers(GetAllUsers());

            hdnCurrentPanel.Value = "1";
            hdnCurrentSubPanel.Value = "0";
        }

        protected void btnAddUser_StageUserGroup_Click(object sender, EventArgs e)
        {
            CheckAuthentication();
            try
            {
                string selUserID = lstAvailableUsers_StageUserGroup.SelectedItem.Value;

                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objUserMapping.LoginToken = loginUser.LoginToken;
                objUserMapping.LoginOrgId = loginUser.LoginOrgId;
                objUserMapping.WfUserMappingAction = "SaveUser";
                objUserMapping.WfUserMappingProcessId = Convert.ToInt32(Session["ProcessId"].ToString());
                objUserMapping.WfUserMappingWorkflowId = Convert.ToInt32(Session["WorkflowId"].ToString());
                objUserMapping.WfUserMappingStageId = Convert.ToInt32(Session["StageId"].ToString());

                objUserMapping.WfUserMappingCategoryId = 1005; //Stage usergroup
                objUserMapping.WfUserMappingGroupId = Convert.ToInt32(selUserID);
                objUserMapping.WfUserMappingUserId = 0;


                objUserMapping.ManageWorkflowUserMapping(objUserMapping);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            BindUsers(GetAllUsers());

            hdnCurrentPanel.Value = "1";
            hdnCurrentSubPanel.Value = "1";
        }

        protected void btnRemoveUser_StageUserGroup_Click(object sender, EventArgs e)
        {
            CheckAuthentication();
            try
            {
                string selUserID = lstAssignedUsers_StageUserGroup.SelectedItem.Value;

                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objUserMapping.LoginToken = loginUser.LoginToken;
                objUserMapping.LoginOrgId = loginUser.LoginOrgId;
                objUserMapping.WfUserMappingAction = "DeleteUser";
                objUserMapping.WfUserMappingProcessId = Convert.ToInt32(Session["ProcessId"].ToString());
                objUserMapping.WfUserMappingWorkflowId = Convert.ToInt32(Session["WorkflowId"].ToString());
                objUserMapping.WfUserMappingStageId = Convert.ToInt32(Session["StageId"].ToString());

                objUserMapping.WfUserMappingCategoryId = 1005; //Stage usergroup
                objUserMapping.WfUserMappingGroupId = Convert.ToInt32(selUserID); ;
                objUserMapping.WfUserMappingUserId = 0;


                objUserMapping.ManageWorkflowUserMapping(objUserMapping);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            BindUsers(GetAllUsers());

            hdnCurrentPanel.Value = "1";
            hdnCurrentSubPanel.Value = "1";
        }

        protected void btnAddUser_NotoficationUser_Click(object sender, EventArgs e)
        {
            CheckAuthentication();
            try
            {
                string selUserID = lstAvailableUsers_NotoficationUser.SelectedItem.Value;

                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objUserMapping.LoginToken = loginUser.LoginToken;
                objUserMapping.LoginOrgId = loginUser.LoginOrgId;
                objUserMapping.WfUserMappingAction = "SaveUser";
                objUserMapping.WfUserMappingProcessId = Convert.ToInt32(Session["ProcessId"].ToString());
                objUserMapping.WfUserMappingWorkflowId = Convert.ToInt32(Session["WorkflowId"].ToString());
                objUserMapping.WfUserMappingStageId = Convert.ToInt32(Session["StageId"].ToString());

                objUserMapping.WfUserMappingCategoryId = 1006; //Stage NotoficationUser
                objUserMapping.WfUserMappingGroupId = 0;
                objUserMapping.WfUserMappingUserId = Convert.ToInt32(selUserID);


                objUserMapping.ManageWorkflowUserMapping(objUserMapping);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            BindUsers(GetAllUsers());

            hdnCurrentPanel.Value = "2";
            hdnCurrentSubPanel.Value = "0";
        }

        protected void btnRemoveUser_NotoficationUser_Click(object sender, EventArgs e)
        {
            CheckAuthentication();
            try
            {
                string selUserID = lstAssignedUsers_NotoficationUser.SelectedItem.Value;

                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objUserMapping.LoginToken = loginUser.LoginToken;
                objUserMapping.LoginOrgId = loginUser.LoginOrgId;
                objUserMapping.WfUserMappingAction = "DeleteUser";
                objUserMapping.WfUserMappingProcessId = Convert.ToInt32(Session["ProcessId"].ToString());
                objUserMapping.WfUserMappingWorkflowId = Convert.ToInt32(Session["WorkflowId"].ToString());
                objUserMapping.WfUserMappingStageId = Convert.ToInt32(Session["StageId"].ToString());

                objUserMapping.WfUserMappingCategoryId = 1006; //Stage NotoficationUser
                objUserMapping.WfUserMappingGroupId = 0;
                objUserMapping.WfUserMappingUserId = Convert.ToInt32(selUserID);


                objUserMapping.ManageWorkflowUserMapping(objUserMapping);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            BindUsers(GetAllUsers());

            hdnCurrentPanel.Value = "2";
            hdnCurrentSubPanel.Value = "0";
        }

        protected void btnGoBacktoStage_Click(object sender, EventArgs e)
        {
            CheckAuthentication();

            string ProcessId = Session["ProcessId"].ToString(); //ProcessId
            string WorkflowId = Session["WorkflowId"].ToString(); //WorkflowId
            string workflowConfirmed = string.Empty;

            ProcessId = HttpUtility.UrlEncode(Encrypt(ProcessId));
            WorkflowId = HttpUtility.UrlEncode(Encrypt(WorkflowId));
            workflowConfirmed = HttpUtility.UrlEncode(Encrypt(workflowConfirmed));
            string QueryString = "?ProcessId=" + ProcessId + "&WorkflowId=" + (WorkflowId) + "&WorkflowConfirmed=" + workflowConfirmed; 

            Response.Redirect("~/Workflow/WorkflowAdmin/ManageStages.aspx" + QueryString);
        }

        // DMSENH6-4732 BS
        protected void DisableControls()
        {
            btnAddUser_StageOwner.Enabled = false;
            btnRemoveUser_StageOwner.Enabled = false;
            btnAddUser_StageUser.Enabled = false;
            btnRemoveUser_StageUser.Enabled = false;
            btnAddUser_StageUserGroup.Enabled = false;
            btnRemoveUser_StageUserGroup.Enabled = false;
            btnAddUser_NotoficationUser.Enabled = false;
            btnRemoveUser_NotoficationUser.Enabled = false;
        }
        // DMSENH6-4732 BE
    }
}