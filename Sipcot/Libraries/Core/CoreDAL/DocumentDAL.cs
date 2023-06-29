
/* ===================================================================================================================================
 * Author     : 
 * Create date: 
 * Description:  
======================================================================================================================================  
 * Change History   
 * Date:            Author:             Issue ID            Description:  
 * ----------       -------------       ----------          ------------------------------------------------------------------

 * 15 06 2015       gokuldas.palapatta  DMS5-4370	        Version Control has been implemented, but system does not have the menu to view/retrieve earlier versions of the document.( a new button and function added in java script to redirect
====================================================================================================================================== */
using System;
using System.Collections.Generic;
using System.Data;
using Lotex.EnterpriseSolutions.CoreBE;
using DataAccessLayer;

namespace Lotex.EnterpriseSolutions.CoreDAL
{
    public class DocumentDAL : BaseDAL
    {
        public DocumentDAL() { }

        //REFID Generation
        public string GetDocumentReferenceId(SearchFilter Entityobj)
        {
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(5);

                dbManager.AddParameters(0, "@Doctype", Entityobj.DocumentTypeName);
                dbManager.AddParameters(1, "@Deptname", Entityobj.DepartmentName);
                dbManager.AddParameters(2, "@userid", Convert.ToInt32(Entityobj.CurrUserId));
                dbManager.AddParameters(3, "@orgid", Convert.ToInt32(Entityobj.CurrOrgId));
                dbManager.AddParameters(4, "@refid", string.Empty, DbType.String, 50, ParameterDirection.Output);

                dbManager.ExecuteNonQuery(CommandType.StoredProcedure, "SP_CreateRefid");
                string Generatedid = Convert.ToString(dbManager.GetOutputParameterValue("@refid"));
                return Generatedid;

            }
            catch
            {
                return "ERROR";
            }
            finally
            {
                dbManager.Dispose();
            }
        }

