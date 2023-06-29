using System;
using System.Drawing;
namespace PdfViewer
{
    public partial class PDFViewer : System.Web.UI.UserControl
    {
        public bool NavigationToolbarVisible = true;
        public bool AnnotationToolbarVisible = true;
        public string filepath = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            ImageControls_Part2.Visible = AnnotationToolbarVisible;
            //ThumbnailViewerContainer();
        }

        public void JustPostbakUserControl(object sender, EventArgs e)
        {
            Page_Load(sender, e);
        }

        //private void ThumbnailViewerContainer()
        //{
        //    try
        //    {
        //        string returnValue = "";
        //        string OriginalFilePath = filepath;
        //        // File Name
        //        string FileName = Path.GetFileNameWithoutExtension(filepath);
        //        // File Extension
        //        string FileExtension = Path.GetExtension(filepath);

        //        // Temp folder path to store splitted files
        //        string TempWorkFolderPath = ConfigurationManager.AppSettings["TempWorkFolder"] + @"\" + FileName;
        //        // Splitted files original folder path
        //        string SplittedFilesOriginalFolderPath = IOUtility.beforedot(filepath);

        //        string OriginalFilePathZip = OriginalFilePath.Replace(FileExtension, ".zip");
        //        string ZippedFilePath = TempWorkFolderPath + ".zip";
        //        File.Copy(OriginalFilePathZip, ZippedFilePath, true);


        //        if (File.Exists(ZippedFilePath))
        //        {
        //            //Unzip the document for downloading: if the file is in zipped format
        //            Logger.Trace("Unzipping file. Filepath: " + ZippedFilePath, Session["LoggedUserId"].ToString());
        //            FileZip.Unzip(ZippedFilePath);


        //        }

        //        string sFilepath = ConfigurationManager.AppSettings["TempWorkFolder"] + Path.GetFileName(filepath);

        //        if (Path.GetExtension(sFilepath) != ".pdf")
        //        {
        //            ConvertFiles(sFilepath);
        //            sFilepath = sFilepath.Replace(Path.GetExtension(sFilepath), ".pdf");
        //        }

        //        string TextFilePath = ConfigurationManager.AppSettings["TempWorkFolder"] + Path.GetFileName(filepath);

        //        if (!File.Exists(TextFilePath.Replace(FileExtension, ".txt")))
        //        {
        //            File.Create(TextFilePath.Replace(FileExtension, ".txt")).Dispose();

        //            PDFLibNet.PDFWrapper pdfDoc = new PDFLibNet.PDFWrapper();
        //            pdfDoc.LoadPDF(sFilepath);
        //            if (pdfDoc.Outline.Count <= 0)
        //            {
        //                //StartPageList:
        //                returnValue = "<!--PageNumberOnly--><ul>";
        //                //for (int i = 1; i <= pdfDoc.PageCount; i++)
        //                using (TextWriter tw = new StreamWriter(TextFilePath.Replace(FileExtension, ".txt")))
        //                {
        //                    for (int i = 1; i <= pdfDoc.PageCount; i++)
        //                    {
        //                        PDFLibNet.PDFPage pdfPage = pdfDoc.Pages[i];
        //                        pdfDoc.CurrentPage = i;
        //                        pdfDoc.CurrentX = 0;
        //                        pdfDoc.CurrentY = 0;

        //                        System.Windows.Forms.PictureBox oPictureBox = new System.Windows.Forms.PictureBox();

        //                        pdfDoc.RenderPage(oPictureBox.Handle);

        //                        System.Drawing.Bitmap bmp = new Bitmap(pdfDoc.PageWidth, pdfDoc.PageHeight);
        //                        pdfDoc.ClientBounds = new Rectangle(0, 0, pdfDoc.PageWidth, pdfDoc.PageHeight);
        //                        Graphics g = Graphics.FromImage(bmp);
        //                        using (g)
        //                        {
        //                            IntPtr hdc = g.GetHdc();
        //                            pdfDoc.DrawPageHDC(hdc);
        //                            g.ReleaseHdc();
        //                        }
        //                        g.Dispose();

