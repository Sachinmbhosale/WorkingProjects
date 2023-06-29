
//  Date        Programmer        Issue           Description
//  16/07/2013	gokuldas           997           Viewed count is not reflecting in the log report




using System;
using System.Data;
using Lotex.EnterpriseSolutions.CoreBE;
using DataAccessLayer;

namespace Lotex.EnterpriseSolutions.CoreDAL
{
    public class UploadDocDAL : BaseDAL
    {
        public UploadDocDAL() { }



        #region GetUploadDocumentDetailswithProcessID
        public DataSet GetUploadDocumentDetails(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            DataSet ds = new DataSet();

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(4);
                dbManager.AddParameters(0, "@in_iProcessID", filter.UploadDocID);
                dbManager.AddParameters(1, "@in_vAction", action);
                dbManager.AddParameters(2, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(3, "@in_iLoginOrgId", loginOrgId);

                ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GetUploadDocumentDetails");


            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }
            return ds;

            /* SqlDataReader dbRdr = null;
             SqlCommand dbCmd = null;
             SqlConnection dbCon = null;
             DataSet ds = new DataSet();
         
           
             try
             {

                 dbCon = OpenConnection();
                 dbCmd = new SqlCommand();
                 dbCmd.Connection = dbCon;
                 dbCmd.CommandType = CommandType.StoredProcedure;

                 dbCmd.CommandText = "USP_GetUploadDocumentDetails";
                 dbCmd.Parameters.Add("@in_iProcessID", SqlDbType.Int).Value = filter.UploadDocID;
                 dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar, 40).Value = action;
                 //session
                 dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                 dbCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;

                 SqlDataAdapter DA = new SqlDataAdapter();
                 DA.SelectCommand = dbCmd;
                 DA.Fill(ds);

                
             }
             catch (Exception ex)
             {
                 throw ex;
             }
             finally
             {
                 if (dbRdr != null)
                 {
                     dbRdr.Dispose();
                 }
                 if (dbCmd != null)
                 {
                     dbCmd.Dispose();
                 }
                 CloseConnection(dbCon);
             }
             return ds;*/
        }
        #endregion
        #region DiscardUploadDocument
        public Results DiscardUploadDocument(SearchFilter filter, string action, string loginOrgId, string loginToken, string archiveAction)
        {
            Results results = new Results();
            results.ActionStatus = "SUCCESS";

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(7);               
                dbManager.AddParameters(0, "@in_vDocVirPath", filter.DocVirPath);
                dbManager.AddParameters(1, "@in_vDocPhyPath", filter.DocPhyPath);
                dbManager.AddParameters(2, "@in_vAction", action);
                dbManager.AddParameters(3, "@in_vArchiveAction", archiveAction);
                dbManager.AddParameters(4, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(5, "@in_iLoginOrgId", loginOrgId);
                dbManager.AddParameters(6, "@in_iProcessID", filter.UploadDocID);
                dbManager.ExecuteNonQuery(CommandType.StoredProcedure, "Usp_archiveuploadeddocument");
                return results;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }

            /* SqlCommand dbCmd = null;
             SqlConnection dbCon = null;
             Results results = new Results();
             results.ActionStatus = "SUCCESS";
             try
             {
                 dbCon = OpenConnection();
                 dbCmd = new SqlCommand();
                 dbCmd.Connection = dbCon;
                 dbCmd.CommandType = CommandType.StoredProcedure;

                 dbCmd.CommandText = "Usp_archiveuploadeddocument";
                 dbCmd.Parameters.Add("@in_iProcessID", SqlDbType.Int).Value = filter.UploadDocID;
                 dbCmd.Parameters.Add("@in_vDocVirPath", SqlDbType.VarChar, 400).Value = filter.DocVirPath;
                 dbCmd.Parameters.Add("@in_vDocPhyPath", SqlDbType.VarChar, 400).Value = filter.DocPhyPath;
                 dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar, 30).Value = action;
                 //Added newly for version control
                 dbCmd.Parameters.Add("@in_vArchiveAction", SqlDbType.VarChar, 30).Value = archiveAction;
                 //sesstion
                 dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                 dbCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;


                 dbCmd.ExecuteNonQuery();
                 //results.ActionStatus = dbRdr["ActionStatus"].ToString();
                 return results;
             }
             catch (Exception ex)
             {
                 results.Message = ex.ToString();
                 throw ex;
             }
             finally
             {
                 if (dbCmd != null)
                 {
                     dbCmd.Dispose();
                 }
                 CloseConnection(dbCon);
             }*/
        }
        #endregion
        #region UpdateTrackTableOnDownload
        public Results UpdateTrackTableOnDownload(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            Results results = new Results();
            results.ActionStatus = "SUCCESS";

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(4);

                dbManager.AddParameters(0, "@in_iProcessID", filter.UploadDocID);
                dbManager.AddParameters(1, "@in_vAction", action);
                dbManager.AddParameters(2, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(3, "@in_iLoginOrgId", loginOrgId);

                dbManager.ExecuteNonQuery(CommandType.StoredProcedure, "USP_UpdateTrackTableOnDownload");
                return results;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }

            /*SqlCommand dbCmd = null;
            SqlConnection dbCon = null;
            Results results = new Results();
            results.ActionStatus = "SUCCESS";
            try
            {
                dbCon = OpenConnection();
                dbCmd = new SqlCommand();
                dbCmd.Connection = dbCon;
                dbCmd.CommandType = CommandType.StoredProcedure;

                dbCmd.CommandText = "USP_UpdateTrackTableOnDownload";
                dbCmd.Parameters.Add("@in_iProcessID", SqlDbType.Int).Value = filter.UploadDocID;
                dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar, 30).Value = action;
                //sesstion
                dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                dbCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;


                dbCmd.ExecuteNonQuery();
                //results.ActionStatus = dbRdr["ActionStatus"].ToString();
                return results;
            }
            catch (Exception ex)
            {
                results.Message = ex.ToString();
                throw ex;
            }
            finally
            {
                if (dbCmd != null)
                {
                    dbCmd.Dispose();
                }
                CloseConnection(dbCon);
            }*/
        }
        #endregion
        //997BS
        #region UpdateTrackTableOnviewd
        public Results UpdateTrackTableOnviewd(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            Results results = new Results();
            results.ActionStatus = "SUCCESS";

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(4);

                dbManager.AddParameters(0, "@in_iProcessID", filter.UploadDocID);
                dbManager.AddParameters(1, "@in_vAction", action);
                dbManager.AddParameters(2, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(3, "@in_iLoginOrgId", loginOrgId);

                dbManager.ExecuteNonQuery(CommandType.StoredProcedure, "USP_DOCUMENTVIEWCOUNT");
                return results;


            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }
            /*SqlCommand dbCmd = null;
            SqlConnection dbCon = null;
            Results results = new Results();
            results.ActionStatus = "SUCCESS";
            try
            {
                dbCon = OpenConnection();
                dbCmd = new SqlCommand();
                dbCmd.Connection = dbCon;
                dbCmd.CommandType = CommandType.StoredProcedure;

                dbCmd.CommandText = "USP_DOCUMENTVIEWCOUNT";
                dbCmd.Parameters.Add("@in_iProcessID", SqlDbType.Int).Value = filter.UploadDocID;
                dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar, 30).Value = action;
                //sesstion
                dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                dbCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;


                dbCmd.ExecuteNonQuery();
                //results.ActionStatus = dbRdr["ActionStatus"].ToString();
                return results;
            }
            catch (Exception ex)
            {
                results.Message = ex.ToString();
                throw ex;
            }
            finally
            {
                if (dbCmd != null)
                {
                    dbCmd.Dispose();
                }
                CloseConnection(dbCon);
            }*/
        }
        #endregion

