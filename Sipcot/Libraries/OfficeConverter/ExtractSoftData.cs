using System;
using System.Data;
using System.Data.OleDb;
namespace OfficeConverter
{
    public class ExtractSoftData
    {
        OleDbConnection oledbConn;

        private void GenerateExcelData(string filepath,string UserID,string TemplateID)
        {
            try
            {
                Logger.Trace("Started Extracting Soft Data", UserID);
                // need to pass relative path after deploying on server
                string path = System.IO.Path.GetFullPath(@"F:\Test.xlsx");
                oledbConn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                  path + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';");
                oledbConn.Open();
                OleDbCommand cmd = new OleDbCommand(); ;
                OleDbDataAdapter oleda = new OleDbDataAdapter();
                DataSet ds = new DataSet();
                cmd.Connection = oledbConn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT [Slno],[FirstName],[LastName] FROM [Sheet1$]";
                oleda = new OleDbDataAdapter(cmd);
                oleda.Fill(ds);
                Logger.Trace("Finished Extracting Soft Data", UserID);
            }
            // need to catch possible exceptions
            catch (Exception ex)
            {
                Logger.TraceErrorLog("UserID : " + UserID + ", - Error : " + ex.Message);

            }
            finally
            {
                oledbConn.Close();
            }
        }
    }
}
