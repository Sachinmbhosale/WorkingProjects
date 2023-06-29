using System;
using System.Data;
using System.Configuration;
using DataAccessLayer;

namespace WorkflowNotifications
{
   public class WorkflowNotificationData
    {

        public string DbConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["DatabaseConnString"].ConnectionString;
            }
        }

        public DataProvider ConfiguredDataProvider
        {
            get
            {
                string DatabaseSystem = ConfigurationManager.AppSettings["DatabaseSystem"];
                switch (DatabaseSystem.ToUpper())
                {
                    default:
                    case "SQLSERVER":
                        return DataProvider.SqlServer;
                    case "MYSQL":
                        return DataProvider.MySql;
                }
            }
        }

        public DBResult ManageNotificationData(WorkflowNotificationData Properties, string Action)
        {
            DBResult objDBResult = new DBResult();
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                // Query will be parsed to specific provider type
                string NotificationQuery = dbManager.ParseQuery(Properties.WorkflowNotificationData_NotificationQuery);
                dbManager.CreateParameters(9);
                dbManager.AddParameters(0, "@in_iProcessId", Properties.WorkflowNotificationData_ProcessId);
                dbManager.AddParameters(1, "@in_iWorkflowId", Properties.WorkflowNotificationData_WorkflowId);
                dbManager.AddParameters(2, "@in_iStageId", Properties.WorkflowNotificationData_StageId);
                dbManager.AddParameters(3, "@in_vAction", Action);
                dbManager.AddParameters(4, "@in_iDataFieldId", Properties.WorkflowNotificationData_FieldDataId);
                dbManager.AddParameters(5, "@out_iErrorState", 0, ParameterDirection.Output);
                dbManager.AddParameters(6, "@out_iErrorSeverity", 0, ParameterDirection.Output);
                dbManager.AddParameters(7, "@out_vMessage", string.Empty, DbType.String, 250, ParameterDirection.Output);
                dbManager.AddParameters(8, "@in_vSaveQuery", NotificationQuery);

                objDBResult.dsResult = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_Workflow_ManageNotificationData");

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

        public WorkflowNotificationData()
        {
            //Constructor Logic
            WorkflowNotificationData_ProcessId = 0;
            WorkflowNotificationData_WorkflowId = 0;
            WorkflowNotificationData_StageId = 0;
            WorkflowNotificationData_FieldDataId = 0;
            WorkflowNotificationData_NotificationQuery = string.Empty;
        }

        public int WorkflowNotificationData_ProcessId { get; set; }
        public int WorkflowNotificationData_WorkflowId { get; set; }
        public int WorkflowNotificationData_StageId { get; set; }
        public int WorkflowNotificationData_FieldDataId { get; set; }
        public string WorkflowNotificationData_NotificationQuery { get; set; }
       
       
    }
}
