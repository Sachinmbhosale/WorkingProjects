using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreDAL;

namespace Lotex.EnterpriseSolutions.CoreBL
{
    public class PriceBL
    {
        public PriceBL() { }

        public Results ManagePrice(Price objPrice, string action, string loginToken, int loginOrgId)
        {
            return new PriceDAL().ManagePrice(objPrice, action, loginToken, loginOrgId);
        }
    }
}
