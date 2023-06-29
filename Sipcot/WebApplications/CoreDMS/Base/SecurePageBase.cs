using System;
using Lotex.EnterpriseSolutions.CoreBE;

namespace Lotex.EnterpriseSolutions.WebUI
{
    public class SecurePageBase : PageBase
    {
        public SecurePageBase() { }
        protected void Page_Load(object sender, EventArgs e)
        {
            CheckAuthentication();
        }
        public BusinessTypes GetBusinessType(string strBusinessType)
        {
            switch (strBusinessType.ToLower())
            {
                case "organization":
                    return BusinessTypes.Organization;
                //case "user":
                //    return BusinessTypes.User;
                //case "usergroup":
                //    return BusinessTypes.UserGroup;
                case "location":
                    return BusinessTypes.Location;
                case "department":
                    return BusinessTypes.Department;
                case "documenttype":
                    return BusinessTypes.DocumentType;
                case "designation":
                    return BusinessTypes.Designation;
                default:
                    return BusinessTypes.Default;
            }
        }
        protected string GetStartDate(object CreatedDate)
        {
            if (Convert.ToDateTime(CreatedDate) != new DateTime(0001, 1, 1))
                return CreatedDate.ToString();
            else
                return "";
        }
    }
}
