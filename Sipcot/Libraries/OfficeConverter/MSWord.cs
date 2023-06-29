using System;
using Microsoft.Office.Interop.Word;


namespace OfficeConverter
{
    public class MSWord
    {
        Application wordApplication = new Application();
        Document wordDocument = null;
        object paramMissing = Type.Missing;
        
        WdExportFormat paramExportFormat = WdExportFormat.wdExportFormatPDF;
        bool paramOpenAfterExport = false;
        WdExportOptimizeFor paramExportOptimizeFor = WdExportOptimizeFor.wdExportOptimizeForPrint;
        WdExportRange paramExportRange = WdExportRange.wdExportAllDocument;
        int paramStartPage = 0;
        int paramEndPage = 0;
        WdExportItem paramExportItem = WdExportItem.wdExportDocumentContent;
        bool paramIncludeDocProps = true;
        bool paramKeepIRM = true;
        WdExportCreateBookmarks paramCreateBookmarks = WdExportCreateBookmarks.wdExportCreateWordBookmarks;
        bool paramDocStructureTags = true;
        bool paramBitmapMissingFonts = true;
        bool paramUseISO19005_1 = false;

        public void Convert(string FileName, string destinationFileName)
        {
            object paramSourceDocPath = FileName;
            string paramExportFilePath = destinationFileName;

            try
            {
                Logger.Trace(FileName, "WordConvertion");
                wordDocument = wordApplication.Documents.Open(
                    ref paramSourceDocPath, ref paramMissing, ref paramMissing,
                    ref paramMissing, ref paramMissing, ref paramMissing,
                    ref paramMissing, ref paramMissing, ref paramMissing,
                    ref paramMissing, ref paramMissing, ref paramMissing,
                    ref paramMissing, ref paramMissing, ref paramMissing,
                    ref paramMissing);

                if (wordDocument != null)
                    wordDocument.ExportAsFixedFormat(paramExportFilePath,
                        paramExportFormat, paramOpenAfterExport,
                        paramExportOptimizeFor, paramExportRange, paramStartPage,
                        paramEndPage, paramExportItem, paramIncludeDocProps,
                        paramKeepIRM, paramCreateBookmarks, paramDocStructureTags,
                        paramBitmapMissingFonts, paramUseISO19005_1,
                        ref paramMissing);
            }
            catch (Exception ex)
            {
                Logger.TraceErrorLog(ex.ToString());
            }
            finally
            {
                if (wordDocument != null)
                {
                    wordDocument.Close(ref paramMissing, ref paramMissing,
                        ref paramMissing);
                    wordDocument = null;
                }

                if (wordApplication != null)
                {
                    wordApplication.Quit(ref paramMissing, ref paramMissing,
                        ref paramMissing);
                    wordApplication = null;
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
        public int Convert(string FileName, string destinationFileName, bool slice)
        {
            object paramSourceDocPath = FileName;
            string paramExportFilePath = destinationFileName;
            int totalpages = 0;
            

            try
            {
                Logger.Trace(FileName, "WordConvertion");
                wordDocument = wordApplication.Documents.Open(
                    ref paramSourceDocPath, ref paramMissing, ref paramMissing,
                    ref paramMissing, ref paramMissing, ref paramMissing,
                    ref paramMissing, ref paramMissing, ref paramMissing,
                    ref paramMissing, ref paramMissing, ref paramMissing,
                    ref paramMissing, ref paramMissing, ref paramMissing,
                    ref paramMissing);

                if (wordDocument != null)
                {
                    wordDocument.ExportAsFixedFormat(paramExportFilePath,
                        paramExportFormat, paramOpenAfterExport,
                        paramExportOptimizeFor, paramExportRange, paramStartPage,
                        paramEndPage, paramExportItem, paramIncludeDocProps,
                        paramKeepIRM, paramCreateBookmarks, paramDocStructureTags,
                        paramBitmapMissingFonts, paramUseISO19005_1,
                        ref paramMissing);
                    totalpages = new Image2Pdf().ExtractPages(paramExportFilePath);
                }


            }

            catch (Exception ex)
            {
                Logger.TraceErrorLog(ex.ToString());
            }
            finally
            {
                if (wordDocument != null)
                {
                    wordDocument.Close(ref paramMissing, ref paramMissing,
                        ref paramMissing);
                    wordDocument = null;
                }

                if (wordApplication != null)
                {
                    wordApplication.Quit(ref paramMissing, ref paramMissing,
                        ref paramMissing);
                    wordApplication = null;
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return totalpages;
        }
    }
}
