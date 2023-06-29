using System;
using System.IO;
//using System.Web.UI.WebControls;
using System.Drawing;
using System.Drawing.Imaging;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.Text;

namespace OfficeConverter
{
    public class Image2Pdf
    {
        // Retrive PageCount of a multi-page tiff image
        public int getPageCount(String fileName)
        {
            int pageCount = -1;
            try
            {
                Image img = Bitmap.FromFile(fileName);
                pageCount = img.GetFrameCount(FrameDimension.Page);
                if (pageCount == 0) //single page tiff
                {
                    pageCount = 1;
                }
                else if (pageCount > 1) // tiffs having pages more than one
                {
                    //pageCount = pageCount - 1;
                }
                img.Dispose();

            }
            catch
            {
                pageCount = 1;
            }
            return pageCount;
        }

        public int getPageCount(Image img)
        {
            int pageCount = -1;
            try
            {
                pageCount = img.GetFrameCount(FrameDimension.Page);
            }
            catch
            {
                pageCount = 0;
            }
            return pageCount;
        }

        // Retrive a specific Page from a multi-page tiff image
        public Image getTiffImage(String sourceFile, int pageNumber)
        {
            Image returnImage = null;

            try
            {
                Image sourceIamge = Bitmap.FromFile(sourceFile);
                returnImage = getTiffImage(sourceIamge, pageNumber);
                sourceIamge.Dispose();
            }
            catch
            {
                returnImage = null;
            }

            //       String splittedImageSavePath = "X:\\CJT\\CJT-Docs\\CJT-Images\\result001.tif";
            //       returnImage.Save(splittedImageSavePath);

            return returnImage;
        }

        public Image getTiffImage(Image sourceImage, int pageNumber)
        {
            MemoryStream ms = null;
            Image returnImage = null;

            try
            {
                ms = new MemoryStream();
                Guid objGuid = sourceImage.FrameDimensionsList[0];
                FrameDimension objDimension = new FrameDimension(objGuid);
                sourceImage.SelectActiveFrame(objDimension, pageNumber);
                sourceImage.Save(ms, ImageFormat.Tiff);
                returnImage = Image.FromStream(ms);
            }
            catch
            {
                returnImage = null;
            }
            return returnImage;
        }
        public void tiff2PDF(string fileName, string destination)
        {
            PdfDocument doc = new PdfDocument();

            int pageCount = getPageCount(fileName);
            if (pageCount == 0)
            {
                pageCount = 1;
            }

            for (int i = 0; i < pageCount; i++)
            {
                PdfPage page = new PdfPage();

                Image tiffImg = getTiffImage(fileName, i);

                XImage img = XImage.FromGdiPlusImage(tiffImg);

                page.Width = img.PointWidth;
                page.Height = img.PointHeight;
                doc.Pages.Add(page);

                XGraphics xgr = XGraphics.FromPdfPage(doc.Pages[i]);

                xgr.DrawImage(img, 0, 0);
            }

            doc.Save(destination);

            doc.Close();
            GC.Collect();

        }
        public int tiff2PDF(string filePath, string destination, bool slice)
        {

            int pageCount = getPageCount(filePath);


            PdfDocument doc;
            PdfPage page;

            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }
            else
            {
                Directory.Delete(destination, true);
                Directory.CreateDirectory(destination);
            }
            for (int i = 0; i < pageCount; i++)
            {
                doc = new PdfDocument();
                page = new PdfPage();
                Image tiffImg = getTiffImage(filePath, i);

                XImage img = XImage.FromGdiPlusImage(tiffImg);

                page.Width = img.PointWidth;
                page.Height = img.PointHeight;
                doc.Pages.Add(page);
                XGraphics xgr = XGraphics.FromPdfPage(doc.Pages[0]);
                xgr.DrawImage(img, 0, 0);
                doc.Save(destination + "/" + (i + 1).ToString() + ".pdf");
                doc.Close();

            }
            GC.Collect();

            return pageCount;


        }

