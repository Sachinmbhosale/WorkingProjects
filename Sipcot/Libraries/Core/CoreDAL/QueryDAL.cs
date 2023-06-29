using System;
using System.Collections.Generic;
using System.Data;
using Lotex.EnterpriseSolutions.CoreBE;
using DataAccessLayer;

namespace Lotex.EnterpriseSolutions.CoreDAL
{
    public class QueryDAL : BaseDAL
    {

        /// <summary>
        /// Saves the query.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="projectId">The project id.</param>
        /// <param name="queryName">Name of the query.</param>
        /// <param name="queryClauses">The query clauses.</param>
        /// <returns></returns>
        public bool SaveQuery(int projectId, string queryName, bool isPublic, string queryType, List<QueryClause> queryClauses,int orgId,string Token)
        {
            if (queryName == null || queryName == String.Empty)
                throw (new ArgumentOutOfRangeException("queryName"));

            if (queryClauses.Count == 0)
                throw (new ArgumentOutOfRangeException("queryClauses"));

            int queryId = 0;

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(7);                
                dbManager.AddParameters(0, "@projectId", projectId);
                dbManager.AddParameters(1, "@QueryName", queryName);
                dbManager.AddParameters(2, "@IsPublic", isPublic);
                dbManager.AddParameters(3, "@Query_vType", queryType);
                dbManager.AddParameters(4, "@in_vLoginToken", Token);
                dbManager.AddParameters(5, "@in_iLoginOrgId", orgId);
                dbManager.AddParameters(6, "@ReturnValue", 0, ParameterDirection.ReturnValue);
                object value = dbManager.ExecuteScalar(CommandType.StoredProcedure, "USP_Query_SaveQuery");
                queryId = Convert.ToInt32(value);

                // Save Query Clauses
                foreach (QueryClause clause in queryClauses)
                {
                    dbManager.CreateParameters(10);

                    dbManager.AddParameters(0, "@QueryId", queryId);
                    dbManager.AddParameters(1, "@BooleanOperator", clause.BooleanOperator);
                    dbManager.AddParameters(2, "@FieldName", clause.FieldName);
                    dbManager.AddParameters(3, "@ComparisonOperator", clause.ComparisonOperator);
                    dbManager.AddParameters(4, "@FieldValue", clause.FieldValue);
                    dbManager.AddParameters(5, "@DataType", clause.DataType);
                    dbManager.AddParameters(6, "@IsCustomField", clause.CustomFieldQuery);
                    dbManager.AddParameters(7, "@ObjectType", clause.ObjectType);
                    dbManager.AddParameters(8, "@in_iLoginOrgId", orgId);
                    dbManager.AddParameters(9, "@in_vLoginToken", Token);
                    dbManager.ExecuteScalar(CommandType.StoredProcedure, "USP_Query_SaveQueryClause");
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

            /*
                // Create Save Query Command
                dbCon = OpenConnection();
                cmdSaveQuery = new SqlCommand();
                cmdSaveQuery.Connection = dbCon;
                cmdSaveQuery.CommandType = CommandType.StoredProcedure;
                cmdSaveQuery.CommandText = "USP_Query_SaveQuery";
                cmdSaveQuery.Parameters.Add("@Username", SqlDbType.NVarChar).Value = userName;
                cmdSaveQuery.Parameters.Add("@projectId", SqlDbType.Int).Value = projectId;
                cmdSaveQuery.Parameters.Add("@QueryName", SqlDbType.NVarChar).Value = queryName;
                cmdSaveQuery.Parameters.Add("@IsPublic", SqlDbType.Bit).Value = isPublic;
                cmdSaveQuery.Parameters.Add("@Query_vType", SqlDbType.VarChar).Value = queryType;
                cmdSaveQuery.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                // Create Save Query Clause Command

                cmdClause.CommandType = CommandType.StoredProcedure;
                cmdClause.Connection = dbCon;
                cmdClause.CommandText = "USP_Query_SaveQueryClause";
                cmdClause.Parameters.Add("@QueryId", SqlDbType.Int);
                cmdClause.Parameters.Add("@BooleanOperator", SqlDbType.NVarChar, 50);
                cmdClause.Parameters.Add("@FieldName", SqlDbType.NVarChar, 50);
                cmdClause.Parameters.Add("@ComparisonOperator", SqlDbType.NVarChar, 50);
                cmdClause.Parameters.Add("@FieldValue", SqlDbType.NVarChar, 50);
                cmdClause.Parameters.Add("@DataType", SqlDbType.Int);
                cmdClause.Parameters.Add("@IsCustomField", SqlDbType.Bit);
                cmdClause.Parameters.Add("@ObjectType", SqlDbType.NVarChar, 50);
                queryId = Convert.ToInt32(cmdSaveQuery.ExecuteScalar());

                //queryId = (int)cmdSaveQuery.Parameters["@ReturnValue"].Value;

                // Save Query Clauses
                foreach (QueryClause clause in queryClauses)
                {
                    cmdClause.Parameters["@QueryId"].Value = queryId;
                    cmdClause.Parameters["@BooleanOperator"].Value = clause.BooleanOperator;
                    cmdClause.Parameters["@FieldName"].Value = clause.FieldName;
                    cmdClause.Parameters["@ComparisonOperator"].Value = clause.ComparisonOperator;
                    cmdClause.Parameters["@FieldValue"].Value = clause.FieldValue;
                    cmdClause.Parameters["@DataType"].Value = clause.DataType;
                    cmdClause.Parameters["@IsCustomField"].Value = clause.CustomFieldQuery;
                    cmdClause.Parameters["@ObjectType"].Value = clause.ObjectType;
                    cmdClause.ExecuteScalar();

                }
            */

            if (queryId == 0)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Gets the name of the queries by user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="projectId">The project id.</param>
        /// <returns></returns>
        public DataSet GetQueriesByUserName(int projectId,int orgId,string Token)
        {

            DataSet dsValue = new DataSet();

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                // Validate Parameters
                if (projectId <= 0)
                    throw (new ArgumentOutOfRangeException("projectId"));

                dbManager.Open();
                dbManager.CreateParameters(3);
                dbManager.AddParameters(0, "@ProjectId", projectId);
                dbManager.AddParameters(1, "@in_iLoginOrgId", orgId);
                dbManager.AddParameters(2, "@in_vLoginToken", Token);
               

                dsValue = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_Query_GetQueriesByUserName");

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }

            return dsValue;

            /*SqlCommand sqlCmd = new SqlCommand();
            SqlConnection dbCon = null;
            SqlDataAdapter sqlda = new SqlDataAdapter();
            DataSet dsValue = new DataSet();
            try
            {
                // Validate Parameters
                if (projectId <= 0)
                    throw (new ArgumentOutOfRangeException("projectId"));

                // Execute SQL Command
                dbCon = OpenConnection();               
                sqlCmd.Connection = dbCon;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "USP_Query_GetQueriesByUserName";
                sqlCmd.Parameters.Add("@ProjectId", SqlDbType.Int).Value = projectId;
                sqlCmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = userName;
                sqlda.SelectCommand = sqlCmd;
                sqlda.Fill(dsValue);

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
            return dsValue;*/
        }
        /// <summary>
        /// Get saved query based on query value selection
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="queryId"></param>
        /// <returns></returns>
        public DataSet GetSavedQuery(int queryId,int orgId,string Token)
        {

            DataSet dsValue = new DataSet();

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                // Validate Parameters
                // Validate Parameters
                if (queryId <= 0)
                    throw (new ArgumentOutOfRangeException("queryId"));

                dbManager.Open();
                dbManager.CreateParameters(3);
                dbManager.AddParameters(0, "@QueryId", queryId);
                dbManager.AddParameters(1, "@in_iLoginOrgId", orgId);
                dbManager.AddParameters(2, "@in_vLoginToken", Token);
                dsValue = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_Query_GetSavedQuery");

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }

            return dsValue;

            /*SqlCommand sqlCmd = new SqlCommand();
            SqlConnection dbCon = null;
            SqlDataAdapter sqlda = new SqlDataAdapter();
            DataSet dsValue = new DataSet();
            try
            {
                // Validate Parameters
                if (queryId <= 0)
                    throw (new ArgumentOutOfRangeException("queryId"));
                
                dbCon = OpenConnection();
                sqlCmd.Connection = dbCon;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "USP_Query_GetSavedQuery";
                sqlCmd.Parameters.Add("@QueryId", SqlDbType.Int).Value = queryId;
                sqlda.SelectCommand = sqlCmd;
                sqlda.Fill(dsValue);
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
            return dsValue;*/
        }
        /// <summary>
        /// Get the query name based on query id
        /// </summary>
        /// <param name="queryId"></param>
        /// <returns></returns>
        public DataSet GetQueryById(int queryId,int orgId,string token)
        {

            DataSet dsQuery = new DataSet();
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                // Validate Parameters
                // Validate Parameters
                if (queryId <= 0)
                    throw (new ArgumentOutOfRangeException("queryId"));

                dbManager.Open();
                dbManager.CreateParameters(3);
                dbManager.AddParameters(0, "@QueryId", queryId);
                dbManager.AddParameters(1,"@in_vLoginToken",token);
                dbManager.AddParameters(2, "@in_iLoginOrgId", orgId);
                dsQuery = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_Query_GetQueryById");

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }

