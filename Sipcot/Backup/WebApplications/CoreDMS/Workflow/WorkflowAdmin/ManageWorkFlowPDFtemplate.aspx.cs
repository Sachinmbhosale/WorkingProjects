
/**============================================================================  
Author     : Gokuldas Palapatta
Create date: 24-08-2015
===============================================================================  
** Change History   
** Date:            Author:    Issue ID    	     Description: 
**(dd MMM yyyy)
** -----------------------------------------------------------------------------
** 25/8/2015       Gokul       DMSENH6-4776     Data Captured Form usage
**===============================================================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using Lotex.EnterpriseSolutions.CoreBE;
using System.Data;
using OfficeConverter;
using WorkflowBLL.Classes;
using WorkflowBAL;


namespace Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin
{
    // DMSENH6-4776 BS
    public partial class ManageWorkFlowPDFtemplate : PageBase
    {
        WorkflowPdfTemplateBLL objWorkflowPdfTemplateBLL = new WorkflowPdfTemplateBLL();
        public string pageRights = string.Empty; /* DMS5-3946 A */
        public string Message = string.Empty;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CheckAuthentication();
                pageRights = GetPageRights("CONFIGURATION");/* DMS5-3946 A,DMS5-4122M added parameter configuration */
                ApplyPageRights(pageRights, this.Form.Controls); /* DMS5-3946 A */
                hdnProcessId.Value = Decrypt(HttpUtility.UrlDecode(Request.QueryString["ProcessId"]));
                hdnWorkflowId.Value = Decrypt(HttpUtility.UrlDecode(Request.QueryString["WorkflowId"]));
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
                hdnLoginToken.Value = loginUser.LoginToken;
                InitialLoadControls();
            }
        }


        /// <summary>
        /// loading all values intitailly
        /// </summary>
        protected void InitialLoadControls()
        {
            try
            {

                Logger.Trace("InitialLoadControls ", Session["LoggedUserId"].ToString());

                DataSet Ds = new DataSet();
                Ds = DbCall("GetAllWorkFlowControls", "", "");



                if (Ds.Tables.Count > 0)
                {
                    LoadControlListBox(Ds.Tables[0]);
                }
                Ds = new DataSet();
                Ds = DbCall("GetStageFields", "", "");


                if (Ds.Tables.Count > 0)
                {
                    LoadStageFields(Ds.Tables[0]);
                }

                DataSet dsResult = new DataSet();
                dsResult = DbCall("GetAllTemplateMapping", "", "");

                if (dsResult.Tables.Count > 0)
                {
                    LoadListMapping(dsResult.Tables[0]);
                }


            }
            catch (Exception ex)
            {
                divMsg.InnerHtml = ex.Message.ToString();
                Logger.Trace("InitialLoadControls Error", Session["LoggedUserId"].ToString());
            }

        }

        /// <summary>
        /// load stagefields listbox
        /// </summary>
        /// <param name="DT"></param>
        protected void LoadStageFields(DataTable DT)
        {
            try
            {


                lststagefields.DataTextField = "VALUE";
                lststagefields.DataValueField = "ID";
                lststagefields.DataSource = DT;
                lststagefields.DataBind();
            }
            catch (Exception ex)
            {
                divMsg.InnerHtml = ex.Message.ToString();
            }

        }

        /// <summary>
        /// load controll listbox
        /// </summary>
        /// <param name="DT"></param>
        protected void LoadControlListBox(DataTable DT)
        {
            try
            {


                lstPdfControls.DataTextField = "ControlNameAndType";
                lstPdfControls.DataValueField = "ID";
                lstPdfControls.DataSource = DT;
                lstPdfControls.DataBind();
                if (lstPdfControls.Items.Count <= 0)
                {
                    divMsg.InnerHtml = "Please upload a Pdf template.";
                }

            }
            catch (Exception ex)
            {
                divMsg.InnerHtml = ex.Message.ToString();
            }

        }

        /// <summary>
        /// uploading pdf template
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AsyncFileUpload1_UploadedComplete(object sender, AjaxControlToolkit.AsyncFileUploadEventArgs e)
        {

            try
            {
                Logger.Trace("AsyncFileUpload1_UploadedComplete started ", Session["LoggedUserId"].ToString());

                string TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"];
                string OrginalFolder = System.Configuration.ConfigurationManager.AppSettings["WorkFlowTemplateLocation"];

                string key = Guid.NewGuid().ToString();


                OrginalFolder += hdnLoginOrgId.Value + "\\" + hdnLoginToken.Value + "\\" + key + "\\";

                TempFolder += hdnLoginOrgId.Value + "\\" + hdnLoginToken.Value + "\\";

                DataTable Dt = new DataTable();

                if (!Directory.Exists(TempFolder))
                {
                    Directory.CreateDirectory(TempFolder);
                }
                if (!Directory.Exists(OrginalFolder))
                {
                    Directory.CreateDirectory(OrginalFolder);
                }
                if (AsyncFileUpload1.HasFile)
                {
                    AsyncFileUpload1.SaveAs(TempFolder + AsyncFileUpload1.FileName);

                    Dt = new GettingControlsFromPDF().getcontrolsnames(TempFolder + AsyncFileUpload1.FileName);
                    DataSet ds = new DataSet();
                    ds.Tables.Add(Dt);
                    string xmlValues = ds.GetXml().ToString();
                    File.Copy(TempFolder + AsyncFileUpload1.FileName, OrginalFolder + AsyncFileUpload1.FileName);
                    DataSet Ds = new DataSet();
                    Ds = DbCall("WorkFlowInsertControls", xmlValues, OrginalFolder + AsyncFileUpload1.FileName);


                    if (Ds.Tables.Count > 0)
                    {
                        LoadControlListBox(Ds.Tables[0]);
                    }


                }
                Logger.Trace("AsyncFileUpload1_UploadedComplete Completed ", Session["LoggedUserId"].ToString());
            }

            catch (Exception ex)
            {
                divMsg.InnerHtml = ex.Message.ToString();
                Logger.Trace(ex.Message.ToString(), Session["LoggedUserId"].ToString());
            }

        }

        /// <summary>
        /// handling mapping betweeen the controls and stage fields
        /// </summary>
        /// <returns></returns>
        protected string AddMappingItems()
        {
            string XML = string.Empty;
            try
            {
                Logger.Trace("Generateing XML started ", Session["LoggedUserId"].ToString());

                DataTable dt = new DataTable();
                DataSet ds = new DataSet();
                dt.Columns.Add("ControliId", typeof(string)); //adding columns
                dt.Columns.Add("FieldsiId", typeof(string));
                dt.Rows.Add(lstPdfControls.SelectedValue.ToString(), lststagefields.SelectedValue.ToString());
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
        /// <summary>
        /// general methord for DB call
        /// </summary>
        /// <param name="Action"></param>
        /// <param name="XML"></param>
        /// <param name="Filepath"></param>
        /// <param name="MappingId"></param>
        /// <returns></returns>

        protected DataSet DbCall(string Action, string XML, string Filepath, int MappingId = 0)
        {
            DataSet Ds = new DataSet();
            try
            {


                objWorkflowPdfTemplateBLL.ProcessId = Convert.ToInt32(hdnProcessId.Value);
                objWorkflowPdfTemplateBLL.WorkflowId = Convert.ToInt32(hdnWorkflowId.Value);
                objWorkflowPdfTemplateBLL.LoginToken = hdnLoginToken.Value;
                objWorkflowPdfTemplateBLL.LoginOrgId = Convert.ToInt32(hdnLoginOrgId.Value);
                objWorkflowPdfTemplateBLL.xmlValues = XML;
                objWorkflowPdfTemplateBLL.MappingId = MappingId;
                objWorkflowPdfTemplateBLL.Path = Filepath;
                DBResult ObjstageFieldDetails = objWorkflowPdfTemplateBLL.ManageWorkflowPdfTemplate(objWorkflowPdfTemplateBLL, Action);
                Message = ObjstageFieldDetails.Message;
                Ds = ObjstageFieldDetails.dsResult;


            }
            catch (Exception ex)
            {
                divMsg.InnerHtml = ex.Message.ToString();
            }

            return Ds;

        }

        /// <summary>
        /// loading listbox of mapping 
        /// </summary>
        /// <param name="DT"></param>
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

        /// <summary>
        /// manage mapping 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnmap_Click(object sender, EventArgs e)
        {

            try
            {
                Logger.Trace("Inserting mapping started", Session["LoggedUserId"].ToString());

                string xmlValues = AddMappingItems();
                DataSet dsResult = new DataSet();
                dsResult = DbCall("InsertTemplateMapping", xmlValues, "");

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

        /// <summary>
        /// handlig commit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btncommit_Click(object sender, EventArgs e)
        {
            try
            {
                Logger.Trace("Commiting mapping started", Session["LoggedUserId"].ToString());

                DataSet dsResult = new DataSet();
                dsResult = DbCall("CommitTemplateMapping", "", "");

                if (dsResult.Tables.Count > 0)
                {
                    divMsg.InnerHtml = Message;
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

        /// <summary>
        ///handling remove mapping  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnremove_Click(object sender, EventArgs e)
        {
            try
            {
                Logger.Trace("Deleteing mapping started", Session["LoggedUserId"].ToString());

                DataSet dsResult = new DataSet();
                dsResult = DbCall("DeleteTemplateMapping", "", "", Convert.ToInt32(lstMappedcontrolfields.SelectedValue));

                if (dsResult.Tables.Count > 0)
                {
                    divMsg.InnerHtml = Message;
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

        /// <summary>
        /// purposeful post back to fill dropdowns 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCallFromJavascript_Click(object sender, EventArgs e)
        {
            try
            {
                Logger.Trace("Getting Workflow Controls", Session["LoggedUserId"].ToString());

                DataSet Ds = new DataSet();
                Ds = DbCall("GetAllWorkFlowControls", "", "");


                if (Ds.Tables.Count > 0)
                {

                    LoadControlListBox(Ds.Tables[0]);
                }
            }
            catch (Exception ex)
            {

                divMsg.InnerHtml = ex.Message.ToString();
                Logger.Trace(ex.Message.ToString(), Session["LoggedUserId"].ToString());
            }


        }

        /// <summary>
        /// going back to workflow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnGotoWorkflow_Click(object sender, EventArgs e)
        {

            string ProcessId = HttpUtility.UrlEncode(Encrypt(hdnProcessId.Value)); //ProcessId
            string QueryString = "?ProcessId=" + Server.UrlEncode(ProcessId);
            Response.Redirect("~/Workflow/WorkflowAdmin/ManageWorkflow.aspx" + QueryString);


        }
    }

    
}