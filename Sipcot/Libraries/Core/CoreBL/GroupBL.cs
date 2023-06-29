using System;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreDAL;
using System.Data;

namespace Lotex.EnterpriseSolutions.CoreBL
{
    public class GroupBL
    {
        public GroupBL() { }

        public Results SearchGroups(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            Results results = null;
            GroupDAL dal = new GroupDAL();
            try
            {
                results = dal.SearchGroups(filter, action, loginOrgId, loginToken);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message =  CoreMessages.GetMessages(action, results.ActionStatus,ex.ToString());
            }
            return results;
        }
        public Results GetGroupRightsWithGroupId(int currentGroupId, string action, string loginOrgId, string loginToken)
        {
            Results results = null;
            GroupDAL dal = new GroupDAL();
            try
            {
                results = dal.GetGroupRightsWithGroupId(currentGroupId, action, loginOrgId, loginToken);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message =  CoreMessages.GetMessages(action, results.ActionStatus,ex.ToString());
            }
            return results;
        }
        public Results ManageGroups(Group group, string action, string loginOrgId, string loginToken)
        {
            Results results = null;
            GroupDAL dal = new GroupDAL();
            try
            {
                results = dal.ManageGroups(group, action, loginOrgId, loginToken);
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
        public Results GetAppPages(int loginParentOrgId, string CommaSeparatedApplicationIds)
        {
            Results results = null;
            GroupDAL dal = new GroupDAL();
            try
            {
                results = dal.GetAppPages(loginParentOrgId, CommaSeparatedApplicationIds);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message = CoreMessages.GetMessages(string.Empty, results.ActionStatus,ex.ToString());
            }
            return results;
        }
        public Results GetAppRights(string applicationId)
        {
            Results results = null;
            GroupDAL dal = new GroupDAL();
            try
            {
                results = dal.GetAppRights(applicationId);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message = CoreMessages.GetMessages(string.Empty, results.ActionStatus,ex.ToString());
            }
            return results;
        }
        public DataSet GetMenuItemDatasetByGroupId(int groupId, string pageName, string action, string loginOrgId, string loginToken)
        {
            return new GroupDAL().GetMenuItemDatasetByGroupId(groupId, pageName, action, loginOrgId, loginToken);           
        }

        // DMS5-3929M
        public Results GetApps(string action,int LoginOrgId, string LoginToken)
        {
            Results results = null;
            GroupDAL dal = new GroupDAL();
            try
            {
                results = dal.GetApps(action,LoginOrgId, LoginToken);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message = CoreMessages.GetMessages(string.Empty, results.ActionStatus,ex.ToString());
            }
            return results;
        }
    }
}