        //                        Bitmap ResizeBmp = resizeImage(bmp, 70, 120);
        //                        using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
        //                        {
        //                            ResizeBmp.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
        //                            Byte[] bytes = new Byte[memoryStream.Length];
        //                            memoryStream.Position = 0;
        //                            memoryStream.Read(bytes, 0, (int)bytes.Length);
        //                            string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
        //                            //Image2.ImageUrl = "data:image/png;base64," + base64String;
        //                            //Image2.Visible = true;

        //                            //returnValue += "<li><a href=\"javascript:changePage('" + System.Convert.ToString(i) + "')\">Page " + System.Convert.ToString(i) + "</a></li>";
        //                            //returnValue += "<li><a href=\"javascript:changePage('" + System.Convert.ToString(i) + "')\">" + System.Convert.ToString(i) + "</a><img src=\"data:image/png;base64," + base64String + "\" alt=\"Smiley face\" height=\"150\" width=\"100\"></li>";


        //                            //tw.WriteLine("<li><a href=\"#\" onclick=\"clickEffect(this);thumbnavigationHandler('" + System.Convert.ToString(i) + "','THUMB');\">" + System.Convert.ToString(i) + "<img src=\"data:image/png;base64," + base64String + "\" alt=\"Smiley face\" height=\"150\" width=\"100\"></a></li>");
        //                            tw.WriteLine("<ul><li><a href=\"#\" onclick=\"clickEffect(this);thumbnavigationHandler('" + System.Convert.ToString(i) + "','THUMB');\"><img src=\"data:image/png;base64," + base64String + "\" alt=\"highlight\" height=\"120\" width=\"70\"></a></li></ul>");
        //                        }

        //                    }
        //                    tw.Close();
        //                }

        //            }
        //        }

        //        string[] lines = System.IO.File.ReadAllLines(TextFilePath.Replace(FileExtension, ".txt"));
        //        foreach (string line in lines)
        //        {
        //            //BookmarkContentCell.Text += line;
        //        }
        //        // BookmarkContentCell.Text = returnValue;
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        private static System.Drawing.Bitmap resizeImage(System.Drawing.Bitmap imgToResize, int width, int height)
        {
            //Get the image current width
            int sourceWidth = imgToResize.Width;
            //Get the image current height
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;
            //Calulate  width with new desired size
            //nPercentW = ((float)size.Width / (float)sourceWidth);
            //nPercentW = ((((float)sourceWidth / 4) * 3) / (float)sourceWidth);
            ////Calculate height with new desired size
            //// nPercentH = ((float)size.Height / (float)sourceHeight);
            //nPercentH = ((((float)sourceHeight / 4) * 3) / (float)sourceHeight);

            //if (nPercentH < nPercentW)
            //    nPercent = nPercentH;
            //else
            //    nPercent = nPercentW;
            //New Width
            //int destWidth = (int)(sourceWidth * nPercent);
            ////New Height
            //int destHeight = (int)(sourceHeight * nPercent);
            int destWidth = width;
            //New Height
            int destHeight = height;

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((System.Drawing.Image)b);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            // Draw image with new width and height
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();
            return (System.Drawing.Bitmap)b;
        }

