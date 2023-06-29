using System;
using GenService.Service;
using GenServiceDataContractLibrary;
using System.IO;

namespace GenAPI
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Document" in code, svc and config file together.
    public class Document : IDocument
    {
        public ServiceData GetDocumentInfo(ServiceData data)
        {
            try
            {
                SP SP = new SP(data.API_Key);

                Logger.logTrace("Calling ExecuteDataSet() to get data from database.", data.API_Key);
                data = SP.ExecuteDataSet(data,
                    "GenAPI_ConnectionString",
                    "USP_API_GetData");
                Logger.logTrace("Completed ExecuteDataSet() method execution.", data.API_Key);
            }
            catch (Exception ex)
            {
                Logger.logTrace("Exception: " + ex.Message, data.API_Key);
                data.ErrorState = 1;
                data.Message = ex.Message.Replace("Procedure or function 'USP_API_GetData'", "API ");
            }
            return data;
        }

        public ServiceData DownloadDocument(ServiceData data)
        {
            try
            {
                SP SP = new SP(data.API_Key);
                data.FilePath = data.FileContent = string.Empty;

                Logger.logTrace("Calling ExecuteDataSet() to get data from database.", data.API_Key);
                data = SP.ExecuteDataSet(data,
                    "GenAPI_ConnectionString",
                    "USP_API_GetData");
                Logger.logTrace("Completed ExecuteDataSet() method execution.", data.API_Key);

                if (data.Resultsets != null && data.Resultsets.Tables.Count > 0 && data.Resultsets.Tables[0].Rows.Count > 0)
                {
                    data.FilePath = data.Resultsets.Tables[0].Rows[0]["FilePath"].ToString();
                    Logger.logTrace("Resultset has values. FilePath: " + data.FilePath, data.API_Key);
                }
                else
                    Logger.logTrace("Resultset is empty.", data.API_Key);

                if (data.FilePath.Length > 0)
                {
                    Logger.logTrace("Checking file exists. FilePath: " + data.FilePath, data.API_Key);
                    if (File.Exists(data.FilePath))
                    {
                        Logger.logTrace("File exist. FilePath: " + data.FilePath, data.API_Key);
                        Logger.logTrace("Calling FileUtility.ConvertFileToBase4String() to convert image to Base4String. FilePath: " + data.FilePath, data.API_Key);
                        data.FileContent = FileUtility.ConvertFileToBase4String(data.FilePath);
                    }
                    else
                        Logger.logTrace("File doesn't exist. FilePath: " + data.FilePath, data.API_Key);
                }
            }
            catch (Exception ex)
            {
                Logger.logTrace("Exception: " + ex.Message, data.API_Key);
                data.ErrorState = 1;
                data.Message = ex.Message.Replace("Procedure or function 'USP_API_GetData'", "API ");
            }
            return data;
        }
    }
}