            return dsQuery;

            /*SqlCommand sqlCmd = new SqlCommand();
            SqlConnection sqlcon = new SqlConnection();
            SqlDataAdapter sqlda = new SqlDataAdapter();
            DataSet dsQuery=new DataSet();
            List<Query> queryList = new List<Query>();
            
            try
            {
                sqlcon = OpenConnection();
                sqlCmd.Connection = sqlcon;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "USP_Query_GetQueryById";
                sqlCmd.Parameters.Add("@QueryId", SqlDbType.Int).Value = queryId;
                sqlda.SelectCommand = sqlCmd;
                sqlda.Fill(dsQuery);               
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
                CloseConnection(sqlcon);
            }
            return dsQuery;*/
        }
        /// <summary>
        /// Delete the query
        /// </summary>
        /// <param name="QueryId"></param>
        /// <returns></returns>
        public bool DeleteQuery(int QueryId,int OrgId,string Token)
        {
            int returnValue = 1;
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(4);
                dbManager.AddParameters(0, "@QueryId", QueryId);
                dbManager.AddParameters(1, "@in_iLoginOrgId", OrgId);
                dbManager.AddParameters(2, "@in_vLoginToken", Token);
                dbManager.AddParameters(3, "@ReturnValue", ParameterDirection.ReturnValue);
                dbManager.ExecuteScalar(CommandType.StoredProcedure, "USP_Query_DeleteQuery");
                //returnValue = (int)dbManager.GetOutputParameterValue("@ReturnValue");
                returnValue = 0;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }
            if (returnValue == 0)
            {
                return true;
            }
            else
            {
                return false;
            }

