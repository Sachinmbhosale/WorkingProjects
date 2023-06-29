using System;
using WorkflowBAL;
using DataAccessLayer;
using System.Data;

namespace WorkflowBLL.Classes
{
    public class WorkFlowSentMailBLL : WorkflowBase
    {

        #region ConstructorLogic
        public WorkFlowSentMailBLL()
        {
            WorkflowId = 0;
            ProcessId = 0;
            StageId = 0;
            DataId = 0;
            Filedvalue = 0;
            
        }

        #endregion

        #region Default Properties

        public int StageId { get; set; }
        public int WorkflowId { get; set; }
        public int ProcessId { get; set; }
        public int DataId { get; set; }
        public int Filedvalue { get; set; }
        #endregion


        public DBResult ManageWrokFlowSentMail(WorkFlowSentMailBLL Prop, string Action)
        {
            DBResult objDBResult = new DBResult();
            DataSet ds = new DataSet();
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(10);
                dbManager.AddParameters(0, "@in_iProcessId", Prop.ProcessId);
                dbManager.AddParameters(1, "@in_DataId", Prop.DataId);
                dbManager.AddParameters(2, "@in_iWorkflowId", Prop.WorkflowId);
                dbManager.AddParameters(3, "@in_vFieldVlue", Prop.Filedvalue);
                dbManager.AddParameters(4, "@in_vAction", Action);
                dbManager.AddParameters(5, "@in_vLoginToken", Prop.LoginToken);
                dbManager.AddParameters(6, "@in_iLoginOrgId", Prop.LoginOrgId);
           
                dbManager.AddParameters(7, "@out_iErrorState", 0, ParameterDirection.Output);
                dbManager.AddParameters(8, "@out_iErrorSeverity", 0, ParameterDirection.Output);
                dbManager.AddParameters(9, "@out_vMessage", string.Empty, DbType.String, 250, ParameterDirection.Output);
                ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_Workflow_ManageDataCaptureSentMail");
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



            return objDBResult;
        }
    }
}
