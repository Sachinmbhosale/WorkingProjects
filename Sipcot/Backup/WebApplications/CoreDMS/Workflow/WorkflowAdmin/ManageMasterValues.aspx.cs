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
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WorkflowBAL;
using Lotex.EnterpriseSolutions.CoreBE;
using WorkflowBLL.Classes;
using System.Web.Services;
using System.Data;

namespace Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin
{
    public partial class ManageMasterValues : PageBase
    {
        #region GLOBAL DECLARATION

        WorkflowMasterValues ObjMasterValues = new WorkflowMasterValues();
        WorkflowMasterTypes ObjMasterTypes = new WorkflowMasterTypes();
        public string pageRights = string.Empty;
        
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            CheckAuthentication();          
            pageRights = GetPageRights();/* DMS5-3946 A */
            //Start: ------------------SitePath Details ----------------------
            Label sitePath = (Label)Master.FindControl("SitePath");
            sitePath.Text = "Define >> Master Values";
            //End: ------------------SitePath Details ------------------------

            if (!IsPostBack)
            {

                ApplyPageRights(pageRights, this.Form.Controls); /* DMS5-3946 A */
                BindDropDownMasterType();

                BindMasterValueGridview(GetMasterValues());

            }


            ViewState["TypeId"] = ddlMasterType.SelectedValue;
            ViewState["ParentId"] = ddlParentValue.SelectedValue;
        }


        protected void BindDropDownMasterType()
        {
            DBResult objresult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            ObjMasterTypes.LoginToken = loginUser.LoginToken;
            ObjMasterTypes.LoginOrgId = loginUser.LoginOrgId;

            objresult = ObjMasterTypes.ManageMasterType(ObjMasterTypes, "BindDropDownMasterType");
            ddlMasterType.DataSource = objresult.dsResult;
            ddlMasterType.DataValueField = "WorkflowMasterTypes_iId";
            ddlMasterType.DataTextField = "WorkflowMasterTypes_vName";
            ddlMasterType.DataBind();

        }
        protected void BindDropDownParentValue()
        {
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            ObjMasterValues.LoginToken = loginUser.LoginToken;
            ObjMasterValues.LoginOrgId = loginUser.LoginOrgId;

            ObjMasterValues.WfMasterValueName = txtValueName.Text.Trim();
            ObjMasterValues.WfMasterValueDescription = txtValueDescription.Text.Trim();
            ObjMasterValues.WfMasterValueIsActive = chkActive.Checked;
            try
            {
                ObjMasterValues.WfMasterTypeId = Convert.ToInt32(ddlMasterType.SelectedValue.ToString());
            }
            catch
            {
                ObjMasterValues.WfMasterTypeId = 0;
            }
            ObjMasterValues.WfMasterParentId = 0;

            DBResult objresult = new DBResult();
            objresult = ObjMasterValues.ManageMasterValues(ObjMasterValues, "BindDropDownParentValue");
            ddlParentValue.DataSource = objresult.dsResult;
            ddlParentValue.DataValueField = "WorkflowMasterValues_iId";
            ddlParentValue.DataTextField = "WorkflowMasterValues_vName";
            ddlParentValue.DataBind();

        }

        protected void ddlMasterValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckAuthentication();
            lblMessage.Text = "";
            BindDropDownParentValue();

            hdnErrorStatus.Value = "FILL_MASTER_VALUES";

        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            CheckAuthentication();
            string action = "AddMasterValues";
            DBResult objResult = new DBResult();

            try
            {
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                ObjMasterValues.LoginToken = loginUser.LoginToken;
                ObjMasterValues.LoginOrgId = loginUser.LoginOrgId;

                ObjMasterValues.WfMasterValueName = txtValueName.Text.Trim();
                ObjMasterValues.WfMasterValueDescription = txtValueDescription.Text.Trim();
                ObjMasterValues.WfMasterValueIsActive = chkActive.Checked;
                ObjMasterValues.WfMasterTypeId = Convert.ToInt32(ViewState["TypeId"]);
                ObjMasterValues.WfMasterParentId = Convert.ToInt32(ViewState["ParentId"]);
                string strSaveStatus = hdnSaveStatus.Value;

                if (strSaveStatus == "Save Changes")
                {
                    ObjMasterValues.WfMasterValueId = Convert.ToInt32(hiddenValueId.Value);
                    action = "EditMasterValues";
                }

                objResult = ObjMasterValues.ManageMasterValues(ObjMasterValues, action);
                handleDBResult(objResult);

                //only if success
                if (objResult.ErrorState == 0)
                {
                    clearControls();
                    BindMasterValueGridview(GetMasterValues());
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
                    
                    try
                    {
                        ddlMasterType.SelectedValue = hiddenMasterTypeId.Value;
                        ddlParentValue.SelectedValue = hiddenParentValueId.Value;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
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

        protected void gridMasterValue_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //// Hide unwanted rows

            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[1].Style.Add("display", "none");
                e.Row.Cells[3].Style.Add("display", "none");
                e.Row.Cells[5].Style.Add("display", "none");
                e.Row.Cells[8].Style.Add("display", "none");
                e.Row.Cells[11].Style.Add("display", "none");
                e.Row.Cells[0].Style.Add("width", "20px");
                e.Row.Cells[2].Style.Add("width", "20px");
                e.Row.Cells[4].Style.Add("width", "40px");
                e.Row.Cells[6].Style.Add("width", "40px");
                e.Row.Cells[7].Style.Add("width", "200px");
                e.Row.Cells[9].Style.Add("width", "40px");
                e.Row.Cells[10].Style.Add("width", "20px");
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //string ProcessId = HttpUtility.UrlEncode(Encrypt(e.Row.Cells[3].Text)); //ProcessId
                //string QueryString = "?ProcessId=" + Server.UrlEncode(ProcessId);
                //string URL = "ManageWorkflow.aspx" + QueryString;

                //e.Row.Attributes.Add("ondblclick", "window.location.href ='" + URL + "'");

            }
        }
        private void clearControls()
        {
            txtValueName.Text = string.Empty;
            txtValueDescription.Text = string.Empty;
            chkActive.Checked = false;
            BindDropDownMasterType();
            ddlMasterType.SelectedIndex = 0;
        }

        private DBResult GetMasterValues()
        {
            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            ObjMasterValues.LoginToken = loginUser.LoginToken;
            ObjMasterValues.LoginOrgId = loginUser.LoginOrgId;
            return ObjMasterValues.ManageMasterValues(ObjMasterValues, "GetAllMasterValues");
        }
        protected void BindMasterValueGridview(DBResult objResult)
        {
            if (objResult.dsResult != null && objResult.dsResult.Tables.Count > 0)
            {
                gridMasterValues.DataSource = objResult.dsResult.Tables[0];
                gridMasterValues.DataBind();
            }
        }

        protected void btnHiddenSubmit_Click(object sender, EventArgs e)
        {

            CheckAuthentication();

            BindDropDownParentValue();
            lblMessage.Text = "";
            try
            {
                ddlMasterType.SelectedValue = hiddenMasterTypeId.Value;
                ddlParentValue.SelectedValue = hiddenParentValueId.Value;
            }
            catch
            {
            }
            hdnErrorStatus.Value = "FILL_MASTER_VALUES";
        }

        protected void gridMasterValue_PageIndexChanging(Object sender, GridViewPageEventArgs e)
        {
            CheckAuthentication();
            gridMasterValues.PageIndex = e.NewPageIndex;
            BindMasterValueGridview(GetMasterValues());

        }
    }
}