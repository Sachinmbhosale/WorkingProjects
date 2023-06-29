using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lotex.EnterpriseSolutions.CoreBL;
using Lotex.EnterpriseSolutions.CoreBE;
using System.Data;
using System.IO;
using System.Configuration;
using OfficeConverter;


namespace Lotex.EnterpriseSolutions.WebUI.Secure.Core
{
    public partial class DigitalSignature : PageBase
    {
        //creating  a temp table and keeping in Viewstate
        public void Createtemptable()
        {
            DataTable DT = new DataTable();
            DT.Columns.Add("UploadDocID", typeof(int));
            ViewState["SignatureDT"] = DT;
        }

        protected int currentPageNumber = 1;

        protected int PAGE_SIZE;

        protected void Page_Load(object sender, EventArgs e)
        {
            PDFViewer1.AnnotationToolbarVisible = false;
            CheckAuthentication();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
            hdnLoginToken.Value = loginUser.LoginToken;
            if (!IsPostBack)
            {


                Createtemptable();
                hdnSearchAction.Value = Request.QueryString["Action"] == null ? "" : Request.QueryString["Action"];
                hdnSearchCriteria.Value = Request.QueryString["parms"] == null ? "" : Request.QueryString["parms"];
                BatchUploadBL bal = new BatchUploadBL();
                DataSet ds = bal.ComboFillerBySP("USP_COMBOFILLER_PAGE_SIZE");
                ddlRows.DataTextField = "_Name";
                ddlRows.DataValueField = "_ID";
                ddlRows.DataSource = ds;
                ddlRows.DataBind();
                ddlRows.SelectedValue = "10";
                PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);
                LoadGrid(1);

            }
            PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);
            ManageButtons();
        }

        /// <summary>
        /// For filtering the value from the grid//
        /// </summary>
        /// <param name="DT"></param>
        /// <returns></returns>
        protected DataTable FilterGridData(DataTable DT)
        {
            DataTable iDt = new DataTable();
            try
            {
                Logger.Trace("FilterGridData Started:-", Session["LoggedUserId"].ToString());

                if (DT.Rows.Count > 0)
                {


                    if (DDLSignedStatus.SelectedItem.Text == "Digitally Signed")
                    {
                        DataView cdv = new DataView(DT);
                        cdv.RowFilter = ("[Digitally Signed] = 'Yes'");
                        iDt = cdv.ToTable();
                    }
                    else if (DDLSignedStatus.SelectedItem.Text == "Not Digitally Signed")
                    {
                        DataView cdv = new DataView(DT);
                        cdv.RowFilter = ("[Digitally Signed] = 'No'");
                        iDt = cdv.ToTable();
                    }


                }
                Logger.Trace("FilterGridData Completed:-", Session["LoggedUserId"].ToString());
            }

            catch (Exception ex)
            {

                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
            return iDt;
        }

        /// <summary>
        /// To load the grid with values
        /// </summary>
        /// <param name="pagenumber"></param>
        public void LoadGrid(int pagenumber)
        {
            try
            {
                Logger.Trace("LoadGrid Started:-", Session["LoggedUserId"].ToString());
                //To get the data and load the grid view//
                Results results = new Results();
                string[] userdata = null;
                SearchFilter filter = new SearchFilter();
                DocumentBL bl = new DocumentBL();

                userdata = hdnSearchCriteria.Value.Split('|');
                filter.SearchOption = userdata[0];
                filter.DocumentTypeID = Convert.ToInt32(userdata[1]);
                filter.DepartmentID = Convert.ToInt32(userdata[2]);
                filter.Refid = userdata[3].Trim();
                filter.keywords = userdata[4].Trim();
                filter.Active = Convert.ToInt32(userdata[5].Trim());
                filter.MainTagID = Convert.ToInt32(userdata[6].Trim());
                filter.SubTagID = Convert.ToInt32(userdata[7].Trim());
                filter.DocPageNo = Convert.ToInt32(userdata[8].Trim());
                filter.RowsPerPage = Convert.ToInt32(ddlRows.SelectedItem.Text);
                filter.DocPageNo = pagenumber;
                if (DDLSignedStatus.SelectedItem.Text == "Digitally Signed")
                {
                    filter.WhereClause = userdata[10] + "AND ( UPLOAD_vType = 'application/pdf' ) AND ( UPLOAD_bIsDigitallySigned = 1 )";
                }
                else if (DDLSignedStatus.SelectedItem.Text == "Not Digitally Signed")
                {
                    filter.WhereClause = userdata[10] + "AND ( UPLOAD_vType = 'application/pdf' ) AND ( UPLOAD_bIsDigitallySigned = 0 )";
                }
                //  filter.WhereClause = userdata[10] + "AND ( UPLOAD_vType = 'application/pdf' )";
                results = bl.SearchDocuments(filter, hdnSearchAction.Value, hdnLoginOrgId.Value, hdnLoginToken.Value);
                grdView.DataSource = FilterGridData(results.ResultDS.Tables[0]);
                grdView.DataBind();
                if (results.ResultDS.Tables[0].Rows.Count > 0)
                {
                    ManagePaging(Convert.ToInt32(results.ResultDS.Tables[0].Rows[0]["TotalRows"]));
                }

                Logger.Trace("LoadGrid Completed:-", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {

                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }

        }
        /// <summary>
        /// Manage the disabling and enabling of apply and remove signature According to the status.
        /// </summary>
        protected void ManageButtons()
        {
            try
            {

                Logger.Trace("ManageButtons Started:-", Session["LoggedUserId"].ToString());
                if (DDLSignedStatus.SelectedItem.Text == "Digitally Signed")
                {
                    btnAddDigitalSignature.Enabled = false;
                    btnRemoveDigitalSignature.Enabled = true;
                }
                else if (DDLSignedStatus.SelectedItem.Text == "Not Digitally Signed")
                {
                    btnAddDigitalSignature.Enabled = true;
                    btnRemoveDigitalSignature.Enabled = false;
                }

                Logger.Trace("ManageButtons Completed:-", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }
        //Toload Image to viewer
        private void LoadImageToViewer(string Action)
        {
            int pagecount = 0;

            if (hdnpagecount.Value != "" || hdnpagecount.Value != string.Empty)
            {
                pagecount = Convert.ToInt32(hdnpagecount.Value);
            }
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
                default:
                    PageNo = CurrentPage;
                    break;
            }

            if (PageNo > 0 && PageNo <= pagecount)
            {
                hdnPageNo.Value = PageNo.ToString();
            }
            else
                PageNo = CurrentPage;


            string PdfPath = Session["TempFolder"].ToString() + beforedot(hdnEncrpytFileName.Value.ToString()) + "\\" + PageNo.ToString() + ".pdf";
            string ImageName = hdnEncrpytFileName.Value.ToString();

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
        }

        /// <summary>
        /// To handle Viewer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCallFromJavascript_Click(object sender, EventArgs e)
        {
            Logger.Trace("btnCallFromJavascript_Click Started:-", Session["LoggedUserId"].ToString());
            try
            {
                LoadImageToViewer(hdnAction.Value);
            }
            catch(Exception ex)
            {

                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
            Logger.Trace("btnCallFromJavascript_Click Completed:-", Session["LoggedUserId"].ToString());
        }

        /// <summary>
        /// Converting PDF to image
        /// </summary>
        /// <param name="Filepath"></param>
        /// <param name="EncryptedFilepath"></param>
        /// <param name="RequestingPageNo"></param>
        /// <returns></returns>
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

        /// <summary>
        /// grid handling
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grdView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                Logger.Trace("grdView_RowDataBound Started:-", Session["LoggedUserId"].ToString());
                for (int i = 1; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[3].Visible = false;
                    e.Row.Cells[4].Visible = false;
                    e.Row.Cells[5].Visible = false;
                    e.Row.Cells[6].Visible = false;
                    e.Row.Cells[8].Visible = false;
                    e.Row.Cells[11].Visible = false;
                    e.Row.Cells[13].Visible = false;
                }
                DataTable DT = (DataTable)ViewState["SignatureDT"];
                CheckBox chkbox = (CheckBox)e.Row.FindControl("chkSelected");

                if (DT.Rows.Count > 0 && DT != null)
                {
                    foreach (DataRow row in DT.Rows)
                    {
                        if (e.Row.Cells[8].Text == row["UploadDocID"].ToString())
                        {
                            chkbox.Checked = true;
                        }
                    }
                }
                Logger.Trace("grdView_RowDataBound Completed:-", Session["LoggedUserId"].ToString());
            }
            catch(Exception ex)
            {

                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
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
                catch (Exception)
                {
                    continue;
                }
            }
        }

        //for copying files to the temporary folder//
        private void CopyFileToTempfolder()
        {
            string TempFolder = ConfigurationManager.AppSettings["TempWorkFolder"];
            Session["TempFolder"] = TempFolder;
            if (!Directory.Exists(TempFolder))
            {
                Directory.CreateDirectory(TempFolder);
            }
            string filepath = beforedot(hdnFileLocation.Value).ToLower();

            if (Directory.Exists(filepath.ToLower()))
            {
                string sourceFile = filepath;
                string destinationFile = TempFolder + beforedot(hdnEncrpytFileName.Value);
                try
                {
                    copyDirectory(sourceFile, destinationFile);
                }
                catch (Exception ex)
                {
                    Logger.Trace("loadfileiniframe Exception:" + ex.Message, Session["LoggedUserId"].ToString());
                }

                if (Directory.Exists(destinationFile.ToLower()))
                {
                    DirectoryInfo di = new DirectoryInfo(destinationFile);
                    int count = di.GetFiles("*.pdf", SearchOption.AllDirectories).Length;
                    hdnpagecount.Value = count.ToString();
                }
            }
        }

        protected void grdView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                Logger.Trace("grdView_RowCommand Started:-", Session["LoggedUserId"].ToString());
                int index = Convert.ToInt32(e.CommandArgument.ToString());
                int ProcessId = Convert.ToInt32(grdView.Rows[index].Cells[8].Text);
                UploadDocBL b2 = new UploadDocBL();
                SearchFilter filter = new SearchFilter();
                DataSet Ds = new DataSet();
                string action = "GetUploadDocumentDetailsForDownload";
                filter.UploadDocID = ProcessId;

                Ds = b2.GetUploadDocumentDetails(filter, action, hdnLoginOrgId.Value, hdnLoginToken.Value);
                DataRow currentRow = Ds.Tables[0].Rows[0];
                if (Ds != null && Ds.Tables.Count > 0 && Ds.Tables[0].Rows.Count > 0)
                {
                    hdnFileLocation.Value = currentRow["phyFilepath"].ToString();
                    hdnEncrpytFileName.Value = currentRow["EncrDocName"].ToString();
                    CopyFileToTempfolder();
                    LoadImageToViewer(string.Empty);
                }
                Logger.Trace("grdView_RowCommand Completed:-", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }
        /// <summary>
        /// for validating the certificate
        /// </summary>
        /// <param name="CerFilepath"></param>
        /// <returns></returns>
        protected bool ValidateCertificate(string CerFilepath)
        {
            bool Valid = false;
            Cert myCert = null;
            try
            {
                Logger.Trace("ValidateCertificate Started:-", Session["LoggedUserId"].ToString());
                myCert = new Cert(CerFilepath, txtpassword.Text.ToString());
                Valid = true;
                Logger.Trace("ValidateCertificate Completed:-", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
            return Valid;

        }
        /// <summary>
        /// Manage Paging Filling the Dropdown
        /// </summary>
        /// <param name="totalRows"></param>
        protected void ManagePaging(int totalRows)
        {
            try
            {
                Logger.Trace("ManagePaging Started:-", Session["LoggedUserId"].ToString());
                lblTotalPages.Text = GetTotalPages(totalRows).ToString();

                ddlPage.Items.Clear();
                for (int i = 1; i < Convert.ToInt32(lblTotalPages.Text) + 1; i++)
                {
                    ddlPage.Items.Add(new ListItem(i.ToString()));
                }

                ddlPage.SelectedValue = currentPageNumber.ToString();
                Logger.Trace("ManagePaging Completed:-", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }

        }

        //For Create and manage XML
        protected string CreateXML()
        {
            string xml = "<Signature>";
            try
            {
                Logger.Trace("CreateXML Started:-", Session["LoggedUserId"].ToString());
                DataTable DT = (DataTable)ViewState["SignatureDT"];
                UserBase loginUser = (UserBase)Session["LoggedUser"];

                string UploadId = string.Empty;
              

                string Password = txtpassword.Text;
                string Author = txtauther.Text;
                string Title = txtTitle.Text;
                string Subject = txtsubject.Text;
                string Keywords = txtkeywods.Text;
                string Creator = txtcreater.Text;
                string Producer = txtproducer.Text;
                string Reason = txtReason.Text;
                string Contact = txtcontact.Text;
                string Location = txtlocation.Text;
                string Createdby = loginUser.UserId.ToString();
                string Isvisible = chkvisibleSignature.Checked == true ? "1" : "0";



                for (int i = DT.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = DT.Rows[i];
                    UploadId = dr["UploadDocID"].ToString();
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
                }
                xml += "</Signature>";
                Logger.Trace("CreateXML Completed:-", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
            return xml;

        }

        //For Adding Signature to the DB//
        protected void btnadd_Click(object sender, EventArgs e)
        {
            try
            {

                Logger.Trace("btnadd_Click Started:-", Session["LoggedUserId"].ToString());
                if (ValidateCertificate(Session["TempCertficatePath"].ToString()))
                {
                    string action = "InsertDigitalSignature";
                    string OrginalCertificateFilePath = string.Empty;
                    UserBase loginUser = (UserBase)Session["LoggedUser"];
                    string CertificateFolder = System.Configuration.ConfigurationManager.AppSettings["CertificateFolder"];
                    if (!Directory.Exists(CertificateFolder))
                    {
                        Directory.CreateDirectory(CertificateFolder);
                    }
                    string CertificateFinalFolder = CertificateFolder + loginUser.UserOrgId.ToString() + "\\" + loginUser.UserId.ToString() + "\\";
                    if (!Directory.Exists(CertificateFinalFolder))
                    {
                        Directory.CreateDirectory(CertificateFinalFolder);
                    }
                    OrginalCertificateFilePath = CertificateFinalFolder + Session["CertficateName"].ToString();
                    File.Copy(Session["TempCertficatePath"].ToString(), OrginalCertificateFilePath, true);
                    string signatureXML = CreateXML();

                    Results result = null;
                    DocumentBL objDocumentBL = new DocumentBL();
                    result = objDocumentBL.ManageDigitalSignature(action, OrginalCertificateFilePath, signatureXML, hdnLoginOrgId.Value, hdnLoginToken.Value);
                    if (result.ResultDS.Tables[0].Rows.Count > 0)
                    {
                        if (result.ResultDS.Tables[0].Rows[0]["ActionStatus"].ToString() == "SUCCESS")
                        {
                            Createtemptable();
                            chkvisibleSignature.Checked = true;
                            LoadGrid(1);
                            divMsg.Style.Add("color", "green");
                            divMsg.InnerHtml = "Document/s signed successfully.";
                        }
                    }
                    else
                    {
                        divMsg.Style.Add("color", "Red");
                        divMsg.InnerHtml = "Failed to Add the certificate";
                    }

                }
                else
                {
                    divMsg.Style.Add("color", "Red");
                    divCertMsg.InnerHtml = "Certificate or password is invalid";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowApplySignaturePopUP", "ShowApplySignaturePopUP();", true);
                }

                Logger.Trace("btnadd_Click Completed:-", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }

        }

        //FileUpload//
        protected void AsyncFileUpload1_UploadedComplete(object sender, AjaxControlToolkit.AsyncFileUploadEventArgs e)
        {
            try
            {
                Logger.Trace("AsyncFileUpload1_UploadedComplete Started:-", Session["LoggedUserId"].ToString());
                string TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"];
                Session["CertficateName"] = AsyncFileUpload1.FileName;
                Session["TempCertficatePath"] = TempFolder + AsyncFileUpload1.FileName;
                if (!Directory.Exists(TempFolder))
                {
                    Directory.CreateDirectory(TempFolder);
                }
                if (AsyncFileUpload1.HasFile)
                {
                    AsyncFileUpload1.SaveAs(TempFolder + AsyncFileUpload1.FileName);

                }
                Logger.Trace("AsyncFileUpload1_UploadedComplete Completed:-", Session["LoggedUserId"].ToString());
            }
            catch(Exception ex)
            {
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }

        }


        //Selected Index change of Status dropdown//
        protected void DDLSignedStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Logger.Trace("DDLSignedStatus_SelectedIndexChanged Started:-", Session["LoggedUserId"].ToString());
                Createtemptable();
                chkvisibleSignature.Checked = true;
                LoadGrid(1);
                Logger.Trace("DDLSignedStatus_SelectedIndexChanged Completed:-", Session["LoggedUserId"].ToString());
            }
            catch(Exception ex)
            {

                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        //For handling Select pagewise//
        public void SelectPageWise()
        {
            try
            {
                Logger.Trace("SelectPageWise Started:-", Session["LoggedUserId"].ToString());
                foreach (GridViewRow row in grdView.Rows)
                {
                    int UploadDocID = Convert.ToInt32(row.Cells[8].Text);
                    DataTable DT = (DataTable)ViewState["SignatureDT"];
                    CheckBox ChkBoxRows = (CheckBox)row.FindControl("chkSelected");
                    if (CheckSelectPageWise.Checked == true)
                    {
                        ChkBoxRows.Checked = true;
                        DT.Rows.Add(UploadDocID);
                    }
                    else
                    {
                        ChkBoxRows.Checked = false;
                        for (int i = DT.Rows.Count - 1; i >= 0; i--)
                        {
                            DataRow dr = DT.Rows[i];
                            if (dr["UploadDocID"].ToString() == UploadDocID.ToString())
                            {
                                dr.Delete();
                            }
                        }
                    }
                    DT.AcceptChanges();
                    ViewState["SignatureDT"] = DT;
                }
                Logger.Trace("SelectPageWise Completed:-", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {

                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        //Grid check box even to add to view state//
        protected void chkSelected_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                Logger.Trace("chkSelected_CheckedChanged Started:-", Session["LoggedUserId"].ToString());
                CheckBox chkBx = sender as CheckBox;
                GridViewRow row = chkBx.NamingContainer as GridViewRow;
                int UploadDocID = Convert.ToInt32(row.Cells[8].Text);
                DataTable DT = (DataTable)ViewState["SignatureDT"];


                if (chkBx.Checked == true)
                {
                    chkBx.Checked = true;
                    DT.Rows.Add(UploadDocID);

                }
                else
                {
                    for (int i = DT.Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow dr = DT.Rows[i];
                        if (dr["UploadDocID"].ToString() == UploadDocID.ToString())
                        {
                            dr.Delete();
                        }
                    }
                }
                DT.AcceptChanges();
                ViewState["SignatureDT"] = DT;
                Logger.Trace("chkSelected_CheckedChanged Completed:-", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {

                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

       

        //Select page wise dropdown event to add selected values to DT//
        protected void CheckSelectPageWise_CheckedChanged(object sender, EventArgs e)
        {
            SelectPageWise();
        }


        //For Removing Digital Signature//
        protected void btnRemoveDigitalSignature_Click(object sender, EventArgs e)
        {
            try
            {
                Logger.Trace("btnRemoveDigitalSignature_Click Started:-", Session["LoggedUserId"].ToString());
                string action = "RemoveDigitalSignature";
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                string signatureXML = CreateXML();
                Results result = null;
                DocumentBL objDocumentBL = new DocumentBL();
                result = objDocumentBL.ManageDigitalSignature(action, "", signatureXML, hdnLoginOrgId.Value, hdnLoginToken.Value);
                if (result.ResultDS.Tables[0].Rows[0]["ActionStatus"].ToString() == "SUCCESS")
                {
                    Createtemptable();
                    chkvisibleSignature.Checked = true;
                    LoadGrid(1);
                }
                Logger.Trace("btnRemoveDigitalSignature_Click Completed:-", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        #region Paging Events
        private int GetTotalPages(double totalRows)
        {

            int totalPages = (int)Math.Ceiling(totalRows / PAGE_SIZE);

            return totalPages;
        }
        protected void GetPageIndex(object sender, CommandEventArgs e)
        {
            CheckSelectPageWise.Checked = false;
            switch (e.CommandName)
            {
                case "First":
                    PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);  // Added later
                    currentPageNumber = 1;
                    break;

                case "Previous":
                    PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);  // Added later
                    currentPageNumber = Int32.Parse(ddlPage.SelectedValue) - 1;
                    break;

                case "Next":
                    PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);  // Added later
                    currentPageNumber = Int32.Parse(ddlPage.SelectedValue) + 1;
                    break;

                case "Last":
                    PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);  // Added later
                    currentPageNumber = Int32.Parse(lblTotalPages.Text);
                    break;
            }

            LoadGrid(Convert.ToInt32(currentPageNumber));
        }

        protected void ddlRows_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckSelectPageWise.Checked = false;
            currentPageNumber = 1;
            grdView.PageSize = Int32.Parse(ddlRows.SelectedValue);
            PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);
            LoadGrid(Convert.ToInt32(ddlPage.SelectedValue));
        }

        protected void ddlPage_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckSelectPageWise.Checked = false;
            PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);  // Added later
            currentPageNumber = Int32.Parse(ddlPage.SelectedValue);
            LoadGrid(Convert.ToInt32(ddlPage.SelectedValue));
        }




        #endregion
        
    }
}