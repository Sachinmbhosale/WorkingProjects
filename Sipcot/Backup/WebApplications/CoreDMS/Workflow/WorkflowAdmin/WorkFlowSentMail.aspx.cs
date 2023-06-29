/**============================================================================  
Author     : Gokuldas.Palapatta
Create date: 08-26-2015
===============================================================================  
** Change History   
** Date:        Author:    Issue ID    	Description: 
**(dd MMM yyyy)
** -----------------------------------------------------------------------------
** 
**===============================================================================*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WorkflowBLL.Classes;
using Lotex.EnterpriseSolutions.CoreBE;
using System.Data;
using WorkflowBAL;
using System.IO;
using OfficeConverter;
using Lotex.EnterpriseSolutions.CoreBL;

namespace Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin
{
    public partial class WorkFlowSentMail1 : PageBase
    {
        public string pageRights = string.Empty; /* DMS5-3946 A */


        WorkFlowSentMailBLL objWorkflowSentMailBLL = new WorkFlowSentMailBLL();

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                Logger.Trace("Strated Page_Load", Session["LoggedUserId"].ToString());
                CheckAuthentication();
                pageRights = GetPageRights("CONFIGURATION");/* DMS5-3946 A,DMS5-4122M added parameter configuration */
                ApplyPageRights(pageRights, this.Form.Controls); /* DMS5-3946 A */

                UserBase loginUser = (UserBase)Session["LoggedUser"];
                hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
                hdnLoginToken.Value = loginUser.LoginToken;
                FillAvailableUsers();
                GetTemplateDetails();
                Logger.Trace("Strated Page_Load", Session["LoggedUserId"].ToString());
            }
            WFPDFViewer.AnnotationToolbarVisible = false;
        }

        #region SentMailPreparation
        /// <summary>
        /// to fill available users in checklistbox
        /// </summary>
        protected void FillAvailableUsers()
        {
            Logger.Trace("Strated FillAvailableUsers", Session["LoggedUserId"].ToString());
            DataSet Ds = DBcall("GetAvailabeUsers");
            if (Ds.Tables.Count > 0)
            {
                chklstLoadUsers.DataTextField = "VALUE";
                chklstLoadUsers.DataValueField = "ID";
                chklstLoadUsers.DataSource = Ds.Tables[0];
                chklstLoadUsers.DataBind();
            }
            Logger.Trace("Finished FillAvailableUsers", Session["LoggedUserId"].ToString());
        }

        /// <summary>
        ///sub methord to take mailid from user 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        protected string GetMaidFromUserID(string userId)
        {
            Logger.Trace("Strated GetMaidFromUserID", Session["LoggedUserId"].ToString());
            string Mailid = string.Empty;
            DataSet Ds = DBcall("GetAvailabeUsers");


            if (Ds.Tables.Count > 0)
            {
                if (Ds.Tables[0].Rows.Count > 0)
                {
                    var rows = Ds.Tables[0].Select("ID = '" + userId + "'");
                    foreach (var row in rows)
                    {
                        Mailid = row["MailId"].ToString();
                    }
                }
            }
            Logger.Trace("Finished GetMaidFromUserID", Session["LoggedUserId"].ToString());
            return Mailid;

        }

        /// <summary>
        /// main methord to pull mail id
        /// </summary>
        /// <returns></returns>
        protected string ReturnmailIds()
        {
            Logger.Trace("Strated ReturnmailIds", Session["LoggedUserId"].ToString());
            string mailid = string.Empty;

            foreach (ListItem item in this.chklstLoadUsers.Items)
            {
                if (item.Selected)
                {
                    if (mailid.Length > 0)
                    {
                        mailid += "," + GetMaidFromUserID(item.Value);
                    }
                    else
                    {
                        mailid += GetMaidFromUserID(item.Value);
                    }
                }
            }
            Logger.Trace("Finished ReturnmailIds", Session["LoggedUserId"].ToString());

            return mailid;


        }

        /// <summary>
        /// handling sentmail functonality
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSendmail_Click(object sender, EventArgs e)
        {
            Logger.Trace("Strated btnSendmail_Click", Session["LoggedUserId"].ToString());
            string From = string.Empty;
            string To = string.Empty;
            string Subject = string.Empty;
            string Message = string.Empty;
            string Filepath = string.Empty;
            UserBase loginUser = (UserBase)Session["LoggedUser"];

            From = GetMaidFromUserID(loginUser.UserId.ToString());
            To = txtMailto.Text.Trim();
            Subject = txtsubject.Text;
            Message = txtmessage.Text;
            Filepath = hdnfilepath.Value;


            bool IsSuccess = MailHelper.SendMail(From, To, "", "", Subject, Message, Filepath);
            if (IsSuccess)
            {
                divMsg.InnerHtml = "Mail sent successfully";
                LoadImageToViewer(string.Empty);
            }
            else
            {
                divMsg.InnerHtml = "Kindly verify the mail Id(s) and resend again";

            }

            Logger.Trace("Finished btnSendmail_Click", Session["LoggedUserId"].ToString());

        }

        #endregion  SentMailPreparation

        /// <summary>
        /// handling check list box click event and fill the mail id in the text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void chklstLoadUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            Logger.Trace("Strated chklstLoadUsers_SelectedIndexChanged", Session["LoggedUserId"].ToString());
            txtMailto.Text = ReturnmailIds();
            LoadImageToViewer(string.Empty);
            Logger.Trace("Finished chklstLoadUsers_SelectedIndexChanged", Session["LoggedUserId"].ToString());

        }

        /// <summary>
        /// general DB call
        /// </summary>
        /// <param name="Action"></param>
        /// <returns></returns>
        protected DataSet DBcall(string Action)
        {
            Logger.Trace("Strated DBcall", Session["LoggedUserId"].ToString());
            DataSet Ds = new DataSet();
            objWorkflowSentMailBLL.ProcessId = Convert.ToInt32(Session["ProcessId"]);
            objWorkflowSentMailBLL.WorkflowId = Convert.ToInt32(Session["WorkflowId"]);
            objWorkflowSentMailBLL.LoginToken = hdnLoginToken.Value;
            objWorkflowSentMailBLL.LoginOrgId = Convert.ToInt32(hdnLoginOrgId.Value);
            objWorkflowSentMailBLL.DataId = Convert.ToInt32(Session["FieldDataId"]);

            DBResult ObjstageFieldDetails = objWorkflowSentMailBLL.ManageWrokFlowSentMail(objWorkflowSentMailBLL, Action);

            Ds = ObjstageFieldDetails.dsResult;

            Logger.Trace("Finished DBcall", Session["LoggedUserId"].ToString());
            return Ds;
        }




        #region Document View


        /// <summary>
        /// getting the fieldvale if it is a dropdown
        /// </summary>
        /// <param name="fieldId"></param>
        /// <returns></returns>
        protected string getfieldvalue(string fieldId)
        {
            string Value = string.Empty;
            Logger.Trace("Strated getfieldvalue", Session["LoggedUserId"].ToString());
            try
            {
                int Fieldvalue = Convert.ToInt32(fieldId);
                DataSet Ds = new DataSet();
                objWorkflowSentMailBLL.ProcessId = Convert.ToInt32(Session["ProcessId"]);
                objWorkflowSentMailBLL.WorkflowId = Convert.ToInt32(Session["WorkflowId"]);
                objWorkflowSentMailBLL.LoginToken = hdnLoginToken.Value;
                objWorkflowSentMailBLL.LoginOrgId = Convert.ToInt32(hdnLoginOrgId.Value);
                objWorkflowSentMailBLL.DataId = Convert.ToInt32(Session["FieldDataId"]);
                objWorkflowSentMailBLL.Filedvalue = Fieldvalue;
                DBResult ObjstageFieldDetails = objWorkflowSentMailBLL.ManageWrokFlowSentMail(objWorkflowSentMailBLL, "GetFieldValue");

                if (ObjstageFieldDetails.dsResult.Tables.Count > 0)
                {
                    if (ObjstageFieldDetails.dsResult.Tables[0].Rows.Count > 0)
                    {
                        Value = ObjstageFieldDetails.dsResult.Tables[0].Rows[0]["VALUE"].ToString();
                    }

                }
                Logger.Trace("Finished getfieldvalue", Session["LoggedUserId"].ToString());
            }
            catch(Exception ex)
            {
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }


            return Value;
        }

        /// <summary>
        /// getting field value by matching  headers
        /// </summary>
        /// <param name="template"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        protected DataTable getFieldvaules(DataTable template, DataTable Data)
        {
            Logger.Trace("Strated getFieldvaules", Session["LoggedUserId"].ToString());
            string DataHeader = string.Empty;
            string Value = string.Empty;
            string fieldtype = string.Empty;
            DataTable tdt = new DataTable();
            tdt.Columns.Add("Controlname", typeof(string)); //adding columns
            tdt.Columns.Add("FieldName", typeof(string));
            tdt.Columns.Add("ControlValue", typeof(string));
            tdt.Columns.Add("ControlType", typeof(string));
            for (int i = 0; i < template.Rows.Count; i++)
            {
                DataHeader = template.Rows[i]["FieldName"].ToString();
                fieldtype = template.Rows[i]["FieldControlType"].ToString();
                Value = Data.Rows[0][DataHeader].ToString();
                if (fieldtype == "MultiDropDown" || fieldtype == "DropDown")
                {
                    Value = getfieldvalue(Value);
                }


                tdt.Rows.Add(template.Rows[i]["ControlName"].ToString(), template.Rows[i]["FieldName"].ToString(), Value, template.Rows[i]["ControlType"].ToString());

            }
            Logger.Trace("Finished getFieldvaules", Session["LoggedUserId"].ToString());
            return tdt;
        }

        /// <summary>
        /// applying the values to the controls
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="Dt"></param>
        protected void ApplyFieldsOnPDF(string FilePath, DataTable Dt)
        {
            Logger.Trace("Strated ApplyFieldsOnPDF", Session["LoggedUserId"].ToString());

            string key = Guid.NewGuid().ToString();
            string TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"] + hdnLoginToken.Value + "\\" + key + "\\";
            txtsubject.Text = Path.GetFileName(FilePath);

            if (!Directory.Exists(TempFolder))
            {
                Directory.CreateDirectory(TempFolder);
            }
            string TempFilePath = TempFolder + Path.GetFileName(FilePath);
            File.Copy(FilePath, TempFilePath, true);
            string NewFilePath = TempFolder + beforedot(Path.GetFileName(FilePath)) + "_New.pdf";
            if (File.Exists(NewFilePath))
            {
                File.Delete(NewFilePath);
            }

            new GettingControlsFromPDF().FillControlValues(TempFilePath, Dt, NewFilePath);
            hdnfilepath.Value = NewFilePath;
            LoadImageToViewer(string.Empty);

            Logger.Trace("Finished ApplyFieldsOnPDF", Session["LoggedUserId"].ToString());
        }

        /// <summary>
        /// converting pdf to jpg
        /// </summary>
        /// <param name="Filepath"></param>
        /// <param name="EncryptedFilepath"></param>
        /// <param name="RequestingPageNo"></param>
        /// <returns></returns>
        private string convertPDFToImage(string Filepath, string EncryptedFilepath, int RequestingPageNo)
        {
            string CurrentFilepath = string.Empty, filepath = string.Empty;
            Logger.Trace("Strated convertPDFToImage", Session["LoggedUserId"].ToString());
            try
            {
                string TempFolder = string.Empty;
                ConvertPdf2Image obj = new ConvertPdf2Image();

                string key = Guid.NewGuid().ToString();
                TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"] + hdnLoginToken.Value + "\\" + key + "\\";
                TempFolder += EncryptedFilepath + "JPG";


                //CurrentFilepath = obj.GetJpegPageFromPdf(Filepath, TempFolder, RequestingPageNo.ToString());

                // Get watermark for the document from session

                Logger.Trace("Converting PDF to JPEG without applying watermark.", Session["LoggedUserId"].ToString());
                // Convert pdf to JPEG 
                CurrentFilepath = obj.ConvertPDFtoHojas(Filepath, TempFolder, RequestingPageNo.ToString());


            }
            catch (Exception ex)
            {
                Logger.Trace("Exception: " + ex.Message, Session["LoggedUserId"].ToString());

            }
            Logger.Trace("Finished convertPDFToImage", Session["LoggedUserId"].ToString());
            return CurrentFilepath;
        }

        /// <summary>
        /// loading image to the viewer
        /// </summary>
        /// <param name="Action"></param>
        private void LoadImageToViewer(string Action)
        {
            Logger.Trace("Strated LoadImageToViewer", Session["LoggedUserId"].ToString());
            string Filepath = hdnfilepath.Value;
            int pagecount = Convert.ToInt32(Session["Pagecount"]);
            string src = string.Empty;

            int CurrentPage = 1;





            int PageNo = 0;

            switch (Action.ToUpper())
            {
                case "NEXT":
                    PageNo = CurrentPage + 1;
                    break;
                case "PREVIOUS":
                    PageNo = CurrentPage - 1;
                    break;

                default:
                    PageNo = CurrentPage;
                    break;
            }




            string PdfPath = Filepath;
            string ImageName = beforedot(Path.GetFileName(Filepath)).ToString();

            Logger.Trace("Converting PDF to JPEG. PDF Path: " + PdfPath + " Image Name: " + ImageName + " Page No: " + PageNo, Session["LoggedUserId"].ToString());
            string CurrentFilepath = convertPDFToImage(PdfPath, ImageName, PageNo);

            if (CurrentFilepath.Length > 0)
            {
                src = GetSrc("Handler") + CurrentFilepath;
                src = src.Replace("\\", "//").ToLower();

                Logger.Trace("Setting image to viewer using RegisterStartupScript: " + src, Session["LoggedUserId"].ToString());
                ScriptManager.RegisterStartupScript(this, this.GetType(), "loadImageAndAnnotations", "loadImageAndAnnotations('" + src + "');", true);

                //PDFViewer1.Load += new EventHandler(PDFViewer1.JustPostbakUserControl);
            }
            else
            {
                Logger.Trace("convertPDFToImage method returned file path as empty.", Session["LoggedUserId"].ToString());
            }
            Logger.Trace("Finished LoadImageToViewer", Session["LoggedUserId"].ToString());
        }

        /// <summary>
        /// getting the pdf tempate details
        /// </summary>
        protected void GetTemplateDetails()
        {
            try
            {

                Logger.Trace("Strated GetTemplateDetails", Session["LoggedUserId"].ToString());

                DataSet Ds = DBcall("GetTemplateDetails");
                if (Ds.Tables.Count > 0)
                {
                    DataSet DataDs = DBcall("GetTemplateDataDetails");

                    if (DataDs.Tables[0].Rows.Count > 0 && Ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable Dt = new DataTable();
                        Dt = getFieldvaules(Ds.Tables[0], DataDs.Tables[0]);
                        DataDs = new DataSet();
                        DataDs = DBcall("GetWrorkFlowFilePath");
                        if (DataDs.Tables.Count > 0)
                        {
                            if (DataDs.Tables[0].Rows.Count > 0)
                            {

                                ApplyFieldsOnPDF(DataDs.Tables[0].Rows[0]["Path"].ToString(), Dt);
                            }
                        }
                    }
                }
                else
                {
                    divMsg.InnerHtml = "No pdf template associated with the workflow";
                }
                Logger.Trace("Finished GetTemplateDetails", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());

            }
        }

        /// <summary>
        /// to handle postback for viewer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCallFromJavascript_Click(object sender, EventArgs e)
        {
            LoadImageToViewer(hdnAction.Value);
        }

        #endregion  Document View






    }
}