using System;
using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Data;

namespace OfficeConverter
{
    public class PDFMerging
    {
        public static void MergeFiles(string destinationFile, string[] sourceFiles, int position, string action)
        {
            try
            {
                // we create a reader for a certain document
                PdfReader reader = new PdfReader(sourceFiles[0]);
                PdfReader reader2 = new PdfReader(sourceFiles[1]);
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
                int file1count = CountPageNo(sourceFiles[0]);
                if (action == "add")
                {
                    if (position == 0)
                    {
                        position = 1;
                    }
                    int file2count = CountPageNo(sourceFiles[1]);
                    for (int fc = 1; fc <= file1count; fc++)
                    {
                        if (fc == position)
                        {

                            for (int sc = 1; sc <= file2count; sc++)
                            {
                                document.SetPageSize(reader2.GetPageSizeWithRotation(sc));
                                document.NewPage();
                                page = writer.GetImportedPage(reader2, sc);
                                rotation = reader2.GetPageRotation(sc);
                                if (rotation == 90 || rotation == 270)
                                {
                                    cb.AddTemplate(page, 0, -1f, 1f, 0, 0, reader2.GetPageSizeWithRotation(sc).Height);
                                }
                                else
                                {
                                    cb.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
                                }
                            }
                        }
                        document.SetPageSize(reader.GetPageSizeWithRotation(fc));
                        document.NewPage();
                        page = writer.GetImportedPage(reader, fc);
                        rotation = reader.GetPageRotation(fc);
                        if (rotation == 90 || rotation == 270)
                        {
                            cb.AddTemplate(page, 0, -1f, 1f, 0, 0, reader.GetPageSizeWithRotation(fc).Height);
                        }
                        else
                        {
                            cb.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
                        }


                    }
                }
                else if (action == "delete")
                {
                    for (int fc = 1; fc <= file1count; fc++)
                    {
                        if (fc == position)
                        {
                            continue;
                        }
                        document.SetPageSize(reader.GetPageSizeWithRotation(fc));
                        document.NewPage();
                        page = writer.GetImportedPage(reader, fc);
                        rotation = reader.GetPageRotation(fc);
                        if (rotation == 90 || rotation == 270)
                        {
                            cb.AddTemplate(page, 0, -1f, 1f, 0, 0, reader.GetPageSizeWithRotation(fc).Height);
                        }
                        else
                        {
                            cb.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
                        }
                    }
                }
                document.Close();

               
                writer.Dispose();
                document.Dispose();
                
                GC.Collect();
                //reader.Close();
                //reader2.Close();
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
            reader.Close();
           
            return reader.NumberOfPages;
        }

        public static void deleteMergeFiles(string destinationFile, string sourceFiles, int position, string action)
        {
            try
            {
                // we create a reader for a certain document
                PdfReader reader = new PdfReader(sourceFiles);
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
                int file1count = CountPageNo(sourceFiles);
                for (int fc = 1; fc <= file1count; fc++)
                {
                    if (fc == position)
                    {
                        continue;
                    }
                    document.SetPageSize(reader.GetPageSizeWithRotation(fc));
                    document.NewPage();
                    page = writer.GetImportedPage(reader, fc);
                    rotation = reader.GetPageRotation(fc);
                    if (rotation == 90 || rotation == 270)
                    {
                        cb.AddTemplate(page, 0, -1f, 1f, 0, 0, reader.GetPageSizeWithRotation(fc).Height);
                    }
                    else
                    {
                        cb.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
                    }
                }
                document.Close();

               
                writer.Dispose();
                document.Dispose();
                GC.Collect();
            }
            catch (Exception e)
            {
                string strOb = e.Message;

            }
        }

        public static void MergeFilesForbulk(string destinationFile, string[] sourceFiles)
        {

            try
            {
                int f = 0;
                // we create a reader for a certain document
                PdfReader reader = new PdfReader(sourceFiles[f]);
                // we retrieve the total number of pages
                int n = reader.NumberOfPages;
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
                    while (i < n)
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
                        n = reader.NumberOfPages;
                        //Console.WriteLine("There are " + n + " pages in the original file.");
                    }
                }
                // step 5: we close the document
                document.Close();
            }
            catch (Exception e)
            {
                string strOb = e.Message;
            }
        }

