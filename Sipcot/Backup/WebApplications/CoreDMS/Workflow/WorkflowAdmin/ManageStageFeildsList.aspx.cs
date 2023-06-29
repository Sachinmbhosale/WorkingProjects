/* ============================================================================  
Author     : 
Create date: 
===============================================================================  
** Change History   
** Date:          Author:            Issue ID                         Description:  
** ----------   -------------       ----------                  ----------------------------
** 03 Dec 2013          Mandatory   (UMF)                Set Mandatory for index fields
 *
 * Modified
 * 28/02/2015   Sabina              DMS5-3384               Add dataentryId to querystring to enable coordinate textbox
 * 17 Apr 2015      Gokul               DMS5-3946       Applying rights an permissions in all pages as per user group rights!
 * 08 Apr 2015     Sabina              DMS5-4122       Button not enabled in workflow pages
=============================================================================== */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WorkflowBLL.Classes;
using Lotex.EnterpriseSolutions.CoreBE;
using WorkflowBAL;

namespace Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin
{
    public partial class ManageStageFeildsList : PageBase
    {
        WorkflowStageField ObjStageFeild = new WorkflowStageField();
        public string pageRights = string.Empty; /* DMS5-3946 A */
        
        protected void Page_Load(object sender, EventArgs e)
        {
            CheckAuthentication();
            pageRights = GetPageRights("CONFIGURATION");/* DMS5-3946 A,DMS5-4122M added parameter configuration */
            //Start: ------------------SitePath Details ----------------------
            Label sitePath = (Label)Master.FindControl("SitePath");
            sitePath.Text = "WorkFlow >> Process >> Workflow >> Stages >> Manage Fields";
            //End: ------------------SitePath Details ------------------------

            if (!IsPostBack)
            {
                ApplyPageRights(pageRights, this.Form.Controls); /* DMS5-3946 A */
                if (Request.QueryString["ProcessId"] != null)
                {
                    // Store process id in view state for later use
                    Session["ProcessId"] = Decrypt(HttpUtility.UrlDecode(Request.QueryString["ProcessId"]));
                    Session["WorkflowId"] = Decrypt(HttpUtility.UrlDecode(Request.QueryString["WorkflowId"]));
                    Session["StageId"] = Decrypt(HttpUtility.UrlDecode(Request.QueryString["StageId"]));

                    //DMS5-3384A
                    if (Request.QueryString["dataEntryTypeId"]!=null)
                    {
                        Session["DataEntryTypeId"] = Decrypt(HttpUtility.UrlDecode(Request.QueryString["dataEntryTypeId"]));
                    }
                   

                    if (Request.QueryString["StageName"] != null)
                    {
                        Session["StageName"] = Decrypt(HttpUtility.UrlDecode(Request.QueryString["StageName"]));
                    }

                    lblCurrentStageNameHeaderValue.Text = Session["StageName"].ToString();
                }
                else
                {
                    Response.Redirect("~/Workflow/WorkflowAdmin/ManageProcess.aspx");
                }
                Response.CacheControl = "no-cache";
                BindgridStageList(GetGridStageList());
            }
            
        }

        protected void gridStageList_Init(object sender, EventArgs e)
        {
            Response.CacheControl = "no-cache";
        }

        private DBResult GetGridStageList()
        {
            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            ObjStageFeild.LoginToken = loginUser.LoginToken;
            ObjStageFeild.LoginOrgId = loginUser.LoginOrgId;

            ObjStageFeild.WorkflowStageFieldsProcessId = Convert.ToInt32(Session["ProcessId"]);
            ObjStageFeild.WorkflowStageFieldsWorkflowId = Convert.ToInt32(Session["WorkflowId"]);
            ObjStageFeild.WorkflowStageFieldsStageId = Convert.ToInt32(Session["StageId"]);

            return ObjStageFeild.ManageStageFields(ObjStageFeild, "GetAllStageFields");
        }


        protected void BindgridStageList(DBResult objResult)
        {
            if (objResult.dsResult != null && objResult.dsResult.Tables.Count > 0)
            {
                gridStageList.DataSource = objResult.dsResult.Tables[0];
                gridStageList.DataBind();
            }

            if (objResult.dsResult != null && objResult.dsResult.Tables.Count > 1)
            {
                gridInheritedFields.DataSource = objResult.dsResult.Tables[1];
                gridInheritedFields.DataBind();
            }

        }
        
