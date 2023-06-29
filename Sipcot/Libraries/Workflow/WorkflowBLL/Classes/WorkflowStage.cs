/* ============================================================================  
Author     : 
Create date: 
===============================================================================  
** Change History   
** Date:          Author:            Issue ID                         Description:  
** ----------   -------------       ----------                  ----------------------------
** 03 Dec 2013          Mandatory   (UMF)                Set Mandatory for index fields
 *
 * Modified
 * 28/02/2015   Sabina             DMS5-3498                    added fields in stage class
 * 03/20/2015   Sabina             DMS5-3579                    Bringing a new checkbox to denote form type dataentry requires with image or without image
=============================================================================== */
using System;
using System.Data;
using WorkflowBAL;
using DataAccessLayer;

namespace WorkflowBLL.Classes
{
    public class WorkflowStage : WorkflowBase
    {


        #region ConstructorLogic
        public WorkflowStage()
        {
            StageId = 0;
            WorkflowId = 0;
            StageName = string.Empty;
            DisplayName = string.Empty;
            Description = string.Empty;
            OrderNo = 0;
            ProcessId = 0;
            Active = false;
            Deleted = false;
            TatDuration = 0;
            //DMS5-3498A
            TemplatePath = string.Empty;
            DataEntryId = 0;
            TotalPages = 0;
            CommaSeparatedStageIds = string.Empty;
            bShowBackgroundImage = true;// DMS5-3579A
        }

        #endregion

        #region Default Properties
        public int StageId { get; set; }
        public int WorkflowId { get; set; }
        public string StageName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public int OrderNo { get; set; }
        public int ProcessId { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public int TatDuration { get; set; }
        //DMS5-3498A
        public string TemplatePath { get; set; }
        public int DataEntryId { get; set; }
        public int TotalPages { get; set; }
        public string CommaSeparatedStageIds { get; set; }
        public bool bShowBackgroundImage { get; set; }// DMS5-3579A
        #endregion

        /// <summary>
        /// SaveWorkFlowSatges
        /// </summary>
        public DBResult ManageWorkflowStages(WorkflowStage Prop, string Action)
        {
            DBResult objDBResult = new DBResult();
            DataSet ds = new DataSet();
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(20);
                dbManager.AddParameters(0, "@in_iStageId", Prop.StageId);
                dbManager.AddParameters(1, "@in_vCommaSeparatedStageIds", Prop.CommaSeparatedStageIds);
                dbManager.AddParameters(2, "@in_vStageName", Prop.StageName);
                dbManager.AddParameters(3, "@in_vDisplayName", Prop.DisplayName);
                dbManager.AddParameters(4, "@in_vDescription", Prop.Description);
                dbManager.AddParameters(5, "@in_iOrderNo", Prop.OrderNo);
                dbManager.AddParameters(6, "@in_iWorkflowId", Prop.WorkflowId);
                dbManager.AddParameters(7, "@in_iProcessId", Prop.ProcessId);
                dbManager.AddParameters(8, "@in_bIsActive", Prop.Active);
                dbManager.AddParameters(9, "@in_vAction", Action);
                dbManager.AddParameters(10, "@in_vLoginToken", Prop.LoginToken);
                dbManager.AddParameters(11, "@in_iLoginOrgId", Prop.LoginOrgId);
                dbManager.AddParameters(12, "@in_vTemplatePath", Prop.TemplatePath);
                dbManager.AddParameters(13, "@in_iDataEntryId", Prop.DataEntryId);
                dbManager.AddParameters(14, "@in_iTotalPages", Prop.TotalPages);
                dbManager.AddParameters(15, "@out_iErrorState", 0, ParameterDirection.Output);
                dbManager.AddParameters(16, "@out_iErrorSeverity", 0, ParameterDirection.Output);
                dbManager.AddParameters(17, "@out_vMessage", string.Empty, DbType.String, 250, ParameterDirection.Output);
                dbManager.AddParameters(18, "@in_iTATDuration", Prop.TatDuration);
                dbManager.AddParameters(19, "@in_bShowBackgroundImage", Prop.bShowBackgroundImage);// DMS5-3579A

                ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_Workflow_ManageWorkflowStages");

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
            //string storedProcedure = "USP_Workflow_ManageWorkflowStages";
            //SqlParameter[] parameters = new SqlParameter[16];
            //parameters[0] = new SqlParameter("@in_iStageId", Prop.StageId);
            //parameters[1] = new SqlParameter("@in_vCommaSeparatedStageIds", Prop.CommaSeparatedStageIds);
            //parameters[2] = new SqlParameter("@in_vStageName", Prop.StageName);
            //parameters[3] = new SqlParameter("@in_vDisplayName", Prop.DisplayName);
            //parameters[4] = new SqlParameter("@in_vDescription", Prop.Description);
            //parameters[5] = new SqlParameter("@in_iOrderNo", Prop.OrderNo);
            //parameters[6] = new SqlParameter("@in_iWorkflowId", Prop.WorkflowId);
            //parameters[7] = new SqlParameter("@in_iProcessId", Prop.ProcessId);
            //parameters[8] = new SqlParameter("@in_bIsActive", Prop.Active);
            //parameters[9] = new SqlParameter("@in_vAction", Action);
            //parameters[10] = new SqlParameter ( "@in_vLoginToken", Prop.LoginToken );
            //parameters[11] = new SqlParameter ("@in_iLoginOrgId", Prop.LoginOrgId );
            //parameters[12] = new SqlParameter("@out_iErrorState", System.Data.SqlDbType.Int) { Direction = ParameterDirection.Output };
            //parameters[13] = new SqlParameter("@out_iErrorSeverity", System.Data.SqlDbType.Int) { Direction = ParameterDirection.Output };
            //parameters[14] = new SqlParameter("@out_vMessage", System.Data.SqlDbType.NVarChar, 250) { Direction = ParameterDirection.Output };
            //parameters[15] = new SqlParameter("@in_iTATDuration", Prop.TatDuration);

            //ObjDbResult = objDBHelper.ExecuteDataset(storedProcedure, parameters);
            return objDBResult;
        }

    }
}
