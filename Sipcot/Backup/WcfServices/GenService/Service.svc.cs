using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using GenServiceDataContracts;
using GenServiceCommon;
using System.Data;
using GenServiceLibrary;

namespace GenService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    public class Service : IService
    {

        protected string CheckResult(DBResult ResultOBJ, string strServiceName)
        {
            // Get output fromate seeting valur from config file
            string OutputFormat = Utility.GetAppSettingValue("OutputFormat");

            if (ResultOBJ.ErrorState == 0)
            {
                if (ResultOBJ.ResultDS.Tables.Count > 0)
                {
                    if (OutputFormat.Trim().ToUpper() == "JSON")
                        return JsonSerde.BuildJsonString(ResultOBJ.ResultDS, strServiceName);
                    if (OutputFormat.Trim().ToUpper() == "XML")
                        return JsonSerde.BuildXmlStringFromDataset(ResultOBJ.ResultDS, strServiceName);
                    else
                        return JsonSerde.BuildXmlStringFromDataset(ResultOBJ.ResultDS, strServiceName);
                }
                else
                {
                    // WebOperationContext.Current.OutgoingResponse.Headers.Add("ecode:" + ResultOBJ.ErrorState);
                    //WebOperationContext.Current.OutgoingResponse.Headers.Add("msg:" + ResultOBJ.Msg);
                    //return @"[{""ecode"":" + ResultOBJ.ErrorState.ToString() + "," + @"""msg"":" + ResultOBJ.Msg + "}]";
                    if (OutputFormat.Trim().ToUpper() == "JSON")
                        return JsonSerde.BuildJsonString(ResultOBJ.ResultDS, strServiceName);
                    if (OutputFormat.Trim().ToUpper() == "XML")
                        return JsonSerde.BuildXmlStringFromDataset(ResultOBJ.ResultDS, strServiceName);
                    else
                        return "";
                }

            }
            else if (ResultOBJ.ErrorState > 0) //SP Error - SP Catch Block
            {
                // WebOperationContext.Current.OutgoingResponse.Headers.Add("ecode:" + ResultOBJ.ErrorState);
                // WebOperationContext.Current.OutgoingResponse.Headers.Add("msg:" + Utility.GetAppSettingValue("ErrorMsg1"));
                // return @"[{""ecode"":" + ResultOBJ.ErrorState.ToString() + "," + @"""msg"":" + Utility.GetAppSettingValue("ErrorMsg1") + "}]";
                if (OutputFormat.Trim().ToUpper() == "JSON")
                    return JsonSerde.BuildJsonString(ResultOBJ.ResultDS, strServiceName);
                if (OutputFormat.Trim().ToUpper() == "XML")
                    return JsonSerde.BuildXmlStringFromDataset(ResultOBJ.ResultDS, strServiceName);
                else
                    return "";
            }
            else //Negative values - returns values that SP returns
            {
             
                if (OutputFormat.Trim().ToUpper() == "JSON")
                    return JsonSerde.BuildJsonString(ResultOBJ.ResultDS, strServiceName);
                if (OutputFormat.Trim().ToUpper() == "XML")
                    return JsonSerde.BuildXmlStringFromDataset(ResultOBJ.ResultDS, strServiceName);
                else
                    return "";
            }
        }

        public string RunStoredProcedureReturnsDataset(StoredProcedureReturnsDataset ProcedureReturnsDataset)
        {
            DBResult Result = new DBResult();
            Crypt crypt = new Crypt();
            try
            {
                Logger.logTrace("Started RunStoredProcedureReturnsDataset",100);
                DataSet ds = new DataSet();
                StoredProcedureLib.SP.RunStoredProcedure(Result, crypt.Decrypt(ProcedureReturnsDataset.ConnectionString_Encrypted), ds,
                    crypt.Decrypt(ProcedureReturnsDataset.ProcedureName_Encrypted), crypt.Decrypt(ProcedureReturnsDataset.ParametersList_Encrypted));
            }
            catch (Exception ex)
            {
                Logger.logTrace("Exception from service" + ex.ToString() + "Result-" + CheckResult(Result, ProcedureReturnsDataset.ProcedureName_Encrypted),100);
                //Message = ex.Message; ErrorState = 1; ErrorSeverity = 0;
                //Result.Msg = Message; Result.ErrorState = ErrorState; Result.ErrorSeverity = ErrorSeverity;
                return CheckResult(Result, ProcedureReturnsDataset.ProcedureName_Encrypted);
                // throw new Exception("Service error : " + ex.Message.ToString());
            }
            //Message = Result.Msg; ErrorState = Result.ErrorState; ErrorSeverity = Result.ErrorSeverity;
            Logger.logTrace("Complted RunStoredProcedureReturns Succesfully" + "Result-" + CheckResult(Result, ProcedureReturnsDataset.ProcedureName_Encrypted),100);
            return CheckResult(Result, ProcedureReturnsDataset.ProcedureName_Encrypted);

        }
       
    }
}
