using System;
using System.Data;
using WorkflowBAL;
using DataAccessLayer;

namespace WorkflowBLL.Classes
{
    public class Workflows : WorkflowBase
    {      

        #region Constructorlogic
        public Workflows()
        {
            WorkFlowName = string.Empty;
            WorkFlowDescription = string.Empty;
            WorkFlowProcessId = 0;
            WorkFlowOrgId = 0;
            WorkFlowOrderNo = 0;
            WorkflowIsActive = false;
            WorkflowIsDeleted = false;
            WorkflowDMSProjectId = 0;
            WorkflowPriority = string.Empty;
            WorkFlowSelectedOrgId = 0;
        }
        #endregion

        #region Properties
        public int WorkflowId { get; set; }
        public string WorkFlowName { get; set; }
        public string WorkFlowDescription { get; set; }
        public int WorkFlowProcessId { get; set; }
        public int WorkFlowOrgId { get; set; }
        public int WorkFlowOrderNo { get; set; }
        public bool WorkflowIsActive { get; set; }
        public bool WorkflowIsDeleted { get; set; }
        public int WorkflowDMSProjectId { get; set; }
        public string WorkflowPriority { get; set; }
        public int WorkFlowSelectedOrgId { get; set; }
        #endregion

        #region Insert workflows

        public DBResult ManageWorkflows(Workflows Properties, string Actions)
        {

            DBResult objDBResult = new DBResult();
            DataSet ds = new DataSet();
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(16);
                dbManager.AddParameters(0, "@in_iCurrWorkflowId", Properties.WorkflowId);
                dbManager.AddParameters(1, "@in_vWorkflowName", Properties.WorkFlowName);
                dbManager.AddParameters(2, "@in_vDescription", Properties.WorkFlowDescription);
                dbManager.AddParameters(3, "@in_bWorkflowIsActive", Properties.WorkflowIsActive);
                dbManager.AddParameters(4, "@in_bIsDeleted", Properties.WorkflowIsDeleted);
                dbManager.AddParameters(5, "@in_iWorkflowOrderNo", Properties.WorkFlowOrderNo);
                dbManager.AddParameters(6, "@in_iWorkflowProcessId", Properties.WorkFlowProcessId);
                dbManager.AddParameters(7, "@in_vAction", Actions);
                dbManager.AddParameters(8, "@in_vLoginToken", Properties.LoginToken);
                dbManager.AddParameters(9, "@in_iLoginOrgId", Properties.LoginOrgId);
                dbManager.AddParameters(10, "@out_iErrorState", 0, ParameterDirection.Output);
                dbManager.AddParameters(11, "@out_iErrorSeverity", 0, ParameterDirection.Output);
                dbManager.AddParameters(12, "@out_vMessage", string.Empty, DbType.String, 250, ParameterDirection.Output);
                dbManager.AddParameters(13, "@in_iDMSProjectId", Properties.WorkflowDMSProjectId);
                dbManager.AddParameters(14, "@in_vWorkflowPriority", Properties.WorkflowPriority);
                dbManager.AddParameters(15, "@in_iSelectedOrgId", Properties.WorkFlowSelectedOrgId);
                ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_Workflow_ManageWorkflow");

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

            //DBResult objDBResult = new DBResult();
            //string storedProcedure = "USP_Workflow_ManageWorkflow";
            //SqlParameter[] parameters = new SqlParameter[14];
            //parameters[0] = new SqlParameter("@in_iCurrWorkflowId", Properties.WorkflowId);
            //parameters[1] = new SqlParameter("@in_vWorkflowName", Properties.WorkFlowName);
            //parameters[2] = new SqlParameter("@in_vDescription", Properties.WorkFlowDescription);
            //parameters[3] = new SqlParameter("@in_bWorkflowIsActive", Properties.WorkflowIsActive);
            //parameters[4] = new SqlParameter("@in_bIsDeleted", Properties.WorkflowIsDeleted);
            //parameters[5] = new SqlParameter("@in_iWorkflowOrderNo", Properties.WorkFlowOrderNo);
            //parameters[6] = new SqlParameter("@in_iWorkflowProcessId", Properties.WorkFlowProcessId);
            //parameters[7] = new SqlParameter("@in_vAction", Actions);
            //parameters[8] = new SqlParameter("@in_vLoginToken", Properties.LoginToken);
            //parameters[9] = new SqlParameter("@in_iLoginOrgId", Properties.LoginOrgId);
            //(parameters[10] = new SqlParameter("@out_iErrorState", System.Data.SqlDbType.Int)).Direction = ParameterDirection.Output;
            //(parameters[11] = new SqlParameter("@out_iErrorSeverity", System.Data.SqlDbType.Int)).Direction = ParameterDirection.Output;
            //(parameters[12] = new SqlParameter("@out_vMessage", System.Data.SqlDbType.NVarChar, 250)).Direction = ParameterDirection.Output;
            //parameters[13] = new SqlParameter("@in_iDMSProjectId",Properties.WorkflowDMSProjectId);

            //objDBResult = objDBHelper.ExecuteDataset(storedProcedure, parameters);
            return objDBResult;
        }
        #endregion

        public IDataReader GetChartData(Workflows Properties, string Actions)
        {
            DBResult objDBResult = new DBResult();
           
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(16);
                dbManager.AddParameters(0, "@in_iCurrWorkflowId", Properties.WorkflowId);
                dbManager.AddParameters(1, "@in_vWorkflowName", Properties.WorkFlowName);
                dbManager.AddParameters(2, "@in_vDescription", Properties.WorkFlowDescription);
                dbManager.AddParameters(3, "@in_bWorkflowIsActive", Properties.WorkflowIsActive);
                dbManager.AddParameters(4, "@in_bIsDeleted", Properties.WorkflowIsDeleted);
                dbManager.AddParameters(5, "@in_iWorkflowOrderNo", Properties.WorkFlowOrderNo);
                dbManager.AddParameters(6, "@in_iWorkflowProcessId", Properties.WorkFlowProcessId);
                dbManager.AddParameters(7, "@in_vAction", Actions);
                dbManager.AddParameters(8, "@in_vLoginToken", Properties.LoginToken);
                dbManager.AddParameters(9, "@in_iLoginOrgId", Properties.LoginOrgId);
                dbManager.AddParameters(10, "@out_iErrorState", 0, ParameterDirection.Output);
                dbManager.AddParameters(11, "@out_iErrorSeverity", 0, ParameterDirection.Output);
                dbManager.AddParameters(12, "@out_vMessage", string.Empty, DbType.String, 250, ParameterDirection.Output);
                dbManager.AddParameters(13, "@in_iDMSProjectId", Properties.WorkflowDMSProjectId);
                dbManager.AddParameters(14, "@in_vWorkflowPriority", Properties.WorkflowPriority);
                dbManager.AddParameters(15, "@in_iSelectedOrgId", Properties.WorkFlowSelectedOrgId);
                objDBResult.dataReader = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_Workflow_ManageWorkflow"); 
            
               
            }
            catch (Exception ex)
            {

                throw ex;
            }

            finally
            {
               
            }
                      
            return objDBResult.dataReader;
        }
    }
}
