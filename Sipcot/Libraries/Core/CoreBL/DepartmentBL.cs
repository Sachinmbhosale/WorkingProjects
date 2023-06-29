using System;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreDAL;

namespace Lotex.EnterpriseSolutions.CoreBL
{
    public class DepartmentBL
    {
        public DepartmentBL() { }

        public Results ManageDepartment(Department Entityobj, string action, string loginToken, int loginOrgId)
        {
            return new DepartmentDAL().ManageDepartment(Entityobj, action, loginToken, loginOrgId);
        }

        public Results SearchDepartments(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            Results results = null;
            DepartmentDAL dal = new DepartmentDAL();
            try
            {
                results = dal.SearchDepartments(filter, action, loginOrgId, loginToken);
            }
            catch (Exception ex)
            {
                results = new Results();
                results.ActionStatus = "ERROR";
                results.Message =  CoreMessages.GetMessages(action, results.ActionStatus,ex.ToString());
            }
            return results;
        }
    }
}
