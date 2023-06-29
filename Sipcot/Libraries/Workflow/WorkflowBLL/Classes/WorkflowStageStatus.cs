using System;
using System.Data;
using WorkflowBAL;
using DataAccessLayer;

namespace WorkflowBLL.Classes
{
    #region

    #endregion
    public class WorkflowStageStatus : WorkflowBase
    {      

        #region Constructor Logic
        public WorkflowStageStatus()
        {
            WorkflowStageStatusesMasterId = 0;
            WorkflowStageStatusesStageId = 0;
            WorkflowStageStatusesStatusId = 0;
            WorkflowStageStatusCurrentstatusId = 0;
            WorkflowStageStatusesMoveToStageId = 0;
            WorkflowStageStatusesConfirmOnSubmit = false;
            WorkflowStageStatusesConfirmMsgOnSubmit = string.Empty;
            WorkflowStageStatusesSendNotification = false;
            WorkflowStageStatusesWorkFlowId = 0;
            WorkflowStageStatusesProcessId = 0;
            WorkflowStageStatusesOrgId = 0;
            WorkflowStageStatusesIsActive = true;
            CommaSeparatedStatusIds = string.Empty;
            WorkflowStageStatusesName = string.Empty;
            WorkflowStageStatusesDescription = string.Empty;
         
        }
        #endregion
        #region Default Propperties

        public int WorkflowStageStatusesMasterId { get; set; }
        public int WorkflowStageStatusesStageId { get; set; }
        public int WorkflowStageStatusesStatusId { get; set; }
        public int WorkflowStageStatusesMoveToStageId { get; set; }
        public bool WorkflowStageStatusesConfirmOnSubmit { get; set; }
        public string WorkflowStageStatusesConfirmMsgOnSubmit { get; set; }
        public int WorkflowStageStatusesWorkFlowId { get; set; }
        public int WorkflowStageStatusesProcessId { get; set; }
        public int WorkflowStageStatusesOrgId { get; set; }
        public bool WorkflowStageStatusesIsActive { get; set; }
        public string WorkflowStageStatusesName { get; set; }
        public string WorkflowStageStatusesDescription { get; set; }
        public string CommaSeparatedStatusIds { get; set; }
        public int WorkflowStageStatusCurrentstatusId { get; set; }
        public bool WorkflowStageStatusesSendNotification { get; set; }
        #endregion

        #region ManageWorkflowStageStatuses

