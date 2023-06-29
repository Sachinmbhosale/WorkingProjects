
/**============================================================================  
Author     : Gokuldas.palapatta
Create date: 28/02/2013
===============================================================================  
** Change History   
** Date:        Author:             Issue ID    	Description: 
**(dd MMM yyyy)
** -----------------------------------------------------------------------------
** 10 jun 2015  Gokuldas.palapatta	DMS5-4339	Soft data upload checking for duplicates
**===============================================================================*/
using System;
using System.Collections.Generic;
using System.Data;
using Lotex.EnterpriseSolutions.CoreBE;
using DataAccessLayer;

namespace Lotex.EnterpriseSolutions.CoreDAL
{
    public class BatchUploadDAL : BaseDAL
    {
        public BatchUploadDAL() { }

        public Results ManageFilterOptions(string action, string loginOrgId, string loginToken)
        {
            Results rs = new Results();
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);

            try
            {

                
                dbManager.Open();

                dbManager.CreateParameters(3);
                dbManager.AddParameters(0, "@in_vAction", action);
                dbManager.AddParameters(1, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(2, "@in_iLoginOrgId", loginOrgId);


                using (IDataReader dbRdr = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_GetBatchUploadFilter"))
                {
                    while (dbRdr.Read())
                    {
                        rs.ActionStatus = dbRdr["ActionStatus"].ToString();
                        if (rs.ActionStatus == ActionStatus.SUCCESS.ToString())
                        {
                            if (rs.Items == null)
                            {
                                rs.Items = new List<Item>();
                            }
                            Item item = new Item();
                            item.Key = dbRdr["GEN_iID"].ToString();
                            item.Value = dbRdr["GEN_vDescription"].ToString();
                            rs.Items.Add(item);
                        }
                    }
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
            return rs;

            //SqlDataReader dbRdr = null;
            //SqlCommand dbCmd = null;
            //SqlConnection dbCon = null;
            //Results results = new Results();
            //SqlTransaction myTrans = null;

            //try
            //{
            //    dbCon = OpenConnection();
            //    dbCmd = new SqlCommand();
            //    dbCmd.Connection = dbCon;
            //    myTrans = dbCon.BeginTransaction();
            //    dbCmd.CommandType = CommandType.StoredProcedure;
            //    dbCmd.Transaction = myTrans;

            //    dbCmd.CommandText = "USP_GetBatchUploadFilter";
            //    //used to create user for specific organisation - in a autocreation scenario
            //dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
            //dbCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;
            //dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar, 30).Value = action;

            //    dbRdr = dbCmd.ExecuteReader();

            //    while (dbRdr.Read())
            //    {
            //        results.ActionStatus = dbRdr["ActionStatus"].ToString();
            //        if (results.ActionStatus == ActionStatus.SUCCESS.ToString())
            //        {
            //            if (results.Items == null)
            //            {
            //                results.Items = new List<Item>();
            //            }
            //            Item item = new Item();
            //            item.Key = dbRdr["GEN_iID"].ToString();
            //            item.Value = dbRdr["GEN_vDescription"].ToString();
            //            results.Items.Add(item);
            //        }
            //    }
            //}

            //catch (Exception ex)
            //{
            //    results.Message = ex.ToString();
            //    if (results.ActionStatus != string.Empty)
            //    {
            //        results.ActionStatus = ActionStatus.ERROR.ToString();
            //    }
            //    myTrans.Rollback();
            //    throw ex;
            //}
            //finally
            //{
            //    if (dbRdr != null)
            //    {
            //        dbRdr.Dispose();
            //    }
            //    if (dbCmd != null)
            //    {
            //        dbCmd.Dispose();
            //    }
            //    CloseConnection(dbCon);
            //}
            //return results;
        }

        #region Batch data upload
        public Results UploadBatchData(BatchUploadBE BatchUpload, string action, string loginOrgId, string loginToken)
        {

            Results results = new Results();
            results.ActionStatus = "SUCCESS";
            DataSet Ds = new DataSet();
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);

            try
            {
                string batchData = QueryParser(BatchUpload.batchData, DataProvider.MySql);
                dbManager.Open();

                dbManager.CreateParameters(6);
                dbManager.AddParameters(0, "@in_iDepartmentId", BatchUpload.DepartmentId);
                dbManager.AddParameters(1, "@in_iDocTypeId", BatchUpload.DocTypeId);
                dbManager.AddParameters(2, "@in_xBatchData", batchData);
                dbManager.AddParameters(3, "@in_vAction", action);
                dbManager.AddParameters(4, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(5, "@in_iLoginOrgId", loginOrgId);

                /* DMS5-4339 BS */

                Ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_UploadBatchData");

                results.ActionStatus = Ds.Tables[0].Rows[0][0].ToString();

                /* DMS5-4339 BE */

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }
            return results;



            /*SqlCommand uploadcmd = null;
            SqlConnection dbCon = null;
            Results results = new Results();
            results.ActionStatus = "SUCCESS";
            try
            {

                dbCon = OpenConnection();
                uploadcmd = new SqlCommand();
                uploadcmd.Connection = dbCon;
                uploadcmd.CommandType = CommandType.StoredProcedure;
                uploadcmd.CommandTimeout = 500;
                uploadcmd.CommandText = "USP_UploadBatchData";
                uploadcmd.Parameters.AddWithValue("@in_iDepartmentId", BatchUpload.DepartmentId);
                uploadcmd.Parameters.AddWithValue("@in_xBatchData ", BatchUpload.batchData);
                uploadcmd.Parameters.Add("@in_vAction", SqlDbType.VarChar, 50).Value = action;
                uploadcmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                uploadcmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;
                uploadcmd.ExecuteNonQuery();
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
                if (uploadcmd != null)
                {
                    uploadcmd.Dispose();
                }
                CloseConnection(dbCon);
            }*/
        }
        #endregion

        #region GetBatchUploadedData
        public Results GetBatchUploadedData(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            DataSet ds = new DataSet();
            Results results = new Results();
            results.ActionStatus = "SUCCESS";
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);

            try
            {
                dbManager.Open();

                dbManager.CreateParameters(13);
                dbManager.AddParameters(0, "@in_iFilterId", filter.BatchUploadFilterId);
                dbManager.AddParameters(1, "@in_iDocTypeId", filter.DocumentTypeID);
                dbManager.AddParameters(2, "@in_iCurrTemplateId", filter.CurrTemplateId);
                dbManager.AddParameters(3, "@in_iDepartmentID", filter.DepartmentID);
                dbManager.AddParameters(4, "@PageNo", filter.DocPageNo);
                dbManager.AddParameters(5, "@PageSize", filter.RowsPerPage);
                dbManager.AddParameters(6, "@in_iDeleteId", filter.DeleteID);
                dbManager.AddParameters(7, "@SortColumn", string.Empty);
                dbManager.AddParameters(8, "@SortOrder", string.Empty);
                dbManager.AddParameters(9, "@in_vAction", action);
                dbManager.AddParameters(10, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(11, "@in_iLoginOrgId", loginOrgId);
                dbManager.AddParameters(12, "@totalCount", 0, ParameterDirection.Output);

                ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GetBatchUploadedData");


                BatchUploadBE BatchUpload = new BatchUploadBE();
                BatchUpload.batchData = createHtmlTable(ds);
                if (results.BatchUpload == null)
                {
                    results.BatchUpload = new List<BatchUploadBE>();
                }
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    BatchUpload.RowsCount = ds.Tables[0].Rows.Count;
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        BatchUpload.TotalRowcount = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalRows"].ToString());
                    }
                    results.BatchUpload.Add(BatchUpload);
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
            return results;






            /* SqlDataReader dbRdr = null;
             SqlCommand dbCmd = null;
             SqlConnection dbCon = null;
             Results results = new Results();
             results.ActionStatus = "SUCCESS";
             try
             {
                 dbCon = OpenConnection();
                 dbCmd = new SqlCommand();
                 dbCmd.Connection = dbCon;
                 dbCmd.CommandType = CommandType.StoredProcedure;

                 dbCmd.CommandText = "USP_GetBatchUploadedData";

                 if (filter.BatchUploadFilterId > 0)
                 {
                     dbCmd.Parameters.Add("@in_iFilterId", SqlDbType.Int).Value = filter.BatchUploadFilterId;
                 }
                 if (filter.DocumentTypeID > 0)
                 {
                     dbCmd.Parameters.Add("@in_iDocTypeId", SqlDbType.Int).Value = filter.DocumentTypeID;
                 }
                 if (filter.CurrTemplateId > 0)
                 {
                     dbCmd.Parameters.Add("@in_iCurrTemplateId", SqlDbType.Int).Value = filter.CurrTemplateId;
                 }
                 if (filter.DepartmentID > 0)
                 {
                     dbCmd.Parameters.Add("@in_iDepartmentID", SqlDbType.Int).Value = filter.DepartmentID;
                 }

                 if (filter.DocPageNo != 0)
                 {
                     dbCmd.Parameters.Add("@PageNo", SqlDbType.Int).Value = filter.DocPageNo;
                 }
                 if (filter.RowsPerPage != 0)
                 {
                     dbCmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = filter.RowsPerPage;
                 }

                 dbCmd.Parameters.Add("@in_iDeleteId", SqlDbType.Int).Value = filter.DeleteID;
                 dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar, 30).Value = action;
                 //sesstion
                 dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                 dbCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;

                 SqlDataAdapter da = new SqlDataAdapter();
                 da.SelectCommand = dbCmd;
                 DataSet ds = new DataSet();
                 da.Fill(ds);

                 BatchUploadBE BatchUpload = new BatchUploadBE();
                 BatchUpload.batchData = createHtmlTable(ds);
                 dbRdr = dbCmd.ExecuteReader();
                 if (results.BatchUpload == null)
                 {
                     results.BatchUpload = new List<BatchUploadBE>();
                 }

                 BatchUpload.RowsCount = ds.Tables[0].Rows.Count;
                 if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                 {
                     BatchUpload.TotalRowcount = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalRows"].ToString());
                 }
                 results.BatchUpload.Add(BatchUpload);

             }
             catch (Exception ex)
             {
                 results.Message = ex.ToString();
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
             return results;*/
        }
        #endregion

        public DataSet GetHeadersForBatchUpload(SearchFilter filter, string loginOrgId, string loginToken)
        {
            Results results = new Results();
            results.ActionStatus = "SUCCESS";
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);

            try
            {
                dbManager.Open();

                dbManager.CreateParameters(4);
                dbManager.AddParameters(0, "@in_iDocTypeId", filter.DocumentTypeID);
                dbManager.AddParameters(1, "@in_iDepartmentId", filter.DepartmentID);
                dbManager.AddParameters(2, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(3, "@in_iLoginOrgId", loginOrgId);

                results.ResultDS = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GetHeadersForBatchUpload");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }
            return results.ResultDS;


            /*  SqlDataReader dbRdr = null;
              SqlCommand dbCmd = null;
              SqlConnection dbCon = null;
              Results results = new Results();
              results.ActionStatus = "SUCCESS";
              try
              {
                  dbCon = OpenConnection();
                  dbCmd = new SqlCommand();
                  dbCmd.Connection = dbCon;
                  dbCmd.CommandType = CommandType.StoredProcedure;

                  dbCmd.CommandText = "USP_GetHeadersForBatchUpload";

                  if (filter.DocumentTypeID > 0)
                  {
                      dbCmd.Parameters.Add("@in_iDocTypeId", SqlDbType.Int).Value = filter.DocumentTypeID;
                  }
                  if (filter.DepartmentID > 0)
                  {
                      dbCmd.Parameters.Add("@in_iDepartmentId", SqlDbType.Int).Value = filter.DepartmentID;
                  }

                  //sesstion
                  dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                  dbCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;

                  SqlDataAdapter da = new SqlDataAdapter();
                  da.SelectCommand = dbCmd;
                  DataSet ds = new DataSet();
                  da.Fill(ds);

                  results.ResultDS = ds;

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
              return results.ResultDS;*/
        }

        public bool ColumnExists(IDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Trim().ToUpper() == columnName.Trim().ToUpper())
                {
                    return true;
                }
            }
            return false;
        }


        #region MIS Report
        public DataSet ReportDocumentWise(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            Results results = new Results();
            results.ActionStatus = "SUCCESS";
            DataSet ds = new DataSet();
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);

            try
            {
                dbManager.Open();

                dbManager.CreateParameters(11);
                dbManager.AddParameters(0, "@in_startRowIndex", filter.startRowIndex);
                dbManager.AddParameters(1, "@in_endRowIndex", filter.endRowIndex);
                dbManager.AddParameters(2, "@out_TotalRows", 0, ParameterDirection.Output);
                dbManager.AddParameters(3, "@in_SortExp", filter.PagingSortExp);
                dbManager.AddParameters(4, "@in_iDocTypeId", filter.DocumentTypeID);
                dbManager.AddParameters(5, "@in_iDepartmentID", filter.DepartmentID);
                dbManager.AddParameters(6, "@in_dStartDate", filter.StartDate);
                dbManager.AddParameters(7, "@in_dEndDate", filter.EndDate);
                dbManager.AddParameters(8, "@in_vAction", action);
                dbManager.AddParameters(9, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(10, "@in_iLoginOrgId", loginOrgId);

                ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_ReportDocumentWise");
                {
                    try
                    {
                        filter.PagingTotalRows = (int)dbManager.GetOutputParameterValue("@out_TotalRows");
                    }
                    catch { }
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
            return ds;





            /*  DataSet ds = new DataSet();
              SqlDataAdapter da = new SqlDataAdapter();
              SqlCommand dbCmd = null;
              SqlConnection dbCon = null;
              Results results = new Results();
              results.ActionStatus = "SUCCESS";
              try
              {
                  dbCon = OpenConnection();
                  dbCmd = new SqlCommand();
                  dbCmd.Connection = dbCon;
                  dbCmd.CommandType = CommandType.StoredProcedure;

                  dbCmd.CommandText = "USP_ReportDocumentWise";

                  //Create Parameter Object
                  dbCmd.Parameters.Add("@in_startRowIndex", SqlDbType.Int).Value = filter.startRowIndex;
                  dbCmd.Parameters.Add("@in_endRowIndex", SqlDbType.Int).Value = filter.endRowIndex;
                  dbCmd.Parameters.Add(new SqlParameter("@out_TotalRows", SqlDbType.Int)).Direction = ParameterDirection.Output;

                  //objCmd.Parameters.Add(new SqlParameter
                  //("AreaCur", SqlDbType.RefCursor)).Direction = ParameterDirection.Output;

                  dbCmd.Parameters.Add("@in_SortExp", SqlDbType.VarChar, 30).Value = filter.PagingSortExp;
                  dbCmd.Parameters.Add("@in_iDocTypeId", SqlDbType.Int).Value = filter.DocumentTypeID;
                  dbCmd.Parameters.Add("@in_iDepartmentID", SqlDbType.Int).Value = filter.DepartmentID;
                  dbCmd.Parameters.Add("@in_dStartDate", SqlDbType.VarChar, 15).Value = filter.StartDate;
                  dbCmd.Parameters.Add("@in_dEndDate", SqlDbType.VarChar, 15).Value = filter.EndDate;
                  dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar, 30).Value = action;
                  //sesstion
                  dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                  dbCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;

                  da.SelectCommand = dbCmd;
                  da.Fill(ds);

                  try
                  {
                      filter.PagingTotalRows = (int)dbCmd.Parameters["@out_TotalRows"].Value;
                  }
                  catch { }
              }
              catch (Exception ex)
              {

                  throw ex;
              }
              finally
              {
                  if (ds != null)
                  {
                      ds.Dispose();
                  }
                  if (da != null)
                  {
                      da.Dispose();
                  }
                  if (dbCmd != null)
                  {
                      dbCmd.Dispose();
                  }
                  CloseConnection(dbCon);
              }
              return ds;*/
        }

        public DataSet ReportDocumentTagWise(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            Results results = new Results();
            results.ActionStatus = "SUCCESS";
            DataSet ds = new DataSet();
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);

            try
            {
                dbManager.Open();
              //  dbManager.Command.CommandTimeout=60000;
                dbManager.CreateParameters(9);
                dbManager.AddParameters(0, "@in_startRowIndex", filter.startRowIndex);
                dbManager.AddParameters(1, "@in_endRowIndex", filter.endRowIndex);
                dbManager.AddParameters(2, "@out_TotalRows", 0, ParameterDirection.Output);
                dbManager.AddParameters(3, "@in_SortExp", filter.PagingSortExp);
                //dbManager.AddParameters(4, "@in_iDocTypeId", filter.DocumentTypeID);
                //dbManager.AddParameters(5, "@in_iDepartmentID", filter.DepartmentID);
                dbManager.AddParameters(4, "@in_dStartDate", filter.StartDate);
                dbManager.AddParameters(5, "@in_dEndDate", filter.EndDate);
                dbManager.AddParameters(6, "@in_vAction", action);
                dbManager.AddParameters(7, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(8, "@in_iLoginOrgId", loginOrgId);


                ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_ReportDocumentTagWise");
                {
                    try
                    {
                        filter.PagingTotalRows = (int)dbManager.GetOutputParameterValue("@out_TotalRows");
                    }
                    catch { }
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
            return ds;

        }

        public DataSet AuditReport(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            Results results = new Results();
            results.ActionStatus = "SUCCESS";
            DataSet ds = new DataSet();
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);

            try
            {
                dbManager.Open();

                dbManager.CreateParameters(15);
                dbManager.AddParameters(0, "@in_startRowIndex", filter.startRowIndex);
                dbManager.AddParameters(1, "@in_endRowIndex", filter.endRowIndex);
                dbManager.AddParameters(2, "@out_TotalRows", 0, ParameterDirection.Output);
                dbManager.AddParameters(3, "@in_SortExp", filter.PagingSortExp);
                dbManager.AddParameters(4, "@in_iSelectedUserId", filter.CurrUserId);
                dbManager.AddParameters(5, "@in_iSelectedUserIdstring", filter.SelectedUserIdstring);
                dbManager.AddParameters(6, "@in_Audittype", filter.Audittypes);
                dbManager.AddParameters(7, "@in_iOrgID", filter.CurrOrgId);
                dbManager.AddParameters(8, "@in_iDocuTypeID", filter.DocumentTypeID);
                dbManager.AddParameters(9, "@in_dStartDate", filter.StartDate);
                dbManager.AddParameters(10, "@in_dEndDate", filter.EndDate);
                dbManager.AddParameters(11, "@in_vDocname", filter.Documentname);
                dbManager.AddParameters(12, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(13, "@in_iLoginOrgId", loginOrgId);
                dbManager.AddParameters(14, "@in_vAction", action);

                ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GetAuditDetails");
                {
                    try
                    {
                        filter.PagingTotalRows = (int)dbManager.GetOutputParameterValue("@out_TotalRows");
                    }
                    catch { }
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
            return ds;


            /*  DataSet ds = new DataSet();
              SqlDataAdapter da = new SqlDataAdapter();
              SqlCommand dbCmd = null;
              SqlConnection dbCon = null;
              Results results = new Results();
              results.ActionStatus = "SUCCESS";
              try
              {
                  dbCon = OpenConnection();
                  dbCmd = new SqlCommand();
                  dbCmd.Connection = dbCon;
                  dbCmd.CommandType = CommandType.StoredProcedure;

                  dbCmd.CommandText = "USP_GetAuditDetails";

                  //Create Parameter Object

                  dbCmd.Parameters.Add("@in_startRowIndex", SqlDbType.Int).Value = filter.startRowIndex;
                  dbCmd.Parameters.Add("@in_endRowIndex", SqlDbType.Int).Value = filter.endRowIndex;
                  dbCmd.Parameters.Add(new SqlParameter("@out_TotalRows", SqlDbType.Int)).Direction = ParameterDirection.Output;

                  //objCmd.Parameters.Add(new SqlParameter
                  //("AreaCur", SqlDbType.RefCursor)).Direction = ParameterDirection.Output;

                  dbCmd.Parameters.Add("@in_SortExp", SqlDbType.VarChar, 30).Value = filter.PagingSortExp;
                  if (filter.CurrUserId > 0)
                  {
                      dbCmd.Parameters.Add("@in_iSelectedUserId", SqlDbType.Int).Value = filter.CurrUserId;
                  }
                  if (filter.CurrOrgId > 0)
                  {
                      dbCmd.Parameters.Add("@in_iOrgID", SqlDbType.Int).Value = filter.CurrOrgId;
                  }
                  if (filter.DocumentTypeID > 0)
                  {
                      dbCmd.Parameters.Add("@in_iDocuTypeID", SqlDbType.Int).Value = filter.DocumentTypeID;
                  }
                  if (filter.StartDate != string.Empty)
                  {
                      dbCmd.Parameters.Add("@in_dStartDate", SqlDbType.DateTime, 15).Value = filter.StartDate;
                  }

                  if (filter.EndDate != string.Empty)
                  {
                      dbCmd.Parameters.Add("@in_dEndDate", SqlDbType.DateTime, 15).Value = filter.EndDate;
                  }
                  if (filter.Documentname != string.Empty)
                  {
                      dbCmd.Parameters.Add("@in_vDocname", SqlDbType.VarChar, 15).Value = filter.Documentname;
                  }
                  if (action != string.Empty)
                  {
                      dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar, 100).Value = action;
                  }
                  //sesstion
                  dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                  dbCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;

                  da.SelectCommand = dbCmd;
                  da.Fill(ds);

                  try
                  {
                      filter.PagingTotalRows = (int)dbCmd.Parameters["@out_TotalRows"].Value;
                  }
                  catch { }
              }
              catch (Exception ex)
              {

                  throw ex;
              }
              finally
              {
                  if (ds != null)
                  {
                      ds.Dispose();
                  }
                  if (da != null)
                  {
                      da.Dispose();
                  }
                  if (dbCmd != null)
                  {
                      dbCmd.Dispose();
                  }
                  CloseConnection(dbCon);
              }
              return ds;*/
        }

        public DataSet ComboFillerBySP(string StoredProcedure)
        {

            DataSet ds = new DataSet();
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);

            try
            {
                dbManager.Open();

                ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, StoredProcedure);

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



            /* SqlConnection dbCon = null;
             SqlCommand dbCmd = new SqlCommand();
             DataSet ds = new DataSet();
             SqlDataAdapter da = new SqlDataAdapter();
             try
             {
                 dbCon = OpenConnection();
                 dbCmd = new SqlCommand(StoredProcedure, dbCon);
                 dbCmd.CommandType = CommandType.StoredProcedure;
                 da.SelectCommand = dbCmd;
                 da.Fill(ds);
             }
             catch (Exception ex)
             {
                 throw ex;
             }
             finally
             {
                 if (ds != null)
                 {
                     ds.Dispose();
                 }
                 if (da != null)
                 {
                     da.Dispose();
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

        #region Manage Upload Batches

        public Results ManageUploadBatches(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            Results results = new Results();
            results.ActionStatus = "SUCCESS";

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);

            try
            {
                dbManager.Open();
                dbManager.CreateParameters(15);
                dbManager.AddParameters(0, "@in_startRowIndex", filter.startRowIndex);
                dbManager.AddParameters(1, "@in_endRowIndex", filter.endRowIndex);
                dbManager.AddParameters(2, "@in_SortExp", filter.PagingSortExp);
                dbManager.AddParameters(3, "@in_iDocTypeId", filter.DocumentTypeID);
                dbManager.AddParameters(4, "@in_iDepartmentID", filter.DepartmentID);
                dbManager.AddParameters(5, "@in_vBatchName", filter.BatchName);
                dbManager.AddParameters(6, "@in_iBatchId", filter.BatchId);
                dbManager.AddParameters(7, "@in_xBatchData", filter.BatchData);
                dbManager.AddParameters(8, "@in_vAction", action);
                dbManager.AddParameters(9, "@in_vSubAction", filter.Subaction);
                dbManager.AddParameters(10, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(11, "@in_iLoginOrgId", loginOrgId);
                dbManager.AddParameters(12, "@out_iErrorState", 0, ParameterDirection.Output);
                dbManager.AddParameters(13, "@out_iErrorSeverity", 0, ParameterDirection.Output);
                dbManager.AddParameters(14, "@out_vMessage", string.Empty, DbType.String, 250, ParameterDirection.Output);

                results.ResultDS = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_ManageUploadBatches");

                try
                {
                    results.ErrorState = (int)dbManager.GetOutputParameterValue("@out_iErrorState");
                    results.ErrorSeverity = (int)dbManager.GetOutputParameterValue("@out_iErrorSeverity");
                    results.Message = (string)dbManager.GetOutputParameterValue("@out_vMessage");
                }
                catch { }
                try
                {
                    filter.PagingTotalRows = Convert.ToInt32(results.ResultDS.Tables[0].Rows[0]["Total Rows"]);
                    //filter.PagingTotalRows = (int)dbCmd.Parameters["@out_TotalRows"].Value;
                }
                catch { }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }
            return results;


            /* Results result = new Results();
             SqlDataAdapter da = new SqlDataAdapter();
             SqlCommand dbCmd = null;
             SqlConnection dbCon = null;
             try
             {
                 dbCon = OpenConnection();
                 dbCmd = new SqlCommand();
                 dbCmd.Connection = dbCon;
                 dbCmd.CommandType = CommandType.StoredProcedure;

                 dbCmd.CommandText = "USP_ManageUploadBatches";

                 //Create Parameter Object
                 dbCmd.Parameters.Add("@in_startRowIndex", SqlDbType.Int).Value = filter.startRowIndex;
                 dbCmd.Parameters.Add("@in_endRowIndex", SqlDbType.Int).Value = filter.endRowIndex;
                 //dbCmd.Parameters.Add(new SqlParameter("@out_TotalRows", SqlDbType.Int)).Direction = ParameterDirection.Output;

                 dbCmd.Parameters.Add("@in_SortExp", SqlDbType.VarChar, 30).Value = filter.PagingSortExp;
                 dbCmd.Parameters.Add("@in_iDocTypeId", SqlDbType.Int).Value = filter.DocumentTypeID;
                 dbCmd.Parameters.Add("@in_iDepartmentID", SqlDbType.Int).Value = filter.DepartmentID;
                 dbCmd.Parameters.Add("@in_vBatchName", SqlDbType.VarChar, 50).Value = filter.BatchName;
                 dbCmd.Parameters.Add("@in_iBatchId", SqlDbType.Int).Value = filter.BatchId;
                 dbCmd.Parameters.Add("@in_xBatchData", SqlDbType.Xml).Value = filter.BatchData;
                 dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar, 30).Value = action;
                 if (filter.Subaction != string.Empty)
                 {
                     dbCmd.Parameters.Add("@in_vSubAction", SqlDbType.VarChar, 30).Value = filter.Subaction;
                 }
                 //sesstion
                 dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                 dbCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;

                 // Output parameters
                 dbCmd.Parameters.Add(new SqlParameter("@out_iErrorState", SqlDbType.Int)).Direction = ParameterDirection.Output;
                 dbCmd.Parameters.Add(new SqlParameter("@out_iErrorSeverity", SqlDbType.Int)).Direction = ParameterDirection.Output;
                 dbCmd.Parameters.Add(new SqlParameter("@out_vMessage", SqlDbType.NVarChar, 8000)).Direction = ParameterDirection.Output;

                 da.SelectCommand = dbCmd;
                 da.Fill(result.ResultDS);

                 try
                 {
                     result.ErrorState = (int)dbCmd.Parameters["@out_iErrorState"].Value;
                     result.ErrorSeverity = (int)dbCmd.Parameters["@out_iErrorSeverity"].Value;
                     result.Message = dbCmd.Parameters["@out_vMessage"].Value.ToString();
                 }
                 catch { }
                 try
                 {
                     filter.PagingTotalRows = (int)result.ResultDS.Tables[0].Rows[0]["Total Rows"];
                     //filter.PagingTotalRows = (int)dbCmd.Parameters["@out_TotalRows"].Value;
                 }
                 catch { }
             }
             catch (Exception ex)
             {
                 throw ex;
             }
             finally
             {
                 if (result.ResultDS != null)
                 {
                     result.ResultDS.Dispose();
                 }
                 if (da != null)
                 {
                     da.Dispose();
                 }
                 if (dbCmd != null)
                 {
                     dbCmd.Dispose();
                 }
                 CloseConnection(dbCon);
             }
             return result;*/
        }

        #endregion
    }
}