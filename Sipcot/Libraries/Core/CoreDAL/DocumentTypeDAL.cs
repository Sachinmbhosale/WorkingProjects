using System;
using System.Collections.Generic;
using System.Data;
using Lotex.EnterpriseSolutions.CoreBE;
using DataAccessLayer;

namespace Lotex.EnterpriseSolutions.CoreDAL
{
    public class DocumentTypeDAL : BaseDAL
    {
        public DocumentTypeDAL() { }
        public Results SearchDocumentTypes(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            Results results = new Results();
            results.ActionStatus = "SUCCESS";

            DateTime ? FromDate = null;
            FromDate = filter.FromDate.Length > 1 ? FormatScriptDateToSystemDate(filter.FromDate) : FromDate;

            DateTime ? Todate = null;
            Todate = filter.ToDate.Length > 0 ? FormatScriptDateToSystemDate(filter.ToDate) : Todate;

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(10);
                dbManager.AddParameters(0, "@in_iCurrDocumnetTypeId", filter.CurrDocumentTypeId);
                dbManager.AddParameters(1, "@DepartmentID", filter.DepartmentID);
                dbManager.AddParameters(2, "@CurrTemplateId", filter.CurrTemplateId);
                dbManager.AddParameters(3, "@in_iCurrOrgId", filter.CurrOrgId);
                dbManager.AddParameters(4, "@in_vCreaedDateFrom", FromDate);
                dbManager.AddParameters(5, "@in_vCreaedDateTo", Todate);
                dbManager.AddParameters(6, "@in_vAction", action);
                dbManager.AddParameters(7, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(8, "@in_iLoginOrgId", loginOrgId);
                dbManager.AddParameters(9, "@in_vDocumentTypeName", filter.DocumentTypeName);

                using (IDataReader dbRdr = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_SearchDocumentTypeId"))
                {

                    if (action == "ExportDocumentType")
                    {
                        while (dbRdr.Read())
                        {
                            results.ActionStatus = dbRdr["ActionStatus"].ToString();
                            if (results.ActionStatus == ActionStatus.SUCCESS.ToString())
                            {
                                if (results.IndexFields == null)
                                {
                                    results.IndexFields = new List<IndexField>();
                                }
                                IndexField docType = new IndexField();
                                docType.IndexName = dbRdr["FieldName"].ToString();

                                results.IndexFields.Add(docType);
                            }
                        }
                    }
                    else if (action == "SearchTypeForDownload")
                    {
                        while (dbRdr.Read())
                        {
                            results.ActionStatus = dbRdr["ActionStatus"].ToString();
                            if (results.ActionStatus == ActionStatus.SUCCESS.ToString())
                            {
                                if (results.DocumentTypes == null)
                                {
                                    results.DocumentTypes = new List<DocumentType>();
                                }
                                DocumentType docType = new DocumentType();
                                docType.DocumentTypeName = dbRdr["ID"].ToString();
                                docType.Description = dbRdr["NAME"].ToString();
                                results.DocumentTypes.Add(docType);
                            }
                        }
                    }
                    else
                    {
                        // ExportDocumentType
                        while (dbRdr.Read())
                        {
                            results.ActionStatus = dbRdr["ActionStatus"].ToString();
                            if (results.ActionStatus == ActionStatus.SUCCESS.ToString())
                            {
                                if (results.DocumentTypes == null)
                                {
                                    results.DocumentTypes = new List<DocumentType>();
                                }
                                DocumentType docType = new DocumentType();
                                docType.DocumentTypeId = Convert.ToInt32(dbRdr["DocumentTypeId"].ToString());
                                docType.DocumentTypeName = dbRdr["DocumentTypeName"].ToString();
                                docType.Description = dbRdr["Description"].ToString();
                                docType.Active = Convert.ToBoolean(dbRdr["Active"].ToString());
                                docType.GroupCount = Convert.ToInt32(dbRdr["GroupCount"].ToString());
                                docType.GroupIds = dbRdr["GroupIds"].ToString().TrimEnd(',');
                                docType.CanDelete = Convert.ToInt32(dbRdr["CanDelete"].ToString());
                                if (dbRdr["CreatedDate"].ToString() != string.Empty)
                                {
                                    docType.CreatedDate = FormatSystemDateToScriptDate(Convert.ToDateTime(dbRdr["CreatedDate"].ToString()));
                                }
                                docType.TemplateId = dbRdr["TemplateId"].ToString();
                                docType.DepartmentName = dbRdr["DepartmentName"].ToString();

                                docType.TemplateName = dbRdr["TemplateName"].ToString();
                                docType.DepartmentId = dbRdr["DepartmentId"].ToString();
                                docType.ArchivalD = dbRdr["ArchivalD"].ToString();
                                docType.WaterMarkT = dbRdr["WaterMarkT"].ToString();
                                docType.Makerchecker = Convert.ToBoolean(dbRdr["Makerchecker"].ToString());

                                results.DocumentTypes.Add(docType);
                            }
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

                 dbCmd.CommandText = "USP_SearchDocumentTypeId";
                 if (filter.CurrDocumentTypeId > 0)
                 {
                     dbCmd.Parameters.Add("@in_iCurrDocumnetTypeId", SqlDbType.Int).Value = filter.CurrDocumentTypeId;
                 }
                 if (filter.DepartmentID > 0)
                 {
                     dbCmd.Parameters.Add("@DepartmentID", SqlDbType.Int).Value = filter.DepartmentID;
                 }
                 if (filter.CurrTemplateId > 0)
                 {
                     dbCmd.Parameters.Add("@CurrTemplateId", SqlDbType.Int).Value = filter.CurrTemplateId;
                 }
                 if (filter.CurrOrgId > 0)
                 {
                     dbCmd.Parameters.Add("@in_iCurrOrgId", SqlDbType.Int).Value = filter.CurrOrgId;
                 }
                 if (filter.DocumentTypeName != string.Empty)
                 {
                     dbCmd.Parameters.Add("@in_vDocumentTypeName", SqlDbType.VarChar, 300).Value = filter.DocumentTypeName;
                 }
                 if (filter.FromDate != string.Empty)
                 {
                     dbCmd.Parameters.Add("@in_vCreaedDateFrom", SqlDbType.DateTime).Value = FormatScriptDateToSystemDate(filter.FromDate);
                 }
                 if (filter.ToDate != string.Empty)
                 {
                     dbCmd.Parameters.Add("@in_vCreaedDateTo", SqlDbType.DateTime).Value = FormatScriptDateToSystemDate(filter.ToDate);
                 }
                 dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar, 30).Value = action;
                 //sesstion
                 dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                 dbCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;

                 dbRdr = dbCmd.ExecuteReader();

                 if (action == "ExportDocumentType")
                 {
                     while (dbRdr.Read())
                     {
                         results.ActionStatus = dbRdr["ActionStatus"].ToString();
                         if (results.ActionStatus == ActionStatus.SUCCESS.ToString())
                         {
                             if (results.IndexFields == null)
                             {
                                 results.IndexFields = new List<IndexField>();
                             }
                             IndexField docType = new IndexField();
                             docType.IndexName = dbRdr["FieldName"].ToString();

                             results.IndexFields.Add(docType);
                         }
                     }
                 }
                 else if (action == "SearchTypeForDownload")
                 {
                     while (dbRdr.Read())
                     {
                         results.ActionStatus = dbRdr["ActionStatus"].ToString();
                         if (results.ActionStatus == ActionStatus.SUCCESS.ToString())
                         {
                             if (results.DocumentTypes == null)
                             {
                                 results.DocumentTypes = new List<DocumentType>();
                             }
                             DocumentType docType = new DocumentType();
                             docType.DocumentTypeName = dbRdr["ID"].ToString();
                             docType.Description = dbRdr["NAME"].ToString();
                             results.DocumentTypes.Add(docType);
                         }
                     }
                 }
                 else
                 {
                     // ExportDocumentType
                     while (dbRdr.Read())
                     {
                         results.ActionStatus = dbRdr["ActionStatus"].ToString();
                         if (results.ActionStatus == ActionStatus.SUCCESS.ToString())
                         {
                             if (results.DocumentTypes == null)
                             {
                                 results.DocumentTypes = new List<DocumentType>();
                             }
                             DocumentType docType = new DocumentType();
                             docType.DocumentTypeId = Convert.ToInt32(dbRdr["DocumentTypeId"].ToString());
                             docType.DocumentTypeName = dbRdr["DocumentTypeName"].ToString();
                             docType.Description = dbRdr["Description"].ToString();
                             docType.Active = Convert.ToBoolean(dbRdr["Active"].ToString());
                             docType.GroupCount = Convert.ToInt32(dbRdr["GroupCount"].ToString());
                             docType.GroupIds = dbRdr["GroupIds"].ToString().TrimEnd(',');
                             docType.CanDelete = Convert.ToInt32(dbRdr["CanDelete"].ToString());
                             if (dbRdr["CreatedDate"].ToString() != string.Empty)
                             {
                                 docType.CreatedDate = FormatSystemDateToScriptDate(Convert.ToDateTime(dbRdr["CreatedDate"].ToString()));
                             }
                             docType.TemplateId = dbRdr["TemplateId"].ToString();
                             docType.DepartmentName = dbRdr["DepartmentName"].ToString();

                             docType.TemplateName = dbRdr["TemplateName"].ToString();
                             docType.DepartmentId = dbRdr["DepartmentId"].ToString();
                             docType.ArchivalD = dbRdr["ArchivalD"].ToString();
                             docType.WaterMarkT = dbRdr["WaterMarkT"].ToString();
                             docType.Makerchecker = Convert.ToBoolean(dbRdr["Makerchecker"].ToString());

                             results.DocumentTypes.Add(docType);
                         }
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

        public Results DocTypeCheckBeforeRemove(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            Results results = new Results();
            results.ActionStatus = "SUCCESS";

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(5);
                dbManager.AddParameters(0, "@in_iCurrDocumnetTypeId", filter.DocumentTypeID);
                dbManager.AddParameters(1, "@DepartmentID", filter.DepartmentID);
                dbManager.AddParameters(2, "@in_vAction", action);
                dbManager.AddParameters(3, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(4, "@in_iLoginOrgId", loginOrgId);

                using (IDataReader dbRdr = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_SearchDocumentTypeId"))
                {
                    if (dbRdr.Read())
                    {
                        results.DocTypeCheckDeleteStatus = dbRdr["DocTypeActionStatus"].ToString();
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

            /*SqlDataReader dbRdr = null;
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

                dbCmd.CommandText = "USP_SearchDocumentTypeId";
                if (filter.DocumentTypeID > 0)
                {
                    dbCmd.Parameters.Add("@in_iCurrDocumnetTypeId", SqlDbType.Int).Value = filter.DocumentTypeID;
                }
                if (filter.DepartmentID > 0)
                {
                    dbCmd.Parameters.Add("@DepartmentID", SqlDbType.Int).Value = filter.DepartmentID;
                }

                dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar, 30).Value = action;
                //sesstion
                dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                dbCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;

                dbRdr = dbCmd.ExecuteReader();

                if (dbRdr.Read())
                {
                    results.DocTypeCheckDeleteStatus = dbRdr["DocTypeActionStatus"].ToString();
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

        public Results ManageDocumentType(DocumentType docType, string action, string loginOrgId, string loginToken)
        {
            Results results = new Results();
            results.ActionStatus = "SUCCESS";

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(10);
                dbManager.AddParameters(0, "@in_iCurrDocumentTypeId", docType.DocumentTypeId);
                dbManager.AddParameters(1, "@in_iOrgId", docType.OrgId);
                dbManager.AddParameters(2, "@in_vDocumentTypeName", docType.DocumentTypeName);
                dbManager.AddParameters(3, "@in_vDescription", docType.Description);
                dbManager.AddParameters(4, "@in_iGroupIds", docType.GroupIds);
                dbManager.AddParameters(5, "@in_bActive", docType.Active);
                dbManager.AddParameters(6, "@XMLDocType", docType.XMLDocType);
                dbManager.AddParameters(7, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(8, "@in_iLoginOrgId", loginOrgId);
                dbManager.AddParameters(9, "@in_vAction", action);


                using (IDataReader dbRdr = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_ManageDocumentType"))
                {
                    if (dbRdr.Read())
                    {
                        results.ActionStatus = dbRdr["ActionStatus"].ToString();
                        if (results.ActionStatus == ActionStatus.SUCCESS.ToString())
                        {
                            if (action == "AddDocumentType" || action == "EditDocumentType")
                            {
                                if (action == "AddDocumentType")
                                {
                                    docType.DocumentTypeId = Convert.ToInt32(dbRdr["IdentityId"].ToString());
                                }

                                dbManager.Open();
                                dbManager.CreateParameters(5);
                                dbManager.AddParameters(0, "@in_iCurrDocumentTypeId", docType.DocumentTypeId);
                                dbManager.AddParameters(1, "@in_iGroupId", docType.OrgId);
                                dbManager.AddParameters(2, "@in_vLoginToken", loginToken);
                                dbManager.AddParameters(3, "@in_iLoginOrgId", loginOrgId);
                                dbManager.AddParameters(4, "@in_vAction", "ExpaireMappedGroups");

                                using (IDataReader dbRdr1 = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_InsertGroupsforDocType"))
                                {
                                    if (dbRdr1.Read())
                                    {
                                        string[] groupIds = docType.GroupIds.Split(',');
                                        foreach (string id in groupIds)
                                        {
                                            results.ActionStatus = dbRdr["ActionStatus"].ToString();

                                            dbManager.Open();
                                            dbManager.CreateParameters(5);
                                            dbManager.AddParameters(0, "@in_iCurrDocumentTypeId", docType.DocumentTypeId);
                                            dbManager.AddParameters(1, "@in_iGroupId", id);
                                            dbManager.AddParameters(2, "@in_vLoginToken", loginToken);
                                            dbManager.AddParameters(3, "@in_iLoginOrgId", loginOrgId);
                                            dbManager.AddParameters(4, "@in_vAction", "AddMappedGroups");
                                            using (IDataReader dbRdr2 = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_InsertGroupsforDocType"))
                                            {
                                                if (dbRdr2.Read())
                                                {
                                                    results.ActionStatus = dbRdr2["ActionStatus"].ToString();

                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            Exception ex = new Exception();

                            throw ex;
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
            return results;



            /*SqlDataReader dbRdr = null;
            SqlCommand dbCmd = null;
            SqlConnection dbCon = null;
            Results results = new Results();
            SqlTransaction myTrans = null;

            try
            {
                dbCon = OpenConnection();
                dbCmd = new SqlCommand();
                dbCmd.Connection = dbCon;
                myTrans = dbCon.BeginTransaction();
                dbCmd.CommandType = CommandType.StoredProcedure;
                dbCmd.Transaction = myTrans;

                dbCmd.CommandText = "USP_ManageDocumentType";
                dbCmd.Parameters.Add("@in_iCurrDocumentTypeId", SqlDbType.Int).Value = docType.DocumentTypeId;

                //used to create user for specific organisation - in a autocreation scenario
                dbCmd.Parameters.Add("@in_iOrgId", SqlDbType.Int).Value = docType.OrgId;

                dbCmd.Parameters.Add("@in_vDocumentTypeName", SqlDbType.VarChar, 100).Value = docType.DocumentTypeName;
                dbCmd.Parameters.Add("@in_vDescription", SqlDbType.VarChar, 1000).Value = docType.Description;
                dbCmd.Parameters.Add("@in_iGroupIds", SqlDbType.VarChar, 500).Value = docType.GroupIds;

                dbCmd.Parameters.Add("@in_bActive", SqlDbType.Bit).Value = docType.Active;
                //dbCmd.Parameters.Add("@in_iTemplateId", SqlDbType.Int).Value = docType.TemplateId;Templatedepid
                dbCmd.Parameters.Add("@XMLDocType", SqlDbType.Xml, 8000).Value = docType.XMLDocType;
                dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                dbCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;
                dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar, 30).Value = action;

                dbRdr = dbCmd.ExecuteReader();

                if (dbRdr.Read())
                {
                    results.ActionStatus = dbRdr["ActionStatus"].ToString();
                    if (results.ActionStatus == ActionStatus.SUCCESS.ToString())
                    {
                        if (action == "AddDocumentType" || action == "EditDocumentType")
                        {
                            if (action == "AddDocumentType")
                            {
                                docType.DocumentTypeId = Convert.ToInt32(dbRdr["IdentityId"].ToString());
                            }

                            dbRdr.Close();
                            dbCmd = new SqlCommand();
                            dbCmd.Connection = dbCon;
                            dbCmd.CommandType = CommandType.StoredProcedure;
                            dbCmd.Transaction = myTrans;

                            dbCmd.CommandText = "USP_InsertGroupsforDocType";
                            dbCmd.Parameters.Add("@in_iCurrDocumentTypeId", SqlDbType.Int).Value = docType.DocumentTypeId;

                            //used to create user for specific organisation - in a autocreation scenario
                            dbCmd.Parameters.Add("@in_iGroupId", SqlDbType.Int).Value = docType.OrgId;

                            dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                            dbCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;
                            dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar, 30).Value = "ExpaireMappedGroups";

                            dbRdr = dbCmd.ExecuteReader();

                            if (dbRdr.Read())
                            {
                                string[] groupIds = docType.GroupIds.Split(',');
                                foreach (string id in groupIds)
                                {
                                    results.ActionStatus = dbRdr["ActionStatus"].ToString();
                                    dbRdr.Close();
                                    dbCmd = new SqlCommand();
                                    dbCmd.Connection = dbCon;
                                    dbCmd.CommandType = CommandType.StoredProcedure;
                                    dbCmd.Transaction = myTrans;

                                    dbCmd.CommandText = "USP_InsertGroupsforDocType";
                                    dbCmd.Parameters.Add("@in_iCurrDocumentTypeId", SqlDbType.Int).Value = docType.DocumentTypeId;

                                    //used to create user for specific organisation - in a autocreation scenario
                                    dbCmd.Parameters.Add("@in_iGroupId", SqlDbType.Int).Value = id;

                                    dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                                    dbCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;
                                    dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar, 30).Value = "AddMappedGroups";

                                    dbRdr = dbCmd.ExecuteReader();

                                    if (dbRdr.Read())
                                    {
                                        results.ActionStatus = dbRdr["ActionStatus"].ToString();

                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Exception ex = new Exception();
                      
                        throw ex;
                    }
                }
                dbRdr.Close();
                myTrans.Commit();
            }
            catch (Exception ex)
            {
                results.Message = ex.ToString();
                if (results.ActionStatus != string.Empty)
                {
                    results.ActionStatus = ActionStatus.ERROR.ToString();
                }
                myTrans.Rollback();
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
        public Results GetDocumnetTypeForAOrg(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            Results results = new Results();
            results.ActionStatus = "SUCCESS";


            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(4);
                dbManager.AddParameters(0, "@in_iCurrOrgID", filter.CurrOrgId);
                dbManager.AddParameters(1, "@in_vAction", action);
                dbManager.AddParameters(2, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(3, "@in_iLoginOrgId", loginOrgId);

                using (IDataReader dbRdr = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_GetDocumnetTypeForAOrg"))
                {
                    while (dbRdr.Read())
                    {
                        results.ActionStatus = dbRdr["ActionStatus"].ToString();
                        if (results.ActionStatus == ActionStatus.SUCCESS.ToString())
                        {
                            if (results.DocumentTypes == null)
                            {
                                results.DocumentTypes = new List<DocumentType>();
                            }
                            DocumentType docType = new DocumentType();
                            docType.DocumentTypeId = Convert.ToInt32(dbRdr["DocumentTypeId"].ToString());
                            docType.DocumentTypeName = dbRdr["DocumentTypeName"].ToString();
                            results.DocumentTypes.Add(docType);
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

            return results;
            /*SqlDataReader dbRdr = null;
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

                dbCmd.CommandText = "USP_GetDocumnetTypeForAOrg";
                dbCmd.Parameters.Add("@in_iCurrOrgID", SqlDbType.Int).Value = filter.CurrOrgId;
                dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar, 30).Value = action;
                //sesstion
                dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                dbCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;

                dbRdr = dbCmd.ExecuteReader();

                while (dbRdr.Read())
                {
                    results.ActionStatus = dbRdr["ActionStatus"].ToString();
                    if (results.ActionStatus == ActionStatus.SUCCESS.ToString())
                    {
                        if (results.DocumentTypes == null)
                        {
                            results.DocumentTypes = new List<DocumentType>();
                        }
                        DocumentType docType = new DocumentType();
                        docType.DocumentTypeId = Convert.ToInt32(dbRdr["DocumentTypeId"].ToString());
                        docType.DocumentTypeName = dbRdr["DocumentTypeName"].ToString();
                        results.DocumentTypes.Add(docType);
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
    }
}
