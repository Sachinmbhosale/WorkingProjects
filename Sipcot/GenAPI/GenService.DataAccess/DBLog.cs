using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace GenService.DataAccess
{
    public class Trace
    {
        public string direction;
        public DateTime createddatetime;
        public string URI;
        public string Status;
        public string Headers;
        public string Message;
        public string GUID;
    }
    public class DBLog
    {
        string ConnectionString = string.Empty;
        public DBLog()
        {
            ConnectionString = ConfigurationManager.AppSettings["ConnectionString"].ToString();
        }

        public void InsertTrace(Trace trace)
        {
            try
            {
                using (var DB_Connection = new SqlConnection(ConnectionString))
                {
                    DB_Connection.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = DB_Connection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "USP_INSERT_TRACE";
                    cmd.Parameters.Add("@direction", SqlDbType.VarChar, 50).Value = trace.direction;
                    cmd.Parameters.Add("@createddatetime", SqlDbType.DateTime).Value = trace.createddatetime;
                    cmd.Parameters.Add("@URI", SqlDbType.VarChar, 500).Value = trace.URI;
                    cmd.Parameters.Add("@Status", SqlDbType.VarChar, 50).Value = trace.Status;
                    cmd.Parameters.Add("@Headers", SqlDbType.VarChar, 1000).Value = trace.Headers;
                    cmd.Parameters.Add("@Message", SqlDbType.NVarChar).Value = trace.Message;
                    cmd.Parameters.Add("@GUID", SqlDbType.UniqueIdentifier).Value = new Guid(trace.GUID);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SendTraceToDB(string strFOID, string strMethodName, DateTime ReqDate, string ReqStr, DateTime ResDate, string ResStr, string Error)
        {
            try
            {
                using (var DB_Connection = new SqlConnection(ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = DB_Connection;
                    //Logger.logTrace("Executing Stored Procedure USP_INSERT_TRACE " + " Agent ID : " + strFOID + " Method Name : " + strMethodName + " Request Date : " + ReqDate + " Request String : " + ReqStr + "Response Date : " + ResDate + " Response String :  " + ResStr + " Error : " + Error, 0);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "USP_INSERT_TRACE";
                    cmd.Parameters.Add("@SERVICELOG_vFOID", SqlDbType.VarChar, 20).Value = strFOID;
                    cmd.Parameters.Add("@SERVICELOG_vType", SqlDbType.VarChar, 50).Value = strMethodName;
                    cmd.Parameters.Add("@SERVICELOG_dRequestDatetime", SqlDbType.DateTime).Value = ReqDate;
                    cmd.Parameters.Add("@SERVICELOG_vRequestedValue", SqlDbType.VarChar, 8000).Value = ReqStr;
                    cmd.Parameters.Add("@SERVICELOG_dResponseDatetime", SqlDbType.DateTime).Value = ResDate;
                    cmd.Parameters.Add("@SERVICELOG_vResponseGiven", SqlDbType.VarChar, 8000).Value = ResStr;
                    cmd.Parameters.Add("@SERVICELOG_vRemarks", SqlDbType.VarChar, 8000).Value = Error;
                    cmd.ExecuteNonQuery();
                    //Logger.logTrace("Successfully executed Stored Procedure USP_INSERT_TRACE " + " Agent ID : " + strFOID + " Method Name : " + strMethodName + " Request Date : " + ReqDate + " Request String : " + ReqStr + "Response Date : " + ResDate + " Response String :  " + ResStr + " Error : " + Error, 0);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }    
    }
}
