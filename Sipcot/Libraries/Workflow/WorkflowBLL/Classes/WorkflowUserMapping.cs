using System;
using System.Data;
using WorkflowBAL;
using DataAccessLayer;

namespace WorkflowBLL.Classes
{
    public class WorkflowUserMapping : WorkflowBase 
    {

        /// <summary>
        /// Global Declataions
        /// </summary>


        public WorkflowUserMapping()
        {
            WfUserMappingId = 0;
            WfUserMappingStageId = 0;
            WfUserMappingWorkflowId = 0;
            WfUserMappingProcessId = 0;
            WfUserMappingCategoryId = 0;
            WfUserMappingXmlData = string.Empty;
            WfUserMappingAction = string.Empty;
        }


        #region DefaultProperties
        public int WfUserMappingId { get; set; }
        public int WfUserMappingUserId { get; set; }
        public int WfUserMappingGroupId { get; set; }
        public int WfUserMappingStageId { get; set; }
        public int WfUserMappingWorkflowId { get; set; }
        public int WfUserMappingProcessId { get; set; }
        public int WfUserMappingCategoryId { get; set; }
        public string WfUserMappingXmlData { get; set; }
        public string WfUserMappingAction { get; set; }
        
        #endregion

        public bool ManageWorkflowUserMapping(WorkflowUserMapping Properties)
        {
            DBResult objDBResult = new DBResult();
            DataSet ds = new DataSet();
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(13);
                dbManager.AddParameters(0, "@in_iUserId", Properties.WfUserMappingUserId);
                dbManager.AddParameters(1, "@in_iUserGroupId", Properties.WfUserMappingGroupId);
                dbManager.AddParameters(2, "@in_xXMLData", Properties.WfUserMappingXmlData);
                dbManager.AddParameters(3, "@in_vAction", Properties.WfUserMappingAction);
                dbManager.AddParameters(4, "@in_vLoginToken", Properties.LoginToken);
                dbManager.AddParameters(5, "@in_iLoginOrgId", Properties.LoginOrgId);
                dbManager.AddParameters(6, "@in_iProcessId", Properties.WfUserMappingProcessId);
                dbManager.AddParameters(7, "@in_iWorkflowId", Properties.WfUserMappingWorkflowId);
                dbManager.AddParameters(8, "@in_iStageId", Properties.WfUserMappingStageId);
                dbManager.AddParameters(9, "@in_iMappingCategoryId", Properties.WfUserMappingCategoryId);
                dbManager.AddParameters(10, "@out_iErrorState", 0, ParameterDirection.Output);
                dbManager.AddParameters(11, "@out_iErrorSeverity", 0, ParameterDirection.Output);
                dbManager.AddParameters(12, "@out_vMessage", string.Empty, DbType.String, 250, ParameterDirection.Output);

                dbManager.ExecuteScalar(CommandType.StoredProcedure, "USP_Workflow_ManageWorkflowUserMapping");
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }
            
            //string StroredProcedure = "USP_Workflow_ManageWorkflowUserMapping";
            //SqlParameter []Parameters = new SqlParameter[13];
            //Parameters[0] = new SqlParameter { ParameterName="@in_iUserId",Value=Properties.WfUserMappingUserId};
            //Parameters[1] = new SqlParameter { ParameterName = "@in_iUserGroupId", Value = Properties.WfUserMappingGroupId};
            //Parameters[2] = new SqlParameter { ParameterName = "@in_xXMLData", Value = Properties.WfUserMappingXmlData};
            //Parameters[3] = new SqlParameter { ParameterName = "@in_vAction", Value = Properties.WfUserMappingAction };
            //Parameters[4] = new SqlParameter { ParameterName = "@in_vLoginToken", Value = Properties.LoginToken };
            //Parameters[5] = new SqlParameter { ParameterName = "@in_iLoginOrgId", Value = Properties.LoginOrgId };
            //Parameters[6] = new SqlParameter { ParameterName = "@in_iProcessId", Value = Properties.WfUserMappingProcessId };
            //Parameters[7] = new SqlParameter { ParameterName = "@in_iWorkflowId", Value = Properties.WfUserMappingWorkflowId };
            //Parameters[8] = new SqlParameter { ParameterName = "@in_iStageId", Value = Properties.WfUserMappingStageId };
            //Parameters[9] = new SqlParameter { ParameterName = "@in_iMappingCategoryId", Value = Properties.WfUserMappingCategoryId };
            //Parameters[10] = new SqlParameter { ParameterName = "@out_iErrorState", SqlDbType=SqlDbType.Int,Direction=ParameterDirection.Output };
            //Parameters[11] = new SqlParameter { ParameterName = "@out_iErrorSeverity",SqlDbType=SqlDbType.Int,Direction=ParameterDirection.Output };
            //Parameters[12] = new SqlParameter { ParameterName = "@out_vMessage", SqlDbType=SqlDbType.NVarChar,Direction=ParameterDirection.Output,Size=250};

