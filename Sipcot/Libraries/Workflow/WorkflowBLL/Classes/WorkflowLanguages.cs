using System;
using System.Data;
using WorkflowBAL;
using DataAccessLayer;

namespace WorkflowBLL.Classes
{
    public class WorkflowLanguages : WorkflowBase
    {

        #region Constructor logic
        public WorkflowLanguages()
        {
            WfLanguagesCreatedBy = 0;
            WfLanguageCode = string.Empty;
            WfLanguageImagePath = string.Empty;
            WfLanguagesOrgId = 0;
            WfLanguagesIsActive = false;
            WfLanguagesIsDeleted = false;
            WfLanguagesName = string.Empty;
            WfLanguagesId = 0;
            WfLanguagesAction = string.Empty;
        }
        #endregion

        #region Properties
        public int WfLanguagesId { get; set; }
        public string WfLanguagesName { get; set; }
        public string WfLanguageCode { get; set; }
        public string WfLanguageImagePath { get; set; }
        public int WfLanguagesOrgId { get; set; }
        public bool WfLanguagesIsActive { get; set; }
        public bool WfLanguagesIsDeleted { get; set; }
        public int WfLanguagesCreatedBy { get; set; }
        public string WfLanguagesAction { get; set; }
        #endregion

        #region Manage Languages
        public DBResult GetAllLanguages(WorkflowLanguages Properties)
        {
            DBResult objDBResult = new DBResult();
            DataSet ds = new DataSet();
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(5);


                dbManager.AddParameters(0, "@in_vLoginToken", Properties.LoginToken);
                dbManager.AddParameters(1, "@in_iLoginOrgId", Properties.LoginOrgId);
                dbManager.AddParameters(2, "@out_iErrorState", 0, ParameterDirection.Output);
                dbManager.AddParameters(3, "@out_iErrorSeverity", 0, ParameterDirection.Output);
                dbManager.AddParameters(4, "@out_vMessage", string.Empty, DbType.String, 250, ParameterDirection.Output);

                ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_Workflow_GetLanguages");

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

            //string storedProcedure = "USP_Workflow_GetLanguages";
            //SqlParameter[] parameters = new SqlParameter[5];
            //parameters[0] = new SqlParameter("@in_vLoginToken", Properties.LoginToken);
            //parameters[1] = new SqlParameter("@in_iLoginOrgId", Properties.LoginOrgId);
            //parameters[2] = new SqlParameter("@out_iErrorState", SqlDbType.Int) { Direction = ParameterDirection.Output };
            //parameters[3] = new SqlParameter("@out_iErrorSeverity", SqlDbType.Int) { Direction = ParameterDirection.Output };
            //parameters[4] = new SqlParameter("@out_vMessage", SqlDbType.NVarChar, 250) { Direction = ParameterDirection.Output };

            //objDBResult = objDBHelper.ExecuteDataset(storedProcedure, parameters);


            return objDBResult;
        }

        public DBResult SaveUserLanguagePreference(WorkflowLanguages Properties)
        {
            DBResult objDBResult = new DBResult();
            DataSet ds = new DataSet();
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(7);

                dbManager.AddParameters(0, "@in_vLoginToken", Properties.LoginToken);
                dbManager.AddParameters(1, "@in_iLoginOrgId", Properties.LoginOrgId);
                dbManager.AddParameters(2, "@in_iLanguageId", Properties.WfLanguagesId);
                dbManager.AddParameters(3, "@in_vAction", Properties.WfLanguagesAction);
                dbManager.AddParameters(4, "@out_iErrorState", 0, ParameterDirection.Output);
                dbManager.AddParameters(5, "@out_iErrorSeverity", 0, ParameterDirection.Output);
                dbManager.AddParameters(6, "@out_vMessage", string.Empty, DbType.String, 250, ParameterDirection.Output);

                ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_Workflow_UserLanguagePreference");

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
           
            //string storedProcedure = "USP_Workflow_UserLanguagePreference";
            //SqlParameter[] parameters = new SqlParameter[7];
            //parameters[0] = new SqlParameter("@in_vLoginToken", Properties.LoginToken);
            //parameters[1] = new SqlParameter("@in_iLoginOrgId", Properties.LoginOrgId);
            //parameters[2] = new SqlParameter("@in_iLanguageId", Properties.WfLanguagesId);
            //parameters[3] = new SqlParameter("@in_vAction", Properties.WfLanguagesAction);
            //parameters[4] = new SqlParameter("@out_iErrorState", SqlDbType.Int) { Direction = ParameterDirection.Output };
            //parameters[5] = new SqlParameter("@out_iErrorSeverity", SqlDbType.Int) { Direction = ParameterDirection.Output };
            //parameters[6] = new SqlParameter("@out_vMessage", SqlDbType.NVarChar, 250) { Direction = ParameterDirection.Output };

            //objDBResult = objDBHelper.ExecuteDataset(storedProcedure, parameters);

            finally
            {
                dbManager.Dispose();
            }
            return objDBResult;
        }
        #endregion

        #region Manage Grid Headers
        public DBResult GetGridHeaders(string vLoginToken, int iLoginOrgId, string vGridType, int iUserLanguageID, int iUserID)
        {
            DBResult objDBResult = new DBResult();
            DataSet ds = new DataSet();
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(8);
                dbManager.AddParameters(0, "@in_vLoginToken", vLoginToken);
                dbManager.AddParameters(1, "@in_iLoginOrgId", iLoginOrgId);
                dbManager.AddParameters(2, "@in_vGridType", vGridType);
                dbManager.AddParameters(3, "@in_iUserLanguageID", iUserLanguageID);
                dbManager.AddParameters(4, "@in_iUserID", iUserID);
                dbManager.AddParameters(5, "@out_iErrorState", 0, ParameterDirection.Output);
                dbManager.AddParameters(6, "@out_iErrorSeverity", 0, ParameterDirection.Output);
                dbManager.AddParameters(7, "@out_vMessage", string.Empty, DbType.String, 250, ParameterDirection.Output);

                ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_Workflow_GetGridHeaders");

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
            
            
            //string storedProcedure = "USP_Workflow_GetGridHeaders";
            //SqlParameter[] parameters = new SqlParameter[8];
            //parameters[0] = new SqlParameter("@in_vLoginToken", vLoginToken);
            //parameters[1] = new SqlParameter("@in_iLoginOrgId", iLoginOrgId);
            //parameters[2] = new SqlParameter("@in_vGridType", vGridType);
            //parameters[3] = new SqlParameter("@in_iUserLanguageID", iUserLanguageID);
            //parameters[4] = new SqlParameter("@in_iUserID", iUserID);

            //parameters[5] = new SqlParameter("@out_iErrorState", SqlDbType.Int) { Direction = ParameterDirection.Output };
            //parameters[6] = new SqlParameter("@out_iErrorSeverity", SqlDbType.Int) { Direction = ParameterDirection.Output };
            //parameters[7] = new SqlParameter("@out_vMessage", SqlDbType.NVarChar, 250) { Direction = ParameterDirection.Output };

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