        public static void MergeFilesDownload(string destinationFile, string sourceFilesPath)
        {

            try
            {

                DirectoryInfo di = new DirectoryInfo(sourceFilesPath);
                int pagecount = di.GetFiles("*.pdf", SearchOption.AllDirectories).Length;

                FileInfo[] files = di.GetFiles("*.pdf", SearchOption.AllDirectories);
                CombineMultiplePDFs(files, destinationFile);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// used for merging //
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>


        public static void MergeFilesDownload(string destinationFile, string sourceFilesPath, DataTable DT, int totalpages)
        {

            try
            {

                DirectoryInfo di = new DirectoryInfo(sourceFilesPath);
                int pagecount = di.GetFiles("*.pdf", SearchOption.AllDirectories).Length;
                int index = 0;
                FileInfo[] files = di.GetFiles("*.pdf", SearchOption.AllDirectories);
                string FileN = string.Empty;
                FileInfo[] fileArray = new FileInfo[totalpages];
                if (DT.Rows.Count > 0)
                {
                    foreach (FileInfo fileName in files)
                    {
                        // fileName.Name;
                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            int l = fileName.Name.ToString().LastIndexOf(".");
                            FileN = fileName.Name.ToString().Substring(0, l);

                            if (DT.Rows[i]["PageNumbers"].ToString() == FileN)
                            {
                                fileArray[index] = new FileInfo(fileName.FullName);
                                index++;
                            }
                        }

                    }
                    CombineMultiplePDFs(fileArray, destinationFile);
                }
                else
                {
                    CombineMultiplePDFs(files, destinationFile);
                }






            }
            catch
            {
                throw;
            }
        }
        public static void CombineMultiplePDFs(FileInfo[] fileNames, string outFile)
        {
            if (fileNames != null && fileNames.Length > 0)
            {
                // Sort by number
                Array.Sort(fileNames, new MyCustomComparer());

                // step 1: creation of a document-object
                Document document = new Document();

                // step 2: we create a writer that listens to the document
                PdfCopy writer = new PdfCopy(document, new FileStream(outFile, FileMode.Create));
                if (writer == null)
                {
                    return;
                }

                // step 3: we open the document
                document.Open();

                foreach (FileInfo fileName in fileNames)
                {
                    // we create a reader for a certain document
                    PdfReader reader = new PdfReader(fileName.FullName);
                    reader.ConsolidateNamedDestinations();

                    // step 4: we add content
                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        PdfImportedPage page = writer.GetImportedPage(reader, i);
                        writer.AddPage(page);
                    }

                    PRAcroForm form = reader.AcroForm;
                    if (form != null)
                    {
                        writer.CopyAcroForm(reader);
                    }

                    reader.Close();
                }

                // step 5: we close the document and writer
                writer.Close();
                document.Close();
            }
            else
            {
                throw new Exception("CombineMultiplePDFs - Files array is empty.");
            }
        }

    }

    public class MyCustomComparer : IComparer<FileInfo>
    {
        public int Compare(FileInfo x, FileInfo y)
        {
            // split filename
            string[] parts1 = x.Name.Split('-');
            string[] parts2 = y.Name.Split('-');

            // calculate how much leading zeros we need
            int toPad1 = 10 - parts1[0].Length;
            int toPad2 = 10 - parts2[0].Length;

            // add the zeros, only for sorting
            parts1[0] = parts1[0].Insert(0, new String('0', toPad1));
            parts2[0] = parts2[0].Insert(0, new String('0', toPad2));

            // create the comparable string
            string toCompare1 = string.Join("", parts1);
            string toCompare2 = string.Join("", parts2);

            // compare
            return toCompare1.CompareTo(toCompare2);
        }
    }
}
