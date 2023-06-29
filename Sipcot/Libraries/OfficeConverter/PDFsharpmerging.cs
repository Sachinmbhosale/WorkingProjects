using System;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;


public static class PDFsharpmerging
{
    public static void MergeFiles(string destinationFile, string[] sourceFiles)
    {

        try
        {
            int f = 0;
            // we create a reader for a certain document
            PdfReader reader = new PdfReader(sourceFiles[f]);
            // we retrieve the total number of pages
            //int n = reader.NumberOfPages;
            //Console.WriteLine("There are " + n + " pages in the original file.");
            // step 1: creation of a document-object
            Document document = new Document(reader.GetPageSizeWithRotation(1));
            // step 2: we create a writer that listens to the document
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(destinationFile, FileMode.Create));
            // step 3: we open the document
            document.Open();
            PdfContentByte cb = writer.DirectContent;
            PdfImportedPage page;
            int rotation;
            // step 4: we add content
            while (f < sourceFiles.Length)
            {
                int i = 0;
                while (i < 1)
                {
                    i++;
                    document.SetPageSize(reader.GetPageSizeWithRotation(i));
                    document.NewPage();
                    page = writer.GetImportedPage(reader, i);
                    rotation = reader.GetPageRotation(i);
                    if (rotation == 90 || rotation == 270)
                    {
                        cb.AddTemplate(page, 0, -1f, 1f, 0, 0, reader.GetPageSizeWithRotation(i).Height);
                    }
                    else
                    {
                        cb.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
                    }
                    //Console.WriteLine("Processed page " + i);
                }
                f++;
                if (f < sourceFiles.Length)
                {
                    reader = new PdfReader(sourceFiles[f]);
                    // we retrieve the total number of pages
                    //n = reader.NumberOfPages;
                    //Console.WriteLine("There are " + n + " pages in the original file.");
                }
            }
            // step 5: we close the document
            document.Close();
            document.Dispose();
            writer.Close();
            writer.Dispose();
            reader.Close();
           
        }
        catch (Exception e)
        {
            string strOb = e.Message;
        }
    }

    public static int CountPageNo(string strFileName)
    {
        // we create a reader for a certain document
        PdfReader reader = new PdfReader(strFileName);
        // we retrieve the total number of pages
        return reader.NumberOfPages;
    }

    public static void PDFtiff(string source, string destination, string PDFName)
    {
        string RootDirPath = source;
        string PDFDirPath = destination;
        //string TmpFolderpath = System.DateTime.Now.ToString("d").Replace('/', '_');


        // creation of the document with a certain size and certain margins
        iTextSharp.text.Document document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 0, 0, 0, 0);

        // creation of the different writers
        iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, new System.IO.FileStream((PDFDirPath + PDFName + ".pdf"), System.IO.FileMode.Create));

        // load the tiff image and count the total images

        //DirectoryInfo RootDir = new DirectoryInfo(RootDirPath + TmpFolderpath);
        //FileInfo[] files = RootDirPath;

        System.Drawing.Bitmap bm = null;

        document.Open();
      
      
            bm = new System.Drawing.Bitmap(RootDirPath);

            iTextSharp.text.pdf.PdfContentByte cb = writer.DirectContent;
            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(bm, System.Drawing.Imaging.ImageFormat.Bmp);

            img.ScalePercent(72f / img.DpiX * 100);
            img.SetAbsolutePosition(0, 0);
            cb.AddImage(img);


       
        document.Close();
        writer.Close();

        bm.Dispose();
    }
  

}