        public string  tiff2PDF(string filePath, string destination, bool slice,string pagenumber)
        {
            string currentfilepath = string.Empty;
            int pageCount = getPageCount(filePath);


            PdfDocument doc;
            PdfPage page;

            //if (!Directory.Exists(destination))
            //{
            //    Directory.CreateDirectory(destination);
            //}
            //else
            //{
            //    Directory.Delete(destination, true);
            //    Directory.CreateDirectory(destination);
            //}
            for (int i = 0; i < pageCount; i++)
            {
                doc = new PdfDocument();
                page = new PdfPage();
                Image tiffImg = getTiffImage(filePath, i);

                XImage img = XImage.FromGdiPlusImage(tiffImg);

                page.Width = img.PointWidth;
                page.Height = img.PointHeight;
                doc.Pages.Add(page);
                XGraphics xgr = XGraphics.FromPdfPage(doc.Pages[0]);
                xgr.DrawImage(img, 0, 0);
                doc.Save(destination + "\\" +  pagenumber + ".pdf");
                doc.Close();

            }
            GC.Collect();
            currentfilepath = destination + "\\" + pagenumber + ".pdf";
            return currentfilepath;


        }
        // Fuction will accept pdf file with full path and split the file in to 
        //individual files as per pages and create a folder i folder name.
        //public int ExtractPages(string sourcePdfPath)
        //{
        //    int p = 0;
        //    try
        //    {
        //        iTextSharp.text.Document document;
        //        iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(new iTextSharp.text.pdf.RandomAccessFileOrArray(sourcePdfPath), new ASCIIEncoding().GetBytes(""));
        //        if (!Directory.Exists(sourcePdfPath.ToLower().Replace(".pdf", "")))
        //        {
        //            Directory.CreateDirectory(sourcePdfPath.ToLower().Replace(".pdf", ""));
        //        }
        //        else
        //        {
        //            Directory.Delete(sourcePdfPath.ToLower().Replace(".pdf", ""), true);
        //            Directory.CreateDirectory(sourcePdfPath.ToLower().Replace(".pdf", ""));
        //        }

        //        for (p = 1; p <= reader.NumberOfPages; p++)
        //        {
        //            using (MemoryStream memoryStream = new MemoryStream())
        //            {

        //                document = new iTextSharp.text.Document();
        //                iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, memoryStream);
        //                writer.SetPdfVersion(iTextSharp.text.pdf.PdfWriter.PDF_VERSION_1_6);
        //                writer.CompressionLevel = iTextSharp.text.pdf.PdfStream.BEST_COMPRESSION;
        //                writer.SetFullCompression();
        //                document.SetPageSize(reader.GetPageSize(p));
        //                document.NewPage();
        //                document.Open();
        //                document.AddDocListener(writer);
        //                iTextSharp.text.pdf.PdfContentByte cb = writer.DirectContent;
        //                iTextSharp.text.pdf.PdfImportedPage pageImport = writer.GetImportedPage(reader, p);
        //                int rot = reader.GetPageRotation(p);
        //                if (rot == 90 || rot == 270)
        //                {
        //                    cb.AddTemplate(pageImport, 0, -1.0F, 1.0F, 0, 0, reader.GetPageSizeWithRotation(p).Height);
        //                }
        //                else
        //                {
        //                    cb.AddTemplate(pageImport, 1.0F, 0, 0, 1.0F, 0, 0);
        //                }
        //                document.Close();
        //                document.Dispose();
        //                File.WriteAllBytes(sourcePdfPath.ToLower().Replace(".pdf", "") + "/" + p + ".pdf", memoryStream.ToArray());
        //            }
        //        }
        //        reader.Close();
               
        //    }
        //    catch
        //    {
        //    }
        //    finally
        //    {
        //        GC.Collect();
        //    }
        //    return p - 1;
        //    //    iTextSharp.text.pdf.PdfReader reader = null;
        //    //    iTextSharp.text.Document sourceDocument = null;
        //    //    iTextSharp.text.Document.Compress = true;
        //    //    iTextSharp.text.pdf.PdfSmartCopy pdfCopyProvider = null;
        //    //    iTextSharp.text.pdf.PdfImportedPage importedPage = null;

        //    //    reader = new iTextSharp.text.pdf.PdfReader(new iTextSharp.text.pdf.RandomAccessFileOrArray(sourcePdfPath), null);
        //    //    int TotalPageCount = reader.NumberOfPages;

        //    //    if (!Directory.Exists(sourcePdfPath.ToLower().Replace(".pdf", "")))
        //    //    {
        //    //        Directory.CreateDirectory(sourcePdfPath.ToLower().Replace(".pdf", ""));
        //    //    }
        //    //    else
        //    //    {
        //    //        Directory.Delete(sourcePdfPath.ToLower().Replace(".pdf", ""), true);
        //    //        Directory.CreateDirectory(sourcePdfPath.ToLower().Replace(".pdf", ""));
        //    //    }

