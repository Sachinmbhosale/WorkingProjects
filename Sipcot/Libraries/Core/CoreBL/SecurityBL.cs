using System;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreDAL;

namespace Lotex.EnterpriseSolutions.CoreBL
{
    public class SecurityBL : BaseBL
    {

        /// <summary>
        /// This function is used to manage users including login
        /// </summary>
        /// <param name="loginUser"></param>
        /// <param name="action"></param>
        /// <returns></returns>               
        public Results LoggedUserManagement(ref User loginUser, string action)
        {
            SecurityDAL dal = new SecurityDAL();
            Results rs = new Results();

            try
            {
                rs = dal.LoggedUserManagement(ref loginUser, action);
                rs.Message = CoreMessages.GetMessages(action, rs.ActionStatus);

            }
            catch (Exception ex)
            {
                rs.ActionStatus = ActionStatus.ERROR.ToString();
                rs.Message = CoreMessages.GetMessages(action, rs.ActionStatus, ex.ToString());               
            }
            return rs;
        }
        public Results ValidateUserManagement(ref User loginUser, string action)
        {
            SecurityDAL dal = new SecurityDAL();
            Results rs = new Results();

            try
            {
                rs = dal.ValidateUserManagement(ref loginUser, action);
                rs.Message = CoreMessages.GetMessages(action, rs.ActionStatus);
            }
            catch (Exception ex)
            {
                rs.ActionStatus = ActionStatus.ERROR.ToString();
                rs.Message = CoreMessages.GetMessages(action, rs.ActionStatus, ex.ToString());
            }
            return rs;
        }
        public Results GetUsersByUserNameOrgId(string action, string userName, string orgName, int loginOrgId)
        {
            Results results = null;
            SecurityDAL dal = new SecurityDAL();
            try
            {
                results = dal.GetUsersByUserNameOrgId(action, userName, orgName, loginOrgId);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message =  CoreMessages.GetMessages(action, results.ActionStatus,ex.ToString());
            }
            return results;
        }
        public Results SearchOrgs(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            Results results = null;
            SecurityDAL dal = new SecurityDAL();
            try
            {
                results = dal.SearchOrgs(filter, action, loginOrgId, loginToken);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message =  CoreMessages.GetMessages(action, results.ActionStatus,ex.ToString());
            }
            return results;
        }
        public Results GetOrgById(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            Results results = null;
            SecurityDAL dal = new SecurityDAL();
            try
            {
                results = dal.GetOrgById(filter, action, loginOrgId, loginToken);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message =  CoreMessages.GetMessages(action, results.ActionStatus,ex.ToString());
            }
            return results;
        }
        public Results ManageOrgs(Org customer, string action, string loginOrgId, string loginToken)
        {
            Results results = null;
            SecurityDAL dal = new SecurityDAL();
            try
            {
                results = dal.ManageOrgs(customer, action, loginOrgId, loginToken);
                results.Message =  CoreMessages.GetMessages(action, results.ActionStatus);
                //if (action == "AddOrg" && results.ActionStatus == "SUCCESS" && results.IdentityId != string.Empty)
                //{
                //    UserBL bl = new UserBL();
                //    bl.CreateAdminUserForOrgAndSentEmail(ref results, customer, loginOrgId, loginToken);

                //}
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message =  CoreMessages.GetMessages(action, results.ActionStatus,ex.ToString());
            }
            return results;
        }

       public Results GetApplication(string action, string loginOrgId, string loginToken)
        {
            Results results = null;
            SecurityDAL dal = new SecurityDAL();
            try
            {
                results = dal.GetApplication(action, loginOrgId, loginToken);              
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message = CoreMessages.GetMessages(action, results.ActionStatus, ex.ToString());
            }
            return results;
        }

    }
}