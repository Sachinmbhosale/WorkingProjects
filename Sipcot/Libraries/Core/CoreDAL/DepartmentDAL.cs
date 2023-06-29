using System;
using System.Collections.Generic;
using System.Data;
using Lotex.EnterpriseSolutions.CoreBE;
using DataAccessLayer;

namespace Lotex.EnterpriseSolutions.CoreDAL
{
    public class DepartmentDAL : BaseDAL
    {
        public Results ManageDepartment(Department Entityobj, string action, string loginToken, int loginOrgId)
        {
            Results results = new Results();
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(11);

                dbManager.AddParameters(0, "@in_iOrganizationId", loginOrgId);
                dbManager.AddParameters(1, "@in_iDepartmentId", Entityobj.DepartmentId);
                dbManager.AddParameters(2, "@in_vDepartment", Entityobj.DepartmentName);
                dbManager.AddParameters(3, "@in_vDescription", Entityobj.Description);
                dbManager.AddParameters(4, "@in_vDepartmentHead", Entityobj.Head);
                dbManager.AddParameters(5, "@in_vAction", action);
                dbManager.AddParameters(6, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(7, "@in_iLoginOrgId", loginOrgId);

                dbManager.AddParameters(8, "@out_iErrorState", 0, ParameterDirection.Output);
                dbManager.AddParameters(9, "@out_iErrorSeverity", 0, ParameterDirection.Output);
                dbManager.AddParameters(10, "@out_vMessage", string.Empty, DbType.String, 250, ParameterDirection.Output);

                results.ResultDS = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_ManageDepartments");
                results.ErrorState = Convert.ToInt32(dbManager.GetOutputParameterValue("@out_iErrorState"));
                results.ErrorSeverity = Convert.ToInt32(dbManager.GetOutputParameterValue("@out_iErrorSeverity"));
                results.Message = Convert.ToString(dbManager.GetOutputParameterValue("@out_vMessage"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }
            return results;
        }

        public Results SearchDepartments(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            Results results = new Results();
            results.ActionStatus = "SUCCESS";

            DateTime? FromDate = null;
            FromDate = filter.FromDate.Length > 1 ? FormatScriptDateToSystemDate(filter.FromDate) : FromDate;

            DateTime? Todate = null;
            Todate = filter.ToDate.Length > 0 ? FormatScriptDateToSystemDate(filter.ToDate) : Todate;

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);

            try
            {
                dbManager.Open();
                dbManager.CreateParameters(8);
                dbManager.AddParameters(0, "@in_iCurrDepartmentId", filter.DepartmentID);
                dbManager.AddParameters(1, "@docType_iId", filter.DocumentTypeID);
                dbManager.AddParameters(2, "@in_vDepartmentName", filter.DepartmentName);
                dbManager.AddParameters(3, "@in_vCreaedDateFrom", FromDate);
                dbManager.AddParameters(4, "@in_vCreaedDateTo", Todate);
                dbManager.AddParameters(5, "@in_vAction", action);
                dbManager.AddParameters(6, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(7, "@in_iLoginOrgId", loginOrgId);

                using (IDataReader dbRdr = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_SearchDepartmentById"))
                {
                    if (action == "SearchDepartmentsForDeptPage" || action == "DeleteDepartmentAndSearch")
                    {
                        while (dbRdr.Read())
                        {
                            results.ActionStatus = dbRdr["ActionStatus"].ToString();
                            if (results.ActionStatus == ActionStatus.SUCCESS.ToString())
                            {
                                if (results.Departments == null)
                                {
                                    results.Departments = new List<Department>();
                                }
                                Department dept = new Department();
                                dept.DepartmentId = Convert.ToInt32(dbRdr["DepartmentID"].ToString());
                                dept.DepartmentName = dbRdr["DepartmentName"].ToString();
                                dept.Description = dbRdr["DepartmentDesc"].ToString();
                                dept.Head = dbRdr["DepartmentHead"].ToString();
                                dept.CreatedDate = Convert.ToDateTime(dbRdr["CreatedDate"].ToString()).ToShortDateString();
                                dept.Active = Convert.ToBoolean(dbRdr["Active"].ToString());
                                results.Departments.Add(dept);
                            }
                        }
                    }
                    else
                    {
                        while (dbRdr.Read())
                        {
                            results.ActionStatus = dbRdr["ActionStatus"].ToString();
                            if (results.ActionStatus == ActionStatus.SUCCESS.ToString())
                            {
                                if (results.Departments == null)
                                {
                                    results.Departments = new List<Department>();
                                }
                                Department dept = new Department();
                                dept.DepartmentId = Convert.ToInt32(dbRdr["DepartmentID"].ToString());
                                dept.DepartmentName = dbRdr["DepartmentName"].ToString();
                                dept.CreatedDate = Convert.ToDateTime(dbRdr["CreatedDate"].ToString()).ToShortDateString();
                                results.Departments.Add(dept);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }
            return results;


            /* SqlDataReader dbRdr = null;
             SqlCommand dbCmd = null;
             SqlConnection dbCon = null;
             Results results = new Results();
             results.ActionStatus = "SUCCESS";
             try
             {
                 dbCon = OpenConnection();
                 dbCmd = new SqlCommand();
                 dbCmd.Connection = dbCon;
                 dbCmd.CommandType = CommandType.StoredProcedure;

                 dbCmd.CommandText = "USP_SearchDepartmentById";
                 if (filter.DepartmentID > 0)
                 {
                     dbCmd.Parameters.Add("@in_iCurrDepartmentId", SqlDbType.Int).Value = filter.DepartmentID;
                 }
                 if (filter.DocumentTypeID > 0)
                 {
                     dbCmd.Parameters.Add("@docType_iId", SqlDbType.Int).Value = filter.DocumentTypeID;
                 }
                 if (filter.DepartmentName != string.Empty)
                 {
                     dbCmd.Parameters.Add("@in_vDepartmentName", SqlDbType.VarChar, 300).Value = filter.DepartmentName;
                 }
                 if (filter.StartDate != string.Empty)
                 {
                     dbCmd.Parameters.Add("@in_vCreaedDateFrom", SqlDbType.DateTime).Value = FormatScriptDateToSystemDate(filter.StartDate);
                 }
                 if (filter.EndDate != string.Empty)
                 {
                     dbCmd.Parameters.Add("@in_vCreaedDateTo", SqlDbType.DateTime).Value = FormatScriptDateToSystemDate(filter.EndDate);
                 }
                 dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar, 30).Value = action;
                 //sesstion
                 dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                 dbCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;
                 dbRdr = dbCmd.ExecuteReader();

                 if (action == "SearchDepartmentsForDeptPage" || action == "DeleteDepartmentAndSearch")
                 {
                     while (dbRdr.Read())
                     {
                         results.ActionStatus = dbRdr["ActionStatus"].ToString();
                         if (results.ActionStatus == ActionStatus.SUCCESS.ToString())
                         {
                             if (results.Departments == null)
                             {
                                 results.Departments = new List<Department>();
                             }
                             Department dept = new Department();
                             dept.DepartmentId = Convert.ToInt32(dbRdr["DepartmentID"].ToString());
                             dept.DepartmentName = dbRdr["DepartmentName"].ToString();
                             dept.Description = dbRdr["DepartmentDesc"].ToString();
                             dept.Head = dbRdr["DepartmentHead"].ToString();
                             dept.CreatedDate = Convert.ToDateTime(dbRdr["CreatedDate"].ToString()).ToShortDateString();
                             dept.Active = Convert.ToBoolean(dbRdr["Active"].ToString());
                             results.Departments.Add(dept);
                         }
                     }
                 }
                 else
                 {
                     while (dbRdr.Read())
                     {
                         results.ActionStatus = dbRdr["ActionStatus"].ToString();
                         if (results.ActionStatus == ActionStatus.SUCCESS.ToString())
                         {
                             if (results.Departments == null)
                             {
                                 results.Departments = new List<Department>();
                             }
                             Department dept = new Department();
                             dept.DepartmentId = Convert.ToInt32(dbRdr["DepartmentID"].ToString());
                             dept.DepartmentName = dbRdr["DepartmentName"].ToString();
                             dept.CreatedDate = Convert.ToDateTime(dbRdr["CreatedDate"].ToString()).ToShortDateString();
                             results.Departments.Add(dept);
                         }
                     }
                 }
             }
             catch (Exception ex)
             {
                 results.Message = ex.ToString();
                 throw ex;
             }
             finally
             {
                 if (dbRdr != null)
                 {
                     dbRdr.Dispose();
                 }
                 if (dbCmd != null)
                 {
                     dbCmd.Dispose();
                 }
                 CloseConnection(dbCon);
             }
             return results;*/
        }
    }
}
