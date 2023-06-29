using System;
using System.Collections.Generic;
using System.Data;
using Lotex.EnterpriseSolutions.CoreBE;
using DataAccessLayer;

namespace Lotex.EnterpriseSolutions.CoreDAL
{
    public class TemplateDAL : BaseDAL
    {
        public TemplateDAL() { }

        #region Templates

        //Datatble for a particular template id
        public DataSet GetTemplateDetails(SearchFilter Entityobj)
        {
            DataSet temdet = new DataSet();
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(4);

                dbManager.AddParameters(0, "@Userid", Convert.ToInt32(Entityobj.CurrUserId));
                dbManager.AddParameters(1, "@Orgid", Convert.ToInt32(Entityobj.CurrOrgId));
                dbManager.AddParameters(2, "@docutype", Entityobj.DocumentTypeName);
                dbManager.AddParameters(3, "@DepartmentId", Entityobj.DepartmentID);

                temdet = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "Template_Details_Proc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }
            return temdet;
        }

        public Results SearchTemplates(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            Results results = new Results();
            results.ActionStatus = "SUCCESS";

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(7);
               
                DateTime? FromDate = null;
                FromDate = filter.FromDate.Length > 0 ? FormatScriptDateToSystemDate(filter.FromDate) : FromDate;
                DateTime? ToDate = null;
                ToDate = filter.ToDate.Length > 0 ? FormatScriptDateToSystemDate(filter.ToDate) : ToDate;

                dbManager.AddParameters(0, "@in_iCurrTemplateId", filter.CurrTemplateId);
                dbManager.AddParameters(1, "@in_vTemplateName", filter.TemplateName);
                dbManager.AddParameters(2, "@in_vCreaedDateFrom",FromDate);
                dbManager.AddParameters(3, "@in_vCreaedDateTo", ToDate);
                dbManager.AddParameters(4, "@in_vAction", action);
                dbManager.AddParameters(5, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(6, "@in_iLoginOrgId", loginOrgId);

                using (IDataReader dbRdr = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_SearchTemplatesById"))
                {
                    while (dbRdr.Read())
                    {
                        results.ActionStatus = dbRdr["ActionStatus"].ToString();
                        if (results.ActionStatus == "SUCCESS")
                        {
                            if (results.Templates == null)
                            {
                                results.Templates = new List<Template>();
                            }
                            Template tpl = new Template();
                            tpl.TemplateId = Convert.ToInt32(dbRdr["TemplateId"].ToString());
                            tpl.TemplateName = dbRdr["TemplateName"].ToString();
                            tpl.Active = Convert.ToBoolean(dbRdr["Active"].ToString());
                            tpl.FieldCount = Convert.ToInt32(dbRdr["FieldCount"].ToString());
                            tpl.CanDelete = Convert.ToInt32(dbRdr["CanDelete"].ToString());
                            tpl.CreatedDate = FormatSystemDateToScriptDate(Convert.ToDateTime(dbRdr["CreatedDate"].ToString()));
                            tpl.UploadFIleName = dbRdr["UploadFileName"].ToString();
                            tpl.UploadFIleNameSeperator = dbRdr["UploadFIleNameSeperator"].ToString();
                            results.Templates.Add(tpl);
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

                dbCmd.CommandText = "USP_SearchTemplatesById";
                if (filter.CurrTemplateId > 0)
                {
                    dbCmd.Parameters.Add("@in_iCurrTemplateId", SqlDbType.Int).Value = filter.CurrTemplateId;
                }
                if (filter.TemplateName != string.Empty)
                {
                    dbCmd.Parameters.Add("@in_vTemplateName", SqlDbType.VarChar, 300).Value = filter.TemplateName;
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

                while (dbRdr.Read())
                {
                    results.ActionStatus = dbRdr["ActionStatus"].ToString();
                    if (results.ActionStatus == "SUCCESS")
                    {
                        if (results.Templates == null)
                        {
                            results.Templates = new List<Template>();
                        }
                        Template tpl = new Template();
                        tpl.TemplateId = Convert.ToInt32(dbRdr["TemplateId"].ToString());
                        tpl.TemplateName = dbRdr["TemplateName"].ToString();
                        tpl.Active = Convert.ToBoolean(dbRdr["Active"].ToString());
                        tpl.FieldCount = Convert.ToInt32(dbRdr["FieldCount"].ToString());
                        tpl.CanDelete = Convert.ToInt32(dbRdr["CanDelete"].ToString());
                        tpl.CreatedDate = FormatSystemDateToScriptDate(Convert.ToDateTime(dbRdr["CreatedDate"].ToString()));
                        tpl.UploadFIleName = dbRdr["UploadFileName"].ToString();
                        tpl.UploadFIleNameSeperator = dbRdr["UploadFIleNameSeperator"].ToString();
                        results.Templates.Add(tpl);
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
        public Results ManageTemplates(Template tpl, string action, string loginOrgId, string loginToken)
        {
            Results results = new Results();

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(11);
                dbManager.AddParameters(0, "@in_iCurrTemplateId", tpl.TemplateId);
                dbManager.AddParameters(1, "@in_vTemplateName", tpl.TemplateName);
                dbManager.AddParameters(2, "@in_vFileName", tpl.UploadFIleName);
                dbManager.AddParameters(3, "@in_vSeperator", tpl.UploadFIleNameSeperator);
                dbManager.AddParameters(4, "@in_bActive", tpl.Active);
                dbManager.AddParameters(5, "@in_xIndexfieldDetails", tpl.IndexFieldDetails);
                dbManager.AddParameters(6, "@in_xIndexListItem", tpl.IndexListDetails);
                dbManager.AddParameters(7, "@in_xTagItemDetails", tpl.TagListDetails);
                dbManager.AddParameters(8, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(9, "@in_iLoginOrgId", loginOrgId);
                dbManager.AddParameters(10, "@in_vAction", action);

                using (IDataReader dbRdr = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_ManageTemplate"))
                {

                    if (dbRdr.Read())
                    {
                        results.ActionStatus = dbRdr["ActionStatus"].ToString();
                        if (action == "AddTemplate" || action == "EditTemplate")
                        {
                            if (results.ActionStatus == "SUCCESS")
                            {
                                if (dbRdr != null)
                                {
                                    dbRdr.Close();

                                }
                                return results;
                            }

                            else
                            {

                                if (dbRdr != null)
                                {
                                    dbRdr.Close();

                                }

                                return results;
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
             //  SqlTransaction myTrans = null;
             int TemplateID = 0;
             int CategoryID = 0;
             int CurTemplateForIndex = 0;
             try
             {
                 dbCon = OpenConnection();
                 dbCmd = new SqlCommand();
                 dbCmd.Connection = dbCon;
                 //myTrans = dbCon.BeginTransaction();
                 dbCmd.CommandType = CommandType.StoredProcedure;

                 //    dbCmd.Transaction = myTrans;


                 dbCmd.CommandText = "USP_ManageTemplate";
                 dbCmd.Parameters.Add("@in_iCurrTemplateId", SqlDbType.Int).Value = tpl.TemplateId;

                 dbCmd.Parameters.Add("@in_vTemplateName", SqlDbType.VarChar, 100).Value = tpl.TemplateName;
                 dbCmd.Parameters.Add("@in_vFileName", SqlDbType.VarChar, 50).Value = tpl.UploadFIleName;
                 dbCmd.Parameters.Add("@in_vSeperator", SqlDbType.VarChar, 5).Value = tpl.UploadFIleNameSeperator;
                 dbCmd.Parameters.Add("@in_bActive", SqlDbType.Bit).Value = tpl.Active;
                 dbCmd.Parameters.Add("@in_xIndexfieldDetails", SqlDbType.Xml).Value = tpl.IndexFieldDetails;
                 dbCmd.Parameters.Add("@in_xIndexListItem", SqlDbType.Xml).Value = tpl.IndexListDetails;
                 dbCmd.Parameters.Add("@in_xTagItemDetails", SqlDbType.Xml).Value = tpl.TagListDetails;
                 //session
                 dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                 dbCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;
                 dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar, 30).Value = action;

                 dbRdr = dbCmd.ExecuteReader();



                 if (dbRdr.Read())
                 {
                     results.ActionStatus = dbRdr["ActionStatus"].ToString();
                     if (action == "AddTemplate" || action == "EditTemplate")
                     {
                         if (results.ActionStatus == "SUCCESS")
                         {
                             if (dbRdr != null)
                             {
                                 dbRdr.Close();
                                 dbRdr = null;
                             }
                             return results;
                         }

                         else
                         {

                             if (dbRdr != null)
                             {
                                 dbRdr.Close();
                                 dbRdr = null;
                             }
                             //  myTrans.Rollback();
                             return results;
                         }
                     }
                 }

             }
             catch (Exception ex)
             {
                 //   myTrans.Rollback();
                 results.Message = ex.ToString();
                 if (results.ActionStatus != string.Empty)
                 {
                     results.ActionStatus = "ERROR";
                 }
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
        public Results GetTemplateFieldList(Template tpl, string action, string loginOrgId, string loginToken)
        {

            Results results = new Results();
            results.ActionStatus = "SUCCESS";

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(4);

                dbManager.AddParameters(0, "@in_iCurrTemplateId", tpl.TemplateId);
                dbManager.AddParameters(1, "@in_vAction", "GetTemplateFieldList");
                dbManager.AddParameters(2, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(3, "@in_iLoginOrgId", loginOrgId);

                results.ResultDS = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GetTemplateFieldList");
                if (results.ResultDS.Tables.Count > 0)
                {

                    Template resTpl = new Template();
                    for (int counter = 0; counter < results.ResultDS.Tables[0].Rows.Count; counter++)
                    {
                        results.ActionStatus = results.ResultDS.Tables[0].Rows[0]["ActionStatus"].ToString();

                        if (results.Templates == null)
                        {
                            results.Templates = new List<Template>();
                        }
                        if (resTpl.IndexFields == null)
                        {
                            resTpl.IndexFields = new List<IndexField>();
                        }
                        IndexField fld = new IndexField();


                        //fld.Values = dbRdr["Values"].ToString();
                        //fld.SubValues = dbRdr["SubValues"].ToString();
                        fld.IndexFldId = Convert.ToInt32(results.ResultDS.Tables[0].Rows[counter]["IndexFldId"]);
                        fld.IndexName = results.ResultDS.Tables[0].Rows[counter]["IndexName"].ToString();
                        fld.EntryType = results.ResultDS.Tables[0].Rows[counter]["EntryType"].ToString();
                        fld.CharIndexDataType = results.ResultDS.Tables[0].Rows[counter]["CharDataType"].ToString();
                        fld.MinLength = Convert.ToInt32(results.ResultDS.Tables[0].Rows[counter]["MinLength"].ToString());
                        fld.MaxLength = Convert.ToInt32(results.ResultDS.Tables[0].Rows[counter]["MaxLength"].ToString());
                        fld.Mandatory = results.ResultDS.Tables[0].Rows[counter]["Mandatory"].ToString();
                        fld.Display = results.ResultDS.Tables[0].Rows[counter]["Display"].ToString();
                        fld.ActiveIndex = results.ResultDS.Tables[0].Rows[counter]["ActiveIndex"].ToString();
                        fld.SortOrder = Convert.ToInt32(results.ResultDS.Tables[0].Rows[counter]["SortOrder"]);
                        fld.OrginalOrder = Convert.ToInt32(results.ResultDS.Tables[0].Rows[counter]["OrginalOrder"]);
                        fld.haschild = results.ResultDS.Tables[0].Rows[counter]["HasChild"].ToString();
                        resTpl.IndexFields.Add(fld);
                    }
                    results.Templates.Add(resTpl);
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
            /*SqlDataAdapter da = new SqlDataAdapter();
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

                dbCmd.CommandText = "USP_GetTemplateFieldList";
                dbCmd.Parameters.Add("@in_iCurrTemplateId", SqlDbType.Int).Value = tpl.TemplateId;
                dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar, 30).Value = "GetTemplateFieldList";
                //session
                dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                dbCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;

                da.SelectCommand = dbCmd;
                da.Fill(results.ResultDS);

                if (results.ResultDS.Tables.Count > 0)
                {

                    Template resTpl = new Template();
                    for (int counter = 0; counter < results.ResultDS.Tables[0].Rows.Count; counter++)
                    {
                        results.ActionStatus = results.ResultDS.Tables[0].Rows[0]["ActionStatus"].ToString();

                        if (results.Templates == null)
                        {
                            results.Templates = new List<Template>();
                        }
                        if (resTpl.IndexFields == null)
                        {
                            resTpl.IndexFields = new List<IndexField>();
                        }
                        IndexField fld = new IndexField();
                
         
                        //fld.Values = dbRdr["Values"].ToString();
                        //fld.SubValues = dbRdr["SubValues"].ToString();
                        fld.IndexFldId = Convert.ToInt32(results.ResultDS.Tables[0].Rows[counter]["IndexFldId"]);
                        fld.IndexName = results.ResultDS.Tables[0].Rows[counter]["IndexName"].ToString();
                        fld.EntryType = results.ResultDS.Tables[0].Rows[counter]["EntryType"].ToString();
                        fld.CharIndexDataType = results.ResultDS.Tables[0].Rows[counter]["CharDataType"].ToString();
                        fld.MinLength = Convert.ToInt32(results.ResultDS.Tables[0].Rows[counter]["MinLength"].ToString());
                        fld.MaxLength = Convert.ToInt32(results.ResultDS.Tables[0].Rows[counter]["MaxLength"].ToString());
                        fld.Mandatory = results.ResultDS.Tables[0].Rows[counter]["Mandatory"].ToString();
                        fld.Display = results.ResultDS.Tables[0].Rows[counter]["Display"].ToString();
                        fld.ActiveIndex = results.ResultDS.Tables[0].Rows[counter]["ActiveIndex"].ToString();
                        fld.SortOrder = Convert.ToInt32(results.ResultDS.Tables[0].Rows[counter]["SortOrder"]);
                        fld.OrginalOrder = Convert.ToInt32(results.ResultDS.Tables[0].Rows[counter]["OrginalOrder"]);
                        fld.haschild = results.ResultDS.Tables[0].Rows[counter]["HasChild"].ToString();
                        resTpl.IndexFields.Add(fld);
                    }
                    results.Templates.Add(resTpl);
                }
                
            }
            catch (Exception ex)
            {
                results.Message = ex.ToString();
                throw ex;
            }
            finally
            {
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
            return results;*/
        }
        public Results GetAllTemplatesForOrg(int OrgId, string TempateName, string action, string loginOrgId, string loginToken)
        {
            Results results = new Results();

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(5);
                dbManager.AddParameters(0, "@in_vTempateName", TempateName);
                dbManager.AddParameters(1, "@in_iOrgId", OrgId);
                dbManager.AddParameters(2, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(3, "@in_iLoginOrgId", loginOrgId);
                dbManager.AddParameters(4, "@in_vAction", action);

                using (IDataReader dbRdr = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_GetAllTemplatesForOrgId"))
                {
                    while (dbRdr.Read())
                    {
                        if (results.Templates == null)
                        {
                            results.Templates = new List<Template>();
                        }
                        Template item = new Template();
                        item.TemplateId = Convert.ToInt32(dbRdr["TemplateId"].ToString());
                        item.TemplateName = dbRdr["TemplateName"].ToString();
                        item.Active = Convert.ToBoolean(dbRdr["Active"].ToString());
                        results.Templates.Add(item);
                        results.ActionStatus = dbRdr["ActionStatus"].ToString();
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
             try
             {
                 dbCon = OpenConnection();
                 dbCmd = new SqlCommand();
                 dbCmd.Connection = dbCon;
                 dbCmd.CommandType = CommandType.StoredProcedure;

                 dbCmd.CommandText = "USP_GetAllTemplatesForOrgId";
                 dbCmd.Parameters.Add("@in_vTempateName", SqlDbType.VarChar, 300).Value = TempateName;
                 dbCmd.Parameters.Add("@in_iOrgId", SqlDbType.Int).Value = OrgId;

                 dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                 dbCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;
                 dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar, 30).Value = action;

                 dbRdr = dbCmd.ExecuteReader();

                 while (dbRdr.Read())
                 {
                     if (results.Templates == null)
                     {
                         results.Templates = new List<Template>();
                     }
                     Template item = new Template();
                     item.TemplateId = Convert.ToInt32(dbRdr["TemplateId"].ToString());
                     item.TemplateName = dbRdr["TemplateName"].ToString();
                     item.Active = Convert.ToBoolean(dbRdr["Active"].ToString());
                     results.Templates.Add(item);
                     results.ActionStatus = dbRdr["ActionStatus"].ToString();
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
        #region GetTemplateFiledsByDocumenttypeandDepartment
        public Results GetTemplateDetails(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            Results results = new Results();
            results.ActionStatus = "SUCCESS";

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(6);

                dbManager.AddParameters(0, "@in_iCurrUserId", filter.CurrOrgId);
                dbManager.AddParameters(1, "@in_iDocuTypeID", filter.DocumentTypeID);
                dbManager.AddParameters(2, "@in_iDepartmentID", filter.DepartmentID);
                dbManager.AddParameters(3, "@in_vAction", action);
                dbManager.AddParameters(4, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(5, "@in_iLoginOrgId", loginOrgId);

                using (IDataReader dbRdr = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_GetTemplateDetails"))
                {
                    while (dbRdr.Read())
                    {
                        results.ActionStatus = dbRdr["ActionStatus"].ToString();
                        if (results.ActionStatus == "SUCCESS")
                        {
                            if (results.IndexFields == null)
                            {
                                results.IndexFields = new List<IndexField>();
                            }
                            IndexField indexfields = new IndexField();
                            indexfields.IndexName = dbRdr["LabelName"].ToString();
                            indexfields.DataType = dbRdr["DBType"].ToString();
                            indexfields.MinLength = Convert.ToInt32(dbRdr["Min"].ToString());
                            indexfields.MaxLength = Convert.ToInt32(dbRdr["Max"].ToString());
                            indexfields.EntryType = dbRdr["InputType"].ToString();
                            indexfields.Indexidentity = Convert.ToInt32(dbRdr["SlNo"].ToString());
                            indexfields.Mandatory = dbRdr["Mandatory"].ToString();//MD -Modified
                            indexfields.SortOrder = Convert.ToInt32(dbRdr["SortOrder"]);
                            //indexfields.ActiveIndex = dbRdr["IndexActive"].ToString();//MD -Modified
                            //indexfields.Values = dbRdr["Values"].ToString();
                            results.IndexFields.Add(indexfields);

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

                 dbCmd.CommandText = "USP_GetTemplateDetails";
                 dbCmd.Parameters.Add("@in_iCurrUserId", SqlDbType.Int).Value = filter.CurrOrgId;
                 dbCmd.Parameters.Add("@in_iDocuTypeID", SqlDbType.Int).Value = filter.DocumentTypeID;
                 dbCmd.Parameters.Add("@in_iDepartmentID", SqlDbType.Int).Value = filter.DepartmentID;
                 dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar, 30).Value = action;
                 //sesstion
                 dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                 dbCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;

                 dbRdr = dbCmd.ExecuteReader();

                 while (dbRdr.Read())
                 {
                     results.ActionStatus = dbRdr["ActionStatus"].ToString();
                     if (results.ActionStatus == "SUCCESS")
                     {
                         if (results.IndexFields == null)
                         {
                             results.IndexFields = new List<IndexField>();
                         }
                         IndexField indexfields = new IndexField();
                         indexfields.IndexName = dbRdr["LabelName"].ToString();
                         indexfields.DataType = dbRdr["DBType"].ToString();
                         indexfields.MinLength = Convert.ToInt32(dbRdr["Min"].ToString());
                         indexfields.MaxLength = Convert.ToInt32(dbRdr["Max"].ToString());
                         indexfields.EntryType = dbRdr["InputType"].ToString();
                         indexfields.Indexidentity = Convert.ToInt32(dbRdr["SlNo"].ToString());
                         indexfields.Mandatory = dbRdr["Mandatory"].ToString();//MD -Modified
                         indexfields.SortOrder = Convert.ToInt32(dbRdr["SortOrder"]);
                         indexfields.ActiveIndex = dbRdr["IndexActive"].ToString();//MD -Modified
                         //indexfields.Values = dbRdr["Values"].ToString();
                         results.IndexFields.Add(indexfields);

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
        #endregion
        #region GetTemplateMultiValues
        public Results GetTemplateMultiValues(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {

            Results results = new Results();
            results.ActionStatus = "SUCCESS";


            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(4);

                dbManager.AddParameters(0, "@in_iCurrTemplateId", filter.CurrTemplateId);
                dbManager.AddParameters(1, "@in_vAction", action);
                dbManager.AddParameters(2, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(3, "@in_iLoginOrgId", loginOrgId);

                using (IDataReader dbRdr = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_GetTemplateMultiValues"))
                {
                    while (dbRdr.Read())
                    {
                        results.ActionStatus = dbRdr["ActionStatus"].ToString();
                        if (results.ActionStatus == "SUCCESS")
                        {
                            if (results.IndexFields == null)
                            {
                                results.IndexFields = new List<IndexField>();
                            }
                            IndexField indexfields = new IndexField();
                            indexfields.Indexidentity = Convert.ToInt32(dbRdr["TempID"]);
                            indexfields.Values = dbRdr["Value"].ToString();
                            indexfields.EntrySubType = dbRdr["InputType"].ToString();
                            results.IndexFields.Add(indexfields);
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

                 dbCmd.CommandText = "USP_GetTemplateMultiValues";
                 dbCmd.Parameters.Add("@in_iCurrTemplateId", SqlDbType.Int).Value = filter.CurrTemplateId;
                 dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar, 30).Value = action;
                 //sesstion
                 dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                 dbCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;

                 dbRdr = dbCmd.ExecuteReader();

                 while (dbRdr.Read())
                 {
                     results.ActionStatus = dbRdr["ActionStatus"].ToString();
                     if (results.ActionStatus == "SUCCESS")
                     {
                         if (results.IndexFields == null)
                         {
                             results.IndexFields = new List<IndexField>();
                         }
                         IndexField indexfields = new IndexField();
                         indexfields.Indexidentity = Convert.ToInt32(dbRdr["TempID"]);
                         indexfields.Values = dbRdr["Value"].ToString();
                         indexfields.EntrySubType = dbRdr["InputType"].ToString();
                         results.IndexFields.Add(indexfields);
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
        #endregion
        #region Manage List Items
        public DataSet ManageListItems(IndexField filter, string action, string loginOrgId, string loginToken)
        {
            DataSet ds = new DataSet();

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(4);

                dbManager.AddParameters(0, "@in_iTemplateId", filter.TemplateID);
                dbManager.AddParameters(1, "@in_vAction", action);
                dbManager.AddParameters(2, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(3, "@in_iLoginOrgId", loginOrgId);
                ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_ManageListItems");
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

            /*DataSet ds = new DataSet();
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

                dbCmd.CommandText = "USP_ManageListItems";
                dbCmd.Parameters.Add("@in_iTemplateId", SqlDbType.Int).Value = filter.TemplateID;
                //Common
                dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar, 30).Value = action;
                //sesstion
                dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                dbCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;
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
        #region Get Tag Details
        public DataSet GeTaggedtSubTag(string Documentid, string Maintagid)
        {
            DataSet ds = new DataSet();

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(2);
                dbManager.AddParameters(0, "@in_iDocTypeID", Convert.ToInt32(Documentid));
                dbManager.AddParameters(1, "@in_iMaintagid", Convert.ToInt32(Maintagid)); 
                ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GetTaggedMaintagSubtag");
                


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
            /*DataSet ds = new DataSet();
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

                dbCmd.CommandText = "USP_GetTagDetailswithDocTypeID";
                dbCmd.Parameters.Add("@in_iDocTypeID", SqlDbType.Int).Value = filter.DocumentTypeID;
                if (filter.DepartmentID > 0)
                {
                    dbCmd.Parameters.Add("@DepId", SqlDbType.Int).Value = filter.DepartmentID;
                }
                dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar, 50).Value = action;
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

        public DataSet GeTaggedtMainTag(string Documentid)
        {
            DataSet ds = new DataSet();

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(1);
                dbManager.AddParameters(0, "@in_iDocTypeID", Convert.ToInt32(Documentid));
                ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GetTaggedMaintag");
                


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
        

        public DataSet GetTagDetails(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            DataSet ds = new DataSet();

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(3);

                dbManager.AddParameters(0, "@in_iDocTypeID", filter.DocumentTypeID);
                dbManager.AddParameters(1, "@DepId", filter.DepartmentID);
                dbManager.AddParameters(2, "@in_vAction", action);

                ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GetTagDetailswithDocTypeID");
               

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
            /*DataSet ds = new DataSet();
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

                dbCmd.CommandText = "USP_GetTagDetailswithDocTypeID";
                dbCmd.Parameters.Add("@in_iDocTypeID", SqlDbType.Int).Value = filter.DocumentTypeID;
                if (filter.DepartmentID > 0)
                {
                    dbCmd.Parameters.Add("@DepId", SqlDbType.Int).Value = filter.DepartmentID;
                }
                dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar, 50).Value = action;
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
        



    }
}
