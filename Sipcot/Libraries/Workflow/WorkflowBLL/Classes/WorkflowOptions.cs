using System;
using System.Data;
using WorkflowBAL;
using DataAccessLayer;

namespace WorkflowBLL.Classes
{
    public class WorkflowOptions : WorkflowBase
    {     

        #region ConstructorLogic
        public WorkflowOptions()
        {
            WorkflowOptions_OutOfOffice = 0;
            WorkflowOptions_Action = string.Empty;
            WorkflowOptions_OutOfOfficeStartDate = string.Empty;
            WorkflowOptions_OutOfOfficeEndDate = string.Empty;
        }
        #endregion

        #region
        public int WorkflowOptions_OutOfOffice { get; set; }
        public string WorkflowOptions_Action { get; set; }
        public string WorkflowOptions_OutOfOfficeStartDate { get; set; }
        public string WorkflowOptions_OutOfOfficeEndDate { get; set; }
        #endregion

        public DBResult ManageWorkflowOptions(WorkflowOptions prop)
        {
            DBResult objDBResult = new DBResult();
            DataSet ds = new DataSet();
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(9);
                dbManager.AddParameters(0, "@in_iOutOfOffice", prop.WorkflowOptions_OutOfOffice);
                dbManager.AddParameters(1, "@in_vAction", prop.WorkflowOptions_Action);
                dbManager.AddParameters(2, "@in_vLoginToken", prop.LoginToken);
                dbManager.AddParameters(3, "@in_iLoginOrgId", prop.LoginOrgId);
                dbManager.AddParameters(4, "@in_vOutOfOfficeStartDate", prop.WorkflowOptions_OutOfOfficeStartDate);
                dbManager.AddParameters(5, "@in_vOutOfOfficeEndDate", prop.WorkflowOptions_OutOfOfficeEndDate);
                dbManager.AddParameters(6, "@out_iErrorState", 0, ParameterDirection.Output);
                dbManager.AddParameters(7, "@out_iErrorSeverity", 0, ParameterDirection.Output);
                dbManager.AddParameters(8, "@out_vMessage", string.Empty, DbType.String, 250, ParameterDirection.Output);

                ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_Workflow_WorkflowOptions");

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
            

            //string StoredProcedure = "USP_Workflow_WorkflowOptions";
            //SqlParameter[] Parameters = new SqlParameter[9];
            //Parameters[0] = new SqlParameter { ParameterName = "@in_iOutOfOffice", Value = prop.WorkflowOptions_OutOfOffice };
            //Parameters[1] = new SqlParameter { ParameterName = "@in_vAction", Value = prop.WorkflowOptions_Action };
            //Parameters[2] = new SqlParameter { ParameterName = "@in_vLoginToken", Value = prop.LoginToken};
            //Parameters[3] = new SqlParameter { ParameterName = "@in_iLoginOrgId", Value = prop.LoginOrgId};
            //Parameters[4] = new SqlParameter { ParameterName = "@in_vOutOfOfficeStartDate", Value = prop.WorkflowOptions_OutOfOfficeStartDate };
            //Parameters[5] = new SqlParameter { ParameterName = "@in_vOutOfOfficeEndDate", Value = prop.WorkflowOptions_OutOfOfficeEndDate };
            //Parameters[6] = new SqlParameter { ParameterName = "@out_iErrorState", Direction = ParameterDirection.Output, SqlDbType = SqlDbType.Int };
            //Parameters[7] = new SqlParameter { ParameterName = "@out_iErrorSeverity", Direction = ParameterDirection.Output, SqlDbType = SqlDbType.Int };
            //Parameters[8] = new SqlParameter { ParameterName = "@out_vMessage", Direction = ParameterDirection.Output, SqlDbType = SqlDbType.NVarChar, Size = 250 };
            //objDBResult = objDBHelper.ExecuteDataset(StoredProcedure, Parameters);
            finally
            {
                dbManager.Dispose();
            }
            return objDBResult;

        }
    }


}
