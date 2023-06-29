using System;
using Microsoft.Office.Interop.Excel;

namespace OfficeConverter
{
    public class MSExcel
    {
        Application excelApplication = new Application();
        Workbook excelWorkBook = null;

        object paramMissing = Type.Missing;


        XlFixedFormatType paramExportFormat = XlFixedFormatType.xlTypePDF;
        XlFixedFormatQuality paramExportQuality = XlFixedFormatQuality.xlQualityStandard;
        bool paramOpenAfterPublish = false;
        bool paramIncludeDocProps = true;
        bool paramIgnorePrintAreas = true;
        object paramFromPage = Type.Missing;
        object paramToPage = Type.Missing;

        public void Convert(string FileName, string destinationFileName)
        {
            string paramSourceBookPath = FileName;
            string paramExportFilePath = destinationFileName;
            Logger.Trace("FileName :" + FileName + "DestinationFileName : " + destinationFileName, "Excel Application");
            try
            {
                excelWorkBook = excelApplication.Workbooks.Open(paramSourceBookPath,
                    paramMissing, paramMissing, paramMissing, paramMissing,
                    paramMissing, paramMissing, paramMissing, paramMissing,
                    paramMissing, paramMissing, paramMissing, paramMissing,
                    paramMissing, paramMissing);

                if (excelWorkBook != null)
                    excelWorkBook.ExportAsFixedFormat(paramExportFormat,
                        paramExportFilePath, paramExportQuality,
                        paramIncludeDocProps, paramIgnorePrintAreas, paramFromPage,
                        paramToPage, paramOpenAfterPublish,
                        paramMissing);

            }
            catch (Exception ex)
            {
                Logger.Trace(ex.ToString(), "Excel Application");
                throw new Exception(ex.Message);

            }
            finally
            {
                if (excelWorkBook != null)
                {
                    excelWorkBook.Close(false, paramMissing, paramMissing);
                    excelWorkBook = null;
                    Logger.Trace("excelWorkBook_Closed", "Excel Application");
                }

                if (excelApplication != null)
                {

                    excelApplication.Quit();
                    excelApplication = null;
                    Logger.Trace("excelApplication_Closed", "Excel Application");
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public int Convert(string FileName, string destinationFileName, bool slice)
        {
            string paramSourceBookPath = FileName;
            string paramExportFilePath = destinationFileName;
            int totalpages = 0;
            Logger.Trace("FileName :" + FileName + "DestinationFileName : " + destinationFileName, "Excel Application");
            try
            {
                excelWorkBook = excelApplication.Workbooks.Open(paramSourceBookPath,
                    paramMissing, paramMissing, paramMissing, paramMissing,
                    paramMissing, paramMissing, paramMissing, paramMissing,
                    paramMissing, paramMissing, paramMissing, paramMissing,
                    paramMissing, paramMissing);

                if (excelWorkBook != null)
                    excelWorkBook.ExportAsFixedFormat(paramExportFormat,
                        paramExportFilePath, paramExportQuality,
                        paramIncludeDocProps, paramIgnorePrintAreas, paramFromPage,
                        paramToPage, paramOpenAfterPublish,
                        paramMissing);
                totalpages = new Image2Pdf().ExtractPages(paramExportFilePath);
            }
            catch (Exception ex)
            {
                Logger.Trace(ex.ToString(), "Excel Application");
                throw new Exception(ex.Message);

            }
            finally
            {
                if (excelWorkBook != null)
                {
                    excelWorkBook.Close(false, paramMissing, paramMissing);
                    excelWorkBook = null;
                    Logger.Trace("excelWorkBook_Closed", "Excel Application");
                }

                if (excelApplication != null)
                {

                    excelApplication.Quit();
                    excelApplication = null;
                    Logger.Trace("excelApplication_Closed", "Excel Application");
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
