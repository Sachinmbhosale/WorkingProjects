using System;
using WorkflowBAL;
using System.Data;
using DataAccessLayer;

namespace WorkflowBLL.Classes
{


    public class WorkflowMasterTypes : WorkflowBase
    {
   
        #region Properties
        public int WfTypeId { get; set; }
        public string WfTypeName { get; set; }
        public string WfTypeDescription { get; set; }
        public int WfTypeOrgId { get; set; }
        public bool WfTypeIsActive { get; set; }
        public int WfTypeCreatedBy { get; set; }
        public int WfParentTypeId { get; set; }

        #endregion

        #region Constructor logic
        public WorkflowMasterTypes()
        {
            WfTypeCreatedBy = 0;
            WfTypeDescription = string.Empty;
            WfTypeOrgId = 0;
            WfTypeIsActive = false;
            WfTypeName = string.Empty;
            WfTypeId = 0;
            WfParentTypeId = 0;

        }
        #endregion

        #region Manage Master Types
        public DBResult ManageMasterType(WorkflowMasterTypes Properties, string Actions)
        {
            DBResult objDBResult = new DBResult();
            DataSet ds = new DataSet();
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(11);

                dbManager.AddParameters(0, "@in_iId", Properties.WfTypeId);
                dbManager.AddParameters(1, "@in_vMasterName", Properties.WfTypeName);
                dbManager.AddParameters(2, "@in_vMasterDescription", Properties.WfTypeDescription);
                dbManager.AddParameters(3, "@in_bIsActive", Properties.WfTypeIsActive);
                dbManager.AddParameters(4, "@in_vAction", Actions);
                dbManager.AddParameters(5, "@in_vLoginToken", Properties.LoginToken);
                dbManager.AddParameters(6, "@in_iLoginOrgId", Properties.LoginOrgId);
                dbManager.AddParameters(7, "@out_iErrorState", 0, ParameterDirection.Output);
                dbManager.AddParameters(8, "@out_iErrorSeverity", 0, ParameterDirection.Output);
                dbManager.AddParameters(9, "@out_vMessage", string.Empty, DbType.String, 250, ParameterDirection.Output);
                dbManager.AddParameters(10, "@in_iParentTypeId", Properties.WfParentTypeId);

                ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_Workflow_ManageMasterTypes");

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
            


            //string storedProcedure = "USP_Workflow_ManageMasterTypes";
            //SqlParameter[] parameters = new SqlParameter[11];
            //parameters[0] = new SqlParameter("@in_iId", Properties.WfTypeId);
            //parameters[1] = new SqlParameter("@in_vMasterName", Properties.WfTypeName);
            //parameters[2] = new SqlParameter("@in_vMasterDescription", Properties.WfTypeDescription);
            //parameters[3] = new SqlParameter("@in_bIsActive", Properties.WfTypeIsActive);
            //parameters[4] = new SqlParameter("@in_vAction", Actions);
            //parameters[5] = new SqlParameter("@in_vLoginToken", Properties.LoginToken);
            //parameters[6] = new SqlParameter("@in_iLoginOrgId", Properties.LoginOrgId);
            //parameters[7] = new SqlParameter("@out_iErrorState", SqlDbType.Int) { Direction = ParameterDirection.Output };
            //parameters[8] = new SqlParameter("@out_iErrorSeverity", SqlDbType.Int) { Direction = ParameterDirection.Output };
            //parameters[9] = new SqlParameter("@out_vMessage", SqlDbType.NVarChar, 250) { Direction = ParameterDirection.Output };
            //parameters[10] = new SqlParameter("@in_iParentTypeId", Properties.WfParentTypeId);

            //objDBResult = objDBHelper.ExecuteDataset(storedProcedure, parameters);
            finally
            {
                dbManager.Dispose();
            }
            return objDBResult;
        }


        #endregion
    }
}