        //    //    try
        //    //    {
        //    //        for (int i = 1; i < TotalPageCount; i++)
        //    //        {
        //    //            sourceDocument = new iTextSharp.text.Document(reader.GetPageSizeWithRotation(i));
        //    //            pdfCopyProvider = new iTextSharp.text.pdf.PdfSmartCopy(sourceDocument,
        //    //            new System.IO.FileStream(sourcePdfPath.ToLower().Replace(".pdf", "") + "/" + i + ".pdf", System.IO.FileMode.Create));
        //    //            pdfCopyProvider.SetPdfVersion(iTextSharp.text.pdf.PdfSmartCopy.PDF_VERSION_1_2);
        //    //            pdfCopyProvider.CompressionLevel = iTextSharp.text.pdf.PdfStream.DEFAULT_COMPRESSION;
        //    //            pdfCopyProvider.SetFullCompression();
        //    //            sourceDocument.Open();
        //    //            importedPage = pdfCopyProvider.GetImportedPage(reader, i);
        //    //            pdfCopyProvider.AddPage(importedPage);
        //    //            pdfCopyProvider.Close();
        //    //            sourceDocument.Close();
        //    //        }

        //    //    }

        //    //    catch (Exception ex)
        //    //    {


        //    //    }
        //    //    finally
        //    //    {
        //    //        pdfCopyProvider.Close();
        //    //        sourceDocument.Close(); 
        //    //        reader.Close();
        //    //       
        //    //        sourceDocument.Dispose();
        //    //        pdfCopyProvider.Dispose();
        //    //        GC.Collect();


        //    //    }
        //    //    return TotalPageCount;
        //}

        public int ExtractPages(string sourcePdfPath)
        {
            //listBox1.Items.Add("Start Pageing....");
            //listBox1.Refresh();
            //WriteToLogFile("Start Pageing....", "TraceStatus");
            int p = 0;
            try
            {
                string Inputpath = sourcePdfPath;
                int pageCount = getPageCount(sourcePdfPath);


                if (!Directory.Exists(sourcePdfPath.ToLower().Replace(".pdf", "")))
                {
                    Directory.CreateDirectory(sourcePdfPath.ToLower().Replace(".pdf", ""));
                }
                else
                {
                    Directory.Delete(sourcePdfPath.ToLower().Replace(".pdf", ""), true);
                    Directory.CreateDirectory(sourcePdfPath.ToLower().Replace(".pdf", ""));
                }

                iTextSharp.text.pdf.PdfReader pdfReader = new iTextSharp.text.pdf.PdfReader(Inputpath);

                for (int pageNumber = 1; pageNumber <= pdfReader.NumberOfPages; pageNumber++)
                {
                    iTextSharp.text.pdf.parser.ITextExtractionStrategy strategy = new iTextSharp.text.pdf.parser.SimpleTextExtractionStrategy();
                    string currentText = iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(pdfReader, pageNumber, strategy);
                    string Pageno = Convert.ToString(pageNumber);
                    string filename = Pageno + ".pdf";
                    iTextSharp.text.Document document = new iTextSharp.text.Document();
                    iTextSharp.text.pdf.PdfCopy pdfCopy = new iTextSharp.text.pdf.PdfCopy(document, new FileStream(sourcePdfPath.ToLower().Replace(".pdf", "") + "\\" + filename, FileMode.Create));
                    document.Open();
                    pdfCopy.AddPage(pdfCopy.GetImportedPage(pdfReader, pageNumber));
                    document.Close();
                }
                var iNumOfPages = pdfReader.NumberOfPages;
                pdfReader.Close();
                return iNumOfPages;


            }
            catch (Exception ex)
            {
            }
            finally
            {

                GC.Collect();
            }
            return p - 1;

        }