            /*SqlCommand sqlCmd = new SqlCommand();
            SqlConnection sqlcon = new SqlConnection();
            int returnValue = 1;
            try
            {
                sqlcon = OpenConnection();
                sqlCmd.Connection = sqlcon;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "USP_Query_DeleteQuery";
                sqlCmd.Parameters.Add("@QueryId", SqlDbType.Int).Value = QueryId;
                sqlCmd.Parameters.Add("@ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                sqlCmd.ExecuteScalar();
                returnValue = (int)sqlCmd.Parameters["@ReturnValue"].Value;

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
                CloseConnection(sqlcon);
            }
            if (returnValue == 0)
            {
                return true;
            }
            else
            {
                return false;
            }*/
        }
        /// <summary>
        /// Updating and inserting into queries and queryclause tables
        /// </summary>
        /// <param name="queryId"></param>
        /// <param name="userName"></param>
        /// <param name="projectId"></param>
        /// <param name="queryName"></param>
        /// <param name="isPublic"></param>
        /// <param name="queryClauses"></param>
        /// <returns></returns>
        public int UpdateQuery(int queryId,int projectId, string queryName, bool isPublic, List<QueryClause> queryClauses,int orgId,string Token)
        {
            if (queryId <= 0)
                throw new ArgumentOutOfRangeException("queryId");

            if (queryName == null || queryName == String.Empty)
                throw (new ArgumentOutOfRangeException("queryName"));

            if (queryClauses.Count == 0)
                throw (new ArgumentOutOfRangeException("queryClauses"));

            DataSet dsQuery = new DataSet();
            int ReturnValue = 0;

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();

                dbManager.CreateParameters(6);
                dbManager.AddParameters(0, "@QueryId", queryId);               
                dbManager.AddParameters(1, "@ProjectId", projectId);
                dbManager.AddParameters(2, "@QueryName", queryName);
                dbManager.AddParameters(3, "@IsPublic", isPublic);
                dbManager.AddParameters(4, "@in_iLoginOrgId", orgId);
                dbManager.AddParameters(5, "@in_vLoginToken", Token);
                dbManager.ExecuteNonQuery(CommandType.StoredProcedure, "USP_Query_UpdateQuery");

                // Save Query Clauses
                foreach (QueryClause clause in queryClauses)
                {
                    dbManager.CreateParameters(10);
                    dbManager.AddParameters(0, "@QueryId", queryId);
                    dbManager.AddParameters(1, "@BooleanOperator", clause.BooleanOperator);
                    dbManager.AddParameters(2, "@FieldName", clause.FieldName);
                    dbManager.AddParameters(3, "@ComparisonOperator", clause.ComparisonOperator);
                    dbManager.AddParameters(4, "@FieldValue", clause.FieldValue);
                    dbManager.AddParameters(5, "@DataType", clause.DataType);
                    dbManager.AddParameters(6, "@IsCustomField", clause.CustomFieldQuery);
                    dbManager.AddParameters(7, "@ObjectType", clause.ObjectType);
                    dbManager.AddParameters(8,"@in_iLoginOrgId",orgId);
                    dbManager.AddParameters(9,"@in_vLoginToken",Token);
                    ReturnValue = dbManager.ExecuteNonQuery(CommandType.StoredProcedure, "USP_Query_SaveQueryClause");
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

            /*
                 sqlcon = OpenConnection();
                 sqlCmd.Connection = sqlcon;
                 sqlCmd.CommandType = CommandType.StoredProcedure;
                 sqlCmd.CommandText = "USP_Query_UpdateQuery";
                 sqlCmd.Parameters.Add("@QueryId", SqlDbType.Int).Value = queryId;
                 sqlCmd.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = userName;
                 sqlCmd.Parameters.Add("@ProjectId", SqlDbType.Int).Value = projectId;
                 sqlCmd.Parameters.Add("@QueryName", SqlDbType.NVarChar).Value = queryName;
                 sqlCmd.Parameters.Add("@IsPublic", SqlDbType.Bit).Value = isPublic;
                 sqlCmd.ExecuteNonQuery();
                 if (sqlCmd != null)
                 {
                     sqlCmd.Dispose();
                 }

                 SqlCommand cmdClause = new SqlCommand();
                 cmdClause.Connection = sqlcon;
                 cmdClause.CommandType = CommandType.StoredProcedure;
                 cmdClause.CommandText = "USP_Query_SaveQueryClause";
                 cmdClause.Parameters.Add("@QueryId", SqlDbType.Int);
                 cmdClause.Parameters.Add("@BooleanOperator", SqlDbType.NVarChar, 50);
                 cmdClause.Parameters.Add("@FieldName", SqlDbType.NVarChar, 50);
                 cmdClause.Parameters.Add("@ComparisonOperator", SqlDbType.NVarChar, 50);
                 cmdClause.Parameters.Add("@FieldValue", SqlDbType.NVarChar, 50);
                 cmdClause.Parameters.Add("@DataType", SqlDbType.Int);
                 cmdClause.Parameters.Add("@IsCustomField", SqlDbType.Bit);
                 cmdClause.Parameters.Add("@ObjectType", SqlDbType.NVarChar, 50);

                 // Save Query Clauses
                 foreach (QueryClause clause in queryClauses)
                 {
                     cmdClause.Parameters["@QueryId"].Value = queryId;
                     cmdClause.Parameters["@BooleanOperator"].Value = clause.BooleanOperator;
                     cmdClause.Parameters["@FieldName"].Value = clause.FieldName;
                     cmdClause.Parameters["@ComparisonOperator"].Value = clause.ComparisonOperator;
                     cmdClause.Parameters["@FieldValue"].Value = clause.FieldValue;
                     cmdClause.Parameters["@DataType"].Value = clause.DataType;
                     cmdClause.Parameters["@IsCustomField"].Value = clause.CustomFieldQuery;
                     cmdClause.Parameters["@ObjectType"].Value = clause.ObjectType;

                     ReturnValue = cmdClause.ExecuteNonQuery();
                 }
            */
            return ReturnValue;
        }

        public DataSet ManageQueryClause(string Action, int TemplateId,int orgId,string Token)
        {
            DataSet dsQuery = new DataSet();

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(7);

                dbManager.AddParameters(0, "@in_vAction", Action);
                dbManager.AddParameters(1, "@in_iTemplateId", TemplateId);
                dbManager.AddParameters(2, "@in_iLoginOrgId", orgId);
                dbManager.AddParameters(3, "@in_vLoginToken", Token);
                dbManager.AddParameters(4, "@out_iErrorState", 0, ParameterDirection.Output);
                dbManager.AddParameters(5, "@out_iErrorSeverity", 0, ParameterDirection.Output);
                dbManager.AddParameters(6, "@out_vMessage", string.Empty, DbType.String, 250, ParameterDirection.Output);

                dsQuery = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_Query_ManageQueryClause");
                

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }
            return dsQuery;
            /*
            SqlCommand sqlCmd = new SqlCommand();
            SqlConnection sqlcon = new SqlConnection();
            SqlDataAdapter sqlda = new SqlDataAdapter();
            DataSet dsQuery = new DataSet();
            try
            {
                sqlcon = OpenConnection();
                sqlCmd.Connection = sqlcon;
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.CommandText = "USP_Query_ManageQueryClause";
                sqlCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar).Value = Action;
                sqlCmd.Parameters.Add("@in_iTemplateId", SqlDbType.Int).Value = TemplateId;
                sqlda.SelectCommand = sqlCmd;
                sqlda.Fill(dsQuery);
            }
            catch (Exception)
            {


            }
            return dsQuery;
              */
        }
    }
}