        public void ConvertFiles(string filepath)
        {

            //string sFolder = "";
            string sPath = "";
            //string sPath = sFolder + "\\" + attachment.FileName;
            //attachment.SaveAsFile(sFolder + "\\" + attachment.FileName);

            if (System.IO.Path.GetExtension(filepath) == ".jpg")
            {

            }
            else if (System.IO.Path.GetExtension(filepath) == ".doc" || System.IO.Path.GetExtension(filepath) == ".docx")
            {


                object missing = System.Reflection.Missing.Value;

                Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
                System.IO.FileInfo wordFile = new System.IO.FileInfo(filepath);

                word.Visible = false;
                word.ScreenUpdating = false;

                // Cast as Object for word Open method
                Object filename = (Object)filepath;
                //object filename = (Object)wordFile.FullName;

                // Use the dummy value as a placeholder for optional arguments
                Microsoft.Office.Interop.Word.Document doc = word.Documents.Open(ref filename, ref missing,
                ref missing, ref missing, ref missing, ref missing, ref missing,
                ref missing, ref missing, ref missing, ref missing, ref missing,
                ref missing, ref missing, ref missing, ref missing);
                doc.Activate();

                object outputFileName = filepath.Replace(".docx", ".pdf").Replace(".doc", ".pdf");
                object fileFormat = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatPDF;

                // Save document into PDF Format
                doc.SaveAs(ref outputFileName,
                ref fileFormat, ref missing, ref missing,
                ref missing, ref missing, ref missing, ref missing,
                ref missing, ref missing, ref missing, ref missing,
                ref missing, ref missing, ref missing, ref missing);

                // Close the Word document, but leave the Word application open.
                // doc has to be cast to type _Document so that it will find the
                // correct Close method.
                object saveChanges = Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges;
                ((Microsoft.Office.Interop.Word._Document)doc).Close(ref saveChanges, ref missing, ref missing);
                doc = null;

                // word has to be cast to type _Application so that it will find
                // the correct Quit method.
                ((Microsoft.Office.Interop.Word._Application)word).Quit(ref missing, ref missing, ref missing);
                word = null;

            }
            else if (System.IO.Path.GetExtension(filepath) == ".xls" || System.IO.Path.GetExtension(filepath) == ".xlsx")
            {
                Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();

                excel.Visible = false;
                excel.ScreenUpdating = false;
                excel.DisplayAlerts = false;



                object missing = System.Reflection.Missing.Value;

                System.IO.FileInfo excelFile = new System.IO.FileInfo(filepath);

                string filename = filepath;

                Microsoft.Office.Interop.Excel.Workbook wbk = excel.Workbooks.Open(filename, missing,
                        missing, missing, missing, missing, missing,
                        missing, missing, missing, missing, missing,
                        missing, missing, missing);
                wbk.Activate();

                object outputFileName = wbk.FullName.Replace(".xlsx", ".pdf").Replace(".xls", ".pdf");
                Microsoft.Office.Interop.Excel.XlFixedFormatType fileFormat = Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF;

                // Save document into PDF Format
                wbk.ExportAsFixedFormat(fileFormat, outputFileName,
                missing, missing, missing,
                missing, missing, missing,
                missing);

                object saveChanges = Microsoft.Office.Interop.Excel.XlSaveAction.xlDoNotSaveChanges;
                ((Microsoft.Office.Interop.Excel._Workbook)wbk).Close(saveChanges, missing, missing);
                wbk = null;

            }
            else if (System.IO.Path.GetExtension(filepath) == ".ppt" || System.IO.Path.GetExtension(filepath) == ".pptx")
            {
                Microsoft.Office.Interop.PowerPoint.Application app = new Microsoft.Office.Interop.PowerPoint.Application();


                string targetPpt = filepath.Replace(".pptx", ".pdf").Replace(".ppt", ".pdf");
                object missing = Type.Missing;
                Microsoft.Office.Interop.PowerPoint.Presentation pptx = app.Presentations.Open(filepath, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoTrue, Microsoft.Office.Core.MsoTriState.msoFalse);
                pptx.SaveAs(targetPpt, Microsoft.Office.Interop.PowerPoint.PpSaveAsFileType.ppSaveAsPDF, Microsoft.Office.Core.MsoTriState.msoTrue);
                app.Quit();
            }

            // oFiles.Add(sPath.Replace(".docx", ".pdf").Replace(".doc", ".pdf").Replace(".xlsx", ".pdf").Replace(".xls", ".pdf").Replace(".pptx", ".pdf").Replace(".ppt", ".pdf"));



        }
    }
}