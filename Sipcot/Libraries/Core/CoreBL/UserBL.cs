
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
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreDAL;

namespace Lotex.EnterpriseSolutions.CoreBL
{
    public class UserBL
    {
        public UserBL() { }
        public Results SearchUsers(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            return new UserDAL().SearchUsers(filter, action, loginOrgId, loginToken);
        }

        public Results PasswordReset(User user, string action, string loginOrgId, string loginToken)
        {
            Results results = new Results();
            try
            {
                results = new UserDAL().ManageUser(user, action, loginOrgId, loginToken);
                if (results.ActionStatus == "SUCCESS")
                {
                    results.Message = "Password has been reset successfully";
                }
                else
                {
                    results.Message = "Error! Unable to reset password";
                }
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message =  CoreMessages.GetMessages(action, results.ActionStatus,ex.ToString());
            }
            return results;
        }

        public Results ManageUser(User user, string action, string loginOrgId, string loginToken)
        {
            Results results = null;
            UserDAL dal = new UserDAL();
            try
            {
                results = dal.ManageUser(user, action, loginOrgId, loginToken);
                results.Message =  CoreMessages.GetMessages(action, results.ActionStatus);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message =  CoreMessages.GetMessages(action, results.ActionStatus,ex.ToString());
            }
            return results;
        }

        public System.Data.DataTable GetDomains(int orgid, string action, string loginOrgId, string loginToken,string OrgCode = "")
        {
            return new UserDAL().GetDomains(orgid, action, loginOrgId, loginToken, OrgCode);
        }

        public System.Data.DataSet GetValuesForTemplate(int orgid, string action, string loginOrgId, string loginToken)
        {
            return new UserDAL().ManageBulkUserUpload(orgid, action, loginOrgId, loginToken, string.Empty, 0, 0, 0);
        }

        /* DMSENH6-4655 BS*/
        public void TraceLoginLogout(int UserId, string Message, string AuditType)
        {
            new UserDAL().TraceLoginLogout(UserId, Message, AuditType);
        }
        /* DMSENH6-4655 BD*/

        public bool UploadSoftData(int orgid, string action, string loginOrgId, string loginToken, string SoftData)
        {
            return new UserDAL().ManageBulkUserUpload(orgid, action, loginOrgId, loginToken, SoftData, 0);
        }

        public System.Data.DataSet GetDataForGrid(int startRowIndex, int endRowIndex, string action, string loginOrgId, string loginToken)
        {
            return new UserDAL().ManageBulkUserUpload(0, action, loginOrgId, loginToken, "", 0, startRowIndex, endRowIndex);
        }

        public bool DeleteTempUser(int orgid, string action, string loginOrgId, string loginToken, int TempUserID)
        {
            return new UserDAL().ManageBulkUserUpload(orgid, action, loginOrgId, loginToken, "", TempUserID);
        }

        public bool DiscardTempUser(int orgid, string action, string loginOrgId, string loginToken)
        {
            return new UserDAL().ManageBulkUserUpload(orgid, action, loginOrgId, loginToken, "", 0);
        }

        public bool CommitTempUser(int orgid, string action, string loginOrgId, string loginToken)
        {
            return new UserDAL().ManageBulkUserUpload(orgid, action, loginOrgId, loginToken, "", 0);
        }

    }
}
