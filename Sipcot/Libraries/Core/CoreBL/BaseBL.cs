using Lotex.EnterpriseSolutions.CoreDAL;

namespace Lotex.EnterpriseSolutions.CoreBL
{
    public class BaseBL
    {
        public BaseBL() { }
        public bool GetLogoImageName(string loginOrgCode, ref string logoImageName, ref string loginOrgName)
        {
            SecurityDAL dal = new SecurityDAL();
            return dal.GetLogoImageName(loginOrgCode, ref  logoImageName, ref  loginOrgName);
        }
    }
}