            //Status = objDBHelper.ExecuteStoredProcedure(StroredProcedure, Parameters);
            return true;

        }


        public DBResult GetAllWorkflowUserMapping(WorkflowUserMapping Properties)
        {
            DBResult objDBResult = new DBResult();
            DataSet ds = new DataSet();
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(13);
                dbManager.AddParameters(0, "@in_iUserId", Properties.WfUserMappingUserId);
                dbManager.AddParameters(1, "@in_iUserGroupId", Properties.WfUserMappingGroupId);
                dbManager.AddParameters(2, "@in_xXMLData", Properties.WfUserMappingXmlData);
                dbManager.AddParameters(3, "@in_vAction", Properties.WfUserMappingAction);
                dbManager.AddParameters(4, "@in_vLoginToken", Properties.LoginToken);
                dbManager.AddParameters(5, "@in_iLoginOrgId", Properties.LoginOrgId);
                dbManager.AddParameters(6, "@in_iProcessId", Properties.WfUserMappingProcessId);
                dbManager.AddParameters(7, "@in_iWorkflowId", Properties.WfUserMappingWorkflowId);
                dbManager.AddParameters(8, "@in_iStageId", Properties.WfUserMappingStageId);
                dbManager.AddParameters(9, "@in_iMappingCategoryId", Properties.WfUserMappingCategoryId);
                dbManager.AddParameters(10, "@out_iErrorState", 0, ParameterDirection.Output);
                dbManager.AddParameters(11, "@out_iErrorSeverity", 0, ParameterDirection.Output);
                dbManager.AddParameters(12, "@out_vMessage", string.Empty, DbType.String, 250, ParameterDirection.Output);

                ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_Workflow_ManageWorkflowUserMapping");

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
            
            //DBResult objDBResult = new DBResult();
            //string StroredProcedure = "USP_Workflow_ManageWorkflowUserMapping";
            //SqlParameter[] Parameters = new SqlParameter[13];
            //Parameters[0] = new SqlParameter { ParameterName = "@in_iUserId", Value = Properties.WfUserMappingUserId };
            //Parameters[1] = new SqlParameter { ParameterName = "@in_iUserGroupId", Value = Properties.WfUserMappingGroupId };
            //Parameters[2] = new SqlParameter { ParameterName = "@in_xXMLData", Value = Properties.WfUserMappingXmlData };
            //Parameters[3] = new SqlParameter { ParameterName = "@in_vAction", Value = Properties.WfUserMappingAction };
            //Parameters[4] = new SqlParameter { ParameterName = "@in_vLoginToken", Value = Properties.LoginToken };
            //Parameters[5] = new SqlParameter { ParameterName = "@in_iLoginOrgId", Value = Properties.LoginOrgId };
            //Parameters[6] = new SqlParameter { ParameterName = "@in_iProcessId", Value = Properties.WfUserMappingProcessId };
            //Parameters[7] = new SqlParameter { ParameterName = "@in_iWorkflowId", Value = Properties.WfUserMappingWorkflowId };
            //Parameters[8] = new SqlParameter { ParameterName = "@in_iStageId", Value = Properties.WfUserMappingStageId };
            //Parameters[9] = new SqlParameter { ParameterName = "@in_iMappingCategoryId", Value = Properties.WfUserMappingCategoryId };
            //Parameters[10] = new SqlParameter { ParameterName = "@out_iErrorState", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };
            //Parameters[11] = new SqlParameter { ParameterName = "@out_iErrorSeverity", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };
            //Parameters[12] = new SqlParameter { ParameterName = "@out_vMessage", SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Output, Size = 250 };

            //objDBResult = objDBHelper.ExecuteDataset(StroredProcedure, Parameters);
            
            return objDBResult;

        }
     }
}