        //997BE


        #region UpdateDocumentDetails
        public void UpdateDocumentDetails(int processID, string ArchivedPath, string loginOrgId, string loginToken, string archiveAction)
        {

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(5);
                dbManager.AddParameters(0, "@in_iProcessID", processID);
                dbManager.AddParameters(1, "@in_vArchivedPath", ArchivedPath);
                dbManager.AddParameters(2, "@in_vArchiveAction", archiveAction);
                dbManager.AddParameters(3, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(4, "@in_iLoginOrgId", loginOrgId);

                dbManager.ExecuteNonQuery(CommandType.StoredProcedure, "USP_UpdateDocumentDetails");


            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }
            /* SqlCommand uploadcmd = null;
             SqlConnection dbCon = null;           
           
             try
             {

                 dbCon = OpenConnection();
                 uploadcmd = new SqlCommand();
                 uploadcmd.Connection = dbCon;
                 uploadcmd.CommandType = CommandType.StoredProcedure;               
                     uploadcmd.CommandText = "USP_UpdateDocumentDetails";
                     uploadcmd.Parameters.AddWithValue("@in_iProcessID",SqlDbType.Int).Value=processID;
                     uploadcmd.Parameters.AddWithValue("@in_vArchivedPath", SqlDbType.VarChar).Value = ArchivedPath;
                     uploadcmd.Parameters.Add("@in_vArchiveAction", SqlDbType.VarChar).Value = archiveAction;
                     //sesstion
                     uploadcmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                     uploadcmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;                  
                     uploadcmd.ExecuteNonQuery();                   
                
               
             }
             catch (Exception ex)
             {
                 throw ex;
             }
             finally
             {
                 if (uploadcmd != null)
                 {
                     uploadcmd.Dispose();
                 }
                 CloseConnection(dbCon);
             }*/
        }
        #endregion

