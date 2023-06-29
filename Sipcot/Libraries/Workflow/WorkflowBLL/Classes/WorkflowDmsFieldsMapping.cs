/* ============================================================================  
Author     : Sharath
Create date: 31 Aug 2015
===============================================================================  
** Change History   
** Date:          Author:            Issue ID                         Description:  
** ----------   -------------       ----------                  ----------------------------
31/08/2015		Sharath				DMSENH6-4796  Bulk Upload in the Workflow

=============================================================================== */



using System;
using WorkflowBAL;
using System.Data;
using DataAccessLayer;

namespace WorkflowBLL.Classes
{

    //DMSENH6-4796 BS
    public class WorkflowDmsFieldsMapping : WorkflowBase
    {
        public WorkflowDmsFieldsMapping()
        {
            ProcessId = 0;
            WorkflowId = 0;
            StageID = 0;
            DoctypeId = 0;
            WorkFlowOrgId = 0;
            MappingId = 0;
            XmlData = string.Empty;

            DepatmentId = 0;

        }
        #region Properties
        public int WorkflowId { get; set; }
        public int ProcessId { get; set; }
        public int StageID { get; set; }
        public int WorkFlowOrgId { get; set; }
        public int  DoctypeId{ get; set; }
        public int MappingId { get; set; }

        public string XmlData { get; set; }

        public int DepatmentId { get; set; }

        #endregion
        #region Manage Workflow Upload

        public DBResult ManageUpload(WorkflowDmsFieldsMapping Properties, string Action)
        {

            DBResult objDBResult = new DBResult();
            DataSet ds = new DataSet();
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(14);
                dbManager.AddParameters(0, "@in_iProcessId", Properties.ProcessId);
                dbManager.AddParameters(1, "@in_iWorkflowId", Properties.WorkflowId);            
                dbManager.AddParameters(2, "@in_vAction", Action);
                dbManager.AddParameters(3, "@in_vLoginToken", Properties.LoginToken);
                dbManager.AddParameters(4, "@in_iLoginOrgId", Properties.LoginOrgId);
                dbManager.AddParameters(5, "@out_iErrorState", 0, ParameterDirection.Output);
                dbManager.AddParameters(6, "@out_iErrorSeverity", 0, ParameterDirection.Output);
                dbManager.AddParameters(7, "@out_vMessage", string.Empty, DbType.String, 250, ParameterDirection.Output);
                dbManager.AddParameters(8, "@in_iDocTypeId", Properties.DoctypeId);
                dbManager.AddParameters(9, "@in_iStageId", Properties.StageID);
                dbManager.AddParameters(10, "@in_iWorkFlowOrgId", Properties.WorkFlowOrgId);
                dbManager.AddParameters(11, "@in_iDepTypeId", Properties.DepatmentId);
                dbManager.AddParameters(12, "@in_xXmlData", Properties.XmlData);
                dbManager.AddParameters(13, "@in_iMappingId", Properties.MappingId);
                ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_Workflow_ManageWorkflowUpload");

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
        #endregion
    }
    //DMSENH6-4796 BE
}