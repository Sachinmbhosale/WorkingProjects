using System;
using System.Collections.Generic;
using Lotex.EnterpriseSolutions.CoreDAL;
using System.Data;
using Lotex.EnterpriseSolutions.CoreBE;

namespace Lotex.EnterpriseSolutions.CoreBL
{
    public class QueryBL
    {
        public bool SaveQuery(int projectId, string queryName, bool isPublic,string querytpye, List<QueryClause> queryClauses,int orgId,string Token)
        {
            QueryDAL objQueryDAL = new QueryDAL();
            bool bstatus;
            try
            {
                bstatus = objQueryDAL.SaveQuery(projectId, queryName,isPublic,querytpye,queryClauses,orgId,Token);
            }
            catch (Exception)
            {
                
                throw;
            }
            return bstatus;
        }

        public DataSet GetQueriesByUserName(int projectId,int orgId,string token)
        {
            QueryDAL objQueryDAL = new QueryDAL();
            DataSet dsValue = new DataSet();
            try
            {
                dsValue = objQueryDAL.GetQueriesByUserName(projectId,orgId,token);
            }
            catch (Exception)
            {
                
               
            }
            return dsValue;
        }

        public DataSet GetSavedQuery(int queryId,int orgId,string token)
        {
            QueryDAL objQueryDAL = new QueryDAL();
            DataSet dsValue = new DataSet();
            try
            {
                dsValue = objQueryDAL.GetSavedQuery(queryId,orgId,token);
            }
            catch (Exception)
            {
                
               
            }
            return dsValue;
        }
        public DataSet GetQueryById(int queryId,int orgId,string Token)
        {
            QueryDAL objQueryDAL = new QueryDAL();
            DataSet dsValue = new DataSet();
            try
            {
                dsValue = objQueryDAL.GetQueryById(queryId,orgId,Token);
            }
            catch (Exception)
            {
                
                
            }
            return dsValue;
        }
        public bool DeleteQuery(int QueryId,int orgId,string token)
        {
            QueryDAL objQueryDAL = new QueryDAL();
            bool returnValue=false;
            try
            {
                returnValue=objQueryDAL.DeleteQuery(QueryId,orgId,token);
                
            }
            catch (Exception)
            {
              
            }
            return returnValue;
        }

        public int UpdateQuery(int queryId,int projectId, string queryName, bool isPublic, List<QueryClause> queryClauses,int orgId,string Token)
        {
            QueryDAL objQueryDAL = new QueryDAL();
            int returnVal = 0;
            try
            {
                returnVal=objQueryDAL.UpdateQuery(queryId,projectId, queryName, isPublic, queryClauses,orgId,Token);
                
            }
            catch (Exception)
            {
                
                
            }
            return returnVal;
        }

        public DataSet ManageQueryClause(string Action,int TemplateId,int orgId,string Token)
        {
            QueryDAL objQueryDAL = new QueryDAL();
            DataSet dsQuery = new DataSet();
            try
            {
                dsQuery = objQueryDAL.ManageQueryClause(Action, TemplateId,orgId,Token);
            }
            catch (Exception)
            {
                
               
            }
            return dsQuery;
        }
    }
}