        //Upload document details to Upload Table
        public int ManageDocumentUpload(string XMLUploadData, string xmlPageNoMappings, string LoginOrgId, string LoginToken, string sAction, int iProcessID)
        {
            int output = 0;
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(12);
                XMLUploadData = XMLUploadData.Replace("\\", "\\\\");
                XMLUploadData = XMLUploadData.Replace("&apos;", "'");
                XMLUploadData = XMLUploadData.Replace("&", "&amp;");

                dbManager.AddParameters(0, "@XMLUploadData", XMLUploadData);
                dbManager.AddParameters(1, "@in_xPageNoMappings", xmlPageNoMappings);
                dbManager.AddParameters(2, "@in_xTagPageNoMappings", string.Empty);
                dbManager.AddParameters(3, "@in_bSeperatedSplitting", false);
                dbManager.AddParameters(4, "@in_iLoginOrgId", Convert.ToInt32(LoginOrgId));
                dbManager.AddParameters(5, "@in_vLoginToken", LoginToken);
                dbManager.AddParameters(6, "@in_vAction", sAction);
                dbManager.AddParameters(7, "@in_iProcessID", iProcessID);
                dbManager.AddParameters(8, "@out_iErrorState", 0, ParameterDirection.Output);
                dbManager.AddParameters(9, "@out_iErrorSeverity", 0, ParameterDirection.Output);
                dbManager.AddParameters(10, "@out_vMessage", string.Empty, DbType.String, 250, ParameterDirection.Output);
                dbManager.AddParameters(11, "@output", 0, ParameterDirection.Output);

                dbManager.ExecuteNonQuery(CommandType.StoredProcedure, "SP_UploadDocDetails");
                output = Convert.ToInt32(dbManager.GetOutputParameterValue("@output"));

                //results.ErrorState = Convert.ToInt32(dbManager.GetOutputParameterValue("@out_iErrorState"));
                //results.ErrorSeverity = Convert.ToInt32(dbManager.GetOutputParameterValue("@out_iErrorSeverity"));
                //results.Message = Convert.ToString(dbManager.GetOutputParameterValue("@out_vMessage"));

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }
            return output;
        }


        #region SearchDocuments
        public Results SearchDocuments(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            Results results = new Results();
            results.ActionStatus = "SUCCESS";
            DataSet ds = new DataSet();

            int? MainTagID = null;
            MainTagID = filter.MainTagID > 1 ? filter.MainTagID : MainTagID;
            int? SubTagID = null;
            SubTagID = filter.SubTagID > 1 ? filter.SubTagID : SubTagID;

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(15);
                dbManager.AddParameters(0, "@in_iDocID", filter.UploadDocID);
                dbManager.AddParameters(1, "@in_iDocuTypeID", filter.DocumentTypeID);
                dbManager.AddParameters(2, "@in_iDepartmentID", filter.DepartmentID);
                dbManager.AddParameters(3, "@in_vSearchType", filter.SearchOption);
                dbManager.AddParameters(4, "@in_vRefID", filter.Refid);
                dbManager.AddParameters(5, "@in_vKeywords", filter.keywords);
                dbManager.AddParameters(6, "@in_iActive", filter.Active);
                dbManager.AddParameters(7, "@in_iMainTagID", MainTagID);
                dbManager.AddParameters(8, "@in_iSubTagID", SubTagID);
                dbManager.AddParameters(9, "@PageNo", filter.DocPageNo);
                dbManager.AddParameters(10, "@PageSize", filter.RowsPerPage);
                dbManager.AddParameters(11, "@in_vWhereClause", filter.WhereClause);
                dbManager.AddParameters(12, "@in_vAction", action);
                dbManager.AddParameters(13, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(14, "@in_iLoginOrgId", loginOrgId);




                if (action == "GetTaggedPages")
                {
                    using (IDataReader dbRdr = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_SearchDocuments"))
                    {
                        while (dbRdr.Read())
                        {
                            if (results.IndexFields == null)
                            {
                                results.IndexFields = new List<IndexField>();
                            }
                            IndexField Index = new IndexField();
                            Index.ListItemId = Convert.ToInt32(dbRdr["PageNo"]);
                            results.IndexFields.Add(Index);
                        }
                    }
                }
                else
                {
                    ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_SearchDocuments");
                    results.ResultDS = ds;

                    DocumentDownload DocumentDownloadDetails = new DocumentDownload();
                    DocumentDownloadDetails.HtmlTable = createHtmlTable(ds);
                    if (results.DocumentDownloads == null)
                    {
                        results.DocumentDownloads = new List<DocumentDownload>();
                    }

                    DocumentDownloadDetails.RowsCount = ds.Tables[0].Rows.Count;
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        DocumentDownloadDetails.TotalRowcount = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalRows"].ToString());
                    }

                    results.DocumentDownloads.Add(DocumentDownloadDetails);
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

            /* SqlDataAdapter da = new SqlDataAdapter();
             DataSet ds = new DataSet();

             SqlDataReader dbRdr = null;
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

                 dbCmd.CommandText = "USP_SearchDocuments";
                 dbCmd.Parameters.Add("@in_iCurrUserId", SqlDbType.Int).Value = filter.CurrUserId;
                 if (filter.UploadDocID > 0)
                 {
                     dbCmd.Parameters.Add("@in_iDocID", SqlDbType.Int).Value = filter.UploadDocID;
                 }
                 if (filter.DocumentTypeID > 0)
                 {
                     dbCmd.Parameters.Add("@in_iDocuTypeID", SqlDbType.Int).Value = filter.DocumentTypeID;
                 }
                 if (filter.DepartmentID > 0)
                 {
                     dbCmd.Parameters.Add("@in_iDepartmentID", SqlDbType.Int).Value = filter.DepartmentID;
                 }
                 if (filter.SearchOption != string.Empty)
                 {
                     dbCmd.Parameters.Add("@in_vSearchType ", SqlDbType.VarChar, 150).Value = filter.SearchOption;
                 }
                 if (filter.Refid != string.Empty)
                 {
                     dbCmd.Parameters.Add("@in_vRefID ", SqlDbType.VarChar, 150).Value = filter.Refid;
                 }
                 if (filter.keywords != string.Empty)
                 {
                     dbCmd.Parameters.Add("@in_vKeywords", SqlDbType.VarChar, 150).Value = filter.keywords;
                 }
                 if (filter.Active != 0)
                 {
                     dbCmd.Parameters.Add("@in_iActive", SqlDbType.Int).Value = filter.Active;
                 }
                 if (filter.MainTagID != 0)
                 {
                     dbCmd.Parameters.Add("@in_iMainTagID", SqlDbType.Int).Value = filter.MainTagID;
                 }
                 if (filter.SubTagID != 0)
                 {
                     dbCmd.Parameters.Add("@in_iSubTagID", SqlDbType.Int).Value = filter.SubTagID;
                 }
                 if (filter.DocPageNo != 0)
                 {
                     dbCmd.Parameters.Add("@PageNo", SqlDbType.Int).Value = filter.DocPageNo;
                 }
                 if (filter.RowsPerPage != 0)
                 {
                     dbCmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = filter.RowsPerPage;
                 }
                 if (filter.WhereClause != string.Empty)
                 {
                     dbCmd.Parameters.Add("@in_vWhereClause", SqlDbType.NVarChar).Value = filter.WhereClause;
                 }

                 dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar, 30).Value = action;
                 //sesstion
                 dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                 dbCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;



                 if (action == "GetTaggedPages")
                 {
                     dbRdr = dbCmd.ExecuteReader();

                     while (dbRdr.Read())
                     {
                         if (results.IndexFields == null)
                         {
                             results.IndexFields = new List<IndexField>();
                         }
                         IndexField Index = new IndexField();
                         Index.ListItemId = Convert.ToInt32(dbRdr["PageNo"]);
                         results.IndexFields.Add(Index);
                     }
                 }
                 else
                 {
                     da.SelectCommand = dbCmd;
                     da.Fill(ds);
                     results.ResultDS = ds;

                     DocumentDownload DocumentDownloadDetails = new DocumentDownload();
                     DocumentDownloadDetails.HtmlTable = createHtmlTable(ds);
                     if (results.DocumentDownloads == null)
                     {
                         results.DocumentDownloads = new List<DocumentDownload>();
                     }

                     DocumentDownloadDetails.RowsCount = ds.Tables[0].Rows.Count;
                     if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                     {
                         DocumentDownloadDetails.TotalRowcount = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalRows"].ToString());
                     }

                     results.DocumentDownloads.Add(DocumentDownloadDetails);               
                 }
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
        public Results SearchDocumentsForMakerChecker(SearchFilter filter, string action, string loginOrgId, string loginToken, string Makerchecker)
        {
            Results results = new Results();
            results.ActionStatus = "SUCCESS";
            DataSet ds = new DataSet();

            int? MainTagID = null;
            MainTagID = filter.MainTagID > 1 ? filter.MainTagID : MainTagID;
            int? SubTagID = null;
            SubTagID = filter.SubTagID > 1 ? filter.SubTagID : SubTagID;

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(17);
                dbManager.AddParameters(0, "@in_iCurrUserId", filter.CurrUserId);
                dbManager.AddParameters(1, "@in_iDocID", filter.UploadDocID);
                dbManager.AddParameters(2, "@in_iDocuTypeID", filter.DocumentTypeID);
                dbManager.AddParameters(3, "@in_iDepartmentID", filter.DepartmentID);
                dbManager.AddParameters(4, "@in_vSearchType", filter.SearchOption);
                dbManager.AddParameters(5, "@in_vMakerChecker", Makerchecker);
                dbManager.AddParameters(6, "@in_vRefID", filter.Refid);
                dbManager.AddParameters(7, "@in_vKeywords", filter.keywords);
                dbManager.AddParameters(8, "@in_iActive", filter.Active);
                dbManager.AddParameters(9, "@in_iMainTagID", MainTagID);
                dbManager.AddParameters(10, "@in_iSubTagID", SubTagID);
                dbManager.AddParameters(11, "@PageNo", filter.DocPageNo);
                dbManager.AddParameters(12, "@PageSize", filter.RowsPerPage);
                dbManager.AddParameters(13, "@in_vWhereClause", filter.WhereClause);
                dbManager.AddParameters(14, "@in_vAction", action);
                dbManager.AddParameters(15, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(16, "@in_iLoginOrgId", loginOrgId);

                if (action == "GetTaggedPages")
                {
                    using (IDataReader dbRdr = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_GetDataForMakerChecker"))
                    {


                        while (dbRdr.Read())
                        {
                            if (results.IndexFields == null)
                            {
                                results.IndexFields = new List<IndexField>();
                            }
                            IndexField Index = new IndexField();
                            Index.ListItemId = Convert.ToInt32(dbRdr["PageNo"]);
                            results.IndexFields.Add(Index);
                        }
                    }
                }
                else
                {
                    ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GetDataForMakerChecker");

                    DocumentDownload DocumentDownloadDetails = new DocumentDownload();
                    DocumentDownloadDetails.HtmlTable = createHtmlTable(ds);
                    if (results.DocumentDownloads == null)
                    {
                        results.DocumentDownloads = new List<DocumentDownload>();
                    }

                    DocumentDownloadDetails.RowsCount = ds.Tables[0].Rows.Count;
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        DocumentDownloadDetails.TotalRowcount = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalRows"].ToString());
                    }

                    results.DocumentDownloads.Add(DocumentDownloadDetails);
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

            /* SqlDataAdapter da = new SqlDataAdapter();
             DataSet ds = new DataSet();

             SqlDataReader dbRdr = null;
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

                 dbCmd.CommandText = "USP_GetDataForMakerChecker";
                 dbCmd.Parameters.Add("@in_iCurrUserId", SqlDbType.Int).Value = filter.CurrUserId;
                 if (filter.UploadDocID > 0)
                 {
                     dbCmd.Parameters.Add("@in_iDocID", SqlDbType.Int).Value = filter.UploadDocID;
                 }
                 if (filter.DocumentTypeID > 0)
                 {
                     dbCmd.Parameters.Add("@in_iDocuTypeID", SqlDbType.Int).Value = filter.DocumentTypeID;
                 }
                 if (filter.DepartmentID > 0)
                 {
                     dbCmd.Parameters.Add("@in_iDepartmentID", SqlDbType.Int).Value = filter.DepartmentID;
                 }
                 if (filter.SearchOption != string.Empty)
                 {
                     dbCmd.Parameters.Add("@in_vSearchType ", SqlDbType.VarChar, 150).Value = filter.SearchOption;
                 }
                 if (Makerchecker != string.Empty)
                 {
                     dbCmd.Parameters.Add("@in_vMakerChecker ", SqlDbType.VarChar, 50).Value = Makerchecker;
                 }
                 if (filter.Refid != string.Empty)
                 {
                     dbCmd.Parameters.Add("@in_vRefID ", SqlDbType.VarChar, 150).Value = filter.Refid;
                 }
                 if (filter.keywords != string.Empty)
                 {
                     dbCmd.Parameters.Add("@in_vKeywords", SqlDbType.VarChar, 150).Value = filter.keywords;
                 }
                 if (filter.Active != 0)
                 {
                     dbCmd.Parameters.Add("@in_iActive", SqlDbType.Int).Value = filter.Active;
                 }
                 if (filter.MainTagID != 0)
                 {
                     dbCmd.Parameters.Add("@in_iMainTagID", SqlDbType.Int).Value = filter.MainTagID;
                 }
                 if (filter.SubTagID != 0)
                 {
                     dbCmd.Parameters.Add("@in_iSubTagID", SqlDbType.Int).Value = filter.SubTagID;
                 }
                 if (filter.DocPageNo != 0)
                 {
                     dbCmd.Parameters.Add("@PageNo", SqlDbType.Int).Value = filter.DocPageNo;
                 }
                 if (filter.RowsPerPage != 0)
                 {
                     dbCmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = filter.RowsPerPage;
                 }
                 if (filter.WhereClause != string.Empty)
                 {
                     dbCmd.Parameters.Add("@in_vWhereClause", SqlDbType.NVarChar).Value = filter.WhereClause;
                 }

                 dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar, 30).Value = action;
                 //sesstion
                 dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                 dbCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;



                 if (action == "GetTaggedPages")
                 {
                     dbRdr = dbCmd.ExecuteReader();

                     while (dbRdr.Read())
                     {
                         if (results.IndexFields == null)
                         {
                             results.IndexFields = new List<IndexField>();
                         }
                         IndexField Index = new IndexField();
                         Index.ListItemId = Convert.ToInt32(dbRdr["PageNo"]);
                         results.IndexFields.Add(Index);
                     }
                 }
                 else
                 {
                     da.SelectCommand = dbCmd;
                     da.Fill(ds);

                     DocumentDownload DocumentDownloadDetails = new DocumentDownload();
                     DocumentDownloadDetails.HtmlTable = createHtmlTable(ds);
                     if (results.DocumentDownloads == null)
                     {
                         results.DocumentDownloads = new List<DocumentDownload>();
                     }

                     DocumentDownloadDetails.RowsCount = ds.Tables[0].Rows.Count;
                     if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                     {
                         DocumentDownloadDetails.TotalRowcount = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalRows"].ToString());
                     }

                     results.DocumentDownloads.Add(DocumentDownloadDetails);
                 }
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

        public Results ManageTagDetails(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {

            Results results = new Results();
            DataSet ds = new DataSet();



            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(9);
                dbManager.AddParameters(0, "@in_iDocumentId", filter.UploadDocID);
                dbManager.AddParameters(1, "@in_iTotalPages", filter.TotalPages);
                dbManager.AddParameters(2, "@in_xtagPages", filter.taggedPages);
                dbManager.AddParameters(3, "@in_iMainTagId", filter.MainTagID);
                dbManager.AddParameters(4, "@in_iSubTagId", filter.SubTagID);
                dbManager.AddParameters(5, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(6, "@in_iLoginOrgId", loginOrgId);
                dbManager.AddParameters(7, "@in_vAction", action);
                dbManager.AddParameters(8, "@in_xPageNoMappings", filter.xPageNoMappings);

                if (action == "GetTaggedPages")
                {
                    ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_Manage_Tags");
                    results.ResultDS = ds;
                    //results.ActionStatus = ds.Tables[0].Rows[0]["ActionStatus"].ToString();
                    //results.Message = ds.Tables[0].Rows[0]["Message"].ToString();
                    //results.TotalTagPages = Convert.ToInt32(ds.Tables[0].Rows[0]["NewPageNo"].ToString());
                    //results.TotalTagPages = Convert.ToInt32(ds.Tables[0].Rows.Count);
                    using (IDataReader dbRdr = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_Manage_Tags"))
                    {
                        if (dbRdr.Read())
                        {
                            results.ActionStatus = dbRdr["ActionStatus"].ToString();
                            results.Message = dbRdr["Message"].ToString();
                            //results.TotalTagPages = Convert.ToInt32(dbRdr["TotalTagPages"].ToString());

                        }
                    }

                }

                else
                {
                    IDataReader dbRdr = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_Manage_Tags");
                    if (dbRdr.Read())
                    {
                        results.ActionStatus = dbRdr["ActionStatus"].ToString();
                        results.Message = dbRdr["Message"].ToString();
                        //results.TotalTagPages = Convert.ToInt32(dbRdr["TotalTagPages"].ToString());
                        
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
            return results;

            /* SqlDataReader dbRdr = null;
             SqlDataAdapter sqlda = new SqlDataAdapter();
             DataSet ds = new DataSet();
             SqlCommand dbCmd = null;
             SqlConnection dbCon = null;
             Results results = new Results();
             try
             {
                 dbCon = OpenConnection();
                 dbCmd = new SqlCommand();
                 dbCmd.Connection = dbCon;
                 dbCmd.CommandType = CommandType.StoredProcedure;
                 dbCmd.CommandText = "USP_Manage_Tags";
                 dbCmd.Parameters.Add("@in_iDocumentId", SqlDbType.Int).Value = filter.UploadDocID;
                 dbCmd.Parameters.Add("@in_iTotalPages", SqlDbType.Int).Value = filter.TotalPages;
                 dbCmd.Parameters.Add("@in_xtagPages", SqlDbType.VarChar).Value = filter.taggedPages;
                 dbCmd.Parameters.Add("@in_iMainTagId", SqlDbType.Int).Value = filter.MainTagID;
                 dbCmd.Parameters.Add("@in_iSubTagId", SqlDbType.Int).Value = filter.SubTagID;
                 dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                 dbCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;
                 dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar, 30).Value = action;
                 //add or delete tagged pages
                 if (filter.xPageNoMappings!=string.Empty && filter.xPageNoMappings!=null)
                 {
                     dbCmd.Parameters.Add("@in_xPageNoMappings", SqlDbType.VarChar).Value = filter.xPageNoMappings;
                 }
               
                 if (action=="GetTaggedPages")
                 {
                     sqlda.SelectCommand = dbCmd;
                     sqlda.Fill(ds);                   
                     results.ResultDS = ds;
                     dbRdr = dbCmd.ExecuteReader();
                     dbRdr.NextResult();
                 }
                 else
                 {
                     dbRdr = dbCmd.ExecuteReader();
                 }
                 if (dbRdr.Read())
                 {
                     results.ActionStatus = dbRdr["ActionStatus"].ToString();
                    // results.TotalTagPages = Convert.ToInt32(dbRdr["TotalTagPages"].ToString());
                 }
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

        public Results GetTagDetails(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            Results results = new Results();
            
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(5);
                dbManager.AddParameters(0, "@in_iDocumentId", filter.UploadDocID);
                dbManager.AddParameters(1, "@in_iCurrentPageNo", filter.PageId);
                dbManager.AddParameters(2, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(3, "@in_iLoginOrgId", loginOrgId);
                dbManager.AddParameters(4, "@in_vAction", action);

                using (IDataReader dbRdr = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_GETTAGGINGDETAILS"))
                {
                    if (dbRdr.Read())
                    {
                        results.ActionStatus = "SUCCESS";
                        results.TotalTagPages = Convert.ToInt32(dbRdr["TagPageCount"].ToString());
                        results.PageMainTagID = Convert.ToInt32(dbRdr["MainTAGID"].ToString());
                        results.PageSubTagID = Convert.ToInt32(dbRdr["SubTAGID"].ToString());
                    }
                    else
                    {
                        results.ActionStatus = "SUCCESS";
                        results.PageMainTagID = 0;
                        results.PageSubTagID = 0;
                    }
                    if (dbRdr != null)
                    {
                        dbRdr.Dispose();
                    }
                }
                if (results.ActionStatus == "SUCCESS")
                {
                    dbManager.Open();
                    dbManager.CreateParameters(3);
                    dbManager.AddParameters(0, "@in_iDocumentId", filter.UploadDocID);
                    dbManager.AddParameters(1, "@in_iCurrentPageNo", filter.PageId);
                    dbManager.AddParameters(2, "@in_iDepartmentId", filter.DepartmentID);

                    using (IDataReader dbRdr = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_GETMAINTAGWITHPAGENO"))
                    {
                        while (dbRdr.Read())
                        {
                            results.ActionStatus = "SUCCESS";
                            if (results.MainTag == null)
                            {
                                results.MainTag = new List<MainTag>();
                            }
                            MainTag Main = new MainTag();
                            Main.MainTagID = Convert.ToInt32(dbRdr["TagId"].ToString());
                            Main.MainTagValue = dbRdr["TagValue"].ToString();
                            results.MainTag.Add(Main);
                        }
                    }

                }

                //if (results.ActionStatus == "SUCCESS")
                //{

                //    dbManager.Open();
                //    dbManager.CreateParameters(2);
                //    dbManager.AddParameters(0, "@in_iDocumentId", filter.UploadDocID);
                //    dbManager.AddParameters(1, "@in_iCurrentPageNo", filter.PageId);


                //    using (IDataReader dbRdr = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_GETSUBTAGWITHPAGENO"))
                //    {
                //        while (dbRdr.Read())
                //        {
                //            results.ActionStatus = "SUCCESS";
                //            if (results.SubTag == null)
                //            {
                //                results.SubTag = new List<SubTag>();
                //            }
                //            SubTag Sub = new SubTag();
                //            Sub.SubTagID = Convert.ToInt32(dbRdr["TagId"].ToString());
                //            Sub.SubTagValue = dbRdr["TagValue"].ToString();
                //            results.SubTag.Add(Sub);
                //        }
                //    }



                //}


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

             try
             {
                 dbCon = OpenConnection();
                 dbCmd = new SqlCommand();
                 dbCmd.Connection = dbCon;
                 dbCmd.CommandType = CommandType.StoredProcedure;
                 dbCmd.CommandText = "USP_GETTAGGINGDETAILS";
                 dbCmd.Parameters.Add("@in_iDocumentId", SqlDbType.Int).Value = filter.UploadDocID;
                 dbCmd.Parameters.Add("@in_iCurrentPageNo", SqlDbType.Int).Value = filter.PageId;
                 dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                 dbCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;
                 dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar, 30).Value = action;
                 //results.ActionStatus = "SUCCESS";
                 dbRdr = dbCmd.ExecuteReader();

                 if (dbRdr.Read())
                 {
                     results.ActionStatus = "SUCCESS";
                     results.TotalTagPages = Convert.ToInt32(dbRdr["TagPageCount"].ToString());
                     results.PageMainTagID = Convert.ToInt32(dbRdr["MainTAGID"].ToString());
                     results.PageSubTagID = Convert.ToInt32(dbRdr["SubTAGID"].ToString());
                 }
                 else
                 {
                     results.ActionStatus = "SUCCESS";
                     results.PageMainTagID = 0;
                     results.PageSubTagID = 0;
                 }
                 if (dbRdr != null)
                 {
                     dbRdr.Dispose();
                 }
                 if (dbCmd != null)
                 {
                     dbCmd.Dispose();
                 }
                 if (results.ActionStatus == "SUCCESS")
                 {
                     dbCon = OpenConnection();
                     dbCmd = new SqlCommand();
                     dbCmd.Connection = dbCon;
                     dbCmd.CommandType = CommandType.StoredProcedure;
                     dbCmd.CommandText = "USP_GETMAINTAGWITHPAGENO";
                     dbCmd.Parameters.Add("@in_iDocumentId", SqlDbType.Int).Value = filter.UploadDocID;
                     dbCmd.Parameters.Add("@in_iCurrentPageNo", SqlDbType.Int).Value = filter.PageId;
                     dbCmd.Parameters.Add("@in_iDepartmentId", SqlDbType.Int).Value = filter.DepartmentID;
                     dbRdr = dbCmd.ExecuteReader();
                     //results.ActionStatus = "FAILED";
                     while (dbRdr.Read())
                     {
                         results.ActionStatus = "SUCCESS";
                         if (results.MainTag == null)
                         {
                             results.MainTag = new List<MainTag>();
                         }
                         MainTag Main = new MainTag();
                         Main.MainTagID = Convert.ToInt32(dbRdr["TagId"].ToString());
                         Main.MainTagValue = dbRdr["TagValue"].ToString();
                         results.MainTag.Add(Main);
                     }
                 }
                 if (dbRdr != null)
                 {
                     dbRdr.Dispose();
                 }
                 if (dbCmd != null)
                 {
                     dbCmd.Dispose();
                 }
                 if (results.ActionStatus == "SUCCESS")
                 {
                     dbCon = OpenConnection();
                     dbCmd = new SqlCommand();
                     dbCmd.Connection = dbCon;
                     dbCmd.CommandType = CommandType.StoredProcedure;
                     dbCmd.CommandText = "USP_GETSUBTAGWITHPAGENO";
                     dbCmd.Parameters.Add("@in_iDocumentId", SqlDbType.Int).Value = filter.UploadDocID;
                     dbCmd.Parameters.Add("@in_iCurrentPageNo", SqlDbType.Int).Value = filter.PageId;
                     dbRdr = dbCmd.ExecuteReader();

                     while (dbRdr.Read())
                     {
                         results.ActionStatus = "SUCCESS";
                         if (results.SubTag == null)
                         {
                             results.SubTag = new List<SubTag>();
                         }
                         SubTag Sub = new SubTag();
                         Sub.SubTagID = Convert.ToInt32(dbRdr["TagId"].ToString());
                         Sub.SubTagValue = dbRdr["TagValue"].ToString();
                         results.SubTag.Add(Sub);
                     }
                 }
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

        public Results GetAvailablePagesForDocumentView(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            Results Results = new Results();
            DataSet ds = new DataSet();

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(8);
                dbManager.AddParameters(0, "@in_iDocumentId", filter.CurrentDocumentId);
                dbManager.AddParameters(1, "@in_vAction", action);
                dbManager.AddParameters(2, "@in_iMainTagId", filter.MainTagID);
                dbManager.AddParameters(3, "@in_iSubTagId", filter.SubTagID);
                dbManager.AddParameters(4, "@in_iTotalPages", filter.TotalPages);
                dbManager.AddParameters(5, "@in_vWhereClause", filter.WhereClause);
                dbManager.AddParameters(6, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(7, "@in_iLoginOrgId", loginOrgId);
                ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GetAvailablePagesForDocumetView");
                Results.ResultDS = ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }
            return Results;

            /* SqlCommand sqlCmd = new SqlCommand();
             SqlConnection dbCon = null;
             SqlDataAdapter sqlda = new SqlDataAdapter();
             DataSet dsValue = new DataSet();
             Results Results = new Results();
             try
             {

                 dbCon = OpenConnection();
                 sqlCmd.Connection = dbCon;
                 sqlCmd.CommandType = CommandType.StoredProcedure;
                 sqlCmd.CommandText = "USP_GetAvailablePagesForDocumetView";
                 sqlCmd.Parameters.Add("@in_iDocumentId", SqlDbType.Int).Value = filter.CurrentDocumentId;
                 sqlCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar).Value = action;

                 if (filter.MainTagID > 0)
                 {
                     sqlCmd.Parameters.Add("@in_iMainTagId", SqlDbType.Int).Value = filter.MainTagID;
                 }
                 if (filter.SubTagID > 0)
                 {
                     sqlCmd.Parameters.Add("@in_iSubTagId", SqlDbType.Int).Value = filter.SubTagID;
                 }
                 if (filter.TotalPages > 0)
                 {
                     sqlCmd.Parameters.Add("@in_iTotalPages", SqlDbType.Int).Value = filter.TotalPages;
                 }
                 if (filter.WhereClause != null)
                 {
                     sqlCmd.Parameters.Add("@in_vWhereClause", SqlDbType.VarChar).Value = filter.WhereClause;
                 }
                 sqlCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                 sqlCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;

                 sqlda.SelectCommand = sqlCmd;
                 sqlda.Fill(dsValue);
                 Results.ResultDS = dsValue;

             }
             catch (Exception)
             {


             }
             finally
             {
                 if (sqlCmd != null)
                 {
                     sqlCmd.Dispose();
                 }
                 CloseConnection(dbCon);
             }
             return Results;*/
        }
        public Results GetDocumetRemarksAndStatus(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {

            Results Results = new Results();
            DataSet ds = new DataSet();

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(5);
                dbManager.AddParameters(0, "@in_iDocumentId", filter.CurrentDocumentId);
                dbManager.AddParameters(1, "@in_vAction", action);
                dbManager.AddParameters(2, "@in_iGenStatusId", filter.GenStatusID);
                dbManager.AddParameters(3, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(4, "@in_iLoginOrgId", loginOrgId);

                ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_ManageDocumetStatusRemarks");
                Results.ResultDS = ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }
            return Results;

            /* SqlCommand sqlCmd = new SqlCommand();
             SqlConnection dbCon = null;
             SqlDataAdapter sqlda = new SqlDataAdapter();
             DataSet dsValue = new DataSet();
             Results Results = new Results();
             try
             {

                 dbCon = OpenConnection();
                 sqlCmd.Connection = dbCon;
                 sqlCmd.CommandType = CommandType.StoredProcedure;
                 sqlCmd.CommandText = "USP_ManageDocumetStatusRemarks";
                 sqlCmd.Parameters.Add("@in_iDocumentId", SqlDbType.Int).Value = filter.CurrentDocumentId;
                 sqlCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar).Value = action;
                 if (filter.GenStatusID > 0)
                 {
                     sqlCmd.Parameters.Add("@in_iGenStatusId", SqlDbType.Int).Value = filter.GenStatusID;
                 }
                 sqlCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                 sqlCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;

                 sqlda.SelectCommand = sqlCmd;
                 sqlda.Fill(dsValue);
                 Results.ResultDS = dsValue;

             }
             catch (Exception)
             {


             }
             finally
             {
                 if (sqlCmd != null)
                 {
                     sqlCmd.Dispose();
                 }
                 CloseConnection(dbCon);
             }
             return Results;*/
        }

        //Upload document details to Upload Table
        public Results ManageUploadedDocuments(string XMLUploadData, string xmlPageNoMappings, string xmlTagPageNoMaping, string LoginOrgId, string LoginToken, string sAction, int iProcessID, bool bSeperatedSplitting)
        {

            Results result = new Results();


            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(12);
                //bind four slashes in physical path for saving the document path in mysql
                if (Convert.ToString(ConfiguredDataProvider)=="MySql")
                {
                    XMLUploadData = XMLUploadData.Replace("\\", "\\\\");
                }               
                XMLUploadData = XMLUploadData.Replace("&apos;", "'");
                XMLUploadData = XMLUploadData.Replace("&", "&amp;");
                dbManager.AddParameters(0, "@XMLUploadData", XMLUploadData);
                dbManager.AddParameters(1, "@in_xPageNoMappings", xmlPageNoMappings);
                dbManager.AddParameters(2, "@in_xTagPageNoMappings", xmlTagPageNoMaping);
                dbManager.AddParameters(3, "@in_iLoginOrgId", Convert.ToInt32(LoginOrgId));
                dbManager.AddParameters(4, "@in_bSeperatedSplitting", bSeperatedSplitting);
                dbManager.AddParameters(5, "@in_vLoginToken", LoginToken);
                dbManager.AddParameters(6, "@in_vAction", sAction);
                dbManager.AddParameters(7, "@in_iProcessID", iProcessID);
                dbManager.AddParameters(8, "@out_iErrorState", 0, ParameterDirection.Output);
                dbManager.AddParameters(9, "@out_iErrorSeverity", 0, ParameterDirection.Output);
                dbManager.AddParameters(10, "@out_vMessage", string.Empty, DbType.String, 250, ParameterDirection.Output);
                dbManager.AddParameters(11, "@output", 0, ParameterDirection.Output);

                dbManager.ExecuteNonQuery(CommandType.StoredProcedure, "SP_UploadDocDetails");

                string returnvalue = dbManager.GetOutputParameterValue("@output").ToString().Trim() == "" ? "0" : dbManager.GetOutputParameterValue("@output").ToString().Trim();
                string errState = dbManager.GetOutputParameterValue("@out_iErrorState").ToString().Trim() == "" ? "0" : dbManager.GetOutputParameterValue("@out_iErrorState").ToString().Trim();
                string errSev = dbManager.GetOutputParameterValue("@out_iErrorSeverity").ToString().Trim() == "" ? "0" : dbManager.GetOutputParameterValue("@out_iErrorSeverity").ToString().Trim();
                result.ErrorState = Convert.ToInt32(errState);
                result.ErrorSeverity = Convert.ToInt32(errSev);
                result.Message = dbManager.GetOutputParameterValue("@out_vMessage").ToString().Trim();
                result.returncode = Convert.ToInt32(returnvalue);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }
            return result;

            /*SqlCommand uploadcmd = new SqlCommand();
            SqlConnection dbCon = null;
            Results result = new Results();
            try
            {
                dbCon = OpenConnection();
                uploadcmd = new SqlCommand();
                uploadcmd.Connection = dbCon;
                uploadcmd.CommandType = CommandType.StoredProcedure;
                uploadcmd.CommandText = "SP_UploadDocDetails";

                uploadcmd.CommandType = CommandType.StoredProcedure;
                uploadcmd.Parameters.AddWithValue("@XMLUploadData", XMLUploadData);
                if (xmlPageNoMappings != string.Empty && xmlPageNoMappings != null)
                {
                    uploadcmd.Parameters.AddWithValue("@in_xPageNoMappings", xmlPageNoMappings);
                }
                if (xmlTagPageNoMaping != string.Empty && xmlTagPageNoMaping != null)
                {
                    uploadcmd.Parameters.AddWithValue("@in_xTagPageNoMappings", xmlTagPageNoMaping);
                }
                uploadcmd.Parameters.AddWithValue("@in_iLoginOrgId", Convert.ToInt32(LoginOrgId));
                uploadcmd.Parameters.AddWithValue("@in_vLoginToken", LoginToken);
                uploadcmd.Parameters.AddWithValue("@in_vAction", sAction);
                uploadcmd.Parameters.AddWithValue("@in_iProcessID", iProcessID);

                // Output parameters
                uploadcmd.Parameters.Add(new SqlParameter("@out_iErrorState", SqlDbType.Int)).Direction = ParameterDirection.Output;
                uploadcmd.Parameters.Add(new SqlParameter("@out_iErrorSeverity", SqlDbType.Int)).Direction = ParameterDirection.Output;
                uploadcmd.Parameters.Add(new SqlParameter("@out_vMessage", SqlDbType.NVarChar, 8000)).Direction = ParameterDirection.Output;
                uploadcmd.Parameters.Add(new SqlParameter("@output", SqlDbType.Int)).Direction = ParameterDirection.Output;

                uploadcmd.ExecuteNonQuery();
                int output = (uploadcmd.Parameters["@output"].Value) == DBNull.Value ? 0 : Convert.ToInt32(uploadcmd.Parameters["@output"].Value);
                result.returncode = output;
                result.ErrorState = (int)uploadcmd.Parameters["@out_iErrorState"].Value;
                result.ErrorSeverity = (int)uploadcmd.Parameters["@out_iErrorSeverity"].Value;
                result.Message = uploadcmd.Parameters["@out_vMessage"].Value.ToString();

            }
            catch (Exception)
            {

            }
            finally
            {
                if (uploadcmd != null)
                {
                    uploadcmd.Dispose();
                }
                CloseConnection(dbCon);
            }
            return result;*/
        }

        public Results ManageDigitalSignature(string Action, string CertificatePath, string @in_xSignatureDetails, string LoginOrgId, string LoginToken)
        {
            DataSet dsValue = new DataSet();
            Results Results = new Results();


            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(5);
                dbManager.AddParameters(0, "@in_vAction", Action);
                dbManager.AddParameters(1, "@in_xSignatureDetails", @in_xSignatureDetails);
                dbManager.AddParameters(2, "@in_vCertificatePath", CertificatePath);
                dbManager.AddParameters(3, "@in_vLoginToken", LoginToken);
                dbManager.AddParameters(4, "@in_iLoginOrgId", LoginOrgId);

                dsValue = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_ManageDigitalSignature");
                Results.ResultDS = dsValue;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }
            return Results;

            /*SqlCommand sqlCmd = new SqlCommand();
            SqlConnection dbCon = null;
            SqlDataAdapter sqlda = new SqlDataAdapter();
            DataSet dsValue = new DataSet();
            Results Results = new Results();
            try
            {

                dbCon = OpenConnection();
                sqlCmd.Connection = dbCon;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "USP_ManageDigitalSignature";
                sqlCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar).Value = Action;
                sqlCmd.Parameters.Add("@in_xSignatureDetails", SqlDbType.VarChar).Value = @in_xSignatureDetails;
                sqlCmd.Parameters.Add("@in_vCertificatePath", SqlDbType.VarChar).Value = CertificatePath;

                sqlCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = LoginToken;
                sqlCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = LoginOrgId;

                sqlda.SelectCommand = sqlCmd;
                sqlda.Fill(dsValue);
                Results.ResultDS = dsValue;

            }
            catch (Exception)
            {


            }
            finally
            {
                if (sqlCmd != null)
                {
                    sqlCmd.Dispose();
                }
                CloseConnection(dbCon);
            }
            return Results;*/
        }

        public Results GetSignatureDetails(string action, int DocumentId, string LoginOrgId, string LoginToken)
        {
            DataSet dsValue = new DataSet();
            Results Results = new Results();


            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(4);
                dbManager.AddParameters(0, "@in_vAction", action);
                dbManager.AddParameters(1, "@in_iDocumentId", DocumentId);
                dbManager.AddParameters(2, "@in_vLoginToken", LoginToken);
                dbManager.AddParameters(3, "@in_iLoginOrgId", LoginOrgId);

                dsValue = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GetSignatureDetails");
                Results.ResultDS = dsValue;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }
            return Results;


            /*  SqlCommand sqlCmd = new SqlCommand();
              SqlConnection dbCon = null;
              SqlDataAdapter sqlda = new SqlDataAdapter();
              DataSet dsValue = new DataSet();
              Results Results = new Results();
              try
              {

                  dbCon = OpenConnection();
                  sqlCmd.Connection = dbCon;
                  sqlCmd.CommandType = CommandType.StoredProcedure;
                  sqlCmd.CommandText = "USP_GetSignatureDetails";
                  sqlCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar).Value = action;
                  sqlCmd.Parameters.Add("@in_iDocumentId", SqlDbType.Int).Value = DocumentId;

                  sqlCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = LoginToken;
                  sqlCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = LoginOrgId;

                  sqlda.SelectCommand = sqlCmd;
                  sqlda.Fill(dsValue);
                  Results.ResultDS = dsValue;

              }
              catch (Exception)
              {


              }
              finally
              {
                  if (sqlCmd != null)
                  {
                      sqlCmd.Dispose();
                  }
                  CloseConnection(dbCon);
              }
              return Results;*/
        }
        /*DMS5-4370	BS*/
        public Results GetDocumetHistoryDetails(string action, int DocumentId, string LoginOrgId, string LoginToken)
        {
            DataSet dsValue = new DataSet();
            Results Results = new Results();


            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(3);
              
                dbManager.AddParameters(0, "@in_iDocumentId", DocumentId);
                dbManager.AddParameters(1, "@in_vLoginToken", LoginToken);
                dbManager.AddParameters(2, "@in_iLoginOrgId", LoginOrgId);

                dsValue = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GetDocumentHistoryDetails");
                Results.ResultDS = dsValue;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }
            return Results;
        }
        /*DMS5-4370	BE*/
    }
}
