using System;
using WorkflowBAL;
using System.Data;
using DataAccessLayer;

namespace WorkflowBLL.Classes
{
    public class WorkflowReports : WorkflowBase
    {


        public WorkflowReports()
        {
            ProcessId = 0;
            WorkFlowId = 0;
            StageId = 0;
            StatusId = 0;
            StatusName = string.Empty;
        }

        #region Properties
        public int ProcessId { get; set; }
        public int WorkFlowId { get; set; }
        public int StageId { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        #endregion


        public DBResult ManageWorkflowReports(WorkflowReports Properties, string Action)
        { 
            DBResult objDBResult = new DBResult();
            DataSet ds = new DataSet();
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                string StroredProcedure = string.Empty;
                if (Action == "ReportTATSummary")
                {
                    StroredProcedure = "USP_Workflow_ReportTATSummary";
                }
                else if (Action == "ReportTaskStatusSummary")
                {
                    StroredProcedure = "USP_Workflow_ReportTaskStatusSummary";
                }
                else if (Action == "ReportUserwiseProductivity")
                {
                    StroredProcedure = "USP_Workflow_ReportUserwiseProductivity";
                }               
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


                //SqlParameter[] Parameters = new SqlParameter[10];
                //Parameters[0] = new SqlParameter { ParameterName = "@in_vLoginToken", Value = Properties.LoginToken };
                //Parameters[1] = new SqlParameter { ParameterName = "@in_iLoginOrgId", Value = Properties.LoginOrgId };
                //Parameters[2] = new SqlParameter { ParameterName = "@in_vAction", Value = Action }
                //Parameters[3] = new SqlParameter { ParameterName = "@in_iProcessId", Value = Properties.ProcessId };
                //Parameters[4] = new SqlParameter { ParameterName = "@in_iWorkflowId", Value = Properties.WorkFlowId };
                //Parameters[5] = new SqlParameter { ParameterName = "@in_iStageId", Value = Properties.StageId };
                //Parameters[6] = new SqlParameter { ParameterName = "@in_iStatusId", Value = Properties.StatusId };
                //Parameters[7] = new SqlParameter("@out_iErrorState", SqlDbType.Int) { Direction = ParameterDirection.Output };
                //Parameters[8] = new SqlParameter("@out_iErrorSeverity", SqlDbType.Int) { Direction = ParameterDirection.Output };
                //Parameters[9] = new SqlParameter("@out_vMessage", SqlDbType.NVarChar, 250) { Direction = ParameterDirection.Output };

                //objDBResult = objDBHelper.ExecuteDataset(StroredProcedure, Parameters);

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
