/* ===================================================================================================================================
 * Author     : Gokuldas.Palapatta
 * Create date: 15 - 06 -2015
 * Description:  
======================================================================================================================================  
 * Change History   
 * Date:            Author:             Issue ID            Description:  
 * ----------       -------------       ----------          ------------------------------------------------------------------

 * 15 06 2015       gokuldas.palapatta  DMS5-4370	        Version Control has been implemented, but system does not have the menu to view/retrieve earlier versions of the document.( a new button and function added in java script to redirect
====================================================================================================================================== */
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lotex.EnterpriseSolutions.CoreBE;
using OfficeConverter;
using Lotex.EnterpriseSolutions.CoreBL;
using System.Configuration;
using System.IO;

namespace Lotex.EnterpriseSolutions.WebUI.Secure.Core
{
    public partial class DocumentHistoryView : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //
            PDFViewer1.AnnotationToolbarVisible = false;
            CheckAuthentication();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
            hdnLoginToken.Value = loginUser.LoginToken;
            if (!IsPostBack)
            {
              
                    hdnSearchPageUrl.Value = Request.ServerVariables["HTTP_REFERER"];
               
                if (Request.QueryString["id"] != null)
                {
                    hdnDcoumentId.Value = Request.QueryString["id"].ToString();
                }
                LoadGrid();
            }

        }

        private void LoadGrid()
        {
            try
            {
                Logger.Trace("LoadGrid strated", Session["LoggedUserId"].ToString());
                Results results = new Results();
                DocumentBL bl = new DocumentBL();
                results = bl.GetDocumetHistoryDetails("", Convert.ToInt32(hdnDcoumentId.Value), hdnLoginOrgId.Value, hdnLoginToken.Value);
                if (results.ResultDS.Tables[0].Rows.Count > 0)
                {
                    grdView.DataSource = results.ResultDS.Tables[0];
                    grdView.DataBind();
                }
                Logger.Trace("LoadGrid compelted", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("LoadGrid Exception: " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        //Toload Image to viewer
        private void LoadImageToViewer(string Action)
        {
            try
            {
                Logger.Trace("LoadImageToViewer started.", Session["LoggedUserId"].ToString());

                string TempFolder = ConfigurationManager.AppSettings["TempWorkFolder"];
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


                string PdfPath = TempFolder + beforedot(hdnEncrpytFileName.Value.ToString()) + "\\" + PageNo.ToString() + ".pdf";
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
                Logger.Trace("LoadImageToViewer Finished.", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace(" LoadImageToViewer Exception: " + ex.Message, Session["LoggedUserId"].ToString());

            }

        }

        private string convertPDFToImage(string Filepath, string EncryptedFilepath, int RequestingPageNo)
        {
            string CurrentFilepath = string.Empty, filepath = string.Empty;

            try
            {
                Logger.Trace("convertPDFToImage started.", Session["LoggedUserId"].ToString());
                
                string TempFolder = ConfigurationManager.AppSettings["TempWorkFolder"];
                ConvertPdf2Image obj = new ConvertPdf2Image();


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

                Logger.Trace("convertPDFToImage finished.", Session["LoggedUserId"].ToString());
              
            }
            catch (Exception ex)
            {
                Logger.Trace("convertPDFToImage Exception: " + ex.Message, Session["LoggedUserId"].ToString());

            }
            return CurrentFilepath;
        }

        protected void grdView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                Logger.Trace("grdView_RowDataBound started.", Session["LoggedUserId"].ToString());


                e.Row.Cells[1].Visible = false;
                e.Row.Cells[3].Visible = false;
                e.Row.Cells[5].Visible = false;
                Logger.Trace("grdView_RowDataBound finished.", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {

                Logger.Trace("grdView_RowDataBound Exception: " + ex.Message, Session["LoggedUserId"].ToString());
            }


        }

        protected void grdView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                string TempFolder = ConfigurationManager.AppSettings["TempWorkFolder"];
                Logger.Trace("grdView_RowCommand Started:-", Session["LoggedUserId"].ToString());
                string action = e.CommandName.ToString();
                int index = Convert.ToInt32(e.CommandArgument.ToString());
                hdnFileLocation.Value = grdView.Rows[index].Cells[5].Text.ToString();
                hdnEncrpytFileName.Value = grdView.Rows[index].Cells[3].Text.ToString();
                hdnFileName.Value = grdView.Rows[index].Cells[2].Text.ToString();

                if (action == "View")
                {

                    //CopyFil9eToTempfolder();
                    if (File.Exists(TempFolder + hdnEncrpytFileName.Value))
                    {
                        File.Delete(TempFolder + hdnEncrpytFileName.Value);
                    }
                    string FileExtension = Path.GetExtension(hdnFileLocation.Value);
                    string OriginalFilePathZip = hdnFileLocation.Value.Replace(FileExtension, ".zip");
                    string ZippedFilePath = TempFolder + beforedot(hdnEncrpytFileName.Value) + ".zip";
                    File.Copy(OriginalFilePathZip, ZippedFilePath, true);
                    if (File.Exists(ZippedFilePath))
                    {
                        //Unzip the document for downloading: if the file is in zipped format
                        Logger.Trace("Unzipping file. Filepath: " + ZippedFilePath, Session["LoggedUserId"].ToString());
                        FileZip.Unzip(ZippedFilePath);
                    }

                    DocumentSlicing(TempFolder + hdnEncrpytFileName.Value, hdnEncrpytFileName.Value);
                    LoadImageToViewer(string.Empty);
                }
                else if (action == "Download")
                {

                    DownloadFile(hdnFileLocation.Value);
                }





                Logger.Trace("grdView_RowCommand Completed:-", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("grdView_RowCommand Exception: " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        private void DocumentSlicing(string Filepath, string EncryptedFileName)
        {
            try
            {

                int PageCount = 0;
                string TempFolder = ConfigurationManager.AppSettings["TempWorkFolder"];
                string Foldername = TempFolder + beforedot(EncryptedFileName) + "\\";
                if (EncryptedFileName.Length != null && afterdot(EncryptedFileName).Length > 0)
                {
                    switch (afterdot(Session["EncyptImgName"].ToString()).ToLower())
                    {
                        case ".doc":
                        case ".docx":
                            PageCount = new MSWord().Convert(Filepath, Foldername + ".pdf", true);
                            Logger.Trace("DocumentSlicing Pagecount of MSWord " + PageCount, Session["LoggedUserId"].ToString());
                            break;
                        case ".ppt":
                        case ".pptx":
                            PageCount = new MSPowerPoint().Convert(Filepath, Foldername + ".pdf", true);
                            Logger.Trace("DocumentSlicing Pagecount of MSPowerPoint " + PageCount, Session["LoggedUserId"].ToString());
                            break;
                        case ".xls":
                        case ".xlsx":
                            PageCount = new MSExcel().Convert(Filepath, Foldername + ".pdf", true);
                            Logger.Trace("DocumentSlicing Pagecount of MSExcel " + PageCount, Session["LoggedUserId"].ToString());
                            break;
                        case ".tif":
                        case ".tiff":


                        case ".jpg":
                        case ".bmp":
                        case ".jpeg":
                        case ".png":
                        case ".gif":
                        case ".giff":
                            PageCount = new Image2Pdf().tiff2PDF(Filepath, Foldername, true);
                            Logger.Trace("DocumentSlicing Pagecount of Image2Pdf().tiff2PDF " + PageCount, Session["LoggedUserId"].ToString());
                            break;
                        case ".pdf":
                            PageCount = new Image2Pdf().ExtractPages(Filepath);
                            Logger.Trace("DocumentSlicing Pagecount of Image2Pdf().ExtractPages " + PageCount, Session["LoggedUserId"].ToString());
                            break;
                        default:
                            // result = "Failed";
                            break;
                    }
                    hdnpagecount.Value = PageCount.ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.Trace("DocumentSlicing Exception: " + ex.Message, Session["LoggedUserId"].ToString());
            }

        }
        private bool DownloadFile(string FilePath)
        {
            bool Status = false;

            /* DMS5-4232 BS */
            string FileExt = afterdot(Path.GetFileName(FilePath));
            string Filename = beforedot(hdnFileName.Value) + FileExt;
            /* DMS5-4232 BE */
            try
            {
                Response.ContentType = "application/octet-stream";
                Response.AppendHeader("Content-Disposition", "attachment;filename=" + Filename);
                Response.TransmitFile(FilePath);
                Response.Flush();
                Status = true;
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception: " + ex.Message, Session["LoggedUserId"].ToString());
            }
            return Status;
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


        protected void btnCallFromJavascript_Click(object sender, EventArgs e)
        {
            Logger.Trace("btnCallFromJavascript_Click Started:-", Session["LoggedUserId"].ToString());
            try
            {
                LoadImageToViewer(hdnAction.Value);
            }
            catch (Exception ex)
            {

                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
            Logger.Trace("btnCallFromJavascript_Click Completed:-", Session["LoggedUserId"].ToString());
        }

        protected void btnGotoWorkflow_Click(object sender, EventArgs e)
        {
            Response.Redirect(hdnSearchPageUrl.Value);
        }


    }
}