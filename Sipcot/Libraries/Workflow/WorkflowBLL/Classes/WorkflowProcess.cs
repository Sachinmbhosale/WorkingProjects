using System;
using System.Data;
using WorkflowBAL;
using DataAccessLayer;

namespace WorkflowBLL.Classes
{
    public class WorkflowProcess : WorkflowBase
    {

        #region Constructor logic
        public WorkflowProcess()
        {
            WfProcessCreatedBy = 0;
            WfProcessDescription = string.Empty;
            WfProcessOrgId = 0;
            WfProcessIsActive = false;
            WfProcessIsDeleted = false;
            WfProcessName = string.Empty;
            WfProcessId = 0;

        }
        #endregion

        #region Properties
        public int WfProcessId { get; set; }
        public string WfProcessName { get; set; }
        public string WfProcessDescription { get; set; }
        public int WfProcessOrgId { get; set; }
        public bool WfProcessIsActive { get; set; }
        public bool WfProcessIsDeleted { get; set; }
        public int WfProcessCreatedBy { get; set; }
        #endregion

        #region Manage Process
        public DBResult ManageProcess(WorkflowProcess Properties, string Actions)
        {
            DBResult objDBResult = new DBResult();
            DataSet ds = new DataSet();
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(12);
                dbManager.AddParameters(0, "@in_iCurrProcessId", Properties.WfProcessId);
                dbManager.AddParameters(1, "@in_vProcessName", Properties.WfProcessName);
                dbManager.AddParameters(2, "@in_vDescription", Properties.WfProcessDescription);
                dbManager.AddParameters(3, "@in_bProcessIsActive", Properties.WfProcessIsActive);
                dbManager.AddParameters(4, "@in_bIsDeleted", Properties.WfProcessIsDeleted);
                dbManager.AddParameters(5, "@in_vAction", Actions);
                dbManager.AddParameters(6, "@in_vLoginToken", Properties.LoginToken);
                dbManager.AddParameters(7, "@in_iLoginOrgId", Properties.LoginOrgId);
                dbManager.AddParameters(8, "@out_iErrorState", 0, ParameterDirection.Output);
                dbManager.AddParameters(9, "@out_iErrorSeverity", 0, ParameterDirection.Output);
                dbManager.AddParameters(10, "@out_vMessage", string.Empty, DbType.String, 250, ParameterDirection.Output);
                dbManager.AddParameters(11, "@in_iProcessOrgId", Properties.WfProcessOrgId);

                ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_Workflow_ManageProcess");

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

            //string storedProcedure = "USP_Workflow_ManageProcess";
            //SqlParameter[] parameters = new SqlParameter[12];
            //parameters[0] = new SqlParameter("@in_iCurrProcessId", Properties.WfProcessId);
            //parameters[1] = new SqlParameter("@in_vProcessName", Properties.WfProcessName);
            //parameters[2] = new SqlParameter("@in_vDescription", Properties.WfProcessDescription);
            //parameters[3] = new SqlParameter("@in_bProcessIsActive", Properties.WfProcessIsActive);
            //parameters[4] = new SqlParameter("@in_bIsDeleted", Properties.WfProcessIsDeleted);
            //parameters[5] = new SqlParameter("@in_vAction", Actions);
            //parameters[6] = new SqlParameter("@in_vLoginToken", Properties.LoginToken);
            //parameters[7] = new SqlParameter("@in_iLoginOrgId", Properties.LoginOrgId);
            //parameters[8] = new SqlParameter("@out_iErrorState", SqlDbType.Int) { Direction = ParameterDirection.Output };
            //parameters[9] = new SqlParameter("@out_iErrorSeverity", SqlDbType.Int) { Direction = ParameterDirection.Output };
            //parameters[10] = new SqlParameter("@out_vMessage", SqlDbType.NVarChar, 250) { Direction = ParameterDirection.Output };
            //parameters[11] = new SqlParameter("@in_iProcessOrgId", Properties.WfProcessOrgId);

            //objDBResult = objDBHelper.ExecuteDataset(storedProcedure, parameters);
            return objDBResult;
        }
        #endregion

    }
}
