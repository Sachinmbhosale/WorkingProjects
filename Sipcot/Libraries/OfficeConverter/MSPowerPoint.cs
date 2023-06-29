using System;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Core;

namespace OfficeConverter
{
    public class MSPowerPoint
    {
        public void Convert(string FileName, string destinationFileName)
        {
            Application ppApp = new Application();
            Presentation presentation = ppApp.Presentations.Open(FileName, MsoTriState.msoTrue, MsoTriState.msoFalse, MsoTriState.msoFalse);
            presentation.ExportAsFixedFormat(destinationFileName,
                PpFixedFormatType.ppFixedFormatTypePDF,
                PpFixedFormatIntent.ppFixedFormatIntentPrint,
                MsoTriState.msoFalse,
                PpPrintHandoutOrder.ppPrintHandoutHorizontalFirst,
                PpPrintOutputType.ppPrintOutputSlides,
                MsoTriState.msoFalse,
                null,
                PpPrintRangeType.ppPrintAll,
                "",
                false,
                false,
                false,
                true,
                true,
                System.Reflection.Missing.Value);
            presentation.Close();
            presentation = null;
            ppApp = null;
            GC.Collect();
        }
        public int Convert(string FileName, string destinationFileName, bool slice)
        {
            int totalpages;
            Application ppApp = new Application();
            Presentation presentation = ppApp.Presentations.Open(FileName, MsoTriState.msoTrue, MsoTriState.msoFalse, MsoTriState.msoFalse);
            presentation.ExportAsFixedFormat(destinationFileName,
                PpFixedFormatType.ppFixedFormatTypePDF,
                PpFixedFormatIntent.ppFixedFormatIntentPrint,
                MsoTriState.msoFalse,
                PpPrintHandoutOrder.ppPrintHandoutHorizontalFirst,
                PpPrintOutputType.ppPrintOutputSlides,
                MsoTriState.msoFalse,
                null,
                PpPrintRangeType.ppPrintAll,
                "",
                false,
                false,
                false,
                true,
                true,
                System.Reflection.Missing.Value);
            presentation.Close();
            presentation = null;
            ppApp = null;
            GC.Collect();
            totalpages = new Image2Pdf().ExtractPages(destinationFileName);

            return totalpages;
        }
    }
}
