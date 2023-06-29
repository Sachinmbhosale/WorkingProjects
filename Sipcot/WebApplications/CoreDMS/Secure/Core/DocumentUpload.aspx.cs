/* ============================================================================  
Author     : 
Create date: 
===============================================================================  
** Change History   
** Date:            Author:             Issue ID            Description:  
** ----------------------------------------------------------------------------
** 03 Dec 2013      Pratheesh A         Mandatory           Set Mandatory for index fields
 * 22 Mar 2015      Yogeesha naik       DMS04-3520          Ms. Office uploads not working. 
 * 04 Apr 2015      Sabina.K.V          DMS5-4101           If index field name contains a space then calendar/multi select will not work
 * 22-07-2015       Gokuldas.Palapatta  DMSENH6-4637	    Document upload & slicing
=============================================================================== */
using Lotex.EnterpriseSolutions.CoreBL;
using System.Web.UI.WebControls;
using System.Data;
using System;
using System.Collections.Generic;
using Lotex.EnterpriseSolutions.CoreBE;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Text;
using AjaxControlToolkit;
using OfficeConverter;
using Lotex.IFilter;
using System.Xml.Linq;
using System.Configuration;

namespace Lotex.EnterpriseSolutions.WebUI
{
    public partial class DocumentUpload : PageBase
    {
        public const string PageName = "DOCUMENT_UPLOAD";
        public List<string> restwo;
        public string AfterDot;
        public string TodayDate = DateTime.Today.ToString("yyyy-MM-dd");
        public string TempFolder;

