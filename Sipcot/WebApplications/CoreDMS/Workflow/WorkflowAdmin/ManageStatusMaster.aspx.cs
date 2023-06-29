using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lotex.EnterpriseSolutions.CoreBE;
using WorkflowBLL.Classes;
using WorkflowBAL;

namespace Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin
{
    public partial class ManageStatusMaster : PageBase
    {
        WorkflowStatusMaster ObjStatusMaster = new WorkflowStatusMaster();
        public string pageRights = string.Empty; /* DMS5-3946 A */
       

        protected void Page_Load(object sender, EventArgs e)
        {
            CheckAuthentication();           
            pageRights = GetPageRights();/* DMS5-3946 A */
            if (!Page.IsPostBack)
            {
                ApplyPageRights(pageRights, this.Form.Controls); /* DMS5-3946 A */
                //Start: ------------------SitePath Details ----------------------
                Label sitePath = (Label)Master.FindControl("SitePath");
                sitePath.Text = "Define >> Status Master";
                //End: ------------------SitePath Details ------------------------

               // BindDropDownMasterType();

                BindstatusMasterGridview(GetStatusMaster());

            }
        }

        //protected void BindDropDownMasterType()
        //{

        //    DBResult objResult = new DBResult();
        //    UserBase loginUser = (UserBase)Session["LoggedUser"];
        //    ObjStatusMaster.LoginToken = loginUser.LoginToken;
        //    ObjStatusMaster.LoginOrgId = loginUser.LoginOrgId;

        //    DBResult objresult = new DBResult();
        //    objresult = ObjStatusMaster.ManageWorkflowStatusMaster(ObjStatusMaster, "BindDropDownMasterType");
        //    ddlParentType.DataSource = objresult.dsResult;
        //    ddlParentType.DataValueField = "WorkflowMasterTypes_iId";
        //    ddlParentType.DataTextField = "WorkflowMasterTypes_vName";
        //    ddlParentType.DataBind();

        //}

        protected void GridStatusMaster_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //// Hide unwanted rows
            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[1].Style.Add("display", "none"); //StatusId
                e.Row.Cells[3].Style.Add("display", "none"); //Status Short Name
                e.Row.Cells[7].Style.Add("display", "none"); //Active status

                e.Row.Cells[0].Style.Add("width", "20px"); //edit
                e.Row.Cells[2].Style.Add("width", "30px"); //Status Name
                e.Row.Cells[4].Style.Add("width", "70px"); //Name
                e.Row.Cells[5].Style.Add("width", "150px"); //Status Description
                e.Row.Cells[6].Style.Add("width", "30px"); //Active
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center; //edit
                e.Row.Cells[6].HorizontalAlign = HorizontalAlign.Center;//Active
            }

        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            CheckAuthentication();
            string action = "AddStatusMaster";
            DBResult objResult = new DBResult();

            try
            {
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                ObjStatusMaster.LoginToken = loginUser.LoginToken;
                ObjStatusMaster.LoginOrgId = loginUser.LoginOrgId;

                ObjStatusMaster.WorkflowStatusMasterStatusName = txtStatusName.Text.Trim();
                ObjStatusMaster.WorkflowStatusMasterDecription = txtStatusDescription.Text.Trim();
                ObjStatusMaster.WorkflowStatusMasterIsActive = chkActive.Checked;
                ObjStatusMaster.WorkflowStatusMasterShortName = txtStatusName.Text.Substring(0,5).Trim();
                string strSaveStatus = hdnSaveStatus.Value;

                if (strSaveStatus == "Save Changes")
                {
                    ObjStatusMaster.WorkflowStatusMasterStatusId = Convert.ToInt32(hiddenMasterId.Value);
                    action = "EditStatusMaster";
                }

                objResult = ObjStatusMaster.ManageWorkflowStatusMaster(ObjStatusMaster, action);
                handleDBResult(objResult);

                //only if success
                if (objResult.ErrorState == 0)
                {
                    clearControls();
                    BindstatusMasterGridview(GetStatusMaster());
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
            txtStatusName.Text = string.Empty;
            txtStatusDescription.Text = string.Empty;
            chkActive.Checked = false;
        }

        protected void BindstatusMasterGridview(DBResult objResult)
        {
            if (objResult.dsResult != null && objResult.dsResult.Tables.Count > 0)
            {
                GridStatusMaster.DataSource = objResult.dsResult.Tables[0];
                GridStatusMaster.DataBind();


            }
        }

        private DBResult GetStatusMaster()
        {
            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            ObjStatusMaster.LoginToken = loginUser.LoginToken;
            ObjStatusMaster.LoginOrgId = loginUser.LoginOrgId;
            return ObjStatusMaster.ManageWorkflowStatusMaster(ObjStatusMaster, "GetAllStatusMaster");
        }

        protected void GridStatusMaster_PageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            CheckAuthentication();
            GridStatusMaster.PageIndex = e.NewPageIndex;
            BindstatusMasterGridview(GetStatusMaster());

        }
    }
}