using System;
using System.Data;
using DataAccessLayer;

namespace Lotex.EnterpriseSolutions.CoreDAL
{
    public class DocumentMailDAL : BaseDAL
    {
        /// <summary>
        /// To get the mail details to send the mail to endusers who want to download document.
        /// </summary>
        /// <param name="strTo"></param>
        /// <param name="strSubject"></param>
        /// <param name="strMailBody"></param>
        /// <param name="dtsendtimeAt"></param>
        /// <param name="dtCreatedDate"></param>
        /// <param name="strDescription"></param>
        /// <param name="dtsendTime"></param>
        /// <param name="bActive"></param>
        /// <param name="inAttempts"></param>
        /// <returns></returns>
        public DataSet SendDocumentMail(string strAction, int inMailId, string strTo, string strSubject, string strMailBody, string strToken, string strDownloadlink, string Token, int OrgId, out string message, out int ErrorState, out int ErrorSeverity)
        {
            DataSet dsMailDetails = new DataSet();

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);

            try
            {
                dbManager.Open();
                dbManager.CreateParameters(12);
                dbManager.AddParameters(0, "@in_vAction", strAction);
                dbManager.AddParameters(1, "@in_iMailId", inMailId);
                dbManager.AddParameters(2, "@in_vTo", strTo);
                dbManager.AddParameters(3, "@in_vSubject", strSubject);
                dbManager.AddParameters(4, "@in_vMailBody", strMailBody);
                dbManager.AddParameters(5, "@in_vToken", strToken);
                dbManager.AddParameters(6, "@in_vDownloadLink", strDownloadlink);
                dbManager.AddParameters(7, "@in_vLoginToken", Token);
                dbManager.AddParameters(8, "@in_iLoginOrgId", OrgId);
                dbManager.AddParameters(9, "@out_vErrorMessage", string.Empty, DbType.String, 250, ParameterDirection.Output);
                dbManager.AddParameters(10, "@out_iErrorSeverity", 0, ParameterDirection.Output);
                dbManager.AddParameters(11, "@out_iErrorState", 0, ParameterDirection.Output);

                dsMailDetails = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "usp_Mail_ManageMail");
                message = dbManager.GetOutputParameterValue("@out_vErrorMessage").ToString();
                ErrorState = Convert.ToInt32(dbManager.GetOutputParameterValue("@out_iErrorState"));
                ErrorSeverity = Convert.ToInt32(dbManager.GetOutputParameterValue("@out_iErrorSeverity"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }
            return dsMailDetails;

            /* SqlCommand dbCmd = null;
             SqlConnection dbCon = null;
             SqlDataAdapter sqlda = new SqlDataAdapter();
             DataSet dsMailDetails = new DataSet();
             try
             {
                 dbCon = OpenConnection();
                 dbCmd = new SqlCommand();
                 dbCmd.Connection = dbCon;
                 dbCmd.CommandType = CommandType.StoredProcedure;

                 dbCmd.CommandText = "usp_Mail_ManageMail";
                 dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar).Value = strAction;
                 dbCmd.Parameters.Add("@in_iMailId", SqlDbType.Int).Value = inMailId;
                 dbCmd.Parameters.Add("@in_vTo", SqlDbType.VarChar).Value = strTo;
                 dbCmd.Parameters.Add("@in_vSubject", SqlDbType.VarChar).Value = strSubject;
                 dbCmd.Parameters.Add("@in_vMailBody", SqlDbType.VarChar).Value = strMailBody;               
                 dbCmd.Parameters.Add("@in_vToken", SqlDbType.VarChar).Value = strToken;
                 dbCmd.Parameters.Add("@in_vDownloadLink", SqlDbType.VarChar).Value = strDownloadlink;
                 dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar).Value = Token;
                 dbCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = OrgId;
                 dbCmd.Parameters.Add("@out_vErrorMessage", SqlDbType.NVarChar, 4000).Direction = ParameterDirection.Output;
                 dbCmd.Parameters.Add("@out_iErrorSeverity", SqlDbType.Int).Direction = ParameterDirection.Output;
                 dbCmd.Parameters.Add("@out_iErrorState", SqlDbType.Int).Direction = ParameterDirection.Output;

                 sqlda.SelectCommand = dbCmd;
                 sqlda.Fill(dsMailDetails);
                 // Get output parameter values
                 message = dbCmd.Parameters["@out_vErrorMessage"].Value.ToString();
                 ErrorState = Convert.ToInt32(dbCmd.Parameters["@out_iErrorState"].Value);
                 ErrorSeverity = Convert.ToInt32(dbCmd.Parameters["@out_iErrorSeverity"].Value);
             }
             catch (Exception)
             {

                 throw;
             }
             finally
             {
                 if (dbCmd != null)
                 {
                     dbCmd.Dispose();
                 }
                 CloseConnection(dbCon);
             }
             return dsMailDetails;*/
        }

        public DataSet GetDocumentLinkBasedOnToken(string strToken, string strAction)
        {

            DataSet dsData = new DataSet();

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);

            try
            {
                dbManager.Open();
                dbManager.CreateParameters(3);
                dbManager.AddParameters(0, "@in_vToken", strToken);
                dbManager.AddParameters(1, "@in_vAction", strAction);
                dbManager.AddParameters(2, "@out_vErrorMessage", string.Empty, DbType.String, 250, ParameterDirection.Output);
                dsData = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_ManageDocumentDownload");

            }

            catch (Exception ex)
            {
                throw ex;
            }

            finally
            {
                dbManager.Dispose();
            }

            return dsData;

            /* SqlCommand dbCmd = null;
             SqlConnection dbCon = null;
             SqlDataAdapter sqlda = new SqlDataAdapter();
             DataSet dsData = new DataSet();
             try
             {
                 dbCon = OpenConnection();
                 dbCmd = new SqlCommand();
                 dbCmd.Connection = dbCon;
                 dbCmd.CommandType = CommandType.StoredProcedure;

                 dbCmd.CommandText = "USP_ManageDocumentDownload";
                 dbCmd.Parameters.Add("@in_vToken", SqlDbType.VarChar).Value = strToken;
                 dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar).Value = strAction;
                 //dbCmd.Parameters.Add("@out_vErrorMessage", SqlDbType.NVarChar, 4000).Direction = ParameterDirection.Output;
                 //dbCmd.Parameters.Add("@out_iErrorSeverity", SqlDbType.Int).Direction = ParameterDirection.Output;
                 //dbCmd.Parameters.Add("@out_iErrorState", SqlDbType.Int).Direction = ParameterDirection.Output;
                 sqlda.SelectCommand = dbCmd;
                 sqlda.Fill(dsData);

                 // Get output parameter values
                 //message = dbCmd.Parameters["@out_vErrorMessage"].Value.ToString();
                 //ErrorState = Convert.ToInt32(dbCmd.Parameters["@out_iErrorState"].Value);
                 //ErrorSeverity = Convert.ToInt32(dbCmd.Parameters["@out_iErrorSeverity"].Value);
             }
             catch (Exception)
             {

                 throw;
             }
             return dsData;*/
        }
    }
}
