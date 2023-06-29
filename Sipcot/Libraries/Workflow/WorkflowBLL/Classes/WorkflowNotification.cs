using System;
using System.Data;
using WorkflowBAL;
using DataAccessLayer;

namespace WorkflowBLL.Classes
{
    public class WorkflowNotification : WorkflowBase
    {  

        #region ConstructorLogic
        public WorkflowNotification()
        {
            NotificationId = 0;
            NotificationProcessId = 0;
            NotificationStageId = 0;
            NotificationStatusId = 0;
            NotificatontTATDurationTime = 0;
            NotificationCategory = string.Empty;
            NotificationIsActive = false;
        }
        #endregion

        #region
        public int NotificationId { get; set; }
        public int NotificationProcessId { get; set; }
        public int NotificationWorkflowId { get; set; }
        public int NotificationStageId { get; set; }
        public int NotificationStatusId { get; set; }
        public int NotificatontTATDurationTime { get; set; }
        public string NotificationCategory { get; set; }
        public bool NotificationIsActive { get; set; }
        #endregion

        public DBResult ManageNotificationConfiguration(WorkflowNotification prop, string Actions)
        {
            DBResult objDBResult = new DBResult();
            DataSet ds = new DataSet();
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(14);
                dbManager.AddParameters(0, "@in_iNotification_iId", prop.NotificationId);
                dbManager.AddParameters(1, "@in_iProcessId", prop.NotificationProcessId);
                dbManager.AddParameters(2, "@in_iWorkflowId", prop.NotificationWorkflowId);
                dbManager.AddParameters(3, "@in_iStageId", prop.NotificationStageId);
                dbManager.AddParameters(4, "@in_iStatusId", prop.NotificationStatusId);
                dbManager.AddParameters(5, "@in_iTatDurationMinutes", prop.NotificatontTATDurationTime);
                dbManager.AddParameters(6, "@in_vCategory", prop.NotificationCategory);
                dbManager.AddParameters(7, "@in_bIsActive", prop.NotificationIsActive);
                dbManager.AddParameters(8, "@in_vAction", Actions);
                dbManager.AddParameters(9, "@in_vLoginToken", prop.LoginToken);
                dbManager.AddParameters(10, "@in_iLoginOrgId", prop.LoginOrgId);
                dbManager.AddParameters(11, "@out_iErrorState", 0, ParameterDirection.Output);
                dbManager.AddParameters(12, "@out_iErrorSeverity", 0, ParameterDirection.Output);
                dbManager.AddParameters(13, "@out_vMessage", string.Empty, DbType.String, 250, ParameterDirection.Output);

                ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_Workflow_ManageNotification");

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
           

            
            //string StoredProcedure = "USP_Workflow_ManageNotification";
            //SqlParameter[] Parameters = new SqlParameter[14];
            //Parameters[0] = new SqlParameter("@in_iNotification_iId", prop.NotificationId);
            //Parameters[1] = new SqlParameter { ParameterName = "@in_iProcessId", Value = prop.NotificationProcessId };
            //Parameters[2] = new SqlParameter { ParameterName = "@in_iWorkflowId", Value = prop.NotificationWorkflowId };
            //Parameters[3] = new SqlParameter { ParameterName = "@in_iStageId", Value = prop.NotificationStageId };
            //Parameters[4] = new SqlParameter { ParameterName = "@in_iStatusId", Value = prop.NotificationStatusId };
            //Parameters[5] = new SqlParameter { ParameterName = "@in_iTatDurationMinutes", Value = prop.NotificatontTATDurationTime };
            //Parameters[6] = new SqlParameter { ParameterName = "@in_vCategory", Value = prop.NotificationCategory };
            //Parameters[7] = new SqlParameter { ParameterName = "@in_bIsActive", Value = prop.NotificationIsActive };
            //Parameters[8] = new SqlParameter { ParameterName = "@in_vAction", Value = Actions };
            //Parameters[9] = new SqlParameter { ParameterName = "@in_vLoginToken", Value = prop.LoginToken};
            //Parameters[10] = new SqlParameter { ParameterName = "@in_iLoginOrgId", Value = prop.LoginOrgId};
            //Parameters[11] = new SqlParameter { ParameterName = "@out_iErrorState", Direction = ParameterDirection.Output, SqlDbType = SqlDbType.Int };
            //Parameters[12] = new SqlParameter { ParameterName = "@out_iErrorSeverity", Direction = ParameterDirection.Output, SqlDbType = SqlDbType.Int };
            //Parameters[13] = new SqlParameter { ParameterName = "@out_vMessage", Direction = ParameterDirection.Output, SqlDbType = SqlDbType.NVarChar, Size = 250 };
            //objDBResult = objDBHelper.ExecuteDataset(StoredProcedure, Parameters);
            finally
            {
                dbManager.Dispose();
            }
            return objDBResult;

        }
    }
}
