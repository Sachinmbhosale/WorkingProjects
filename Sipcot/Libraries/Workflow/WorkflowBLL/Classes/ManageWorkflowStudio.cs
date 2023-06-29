using System;
using System.Data;
using WorkflowBAL;
using DataAccessLayer;

namespace WorkflowBLL.Classes
{
    public class ManageWorkflowStudio : WorkflowBase
    {
        public  ManageWorkflowStudio()
        {
            WorkflowId = 0;
            ProcessId = 0;
            StageId = 0;
            StatusId=0;
            Notificationid = 0;
        }

        public int ProcessId { get; set; }
        public int WorkflowId { get; set; }
        public int StageId { get; set; }
        public int StatusId { get; set; }
        public int Notificationid { get; set; }

        public DBResult ManageStudio(ManageWorkflowStudio Properties, string Actions)
       {

           DBResult objDBResult = new DBResult();
           DataSet ds = new DataSet();
           IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
           try
           {
               dbManager.Open();
               dbManager.CreateParameters(11);              
               dbManager.AddParameters(0, "@in_vAction", Actions);
               dbManager.AddParameters(1, "@in_vLoginToken", Properties.LoginToken);
               dbManager.AddParameters(2, "@in_iLoginOrgId", Properties.LoginOrgId);
               dbManager.AddParameters(3, "@out_iErrorState", 0, ParameterDirection.Output);
               dbManager.AddParameters(4, "@out_iErrorSeverity", 0, ParameterDirection.Output);
               dbManager.AddParameters(5, "@out_vMessage", string.Empty, DbType.String, 250, ParameterDirection.Output);
               dbManager.AddParameters(6, "@in_iProcessId", Properties.ProcessId);
               dbManager.AddParameters(7, "@in_iWorkflowId", Properties.WorkflowId);
               dbManager.AddParameters(8, "@in_iStageId", Properties.StageId);
               dbManager.AddParameters(9, "@in_iStatusId", Properties.StatusId);
               dbManager.AddParameters(10, "@in_iNotificationId ", Properties.Notificationid);

               ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_Workflow_ManageWorkflowStudio");

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