        //Newly added for enchment4
        /// <summary>
        /// To lock the document for each user when the document need to be edited
        /// </summary>
        /// <param name="action"></param>
        /// <param name="processId"></param>
        /// <param name="userId"></param>
        /// <param name="message"></param>
        /// <param name="ErrorState"></param>
        /// <param name="ErrorSeverity"></param>
        /// <returns></returns>

        public DataSet UpdateDocumentStatusForLock(string action, int processId, int userId, string token, int orgId, out string message, out int ErrorState, out int ErrorSeverity)
        {

            DataSet dsFields = new DataSet();

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                if (action == "Lock Document")
                {
                    dbManager.Open();
                    dbManager.CreateParameters(8);
                    dbManager.AddParameters(0, "@in_vAction", action);
                    dbManager.AddParameters(1, "@in_iUploadProcessId", processId);
                    dbManager.AddParameters(2, "@in_iCurrentUser", userId);
                    dbManager.AddParameters(3, "@in_vLoginToken", token);
                    dbManager.AddParameters(4, "@in_iLoginOrgId", orgId);

                    dbManager.AddParameters(5, "@out_iErrorState", 0, ParameterDirection.Output);
                    dbManager.AddParameters(6, "@out_iErrorSeverity", 0, ParameterDirection.Output);
                    dbManager.AddParameters(7, "@out_vErrorMessage", string.Empty, DbType.String, 250, ParameterDirection.Output);

                    dsFields = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_UpdateDocumentStatusForLock");

                    // Get output parameter values
                    message = dbManager.GetOutputParameterValue("@out_vErrorMessage").ToString();
                    ErrorState = Convert.ToInt32(dbManager.GetOutputParameterValue("@out_iErrorState"));
                    ErrorSeverity = Convert.ToInt32(dbManager.GetOutputParameterValue("@out_iErrorSeverity"));

                }

                else if (action == "UnLock Document")
                {

                    dbManager.Open();
                    dbManager.CreateParameters(8);
                    dbManager.AddParameters(0, "@in_vAction", action);
                    dbManager.AddParameters(1, "@in_iUploadProcessId", processId);
                    dbManager.AddParameters(2, "@in_iCurrentUser", userId);
                    dbManager.AddParameters(3, "@in_vLoginToken", token);
                    dbManager.AddParameters(4, "@in_iLoginOrgId", orgId);
                    dbManager.AddParameters(5, "@out_iErrorState", 0, ParameterDirection.Output);
                    dbManager.AddParameters(6, "@out_iErrorSeverity", 0, ParameterDirection.Output);
                    dbManager.AddParameters(7, "@out_vErrorMessage", string.Empty, DbType.String, 250, ParameterDirection.Output);

                    dsFields = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_UpdateDocumentStatusForLock");

                    // Get output parameter values
                    message = dbManager.GetOutputParameterValue("@out_vErrorMessage").ToString();
                    ErrorState = Convert.ToInt32(dbManager.GetOutputParameterValue("@out_iErrorState"));
                    ErrorSeverity = Convert.ToInt32(dbManager.GetOutputParameterValue("@out_iErrorSeverity"));
                }
                else
                {
                    message = "Invalid action.";
                    ErrorState = 0;
                    ErrorSeverity = 0;
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }

            return dsFields;
            /*SqlCommand sqlcmd = null;
            SqlConnection dbCon = null;
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet dsFields = new DataSet();

            try
            {
                dbCon = OpenConnection();
                sqlcmd = new SqlCommand();
                sqlcmd.Connection = dbCon;
                sqlcmd.CommandType = CommandType.StoredProcedure;
                if (action == "Lock Document")
                {
                    sqlcmd.CommandText = "USP_UpdateDocumentStatusForLock";
                    sqlcmd.Parameters.Add("@in_vAction", SqlDbType.VarChar).Value = action;
                    sqlcmd.Parameters.Add("@in_iUploadProcessId", SqlDbType.Int).Value = processId;
                    sqlcmd.Parameters.Add("@in_iCurrentUser", SqlDbType.Int).Value = userId;
                    sqlcmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar).Value = token;
                    sqlcmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = orgId;
                    sqlcmd.Parameters.Add("@out_vErrorMessage", SqlDbType.NVarChar, 4000).Direction = ParameterDirection.Output;
                    sqlcmd.Parameters.Add("@out_iErrorSeverity", SqlDbType.Int).Direction = ParameterDirection.Output;
                    sqlcmd.Parameters.Add("@out_iErrorState", SqlDbType.Int).Direction = ParameterDirection.Output;
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    da.SelectCommand = sqlcmd;
                    da.Fill(dsFields);

                    // Get output parameter values
                    message = sqlcmd.Parameters["@out_vErrorMessage"].Value.ToString();
                    ErrorState = Convert.ToInt32(sqlcmd.Parameters["@out_iErrorState"].Value);
                    ErrorSeverity = Convert.ToInt32(sqlcmd.Parameters["@out_iErrorSeverity"].Value);

                }
                else if (action == "UnLock Document")
                {
                    sqlcmd.CommandText = "USP_UpdateDocumentStatusForLock";
                    sqlcmd.Parameters.Add("@in_vAction", SqlDbType.VarChar).Value = action;
                    sqlcmd.Parameters.Add("@in_iUploadProcessId", SqlDbType.Int).Value = processId;
                    sqlcmd.Parameters.Add("@in_iCurrentUser", SqlDbType.Int).Value = userId;
                    sqlcmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar).Value = token;
                    sqlcmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = orgId;
                    sqlcmd.Parameters.Add("@out_vErrorMessage", SqlDbType.NVarChar, 4000).Direction = ParameterDirection.Output;
                    sqlcmd.Parameters.Add("@out_iErrorSeverity", SqlDbType.Int).Direction = ParameterDirection.Output;
                    sqlcmd.Parameters.Add("@out_iErrorState", SqlDbType.Int).Direction = ParameterDirection.Output;
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    da.SelectCommand = sqlcmd;
                    da.Fill(dsFields);

                    // Get output parameter values
                    message = sqlcmd.Parameters["@out_vErrorMessage"].Value.ToString();
                    ErrorState = Convert.ToInt32(sqlcmd.Parameters["@out_iErrorState"].Value);
                    ErrorSeverity = Convert.ToInt32(sqlcmd.Parameters["@out_iErrorSeverity"].Value);
                }
                else
                {
                    message = "Invalid action.";
                    ErrorState = 0;
                    ErrorSeverity = 0;
                }

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (sqlcmd != null)
                {
                    sqlcmd.Dispose();
                }
                CloseConnection(dbCon);
            }
            return dsFields;*/

        }
    }
}