        int PageCount = 0;
        List<string> keyvalues;
        int key = 0;
        protected bool bDynamicControlIndexChange = false;
        string cadpreview = string.Empty;
        protected int NumberOfControls
        {
            get { return (int)ViewState["NumControls"]; }
            set { ViewState["NumControls"] = value; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            PDFViewer1.Visible = false;
            PDFViewer1.AnnotationToolbarVisible = false;

            CheckAuthentication();
            Disabled();
            bDynamicControlIndexChange = hdnDynamicControlIndexChange.Value == "1" ? true : false;
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
            hdnLoginToken.Value = loginUser.LoginToken;

            ddlpagecount.Attributes.Add("onChange", "javascript:return navigationHandler('GOTO');");
            btnSave.Attributes.Add("onclick", "javascript:return validate()");

            if (!Page.IsPostBack)
            {
                NavigatePanel.Visible = false;

                hdnUploaded.Value = "FALSE";
                if (Convert.ToString(Request.QueryString["id"]) != null && Convert.ToString(Request.QueryString["docid"]) != null && Convert.ToString(Request.QueryString["depid"]) != null)
                {
                    if (Request.QueryString["showmsg"] != null && Request.QueryString["showmsg"] == "true")
                    {
                        ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('Document Uploaded Successfully.Please process next record.');", true);
                    }

                    EditUploadedFile();

                }
                else if (Convert.ToString(Request.QueryString["action"]) == "upload" || Convert.ToString(Request.QueryString["action"]) == null)
                {
                    Session["TempFileLocation"] = "";
                    Session["Ext"] = "";
                    Session["TempFileLocation"] = "";
                    if (Convert.ToString(Request.QueryString["action"]) == "upload")
                    {
                        Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Alert", "alert('Document Uploaded Successfully.')", true);
                    }

                    GetDocumentType(loginUser.LoginOrgId.ToString(), loginUser.LoginToken);
                    this.NumberOfControls = 0;
                    pnlIndexpro.Controls.Clear();
                    txtRefid.Text = "";
                    txtKeyword.Text = "";
                }
                else if (Convert.ToString(Request.QueryString["action"]) == "uploadFail")
                {
                    Session["TempFileLocation"] = "";
                    Session["Ext"] = "";
                    Session["TempFileLocation"] = "";
                    if (Convert.ToString(Request.QueryString["action"]) == "uploadFail")
                    {
                        Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Alert", "alert('Please select valid PDF file !.')", true);
                    }

                    GetDocumentType(loginUser.LoginOrgId.ToString(), loginUser.LoginToken);
                    this.NumberOfControls = 0;
                    pnlIndexpro.Controls.Clear();
                    txtRefid.Text = "";
                    txtKeyword.Text = "";
                }
                else if (Convert.ToString(Request.QueryString["action"]) == "SpecialCharactors")
                {
                    Session["TempFileLocation"] = "";
                    Session["Ext"] = "";
                    Session["TempFileLocation"] = "";
                    if (Convert.ToString(Request.QueryString["action"]) == "SpecialCharactors")
                    {
                        Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Alert", "alert('Special characters not allowed, Please enter valid characters..!')", true);
                    }

                    GetDocumentType(loginUser.LoginOrgId.ToString(), loginUser.LoginToken);
                    this.NumberOfControls = 0;
                    pnlIndexpro.Controls.Clear();
                    txtRefid.Text = "";
                    txtKeyword.Text = "";
                }


            }
            else
            {
                if (cmbDepartment.SelectedValue != "0" && cmbDocumentType.SelectedValue != "0")
                {
                    this.createControls();
                }
            }

            btnPreview.Enabled = false;

        }

        public void Disabled()
        {
            Button10.Visible = false;
            Button9.Visible = false;
            Button8.Visible = false;
            Button7.Visible = false;
        }

        public void GetDepartments(string loginOrgId, string loginToken)
        {

            DepartmentBL bl = new DepartmentBL();
            SearchFilter filter = new SearchFilter();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            filter.DocumentTypeID = Convert.ToInt32(cmbDocumentType.SelectedValue);
            try
            {
                string action = "DepartmentsForUpload";
                Results res = bl.SearchDepartments(filter, action, loginOrgId, loginToken);
                if (res.ActionStatus == "SUCCESS" && res.Departments != null)
                {
                    cmbDepartment.Items.Clear();
                    cmbDepartment.Items.Add(new ListItem("<Select>", "0"));
                    foreach (Department dp in res.Departments)
                    {
                        cmbDepartment.Items.Add(new ListItem(dp.DepartmentName, dp.DepartmentId.ToString()));
                    }
                }
                else
                {
                    cmbDepartment.Items.Clear();
                    cmbDepartment.Items.Add(new ListItem("<Select>", "0"));
                }
            }
            catch (Exception ex)
            {
                divMsg.Style.Add("display", "block");
                divMsg.InnerHtml = CoreMessages.GetMessages(hdnAction.Value, "ERROR");
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }
        public void GetDocumentType(string loginOrgId, string loginToken)
        {
            DocumentTypeBL bl = new DocumentTypeBL();
            SearchFilter filter = new SearchFilter();
            try
            {
                string action = "DocumentTypeForUpload";
                Results res = bl.SearchDocumentTypes(filter, action, loginOrgId, loginToken);

                if (res.ActionStatus == "SUCCESS" && res.DocumentTypes != null)
                {
                    cmbDocumentType.Items.Clear();
                    cmbDocumentType.Items.Add(new ListItem("<Select>", "0"));
                    foreach (DocumentType dp in res.DocumentTypes)
                    {
                        cmbDocumentType.Items.Add(new ListItem(dp.DocumentTypeName, dp.DocumentTypeId.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                divMsg.Style.Add("display", "block");
                divMsg.InnerHtml = CoreMessages.GetMessages(hdnAction.Value, "ERROR");
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        private void createControls()
        {
            Logger.Trace("createControls Started", Session["LoggedUserId"].ToString());
            string indexname = string.Empty;
            string sortOreder = string.Empty;
            bool IActivive;

            HtmlTable Tab = new HtmlTable();

            UserBase loginUser = (UserBase)Session["LoggedUser"];
            SearchFilter objFilter = new SearchFilter();

            objFilter.CurrOrgId = loginUser.LoginOrgId;
            objFilter.CurrUserId = loginUser.UserId;
            objFilter.DocumentTypeName = cmbDocumentType.SelectedItem.ToString();
            objFilter.DepartmentID = Convert.ToInt32(cmbDepartment.SelectedValue);
            DataSet TemplateDs = new TemplateBL().GetTemplateDetails(objFilter);
            if (cmbDepartment.SelectedValue == "0" || cmbDocumentType.SelectedValue == "0")
            {
                pnlIndexpro.Controls.Clear();
                this.NumberOfControls = 0;
                txtRefid.Text = "";
                hdnCountControls.Value = "";
                hdnIndexMinLen.Value = "";
                hdnIndexNames.Value = "";
                hdnMandatory.Value = "";
            }
            else
            {
                int ControlsCounter = -1; // if the control counter is even bind controls parallelly, i.e., 4 columns :[ lable:control label:control ] other wise 2 columns style
                HtmlTableRow TR = new HtmlTableRow();
                for (int i = 0; i < TemplateDs.Tables[0].Rows.Count; i++)
                {
                    IActivive = Convert.ToBoolean(TemplateDs.Tables[0].Rows[i]["TemplateFields_bActive"]);
                    ++ControlsCounter; // Comment this line to bind controls in 2 coulumns stlye
                    //int counts = control.Control.Count();
                    if (Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iParentId"]) == 0)
                        TR = new HtmlTableRow();
                    HtmlTableCell TD1 = new HtmlTableCell();
                    HtmlTableCell TD2 = new HtmlTableCell();

                    TD1.Width = "150px";
                    TD2.Width = "150px";
                    // Parellel controls binding
                    HtmlTableCell TD3 = new HtmlTableCell();
                    HtmlTableCell TD4 = new HtmlTableCell();

                    TD3.Width = "150px";
                    TD4.Width = "150px";

                    // Add label text
                    if (!string.Equals(TemplateDs.Tables[0].Rows[i]["TemplateFields_cObjType"].ToString(), "Label", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(TemplateDs.Tables[0].Rows[i]["TemplateFields_cObjType"].ToString(), "Button", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(TemplateDs.Tables[0].Rows[i]["TemplateFields_cObjType"].ToString(), "HiddenField", StringComparison.OrdinalIgnoreCase) && Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iParentId"]) == 0)
                    {
                        Label objCnl = new Label();
                        objCnl.ID = "lbl" + i;
                        objCnl.CssClass = Getcssclass("lbl");
                        objCnl.Text = TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString();
                        objCnl.AssociatedControlID = "";
                        if (Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iParentId"]) == 0)
                        {
                            TD1.Controls.Add(objCnl);
                        }
                        else
                            TD3.Controls.Add(objCnl);
                    }
                    #region textbox control
                    if (string.Equals(TemplateDs.Tables[0].Rows[i]["TemplateFields_cObjType"].ToString(), "TextBox", StringComparison.OrdinalIgnoreCase))
                    {
                        TextBox objCnltxt = new TextBox();
                        // objCnltxt.AutoPostBack = Convert.ToBoolean(TemplateDs.Tables[0].Rows[i]["TemplateFields_bPostback"]);
                        //DMS5-4101M replaced the space with empty string
                        objCnltxt.ID = TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString().Replace(" ", string.Empty);
                        objCnltxt.CssClass = Getcssclass("txt");
                        TD2.Controls.Add(objCnltxt);
                        objCnltxt.Width = Unit.Pixel(157);
                        objCnltxt.Height = Unit.Pixel(16);

                        if (TemplateDs.Tables[0].Rows[i]["TemplateFields_vDBType"].ToString() == "Integer")
                        {
                            objCnltxt.MaxLength = Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iMax"].ToString());
                            objCnltxt.Attributes.Add("OnKeyPress", "javascript:return CheckNumericKey(event);");
                        }
                        
                        else if (TemplateDs.Tables[0].Rows[i]["TemplateFields_vDBType"].ToString() == "Boolen")
                        {
                            objCnltxt.MaxLength = 1;
                            objCnltxt.Attributes.Add("OnKeyPress", "javascript:return CheckBoolean(event);");
                        }
                        else if (TemplateDs.Tables[0].Rows[i]["TemplateFields_vDBType"].ToString() == "DateTime")
                        {
                            string t2 = (TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString()).ToLower();
                            if (!(t2.Contains("month")))
                            {
                                objCnltxt.MaxLength = 10;
                                objCnltxt.Attributes.Add("OnKeyPress", "javascript:return CheckDateFormat(event.keyCode,'" + TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString() + "');");

                                if (bDynamicControlIndexChange)
                                {
                                    CalendarExtender calext = new CalendarExtender();
                                    //DMS5-4101M replaced the space with empty string
                                    calext.ID = "cal_" + TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString().Replace(" ", string.Empty);
                                    calext.TargetControlID = TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString().Replace(" ", string.Empty);
                                    calext.Format = "dd/MM/yyyy";
                                    TD2.Controls.Add(calext);
                                }
                            }
                            else
                            {
                                objCnltxt.MaxLength = 10;
                                if (bDynamicControlIndexChange)
                                {
                                    CalendarExtender calext = new CalendarExtender();
                                    //DMS5-4101M replaced the space with empty string
                                    calext.ID = "cal_" + TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString().Replace(" ", string.Empty);
                                    calext.TargetControlID = TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString().Replace(" ", string.Empty);
                                    calext.Format = "MM/yyyy";
                                    TD2.Controls.Add(calext);
                                }
                            }


                        }
                        else
                        {
                            if (Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iMax"].ToString()) != 0)
                            {
                                objCnltxt.MaxLength = Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iMax"].ToString());
                            }
                            else
                            {
                                objCnltxt.MaxLength = 30;
                            }
                        }
                        Logger.Trace("Create controls,Load all Textbox dynamic", Session["LoggedUserId"].ToString());

                    }
                    #endregion

                    #region dropdownlist control
                    if (string.Equals(TemplateDs.Tables[0].Rows[i]["TemplateFields_cObjType"].ToString(), "DropDownList", StringComparison.OrdinalIgnoreCase))
                    {
                        DropDownList objCnl = new DropDownList();
                        objCnl.AutoPostBack = Convert.ToBoolean(TemplateDs.Tables[0].Rows[i]["TemplateFields_bPostback"]);
                        //DMS5-4101M replaced the space with empty string
                        objCnl.ID = TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString().Replace(" ", string.Empty);
                        objCnl.CssClass = Getcssclass("drp");
                        objCnl.Height = Unit.Pixel(30);
                        objCnl.Width = Unit.Pixel(165);
                        int TemplatefldId = Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iId"]);
                        string Iaction = string.Empty;
                        if (TemplateDs.Tables[0].Rows[i]["TemplateFields_iParentId"].ToString() == "0")
                        {
                            Iaction = "LoadMain";

                            PopulateDropdown(objCnl, TemplatefldId, Iaction);//function to populate dropdownlist
                        }
                        else
                        {
                            objCnl.Items.Insert(0, "--Select--");
                        }
                        objCnl.Attributes.Add("onChange", "javascript:SetHiddenVal('Dynamic');");

                        if (objCnl.AutoPostBack)
                        {
                            objCnl.SelectedIndexChanged += new EventHandler(DynamicDropdownList_SelectedIndexChanged);
                        }




                        if (Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iParentId"]) == 0)
                        {
                            TD2.Controls.Add(objCnl);
                        }
                        else
                        {
                            TD4.Controls.Add(objCnl);
                        }
                        Logger.Trace("Createcontrols,Load all Dropdownlist dynamic", Session["LoggedUserId"].ToString());
                    }
                    #endregion
                    if (Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iParentId"]) == 0)
                    {
                        TR.Cells.Add(TD1);
                        TR.Cells.Add(TD2);
                    }

                    else
                    {
                        TR.Cells.Add(TD3);
                        TR.Cells.Add(TD4);
                    }

                    Tab.Rows.Add(TR);
                    Tab.Attributes.Add("CssClass", "tabledesign");
                }
                pnlIndexpro.Controls.Add(Tab);

            }


            Logger.Trace("createControls Finished", Session["LoggedUserId"].ToString());
        }
        /// <summary>
        /// Populate dynamic dropdownlist
        /// </summary>
        /// <param name="drp"></param>
        /// <param name="TemplateId"></param>
        public void PopulateDropdown(DropDownList drp, int TemplateId, string Iaction)
        {
            DynamicControlBL objDynamicControlBL = new DynamicControlBL();
            DataSet ds = new DataSet();
            try
            {
                ds = objDynamicControlBL.DynamicPopulateDropdown(TemplateId, Iaction);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0] != null)
                    {
                        drp.DataSource = ds;
                        drp.DataTextField = "DataTextField";
                        drp.DataValueField = "DataValueField";
                        drp.DataBind();
                    }

                }
            }
            catch (Exception ex)
            {

                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        public void DynamicDropdownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            bDynamicControlIndexChange = true;
            DynamicControlBL objDynamicControlBL = new DynamicControlBL();
            int TemplateFldId = 0;
            DataSet ds = new DataSet();
            try
            {
                DropDownList ddl = (DropDownList)sender;
                DropDownList ddlBind = (DropDownList)pnlIndexpro.FindControl(ddl.ID + "_sub");//Get the sub dropdown id from panel.
                TemplateFldId = Convert.ToInt32(ddl.SelectedValue);
                ds = objDynamicControlBL.DynamicLoadDropdownBasedOnValue(TemplateFldId);

                if (ddlBind != null)
                {
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0] != null)
                        {
                            ddlBind.DataSource = ds;
                            ddlBind.DataTextField = "DataTextField";
                            ddlBind.DataValueField = "DataValueField";
                            ddlBind.DataBind();
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }



        public bool IsPDFHeader(string fileName)
        {
            byte[] buffer = null;
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            long numBytes = new FileInfo(fileName).Length;
            //buffer = br.ReadBytes((int)numBytes);
            buffer = br.ReadBytes(5);
            fs.Close();

            HidMisFile.Value = "False";
            var enc = new ASCIIEncoding();
            var header = enc.GetString(buffer);

            //%PDF−1.0
            // If you are loading it into a long, this is (0x04034b50).
            if (buffer[0] == 0x25 && buffer[1] == 0x50
                && buffer[2] == 0x44 && buffer[3] == 0x46)
            {
                return header.StartsWith("%PDF-");

            }
            else
            {
                File.Delete(fileName);

            }
            return false;

        }

        public bool checkPDFHeader(string fileName)
        {
            byte[] buffer = null;
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            long numBytes = new FileInfo(fileName).Length;
            //buffer = br.ReadBytes((int)numBytes);
            buffer = br.ReadBytes(5);
            fs.Close();

            HidMisFile.Value = "False";
            var enc = new ASCIIEncoding();
            var header = enc.GetString(buffer);

            //%PDF−1.0
            // If you are loading it into a long, this is (0x04034b50).
            if (buffer[0] == 0x25 && buffer[1] == 0x50
                && buffer[2] == 0x44 && buffer[3] == 0x46)
            {
                return header.StartsWith("%PDF-");

            }
            return false;

        }
        //public string GetMimeType(string filePath)
        //{
        //    var provider = new FileExtensionContentTypeProvider();

        //    if (!provider.TryGetContentType(filePath, out var contentType))
        //        contentType = "application/octet-stream"; // fallback: unknown binary type

        //    return contentType;
        //}


        protected void AsyncFileUpload1_UploadedComplete(object sender, AjaxControlToolkit.AsyncFileUploadEventArgs e)
        {

            string type = String.Empty;
            string FileName = String.Empty;
            if (Convert.ToInt32(e.FileSize) > 2.4e+7) // use same condition in client side code
            {
                Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Alert", "alert('Maximum file size allowed 24 MB')", true);
                Logger.Trace("Exception : ", Session["LoggedUserId"].ToString());
                AsyncFileUpload1.Dispose();
                btnSave.Enabled = false;
                return;
            }

            try
            {


                cadpreview = "false";
                Logger.Trace("AsyncFileUpload1_UploadedComplete Started", Session["LoggedUserId"].ToString());

                if (hdnUploaded.Value == "FALSE")
                {

                    if (cmbDepartment.SelectedValue != "0" && cmbDocumentType.SelectedValue != "0")
                    {

                        Logger.Trace("Started Saving Uploaded Document in AsyncFileUpload1_UploadedComplete ", Session["LoggedUserId"].ToString());




                        TodayDate = DateTime.Today.ToString("yyyy-MM-dd");
                        string TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"];

                        if (!Directory.Exists(TempFolder))
                        {
                            Directory.CreateDirectory(TempFolder);
                        }

                        if (Convert.ToString(Session["EncyptImgName"]).Length > 0)
                        {
                            try
                            {
                                File.Delete(TempFolder + Convert.ToString(Session["EncyptImgName"]));
                                File.Delete(TempFolder + beforedot(Convert.ToString(Session["EncyptImgName"])) + ".pdf");
                            }
                            catch (Exception ex)
                            {
                                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
                            }
                        }

                        if (AsyncFileUpload1.HasFile)
                        {
                            AfterDot = afterdot(AsyncFileUpload1.FileName);
                            AsyncFileUpload1.SaveAs(TempFolder + Encryptdata(txtRefid.Text) + AfterDot);
                            Session["TempFolder"] = TempFolder;
                            Session["EncyptImgName"] = Encryptdata(txtRefid.Text) + AfterDot;
                            Session["FileName"] = AsyncFileUpload1.FileName;

                            FileName = TempFolder + beforedot(Convert.ToString(Session["EncyptImgName"])) + AfterDot;
                            Session["TempFilepath"] = FileName;

                        }

                        bool IsPDFheader = checkPDFHeader(FileName);
                        if (IsPDFheader == false)
                        {

                            Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Alert", "alert('Please select valid PDF file for upload!')", true);
                            return;
                        }




                        Logger.Trace("Finished Saving Uploaded Document in AsyncFileUpload1_UploadedComplete ", Session["LoggedUserId"].ToString());
                        // if the checkbox checked then will be allow slicing and preview then upload or splitting task will be handing over to windows service
                        if (System.Convert.ToBoolean(hdnPreviewCheckBoxChecked.Value) == true) /* DMSENH6-4637 A */
                        {
                            //  string result = "Success";
                            if (Session["EncyptImgName"] != null && afterdot(Session["EncyptImgName"].ToString()).Length > 0)
                            {
                                switch (afterdot(Session["EncyptImgName"].ToString()).ToLower())
                                {
                                    case ".doc":
                                    case ".docx":
                                        PageCount = new MSWord().Convert(Session["TempFolder"].ToString() + Encryptdata(txtRefid.Text) + afterdot(Session["EncyptImgName"].ToString()), Session["TempFolder"].ToString() + Encryptdata(txtRefid.Text) + ".pdf", true);
                                        Logger.Trace("AsyncFileUpload1_UploadedComplete Pagecount of MSWord " + PageCount, Session["LoggedUserId"].ToString());
                                        break;
                                    case ".dwg":
                                    case ".dxf":
                                        PageCount = 1;
                                        // Logger.Trace("AsyncFileUpload1_UploadedComplete Pagecount of MSWord " + PageCount, Session["LoggedUserId"].ToString());
                                        break;
                                    case ".ppt":
                                    case ".pptx":
                                        PageCount = new MSPowerPoint().Convert(Session["TempFolder"].ToString() + Encryptdata(txtRefid.Text) + afterdot(Session["EncyptImgName"].ToString()), Session["TempFolder"].ToString() + Encryptdata(txtRefid.Text) + ".pdf", true);
                                        Logger.Trace("AsyncFileUpload1_UploadedComplete Pagecount of MSPowerPoint " + PageCount, Session["LoggedUserId"].ToString());
                                        break;
                                    case ".xls":
                                    case ".xlsx":
                                        PageCount = new MSExcel().Convert(Session["TempFolder"].ToString() + Encryptdata(txtRefid.Text) + afterdot(Session["EncyptImgName"].ToString()), Session["TempFolder"].ToString() + Encryptdata(txtRefid.Text) + ".pdf", true);
                                        Logger.Trace("AsyncFileUpload1_UploadedComplete Pagecount of MSExcel " + PageCount, Session["LoggedUserId"].ToString());
                                        break;
                                    case ".tif":
                                    case ".tiff":


                                    case ".jpg":
                                    case ".bmp":
                                    case ".jpeg":
                                    case ".png":
                                    case ".gif":
                                    case ".giff":
                                        PageCount = new Image2Pdf().tiff2PDF(Session["TempFolder"].ToString() + Encryptdata(txtRefid.Text) + afterdot(Session["EncyptImgName"].ToString()), Session["TempFolder"].ToString() + Encryptdata(txtRefid.Text), true);
                                        Logger.Trace("AsyncFileUpload1_UploadedComplete Pagecount of Image2Pdf().tiff2PDF " + PageCount, Session["LoggedUserId"].ToString());
                                        break;
                                    case ".pdf":
                                        PageCount = new Image2Pdf().ExtractPages(Session["TempFolder"].ToString() + Encryptdata(txtRefid.Text) + afterdot(Session["EncyptImgName"].ToString()));
                                        Logger.Trace("AsyncFileUpload1_UploadedComplete Pagecount of Image2Pdf().ExtractPages " + PageCount, Session["LoggedUserId"].ToString());
                                        break;
                                    default:
                                        // result = "Failed";
                                        break;
                                }
                            }

                            hdnPageCount.Value = PageCount.ToString();
                            Session["PageCount"] = Convert.ToInt32(PageCount.ToString());
                        }
                        else
                        {

                            btnPreview.Enabled = false;/* DMSENH6-4637 A */
                                                       //disabling preview if preview chek box is not checked

                        }

                        //}
                        //else
                        //{

                        //    divMsg.Style.Add("display", "block");
                        //    divMsg.InnerText = "Please select valid PDF file for upload!";
                        //    ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('Please select valid PDF file for upload!');", true);
                        //    return;
                        //}

                    }
                    else
                    {
                        divMsg.Style.Add("display", "block");
                        divMsg.InnerText = "Please select both Project Type and Department!";
                        return;
                    }
                }

                Logger.Trace("AsyncFileUpload1_UploadedComplete Finished", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {

                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }

        }

        private void FindMimeFromData(int v1, object p1, byte[] document, int v2, object p2, int v3, out uint mimetype, int v4)
        {
            throw new NotImplementedException();
        }





        //protected void AsyncFileUpload1_UploadedComplete(object sender, AjaxControlToolkit.AsyncFileUploadEventArgs e)
        //{

        //    try
        //    {
        //        if (Convert.ToInt32(e.FileSize) > 2.4e+7) // use same condition in client side code
        //        {
        //            Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Alert", "alert('Maximum file size allowed 24 MB')", true);
        //            //Logger.Trace("Exception : ", Session["LoggedUserId"].ToString());
        //            AsyncFileUpload1.Dispose();
        //            btnSave.Enabled = false;
        //            return;
        //        }


        //        cadpreview = "false";
        //        Logger.Trace("AsyncFileUpload1_UploadedComplete Started", Session["LoggedUserId"].ToString());

        //        if (hdnUploaded.Value == "FALSE")
        //        {

        //            if (cmbDepartment.SelectedValue != "0" && cmbDocumentType.SelectedValue != "0")
        //            {

        //                Logger.Trace("Started Saving Uploaded Document in AsyncFileUpload1_UploadedComplete ", Session["LoggedUserId"].ToString());


        //                TodayDate = DateTime.Today.ToString("yyyy-MM-dd");
        //                string TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"];

        //                if (!Directory.Exists(TempFolder))
        //                {
        //                    Directory.CreateDirectory(TempFolder);
        //                }

        //                if (Convert.ToString(Session["EncyptImgName"]).Length > 0)
        //                {
        //                    try
        //                    {
        //                        File.Delete(TempFolder + Convert.ToString(Session["EncyptImgName"]));
        //                        File.Delete(TempFolder + beforedot(Convert.ToString(Session["EncyptImgName"])) + ".pdf");
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
        //                    }
        //                }

        //                if (AsyncFileUpload1.HasFile)
        //                {
        //                    AfterDot = afterdot(AsyncFileUpload1.FileName);
        //                    AsyncFileUpload1.SaveAs(TempFolder + Encryptdata(txtRefid.Text) + AfterDot);
        //                    Session["TempFolder"] = TempFolder;
        //                    Session["EncyptImgName"] = Encryptdata(txtRefid.Text) + AfterDot;
        //                    Session["FileName"] = AsyncFileUpload1.FileName;
        //                }
        //                Logger.Trace("Finished Saving Uploaded Document in AsyncFileUpload1_UploadedComplete ", Session["LoggedUserId"].ToString());
        //                // if the checkbox checked then will be allow slicing and preview then upload or splitting task will be handing over to windows service
        //                if (System.Convert.ToBoolean(hdnPreviewCheckBoxChecked.Value) == true) /* DMSENH6-4637 A */
        //                {
        //                    //  string result = "Success";
        //                    if (Session["EncyptImgName"] != null && afterdot(Session["EncyptImgName"].ToString()).Length > 0)
        //                    {
        //                        switch (afterdot(Session["EncyptImgName"].ToString()).ToLower())
        //                        {
        //                            case ".doc":
        //                            case ".docx":
        //                                PageCount = new MSWord().Convert(Session["TempFolder"].ToString() + Encryptdata(txtRefid.Text) + afterdot(Session["EncyptImgName"].ToString()), Session["TempFolder"].ToString() + Encryptdata(txtRefid.Text) + ".pdf", true);
        //                                Logger.Trace("AsyncFileUpload1_UploadedComplete Pagecount of MSWord " + PageCount, Session["LoggedUserId"].ToString());
        //                                break;
        //                            case ".dwg":
        //                            case ".dxf":
        //                                PageCount = 1;
        //                                // Logger.Trace("AsyncFileUpload1_UploadedComplete Pagecount of MSWord " + PageCount, Session["LoggedUserId"].ToString());
        //                                break;
        //                            case ".ppt":
        //                            case ".pptx":
        //                                PageCount = new MSPowerPoint().Convert(Session["TempFolder"].ToString() + Encryptdata(txtRefid.Text) + afterdot(Session["EncyptImgName"].ToString()), Session["TempFolder"].ToString() + Encryptdata(txtRefid.Text) + ".pdf", true);
        //                                Logger.Trace("AsyncFileUpload1_UploadedComplete Pagecount of MSPowerPoint " + PageCount, Session["LoggedUserId"].ToString());
        //                                break;
        //                            case ".xls":
        //                            case ".xlsx":
        //                                PageCount = new MSExcel().Convert(Session["TempFolder"].ToString() + Encryptdata(txtRefid.Text) + afterdot(Session["EncyptImgName"].ToString()), Session["TempFolder"].ToString() + Encryptdata(txtRefid.Text) + ".pdf", true);
        //                                Logger.Trace("AsyncFileUpload1_UploadedComplete Pagecount of MSExcel " + PageCount, Session["LoggedUserId"].ToString());
        //                                break;
        //                            case ".tif":
        //                            case ".tiff":


        //                            case ".jpg":
        //                            case ".bmp":
        //                            case ".jpeg":
        //                            case ".png":
        //                            case ".gif":
        //                            case ".giff":
        //                                PageCount = new Image2Pdf().tiff2PDF(Session["TempFolder"].ToString() + Encryptdata(txtRefid.Text) + afterdot(Session["EncyptImgName"].ToString()), Session["TempFolder"].ToString() + Encryptdata(txtRefid.Text), true);
        //                                Logger.Trace("AsyncFileUpload1_UploadedComplete Pagecount of Image2Pdf().tiff2PDF " + PageCount, Session["LoggedUserId"].ToString());
        //                                break;
        //                            case ".pdf":
        //                                PageCount = new Image2Pdf().ExtractPages(Session["TempFolder"].ToString() + Encryptdata(txtRefid.Text) + afterdot(Session["EncyptImgName"].ToString()));
        //                                Logger.Trace("AsyncFileUpload1_UploadedComplete Pagecount of Image2Pdf().ExtractPages " + PageCount, Session["LoggedUserId"].ToString());
        //                                break;
        //                            default:
        //                                // result = "Failed";
        //                                break;
        //                        }
        //                    }

        //                    hdnPageCount.Value = PageCount.ToString();
        //                    Session["PageCount"] = Convert.ToInt32(PageCount.ToString());
        //                }
        //                else
        //                {

        //                    btnPreview.Enabled = false;/* DMSENH6-4637 A */
        //                    //disabling preview if preview chek box is not checked

        //                }
        //            }
        //            else
        //            {
        //                divMsg.Style.Add("display", "block");
        //                divMsg.InnerText = "Please select both Document Type and Department!";
        //            }
        //        }

        //        Logger.Trace("AsyncFileUpload1_UploadedComplete Finished", Session["LoggedUserId"].ToString());
        //    }
        //    catch (Exception ex)
        //    {

        //        Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
        //    }

        //}

        public void DeleteAnnotation()
        {
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            Results results = new Results();
            Annotations AnnotationsBE = new Annotations();
            AnnotationsBL objAnnotations = new AnnotationsBL();

            try
            {
                AnnotationsBE.DocumentId = Convert.ToInt32(Request.QueryString["id"]);
                results = objAnnotations.ManageAnnotatations(AnnotationsBE, "DeleteAnnotation", loginUser.LoginOrgId.ToString(), loginUser.LoginToken);
            }
            catch (Exception)
            {

                throw;
            }
        }

        protected string getIndexFieldsList(DataSet ds)
        {

            string IndexFieldsList = string.Empty;

            try
            {

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {

                        IndexFieldsList = IndexFieldsList + ds.Tables[0].Rows[i]["TemplateFields_vDBFld"].ToString() + ",";
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
            return IndexFieldsList.Substring(0, IndexFieldsList.Length - 1);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (RadioButtonList1.SelectedValue == "DWGFiles")
            {
                //if (cadpreview == "true")
                //{
                SaveDWGImages();
                //}
                //else
                //{
                //    ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('Please click on CADPREVIEW for upload .CAD files.');", true);
                //}
            }
            //bool IsPDFheader = IsPDFHeader(Session["TempFileLocation"].ToString());
            string TempFolder1 = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"];

            string Actual_TempFolder = TempFolder1 + Session["EncyptImgName"].ToString();
            bool IsPDFheader = IsPDFHeader(Actual_TempFolder);

            if (IsPDFheader == true)
            {
                bool SplittingSeperated = System.Convert.ToBoolean(hdnPreviewCheckBoxChecked.Value) == true ? false : true;/* DMSENH6-4637 A */

                string temp = string.Empty;
                Results result = null;
                DocumentBL objDocumentBL = new DocumentBL();
                string xmlPageNoMappings = string.Empty;
                try
                {
                    hdnUploaded.Value = "TRUE";
                    Logger.Trace("Started Saving Uploaded Document in Save Button Click", Session["LoggedUserId"].ToString());
                    UserBase loginUser = (UserBase)Session["LoggedUser"];
                    string ServerPath = System.Configuration.ConfigurationManager.AppSettings["ImageLocation"];
                    TodayDate = DateTime.Today.ToString("yyyy-MM-dd");
                    string TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"];
                    string ActualFolder = ServerPath + Session["LoginOrgCode"].ToString() + "\\" + Session["LoggedUserId"].ToString() + "\\" + TodayDate + "\\";
                    string OrginalFilePath = string.Empty;
                    if (!Directory.Exists(ActualFolder))
                    {
                        Directory.CreateDirectory(ActualFolder);
                    }

                    NavigatePanel.Visible = true;
                    this.NumberOfControls = 0;

                    DataSet SendTempl = new DataSet();

                    string strQuery = string.Empty;
                    string strColumns = string.Empty;
                    string strValues = string.Empty;
                    int intVersion = 1;

                    if (Session["PageCount"] == null || Session["PageCount"].ToString().Trim() == "")
                    {
                        Session["PageCount"] = 0;
                    }
                    int intPageCount = Convert.ToInt32(Session["PageCount"].ToString());
                    int iProcId = Convert.ToInt32(Request.QueryString["id"]);
                    SearchFilter filter = new SearchFilter();
                    string action = "ExportDocumentType";
                    filter.DocumentTypeID = Convert.ToInt32(cmbDocumentType.SelectedValue);
                    filter.DepartmentID = Convert.ToInt32(cmbDepartment.SelectedValue);
                    int fldcount = 0;
                    string xml = string.Empty;
                    string Uploadaction = string.Empty;

                    DataSet dsFieldNames = new BatchUploadBL().GetHeadersForBatchUpload(filter, action, loginUser.LoginOrgId.ToString(), loginUser.LoginToken);


                    string[] arColList = getIndexFieldsList(dsFieldNames).Split(',');
                    if (!string.IsNullOrEmpty(Request.QueryString["ArchivedAction"]))
                    {
                        Versionmanagement();
                    }

                    if (System.Convert.ToBoolean(hdnPreviewCheckBoxChecked.Value) == true)/* DMSENH6-4637 A */
                    {
                        if (Convert.ToString(Session["Ext"]).Length > 0)
                        {

                            if (Convert.ToString(Session["Ext"]) == ".pdf")
                            {
                                if (Session["TempFileLocation"] != null && Session["hdnOrgFileLocation"] != null)
                                {
                                    //Rename document to actual name and update session value
                                    string FilePath = Session["TempFileLocation"].ToString();
                                    string FilepathToRename = FilePath.Substring(0, FilePath.LastIndexOf("\\")) + "\\" + Session["Encryptdata"].ToString();
                                    try
                                    {
                                        if (File.Exists(FilePath))
                                        {
                                            File.Copy(FilePath, FilepathToRename);
                                        }
                                    }
                                    catch (Exception)
                                    {

                                        Logger.Trace("save buton click file copy failed " + FilePath, Session["LoggedUserId"].ToString());
                                    }

                                    if (File.Exists(FilepathToRename) && File.Exists(FilePath))
                                        File.Delete(FilePath);
                                    Session["TempFileLocation"] = FilepathToRename;

                                    ActualFolder = beforedot(Session["hdnOrgFileLocation"].ToString());
                                    string Actual_FileName_TempFolder = Session["TempFileLocation"].ToString();
                                    string Zip_FileName_TempFolder = beforedot(Session["TempFileLocation"].ToString()) + ".zip";
                                    string Zip_FileName_ActaulFolder = ActualFolder + ".zip";

                                    // Zip file
                                    FileZip.Zip(Actual_FileName_TempFolder);
                                    try
                                    {
                                        if (File.Exists(beforedot(Session["hdnOrgFileLocation"].ToString()) + ".zip"))
                                            File.Delete(beforedot(Session["hdnOrgFileLocation"].ToString()) + ".zip");
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Trace("Exception: " + ex.Message, Session["LoggedUserId"].ToString());
                                    }

                                    if (File.Exists(Zip_FileName_TempFolder))
                                    {
                                        File.Copy(Zip_FileName_TempFolder, beforedot(Session["hdnOrgFileLocation"].ToString()) + ".zip", true);
                                    }
                                    try
                                    {
                                        string Extension = afterdot(Session["hdnOrgFileLocation"].ToString());
                                        if (File.Exists(Session["hdnOrgFileLocation"].ToString()))
                                        {
                                            File.Delete(Session["hdnOrgFileLocation"].ToString());
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Trace("Exception: " + ex.Message, Session["LoggedUserId"].ToString());
                                    }
                                }
                                else
                                {
                                    Logger.Trace("Session value is null ", Session["LoggedUserId"].ToString());
                                }
                            }
                            else
                            {
                                if (Session["hdnOrgFileLocation"] != null && Session["TempFileLocation"] != null)
                                {
                                    ActualFolder = beforedot(Session["hdnOrgFileLocation"].ToString());
                                    string Actual_FileName_TempFolder = Session["TempFileLocation"].ToString();
                                    string Zip_FileName_TempFolder = beforedot(Session["TempFileLocation"].ToString()) + ".zip";
                                    string Zip_FileName_ActaulFolder = ActualFolder + ".zip";
                                    //Added for zipping the document
                                    FileZip.Zip(Actual_FileName_TempFolder);
                                    if (File.Exists(beforedot(Session["hdnOrgFileLocation"].ToString()) + ".zip"))
                                        File.Delete(beforedot(Session["hdnOrgFileLocation"].ToString()) + ".zip");
                                    try
                                    {
                                        if (File.Exists(Zip_FileName_TempFolder))
                                        {
                                            File.Copy(Zip_FileName_TempFolder, beforedot(Session["hdnOrgFileLocation"].ToString()) + ".zip", true);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Trace("Exception " + ex.Message, Session["LoggedUserId"].ToString());
                                    }

                                    try
                                    {
                                        string extension = afterdot(Session["hdnOrgFileLocation"].ToString());
                                        if (File.Exists(Session["hdnOrgFileLocation"].ToString()))
                                        {
                                            File.Delete(Session["hdnOrgFileLocation"].ToString());
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Trace("Exception " + ex.Message, Session["LoggedUserId"].ToString());
                                    }
                                }
                                else
                                {
                                    Logger.Trace("Session value is null ", Session["LoggedUserId"].ToString());
                                }
                            }
                            //long filesize;
                            if (Session["EncyptImgName"] != null)
                            {
                                temp = Encryptdata(Session["EncyptImgName"].ToString());


                                if (Convert.ToString(Session["Ext"]) == ".tif")
                                {
                                    //var fi = new FileInfo( Session["TempFileLocation"].ToString());
                                    //filesize = fi.Length;
                                    if (Directory.Exists(ActualFolder))
                                    {
                                        Directory.Delete(ActualFolder, true);
                                    }
                                    //Directory.Move(TempFolder + temp, ActualFolder);
                                    copyDirectory(TempFolder + temp, ActualFolder);
                                }
                                else
                                {
                                    if (Directory.Exists(ActualFolder))
                                    {
                                        Directory.Delete(ActualFolder, true);
                                    }
                                    copyDirectory(TempFolder + temp, ActualFolder);
                                }
                                Session["EncyptImgName"] = "";
                                try
                                {
                                    Directory.Delete(TempFolder + temp, true);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Trace("Exception " + ex.Message, Session["LoggedUserId"].ToString());
                                }
                            }
                        }
                        else
                        {
                            if (Session["EncyptImgName"] != null && Session["FileName"] != null)
                            {
                                string Actual_FileName_TempFolder = TempFolder + Session["EncyptImgName"].ToString();
                                string Zip_FileName_TempFolder = TempFolder + beforedot(Session["EncyptImgName"].ToString()) + ".zip";
                                string Zip_FileName_ActaulFolder = ActualFolder + beforedot(Session["EncyptImgName"].ToString()) + ".zip";

                                string Splitted_TempFolder = TempFolder + beforedot(Session["EncyptImgName"].ToString());
                                string Splitted_ActualFolder = ActualFolder + beforedot(Session["EncyptImgName"].ToString());

                                //Added for zipping the document
                                FileZip.Zip(Actual_FileName_TempFolder);
                                // Copy zipped file to original location
                                if (File.Exists(Zip_FileName_ActaulFolder))
                                    File.Delete(Zip_FileName_ActaulFolder);
                                try
                                {
                                    if (File.Exists(Zip_FileName_TempFolder))
                                        File.Copy(Zip_FileName_TempFolder, Zip_FileName_ActaulFolder, true);
                                }
                                catch (Exception ex)
                                {

                                    Logger.Trace("Exception:" + ex.Message, Session["LoggedUserId"].ToString());
                                }

                                // Copy splitted files to original location
                                if (Directory.Exists(Splitted_ActualFolder))
                                    Directory.Delete(Splitted_ActualFolder, true);
                                copyDirectory(Splitted_TempFolder, Splitted_ActualFolder);

                                //Directory.Move(Splitted_TempFolder, Splitted_ActualFolder);

                                // Clear Temp folder
                                try
                                {
                                    // Delete zipped file
                                    if (File.Exists(Zip_FileName_ActaulFolder) && File.Exists(Zip_FileName_TempFolder))
                                        File.Delete(Zip_FileName_TempFolder);
                                    Directory.Delete(Splitted_TempFolder, true);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Trace("Error while clearing temporary folder. " + ex.Message, Session["LoggedUserId"].ToString());
                                }
                                Session["EncyptImgName"] = "";
                            }
                            else
                            {
                                Logger.Trace("Session value is null " + Session["EncyptImgName"].ToString(), Session["LoggedUserId"].ToString());
                            }
                        }
                        OrginalFilePath = ActualFolder + Encryptdata(txtRefid.Text) + afterdot(AsyncFileUpload1.FileName);
                    }
                    /* DMSENH6-4637 BS */
                    else
                    {

                        try
                        {
                            //checking for orginal document exists or not if exists deleting that document
                            if (Session["hdnOrgFileLocation"] != null)
                            {
                                ActualFolder = beforedot(Session["hdnOrgFileLocation"].ToString());
                                string Splitted_ActualFolder = ActualFolder + beforedot(Session["EncyptImgName"].ToString());
                                string Zip_FileName_ActaulFolder = ActualFolder + beforedot(Session["EncyptImgName"].ToString()) + ".zip";
                                Directory.Delete(Splitted_ActualFolder, true);
                                File.Delete(Zip_FileName_ActaulFolder);
                            }
                        }
                        catch { }

                        // section to move to temp location so windows servive will be pulling the document and do the slicing
                        AfterDot = afterdot(AsyncFileUpload1.FileName);
                        string FilePath = TempFolder + Encryptdata(txtRefid.Text) + AfterDot;
                        string TempServerPath = System.Configuration.ConfigurationManager.AppSettings["TempImageLocation"] + Session["LoginOrgCode"].ToString() + "\\" + Session["LoggedUserId"].ToString() + "\\" + TodayDate + "\\" + Encryptdata(txtRefid.Text) + "\\";

                        string TempFilePath = TempServerPath + Session["FileName"];

                        if (!Directory.Exists(TempServerPath))
                        {
                            Directory.CreateDirectory(TempServerPath);

                        }
                        File.Copy(FilePath, TempFilePath);
                        OrginalFilePath = TempFilePath;
                    }
                    //END First IF
                    /* DMSENH6-4637 BE */
                    // First upload


                    if (iProcId == 0)
                    {

                        strColumns = "UPLOAD_iDepartment,  UPLOAD_iDocType,  UPLOAD_vOriginFileName,  UPLOAD_vDocName,  UPLOAD_vDocVirtualPath, UPLOAD_vDocPhysicalPath,  UPLOAD_vRefID,  UPLOAD_vDocVersion,  UPLOAD_iSize,  UPLOAD_vType,UPLOAD_iModifiedBy,UPLOAD_iUploadedBy,UPLOAD_vSearchKeyWords, UPLOAD_iPageCount,  UPLOAD_iOrgID,  UPLOAD_bIsUploaded,";
                        strValues = cmbDepartment.SelectedValue.ToString() + ","
                            + cmbDocumentType.SelectedValue.ToString() + ",&apos;"
                            + AsyncFileUpload1.FileName + "&apos;,&apos;"
                            + Encryptdata(txtRefid.Text) + afterdot(AsyncFileUpload1.FileName) + "&apos;,&apos;"
                            + "\\Writer\\UploadedDocuments\\" + Session["LoginOrgCode"].ToString() + "\\" + Session["LoggedUserId"].ToString() + "\\" + TodayDate + "\\" + "&apos;,&apos;"
                            + OrginalFilePath + "&apos;,&apos;"
                            + txtRefid.Text + "&apos;,"
                            + intVersion + ","
                            + AsyncFileUpload1.PostedFile.ContentLength + ",&apos;"
                            + AsyncFileUpload1.PostedFile.ContentType + "&apos;,"
                            + loginUser.UserId.ToString() + ","
                            + loginUser.UserId.ToString() + ",&apos;"
                            + txtKeyword.Text + "&apos;,"
                            + intPageCount + ","
                            + loginUser.LoginOrgId.ToString() + ","
                            + "1,";

                        fldcount = 0;

                        foreach (Control c in pnlIndexpro.Controls)
                        {
                            if (c is HtmlTable)
                            {
                                HtmlTable td = (HtmlTable)c;
                                foreach (Control tc in td.Controls)
                                {
                                    if (tc is HtmlTableRow)
                                    {
                                        HtmlTableRow row = (HtmlTableRow)tc;
                                        foreach (Control trc in row.Controls)
                                        {
                                            if (trc is HtmlTableCell)
                                            {
                                                HtmlTableCell cell = (HtmlTableCell)trc;
                                                //int count = this.NumberOfControls;
                                                
                                                foreach (Control control in cell.Controls)
                                                {
                                                   // string msgv = "";
                                                    if (control is TextBox)
                                                    {
                                                                                                          
                                                        TextBox txt = control as TextBox;

                                                      
                                                        //var regularExpression = "^[a-zA-Z0-9 ]*$";
                                                        //var matchVendorcode = Regex.IsMatch(txt.Text, regularExpression);
                                                        //if (matchVendorcode == true)
                                                        //{
                                                            strColumns += GetDBFieldName(dsFieldNames, txt.ID) + ",";
                                                            strValues += "N&apos;" + txt.Text + "&apos;,";

                                                            fldcount++;
                                                        //}
                                                        //else
                                                        //{

                                                        //    Response.Redirect("DocumentUpload.aspx?action=SpecialCharactors", false);

                                                        //    divMsg.Style.Add("display", "block");
                                                        //    divMsg.InnerText = "Special charactors not allowed, Please enter valid charactors";

                                                        //    //Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Alert", "alert('Please enter valid charactor.Special charactors not allowed.')", true);
                                                        //    return;
                                                        //}

    
                                                    }
                                                    else if (control is DropDownList)
                                                    {
                                                        DropDownList drop = control as DropDownList;
                                                        if (drop.SelectedItem.Text != "--select--")
                                                        {
                                                            strColumns += GetDBFieldName(dsFieldNames, drop.ID) + ",";
                                                            strValues += "&apos;" + drop.SelectedItem + "&apos;,";

                                                            
                                                        }

                                                        fldcount++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                      

                          

                        strQuery = "insert into UPLOAD (" + strColumns.Substring(0, strColumns.Length - 1) + ") values ( " + strValues.Substring(0, strValues.Length - 1) + ")";
                        Uploadaction = "InsertUploadDetails";

                        xml = "<Table><row><query>" + strQuery + "</query></row></Table>";
                    }

                    int ReturnCode = 1;
                    if (Convert.ToString(Request.QueryString["id"]) != null && Convert.ToString(Request.QueryString["docid"]) != null && Convert.ToString(Request.QueryString["depid"]) != null)
                    {

                        intVersion = Convert.ToInt32(ViewState["Version"]) + 1;
                        if (iProcId > 0)
                        {
                            if (Convert.ToString(Session["Ext"]).Length > 0)
                            {

                                strValues = "UPLOAD_iDepartment =" + cmbDepartment.SelectedValue.ToString() + ","
                                   + "UPLOAD_iDocType=" + cmbDocumentType.SelectedValue.ToString() + ","
                                   + "UPLOAD_vDocVersion=" + intVersion + ","
                                   + "UPLOAD_iModifiedBy=" + loginUser.UserId.ToString() + ","
                                   + "UPLOAD_iUploadedBy=" + loginUser.UserId.ToString() + ","
                                   + "UPLOAD_vSearchKeyWords=&apos;" + txtKeyword.Text + "&apos;,"
                                   + "UPLOAD_iPageCount=" + intPageCount + ","
                                   + "UPLOAD_iOrgID=" + loginUser.LoginOrgId.ToString() + ","
                                   + "UPLOAD_bIsUploaded=1,";

                            }
                            else
                            {
                                Uploadaction = "UpdateDocumentDetails";

                                strValues = "UPLOAD_iDepartment =" + cmbDepartment.SelectedValue.ToString() + ","
                                    + "UPLOAD_iDocType=" + cmbDocumentType.SelectedValue.ToString() + ","
                                    + "UPLOAD_vOriginFileName=&apos;" + AsyncFileUpload1.FileName + "&apos;,"
                                    + "UPLOAD_vDocName=&apos;" + Encryptdata(txtRefid.Text) + afterdot(AsyncFileUpload1.FileName) + "&apos;,"
                                    + "UPLOAD_vDocVirtualPath=&apos;" + "\\Writer\\UploadedDocuments\\" + Session["LoginOrgCode"].ToString() + "\\" + Session["LoggedUserId"].ToString() + "\\" + TodayDate + "\\\\" + "&apos;,"
                                    + "UPLOAD_vDocPhysicalPath=&apos;" + OrginalFilePath + "&apos;,"
                                    + "UPLOAD_vRefID=&apos;" + txtRefid.Text + "&apos;,"
                                    + "UPLOAD_vDocVersion=" + intVersion + ","
                                    + "UPLOAD_iSize=" + AsyncFileUpload1.PostedFile.ContentLength + ","
                                    + "UPLOAD_vType=&apos;" + AsyncFileUpload1.PostedFile.ContentType + "&apos;,"
                                    + "UPLOAD_iModifiedBy=" + loginUser.UserId.ToString() + ","
                                    + "UPLOAD_iUploadedBy=" + loginUser.UserId.ToString() + ","
                                    + "UPLOAD_vSearchKeyWords=&apos;" + txtKeyword.Text + "&apos;,"
                                    + "UPLOAD_iPageCount=" + intPageCount + ","
                                    + "UPLOAD_iOrgID=" + loginUser.LoginOrgId.ToString() + ","
                                    + "UPLOAD_bIsUploaded=1,";
                            }
                            foreach (Control c in pnlIndexpro.Controls)
                            {
                                if (c is HtmlTable)
                                {
                                    HtmlTable td = (HtmlTable)c;
                                    foreach (Control tc in td.Controls)
                                    {
                                        if (tc is HtmlTableRow)
                                        {
                                            HtmlTableRow row = (HtmlTableRow)tc;
                                            foreach (Control trc in row.Controls)
                                            {
                                                if (trc is HtmlTableCell)
                                                {
                                                    HtmlTableCell cell = (HtmlTableCell)trc;
                                                    //int count = this.NumberOfControls;

                                                    foreach (Control control in cell.Controls)
                                                    {
                                                        if (control is TextBox)
                                                        {
                                                            TextBox txt = control as TextBox;
                                                            strValues += GetDBFieldName(dsFieldNames, txt.ID) + "=&apos;" + txt.Text + "&apos;,";
                                                            //DR[GetDBFieldName(dsFieldNames, txt.ID)] = txt.Text;
                                                            fldcount++;
                                                        }
                                                        else if (control is DropDownList)
                                                        {
                                                            DropDownList drop = control as DropDownList;
                                                            if (drop.SelectedItem.Text != "--select--")
                                                            {
                                                                strValues += GetDBFieldName(dsFieldNames, drop.ID) + "=&apos;" + drop.SelectedItem + "&apos;,";
                                                                //DR[GetDBFieldName(dsFieldNames, drop.ID)] = drop.SelectedItem;
                                                            }
                                                            fldcount++;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            strQuery = "update upload set " + strValues.Substring(0, strValues.Length - 1) + " where UPLOAD_iProcessID = " + Convert.ToString(Request.QueryString["id"]);
                            xml = "<Table><row><query>" + strQuery + "</query></row></Table>";
                            //UploadDocBL bl = new UploadDocBL();
                            //Results res = new Results();
                            //res = bl.UpdateDocumentDetails(Entityobj.UploadDataset, "UpdateDocumentDetails", hdnLoginOrgId.Value, hdnLoginToken.Value, "Upload New Version");
                            if (Convert.ToString(Session["Ext"]).Length > 0)
                            {
                                Uploadaction = "UpdateDocumentDetails";
                                Session["TempFileLocation"] = "";
                                Session["TempFileLocation"] = "";
                                Session["Ext"] = "";
                                Session["Count"] = "";
                            }

                            //Updating upload table and annotation table 
                            result = objDocumentBL.ManageUploadedDocuments(xml, hdnLoginOrgId.Value, hdnLoginToken.Value, Uploadaction, iProcId, SplittingSeperated, xmlPageNoMappings);
                            RemoveDigitalSignature();
                            if (Convert.ToString(Request.QueryString["id"]) != null && Convert.ToString(Request.QueryString["docid"]) != null && Convert.ToString(Request.QueryString["depid"]) != null)
                            {
                                DeleteAnnotation();
                            }

                            if (Request.QueryString["action"] != null && Request.QueryString["action"] == "batchedit")
                            {
                                //ReturnCode next proc id
                                if (result.returncode > 0)
                                {
                                    ReturnCode = result.returncode;
                                    Logger.Trace("Save button click after saving or updating the ReturnCode= " + ReturnCode, Session["LoggedUserId"].ToString());
                                    Response.Redirect("DocumentUpload.aspx?action=batchedit&showmsg=true&id=" + ReturnCode + "&docid=" + Request.QueryString["docid"] + "&depid=" + Request.QueryString["depid"]);
                                }
                                else
                                {
                                    Response.Redirect("BatchUpload.aspx?action=batchedit");
                                }
                            }
                        }
                    }
                    else
                    {
                        //Updating upload table and annotation table 
                        result = objDocumentBL.ManageUploadedDocuments(xml, hdnLoginOrgId.Value, hdnLoginToken.Value, Uploadaction, iProcId, SplittingSeperated, xmlPageNoMappings);
                        RemoveDigitalSignature();
                        if (Convert.ToString(Request.QueryString["id"]) != null && Convert.ToString(Request.QueryString["docid"]) != null && Convert.ToString(Request.QueryString["depid"]) != null)
                        {
                            DeleteAnnotation();
                        }
                        ReturnCode = result.returncode;
                        Logger.Trace("Save button click after saving or updating the ReturnCode= " + ReturnCode, Session["LoggedUserId"].ToString());
                        //File.Delete(Convert.ToString(Session["TempFolder"]) + Convert.ToString(Session["EncyptImgName"]));
                    }

                    if (result.ErrorState == 0)
                    {
                        Session["TempImg"] = "";

                        Response.Redirect("DocumentUpload.aspx?action=upload", false);
                    }
                    else if (result.ErrorState == 1)
                    {
                        hdnUploaded.Value = "FALSE";
                        //DMS04-3520M 
                        Response.Write("<script>alert('" + result.Message + "');</script>");

                    }
                    else
                    {
                        hdnUploaded.Value = "FALSE";
                        Response.Write("<script>alert('Please verify the values in the Index Properties!');</script>");
                    }
                    Logger.Trace("Finished Saving Uploaded Document in Save Button Click", Session["LoggedUserId"].ToString());
                }
                catch (Exception ex)
                {
                    hdnUploaded.Value = "FALSE";

                    Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
                }


            }
            else
            {


                Response.Redirect("DocumentUpload.aspx?action=uploadFail", false);

                divMsg.Style.Add("display", "block");
                divMsg.InnerText = "Please select valid PDF file for upload!";
            }
        }

        protected void SaveDWGImages()
        {

            //  bool SplittingSeperated = System.Convert.ToBoolean(hdnPreviewCheckBoxChecked.Value) == true ? false : true;/* DMSENH6-4637 A */
            // bool SplittingSeperated = true;
            string temp = string.Empty;
            Results result = null;
            DocumentBL objDocumentBL = new DocumentBL();
            string xmlPageNoMappings = string.Empty;
            try
            {
                hdnUploaded.Value = "TRUE";
                Logger.Trace("Started Saving Uploaded Document in Save Button Click", Session["LoggedUserId"].ToString());
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                string ServerPath = System.Configuration.ConfigurationManager.AppSettings["ImageLocation"];
                TodayDate = DateTime.Today.ToString("yyyy-MM-dd");
                string TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"];
                string ActualFolder = ServerPath + Session["LoginOrgCode"].ToString() + "\\" + Session["LoggedUserId"].ToString() + "\\" + TodayDate + "\\";
                string OrginalFilePath = string.Empty;
                if (!Directory.Exists(ActualFolder))
                {
                    Directory.CreateDirectory(ActualFolder);
                }

                NavigatePanel.Visible = true;
                this.NumberOfControls = 0;

                DataSet SendTempl = new DataSet();

                string strQuery = string.Empty;
                string strColumns = string.Empty;
                string strValues = string.Empty;
                int intVersion = 1;
                if (Session["PageCount"] == null || Session["PageCount"].ToString().Trim() == "")
                {
                    Session["PageCount"] = 0;
                }
                int intPageCount = Convert.ToInt32(Session["PageCount"].ToString());
                int iProcId = Convert.ToInt32(Request.QueryString["id"]);
                SearchFilter filter = new SearchFilter();
                string action = "ExportDocumentType";
                filter.DocumentTypeID = Convert.ToInt32(cmbDocumentType.SelectedValue);
                filter.DepartmentID = Convert.ToInt32(cmbDepartment.SelectedValue);
                int fldcount = 0;
                string xml = string.Empty;
                string Uploadaction = string.Empty;

                DataSet dsFieldNames = new BatchUploadBL().GetHeadersForBatchUpload(filter, action, loginUser.LoginOrgId.ToString(), loginUser.LoginToken);


                string[] arColList = getIndexFieldsList(dsFieldNames).Split(',');
                if (!string.IsNullOrEmpty(Request.QueryString["ArchivedAction"]))
                {
                    Versionmanagement();
                }


                /* DMSENH6-4637 BS */
                //else
                //{

                try
                {
                    //checking for orginal document exists or not if exists deleting that document
                    if (Session["hdnOrgFileLocation"] != null)
                    {
                        ActualFolder = beforedot(Session["hdnOrgFileLocation"].ToString());
                        string Splitted_ActualFolder = ActualFolder + beforedot(Session["EncyptImgName"].ToString());
                        string Zip_FileName_ActaulFolder = ActualFolder + beforedot(Session["EncyptImgName"].ToString()) + ".zip";
                        Directory.Delete(Splitted_ActualFolder, true);
                        File.Delete(Zip_FileName_ActaulFolder);
                    }
                }
                catch { }

                // section to move to temp location so windows servive will be pulling the document and do the slicing
                string dwgfilename = Convert.ToString(Session["FileName"]);
                AfterDot = afterdot(dwgfilename);
                string FilePath = TempFolder + Encryptdata(txtRefid.Text) + AfterDot;
                string TempServerPath = System.Configuration.ConfigurationManager.AppSettings["TempImageLocation"] + Session["LoginOrgCode"].ToString() + "\\" + Session["LoggedUserId"].ToString() + "\\" + TodayDate + "\\" + Encryptdata(txtRefid.Text) + "\\";

                string TempFilePath = TempServerPath + Session["FileName"];

                if (!Directory.Exists(TempServerPath))
                {
                    Directory.CreateDirectory(TempServerPath);

                }
                File.Copy(FilePath, TempFilePath);
                OrginalFilePath = TempFilePath;
                // }
                //END First IF
                /* DMSENH6-4637 BE */
                // First upload


                if (iProcId == 0)
                {

                    strColumns = "UPLOAD_iDepartment,  UPLOAD_iDocType,  UPLOAD_vOriginFileName,  UPLOAD_vDocName,  UPLOAD_vDocVirtualPath, UPLOAD_vDocPhysicalPath,  UPLOAD_vRefID,  UPLOAD_vDocVersion,  UPLOAD_iSize,  UPLOAD_vType,UPLOAD_iModifiedBy,UPLOAD_iUploadedBy,UPLOAD_vSearchKeyWords, UPLOAD_iPageCount,  UPLOAD_iOrgID,  UPLOAD_bIsUploaded,";
                    strValues = cmbDepartment.SelectedValue.ToString() + ","
                        + cmbDocumentType.SelectedValue.ToString() + ",&apos;"
                        + Convert.ToString(Session["FileName"]) + "&apos;,&apos;"
                        + Encryptdata(txtRefid.Text) + afterdot(Convert.ToString(Session["FileName"])) + "&apos;,&apos;"
                        + "\\Writer\\UploadedDocuments\\" + Session["LoginOrgCode"].ToString() + "\\" + Session["LoggedUserId"].ToString() + "\\" + TodayDate + "\\" + "&apos;,&apos;"
                        + OrginalFilePath + "&apos;,&apos;"
                        + txtRefid.Text + "&apos;,"
                        + intVersion + ","
                        + Hidcontentlength.Value + ",&apos;"
                        + Hidconttype.Value + "&apos;,"
                        + loginUser.UserId.ToString() + ","
                        + loginUser.UserId.ToString() + ",&apos;"
                        + txtKeyword.Text + "&apos;,"
                        + intPageCount + ","
                        + loginUser.LoginOrgId.ToString() + ","
                        + "1,";


                    fldcount = 0;

                    foreach (Control c in pnlIndexpro.Controls)
                    {
                        if (c is HtmlTable)
                        {
                            HtmlTable td = (HtmlTable)c;
                            foreach (Control tc in td.Controls)
                            {
                                if (tc is HtmlTableRow)
                                {
                                    HtmlTableRow row = (HtmlTableRow)tc;
                                    foreach (Control trc in row.Controls)
                                    {
                                        if (trc is HtmlTableCell)
                                        {
                                            HtmlTableCell cell = (HtmlTableCell)trc;
                                            //int count = this.NumberOfControls;

                                            foreach (Control control in cell.Controls)
                                            {
                                                if (control is TextBox)
                                                {
                                                    TextBox txt = control as TextBox;
                                                    strColumns += GetDBFieldName(dsFieldNames, txt.ID) + ",";
                                                    strValues += "&apos;" + txt.Text + "&apos;,";
                                                    //DR[GetDBFieldName(dsFieldNames, txt.ID)] = txt.Text;
                                                    fldcount++;
                                                }
                                                else if (control is DropDownList)
                                                {
                                                    DropDownList drop = control as DropDownList;
                                                    if (drop.SelectedItem.Text != "--select--")
                                                    {
                                                        strColumns += GetDBFieldName(dsFieldNames, drop.ID) + ",";
                                                        strValues += "&apos;" + drop.SelectedItem + "&apos;,";

                                                        //DR[GetDBFieldName(dsFieldNames, drop.ID)] = drop.SelectedItem;
                                                    }

                                                    fldcount++;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    strQuery = "insert into UPLOAD (" + strColumns.Substring(0, strColumns.Length - 1) + ") values ( " + strValues.Substring(0, strValues.Length - 1) + ")";
                    Uploadaction = "InsertUploadDetails";

                    xml = "<Table><row><query>" + strQuery + "</query></row></Table>";
                }

                int ReturnCode = 1;
                if (Convert.ToString(Request.QueryString["id"]) != null && Convert.ToString(Request.QueryString["docid"]) != null && Convert.ToString(Request.QueryString["depid"]) != null)
                {

                    intVersion = Convert.ToInt32(ViewState["Version"]) + 1;
                    if (iProcId > 0)
                    {
                        if (Convert.ToString(Session["Ext"]).Length > 0)
                        {

                            strValues = "UPLOAD_iDepartment =" + cmbDepartment.SelectedValue.ToString() + ","
                               + "UPLOAD_iDocType=" + cmbDocumentType.SelectedValue.ToString() + ","
                               + "UPLOAD_vDocVersion=" + intVersion + ","
                               + "UPLOAD_iModifiedBy=" + loginUser.UserId.ToString() + ","
                               + "UPLOAD_iUploadedBy=" + loginUser.UserId.ToString() + ","
                               + "UPLOAD_vSearchKeyWords=&apos;" + txtKeyword.Text + "&apos;,"
                               + "UPLOAD_iPageCount=" + intPageCount + ","
                               + "UPLOAD_iOrgID=" + loginUser.LoginOrgId.ToString() + ","
                               + "UPLOAD_bIsUploaded=1,";

                        }
                        else
                        {
                            Uploadaction = "UpdateDocumentDetails";

                            strValues = "UPLOAD_iDepartment =" + cmbDepartment.SelectedValue.ToString() + ","
                                + "UPLOAD_iDocType=" + cmbDocumentType.SelectedValue.ToString() + ","
                                + "UPLOAD_vOriginFileName=&apos;" + AsyncFileUpload1.FileName + "&apos;,"
                                + "UPLOAD_vDocName=&apos;" + Encryptdata(txtRefid.Text) + afterdot(AsyncFileUpload1.FileName) + "&apos;,"
                                + "UPLOAD_vDocVirtualPath=&apos;" + "\\Writer\\UploadedDocuments\\" + Session["LoginOrgCode"].ToString() + "\\" + Session["LoggedUserId"].ToString() + "\\" + TodayDate + "\\\\" + "&apos;,"
                                + "UPLOAD_vDocPhysicalPath=&apos;" + OrginalFilePath + "&apos;,"
                                + "UPLOAD_vRefID=&apos;" + txtRefid.Text + "&apos;,"
                                + "UPLOAD_vDocVersion=" + intVersion + ","
                                + "UPLOAD_iSize=" + AsyncFileUpload1.PostedFile.ContentLength + ","
                                + "UPLOAD_vType=&apos;" + AsyncFileUpload1.PostedFile.ContentType + "&apos;,"
                                + "UPLOAD_iModifiedBy=" + loginUser.UserId.ToString() + ","
                                + "UPLOAD_iUploadedBy=" + loginUser.UserId.ToString() + ","
                                + "UPLOAD_vSearchKeyWords=&apos;" + txtKeyword.Text + "&apos;,"
                                + "UPLOAD_iPageCount=" + intPageCount + ","
                                + "UPLOAD_iOrgID=" + loginUser.LoginOrgId.ToString() + ","
                                + "UPLOAD_bIsUploaded=1,";
                        }
                        foreach (Control c in pnlIndexpro.Controls)
                        {
                            if (c is HtmlTable)
                            {
                                HtmlTable td = (HtmlTable)c;
                                foreach (Control tc in td.Controls)
                                {
                                    if (tc is HtmlTableRow)
                                    {
                                        HtmlTableRow row = (HtmlTableRow)tc;
                                        foreach (Control trc in row.Controls)
                                        {
                                            if (trc is HtmlTableCell)
                                            {
                                                HtmlTableCell cell = (HtmlTableCell)trc;
                                                //int count = this.NumberOfControls;

                                                foreach (Control control in cell.Controls)
                                                {
                                                    if (control is TextBox)
                                                    {
                                                        TextBox txt = control as TextBox;
                                                        strValues += GetDBFieldName(dsFieldNames, txt.ID) + "=&apos;" + txt.Text + "&apos;,";
                                                        //DR[GetDBFieldName(dsFieldNames, txt.ID)] = txt.Text;
                                                        fldcount++;
                                                    }
                                                    else if (control is DropDownList)
                                                    {
                                                        DropDownList drop = control as DropDownList;
                                                        if (drop.SelectedItem.Text != "--select--")
                                                        {
                                                            strValues += GetDBFieldName(dsFieldNames, drop.ID) + "=&apos;" + drop.SelectedItem + "&apos;,";
                                                            //DR[GetDBFieldName(dsFieldNames, drop.ID)] = drop.SelectedItem;
                                                        }
                                                        fldcount++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        strQuery = "update upload set " + strValues.Substring(0, strValues.Length - 1) + " where UPLOAD_iProcessID = " + Convert.ToString(Request.QueryString["id"]);
                        xml = "<Table><row><query>" + strQuery + "</query></row></Table>";
                        //UploadDocBL bl = new UploadDocBL();
                        //Results res = new Results();
                        //res = bl.UpdateDocumentDetails(Entityobj.UploadDataset, "UpdateDocumentDetails", hdnLoginOrgId.Value, hdnLoginToken.Value, "Upload New Version");
                        if (Convert.ToString(Session["Ext"]).Length > 0)
                        {
                            Uploadaction = "UpdateDocumentDetails";
                            Session["TempFileLocation"] = "";
                            Session["TempFileLocation"] = "";
                            Session["Ext"] = "";
                            Session["Count"] = "";
                        }

                        //Updating upload table and annotation table 
                        //result = objDocumentBL.ManageUploadedDocuments(xml, hdnLoginOrgId.Value, hdnLoginToken.Value, Uploadaction, iProcId, SplittingSeperated, xmlPageNoMappings);
                        //RemoveDigitalSignature();
                        if (Convert.ToString(Request.QueryString["id"]) != null && Convert.ToString(Request.QueryString["docid"]) != null && Convert.ToString(Request.QueryString["depid"]) != null)
                        {
                            DeleteAnnotation();
                        }

                        if (Request.QueryString["action"] != null && Request.QueryString["action"] == "batchedit")
                        {
                            //ReturnCode next proc id
                            if (result.returncode > 0)
                            {
                                ReturnCode = result.returncode;
                                Logger.Trace("Save button click after saving or updating the ReturnCode= " + ReturnCode, Session["LoggedUserId"].ToString());
                                Response.Redirect("DocumentUpload.aspx?action=batchedit&showmsg=true&id=" + ReturnCode + "&docid=" + Request.QueryString["docid"] + "&depid=" + Request.QueryString["depid"]);
                            }
                            else
                            {
                                Response.Redirect("BatchUpload.aspx?action=batchedit");
                            }
                        }
                    }
                }


                else
                {
                    //Updating upload table and annotation table 
                    result = objDocumentBL.ManageUploadedDocuments(xml, hdnLoginOrgId.Value, hdnLoginToken.Value, Uploadaction, iProcId, true, xmlPageNoMappings);
                    RemoveDigitalSignature();
                    if (Convert.ToString(Request.QueryString["id"]) != null && Convert.ToString(Request.QueryString["docid"]) != null && Convert.ToString(Request.QueryString["depid"]) != null)
                    {
                        DeleteAnnotation();
                    }
                    ReturnCode = result.returncode;
                    Logger.Trace("Save button click after saving or updating the ReturnCode= " + ReturnCode, Session["LoggedUserId"].ToString());
                    //File.Delete(Convert.ToString(Session["TempFolder"]) + Convert.ToString(Session["EncyptImgName"]));
                }


                if (result.ErrorState == 0)
                {
                    Session["TempImg"] = "";

                    Response.Redirect("DocumentUpload.aspx?action=upload");
                }
                else if (result.ErrorState == 1)
                {
                    hdnUploaded.Value = "FALSE";
                    //DMS04-3520M 
                    Response.Write("<script>alert('" + result.Message + "');</script>");

                }
                else
                {
                    hdnUploaded.Value = "FALSE";
                    Response.Write("<script>alert('Please verify the values in the Index Properties!');</script>");
                }
                Logger.Trace("Finished Saving Uploaded Document in Save Button Click", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                hdnUploaded.Value = "FALSE";

                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }
        protected string CreateXML()
        {
            string xml = "<Signature>";
            try
            {
                Logger.Trace("CreateXML Started:-", Session["LoggedUserId"].ToString());
                UserBase loginUser = (UserBase)Session["LoggedUser"];

                string UploadId = string.Empty;


                string Password = "";
                string Author = "";
                string Title = "";
                string Subject = "";
                string Keywords = "";
                string Creator = "";
                string Producer = "";
                string Reason = "";
                string Contact = "";
                string Location = "";
                string Createdby = "";
                string Isvisible = "";

                UploadId = Request.QueryString["id"] != null ? Request.QueryString["id"].ToString() : "0"; ;
                xml += "<SignatureDetails><DigitalSignatureDetails_iDcoumentID>" + UploadId + "</DigitalSignatureDetails_iDcoumentID>"
                    + "<DigitalSignatureDetails_vCertificatePassword>" + Password + "</DigitalSignatureDetails_vCertificatePassword>"
                    + "<DigitalSignatureDetails_vAuthor>" + Author + "</DigitalSignatureDetails_vAuthor>"
                    + "<DigitalSignatureDetails_vTitle>" + Title + "</DigitalSignatureDetails_vTitle>"
                    + "<DigitalSignatureDetails_vSubject>" + Subject + "</DigitalSignatureDetails_vSubject>"
                    + "<DigitalSignatureDetails_vKeywords>" + Keywords + "</DigitalSignatureDetails_vKeywords>"
                    + "<DigitalSignatureDetails_vCreator>" + Creator + "</DigitalSignatureDetails_vCreator>"
                    + "<DigitalSignatureDetails_vProducer>" + Producer + "</DigitalSignatureDetails_vProducer>"
                    + "<DigitalSignatureDetails_vReason>" + Reason + "</DigitalSignatureDetails_vReason>"
                    + "<DigitalSignatureDetails_vContact>" + Contact + "</DigitalSignatureDetails_vContact>"
                    + "<DigitalSignatureDetails_vLocation>" + Location + "</DigitalSignatureDetails_vLocation>"
                    + "<DigitalSignatureDetails_iCreatedby>" + Createdby + "</DigitalSignatureDetails_iCreatedby>"
                    + "<DigitalSignatureDetails_bIsVisible>" + Isvisible + "</DigitalSignatureDetails_bIsVisible></SignatureDetails>";

                xml += "</Signature>";
                Logger.Trace("CreateXML Completed:-", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
            return xml;

        }
        public void RemoveDigitalSignature()
        {
            try
            {
                string action = "RemoveDigitalSignature";
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                string signatureXML = CreateXML();
                Results result = null;
                DocumentBL objDocumentBL = new DocumentBL();
                result = objDocumentBL.ManageDigitalSignature(action, "", signatureXML, hdnLoginOrgId.Value, hdnLoginToken.Value);

            }
            catch
            {

            }
        }
        public static void copyDirectory(string Src, string Dst)
        {
            String[] Files;
            if (Dst[Dst.Length - 1] != Path.DirectorySeparatorChar)
                Dst += Path.DirectorySeparatorChar;
            if (!Directory.Exists(Dst)) Directory.CreateDirectory(Dst);
            Files = Directory.GetFileSystemEntries(Src, "*.pdf");
            foreach (string Element in Files)
            {
                try
                {
                    File.Copy(Element, Dst + Path.GetFileName(Element), true);
                }
                catch
                {
                    continue;

                }
            }
        }
        protected bool Versionmanagement()
        {
            bool isval = true;
            try
            {
                Logger.Trace("Versionmanagement started ", Session["LoggedUserId"].ToString());
                SearchFilter filter = new SearchFilter();
                DataSet data = new DataSet();
                UploadDocBL b2 = new UploadDocBL();
                string Versions = string.Empty;
                string TodayDate = DateTime.Today.ToString("yyyy-MM-dd");
                string action = "GetUploadDocumentDetailsForDownload";
                string archiveAction = string.Empty;
                string filePath = string.Empty;
                string TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"];
                string VersionFolder = System.Configuration.ConfigurationManager.AppSettings["VersionFolder"] + Session["LoginOrgCode"].ToString() + "\\" + Session["LoggedUserId"].ToString() + "\\" + TodayDate + "\\";
                if (!Directory.Exists(VersionFolder))
                {
                    Directory.CreateDirectory(VersionFolder);
                }
                //To get the version of document
                filter.UploadDocID = Convert.ToInt32(Request.QueryString["id"]);
                archiveAction = Request.QueryString["ArchivedAction"];
                data = b2.GetUploadDocumentDetails(filter, action, hdnLoginOrgId.Value, hdnLoginToken.Value);
                Versions = data.Tables[0].Rows[0]["DocVersion"].ToString();
                hdnFileLocation.Value = data.Tables[0].Rows[0]["phyFilepath"].ToString();
                string extension = afterdot(hdnFileLocation.Value);
                if (File.Exists(beforedot(hdnFileLocation.Value) + ".zip"))
                {
                    File.Copy(beforedot(hdnFileLocation.Value) + ".zip", VersionFolder + beforedot(data.Tables[0].Rows[0]["EncrDocName"].ToString()) + "_" + "Version " + Versions + ".zip", true);
                    filePath = VersionFolder + beforedot(data.Tables[0].Rows[0]["EncrDocName"].ToString()) + "_" + "Version " + Versions + ".zip";

                }
                UploadDocBL bl = new UploadDocBL();
                Results res = new Results();

                bl.UpdateDocumentDetails(filter.UploadDocID, filePath, hdnLoginOrgId.Value, hdnLoginToken.Value, archiveAction);
                //File.Delete(hdnFileLocation.Value);
                //Directory.Delete(beforedot(hdnFileLocation.Value), true);
                Logger.Trace("Versionmanagement finished ", Session["LoggedUserId"].ToString());
            }
            catch (IOException ex)
            {
                isval = false;
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                isval = false;
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
            return isval;
        }
        protected string GetDBFieldName(DataSet dsFieldNames, string strControlName)
        {
            string strFieldName = string.Empty;
            for (int i = 0; i < dsFieldNames.Tables[0].Rows.Count; i++)
            {
                //DMS5-4101M replaced the space with empty string
                if (dsFieldNames.Tables[0].Rows[i]["TemplateFields_vName"].ToString().Replace(" ", string.Empty) == strControlName)
                {
                    strFieldName = dsFieldNames.Tables[0].Rows[i]["TemplateFields_vDBFld"].ToString();
                }

            }

            Logger.Trace("GetDBFieldName the DBFld " + strFieldName, Session["LoggedUserId"].ToString());

            return strFieldName;
        }
        protected void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                string TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"];
                Logger.Trace("Started Previewing Uploaded Document in btnPreview_Click ", Session["LoggedUserId"].ToString());
                string src = string.Empty;
                string result = "Success";
                string TagEnabled = "False";

                string FileName = Session["TempFolder"].ToString() + Encryptdata(txtRefid.Text) + ".pdf";
                bool IsPDFheader = IsPDFHeader(FileName);

                if (IsPDFheader == true)
                {
                    if (Session["EncyptImgName"] != null && afterdot(Session["EncyptImgName"].ToString()).Length > 0)
                    {
                        // To load PDF file using Generic Handler
                        if (result == "Success")
                        {
                            TagEnabled = "True";
                            Logger.Trace("Src Details : " + GetSrc("Handler") + Session["TempFolder"].ToString() + Encryptdata(txtRefid.Text) + ".pdf", Session["LoggedUserId"].ToString());
                            src = GetSrc("Handler") + Session["TempFolder"].ToString() + Encryptdata(txtRefid.Text) + @"\1.pdf#toolbar=1";
                            hdnSrc.Value = GetSrc("Handler") + Session["TempFolder"].ToString() + Encryptdata(txtRefid.Text) + @"\";
                            //To load page numbers

                            if (TagEnabled == "True")
                            {
                                if (IsPostBack)
                                {
                                    NavigatePanel.Visible = true;
                                }

                                int pagecount = Convert.ToInt32(Session["PageCount"]);
                                ddlpagecount.Items.Clear();
                                for (int i = 0; i < pagecount; i++)
                                {
                                    ddlpagecount.Items.Insert(i, (i + 1).ToString());
                                }
                                // Load image inot viewer
                                hdnPagesCount.Value = pagecount.ToString();
                                LoadImageToViewer(string.Empty);
                            }
                            else
                            {
                                NavigatePanel.Visible = false;
                            }
                            upPagecount.Update();

                        }
                        else
                        {
                            src = GetSrc("NotAvailable");
                        }
                        //  frame1.Attributes.Add("src", src);
                    }
                    else
                    {
                        src = GetSrc("NotAvailable");
                        //   frame1.Attributes.Add("src", src);
                    }
                    Logger.Trace("Finished Previewing Uploaded Document in btnPreview_Click ", Session["LoggedUserId"].ToString());
                }
                else
                {

                    Response.Redirect("DocumentUpload.aspx?action=uploadFail", false);
                    ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('Please select valid PDF file for upload!');", true);
                    divMsg.Style.Add("display", "block");
                    divMsg.InnerText = "Please select valid PDF file for upload!";
                    return;
                }
            }
            catch (Exception ex)
            {
                // frame1.Attributes.Add("src", GetSrc("NotAvailable"));               
                //Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Alert", "alert('Please select valid PDF file for upload!')", true);
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }
        protected void Button4_Command(object sender, CommandEventArgs e)
        {
            string myText = txtKeyword.Text.Trim();
            if (txtKeyword.Text != "")
            {
                if (myText.EndsWith(","))
                {
                    txtKeyword.Text = txtKeyword.Text;
                }
                else
                {
                    txtKeyword.Text = txtKeyword.Text + ", " + e.CommandArgument.ToString().Replace("\n", " ");
                }
            }
            else
            {
                txtKeyword.Text = e.CommandArgument.ToString().Replace("\n", " ");
            }
        }
        protected void cmbDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            bDynamicControlIndexChange = false;
            pnlIndexpro.Controls.Clear();
            this.NumberOfControls = 0;
            txtRefid.Text = "";
            hdnCountControls.Value = "";
            hdnIndexMinLen.Value = "";
            hdnIndexNames.Value = "";
            hdnMandatory.Value = "";
            generatecontrols();


        }
        public string Getcssclass(string cntrl)
        {
            string csclass = string.Empty;
            if (cntrl == "drp")
            {

                // csclass = "ComboStyle";

            }
            else if (cntrl == "txt")
            {

                // csclass = "TextBoxStyle";

            }
            else
            {
                csclass = "LabelStyle";

            }


            return csclass;
        }
        public void generatecontrols()
        {

            if (Convert.ToInt32(cmbDepartment.SelectedValue) != 0 && Convert.ToInt32(cmbDocumentType.SelectedValue) != 0)
            {

                TemplateBL bl = new TemplateBL();

                SearchFilter filter = new SearchFilter();

                try
                {

                    Logger.Trace("generatecontrols Started", Session["LoggedUserId"].ToString());
                    string indexname = string.Empty;
                    string sortOreder = string.Empty;
                    bool IActivive;
                    HtmlTable Tab = new HtmlTable();
                    UserBase loginUser = (UserBase)Session["LoggedUser"];

                    filter.CurrOrgId = loginUser.LoginOrgId;
                    filter.CurrUserId = loginUser.UserId;
                    filter.DocumentTypeName = cmbDocumentType.SelectedItem.ToString();
                    filter.DepartmentID = Convert.ToInt32(cmbDepartment.SelectedValue);
                    filter.DepartmentName = cmbDepartment.SelectedItem.ToString();
                    DataSet TemplateDs = new TemplateBL().GetTemplateDetails(filter);

                    Logger.Trace("Calling database to get reference id and watermark.", Session["LoggedUserId"].ToString());
                    string result = new DocumentBL().GetDocumentReferenceId(filter);
                    Logger.Trace("Database returned result: " + result, Session["LoggedUserId"].ToString());
                    string[] RefIdAndWatermark = result.Split('|');
                    if (RefIdAndWatermark.Length > 0)
                        txtRefid.Text = RefIdAndWatermark[0];
                    Session["Watermark"] = RefIdAndWatermark.Length > 1 ? RefIdAndWatermark[1] : string.Empty;

                    if (cmbDepartment.SelectedValue == "0" || cmbDocumentType.SelectedValue == "0")
                    {
                        pnlIndexpro.Controls.Clear();
                        this.NumberOfControls = 0;
                        txtRefid.Text = "";
                        hdnCountControls.Value = "";
                        hdnIndexMinLen.Value = "";
                        hdnIndexNames.Value = "";
                        hdnMandatory.Value = "";
                    }
                    else
                    {
                        hdnIndexNames.Value = "";
                        hdnIndexType.Value = "";
                        hdnIndexDataType.Value = "";
                        hdnIndexMinLen.Value = "";
                        hdnIndexMinLen.Value = "";
                        hdnMandatory.Value = "";
                        hdnCountControls.Value = "";

                        int ControlsCounter = -1; // if the control counter is even bind controls parallelly, i.e., 4 columns :[ lable:control label:control ] other wise 2 columns style
                        HtmlTableRow TR = new HtmlTableRow();
                        for (int i = 0; i < TemplateDs.Tables[0].Rows.Count; i++)
                        {
                            IActivive = Convert.ToBoolean(TemplateDs.Tables[0].Rows[i]["TemplateFields_bActive"]);
                            indexname = TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString();
                            ++ControlsCounter; // Comment this line to bind controls in 2 coulumns stlye                            
                            if (Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iParentId"]) == 0)
                                TR = new HtmlTableRow();
                            HtmlTableCell TD1 = new HtmlTableCell();
                            HtmlTableCell TD2 = new HtmlTableCell();

                            TD1.Width = "150px";
                            TD2.Width = "150px";
                            // Parellel controls binding
                            HtmlTableCell TD3 = new HtmlTableCell();
                            HtmlTableCell TD4 = new HtmlTableCell();

                            TD3.Width = "150px";
                            TD4.Width = "150px";

                            // Add label text
                            if (!string.Equals(TemplateDs.Tables[0].Rows[i]["TemplateFields_cObjType"].ToString(), "Label", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(TemplateDs.Tables[0].Rows[i]["TemplateFields_cObjType"].ToString(), "Button", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(TemplateDs.Tables[0].Rows[i]["TemplateFields_cObjType"].ToString(), "HiddenField", StringComparison.OrdinalIgnoreCase) && Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iParentId"]) == 0)
                            {
                                Label objCnl = new Label();
                                objCnl.ID = "lbl" + i;
                                objCnl.Text = TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString();
                                objCnl.CssClass = Getcssclass("lbl");
                                objCnl.AssociatedControlID = "";
                                if (Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iParentId"]) == 0)
                                {
                                    TD1.Controls.Add(objCnl);
                                }
                                else
                                    TD3.Controls.Add(objCnl);
                            }
                            #region textbox control
                            if (string.Equals(TemplateDs.Tables[0].Rows[i]["TemplateFields_cObjType"].ToString(), "TextBox", StringComparison.OrdinalIgnoreCase))
                            {
                                TextBox objCnltxt = new TextBox();
                                //objCnltxt.AutoPostBack = Convert.ToBoolean(TemplateDs.Tables[0].Rows[i]["TemplateFields_bPostback"]);
                                //DMS5-4101M replaced the space with empty string
                                objCnltxt.ID = TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString().Replace(" ", string.Empty);
                                objCnltxt.CssClass = Getcssclass("txt");
                                TD2.Controls.Add(objCnltxt);
                                objCnltxt.Width = Unit.Pixel(157);
                                objCnltxt.Height = Unit.Pixel(16);

                                if (TemplateDs.Tables[0].Rows[i]["TemplateFields_vDBType"].ToString() == "Integer")
                                {
                                    objCnltxt.MaxLength = Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iMax"].ToString());
                                    objCnltxt.Attributes.Add("OnKeyPress", "javascript:return CheckNumericKey(event);");
                                }
                                //else if (TemplateDs.Tables[0].Rows[i]["TemplateFields_vDBType"].ToString() == "String")
                                //{
                                //    objCnltxt.Attributes.Add("OnKeyPress", "javascript:return CheckSpecialcharact(event);");
                                //}
                                else if (TemplateDs.Tables[0].Rows[i]["TemplateFields_vDBType"].ToString() == "Boolen")
                                {
                                    objCnltxt.MaxLength = 1;
                                    objCnltxt.Attributes.Add("OnKeyPress", "javascript:return CheckBoolean(event);");
                                }
                                else if (TemplateDs.Tables[0].Rows[i]["TemplateFields_vDBType"].ToString() == "DateTime")
                                {
                                    //objCnltxt.MaxLength = 10;
                                    //objCnltxt.Attributes.Add("OnKeyPress", "javascript:return CheckDateFormat(event.keyCode,'" + TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString() + "');");
                                    //CalendarExtender calext = new CalendarExtender();

                                    //if (!bDynamicControlIndexChange)
                                    //{
                                    //    //DMS5-4101M replaced the space with empty string
                                    //    calext.ID = "cal_" + TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString().Replace(" ", string.Empty);
                                    //    calext.TargetControlID = TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString().Replace(" ", string.Empty);
                                    //    calext.Format = "dd/MM/yyyy";
                                    //    TD2.Controls.Add(calext);
                                    //}
                                    string t2 = (TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString()).ToLower();
                                    if (!(t2.Contains("month")))
                                    {
                                        objCnltxt.MaxLength = 10;
                                        objCnltxt.Attributes.Add("OnKeyPress", "javascript:return CheckDateFormat(event.keyCode,'" + TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString() + "');");

                                        if (!bDynamicControlIndexChange)
                                        {
                                            CalendarExtender calext = new CalendarExtender();
                                            //DMS5-4101M replaced the space with empty string
                                            calext.ID = "cal_" + TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString().Replace(" ", string.Empty);
                                            calext.TargetControlID = TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString().Replace(" ", string.Empty);
                                            calext.Format = "dd/MM/yyyy";
                                            TD2.Controls.Add(calext);
                                        }
                                    }
                                    else
                                    {
                                        objCnltxt.MaxLength = 10;
                                        if (!bDynamicControlIndexChange)
                                        {
                                            CalendarExtender calext = new CalendarExtender();
                                            //DMS5-4101M replaced the space with empty string
                                            calext.ID = "cal_" + TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString().Replace(" ", string.Empty);
                                            calext.TargetControlID = TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString().Replace(" ", string.Empty);
                                            calext.Format = "MM/yyyy";
                                            TD2.Controls.Add(calext);
                                        }
                                    }

                                }
                                else
                                {
                                    if (Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iMax"].ToString()) != 0)
                                    {
                                        objCnltxt.MaxLength = Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iMax"].ToString());
                                    }
                                    else
                                    {
                                        objCnltxt.MaxLength = 30;
                                    }
                                }
                                Logger.Trace("Generatecontrols,Load all Textbox dynamic", Session["LoggedUserId"].ToString());

                            }
                            #endregion
                            #region dropdownlist control
                            if (string.Equals(TemplateDs.Tables[0].Rows[i]["TemplateFields_cObjType"].ToString(), "DropDownList", StringComparison.OrdinalIgnoreCase))
                            {
                                DropDownList objCnl = new DropDownList();
                                objCnl.AutoPostBack = Convert.ToBoolean(TemplateDs.Tables[0].Rows[i]["TemplateFields_bPostback"]);
                                //DMS5-4101M replaced the space with empty string
                                objCnl.ID = TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString().Replace(" ", string.Empty);
                                objCnl.CssClass = Getcssclass("drp");
                                objCnl.Height = Unit.Pixel(30);
                                objCnl.Width = Unit.Pixel(165);
                                int TemplatefldId = Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iId"]);
                                string Iaction = string.Empty;
                                if (TemplateDs.Tables[0].Rows[i]["TemplateFields_iParentId"].ToString() == "0")
                                {
                                    Iaction = "LoadMain";

                                    PopulateDropdown(objCnl, TemplatefldId, Iaction);//function to populate dropdownlist
                                }
                                else
                                {
                                    objCnl.Items.Insert(0, "--Select--");
                                }
                                objCnl.Attributes.Add("onChange", "javascript:SetHiddenVal('Dynamic');");


                                if (objCnl.AutoPostBack)
                                {
                                    objCnl.SelectedIndexChanged += new EventHandler(DynamicDropdownList_SelectedIndexChanged);
                                }

                                if (Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iParentId"]) == 0)
                                {
                                    TD2.Controls.Add(objCnl);
                                }
                                else
                                {
                                    TD4.Controls.Add(objCnl);
                                }

                                Logger.Trace("generatecontrols,Load all Dropdownlist dynamic", Session["LoggedUserId"].ToString());
                            }
                            #endregion


                            if (Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iParentId"]) == 0)
                            {
                                TR.Cells.Add(TD1);
                                TR.Cells.Add(TD2);
                            }

                            else
                            {
                                TR.Cells.Add(TD3);
                                TR.Cells.Add(TD4);
                            }

                            Tab.Rows.Add(TR);
                            Tab.Attributes.Add("CssClass", "tabledesign");
                            this.NumberOfControls++;
                            //For javascript validation
                            if (hdnIndexNames.Value.Length == 0)
                            {
                                hdnCountControls.Value = NumberOfControls.ToString();
                                hdnControlNames.Value = indexname;
                                hdnIndexNames.Value = TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString();
                                hdnIndexMinLen.Value = TemplateDs.Tables[0].Rows[i]["TemplateFields_iMin"].ToString();
                                hdnIndexType.Value = TemplateDs.Tables[0].Rows[i]["TemplateFields_vInputType"].ToString();
                                hdnIndexDataType.Value = TemplateDs.Tables[0].Rows[i]["TemplateFields_vDBType"].ToString();
                                hdnMandatory.Value = TemplateDs.Tables[0].Rows[i]["TemplateFields_bMandatory"].ToString().ToLower();//UMF - Add
                            }
                            else
                            {
                                hdnCountControls.Value = NumberOfControls.ToString();
                                hdnControlNames.Value = hdnControlNames.Value + '|' + indexname;
                                hdnIndexNames.Value = hdnIndexNames.Value + "|" + TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString();
                                hdnIndexType.Value = hdnIndexType.Value + "|" + TemplateDs.Tables[0].Rows[i]["TemplateFields_vInputType"].ToString();
                                hdnIndexDataType.Value = hdnIndexDataType.Value + "|" + TemplateDs.Tables[0].Rows[i]["TemplateFields_vDBType"].ToString();
                                if (!string.IsNullOrEmpty(TemplateDs.Tables[0].Rows[i]["TemplateFields_iMin"].ToString()) && Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iMin"].ToString()) != 0)
                                {
                                    hdnIndexMinLen.Value = hdnIndexMinLen.Value + "|" + TemplateDs.Tables[0].Rows[i]["TemplateFields_iMin"].ToString();
                                }
                                else
                                {
                                    hdnIndexMinLen.Value = hdnIndexMinLen.Value + "|" + "0";
                                }
                                hdnMandatory.Value = hdnMandatory.Value + "|" + TemplateDs.Tables[0].Rows[i]["TemplateFields_bMandatory"].ToString().ToLower();//UMF - Add
                            }



                        }
                        pnlIndexpro.Controls.Add(Tab);
                        pnlIndexpro.Visible = true;
                    }


                }
                catch (Exception ex)
                {
                    divMsg.Style.Add("display", "block");
                    divMsg.InnerHtml = CoreMessages.GetMessages(hdnAction.Value, "ERROR");
                    Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
                }
            }

        }
        protected void cmbDocumentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // UserBase loginUser = (UserBase)Session["LoggedUser"];
            bDynamicControlIndexChange = false;
            GetDepartments(hdnLoginOrgId.Value, hdnLoginToken.Value);
            UpdatePanel41.Update();
            pnlIndexpro.Controls.Clear();
            this.NumberOfControls = 0;
            txtRefid.Text = "";
            hdnCountControls.Value = "";
            hdnIndexMinLen.Value = "";
            hdnIndexNames.Value = "";
            hdnMandatory.Value = "";
            generatecontrols();
        }
        public void deletefile()
        {
            if (Convert.ToString(Session["pimgpath"]).Length > 0)
            {
                System.IO.File.Delete(Session["pimgpath"].ToString());

            }
            Session["pimgpath"] = "";
            Session["vimgpath"] = "";
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            deletefile();
            Response.Redirect("DocumentUpload.aspx");
        }
        private void ManageEdit()
        {
            string src = string.Empty;
            string TagEnabled = "false";
            string result = "Success";
            if (Session["TempFileLocation"] != null && Convert.ToString(Session["TempFileLocation"]).Length > 0)
            {
                btnSave.Enabled = true;
                btnCancel.Enabled = true;
                btnPreview.Enabled = false;
                //AsyncFileUpload1.Enabled = false;
                if (result == "Success")
                {
                    TagEnabled = "True";

                    src = GetSrc("Handler") + ConfigurationManager.AppSettings["TempWorkFolder"] + "\\" + Encryptdata(txtRefid.Text) + @"\1.pdf#toolbar=1";
                    hdnSrc.Value = GetSrc("Handler") + ConfigurationManager.AppSettings["TempWorkFolder"] + Encryptdata(txtRefid.Text) + @"\";
                    if (TagEnabled == "True")
                    {
                        if (IsPostBack)
                        {
                            NavigatePanel.Visible = true;
                        }
                        string count = ConfigurationManager.AppSettings["TempWorkFolder"] + Encryptdata(txtRefid.Text);
                        DirectoryInfo di = new DirectoryInfo(count);
                        int pagecount = di.GetFiles("*.pdf", SearchOption.AllDirectories).Length;
                        hdnPageCount.Value = pagecount.ToString();

                        ddlpagecount.Items.Clear();
                        for (int i = 0; i < pagecount; i++)
                        {
                            ddlpagecount.Items.Insert(i, (i + 1).ToString());
                        }
                    }
                    else
                    {
                        NavigatePanel.Visible = false;
                    }
                    upPagecount.Update();
                }
                else
                {
                    src = GetSrc("NotAvailable");
                }

                ///  frame1.Attributes.Add("src", src);

            }
            else if (Session["TempFileLocation"] != null && Convert.ToString(Session["TempFileLocation"]).Length > 0)
            {
                btnSave.Enabled = true;
                btnCancel.Enabled = true;
                //AsyncFileUpload1.Enabled = false;
                //src = GetSrc("Handler") + ConfigurationManager.AppSettings["TempWorkFolder"] + Encryptdata(txtRefid.Text) + Session["Count"].ToString() + ".tif";
                src = GetSrc("Handler") + ConfigurationManager.AppSettings["TempWorkFolder"] + "\\" + Encryptdata(txtRefid.Text) + @"\1.pdf#toolbar=1";
                hdnSrc.Value = GetSrc("Handler") + ConfigurationManager.AppSettings["TempWorkFolder"] + Encryptdata(txtRefid.Text) + @"\";
                // frame1.Attributes.Add("src", src);
            }
        }
        public void EditUploadedFile()
        {


            btnSave.Enabled = true;
            TemplateBL bl = new TemplateBL();
            UploadDocBL b2 = new UploadDocBL();
            SearchFilter filter = new SearchFilter();
            DataSet Ds = new DataSet();

            try
            {
                Logger.Trace("EditUploadedFile Started", Session["LoggedUserId"].ToString());
                string indexname = string.Empty;
                string sortOreder = string.Empty;
                bool IActivive;
                HtmlTable Tab = new HtmlTable();
                UserBase loginUser = (UserBase)Session["LoggedUser"];

                string action = "GetUploadDocumentDetails";
                filter.UploadDocID = Convert.ToInt32(Request.QueryString["id"]);
                Ds = b2.GetUploadDocumentDetails(filter, action, hdnLoginOrgId.Value, hdnLoginToken.Value);
                cmbDepartment.Items.Clear();
                cmbDepartment.Items.Add(new ListItem(Ds.Tables[0].Rows[0]["DeptName"].ToString(), Ds.Tables[0].Rows[0]["DepID"].ToString()));
                cmbDepartment.Enabled = false;
                cmbDocumentType.Items.Clear();

                cmbDocumentType.Items.Add(new ListItem(Ds.Tables[0].Rows[0]["DocName"].ToString(), Ds.Tables[0].Rows[0]["DocID"].ToString()));
                cmbDocumentType.Enabled = false;
                ViewState["Version"] = Convert.ToInt32(Ds.Tables[0].Rows[0]["DocVersion"]);
                //  version = Convert.ToInt32(Ds.Tables[0].Rows[0]["DocVersion"]);
                txtKeyword.Text = Ds.Tables[0].Rows[0]["Keywords"].ToString();
                Session["EncyptImgName"] = Ds.Tables[0].Rows[0]["RefId"].ToString();

                filter.CurrOrgId = loginUser.LoginOrgId;
                filter.CurrUserId = loginUser.UserId;
                filter.DocumentTypeName = cmbDocumentType.SelectedItem.ToString();
                filter.DepartmentID = Convert.ToInt32(cmbDepartment.SelectedValue);
                DataSet TemplateDs = new TemplateBL().GetTemplateDetails(filter);

                txtRefid.Text = Ds.Tables[0].Rows[0]["RefId"].ToString();
                if (cmbDepartment.SelectedValue == "0" || cmbDocumentType.SelectedValue == "0")
                {
                    pnlIndexpro.Controls.Clear();
                    this.NumberOfControls = 0;
                    txtRefid.Text = "";
                    hdnCountControls.Value = "";
                    hdnIndexMinLen.Value = "";
                    hdnIndexNames.Value = "";
                    hdnMandatory.Value = "";
                }
                else
                {
                    hdnIndexNames.Value = "";
                    hdnIndexType.Value = "";
                    hdnIndexDataType.Value = "";
                    hdnIndexMinLen.Value = "";
                    hdnIndexMinLen.Value = "";
                    hdnMandatory.Value = "";
                    hdnCountControls.Value = "";

                    int ControlsCounter = -1; // if the control counter is even bind controls parallelly, i.e., 4 columns :[ lable:control label:control ] other wise 2 columns style
                    HtmlTableRow TR = new HtmlTableRow();
                    for (int i = 0; i < TemplateDs.Tables[0].Rows.Count; i++)
                    {
                        IActivive = Convert.ToBoolean(TemplateDs.Tables[0].Rows[i]["TemplateFields_bActive"]);
                        indexname = TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString();
                        ++ControlsCounter; // Comment this line to bind controls in 2 coulumns stlye                            
                        if (Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iParentId"]) == 0)
                            TR = new HtmlTableRow();
                        HtmlTableCell TD1 = new HtmlTableCell();
                        HtmlTableCell TD2 = new HtmlTableCell();

                        TD1.Width = "150px";
                        TD2.Width = "150px";
                        // Parellel controls binding
                        HtmlTableCell TD3 = new HtmlTableCell();
                        HtmlTableCell TD4 = new HtmlTableCell();

                        TD3.Width = "150px";
                        TD4.Width = "150px";

                        // Add label text
                        if (!string.Equals(TemplateDs.Tables[0].Rows[i]["TemplateFields_cObjType"].ToString(), "Label", StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(TemplateDs.Tables[0].Rows[i]["TemplateFields_cObjType"].ToString(), "Button", StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(TemplateDs.Tables[0].Rows[i]["TemplateFields_cObjType"].ToString(), "HiddenField", StringComparison.OrdinalIgnoreCase) && Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iParentId"]) == 0)
                        {
                            Label objCnl = new Label();
                            objCnl.ID = "lbl" + i;
                            objCnl.Text = TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString();
                            objCnl.CssClass = Getcssclass("lbl");
                            objCnl.AssociatedControlID = "";
                            if (Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iParentId"]) == 0)
                            {
                                TD1.Controls.Add(objCnl);
                            }
                            else
                                TD3.Controls.Add(objCnl);
                        }
                        #region textbox control
                        if (string.Equals(TemplateDs.Tables[0].Rows[i]["TemplateFields_cObjType"].ToString(), "TextBox", StringComparison.OrdinalIgnoreCase))
                        {
                            TextBox objCnltxt = new TextBox();
                            objCnltxt.AutoPostBack = Convert.ToBoolean(TemplateDs.Tables[0].Rows[i]["TemplateFields_bPostback"]);
                            objCnltxt.ID = TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString().Replace(" ", string.Empty);
                            objCnltxt.CssClass = Getcssclass("txt");
                            TD2.Controls.Add(objCnltxt);
                            objCnltxt.Width = Unit.Pixel(157);
                            objCnltxt.Height = Unit.Pixel(16);
                            objCnltxt.Text = Ds.Tables[0].Rows[0][TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString()].ToString();

                            if (TemplateDs.Tables[0].Rows[i]["TemplateFields_vDBType"].ToString() == "Integer")
                            {
                                objCnltxt.MaxLength = Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iMax"].ToString());
                                objCnltxt.Attributes.Add("OnKeyPress", "javascript:return CheckNumericKey(event);");
                            }
                            
                            else if (TemplateDs.Tables[0].Rows[i]["TemplateFields_vDBType"].ToString() == "Boolen")
                            {
                                objCnltxt.MaxLength = 1;
                                objCnltxt.Attributes.Add("OnKeyPress", "javascript:return CheckBoolean(event);");
                            }
                            else if (TemplateDs.Tables[0].Rows[i]["TemplateFields_vDBType"].ToString() == "DateTime")
                            {
                                objCnltxt.MaxLength = 10;
                                objCnltxt.Attributes.Add("OnKeyPress", "javascript:return CheckDateFormat(event.keyCode,'" + TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString().Replace(" ", string.Empty) + "');");
                                CalendarExtender calext = new CalendarExtender();
                                calext.ID = "cal_" + TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString().Replace(" ", string.Empty);
                                calext.TargetControlID = TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString().Replace(" ", string.Empty);
                                calext.Format = "dd/MM/yyyy";
                                calext.CssClass = "";
                                TD2.Controls.Add(calext);

                            }
                            else
                            {
                                if (Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iMax"].ToString()) != 0)
                                {
                                    objCnltxt.MaxLength = Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iMax"].ToString());
                                }
                                else
                                {
                                    objCnltxt.MaxLength = 30;
                                }
                            }
                            Logger.Trace("EditUploadedFile,Load all Textbox dynamic", Session["LoggedUserId"].ToString());

                        }
                        #endregion
                        #region dropdownlist control
                        if (string.Equals(TemplateDs.Tables[0].Rows[i]["TemplateFields_cObjType"].ToString(), "DropDownList", StringComparison.OrdinalIgnoreCase))
                        {
                            DropDownList objCnl = new DropDownList();
                            objCnl.AutoPostBack = Convert.ToBoolean(TemplateDs.Tables[0].Rows[i]["TemplateFields_bPostback"]);
                            objCnl.ID = TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString().Replace(" ", string.Empty);
                            objCnl.CssClass = Getcssclass("drp");
                            objCnl.Height = Unit.Pixel(30);
                            objCnl.Width = Unit.Pixel(165);

                            int TemplatefldId = Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iId"]);
                            string Iaction = string.Empty;
                            Iaction = "LoadMain";
                            if (hdnMainvalueid.Value != "")
                            {
                                TemplatefldId = System.Convert.ToInt16(hdnMainvalueid.Value);
                                Iaction = "LoadSub";
                                hdnMainvalueid.Value = "";
                            }


                            PopulateDropdown(objCnl, TemplatefldId, Iaction);//function to populate dropdownlist
                            //     PopulateDropdown(objCnl, TemplatefldId);//function to populate dropdownlist
                            objCnl.SelectedIndex = objCnl.Items.IndexOf(objCnl.Items.FindByText(Ds.Tables[0].Rows[0][TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString()].ToString()));
                            if (objCnl.AutoPostBack)
                            {
                                objCnl.SelectedIndexChanged += new EventHandler(DynamicDropdownList_SelectedIndexChanged);
                            }
                            if (Convert.ToBoolean(TemplateDs.Tables[0].Rows[i]["TemplateFields_bHaschild"]))
                            {
                                hdnMainvalueid.Value = objCnl.SelectedValue;
                            }
                            if (Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iParentId"]) == 0)
                            {
                                TD2.Controls.Add(objCnl);
                            }
                            else
                            {
                                TD4.Controls.Add(objCnl);
                            }

                            Logger.Trace("EditUploadedFile,Load all Dropdownlist dynamic", Session["LoggedUserId"].ToString());
                        }
                        #endregion


                        if (Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iParentId"]) == 0)
                        {
                            TR.Cells.Add(TD1);
                            TR.Cells.Add(TD2);
                        }

                        else
                        {
                            TR.Cells.Add(TD3);
                            TR.Cells.Add(TD4);
                        }

                        Tab.Rows.Add(TR);
                        Tab.Attributes.Add("CssClass", "tabledesign");
                        //For javascript validation
                        if (hdnIndexNames.Value.Length == 0)
                        {
                            hdnControlNames.Value = indexname;
                            hdnIndexNames.Value = TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString();
                            hdnIndexMinLen.Value = TemplateDs.Tables[0].Rows[i]["TemplateFields_iMin"].ToString();
                            hdnIndexType.Value = TemplateDs.Tables[0].Rows[i]["TemplateFields_vInputType"].ToString();
                            hdnIndexDataType.Value = TemplateDs.Tables[0].Rows[i]["TemplateFields_vDBType"].ToString();
                            hdnMandatory.Value = TemplateDs.Tables[0].Rows[i]["TemplateFields_bMandatory"].ToString().ToLower();//UMF - Add
                        }
                        else
                        {
                            hdnControlNames.Value = hdnControlNames.Value + '|' + indexname;
                            hdnIndexNames.Value = hdnIndexNames.Value + "|" + TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString();
                            hdnIndexType.Value = hdnIndexType.Value + "|" + TemplateDs.Tables[0].Rows[i]["TemplateFields_vInputType"].ToString();
                            hdnIndexDataType.Value = hdnIndexDataType.Value + "|" + TemplateDs.Tables[0].Rows[i]["TemplateFields_vDBType"].ToString();
                            if (!string.IsNullOrEmpty(TemplateDs.Tables[0].Rows[i]["TemplateFields_iMin"].ToString()) && Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iMin"].ToString()) != 0)
                            {
                                hdnIndexMinLen.Value = hdnIndexMinLen.Value + "|" + TemplateDs.Tables[0].Rows[i]["TemplateFields_iMin"].ToString();
                            }
                            else
                            {
                                hdnIndexMinLen.Value = hdnIndexMinLen.Value + "|" + "0";
                            }
                            hdnMandatory.Value = hdnMandatory.Value + "|" + TemplateDs.Tables[0].Rows[i]["TemplateFields_bMandatory"].ToString().ToLower();//UMF - Add
                        }



                    }
                    pnlIndexpro.Controls.Add(Tab);
                    pnlIndexpro.Visible = true;
                    ManageEdit();
                }

                Logger.Trace("EditUploadedFile completed", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                divMsg.Style.Add("display", "block");
                divMsg.InnerHtml = CoreMessages.GetMessages(hdnAction.Value, "ERROR");
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());

            }
        }
        class KeyFrequency
        {
            public string Key { get; set; }
            public Int32 Frequency { get; set; }

            public KeyFrequency(string key, Int32 count)
            {
                Key = key;
                Frequency = count;
            }
        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                Logger.Trace("btnSubmit_Click Started", Session["LoggedUserId"].ToString());
                if (cmbDepartment.SelectedValue != "0" && cmbDocumentType.SelectedValue != "0")
                {

                    if (Session["EncyptImgName"] != null && afterdot(Session["EncyptImgName"].ToString()).Length > 0)
                    {
                        switch (afterdot(Session["EncyptImgName"].ToString()).ToLower())
                        {
                            case ".jpg":
                            case ".bmp":
                            case ".jpeg":
                            case ".png":
                            case ".gif":
                            case ".giff":
                            case ".tif":
                            case ".tiff":
                            case ".pdf":
                                Button1.Text = "Keywords Not Available for This Document";
                                Button1.CommandArgument = null;
                                Button2.Visible = false;
                                Button3.Visible = false;
                                Button4.Visible = false;
                                Button5.Visible = false;
                                Button6.Visible = false;
                                Button7.Visible = false;
                                Button8.Visible = false;
                                Button9.Visible = false;
                                Button10.Visible = false;
                                break;
                            case ".doc":
                            case ".docx":
                            case ".xls":
                            case ".xlsx":
                            case ".ppt":
                            case ".pptx":
                                TodayDate = DateTime.Today.ToString("yyyy-MM-dd");
                                TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"];
                                TextReader reader = new FilterReader(TempFolder + Session["EncyptImgName"]);
                                StringBuilder Keywords = new StringBuilder();
                                using (reader)
                                {
                                    Keywords = Keywords.Append(reader.ReadToEnd());
                                }
                                //remove common words
                                string[] removablewords = { ":", ".", "~", "!", "@", "#", "$", "%", "^", "&", "\v", "\\", "*", "(", ")", "_", "-", "+", "=", "<", ">", "?", "/", @"\", "|", "`", "'", " after ", " use ", " two ", " how ", " our ", " work ", " first ", " well ", " way ", " even ", " new ", " want ", " because ", " any ", " these ", " give ", " day ", " most ", " us ", " people ", " into ", " year ", " your ", " good ", " some ", " could ", " them ", " see ", " other ", " than ", " then ", " now ", " look ", " only ", " come ", " its ", " over ", " think ", " also ", " so ", " up ", " out ", " if ", " about ", " who ", " get ", " which ", " go ", " me ", " when ", " make ", " can ", " like ", " time ", " no ", " just ", " him ", " know ", " take ", " this ", " but ", " his ", " by ", " from ", " they ", " we ", " say ", " her ", " she ", " or ", " an ", " will ", " my ", " one ", " all ", " would ", " there ", " their ", " what ", " the ", " be ", " to ", " of ", " and ", " a ", " in ", " that ", " have ", " I ", " it ", " for ", " not ", " on ", " with ", " he ", " as ", " you ", " do ", " at ", " is ", " are ", " following " };
                                foreach (string st in removablewords)
                                {
                                    Keywords.Replace(st, " ");
                                }

                                //Reomve unwated spaces
                                while (Keywords.ToString().Contains("  "))
                                {
                                    Keywords.Replace("  ", " ");
                                }
                                string str = Keywords.ToString();
                                Keywords.Clear();
                                Keywords.Append("<words><s>" + str.Replace(" ", "</s><s>") + "</s></words>");
                                string xml = Keywords.ToString();
                                XElement items = XElement.Parse(xml);
                                var groups = from t in items.Descendants("s")
                                             group t by t.Value.ToLower() into g
                                             select new KeyFrequency(g.Key, g.Count());
                                groups = groups.OrderByDescending(g => g.Frequency).Take(15);

                                keyvalues = new List<string>();
                                foreach (KeyFrequency g in groups)
                                {
                                    keyvalues.Add(g.Key);
                                }

                                for (key = 0; key < keyvalues.Count && key < 10; key++)
                                {
                                    Button btn = (Button)pnlKeywords.FindControl("Button" + Convert.ToString(key + 1));
                                    btn.Visible = true;
                                    btn.Text = keyvalues[key].Replace("\n", " ");
                                    btn.CommandArgument = keyvalues[key];
                                }
                                if (key < 10)
                                {
                                    int oldkey = 0;
                                    oldkey = key;
                                    for (key = oldkey; key < 10; key++)
                                    {
                                        Button btn = (Button)pnlKeywords.FindControl("Button" + Convert.ToString(key + 1));
                                        btn.Visible = false;
                                    }
                                }
                                else
                                {
                                    AsyncFileUpload1.BackColor = System.Drawing.Color.Red;
                                }
                                break;
                            default:
                                break;
                        }
                    }

                }
                Logger.Trace("btnSubmit_Click Finished", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());

                Button1.Text = "Keywords Not Available for This Document";
                Button1.CommandArgument = null;
                Button2.Visible = false;
                Button3.Visible = false;
                Button4.Visible = false;
                Button5.Visible = false;
                Button6.Visible = false;
                Button7.Visible = false;
                Button8.Visible = false;
                Button9.Visible = false;
                Button10.Visible = false;

            }

        }
        #region viewer helper methods
        protected void btnCallFromJavascript_Click(object sender, EventArgs e)
        {
            LoadImageToViewer(hdnAction.Value);
        }
        private string convertPDFToImage(string Filepath, string EncryptedFilepath, int RequestingPageNo)
        {
            string CurrentFilepath = string.Empty, filepath = string.Empty;

            try
            {
                string TempFolder = string.Empty;
                ConvertPdf2Image obj = new ConvertPdf2Image();
                if (Session["TempFolder"] != null)
                {
                    TempFolder = Session["TempFolder"].ToString();
                    TempFolder += EncryptedFilepath + "JPG";
                }

                //CurrentFilepath = obj.GetJpegPageFromPdf(Filepath, TempFolder, RequestingPageNo.ToString());

                // Get watermark for the document from session
                string Watermark = Session["Watermark"] != null ? Session["Watermark"].ToString() : string.Empty;
                if (Watermark.Length > 0)
                {
                    Logger.Trace("Converting PDF to JPEG along with watermark. Watermark text: " + Watermark, Session["LoggedUserId"].ToString());
                    // Convert pdf to JPEG and apply watermark
                    CurrentFilepath = obj.ConvertPDFtoHojas(Filepath, TempFolder, RequestingPageNo.ToString(), Watermark);
                }
                else
                {
                    Logger.Trace("Converting PDF to JPEG without applying watermark.", Session["LoggedUserId"].ToString());
                    // Convert pdf to JPEG 
                    CurrentFilepath = obj.ConvertPDFtoHojas(Filepath, TempFolder, RequestingPageNo.ToString(), Watermark);
                }

            }
            catch (Exception ex)
            {
                Logger.Trace("Exception: " + ex.Message, Session["LoggedUserId"].ToString());

            }
            return CurrentFilepath;
        }
        private void LoadImageToViewer(string Action)
        {
            int pagecount = Convert.ToInt32(Session["Pagecount"]);
            string src = string.Empty;

            int CurrentPage = 1;

            if (hdnPageNo.Value.Length > 0)
                CurrentPage = Convert.ToInt32(hdnPageNo.Value);

            Logger.Trace("Setting image to viewer. Action: " + hdnAction.Value.ToString() + " Page count: " + pagecount + " Current Page:" + CurrentPage, Session["LoggedUserId"].ToString());

            int PageNo = 0;

            switch (Action.ToUpper())
            {
                case "NEXT":
                    PageNo = CurrentPage + 1;
                    break;
                case "PREVIOUS":
                    PageNo = CurrentPage - 1;
                    break;
                case "GOTO":
                    PageNo = Convert.ToInt32(ddlpagecount.SelectedValue);
                    break;
                case "FIRST":
                    PageNo = 1;
                    break;
                case "LAST":
                    PageNo = pagecount;
                    break;
                default:
                    PageNo = CurrentPage;
                    break;
            }

            if (PageNo > 0 && PageNo <= pagecount)
            {
                hdnPageNo.Value = PageNo.ToString();
                ddlpagecount.SelectedValue = PageNo.ToString();
            }
            else
                PageNo = CurrentPage;

            string PdfPath = Session["TempFolder"].ToString() + beforedot(Session["EncyptImgName"].ToString()) + "\\" + PageNo.ToString() + ".pdf";
            string ImageName = beforedot(Session["EncyptImgName"].ToString());

            Logger.Trace("Converting PDF to JPEG. PDF Path: " + PdfPath + " Image Name: " + ImageName + " Page No: " + PageNo, Session["LoggedUserId"].ToString());
            string CurrentFilepath = convertPDFToImage(PdfPath, ImageName, PageNo);

            if (CurrentFilepath.Length > 0)
            {
                myiframe.Attributes.Add("src", "");
                string src1 = GetSrc("Handler") + PdfPath + "#scrollbar=1&toolbar=0&statusbar=0&messages=0&navpanes=1&view=fitH,100;readonly=true;disableprint=true;";
                myiframe.Attributes.Add("src", src1);


                //src = GetSrc("Handler") + CurrentFilepath;
                //src = src.Replace("\\", "//").ToLower();
                        
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "loadImageAndAnnotations", "loadImageAndAnnotations('" + src + "');", true);
                Logger.Trace("Setting image to viewer using RegisterStartupScript: " + src, Session["LoggedUserId"].ToString());
                //PDFViewer1.Load += new EventHandler(PDFViewer1.JustPostbakUserControl);
            }
            else
            {
                Logger.Trace("convertPDFToImage method returned file path as empty.", Session["LoggedUserId"].ToString());
            }
        }
        #endregion
        protected void Button1_Click(object sender, EventArgs e)
        {
            cadpreview = "true";
            // string file = Path.GetTempPath() + Guid.NewGuid().ToString() + "_" + FileUpload1.FileName;
            // FileUpload1.SaveAs(file);

            hdnPreviewCheckBoxChecked.Value = "DWG";
            TodayDate = DateTime.Today.ToString("yyyy-MM-dd");
            string TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"];

            if (!Directory.Exists(TempFolder))
            {
                Directory.CreateDirectory(TempFolder);
            }

            if (Convert.ToString(Session["EncyptImgName"]).Length > 0)
            {
                try
                {
                    File.Delete(TempFolder + Convert.ToString(Session["EncyptImgName"]));
                    File.Delete(TempFolder + beforedot(Convert.ToString(Session["EncyptImgName"])) + ".pdf");
                }
                catch (Exception ex)
                {
                    Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
                }
            }

            if (FileUpload1.HasFile)
            {
                AfterDot = afterdot(FileUpload1.FileName);
                FileUpload1.SaveAs(TempFolder + Encryptdata(txtRefid.Text) + AfterDot);
                Session["TempFolder"] = TempFolder;
                Session["EncyptImgName"] = Encryptdata(txtRefid.Text) + AfterDot;
                Session["FileName"] = FileUpload1.FileName;
            }
            Session["PageCount"] = 1;
            Hidcontentlength.Value = Convert.ToString(FileUpload1.PostedFile.ContentLength);
            Hidconttype.Value = FileUpload1.PostedFile.ContentType;

          //  pdfviewer.Style["visibility"] = "hidden";
            Cadviewer.Style["visibility"] = "visible";
            string query = Convert.ToString(Request.QueryString["id"]);

            myiframe.Attributes["src"] = "http://dms.writercorporation.com/CAD/Default.aspx?VF_field1=" + Request.QueryString["id"];

        }
        protected void RadioButtonList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RadioButtonList1.SelectedValue == "PDFFiles")
            {
                PDF.Style["visibility"] = "visible";
                DWG.Style["visibility"] = "hidden";

               // pdfviewer.Style["visibility"] = "visible";
                Cadviewer.Style["visibility"] = "hidden";

            }
            if (RadioButtonList1.SelectedValue == "DWGFiles")
            {

                DWG.Style["visibility"] = "visible";
                PDF.Style["visibility"] = "hidden";

              //  pdfviewer.Style["visibility"] = "hidden";
                Cadviewer.Style["visibility"] = "visible";
            }

        }

    }
}
