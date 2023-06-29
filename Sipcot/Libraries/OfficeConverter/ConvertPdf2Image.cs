using System;
using System.Drawing;
using System.IO;
using MuPDFLib;
using System.Drawing.Drawing2D;

namespace OfficeConverter
{
    public class ConvertPdf2Image
    {
        public string ConvertPDFtoHojas(string filename, string outputFilepath, string currentpagenumber)
        {
            string currentfilepath = string.Empty;
            PDFLibNet.PDFWrapper _pdfDoc = new PDFLibNet.PDFWrapper();
            try
            {
                if (!Directory.Exists(outputFilepath))
                    Directory.CreateDirectory(outputFilepath);

                Logger.TraceErrorLog(filename);
                _pdfDoc.LoadPDF(filename);

                for (int i = 0; i < _pdfDoc.PageCount; i++)
                {
                    using (Image img = RenderPage(_pdfDoc, i))
                    {
                        Logger.TraceErrorLog(outputFilepath + "\\" + currentpagenumber + ".jpg");
                        img.Save(outputFilepath + "\\" + currentpagenumber + ".jpg");
                        // img.Save(Path.Combine(dirOut, string.Format("{0}{1}.jpg", i, DateTime.Now.ToString("mmss"))));
                        currentfilepath = outputFilepath + "\\" + currentpagenumber + ".jpg";
                    }
                }

            }
            catch(Exception ex)
            {
                Logger.TraceErrorLog(ex.Message);

            }
            finally
            {
                _pdfDoc.Dispose();
                GC.Collect();
            }
            return currentfilepath;
        }
        public Image RenderPage(PDFLibNet.PDFWrapper doc, int page)
        {
            doc.CurrentPage = page + 1;
            doc.CurrentX = 0;
            doc.CurrentY = 0;

            
            doc.RenderPage(IntPtr.Zero);

            // create an image to draw the page into
            var buffer = new Bitmap(doc.PageWidth, doc.PageHeight);

            try
            {

                doc.ClientBounds = new Rectangle(0, 0, doc.PageWidth, doc.PageHeight);
                using (var g = Graphics.FromImage(buffer))
                {
                    var hdc = g.GetHdc();
                    try
                    {
                        doc.DrawPageHDC(hdc);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        g.ReleaseHdc();
                        GC.Collect();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.TraceErrorLog(ex.Message);
            }
            return buffer;

        }
        // for getting single pages to convert as per the request  
        public string ConvertPDFtoHojas(string filename, int pagenumber, string outputFolder)
        {
            string Path = string.Empty;
            //  string dirOut = @"D:\New folder\";
            PDFLibNet.PDFWrapper _pdfDoc = new PDFLibNet.PDFWrapper();
            _pdfDoc.LoadPDF(filename);


            Image img = RenderPage(_pdfDoc, pagenumber);
            img.Save(outputFolder + pagenumber + ".jpg");
            // img.Save(Path.Combine(dirOut, string.Format("{0}{1}.jpg", i, DateTime.Now.ToString("mmss"))));
            Path = "http://localhost/SplitFiles/" + pagenumber + ".jpg";

            try
            {
                _pdfDoc.Dispose();
            }
            catch
            {

            }

            return Path;
        }


        public string ConvertPDFtoHojas(string filename, string outputFilepath, string currentpagenumber, string WaterMarkText)
        {

            string currentfilepath = string.Empty;
            PDFLibNet.PDFWrapper _pdfDoc = new PDFLibNet.PDFWrapper();
            try
            {
                if (!Directory.Exists(outputFilepath))
                    Directory.CreateDirectory(outputFilepath);

                _pdfDoc.LoadPDF(filename);

                for (int i = 0; i < _pdfDoc.PageCount; i++)
                {
                    using (Image img = RenderPage(_pdfDoc, i))
                    {

                        #region watermark properties and drawing

                        // setup default settings
                        string fontName = "Arial"; float fontSize = 80;
                        System.Drawing.Color myWatermarkColor = Color.Gray;
                        System.Drawing.Font myFont = new Font(fontName, fontSize, FontStyle.Bold | FontStyle.Italic);

                        int opac = 0;
                        string sOpacity = "50%";

                        // Determine the opacity of the watermark
                        switch (sOpacity)
                        {
                            case "100%":
                                opac = 255; // 1 * 255
                                break;
                            case "75%":
                                opac = 191; // .75 * 255
                                break;
                            case "50%":
                                opac = 127; // .5 * 255
                                break;
                            case "25%":
                                opac = 64;  // .25 * 255
                                break;
                            case "10%":
                                opac = 25;  // .10 * 255
                                break;
                            default:
                                opac = 127; // default at 50%; .5 * 255
                                break;
                        }

                        // Get a graphics context
                        Graphics g = Graphics.FromImage(img);

                        // Create a solid brush to write the watermark text on the image
                        Brush myBrush = new SolidBrush(Color.FromArgb(opac, myWatermarkColor));

                        // Trigonometry: Tangent = Opposite / Adjacent
                        double tangent = (double)img.Height /
                                         (double)img.Width;

                        // convert arctangent to degrees
                        double angle = Math.Atan(tangent) * (180 / Math.PI);

                        // a^2 = b^2 + c^2 ; a = sqrt(b^2 + c^2)
                        double halfHypotenuse = (Math.Sqrt((img.Height
                                               * img.Height) +
                                               (img.Width *
                                               img.Width))) / 2;

                        // Horizontally and vertically aligned the string
                        // This makes the placement Point the physical 
                        // center of the string instead of top-left.
                        StringFormat stringFormat = new StringFormat();
                        stringFormat.Alignment = StringAlignment.Center;
                        stringFormat.LineAlignment = StringAlignment.Center;

                        g.RotateTransform((float)angle);
                        g.CompositingMode = CompositingMode.SourceOver;
                        g.DrawString(WaterMarkText, myFont, myBrush,
                                     new Point((int)halfHypotenuse, 0),
                                     stringFormat);

                        #endregion
                        
                        img.Save(outputFilepath + "\\" + currentpagenumber + ".jpeg");
                        // img.Save(Path.Combine(dirOut, string.Format("{0}{1}.jpg", i, DateTime.Now.ToString("mmss"))));
                        currentfilepath = outputFilepath + "\\" + currentpagenumber + ".jpeg";
                    }
                }

            }
            catch {
                throw;
            }
            finally
            {
                // Release previously loaded pdf file
                try
                {
                    _pdfDoc.LoadPDF("");
                }
                catch { }

                _pdfDoc.Dispose();
                GC.Collect();
            }
            return currentfilepath;
        }


        /// <summary>
        /// Converts PDF current page to JPEG and return JPEG image path
        /// </summary>
        /// <param name="sourcePdfPath"></param>
        /// <param name="destinationPath"></param>
        /// <param name="currentPageNumber"></param>
        /// <returns></returns>
        public string GetJpegPageFromPdf(string sourcePdfPath, string destinationPath, string currentPageNumber)
        {
            return GetJpegPageFromPdf1(sourcePdfPath, destinationPath, currentPageNumber);
        }

        private string GetJpegPageFromPdf1(string sourcePdfPath, string destinationPath, string currentPageNumber)
        {
            Logger.TraceErrorLog("Starting Splitting Pdf File fn=ConvertPdf2Jpeg SourcePath=" + sourcePdfPath);
            try
            {
                float DotsPerInch = 300;//Convert.ToInt32(ConfigurationManager.AppSettings["DotsPerImage"].ToString());
                int MAXPixels = 50;//Convert.ToInt32(ConfigurationManager.AppSettings["MAXPixels"].ToString());
                RenderType renderType;
                string RenderTypeT = ""; // ConfigurationManager.AppSettings["RenderType"].ToString();
                if (RenderTypeT.ToLower() == "monochrome")
                {
                    renderType = RenderType.Monochrome;
                }
                else if (RenderTypeT.ToLower() == "grayscale")
                {
                    renderType = RenderType.Grayscale;
                }
                else
                {
                    renderType = RenderType.RGB;
                }

                if (!Directory.Exists(destinationPath))
                {
                    Directory.CreateDirectory(destinationPath);
                }
                else
                {
                    Directory.Delete(destinationPath, true);
                    Directory.CreateDirectory(destinationPath);
                }
                //Converting PDF to Jpeg 

                MuPdfConverter.ConvertPdfToTiff(sourcePdfPath, destinationPath + "\\" + currentPageNumber + ".jpeg", DotsPerInch, renderType, false, true, MAXPixels, "");
            
                
                Logger.TraceErrorLog("Finished Splitting Pdf File fn=ConvertPdf2Jpeg SourcePath=" + sourcePdfPath);
            }
            catch (Exception ex)
            {
                Logger.TraceErrorLog("Starting Splitting Pdf File fn=ConvertPdf2Jpeg SourcePath=" + destinationPath + "exeption=" + ex.Message.ToString());
            }
            return destinationPath + "\\" + currentPageNumber + ".jpeg";
        }

        //Converting Pdf files to Jpeg.
        public string ConvertPdf2Jpeg(String sourcePdfPath, string targetPath)
        {
            //Logger.TraceErrorLog("Starting Splitting Pdf File fn=ConvertPdf2Jpeg SourcePath=" + sourcePdfPath);
           
            string outputFilepath=string.Empty;
            string currentpagenumber=string.Empty;
            outputFilepath=Path.GetDirectoryName(targetPath);
            currentpagenumber=Path.GetFileNameWithoutExtension(targetPath);
            try
            {
                ConvertPDFtoHojas(sourcePdfPath, outputFilepath, currentpagenumber);
            //    float DotsPerImage = 100;
            //    int MAXPixels = 200;
            //    RenderType renderType;
            //    string RenderTypeT = "";
            //    if (RenderTypeT.ToLower() == "monochrome")
            //    {
            //        renderType = RenderType.Monochrome;
            //    }
            //    else if (RenderTypeT.ToLower() == "grayscale")
            //    {
            //        renderType = RenderType.Grayscale;
            //    }
            //    else
            //    {
            //        renderType = RenderType.RGB;
            //    }

            //    //Converting PDF to Jpeg 
            //    MuPdfConverter.ConvertPdfToTiff(sourcePdfPath, targetPath, DotsPerImage, renderType, false, true, MAXPixels, "");

            }
            catch (Exception ex)
            {
                Logger.TraceErrorLog("Exception:" + ex.Message);
            }
            return targetPath;
        }

    }
}
