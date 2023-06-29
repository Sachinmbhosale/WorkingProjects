
/* ============================================================================  
Author     : Joby
Create date: 
Description: 
===============================================================================  
** Change History   
** Date:          Author:            Issue ID                   Description:  
** ----------   -------------       ----------                  ----------------------------
16 06 2015     Gokuldas.Palapatta   DMSENH6-4655      Audit Trails & Logs

=============================================================================== */

using System;
using System.Collections.Generic;
using System.Data;
using Lotex.EnterpriseSolutions.CoreBE;
using DataAccessLayer;

namespace Lotex.EnterpriseSolutions.CoreDAL
{
    public class UserDAL : BaseDAL
    {
        public UserDAL() { }
        public Results SearchUsers(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            Results results = new Results();
            results.ActionStatus = "SUCCESS";

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);

            DateTime? FromDate = null;
            FromDate = filter.FromDate.Length > 0 ? FormatScriptDateToSystemDate(filter.FromDate) : FromDate;
            DateTime? ToDate = null;
            ToDate = filter.ToDate.Length > 0 ? FormatScriptDateToSystemDate(filter.ToDate) : ToDate;
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(11);
                dbManager.AddParameters(0, "@in_iCurrUserId", filter.CurrUserId);
                dbManager.AddParameters(1, "@in_iCurrOrgId", filter.CurrOrgId);
                dbManager.AddParameters(2, "@in_vUserName", filter.UserName);
                dbManager.AddParameters(3, "@in_vUserEmailId", filter.UserEmailId);
                dbManager.AddParameters(4, "@in_vFirstName", filter.FirstName);
                dbManager.AddParameters(5, "@in_vLastName", filter.LastName);
                dbManager.AddParameters(6, "@in_vCreatedDateFrom", FromDate);
                dbManager.AddParameters(7, "@in_vCreatedDateTo", ToDate);
                dbManager.AddParameters(8, "@in_vAction", action);
                dbManager.AddParameters(9, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(10, "@in_iLoginOrgId", loginOrgId);
                using (IDataReader dbRdr = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_SearchUsersById"))
                {
                    while (dbRdr.Read())
                    {
                        results.ActionStatus = dbRdr["ActionStatus"].ToString();
                        if (results.ActionStatus == "SUCCESS")
                        {
                            if (results.Users == null)
                            {
                                results.Users = new List<User>();
                            }
                            User user = new User();
                            user.UserId = Convert.ToInt32(dbRdr["UserId"].ToString());
                            user.UserOrgId = Convert.ToInt32(dbRdr["UserOrgId"].ToString());
                            user.UserName = dbRdr["UserName"].ToString();
                            user.Description = dbRdr["Description"].ToString();
                            user.FirstName = dbRdr["FirstName"].ToString();
                            user.LastName = dbRdr["LastName"].ToString();
                            user.Active = Convert.ToBoolean(dbRdr["Active"].ToString());
                            user.EmailId = dbRdr["UserEmailId"].ToString();
                            user.MobileNo = dbRdr["MobileNo"].ToString();
                            user.GroupId = Convert.ToInt32(dbRdr["GroupId"].ToString());
                            user.DepartmentIdsForUserCreattion = dbRdr["DepartmentId"].ToString();
                            user.CreatedDate = FormatSystemDateToScriptDate(Convert.ToDateTime(dbRdr["CreatedDate"].ToString()));
                            user.DomainUser = Convert.ToBoolean(dbRdr["Domain User"]);
                            user.DomainName = Convert.ToString(dbRdr["Domain"]);
                            results.Users.Add(user);
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
        }

        public Results ManageUser(User user, string action, string loginOrgId, string loginToken)
        {
            Results results = new Results();

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(17);
                dbManager.AddParameters(0, "@in_iCurrUserId", user.UserId);
                dbManager.AddParameters(1, "@in_iOrgId", user.UserOrgId);
                dbManager.AddParameters(2, "@in_vUserName", user.UserName);
                dbManager.AddParameters(3, "@in_vDescription", user.Description);
                dbManager.AddParameters(4, "@in_vUserEmailId", user.EmailId);
                dbManager.AddParameters(5, "@in_vMobileNo", user.MobileNo);
                dbManager.AddParameters(6, "@in_bActive", user.Active);
                dbManager.AddParameters(7, "@in_vFirstName", user.FirstName);
                dbManager.AddParameters(8, "@in_vLastName", user.LastName);
                dbManager.AddParameters(9, "@in_iGroupId", user.GroupId);
                dbManager.AddParameters(10, "@in_iDepartmentId", user.DepartmentId);
                dbManager.AddParameters(11, "@in_vDepartmentIds", user.DepartmentIdsForUserCreattion);
                dbManager.AddParameters(12, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(13, "@in_iLoginOrgId", loginOrgId);
                dbManager.AddParameters(14, "@in_vAction", action);
                dbManager.AddParameters(15, "@in_bDomainUser", user.DomainUser);
                dbManager.AddParameters(16, "@in_iDomainId", user.DomainId);

                using (IDataReader dbRdr = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_ManageUser"))
                {
                    if (dbRdr.Read())
                    {
                        results.ActionStatus = dbRdr["ActionStatus"].ToString();
                        results.IdentityId = dbRdr["IdentityId"].ToString();
                        results.UserPassData = dbRdr["UserPass"].ToString();
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
                dbManager.Dispose();
            }

            return results;
        }

        /* Remove once testing is completed
        public Results PasswordReset(UserBase user, string action, string loginOrgId, string loginToken)
        {
            Results results = new Results();

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(5);
                dbManager.AddParameters(0, "@in_iCurrUserId", user.UserId);
                dbManager.AddParameters(1, "@in_vUserName", user.UserName);
                dbManager.AddParameters(2, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(3, "@in_iLoginOrgId", loginOrgId);
                dbManager.AddParameters(4, "@in_vAction", action);

                using (IDataReader dbRdr = dbManager.ExecuteReader(CommandType.StoredProcedure, "USP_ManageUser"))
                {
                    if (dbRdr.Read())
                    {
                        results.ActionStatus = dbRdr["ActionStatus"].ToString();
                        results.IdentityId = dbRdr["IdentityId"].ToString();
                        results.UserPassData = dbRdr["UserPass"].ToString();
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
        }
        */

        public DataTable GetDomains(int orgid, string action, string loginOrgId, string loginToken, string OrgCode = "")
        {
            DataSet dsDomain = new DataSet();

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(5);               
                dbManager.AddParameters(0, "@in_iOrgID", orgid);
                dbManager.AddParameters(1, "@in_vOrgCode", OrgCode);
                dbManager.AddParameters(2, "@in_vAction", action);
                dbManager.AddParameters(3, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(4, "@in_iLoginOrgId", loginOrgId);

                dsDomain = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GetDomains");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }

            return dsDomain.Tables[0];
        }

        public DataSet ManageBulkUserUpload(int orgid, string action, string loginOrgId, string loginToken, string SoftData, int TempUserID, int startRowIndex, int endRowIndex)
        {
            DataSet dsTemplate = new DataSet();
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(9);

                dbManager.AddParameters(0, "@in_iOrgID", orgid);
                dbManager.AddParameters(1, "@in_xBatchData", SoftData);
                dbManager.AddParameters(2, "@in_iTempUserID", TempUserID);
                dbManager.AddParameters(3, "@in_vAction", action);
                dbManager.AddParameters(4, "@in_vSortExpression", string.Empty);
                dbManager.AddParameters(5, "@in_iStartRowIndex", startRowIndex);
                dbManager.AddParameters(6, "@in_iEndRowIndex", endRowIndex);
                dbManager.AddParameters(7, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(8, "@in_iLoginOrgId", loginOrgId);

                dsTemplate = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GetUserTemplateDetails");

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }

            return dsTemplate;
        }

        /* Remove once testing is completed
        public DataSet GetValuesForTemplate(int orgid, string action, string loginOrgId, string loginToken)
        {
            DataSet dsTemplate = new DataSet();
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(4);

                dbManager.AddParameters(0, "@in_iOrgID", orgid);
                dbManager.AddParameters(1, "@in_vAction", action);
                dbManager.AddParameters(2, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(3, "@in_iLoginOrgId", loginOrgId);

                dsTemplate = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GetUserTemplateDetails");

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }

            return dsTemplate;
        }

        public DataSet GetDataForGrid(int startRowIndex, int endRowIndex, string action, string loginOrgId, string loginToken)
        {
            DataSet dsTemplate = new DataSet();

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(5);

                dbManager.AddParameters(0, "@in_iStartRowIndex", startRowIndex);
                dbManager.AddParameters(1, "@in_iEndRowIndex", endRowIndex);
                dbManager.AddParameters(2, "@in_vAction", action);
                dbManager.AddParameters(3, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(4, "@in_iLoginOrgId", loginOrgId);

                dsTemplate = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GetUserTemplateDetails");

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }

            return dsTemplate;
        }
        */

        public bool ManageBulkUserUpload(int orgid, string action, string loginOrgId, string loginToken, string SoftData, int TempUserID)
        {
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(9);

                dbManager.AddParameters(0, "@in_iOrgID", orgid);
                dbManager.AddParameters(1, "@in_xBatchData", SoftData);
                dbManager.AddParameters(2, "@in_iTempUserID", TempUserID);
                dbManager.AddParameters(3, "@in_vAction", action);
                dbManager.AddParameters(4, "@in_vSortExpression", string.Empty);
                dbManager.AddParameters(5, "@in_iStartRowIndex", 0);
                dbManager.AddParameters(6, "@in_iEndRowIndex", 0);
                dbManager.AddParameters(7, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(8, "@in_iLoginOrgId", loginOrgId);

                dbManager.ExecuteNonQuery(CommandType.StoredProcedure, "USP_GetUserTemplateDetails");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }

            return true;
        }

        /* Remove once testing is completed
        public bool UploadSoftData(int orgid, string action, string loginOrgId, string loginToken, string SoftData)
        {
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(5);

                dbManager.AddParameters(0, "@in_iOrgID", orgid);
                dbManager.AddParameters(1, "@in_xBatchData", SoftData);
                dbManager.AddParameters(2, "@in_vAction", action);
                dbManager.AddParameters(3, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(4, "@in_iLoginOrgId", loginOrgId);

                dbManager.ExecuteNonQuery(CommandType.StoredProcedure, "USP_GetUserTemplateDetails");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }

            return true;
        }

        public bool DeleteTempUser(int orgid, string action, string loginOrgId, string loginToken, int TempUserID)
        {
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(5);

                dbManager.AddParameters(0, "@in_iOrgID", orgid);
                dbManager.AddParameters(1, "@in_iTempUserID", TempUserID);
                dbManager.AddParameters(2, "@in_vAction", action);
                dbManager.AddParameters(3, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(4, "@in_iLoginOrgId", loginOrgId);

                dbManager.ExecuteNonQuery(CommandType.StoredProcedure, "USP_GetUserTemplateDetails");

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }
            return true;
        }

        public bool DiscardTempUser(int orgid, string action, string loginOrgId, string loginToken)
        {
            SqlCommand dbCmd = null;
            SqlConnection dbCon = null;
            try
            {

                dbCon = OpenConnection();
                dbCmd = new SqlCommand();
                dbCmd.Connection = dbCon;
                dbCmd.CommandType = CommandType.StoredProcedure;
                dbCmd.CommandText = "USP_GetUserTemplateDetails";
                dbCmd.Parameters.Add("@in_iOrgID", SqlDbType.Int).Value = orgid;
                dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar, 30).Value = action;
                dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                dbCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;
                dbCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (dbCmd != null)
                {
                    dbCmd.Dispose();
                }
                CloseConnection(dbCon);

            }
            return true;
        }

        public bool CommitTempUser(int orgid, string action, string loginOrgId, string loginToken)
        {
            SqlCommand dbCmd = null;
            SqlConnection dbCon = null;
            try
            {

                dbCon = OpenConnection();
                dbCmd = new SqlCommand();
                dbCmd.Connection = dbCon;
                dbCmd.CommandType = CommandType.StoredProcedure;
                dbCmd.CommandText = "USP_GetUserTemplateDetails";
                dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar, 30).Value = action;
                dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                dbCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;
                dbCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (dbCmd != null)
                {
                    dbCmd.Dispose();
                }
                CloseConnection(dbCon);

            }
            return true;
        }
        */
        /* DMSENH6-4655 BS*/
        public Results TraceLoginLogout(int UserId, string Message, string AuditType)
        {
            Results results = new Results();

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(4);

                dbManager.AddParameters(0, "@UserId", UserId);
                dbManager.AddParameters(1, "@Auditmessage", Message);
                dbManager.AddParameters(2, "@AuditType", AuditType);
                dbManager.AddParameters(3, "@Auditid", UserId);


                dbManager.ExecuteNonQuery(CommandType.StoredProcedure, "USP_AuditTrack");

            }
            catch (Exception ex)
            {
                results.Message = ex.ToString();
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }

            return results;
        }
        /* DMSENH6-4655 BD*/
    }
}