        public DBResult ManageWorkflowStageStatuses(WorkflowStageStatus prop, string Actions)
        {

            DBResult objDBResult = new DBResult();
            DataSet ds = new DataSet();
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(19);
                dbManager.AddParameters(0, "@in_iCurrstageStatusesId", prop.WorkflowStageStatusCurrentstatusId);
                dbManager.AddParameters(1, "@in_iStageId", prop.WorkflowStageStatusesStageId);
                dbManager.AddParameters(2, "@in_iStatusId", prop.WorkflowStageStatusesMasterId);
                dbManager.AddParameters(3, "@in_iMoveToStageId", prop.WorkflowStageStatusesMoveToStageId);
                dbManager.AddParameters(4, "@in_vCommaSeparatedStatusIds", prop.CommaSeparatedStatusIds);
                dbManager.AddParameters(5, "@in_iWorkflowId", prop.WorkflowStageStatusesWorkFlowId);
                dbManager.AddParameters(6, "@in_iProcessId", prop.WorkflowStageStatusesProcessId);
                dbManager.AddParameters(7, "@in_vStatusName", prop.WorkflowStageStatusesName);
                dbManager.AddParameters(8, "@in_vStatusDiscription", prop.WorkflowStageStatusesDescription);
                dbManager.AddParameters(9, "@in_bConfirmOnSubmit", prop.WorkflowStageStatusesConfirmOnSubmit);
                dbManager.AddParameters(10, "@in_vConfirmMsgOnSubmit", prop.WorkflowStageStatusesConfirmMsgOnSubmit);
                dbManager.AddParameters(11, "@in_bSendNotification", prop.WorkflowStageStatusesSendNotification);
                dbManager.AddParameters(12, "@in_vAction", Actions);
                dbManager.AddParameters(13, "@in_vLoginToken", prop.LoginToken);
                dbManager.AddParameters(14, "@in_iLoginOrgId", prop.LoginOrgId);
                dbManager.AddParameters(15, "@in_bIsActive", prop.WorkflowStageStatusesIsActive);
                dbManager.AddParameters(16, "@out_iErrorState", 0, ParameterDirection.Output);
                dbManager.AddParameters(17, "@out_iErrorSeverity", 0, ParameterDirection.Output);
                dbManager.AddParameters(18, "@out_vMessage", string.Empty, DbType.String, 250, ParameterDirection.Output);


                ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_Workflow_ManageStageStatuses");

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

            
            //DBResult ObjDbResult = new DBResult();
          
            //string StoredProcedure = "USP_Workflow_ManageStageStatuses";
            //SqlParameter[] parameters = new SqlParameter[19];
            //parameters[0] = new SqlParameter { ParameterName = "@in_iCurrstageStatusesId", Value = prop.WorkflowStageStatusCurrentstatusId };
            //parameters[1] = new SqlParameter { ParameterName = "@in_iStageId", Value = prop.WorkflowStageStatusesStageId };
            //parameters[2] = new SqlParameter { ParameterName = "@in_iStatusId", Value = prop.WorkflowStageStatusesMasterId };
            //parameters[3] = new SqlParameter { ParameterName = "@in_iMoveToStageId", Value = prop.WorkflowStageStatusesMoveToStageId };
            //parameters[4] = new SqlParameter { ParameterName = "@in_vCommaSeparatedStatusIds", Value = prop.CommaSeparatedStatusIds };
            //parameters[5] = new SqlParameter { ParameterName = "@in_iWorkflowId", Value = prop.WorkflowStageStatusesWorkFlowId };
            //parameters[6] = new SqlParameter { ParameterName = "@in_iProcessId", Value = prop.WorkflowStageStatusesProcessId };
            //parameters[7] = new SqlParameter { ParameterName = "@in_vStatusName", Value = prop.WorkflowStageStatusesName };
            //parameters[8] = new SqlParameter { ParameterName = "@in_vStatusDiscription", Value = prop.WorkflowStageStatusesDescription };
            //parameters[9] = new SqlParameter { ParameterName = "@in_bConfirmOnSubmit", Value = prop.WorkflowStageStatusesConfirmOnSubmit };
            //parameters[10] = new SqlParameter { ParameterName = "@in_vConfirmMsgOnSubmit", Value = prop.WorkflowStageStatusesConfirmMsgOnSubmit };
            //parameters[11] = new SqlParameter { ParameterName = "@in_bSendNotification", Value = prop.WorkflowStageStatusesSendNotification };
            //parameters[12] = new SqlParameter { ParameterName = "@in_vAction", Value = Actions };
            //parameters[13] = new SqlParameter { ParameterName = "@in_vLoginToken", Value = prop.LoginToken };
            //parameters[14] = new SqlParameter { ParameterName = "@in_iLoginOrgId", Value = prop.LoginOrgId };
            //parameters[15] = new SqlParameter { ParameterName = "@in_bIsActive", Value = prop.WorkflowStageStatusesIsActive };
            //parameters[16] = new SqlParameter { ParameterName = "@out_iErrorState", Direction = ParameterDirection.Output, SqlDbType = SqlDbType.Int };
            //parameters[17] = new SqlParameter { ParameterName = "@out_iErrorSeverity", Direction = ParameterDirection.Output, SqlDbType = SqlDbType.Int };
            //parameters[18] = new SqlParameter { ParameterName = "@out_vMessage", Direction = ParameterDirection.Output, Size = 250, SqlDbType = SqlDbType.NVarChar };


            //ObjDbResult = objDBHelper.ExecuteDataset(StoredProcedure, parameters);
            return objDBResult;
        }
        #endregion
    }
}
