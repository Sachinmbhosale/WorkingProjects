using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
namespace Lotex.IFilter.Parser
{
    public class PdfParser
    {
        public static string Extract(string FileName)
        {
          
            PdfReader reader = new PdfReader(FileName);
            StringBuilder PdfTextExtractBuilder = new StringBuilder();
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                //ITextExtractionStrategy its = new iTextSharp.text.pdf.parser.SimpleTextExtractionStrategy();
                PdfTextExtractBuilder.Append(PdfTextExtractor.GetTextFromPage(reader, page));
            }
            try { reader.Close(); }
            catch { }
            return PdfTextExtractBuilder.ToString();

            
        }
    }
}