        protected void btnGotoStageFeilds_Click(object sender, EventArgs e)
        {
            string ProcessId = HttpUtility.UrlEncode(Encrypt((string)Session["ProcessId"]));
            string WorkflowId = HttpUtility.UrlEncode(Encrypt((string)Session["WorkflowId"]));
            string StageId = HttpUtility.UrlEncode(Encrypt((string)Session["StageId"]));

            string FieldStageId = HttpUtility.UrlEncode(Encrypt("0"));
            //DMS5-3384A
            string DataentryId = HttpUtility.UrlEncode(Encrypt((string)Session["DataEntryTypeId"]));
            string Querystring = "?ProcessId=" + ProcessId + "&WorkflowId=" + WorkflowId + "&StageId=" + StageId + "&FieldStageId=" + FieldStageId + "&DataentryId=" + DataentryId;
            Response.Redirect("~/Workflow/WorkflowAdmin/ManageStageFeildsEdit.aspx" + Querystring);

        }

        
        protected void gridStageList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[2].Style.Add("display", "none");
                e.Row.Cells[4].Style.Add("display", "none");
                e.Row.Cells[13].Style.Add("display", "none");
                e.Row.Cells[15].Style.Add("display", "none"); //Stage Id
                e.Row.Cells[17].Style.Add("display", "none"); //x1
                e.Row.Cells[18].Style.Add("display", "none"); //x2
                e.Row.Cells[19].Style.Add("display", "none"); //y1
                e.Row.Cells[20].Style.Add("display", "none"); //y2
                e.Row.Cells[21].Style.Add("display", "none"); //imagewidth
                e.Row.Cells[22].Style.Add("display", "none"); //ImageHeiht
                e.Row.Cells[24].Style.Add("display", "none"); //width
                e.Row.Cells[25].Style.Add("display", "none"); //height
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string ProcessId = HttpUtility.UrlEncode(Encrypt((string)Session["ProcessId"]));
                string WorkflowId = HttpUtility.UrlEncode(Encrypt((string)Session["WorkflowId"]));
                string FieldStageId = HttpUtility.UrlEncode(Encrypt(e.Row.Cells[15].Text));
                string StageId = HttpUtility.UrlEncode(Encrypt((string)Session["StageId"]));

