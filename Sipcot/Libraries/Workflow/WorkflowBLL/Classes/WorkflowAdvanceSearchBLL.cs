using System;
using WorkflowBAL;
using System.Data;
using DataAccessLayer;

namespace WorkflowBLL.Classes
{
    public class WorkflowAdvanceSearchBLL : WorkflowBase
    {      

        #region Properties
        public int WorkflowAdvanceSearchProcessId { get; set; }
        public int WorkflowAdvanceSearchWorkflowId { get; set; }
        public int WorkflowAdvanceSearchStageId { get; set; }
        public int WorkflowAdvanceSearchStatusId { get; set; }
        public string WorkflowAdvanceSearchDynamicQuery { get; set; }
        public string WorkflowAdvanceQueryName { get; set; }
        public int WorkflowAdvanceMasterTypeId { get; set; }
        public string WorkflowAdvanceQueryClause { get; set; }
        public int WorkflowAdvanceQueryId { get; set; }
        public string WorkflowAdvanceSearchQueryStartDate { get; set; }
        public string WorkflowAdvanceSearchQueryEndDate { get; set; }
        #endregion

        #region ConstuctorLogic
        public WorkflowAdvanceSearchBLL()
        {
            WorkflowAdvanceSearchProcessId = 0;
            WorkflowAdvanceSearchWorkflowId = 0;
            WorkflowAdvanceSearchStageId = 0;
            WorkflowAdvanceSearchStatusId = 0;
            WorkflowAdvanceSearchDynamicQuery = string.Empty;
            WorkflowAdvanceQueryName = string.Empty;
            WorkflowAdvanceMasterTypeId = 0;
            WorkflowAdvanceQueryClause = string.Empty;
            WorkflowAdvanceQueryId = 0;
            WorkflowAdvanceSearchQueryStartDate = string.Empty;
            WorkflowAdvanceSearchQueryEndDate = string.Empty;
        }

        #endregion

        public DBResult ManageWorkflowAdvanceSearch(WorkflowAdvanceSearchBLL Properties, string Actions)
        {
            DBResult objDBResult = new DBResult();
            DataSet ds = new DataSet();
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            string dynamicQuery = QueryParser(Properties.WorkflowAdvanceSearchDynamicQuery, DataProvider.MySql);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(17);

                dbManager.AddParameters(0, "@in_iProcessId", Properties.WorkflowAdvanceSearchProcessId);
                dbManager.AddParameters(1, "@in_iWorkflowId", Properties.WorkflowAdvanceSearchWorkflowId);
                dbManager.AddParameters(2, "@in_iStageId", Properties.WorkflowAdvanceSearchStageId);
                dbManager.AddParameters(3, "@in_vAction", Actions);
                dbManager.AddParameters(4, "@in_vLoginToken", Properties.LoginToken);
                dbManager.AddParameters(5, "@in_iLoginOrgId", Properties.LoginOrgId);

                dbManager.AddParameters(6, "@out_iErrorState", 0, ParameterDirection.Output);
                dbManager.AddParameters(7, "@out_iErrorSeverity", 0, ParameterDirection.Output);
                dbManager.AddParameters(8, "@out_vMessage", string.Empty, DbType.String, 250, ParameterDirection.Output);

                dbManager.AddParameters(9, "@in_iStatusId", Properties.WorkflowAdvanceSearchStatusId);
                dbManager.AddParameters(10, "@in_vDynamicQuery", dynamicQuery);
                dbManager.AddParameters(11, "@in_vQueryName", Properties.WorkflowAdvanceQueryName);
                dbManager.AddParameters(12, "@in_iMasterTypeId", Properties.WorkflowAdvanceMasterTypeId);
                dbManager.AddParameters(13, "@in_vQueryClause", Properties.WorkflowAdvanceQueryClause);
                dbManager.AddParameters(14, "@in_iQueryId", Properties.WorkflowAdvanceQueryId);
                dbManager.AddParameters(15, "@in_vQueryStartDate", Properties.WorkflowAdvanceSearchQueryStartDate);
                dbManager.AddParameters(16, "@in_vQueryEndDate", Properties.WorkflowAdvanceSearchQueryEndDate);

                ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_Workflow_ManageAdvanceSearch");

                objDBResult.dsResult = ds;
                string errState = dbManager.GetOutputParameterValue("@out_iErrorState").ToString().Trim() == "" ? "0" : dbManager.GetOutputParameterValue("@out_iErrorState").ToString().Trim();
                string errSev = dbManager.GetOutputParameterValue("@out_iErrorSeverity").ToString().Trim() == "" ? "0" : dbManager.GetOutputParameterValue("@out_iErrorSeverity").ToString().Trim();
                objDBResult.ErrorState = Convert.ToInt32(errState);
                objDBResult.ErrorSeverity = Convert.ToInt32(errSev);
                objDBResult.Message = dbManager.GetOutputParameterValue("@out_vMessage").ToString().Trim();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }
           

            //string storedProcedure = "USP_Workflow_ManageAdvanceSearch";
            //SqlParameter[] parameters = new SqlParameter[17];
            //parameters[0] = new SqlParameter("@in_iProcessId", Properties.WorkflowAdvanceSearchProcessId);
            //parameters[1] = new SqlParameter("@in_iWorkflowId", Properties.WorkflowAdvanceSearchWorkflowId);
            //parameters[2] = new SqlParameter("@in_iStageId", Properties.WorkflowAdvanceSearchStageId);
            //parameters[3] = new SqlParameter("@in_vAction", Actions);
            //parameters[4] = new SqlParameter("@in_vLoginToken", Properties.LoginToken);
            //parameters[5] = new SqlParameter("@in_iLoginOrgId", Properties.LoginOrgId);
            //parameters[6] = new SqlParameter("@out_iErrorState", SqlDbType.Int) { Direction = ParameterDirection.Output };
            //parameters[7] = new SqlParameter("@out_iErrorSeverity", SqlDbType.Int) { Direction = ParameterDirection.Output };
            //parameters[8] = new SqlParameter("@out_vMessage", SqlDbType.NVarChar, 250) { Direction = ParameterDirection.Output };
            //parameters[9] = new SqlParameter("@in_iStatusId", Properties.WorkflowAdvanceSearchStatusId);
            //parameters[10] = new SqlParameter("@in_vDynamicQuery", Properties.WorkflowAdvanceSearchDynamicQuery);
            //parameters[11] = new SqlParameter("@in_vQueryName", Properties.WorkflowAdvanceQueryName);
            //parameters[12] = new SqlParameter("@in_iMasterTypeId", Properties.WorkflowAdvanceMasterTypeId);
            //parameters[13] = new SqlParameter("@in_vQueryClause", Properties.WorkflowAdvanceQueryClause);
            //parameters[14] = new SqlParameter("@in_iQueryId", Properties.WorkflowAdvanceQueryId);
            //parameters[15] = new SqlParameter("@in_vQueryStartDate", Properties.WorkflowAdvanceSearchQueryStartDate);
            //parameters[16] = new SqlParameter("@in_vQueryEndDate", Properties.WorkflowAdvanceSearchQueryEndDate);
            //objDBResult = dbManager.ExecuteDataSet(storedProcedure, parameters);
           
            return objDBResult;
        }
    }
}
