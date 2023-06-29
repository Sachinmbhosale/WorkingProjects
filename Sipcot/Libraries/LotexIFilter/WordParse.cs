namespace Lotex.IFilter.Parser
{
    public  class WordParse
    {
      public static string extract(string filename)
      {
          Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
          object file = filename; //  path for word file
          object nullobj = System.Reflection.Missing.Value;
          Microsoft.Office.Interop.Word.Document doc = wordApp.Documents.Open(ref file, ref nullobj, ref nullobj,
          ref nullobj, ref nullobj, ref nullobj, ref nullobj, ref nullobj, ref nullobj,
          ref nullobj, ref nullobj, ref nullobj, ref nullobj, ref nullobj, ref nullobj, ref nullobj);
          doc.ActiveWindow.Selection.WholeStory();
          doc.ActiveWindow.Selection.Copy();
          //System.Runtime.InteropServices.ComTypes.IDataObject data = doc.ActiveWindow.Selection.Copy();

          string content = doc.Content.Text;
          //string allText = data.GetData(DataFormats.Text,out o).ToString();
          doc.Close(ref nullobj, ref nullobj, ref nullobj);
          wordApp.Quit(ref nullobj, ref nullobj, ref nullobj);

          return content;

      }
    }
}
