using System;
using WorkflowBAL;
using System.Data;
using DataAccessLayer;

namespace WorkflowBLL.Classes
{
    public class WorkFlowHomePageBLL : WorkflowBase
    {

        public WorkFlowHomePageBLL()
        {
            ProcessId = 0;
            WorkFlowId = 0;
            StageId = 0;
            StatusId = 0;
        }

        #region Properties
        public int ProcessId { get; set; }
        public int WorkFlowId { get; set; }
        public int StageId { get; set; }
        public int StatusId { get; set; }
        #endregion


        public DBResult ManageWorkflowDashBoard(WorkFlowHomePageBLL Properties, string Action)
        {
            DBResult objDBResult = new DBResult();
            DataSet ds = new DataSet();
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                string StroredProcedure = "USP_Workflow_ManageDashBoard";
               
                dbManager.Open();
                dbManager.CreateParameters(10);
                dbManager.AddParameters(0, "@in_vLoginToken", Properties.LoginToken);
                dbManager.AddParameters(1, "@in_iLoginOrgId", Properties.LoginOrgId);
                dbManager.AddParameters(2, "@in_vAction", Action);
                dbManager.AddParameters(3, "@in_iProcessId", Properties.ProcessId);
                dbManager.AddParameters(4, "@in_iWorkflowId", Properties.WorkFlowId);
                dbManager.AddParameters(5, "@in_iStageId", Properties.StageId);
                dbManager.AddParameters(6, "@in_iStatusId", Properties.StatusId);
                dbManager.AddParameters(7, "@out_iErrorState", 0, ParameterDirection.Output);
                dbManager.AddParameters(8, "@out_iErrorSeverity", 0, ParameterDirection.Output);
                dbManager.AddParameters(9, "@out_vMessage", string.Empty, DbType.String, 250, ParameterDirection.Output);

                ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, StroredProcedure);

                objDBResult.dsResult = ds;
                string errState = dbManager.GetOutputParameterValue("@out_iErrorState").ToString().Trim() == "" ? "0" : dbManager.GetOutputParameterValue("@out_iErrorState").ToString().Trim();
                string errSev = dbManager.GetOutputParameterValue("@out_iErrorSeverity").ToString().Trim() == "" ? "0" : dbManager.GetOutputParameterValue("@out_iErrorSeverity").ToString().Trim();
                objDBResult.ErrorState = Convert.ToInt32(errState);
                objDBResult.ErrorSeverity = Convert.ToInt32(errSev);
                objDBResult.Message = dbManager.GetOutputParameterValue("@out_vMessage").ToString().Trim();


             

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                dbManager.Dispose();
            }
            return objDBResult;
        }
    }
}
