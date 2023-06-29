using System;
using WorkflowBAL;
using System.Data;
using DataAccessLayer;

namespace WorkflowBLL.Classes
{
    public class WorkflowMasterValues:WorkflowBase
    {
      

        #region Properties
       public int WfMasterValueId { get; set; }
       public string WfMasterValueName { get; set; }
       public string WfMasterValueDescription { get; set; }
       public int WfMasterTypeId { get; set; }
       public int WfMasterParentId { get; set; }
       public bool WfMasterValueIsActive { get; set; }
       public int WfMasterValueCreatedBy { get; set; }
       
        #endregion
        #region Constructor logic
         public WorkflowMasterValues()
       {
           WfMasterValueId = 0;
           WfMasterValueName = string.Empty;
           WfMasterValueDescription = string.Empty;
           WfMasterTypeId = 0;
           WfMasterParentId = 0;
           WfMasterValueIsActive = false;
           WfMasterValueCreatedBy = 0;
       }
        #endregion

        #region Methos to manage Mastervalues
         public DBResult ManageMasterValues(WorkflowMasterValues Properties ,string Action)
         {
             DBResult objDBResult = new DBResult();
             DataSet ds = new DataSet();
             IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
             try
             {
                 dbManager.Open();
                 dbManager.CreateParameters(12);

                 dbManager.AddParameters(0, "@in_iId", Properties.WfMasterValueId);
                 dbManager.AddParameters(1, "@in_vValueName", Properties.WfMasterValueName);
                 dbManager.AddParameters(2, "@in_vValueDescription", Properties.WfMasterValueDescription);
                 dbManager.AddParameters(3, "@in_bIsActive", Properties.WfMasterValueIsActive);
                 dbManager.AddParameters(4, "@in_iTypeId", Properties.WfMasterTypeId);
                 dbManager.AddParameters(5, "@in_iParentId", Properties.WfMasterParentId);
                 dbManager.AddParameters(6, "@in_vAction", Action);
                 dbManager.AddParameters(7, "@in_vLoginToken", Properties.LoginToken);
                 dbManager.AddParameters(8, "@in_iLoginOrgId", Properties.LoginOrgId);
                 dbManager.AddParameters(9, "@out_iErrorState", 0, ParameterDirection.Output);
                 dbManager.AddParameters(10, "@out_iErrorSeverity", 0, ParameterDirection.Output);
                 dbManager.AddParameters(11, "@out_vMessage", string.Empty, DbType.String, 250, ParameterDirection.Output);

                 ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_Workflow_ManageMasterValues");

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
             


             //string StoredProcedure = "USP_Workflow_ManageMasterValues";
             //SqlParameter [] Parameters = new SqlParameter[12];
             //Parameters[0] = new SqlParameter { ParameterName = "@in_iId", Value = Properties.WfMasterValueId };
             //Parameters[1] = new SqlParameter { ParameterName = "@in_vValueName", Value = Properties.WfMasterValueName, Direction = ParameterDirection.Input };
             //Parameters[2] = new SqlParameter { ParameterName = "@in_vValueDescription", Value = Properties.WfMasterValueDescription, Direction = ParameterDirection.Input };
             //Parameters[3] = new SqlParameter { ParameterName = "@in_bIsActive", Value = Properties.WfMasterValueIsActive, Direction = ParameterDirection.Input };
             //Parameters[4] = new SqlParameter { ParameterName = "@in_iTypeId", Value = Properties.WfMasterTypeId, Direction = ParameterDirection.Input };
             //Parameters[5] = new SqlParameter { ParameterName = "@in_iParentId", Value = Properties.WfMasterParentId, Direction = ParameterDirection.Input };
             //Parameters[6] = new SqlParameter { ParameterName = "@in_vAction", Value = Action };
             //Parameters[7] = new SqlParameter { ParameterName = "@in_vLoginToken", Value = Properties.LoginToken, Direction = ParameterDirection.Input };
             //Parameters[8] = new SqlParameter { ParameterName = "@in_iLoginOrgId", Value = Properties.LoginOrgId, Direction = ParameterDirection.Input };
             //Parameters[9] = new SqlParameter { ParameterName = "@out_iErrorState", Direction = ParameterDirection.Output, SqlDbType = SqlDbType.Int };
             //Parameters[10] = new SqlParameter { ParameterName = "@out_iErrorSeverity", Direction = ParameterDirection.Output, SqlDbType = SqlDbType.Int };
             //Parameters[11] = new SqlParameter { ParameterName = "@out_vMessage", Direction = ParameterDirection.Output, SqlDbType = SqlDbType.NVarChar, Size = 250 };
             //Objresult = objDBHelper.ExecuteDataset(StoredProcedure, Parameters);
             finally
             {
                 dbManager.Dispose();
             }

             return objDBResult;

         }
        #endregion
    }
}