                string FieldId = HttpUtility.UrlEncode(Encrypt(e.Row.Cells[4].Text));
                string GridType = HttpUtility.UrlEncode(Encrypt("MainGrid"));
                //DMS5-3384A
                string DataentryId = HttpUtility.UrlEncode(Encrypt((string)Session["DataEntryTypeId"]));
                string Querystring = "?ProcessId=" + ProcessId + "&WorkflowId=" + WorkflowId + "&StageId=" + StageId + "&FieldStageId=" + FieldStageId + "&FieldId=" + FieldId + "&GridType=" + GridType+"&DataentryId=" + DataentryId;
                LinkButton lnkManageStageUsers = (LinkButton)e.Row.FindControl("lnkEdit");
                lnkManageStageUsers.PostBackUrl = "~/Workflow/WorkflowAdmin/ManageStageFeildsEdit.aspx" + Querystring;
            }
        }

        protected void gridInheritedFields_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[1].Style.Add("display", "none");
                e.Row.Cells[3].Style.Add("display", "none");
                e.Row.Cells[13].Style.Add("display", "none");
                e.Row.Cells[14].Style.Add("display", "none"); //Stage Id
                e.Row.Cells[16].Style.Add("display", "none"); //x1
                e.Row.Cells[17].Style.Add("display", "none"); //x1
                e.Row.Cells[18].Style.Add("display", "none"); //x2
                e.Row.Cells[19].Style.Add("display", "none"); //y1
                e.Row.Cells[20].Style.Add("display", "none"); //y2
                e.Row.Cells[21].Style.Add("display", "none"); //imagewidth

                e.Row.Cells[23].Style.Add("display", "none"); //ImageHeiht
                e.Row.Cells[24].Style.Add("display", "none"); //width
              
            }
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string ProcessId = HttpUtility.UrlEncode(Encrypt((string)Session["ProcessId"]));
                string WorkflowId = HttpUtility.UrlEncode(Encrypt((string)Session["WorkflowId"]));
                string FieldStageId = HttpUtility.UrlEncode(Encrypt(e.Row.Cells[14].Text));
                string StageId = HttpUtility.UrlEncode(Encrypt((string)Session["StageId"]));

                string FieldId = HttpUtility.UrlEncode(Encrypt(e.Row.Cells[3].Text));
                string GridType = HttpUtility.UrlEncode(Encrypt("InheritedGrid"));
                //DMS5-3384A
                string DataentryId = HttpUtility.UrlEncode(Encrypt((string)Session["DataEntryTypeId"]));
                string Querystring = "?ProcessId=" + ProcessId + "&WorkflowId=" + WorkflowId + "&StageId=" + StageId + "&FieldStageId=" + FieldStageId + "&FieldId=" + FieldId + "&GridType=" + GridType + "&DataentryId=" + DataentryId; 
                LinkButton lnkManageStageUsers = (LinkButton)e.Row.FindControl("lnkView");
                lnkManageStageUsers.PostBackUrl = "~/Workflow/WorkflowAdmin/ManageStageFeildsEdit.aspx" + Querystring;
                
            }

        }
        protected void gridStageList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridStageList.PageIndex = e.NewPageIndex;
            BindgridStageList(GetGridStageList());
        }


        protected void gridInheritedFields_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridInheritedFields.PageIndex = e.NewPageIndex;
            BindgridStageList(GetGridStageList());
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

        protected void btnMoveUp_Click(object sender, EventArgs e)
        {
            CheckAuthentication();
            Response.CacheControl = "no-cache";
            lblMessage.Text = "";
            lblMessage.Visible = false;
            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            ObjStageFeild.LoginToken = loginUser.LoginToken;
            ObjStageFeild.LoginOrgId = loginUser.LoginOrgId;

            ObjStageFeild.WorkflowStageFieldsProcessId = Convert.ToInt32(Session["ProcessId"]);
            ObjStageFeild.WorkflowStageFieldsWorkflowId = Convert.ToInt32(Session["WorkflowId"]);
            ObjStageFeild.WorkflowStageFieldsStageId = Convert.ToInt32(Session["StageId"]);

            ObjStageFeild.WorkflowStageFieldsId = Convert.ToInt32(hdnStageFeildId.Value);
            ObjStageFeild.MapFieldsToTemplate_iSortOrder = Convert.ToInt32(hdnStageFeildOrder.Value);

            objResult = ObjStageFeild.ManageStageFields(ObjStageFeild, "UpdateFieldDisplayOrder");

            //only if success
            if (objResult.ErrorState == 0)
            {
                //Response.CacheControl = "no-cache";
                BindgridStageList(GetGridStageList());
            }
            else
            {
                lblMessage.Visible = true;
                lblMessage.Text = objResult.Message.ToString();
            }
        }

        protected void btnMoveDown_Click(object sender, EventArgs e)
        {
            CheckAuthentication();
            Response.CacheControl = "no-cache";
            lblMessage.Text = "";
            lblMessage.Visible = false;
            DBResult objResult = new DBResult();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            ObjStageFeild.LoginToken = loginUser.LoginToken;
            ObjStageFeild.LoginOrgId = loginUser.LoginOrgId;

            ObjStageFeild.WorkflowStageFieldsProcessId = Convert.ToInt32(Session["ProcessId"]);
            ObjStageFeild.WorkflowStageFieldsWorkflowId = Convert.ToInt32(Session["WorkflowId"]);
            ObjStageFeild.WorkflowStageFieldsStageId = Convert.ToInt32(Session["StageId"]);

            ObjStageFeild.WorkflowStageFieldsId = Convert.ToInt32(hdnStageFeildId.Value);
            ObjStageFeild.MapFieldsToTemplate_iSortOrder = Convert.ToInt32(hdnStageFeildOrder.Value);

            objResult = ObjStageFeild.ManageStageFields(ObjStageFeild, "UpdateFieldDisplayOrder");

            //only if success
            if (objResult.ErrorState == 0)
            {
                //Response.CacheControl = "no-cache";
                BindgridStageList(GetGridStageList());
            }
            else
            {
                lblMessage.Visible = true;
                lblMessage.Text = objResult.Message.ToString();
            }
        }
    }
}