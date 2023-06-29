/* ===================================================================================================================================
 * Author     : 
 * Create date: 
 * Description:  
======================================================================================================================================  
 * Change History   
 * Date:            Author:             Issue ID            Description:  
 * -----------------------------------------------------------------------------------------------------------------------------------

 * 17 Apr 2015      Gokul               DMS5-3946       Applying rights an permissions in all pages as per user group rights!
====================================================================================================================================== */

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lotex.EnterpriseSolutions.CoreBE;
using WorkflowBLL.Classes;
using WorkflowBAL;

namespace Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin
{
    public partial class ManageMasterTypes : PageBase
    {
        public string pageRights = string.Empty;
        public string PageName = string.Empty;
        WorkflowMasterTypes ObjMasterType = new WorkflowMasterTypes();
        protected void Page_Load(object sender, EventArgs e)
        {
            CheckAuthentication();           
            pageRights = GetPageRights();/* DMS5-3946 A */
            if (!Page.IsPostBack)
            {
                ApplyPageRights(pageRights, this.Form.Controls); /* DMS5-3946 A */
                //Start: ------------------SitePath Details ----------------------
                Label sitePath = (Label)Master.FindControl("SitePath");
                sitePath.Text = "Define >> Master Types";
                //End: ------------------SitePath Details ------------------------

                BindDropDownMasterType();

                BindMasterTypeGridview(GetMasterTypes());

            }
        }

        protected void BindDropDownMasterType()
        {

            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            ObjMasterType.LoginToken = loginUser.LoginToken;
            ObjMasterType.LoginOrgId = loginUser.LoginOrgId;

            DBResult objresult = new DBResult();
            objresult = ObjMasterType.ManageMasterType(ObjMasterType, "BindDropDownMasterType");
            ddlParentType.DataSource = objresult.dsResult;
            ddlParentType.DataValueField = "WorkflowMasterTypes_iId";
            ddlParentType.DataTextField = "WorkflowMasterTypes_vName";
            ddlParentType.DataBind();

        }

        protected void gridMasterTypes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //// Hide unwanted rows

            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[1].Style.Add("display", "none"); //totalcnt
                e.Row.Cells[3].Style.Add("display", "none"); //type id
                e.Row.Cells[8].Style.Add("display", "none"); //parent type id
                e.Row.Cells[9].Style.Add("display", "none");  //activstatus

                e.Row.Cells[0].Style.Add("width", "20px"); //edit
                e.Row.Cells[2].Style.Add("width", "20px"); //sl no
                e.Row.Cells[4].Style.Add("width", "40px"); //Name
                e.Row.Cells[5].Style.Add("width", "150px"); //desc
                e.Row.Cells[6].Style.Add("width", "40px"); //parent type name
                e.Row.Cells[7].Style.Add("width", "20px"); //activ

            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // string TypeId = e.Row.Cells[3].Text;
                // Session["TypeId"] = TypeId;

            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            CheckAuthentication();

            string action = "AddMasterTypes";
            DBResult objResult = new DBResult();

            try
            {
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                ObjMasterType.LoginToken = loginUser.LoginToken;
                ObjMasterType.LoginOrgId = loginUser.LoginOrgId;

                ObjMasterType.WfTypeName = txtTypeName.Text.Trim();
                ObjMasterType.WfTypeDescription = txtTypeDescription.Text.Trim();
                ObjMasterType.WfTypeIsActive = chkActive.Checked;
                ObjMasterType.WfParentTypeId = 0;
                try
                {

                    ObjMasterType.WfParentTypeId = Convert.ToInt32(ddlParentType.SelectedValue.ToString());
                }
                catch
                { }
                string strSaveStatus = hdnSaveStatus.Value;

                if (strSaveStatus == "Save Changes")
                {
                    ObjMasterType.WfTypeId = Convert.ToInt32(hiddenMasterId.Value);
                    action = "EditMasterTypes";
                }

                objResult = ObjMasterType.ManageMasterType(ObjMasterType, action);
                handleDBResult(objResult);

                //only if success
                if (objResult.ErrorState == 0)
                {
                    clearControls();
                    BindMasterTypeGridview(GetMasterTypes());
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

                BindDropDownMasterType();
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
            txtTypeName.Text = string.Empty;
            txtTypeDescription.Text = string.Empty;
            chkActive.Checked = false;
        }

        protected void BindMasterTypeGridview(DBResult objResult)
        {
            if (objResult.dsResult != null && objResult.dsResult.Tables.Count > 0)
            {
                gridMasterTypes.DataSource = objResult.dsResult.Tables[0];
                gridMasterTypes.DataBind();


            }
        }

        private DBResult GetMasterTypes()
        {
            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            ObjMasterType.LoginToken = loginUser.LoginToken;
            ObjMasterType.LoginOrgId = loginUser.LoginOrgId;
            return ObjMasterType.ManageMasterType(ObjMasterType, "GetAllMasterTypes");
        }

        protected void gridMasterTypes_PageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            CheckAuthentication();
            gridMasterTypes.PageIndex = e.NewPageIndex;
            BindMasterTypeGridview(GetMasterTypes());

        }
    }
}