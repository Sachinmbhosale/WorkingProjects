/* ============================================================================  
Author     : 
Create date: 
===============================================================================  
** Change History   
** Date:          Author:            Issue ID                         Description:  
** ----------   -------------       ----------                  ----------------------------
31/08/2015		Sharath				DMSENH6-4796  Bulk Upload in the Workflow

=============================================================================== */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WorkflowBLL.Classes;
using WorkflowBAL;
using Lotex.EnterpriseSolutions.CoreBE;
using System.Data;

namespace Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin
{
    public partial class ManageWorkflowDmsFieldsMapping : PageBase
    {
        //global variable
        WorkflowDmsFieldsMapping objUpload = new WorkflowDmsFieldsMapping();
        public string pageRights = string.Empty; /* DMS5-3946 A */

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                CheckAuthentication();
                pageRights = GetPageRights("CONFIGURATION");/* DMS5-3946 A,DMS5-4122M added parameter configuration */
                ApplyPageRights(pageRights, this.Form.Controls); /* DMS5-3946 A */
                hdnProcessId.Value = Decrypt(HttpUtility.UrlDecode(Request.QueryString["ProcessId"]));
                hdnWorkflowId.Value = Decrypt(HttpUtility.UrlDecode(Request.QueryString["WorkflowId"]));
                hdnStageId.Value = Decrypt(HttpUtility.UrlDecode(Request.QueryString["StageId"]));
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
                hdnLoginToken.Value = loginUser.LoginToken;
                BindProjectTypeDropDown();


            }
        }

        //DMSENH6-4796  BS
        protected void BindProjectTypeDropDown()
        {
            try
            {
                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objUpload.LoginToken = loginUser.LoginToken;
                objUpload.LoginOrgId = loginUser.LoginOrgId;
                objUpload.WorkflowId = Convert.ToInt32(hdnWorkflowId.Value);
                objResult = objUpload.ManageUpload(objUpload, "BindDMSProjectDropDown");
                ddlprojecttype.DataSource = objResult.dsResult;
                ddlprojecttype.DataTextField = "DOCTYPEMASTER_vName";
                ddlprojecttype.DataValueField = "DOCTYPEMASTER_iID";
                ddlprojecttype.DataBind();

                BindListitem(ddldepartment);

            }

            catch (Exception ex)
            {

            }
            //Make workflow and stage dropdown as defalt value


        }
        protected void BindDepartmentTypeDropDown()
        {
            try
            {
                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objUpload.LoginToken = loginUser.LoginToken;
                objUpload.LoginOrgId = loginUser.LoginOrgId;
                objUpload.DoctypeId = Convert.ToInt32(ddlprojecttype.SelectedValue);
                objResult = objUpload.ManageUpload(objUpload, "BindDepartmentDropDown");
                ddldepartment.DataSource = objResult.dsResult;
                ddldepartment.DataTextField = "DepartmentName";
                ddldepartment.DataValueField = "DepartmentID";
                ddldepartment.DataBind();


            }

            catch (Exception ex)
            {

            }
            //Make workflow and stage dropdown as defalt value


        }



        protected void BindListitem(DropDownList selectedDropDownId)
        {
            ListItem li = new ListItem("---Select---", "0");
            selectedDropDownId.Items.Clear();
            selectedDropDownId.Items.Add(li);
        }

        protected void ddlprojecttype_SelectedIndexChanged(object sender, EventArgs e)
        {

            BindDepartmentTypeDropDown();


        }

        protected void BindWorkflowFieldsListBox()
        {
            try
            {
                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objUpload.LoginToken = loginUser.LoginToken;
                objUpload.LoginOrgId = loginUser.LoginOrgId;
                objUpload.ProcessId = Convert.ToInt32(hdnProcessId.Value);
                objUpload.WorkflowId = Convert.ToInt32(hdnWorkflowId.Value);
                objUpload.StageID = Convert.ToInt32(hdnStageId.Value);
                objResult = objUpload.ManageUpload(objUpload, "GetWorkflowFieldNamesForMapping");
                lstFieldsFromWorkflow.DataSource = objResult.dsResult;
                lstFieldsFromWorkflow.DataTextField = "VALUE";
                lstFieldsFromWorkflow.DataValueField = "ID";
                lstFieldsFromWorkflow.DataBind();


            }

            catch (Exception ex)
            {

            }
        }

        protected void ddldepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitialLoadControls();

        }

        protected void BindFieldNamesFromDMS()
        {
            try
            {
                DBResult objResult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objUpload.LoginToken = loginUser.LoginToken;
                objUpload.LoginOrgId = loginUser.LoginOrgId;
                objUpload.ProcessId = Convert.ToInt32(hdnProcessId.Value);
                objUpload.WorkflowId = Convert.ToInt32(hdnWorkflowId.Value);
                objUpload.StageID = Convert.ToInt32(hdnStageId.Value);
                objUpload.DoctypeId = Convert.ToInt32(ddlprojecttype.SelectedValue);
                objUpload.DepatmentId = Convert.ToInt32(ddldepartment.SelectedValue);
                objResult = objUpload.ManageUpload(objUpload, "GetDMSFieldNamesForMapping");
                lstFieldsFromDMS.DataSource = objResult.dsResult;
                lstFieldsFromDMS.DataTextField = "VALUE";
                lstFieldsFromDMS.DataValueField = "ID";
                lstFieldsFromDMS.DataBind();


            }

            catch (Exception ex)
            {

            }
        }

        protected void LoadMappingDetails()
        {
            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            objUpload.LoginToken = loginUser.LoginToken;
            objUpload.LoginOrgId = loginUser.LoginOrgId;
            objUpload.DoctypeId = Convert.ToInt32(ddlprojecttype.SelectedValue);
            objUpload.DepatmentId = Convert.ToInt32(ddldepartment.SelectedValue);

            objUpload.ProcessId = Convert.ToInt32(hdnProcessId.Value);
            objUpload.WorkflowId = Convert.ToInt32(hdnWorkflowId.Value);
            objUpload.StageID = Convert.ToInt32(hdnStageId.Value);
            objResult = objUpload.ManageUpload(objUpload, "GetDMSWorkFlowMappingFields");
            lstMappedcontrolfields.DataSource = objResult.dsResult;
            lstMappedcontrolfields.DataTextField = "VALUE";
            lstMappedcontrolfields.DataValueField = "ID";
            lstMappedcontrolfields.DataBind();
        }

        protected void btnmap_Click(object sender, EventArgs e)
        {
            try
            {
                Logger.Trace("Inserting mapping started", Session["LoggedUserId"].ToString());

                string xmlValues = AddMappingItems();
                DataSet dsResult = new DataSet();
                dsResult = DbCall("InsertFeildsMapping", xmlValues, "");

                if (dsResult.Tables.Count > 0)
                {
                    //divMsg.InnerHtml = Message;
                    LoadListMapping(dsResult.Tables[0]);
                }

                InitialLoadControls();
            }
            catch (Exception ex)
            {

                divMsg.InnerHtml = ex.Message.ToString();
                Logger.Trace(ex.Message.ToString(), Session["LoggedUserId"].ToString());
            }




        }

        protected string AddMappingItems()
        {
            string XML = string.Empty;
            try
            {
                Logger.Trace("Generateing XML started ", Session["LoggedUserId"].ToString());

                DataTable dt = new DataTable();
                DataSet ds = new DataSet();
                dt.Columns.Add("DmsFieldsId", typeof(int)); //adding columns
                dt.Columns.Add("WorkflowFieldsId", typeof(int));
                dt.Rows.Add(lstFieldsFromDMS.SelectedValue, lstFieldsFromWorkflow.SelectedValue);
                ds.Tables.Add(dt);
                XML = ds.GetXml();
            }
            catch (Exception ex)
            {
                divMsg.InnerHtml = ex.Message.ToString();
                Logger.Trace(ex.Message.ToString(), Session["LoggedUserId"].ToString());
            }
            return XML;


        }

        protected void LoadListMapping(DataTable DT)
        {
            try
            {


                lstMappedcontrolfields.DataTextField = "Value";
                lstMappedcontrolfields.DataValueField = "Id";
                lstMappedcontrolfields.DataSource = DT;
                lstMappedcontrolfields.DataBind();
            }
            catch (Exception ex)
            {
                divMsg.InnerHtml = ex.Message.ToString();
                Logger.Trace(ex.Message.ToString(), Session["LoggedUserId"].ToString());
            }

        }


        protected DataSet DbCall(string Action, string XML, string Filepath, int MappingId = 0)
        {
            DataSet Ds = new DataSet();
            try
            {


                objUpload.ProcessId = Convert.ToInt32(hdnProcessId.Value);
                objUpload.WorkflowId = Convert.ToInt32(hdnWorkflowId.Value);
                objUpload.StageID = Convert.ToInt32(hdnStageId.Value);
                objUpload.LoginToken = hdnLoginToken.Value;
                objUpload.LoginOrgId = Convert.ToInt32(hdnLoginOrgId.Value);
                objUpload.DoctypeId = Convert.ToInt32(ddlprojecttype.SelectedValue);
                objUpload.DepatmentId = Convert.ToInt32(ddldepartment.SelectedValue);
                objUpload.XmlData = XML;
                objUpload.MappingId = MappingId;

                DBResult ObjstageFieldDetails = objUpload.ManageUpload(objUpload, Action);





                Ds = ObjstageFieldDetails.dsResult;


            }
            catch (Exception ex)
            {
                divMsg.InnerHtml = ex.Message.ToString();
            }

            return Ds;

        }


        protected void btnremove_Click(object sender, EventArgs e)
        {
            try
            {
                Logger.Trace("Deleteing mapping started", Session["LoggedUserId"].ToString());

                DataSet dsResult = new DataSet();
                dsResult = DbCall("DeleteFieldMapping", "", "", Convert.ToInt32(lstMappedcontrolfields.SelectedValue));
                BindFieldNamesFromDMS();
                BindWorkflowFieldsListBox();
                lstMappedcontrolfields.Items.Remove(lstMappedcontrolfields.Items.FindByValue(lstMappedcontrolfields.SelectedValue.ToString()));

            }
            catch (Exception ex)
            {

                divMsg.InnerHtml = ex.Message.ToString();
                Logger.Trace(ex.Message.ToString(), Session["LoggedUserId"].ToString());
            }

        }


        protected void InitialLoadControls()
        {
            try
            {

                BindWorkflowFieldsListBox();
                BindFieldNamesFromDMS();
                LoadMappingDetails();


            }
            catch (Exception ex)
            {
                divMsg.InnerHtml = ex.Message.ToString();
                Logger.Trace("InitialLoadControls Error", Session["LoggedUserId"].ToString());
            }

        }

        protected void LoadDMSFieldsListBox(DataTable DT)
        {
            try
            {


                lstFieldsFromDMS.DataTextField = "Value";
                lstFieldsFromDMS.DataValueField = "ID";
                lstFieldsFromDMS.DataSource = DT;
                lstFieldsFromDMS.DataBind();
                //if (lstFieldsFromDMS.Items.Count <= 0)
                //{
                //    divMsg.InnerHtml = "Please upload a Pdf template.";
                //}

            }
            catch (Exception ex)
            {
                divMsg.InnerHtml = ex.Message.ToString();
            }

        }


        protected void LoadWorkflowFields(DataTable DT)
        {
            try
            {


                lstFieldsFromWorkflow.DataTextField = "Value";
                lstFieldsFromWorkflow.DataValueField = "ID";
                lstFieldsFromWorkflow.DataSource = DT;
                lstFieldsFromWorkflow.DataBind();


            }
            catch (Exception ex)
            {
                divMsg.InnerHtml = ex.Message.ToString();
            }

        }

        protected void btncommit_Click(object sender, EventArgs e)
        {

            DBResult dsResult = new DBResult();
            dsResult.dsResult = DbCall("CommitFieldmapping", string.Empty, string.Empty);
            InitialLoadControls();
            if (dsResult.ErrorState == 0)
            {
                divMsg.InnerHtml = "Fields commited sucessfully";

            }


        }

        protected void btnGotoWorkflow_Click(object sender, EventArgs e)
        {
            //string ProcessId = HttpUtility.UrlEncode(Encrypt(Convert.ToString(hdnProcessId)));
            //string workflowId = HttpUtility.UrlEncode(Encrypt(Convert.ToString(hdnWorkflowId)));
            //string stageId = HttpUtility.UrlEncode(Encrypt(Convert.ToString(hdnStageId)));

            //string Querystring = "?ProcessId=" + ProcessId + "&workflowId=" + workflowId + "&stageId=" + stageId;
            Response.Redirect("~/Workflow/WorkflowAdmin/ManageWorkflowBulkUpload.aspx"); //+ Querystring);
        }


    }
}