        public int ExtractPages(string sourcePdfPath, string DestinationFolder)
        {
            int p = 0, initialcount = 0;
            try
            {
                iTextSharp.text.Document document;
                iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(new iTextSharp.text.pdf.RandomAccessFileOrArray(sourcePdfPath), new ASCIIEncoding().GetBytes(""));
                if (!Directory.Exists(DestinationFolder))
                {
                    Directory.CreateDirectory(DestinationFolder);
                }
                else
                {
                    DirectoryInfo di = new DirectoryInfo(DestinationFolder);
                    initialcount = di.GetFiles("*.pdf", SearchOption.AllDirectories).Length;
                }

                for (p = 1; p <= reader.NumberOfPages; p++)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        document = new iTextSharp.text.Document();
                        iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, memoryStream);
                        writer.SetPdfVersion(iTextSharp.text.pdf.PdfWriter.PDF_VERSION_1_2);
                        writer.CompressionLevel = iTextSharp.text.pdf.PdfStream.BEST_COMPRESSION;
                        writer.SetFullCompression();
                        document.SetPageSize(reader.GetPageSize(p));
                        document.NewPage();
                        document.Open();
                        document.AddDocListener(writer);
                        iTextSharp.text.pdf.PdfContentByte cb = writer.DirectContent;
                        iTextSharp.text.pdf.PdfImportedPage pageImport = writer.GetImportedPage(reader, p);
                        int rot = reader.GetPageRotation(p);
                        if (rot == 90 || rot == 270)
                        {
                            cb.AddTemplate(pageImport, 0, -1.0F, 1.0F, 0, 0, reader.GetPageSizeWithRotation(p).Height);
                        }
                        else
                        {
                            cb.AddTemplate(pageImport, 1.0F, 0, 0, 1.0F, 0, 0);
                        }
                        document.Close();
                        document.Dispose();
                        File.WriteAllBytes(DestinationFolder + "/" + p + ".pdf", memoryStream.ToArray());
                    }
                }
                reader.Close();
               
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                GC.Collect();
            }



            if (initialcount > (p - 1))
            {
                for (int k = (p - 1) + 1; k <= initialcount; k++)
                {
                    try
                    {
                        File.Delete(DestinationFolder + "/" + k + ".pdf");
                    }
                    catch
                    {
                    }
                }
            }

            return p - 1;

            //return p - 1;

            //    iTextSharp.text.pdf.PdfReader reader = null;
            //    iTextSharp.text.Document sourceDocument = null;
            //    iTextSharp.text.Document.Compress = true;
            //    iTextSharp.text.pdf.PdfSmartCopy pdfCopyProvider = null;
            //    iTextSharp.text.pdf.PdfImportedPage importedPage = null;

            //    reader = new iTextSharp.text.pdf.PdfReader(new iTextSharp.text.pdf.RandomAccessFileOrArray(sourcePdfPath), null);
            //    int TotalPageCount = reader.NumberOfPages;

            //    if (!Directory.Exists(sourcePdfPath.ToLower().Replace(".pdf", "")))
            //    {
            //        Directory.CreateDirectory(sourcePdfPath.ToLower().Replace(".pdf", ""));
            //    }
            //    else
            //    {
            //        Directory.Delete(sourcePdfPath.ToLower().Replace(".pdf", ""), true);
            //        Directory.CreateDirectory(sourcePdfPath.ToLower().Replace(".pdf", ""));
            //    }

            //    try
            //    {
            //        for (int i = 1; i < TotalPageCount; i++)
            //        {
            //            sourceDocument = new iTextSharp.text.Document(reader.GetPageSizeWithRotation(i));
            //            pdfCopyProvider = new iTextSharp.text.pdf.PdfSmartCopy(sourceDocument,
            //            new System.IO.FileStream(sourcePdfPath.ToLower().Replace(".pdf", "") + "/" + i + ".pdf", System.IO.FileMode.Create));
            //            pdfCopyProvider.SetPdfVersion(iTextSharp.text.pdf.PdfSmartCopy.PDF_VERSION_1_2);
            //            pdfCopyProvider.CompressionLevel = iTextSharp.text.pdf.PdfStream.DEFAULT_COMPRESSION;
            //            pdfCopyProvider.SetFullCompression();
            //            sourceDocument.Open();
            //            importedPage = pdfCopyProvider.GetImportedPage(reader, i);
            //            pdfCopyProvider.AddPage(importedPage);
            //            pdfCopyProvider.Close();
            //            sourceDocument.Close();
            //        }

            //    }

            //    catch (Exception ex)
            //    {


            //    }
            //    finally
            //    {
            //        pdfCopyProvider.Close();
            //        sourceDocument.Close(); 
            //        reader.Close();
            //       
            //        sourceDocument.Dispose();
            //        pdfCopyProvider.Dispose();
            //        GC.Collect();


            //    }
            //    return TotalPageCount;
        }
    }
}
