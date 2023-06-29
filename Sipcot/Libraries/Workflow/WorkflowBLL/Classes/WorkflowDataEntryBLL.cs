using System;
using WorkflowBAL;
using System.Data;
using DataAccessLayer;


namespace WorkflowBLL.Classes
{
    public class WorkflowDataEntryBLL : WorkflowBase
    {
        public DBResult ManageWorkflowDataEntry(WorkflowDataEntryBLL Properties, string Action)
        {
             DBResult objDBResult = new DBResult();
             DataSet ds = new DataSet();
             IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
               
              
                dbManager.Open();
                dbManager.CreateParameters(14);

                dbManager.AddParameters(0, "@in_iProcessId", Properties.WorkflowDataEntryProcessId);
                dbManager.AddParameters(1, "@in_iWorkflowId", Properties.WorkflowDataEntryWorkflowId);
                dbManager.AddParameters(2, "@in_iStageId", Properties.WorkflowDataEntryStageId);
                dbManager.AddParameters(3, "@in_vAction", Action);
                dbManager.AddParameters(4, "@in_vLoginToken", Properties.LoginToken);
                dbManager.AddParameters(5, "@in_iLoginOrgId", Properties.LoginOrgId);
                dbManager.AddParameters(6, "@in_iMasterTypeId", Properties.WorkflowDataEntryMasterTypeId);
                dbManager.AddParameters(7, "@in_iParentId", Properties.WorkflowDataEntryParentId);
                dbManager.AddParameters(8, "@in_iStatusId", Properties.WorkflowDataEntryFieldStatusId);
                dbManager.AddParameters(9, "@in_xXmlData", Properties.WorkflowDataEntryXmlData);
                dbManager.AddParameters(10, "@in_iDataRowId", Properties.WorkflowDataEntryFieldDataId);

                dbManager.AddParameters(11, "@out_iErrorState", 0, ParameterDirection.Output);
                dbManager.AddParameters(12, "@out_iErrorSeverity", 0, ParameterDirection.Output);
                dbManager.AddParameters(13, "@out_vMessage", string.Empty, DbType.String, 250, ParameterDirection.Output);
               

                ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_Workflow_ManageDataEntry");

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

        public WorkflowDataEntryBLL()
        {
            //Constructor Logic
            WorkflowDataEntryProcessId = 0;
            WorkflowDataEntryWorkflowId = 0;
            WorkflowDataEntryStageId = 0;
            WorkflowDataEntryMasterTypeId = 0;
            WorkflowDataEntryXmlData = string.Empty;
            WorkflowDataEntryParentId = 0;
            WorkflowDataEntryFieldDataId = 0;
            WorkflowDataEntryFieldStatusId = 0;

        }

        public int WorkflowDataEntryProcessId { get; set; }
        public int WorkflowDataEntryWorkflowId { get; set; }
        public int WorkflowDataEntryStageId { get; set; }
        public int WorkflowDataEntryMasterTypeId { get; set; }
        public string WorkflowDataEntryXmlData { get; set; }
        public int WorkflowDataEntryParentId { get; set; }
        public int WorkflowDataEntryFieldDataId { get; set; }
        public int WorkflowDataEntryFieldStatusId { get; set; }

    }
}
