using System;
using Lotex.EnterpriseSolutions.CoreDAL;

namespace Lotex.EnterpriseSolutions.CoreBL
{
    public class AppBL
    {
        public AppBL() { }
        public string getApplicatonData(string keyword)
        {
            try
            {
                AppDAL dal = new AppDAL();
                return dal.getApplicatonData(keyword);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool setApplicatonData(string keyword, string value)
        {
            try
            {
                AppDAL dal = new AppDAL();
                return dal.setApplicatonData(keyword, value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
